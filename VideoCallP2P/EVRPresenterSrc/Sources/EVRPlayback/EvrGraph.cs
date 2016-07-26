using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using SlimDX.Direct3D9;
using DirectShow;
using Sonic;
using SlimDX;

namespace EVR
{
    #region Playback Graph

    public delegate void SurfaceReadyHandler(ref Surface _surface);

    public class DSFilePlaybackEVR : DSFilePlayback
        , IMFVideoDeviceID
        , IMFVideoPresenter
        , IMFGetService
        , IMFTopologyServiceLookupClient
    {
        #region Enums

        public enum RENDER_STATE
        {
            RENDER_STATE_STARTED = 1,
            RENDER_STATE_STOPPED,
            RENDER_STATE_PAUSED,
            RENDER_STATE_SHUTDOWN,  // Initial state. 
        }

        #endregion

        #region Constants

        private const long PRESENTATION_CURRENT_POSITION = 0x7fffffffffffffff;
        private static readonly MFRatio g_DefaultFrameRate = new MFRatio(30, 1);
        private const int PRESENTER_BUFFER_COUNT = 3;

        #endregion

        #region Classes

        protected class MFSample : IDisposable
        {
            #region Variables

            public Surface Target = null;
            public IMFSample Sample = null;
            public ManualResetEvent Free = new ManualResetEvent(true);
            public long PresentationTime = -1;
            public long Duration = -1;

            #endregion

            #region Constructor

            ~MFSample()
            {
                Dispose();
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                Sample = null;
                if (Target != null)
                {
                    Target.Dispose();
                    Target = null;
                }
            }

            #endregion
        }

        protected class SamplePool : MFHelper, IDisposable
        {
            #region Variables

            protected List<MFSample> m_Samples = new List<MFSample>();
            protected EventWaitHandle m_hCancel = null;
            protected object m_csLock = new object();

            #endregion

            #region Constructor

            public SamplePool(EventWaitHandle hCancel)
            {
                m_hCancel = hCancel;
            }

            ~SamplePool()
            {
                Dispose();
            }

            #endregion

            #region Methods

            public int AddSample(MFSample _sample)
            {
                if (_sample != null && m_Samples.IndexOf(_sample) == -1)
                {
                    m_Samples.Add(_sample);
                }
                return m_Samples.Count;
            }

            public bool GetBuffer(out MFSample _sample, bool bLock)
            {
                _sample = null;
                int nIndex = 0;
                WaitHandle[] _handles = new WaitHandle[m_Samples.Count + (m_hCancel == null ? 0 : 1)];
                if (m_hCancel != null) _handles[nIndex++] = m_hCancel;
                foreach (MFSample sample in m_Samples) _handles[nIndex++] = sample.Free;
                lock (m_csLock)
                {
                    nIndex = WaitHandle.WaitAny(_handles, (bLock ? Timeout.Infinite : 0));
                    if ((nIndex == 0 && m_hCancel != null) || nIndex == WaitHandle.WaitTimeout)
                    {
                        return false;
                    }
                    _sample = m_Samples[(m_hCancel != null ? nIndex - 1 : nIndex)];
                    _sample.Free.Reset();
                }
                return (_sample != null);
            }

            public int ReleaseBuffer(MFSample _sample)
            {
                _sample.Free.Set();
                int nCount = 0;
                foreach (MFSample sample in m_Samples)
                {
                    if (sample.Free.WaitOne(0)) nCount++;
                }
                return nCount;
            }

            public void Clear()
            {
                //if (m_hCancel != null) m_hCancel.Set();
                lock (m_csLock)
                {
                    while (m_Samples.Count > 0)
                    {
                        MFSample _sample = m_Samples[0];
                        _sample.Free.Set();
                        m_Samples.Remove(_sample);
                        _sample.Dispose();
                    }
                }
            }

            public bool AreSamplesPending()
            {
                WaitHandle[] _handles = new WaitHandle[m_Samples.Count];
                for (int i = 0; i < m_Samples.Count; i++) _handles[i] = m_Samples[i].Free;
                return !WaitHandle.WaitAll(_handles, 0);
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                Clear();
            }

            #endregion
        }

        protected class Scheduler : MFHelper, IDisposable
        {
            #region Enum

            private enum ScheduleEvent : int
            {
                eTerminate = 0x0400,
                eSchedule = eTerminate + 1,
                eFlush = eSchedule + 1
            };

            #endregion

            #region Constants

            private const int SCHEDULER_TIMEOUT = 5000;

            #endregion

            #region Variables

            private object m_lock = new object();

            private Queue<MFSample> m_ScheduledSamples = new Queue<MFSample>(); // Samples waiting to be presented.
            private IMFClock m_pClock = null;  // Presentation clock. Can be NULL.
            private DSFilePlaybackEVR m_pCB = null;     // Weak reference; do not delete.

            private Thread m_SchedulerThread = null;
            private ManualResetEvent m_hSchedulerQuitEvent = new ManualResetEvent(false);
            private AutoResetEvent m_hThreadReadyEvent = null;
            private AutoResetEvent m_hFlushEvent = null;

            private ManualResetEvent m_hNotifyEvent = new ManualResetEvent(false);
            private object m_EventsLock = new object();
            private Queue<ScheduleEvent> m_Events = new Queue<ScheduleEvent>();

            private float m_fRate = 1.0f;                 // Playback rate.
            private long m_PerFrameInterval = 0;     // Duration of each frame.
            private long m_PerFrame_1_4th = 0;      // 1/4th of the frame duration.
            private long m_LastSampleTime = 0;       // Most recent sample time.

            #endregion

            #region Constructor

            public Scheduler()
            {
            }

            ~Scheduler()
            {
                Dispose();
            }

            #endregion

            #region Properties

            public long LastSampleTime { get { return m_LastSampleTime; } }
            public long FrameDuration { get { return m_PerFrameInterval; } }

            #endregion

            #region Public Methods

            public void SetCallback(DSFilePlaybackEVR pCB)
            {
                m_pCB = pCB;
            }

            public void SetFrameRate(MFRatio fps)
            {
                long AvgTimePerFrame = 0;

                // Convert to a duration.
                MFFrameRateToAverageTimePerFrame(fps.Numerator, fps.Denominator, out AvgTimePerFrame);

                m_PerFrameInterval = AvgTimePerFrame;

                // Calculate 1/4th of this value, because we use it frequently.
                m_PerFrame_1_4th = m_PerFrameInterval / 4;
            }

            public void SetClockRate(float fRate)
            {
                m_fRate = fRate;
            }

            public HRESULT StartScheduler(IMFClock pClock)
            {
                if (m_SchedulerThread != null)
                {
                    return E_UNEXPECTED;
                }

                m_hSchedulerQuitEvent.Reset();
                m_pClock = pClock;

                // Set a high the timer resolution (ie, short timer period).
                timeBeginPeriod(1);

                // Create an event to wait for the thread to start.
                m_hThreadReadyEvent = new AutoResetEvent(false);

                // Create an event to wait for flush commands to complete.
                m_hFlushEvent = new AutoResetEvent(false);

                m_SchedulerThread = new Thread(new ParameterizedThreadStart(SchedulerThreadProc));
                m_SchedulerThread.Start(this);

                // Wait for the thread to signal the "thread ready" event.
                int dwWait = WaitHandle.WaitAny(new WaitHandle[] { m_hThreadReadyEvent, m_hSchedulerQuitEvent });

                // The thread terminated early for some reason. This is an error condition.
                if (0 != dwWait)
                {
                    m_SchedulerThread.Join();
                    m_SchedulerThread = null;
                    m_hThreadReadyEvent = null;
                    m_hFlushEvent = null;
                    return E_UNEXPECTED;
                }

                return S_OK;
            }

            public HRESULT StopScheduler()
            {
                if (m_SchedulerThread == null)
                {
                    return S_OK;
                }

                // Ask the scheduler thread to exit.
                AppendEvent(ScheduleEvent.eTerminate);

                m_hSchedulerQuitEvent.Set();

                m_SchedulerThread.Join();

                m_pClock = null;
                m_SchedulerThread = null;
                m_hSchedulerQuitEvent = null;
                m_hThreadReadyEvent = null;
                m_hFlushEvent = null;

                lock (m_lock)
                {
                    while (m_ScheduledSamples.Count > 0)
                    {
                        MFSample _sample = m_ScheduledSamples.Dequeue();
                        _sample.Free.Set();
                    }
                }

                ScheduleEvent _event;
                while (GetEvent(out _event)) { }
                // Restore the timer resolution.
                timeEndPeriod(1);

                return S_OK;
            }

            public HRESULT ScheduleSample(MFSample pSample, bool bPresentNow)
            {
                if (m_pCB == null)
                {
                    return MF_E_NOT_INITIALIZED;
                }

                if (m_SchedulerThread == null)
                {
                    return MF_E_NOT_INITIALIZED;
                }

                if (!m_SchedulerThread.IsAlive)
                {
                    return E_FAIL;
                }

                if (bPresentNow || (m_pClock == null))
                {
                    // Present the sample immediately.
                    m_pCB.PresentSample(pSample, 0);
                }
                else
                {
                    lock (m_lock)
                    {
                        // Queue the sample and ask the scheduler thread to wake up.
                        m_ScheduledSamples.Enqueue(pSample);
                    }
                    AppendEvent(ScheduleEvent.eSchedule);
                }

                return S_OK;
            }

            public HRESULT Flush()
            {
                TRACE_ENTER();

                if (m_SchedulerThread != null && m_SchedulerThread.IsAlive)
                {
                    // Ask the scheduler thread to flush.
                    AppendEvent(ScheduleEvent.eFlush);

                    // Wait for the scheduler thread to signal the flush event,
                    // OR for the thread to terminate.
                    WaitHandle.WaitAny(new WaitHandle[] { m_hFlushEvent, m_hSchedulerQuitEvent }, SCHEDULER_TIMEOUT);

                    TRACE("Scheduler::Flush completed.");
                }

                return S_OK;
            }

            #endregion

            #region Private Methods

            private HRESULT ProcessSamplesInQueue(out long plNextSleep)
            {
                long lWait = 0;
                HRESULT hr = S_OK;

                // Process samples until the queue is empty or until the wait time > 0.

                // Note: Dequeue returns S_FALSE when the queue is empty.
                while (true)
                {
                    hr = S_FALSE;
                    MFSample pSample = null;

                    lock (m_lock)
                    {
                        if (m_ScheduledSamples.Count > 0)
                        {
                            pSample = m_ScheduledSamples.Peek();
                        }
                    }
                    if (pSample == null)
                    {
                        break;
                    }
                    // Process the next sample in the queue. If the sample is not ready
                    // for presentation. the value returned in lWait is > 0, which
                    // means the scheduler should sleep for that amount of time.

                    hr = ProcessSample(pSample, out lWait);
                    pSample = null;

                    if (hr.Failed)
                    {
                        break;
                    }
                    if (lWait > 0)
                    {
                        break;
                    }
                }

                // If the wait time is zero, it means we stopped because the queue is
                // empty (or an error occurred). Set the wait time to infinite; this will
                // make the scheduler thread sleep until it gets another thread message.
                if (lWait == 0)
                {
                    lWait = Timeout.Infinite;
                }
                plNextSleep = lWait;
                return S_OK;
            }

            private HRESULT ProcessSample(MFSample pSample, out long plNextSleep)
            {
                HRESULT hr = S_OK;
                bool bPresentNow = true;
                plNextSleep = 0;

                long hnsPresentationTime = 0;
                long hnsTimeNow = 0;
                long hnsSystemTime = 0;
                if (m_pClock != null)
                {
                    // Get the sample's time stamp. It is valid for a sample to
                    // have no time stamp.
                    hr = (HRESULT)pSample.Sample.GetSampleTime(out hnsPresentationTime);

                    // Get the clock time. (But if the sample does not have a time stamp, 
                    // we don't need the clock time.)
                    if (hr.Succeeded)
                    {
                        hr = (HRESULT)m_pClock.GetCorrelatedTime(0, out hnsTimeNow, out hnsSystemTime);
                    }

                    // Calculate the time until the sample's presentation time. 
                    // A negative value means the sample is late.
                    long hnsDelta = hnsPresentationTime - hnsTimeNow;
                    if (m_fRate < 0)
                    {
                        // For reverse playback, the clock runs backward. Therefore the delta is reversed.
                        hnsDelta = -hnsDelta;
                    }

                    if (hnsDelta < -m_PerFrame_1_4th)
                    {
                        // This sample is late. 
                        bPresentNow = true;
                    }
                    else if (hnsDelta > (3 * m_PerFrame_1_4th))
                    {
                        // This sample is still too early. Go to sleep.
                        plNextSleep = MFTimeToMsec(hnsDelta - (3 * m_PerFrame_1_4th));

                        // Adjust the sleep time for the clock rate. (The presentation clock runs
                        // at m_fRate, but sleeping uses the system clock.)
                        plNextSleep = (long)(plNextSleep / Math.Abs(m_fRate));

                        // Don't present yet.
                        bPresentNow = false;
                    }
                }

                if (bPresentNow)
                {
                    lock (m_lock)
                    {
                        if (m_ScheduledSamples.Count > 0)
                        {
                            m_ScheduledSamples.Dequeue();
                        }
                    }
                    hr = m_pCB.PresentSample(pSample, hnsPresentationTime);
                }

                return hr;
            }

            private void AppendEvent(ScheduleEvent _event)
            {
                lock (m_EventsLock)
                {
                    m_Events.Enqueue(_event);
                    m_hNotifyEvent.Set();
                }
            }

            private bool GetEvent(out ScheduleEvent _event)
            {
                _event = ScheduleEvent.eTerminate;
                lock (m_EventsLock)
                {
                    int nCount = m_Events.Count;
                    if (nCount == 0) return false;
                    _event = m_Events.Dequeue();
                    if (nCount == 1) m_hNotifyEvent.Reset();
                }
                return true;
            }

            private void SchedulerThreadProc(object lpParameter)
            {
                Scheduler _scheduler = (Scheduler)lpParameter;
                IntPtr _ptr = IntPtr.Zero;
                HRESULT hr = S_OK;
                long lWait = Timeout.Infinite;
                bool bExitThread = false;

                try
                {
                    // Signal to the scheduler that the thread is ready.
                    _scheduler.m_hThreadReadyEvent.Set();

                    while (!bExitThread)
                    {
                        int nResult = WaitHandle.WaitAny(new WaitHandle[] { m_hNotifyEvent, m_hSchedulerQuitEvent }, (int)lWait);

                        if (nResult == WaitHandle.WaitTimeout)
                        {
                            hr = ProcessSamplesInQueue(out lWait);
                            if (FAILED(hr))
                            {
                                bExitThread = true;
                            }
                        }
                        if (nResult == 0)
                        {
                            ScheduleEvent _event;
                            while (GetEvent(out _event))
                            {
                                bool bProcessSamples = !_scheduler.m_hSchedulerQuitEvent.WaitOne(0);
                                switch (_event)
                                {
                                    case ScheduleEvent.eTerminate:
                                        TRACE("eTerminate");
                                        bExitThread = true;
                                        break;

                                    case ScheduleEvent.eFlush:
                                        TRACE("eFlush");
                                        // Flushing: Clear the sample queue and set the event.
                                        lock (m_lock)
                                        {
                                            while (_scheduler.m_ScheduledSamples.Count > 0)
                                            {
                                                MFSample _sample = _scheduler.m_ScheduledSamples.Dequeue();
                                                _sample.Free.Set();
                                            }
                                        }
                                        _scheduler.m_hFlushEvent.Set();
                                        break;

                                    case ScheduleEvent.eSchedule:
                                        // Process as many samples as we can.
                                        if (bProcessSamples)
                                        {
                                            hr = _scheduler.ProcessSamplesInQueue(out lWait);
                                            if (hr.Failed)
                                            {
                                                bExitThread = true;
                                            }
                                            bProcessSamples = (lWait != Timeout.Infinite); 
                                        }
                                        break;
                                } // switch  
                            }
                        } // while (!bExitThread)
                    }
                }
                catch (Exception _exception)
                {
                    if (_exception is COMException)
                    {
                        hr = (HRESULT)(_exception as COMException).ErrorCode;
                    }
                    else
                    {
                        hr = E_FAIL;
                    }
                }
                finally
                {
                    _scheduler.m_hSchedulerQuitEvent.Set();
                }
                hr.Assert();
                TRACE("Schedule Thread Quit");
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                StopScheduler();
            }

            #endregion

            #region API

            [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
            private static extern uint timeBeginPeriod(uint uMilliseconds);

            [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
            public static extern uint timeEndPeriod(uint uMilliseconds);

            #endregion
        }

        private class Wrapper:IDisposable
        {
            #region Enum

            public enum CallType
            {
                Unknown,
                ProcessMessage,
                InitServicePointers,
                ReleaseServicePointers,
                GetCurrentMediaType,
                OnClockStart,
                OnClockRestart,
                OnClockStop,
                OnClockPause,
                OnClockSetRate,
            }

            #endregion

            #region Classes

            public class CCallbackHandler
            {
                public bool m_bAsync = false;
                public CallType m_Type = CallType.Unknown;
                public EventWaitHandle m_Notify = new ManualResetEvent(false);
                public int m_Result = S_OK;

                private Wrapper m_Invoker = null;

                #region Constructor

                public CCallbackHandler(Wrapper _Invoker)
                {
                    m_Invoker = _Invoker;
                }

                #endregion

                #region Methods

                public void Invoke()
                {
                    m_Invoker.InvokeThread(this);
                    WaitHandle.WaitAny(new WaitHandle[] { this.m_Notify, m_Invoker.m_Quit });

                }

                #endregion
            }

            public class ProcessMessageHandler : CCallbackHandler
            {
                public MFVPMessageType eMessage;
                public IntPtr ulParam;
                public ProcessMessageHandler(Wrapper _Invoker, MFVPMessageType _eMessage, IntPtr _ulParam)
                    : base(_Invoker)
                {
                    eMessage = _eMessage;
                    ulParam = _ulParam;
                    m_Type = CallType.ProcessMessage;
                }
            }

            public class InitServicePointersHandler : CCallbackHandler
            {
                public IntPtr pLookup;
                public InitServicePointersHandler(Wrapper _Invoker, IntPtr _pLookup)
                    : base(_Invoker)
                {
                    pLookup = _pLookup;
                    m_Type = CallType.InitServicePointers;
                }
            }

            public class ReleaseServicePointersHandler : CCallbackHandler
            {
                public ReleaseServicePointersHandler(Wrapper _Invoker)
                    : base(_Invoker)
                {
                    m_Type = CallType.ReleaseServicePointers;
                }
            }

            public class GetCurrentMediaTypeHandler : CCallbackHandler
            {
                public IMFVideoMediaType ppMediaType = null;
                public GetCurrentMediaTypeHandler(Wrapper _Invoker)
                    : base(_Invoker)
                {
                    m_Type = CallType.GetCurrentMediaType;
                }
            }

            public class OnClockStartHandler : CCallbackHandler
            {
                public long hnsSystemTime = 0;
                public long llClockStartOffset = 0;

                public OnClockStartHandler(Wrapper _Invoker, long _hnsSystemTime, long _llClockStartOffset)
                    : base(_Invoker)
                {
                    hnsSystemTime = _hnsSystemTime;
                    llClockStartOffset = _llClockStartOffset;
                    m_Type = CallType.OnClockStart;
                }
            }

            public class OnClockRestartHandler : CCallbackHandler
            {
                public long hnsSystemTime = 0;

                public OnClockRestartHandler(Wrapper _Invoker, long _hnsSystemTime)
                    : base(_Invoker)
                {
                    hnsSystemTime = _hnsSystemTime;
                    m_Type = CallType.OnClockRestart;
                }
            }

            public class OnClockStopHandler : CCallbackHandler
            {
                public long hnsSystemTime = 0;

                public OnClockStopHandler(Wrapper _Invoker, long _hnsSystemTime)
                    : base(_Invoker)
                {
                    hnsSystemTime = _hnsSystemTime;
                    m_Type = CallType.OnClockStop;
                }
            }

            public class OnClockPauseHandler : CCallbackHandler
            {
                public long hnsSystemTime = 0;

                public OnClockPauseHandler(Wrapper _Invoker, long _hnsSystemTime)
                    : base(_Invoker)
                {
                    hnsSystemTime = _hnsSystemTime;
                    m_Type = CallType.OnClockPause;
                }
            }

            public class OnClockSetRateHandler : CCallbackHandler
            {
                public long hnsSystemTime = 0;
                public float flRate = 0;

                public OnClockSetRateHandler(Wrapper _Invoker, long _hnsSystemTime, float _flRate)
                    : base(_Invoker)
                {
                    hnsSystemTime = _hnsSystemTime;
                    flRate = _flRate;
                    m_Type = CallType.OnClockSetRate;
                }
            }

            #endregion

            #region Variables

            protected DSFilePlaybackEVR m_Playback = null;
            protected Thread m_ExecutionThread = null;
            protected AutoResetEvent m_Notify = new AutoResetEvent(false);
            protected ManualResetEvent m_Quit = null;
            protected AutoResetEvent m_Ready = new AutoResetEvent(false);
            protected object m_LockThread = new object();
            private object m_Parameter = null;

            #endregion

            #region Constructor

            public Wrapper(DSFilePlaybackEVR _playback)
            {
                m_Playback = _playback;
                m_Quit = _playback.m_evStop;

                m_ExecutionThread = new Thread(new ParameterizedThreadStart(ThreadProc));
                m_ExecutionThread.SetApartmentState(ApartmentState.MTA);
                m_ExecutionThread.Priority = ThreadPriority.AboveNormal;
                m_ExecutionThread.Start();
            }

            ~Wrapper()
            {
                Dispose();
            }

            #endregion

            #region Methods

            private void InvokeThread(object _param)
            {
                lock (m_LockThread)
                {
                    m_Parameter = _param;
                    m_Notify.Set();
                }
                WaitHandle.WaitAny(new WaitHandle[] { m_Quit, m_Ready });
            }

            private void ThreadProc(object _state)
            {
                while (true)
                {
                    int nWait = WaitHandle.WaitAny(new WaitHandle[] { m_Quit, m_Notify });
                    if (nWait == 1)
                    {
                        object _param;
                        lock (m_LockThread)
                        {
                            _param = m_Parameter;
                        }
                        m_Ready.Set();
                        AsyncInvokerProc(_param);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            private void AsyncInvokerProc(object _state)
            {
                CCallbackHandler _handler = (CCallbackHandler)_state;
                switch (_handler.m_Type)
                {
                    case CallType.ProcessMessage:
                        {
                            ProcessMessageHandler _process = (ProcessMessageHandler)_handler;
                            _process.m_Result = m_Playback.ProcessMessageImpl(_process.eMessage, _process.ulParam);
                        }
                        break;
                    case CallType.InitServicePointers:
                        {
                            InitServicePointersHandler _service = (InitServicePointersHandler)_handler;
                            _handler.m_Result = m_Playback.InitServicePointersImpl(_service.pLookup);
                        }
                        break;
                    case CallType.ReleaseServicePointers:
                        {
                            ReleaseServicePointersHandler _service = (ReleaseServicePointersHandler)_handler;
                            _handler.m_Result = m_Playback.ReleaseServicePointersImpl();
                        }
                        break;
                    case CallType.GetCurrentMediaType:
                        {
                            GetCurrentMediaTypeHandler _service = (GetCurrentMediaTypeHandler)_handler;
                            _handler.m_Result = m_Playback.GetCurrentMediaTypeImpl(out _service.ppMediaType);
                        }
                        break;

                    case CallType.OnClockPause:
                        {
                            OnClockPauseHandler _service = (OnClockPauseHandler)_handler;
                            _handler.m_Result = m_Playback.OnClockPauseImpl(_service.hnsSystemTime);
                        }
                        break;
                    case CallType.OnClockRestart:
                        {
                            OnClockRestartHandler _service = (OnClockRestartHandler)_handler;
                            _handler.m_Result = m_Playback.OnClockRestartImpl(_service.hnsSystemTime);
                        }
                        break;
                    case CallType.OnClockSetRate:
                        {
                            OnClockSetRateHandler _service = (OnClockSetRateHandler)_handler;
                            _handler.m_Result = m_Playback.OnClockSetRateImpl(_service.hnsSystemTime, _service.flRate);
                        }
                        break;
                    case CallType.OnClockStart:
                        {
                            OnClockStartHandler _service = (OnClockStartHandler)_handler;
                            _handler.m_Result = m_Playback.OnClockStartImpl(_service.hnsSystemTime, _service.llClockStartOffset);
                        }
                        break;
                    case CallType.OnClockStop:
                        {
                            OnClockStopHandler _service = (OnClockStopHandler)_handler;
                            _handler.m_Result = m_Playback.OnClockStopImpl(_service.hnsSystemTime);
                        }
                        break;
                }
                _handler.m_Notify.Set();
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                if (m_ExecutionThread != null)
                {
                    m_ExecutionThread.Join();
                    m_ExecutionThread = null;
                }
            }

            #endregion
        }

        #endregion

        #region Variables

        protected EVRRenderer m_Renderer = null;
        protected Device m_Device = null;

        protected uint m_DeviceResetToken = 0;
        protected IDirect3DDeviceManager9 m_DeviceManager = null;

        protected object m_ObjectLock = new object();
        protected bool m_bSampleNotify = false;
        protected bool m_bRepaint = false;
        protected bool m_bPrerolled = false;
        protected bool m_bEndStreaming = false;
        protected MFVideoNormalizedRect m_nrcSource = new MFVideoNormalizedRect();
        protected float m_fRate = 1.0f;

        protected IntPtr m_pMixer = IntPtr.Zero;
        protected IMFClock m_pClock = null;                     // The EVR's clock.
        protected IMediaEventSink m_pMediaEventSink = null;      // The EVR's event-sink interface.

        protected IMFMediaType m_pMediaType = null;             // Output media type
        protected RENDER_STATE m_RenderState = RENDER_STATE.RENDER_STATE_SHUTDOWN;

        protected ManualResetEvent m_evStop = new ManualResetEvent(true);
        protected SamplePool m_SamplePool = null;
        protected Scheduler m_scheduler = new Scheduler();
        private Wrapper m_Caller = null;

        protected bool m_bUseSwapChain = false;

        #endregion

        #region Properties

        public Device Direct3DDevice
        {
            get { return m_Device; }
            set { m_Device = value; }
        }

        #endregion

        #region Constructor

        public DSFilePlaybackEVR(Device _device)
        {
            m_Device = _device;
            m_SamplePool = new SamplePool(m_evStop);
            m_scheduler.SetCallback(this);
        }

        #endregion

        #region Events

        public event SurfaceReadyHandler OnSurfaceReady;

        #endregion

        #region Overridden Methods

        protected override HRESULT OnInitInterfaces()
        {
            m_evStop.Reset();
            HRESULT hr;
            {
                hr = (HRESULT)MFHelper.DXVA2CreateDirect3DDeviceManager9(out m_DeviceResetToken, out m_DeviceManager);
                hr.Assert();
                if (hr.Succeeded)
                {
                    hr = (HRESULT)m_DeviceManager.ResetDevice(Marshal.GetObjectForIUnknown(m_Device.ComPointer), m_DeviceResetToken);
                    hr.Assert();
                }
                m_Caller = new Wrapper(this);
            }
            m_Renderer = new EVRRenderer();
            bool bAllocatorUsed = false;
            IMFVideoRenderer _renderer = (IMFVideoRenderer)m_Renderer.QueryInterface(typeof(IMFVideoRenderer));
            hr = (HRESULT)_renderer.InitializeRenderer(null, (IMFVideoPresenter)this);
            hr.Assert();
            bAllocatorUsed = hr.Succeeded;
            m_Renderer.FilterGraph = m_GraphBuilder;
            hr = base.OnInitInterfaces();
            if (!bAllocatorUsed && m_VideoControl != null)
            {
                IMFGetService _service = (IMFGetService)m_Renderer.QueryInterface(typeof(IMFGetService));
                IntPtr _object;
                hr = (HRESULT)_service.GetService(MFServices.MR_VIDEO_RENDER_SERVICE, typeof(IMFVideoDisplayControl).GUID, out _object);
                hr.Assert();
                IMFVideoDisplayControl _display = (IMFVideoDisplayControl)Marshal.GetObjectForIUnknown(_object);
                hr = (HRESULT)_display.SetVideoWindow(m_VideoControl.Handle);
                hr.Assert();
                DsRect _rect = new DsRect(m_VideoControl.ClientRectangle);
                hr = (HRESULT)_display.SetVideoPosition(new MFVideoNormalizedRect(), _rect);
                hr.Assert();
            }
            return hr;
        }

        protected override HRESULT OnCloseInterfaces()
        {
            m_evStop.Set();
            if (m_Renderer)
            {
                m_Renderer.Dispose();
                m_Renderer = null;
            }
            HRESULT hr = base.OnCloseInterfaces();
            if (m_Caller != null) m_Caller.Dispose();
            if (m_scheduler != null)
            {
                m_scheduler.StopScheduler();
            }
            if (m_SamplePool != null)
            {
                m_SamplePool.Clear();
            }
            return hr;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (m_DeviceManager != null)
            {
                Marshal.ReleaseComObject(m_DeviceManager);
                m_DeviceManager = null;
            }
            m_pClock = null;
            if (m_pMixer != IntPtr.Zero)
            {
                Marshal.Release(m_pMixer);
                m_pMixer = IntPtr.Zero;
            }
            m_pMediaEventSink = null;
            m_pMediaType = null;
        }

        #endregion

        #region IMFVideoDeviceID Members

        public int GetDeviceID(out Guid pDeviceID)
        {
            pDeviceID = Device.NativeInterface;
            return S_OK;
        }

        #endregion

        #region IMFGetService Members

        public int GetService(Guid guidService, Guid riid, out IntPtr ppvObject)
        {
            ppvObject = IntPtr.Zero;
            HRESULT hr = MFHelper.MF_E_UNSUPPORTED_SERVICE;
            if (guidService != MFServices.MR_VIDEO_RENDER_SERVICE)
            {
                return hr;
            }

            if (riid == typeof(IDirect3DDeviceManager9).GUID)
            {
                if (m_DeviceManager != null)
                {
                    try
                    {
                        ppvObject = Marshal.GetComInterfaceForObject(m_DeviceManager, typeof(IDirect3DDeviceManager9));
                        hr = NOERROR;
                    }
                    catch
                    {
                    }
                }
            }
            if (hr.Failed)
            {
                // Next, QI to check if this object supports the interface.
                IntPtr _ptr = Marshal.GetIUnknownForObject(this);
                try
                {
                    hr = (HRESULT)Marshal.QueryInterface(_ptr, ref riid, out ppvObject);
                }
                catch
                {
                    hr = E_NOINTERFACE;
                }
                finally
                {
                    Marshal.Release(_ptr);
                }
            }
            return hr;
        }

        #endregion

        #region IMFClockStateSink Members

        public int OnClockStart(long hnsSystemTime, long llClockStartOffset)
        {
            Wrapper.CCallbackHandler _handler =
                new Wrapper.OnClockStartHandler(m_Caller, hnsSystemTime, llClockStartOffset);
            _handler.Invoke();
            return _handler.m_Result;
        }

        public int OnClockRestart(long hnsSystemTime)
        {
            Wrapper.CCallbackHandler _handler =
                new Wrapper.OnClockRestartHandler(m_Caller, hnsSystemTime);
            _handler.Invoke();
            return _handler.m_Result;
        }

        public int OnClockStop(long hnsSystemTime)
        {
            Wrapper.CCallbackHandler _handler =
                new Wrapper.OnClockStopHandler(m_Caller, hnsSystemTime);
            _handler.Invoke();
            return _handler.m_Result;
        }

        public int OnClockPause(long hnsSystemTime)
        {
            Wrapper.CCallbackHandler _handler =
                new Wrapper.OnClockPauseHandler(m_Caller, hnsSystemTime);
            _handler.Invoke();
            return _handler.m_Result;
        }

        public int OnClockSetRate(long hnsSystemTime, float flRate)
        {
            Wrapper.CCallbackHandler _handler =
                new Wrapper.OnClockSetRateHandler(m_Caller, hnsSystemTime, flRate);
            _handler.Invoke();
            return _handler.m_Result;
        }

        #endregion

        #region IMFTopologyServiceLookupClient Members

        public int InitServicePointers(IntPtr pLookup)
        {
            Wrapper.CCallbackHandler _handler =
                new Wrapper.InitServicePointersHandler(m_Caller, pLookup);
            _handler.Invoke();
            return _handler.m_Result;
        }

        public int ReleaseServicePointers()
        {
            Wrapper.CCallbackHandler _handler =
                new Wrapper.ReleaseServicePointersHandler(m_Caller);
            _handler.Invoke();
            return _handler.m_Result;
        }

        #endregion

        #region IMFVideoPresenter Members

        public int ProcessMessage(MFVPMessageType eMessage, IntPtr ulParam)
        {
            Wrapper.CCallbackHandler _handler =
                new Wrapper.ProcessMessageHandler(m_Caller, eMessage, ulParam);
            _handler.Invoke();
            return _handler.m_Result;
        }

        public int GetCurrentMediaType(out IMFVideoMediaType ppMediaType)
        {
            Wrapper.CCallbackHandler _handler =
                new Wrapper.GetCurrentMediaTypeHandler(m_Caller);
            _handler.Invoke();
            ppMediaType = ((Wrapper.GetCurrentMediaTypeHandler)_handler).ppMediaType;
            return _handler.m_Result;
        }

        #endregion

        #region Implementation

        #region IMFClockStateSink Members

        public int OnClockStartImpl(long hnsSystemTime, long llClockStartOffset)
        {
            HRESULT hr = S_OK;

            lock (m_ObjectLock)
            {
                hr = CheckShutdown();
                if (hr.Failed) return hr;

                m_RenderState = RENDER_STATE.RENDER_STATE_STARTED;

                // Check if the clock is already active (not stopped). 
                if (IsActive())
                {
                    // If the clock position changes while the clock is active, it 
                    // is a seek request. We need to flush all pending samples.
                    if (llClockStartOffset != PRESENTATION_CURRENT_POSITION)
                    {
                        Flush();
                    }
                }

                ProcessOutputLoop();
            }
            return hr;
        }

        public int OnClockRestartImpl(long hnsSystemTime)
        {
            HRESULT hr = S_OK;
            lock (m_ObjectLock)
            {
                hr = CheckShutdown();
                if (hr.Failed) return hr;

                // The EVR calls OnClockRestart only while paused.
                ASSERT(m_RenderState == RENDER_STATE.RENDER_STATE_PAUSED);

                m_RenderState = RENDER_STATE.RENDER_STATE_STARTED;

                // Now resume the presentation loop.
                ProcessOutputLoop();
            }
            return hr;
        }

        public int OnClockStopImpl(long hnsSystemTime)
        {
            HRESULT hr = S_OK;
            lock (m_ObjectLock)
            {
                hr = CheckShutdown();
                if (hr.Failed) return hr;
                if (m_RenderState != RENDER_STATE.RENDER_STATE_STOPPED)
                {
                    m_RenderState = RENDER_STATE.RENDER_STATE_STOPPED;
                    Flush();
                }
            }
            return hr;
        }

        public int OnClockPauseImpl(long hnsSystemTime)
        {
            HRESULT hr = S_OK;

            lock (m_ObjectLock)
            {
                // We cannot pause the clock after shutdown.
                hr = CheckShutdown();
                if (hr.Failed) return hr;

                // Set the state. (No other actions are necessary.)
                m_RenderState = RENDER_STATE.RENDER_STATE_PAUSED;

            }
            return hr;
        }

        public int OnClockSetRateImpl(long hnsSystemTime, float flRate)
        {
            // Note: 
            // The presenter reports its maximum rate through the IMFRateSupport interface.
            // Here, we assume that the EVR honors the maximum rate.

            HRESULT hr = S_OK;

            lock (m_ObjectLock)
            {

                hr = CheckShutdown();
                if (hr.Failed) return hr;

                m_fRate = flRate;

                // Tell the scheduler about the new rate.
                m_scheduler.SetClockRate(flRate);
            }
            return hr;
        }

        #endregion

        #region IMFTopologyServiceLookupClient Members

        public int InitServicePointersImpl(IntPtr pLookup)
        {
            try
            {
                if (pLookup == IntPtr.Zero) return E_POINTER;

                HRESULT hr = S_OK;

                lock (m_ObjectLock)
                {
                    // Do not allow initializing when playing or paused.
                    if (IsActive()) return MFHelper.MF_E_INVALIDREQUEST;

                    if (m_pMixer != IntPtr.Zero)
                    {
                        Marshal.Release(m_pMixer);
                        m_pMixer = IntPtr.Zero;
                    }
                    m_pClock = null;
                    m_pMediaEventSink = null;

                    uint dwObjectCount;

                    MFTopologyServiceLookup _lookUp = new MFTopologyServiceLookup(pLookup);

                    if (hr.Succeeded)
                    {
                        // Ask for the clock. Optional, because the EVR might not have a clock.
                        dwObjectCount = 1;

                        IntPtr[] o = new IntPtr[dwObjectCount];

                        _lookUp.LookupService(
                            MFServiceLookUpType.GLOBAL,
                            0,
                            MFServices.MR_VIDEO_RENDER_SERVICE,
                            typeof(IMFClock).GUID,
                            o,
                            ref dwObjectCount
                            );

                        if (o[0] != IntPtr.Zero && dwObjectCount > 0)
                        {
                            m_pClock = (IMFClock)Marshal.GetObjectForIUnknown(o[0]);
                        }
                        // Ask for the mixer. (Required.)
                        dwObjectCount = 1;

                        hr = (HRESULT)_lookUp.LookupService(
                            MFServiceLookUpType.GLOBAL,
                            0,
                            MFServices.MR_VIDEO_MIXER_SERVICE,
                            typeof(IMFTransform).GUID,
                            o,
                            ref dwObjectCount);

                        hr.Assert();

                        if (o[0] != IntPtr.Zero && dwObjectCount > 0)
                        {
                            m_pMixer = o[0];
                        }

                        hr = ConfigureMixer((IMFTransform)Marshal.GetObjectForIUnknown(m_pMixer));

                        hr.Assert();
                        if (hr.Failed) return hr;

                        o[0] = IntPtr.Zero;
                        // Ask for the EVR's event-sink interface. (Required.)
                        dwObjectCount = 1;
                        hr = (HRESULT)_lookUp.LookupService(
                            MFServiceLookUpType.GLOBAL,
                            0,
                            MFServices.MR_VIDEO_RENDER_SERVICE,
                            typeof(IMediaEventSink).GUID,
                            o,
                            ref dwObjectCount);

                        hr.Assert();
                        if (hr.Failed) return hr;

                        if (o[0] != IntPtr.Zero && dwObjectCount > 0)
                        {
                            m_pMediaEventSink = new MediaEventSink(o[0]);
                        }

                        // Successfully initialized. Set the state to "stopped."
                        m_RenderState = RENDER_STATE.RENDER_STATE_STOPPED;
                    }
                    _lookUp.Dispose();
                }

                return hr;
            }
            catch (Exception _exception)
            {
                throw _exception;
            }
        }

        public int ReleaseServicePointersImpl()
        {
            try
            {
                HRESULT hr = S_OK;

                // Enter the shut-down state.
                lock (m_ObjectLock)
                {
                    m_RenderState = RENDER_STATE.RENDER_STATE_SHUTDOWN;
                }

                // Flush any samples that were scheduled.
                Flush();

                // Clear the media type and release related resources (surfaces, etc).
                SetMediaType(null);

                // Release all services that were acquired from InitServicePointers.
                m_pClock = null;

                if (m_pMixer != IntPtr.Zero)
                {
                    Marshal.Release(m_pMixer);
                    m_pMixer = IntPtr.Zero;
                }

                m_pMediaEventSink = null;
                return hr;
            }
            catch (Exception _exception)
            {
                throw _exception;
            }
        }

        #endregion

        #region IMFVideoPresenter Members

        public int ProcessMessageImpl(MFVPMessageType eMessage, IntPtr ulParam)
        {
            try
            {
                HRESULT hr = S_OK;

                lock (m_ObjectLock)
                {
                    hr = CheckShutdown();
                    if (hr.Failed) return hr;

                    switch (eMessage)
                    {
                        // Flush all pending samples.
                        case MFVPMessageType.Flush:
                            hr = Flush();
                            break;

                        // Renegotiate the media type with the mixer.
                        case MFVPMessageType.InvalidateMediaType:
                            hr = RenegotiateMediaType();
                            break;

                        // The mixer received a new input sample. 
                        case MFVPMessageType.ProcessInputNotify:
                            hr = ProcessInputNotify();
                            break;

                        // Streaming is about to start.
                        case MFVPMessageType.BeginStreaming:
                            hr = BeginStreaming();
                            break;

                        // Streaming has ended. (The EVR has stopped.)
                        case MFVPMessageType.EndStreaming:
                            hr = EndStreaming();
                            break;

                        // All input streams have ended.
                        case MFVPMessageType.EndOfStream:
                            // Set the EOS flag. 
                            m_bEndStreaming = TRUE;
                            // Check if it's time to send the EC_COMPLETE event to the EVR.
                            hr = CheckEndOfStream();
                            break;

                        // Frame-stepping is starting.
                        case MFVPMessageType.Step:
                            hr = E_NOTIMPL;
                            break;

                        // Cancels frame-stepping.
                        case MFVPMessageType.CancelStep:
                            hr = E_NOTIMPL;
                            break;

                        default:
                            hr = E_INVALIDARG; // Unknown message. (This case should never occur.)
                            break;
                    }
                }
                return hr;
            }
            catch (Exception _exception)
            {
                throw _exception;
            }
        }

        public int GetCurrentMediaTypeImpl(out IMFVideoMediaType ppMediaType)
        {
            HRESULT hr = S_OK;
            ppMediaType = null;
            try
            {
                lock (m_ObjectLock)
                {
                    hr = CheckShutdown();
                    if (hr.Failed) return hr;

                    if (m_pMediaType == null)
                    {
                        return MFHelper.MF_E_NOT_INITIALIZED;
                    }

                    // The function returns an IMFVideoMediaType pointer, and we store our media
                    // type as an IMFMediaType pointer, so we need to QI.

                    ppMediaType = (IMFVideoMediaType)m_pMediaType;

                }
            }
            catch (Exception _exception)
            {
                hr = (HRESULT)Marshal.GetHRForException(_exception);
            }
            return hr;
        }

        #endregion

        #endregion

        #region Protected Methods

        // CheckShutdown: 
        //     Returns MF_E_SHUTDOWN if the presenter is shutdown.
        //     Call this at the start of any methods that should fail after shutdown.
        protected HRESULT CheckShutdown()
        {
            if (m_RenderState == RENDER_STATE.RENDER_STATE_SHUTDOWN)
            {
                return MFHelper.MF_E_SHUTDOWN;
            }
            else
            {
                return S_OK;
            }
        }

        // IsActive: The "active" state is started or paused.
        protected bool IsActive()
        {
            return ((m_RenderState == RENDER_STATE.RENDER_STATE_STARTED) || (m_RenderState == RENDER_STATE.RENDER_STATE_PAUSED));
        }

        // IsScrubbing: Scrubbing occurs when the frame rate is 0.
        protected bool IsScrubbing()
        {
            return m_fRate == 0.0f;
        }

        // NotifyEvent: Send an event to the EVR through its IMediaEventSink interface.
        protected void NotifyEvent(DsEvCode EventCode, IntPtr Param1, IntPtr Param2)
        {
            try
            {
                if (m_pMediaEventSink != null)
                {
                    m_pMediaEventSink.Notify(EventCode, Param1, Param2);
                }
            }
            catch
            {

            }
        }

        // Mixer operations
        protected HRESULT ConfigureMixer(IMFTransform pMixer)
        {
            HRESULT hr = S_OK;
            // Make sure that the mixer has the same device ID as ourselves.
            IntPtr _mixer = Marshal.GetIUnknownForObject(pMixer);
            Guid _guid;
            _guid = typeof(IMFVideoDeviceID).GUID;
            IntPtr _interface;
            hr = (HRESULT)Marshal.QueryInterface(_mixer, ref _guid, out _interface);
            if (hr.Succeeded)
            {
                IMFVideoDeviceID pDeviceID = (IMFVideoDeviceID)Marshal.GetObjectForIUnknown(_interface);
                Guid deviceID = Guid.Empty;
                hr = (HRESULT)pDeviceID.GetDeviceID(out deviceID);
                if (hr.Succeeded)
                {
                    if (deviceID != Device.NativeInterface)
                    {
                        hr = MFHelper.MF_E_INVALIDREQUEST;
                    }
                }
                Marshal.Release(_interface);
            }
            Marshal.Release(_mixer);

            // Set the zoom rectangle (ie, the source clipping rectangle).
            MFHelper.SetMixerSourceRect(pMixer, m_nrcSource);

            return hr;
        }

        // Formats
        protected HRESULT CreateOptimalVideoType(IMFMediaType pProposedType, out IMFMediaType ppOptimalType)
        {
            ppOptimalType = null;
            HRESULT hr = S_OK;

            DsRect rcOutput;
            MFVideoArea displayArea;

            MFHelper.VideoTypeBuilder pmtOptimal = null;

            // Create the helper object to manipulate the optimal type.
            hr = MFHelper.MediaTypeBuilder.Create(out pmtOptimal);
            if (hr.Failed) return hr;

            // Clone the proposed type.
            hr = pmtOptimal.CopyFrom(pProposedType);
            if (hr.Failed) return hr;

            // Modify the new type.

            // For purposes of this SDK sample, we assume 
            // 1) The monitor's pixels are square.
            // 2) The presenter always preserves the pixel aspect ratio.

            // Set the pixel aspect ratio (PAR) to 1:1 (see assumption #1, above)
            hr = pmtOptimal.SetPixelAspectRatio(1, 1);
            if (hr.Failed) return hr;

            // Get the output rectangle.
            rcOutput = GetDestinationRect();

            if (rcOutput.IsEmpty())
            {
                // Calculate the output rectangle based on the media type.
                hr = CalculateOutputRectangle(pProposedType, out rcOutput);
                if (hr.Failed) return hr;
            }

            // Set the extended color information: Use BT.709 
            hr = pmtOptimal.SetYUVMatrix(MFVideoTransferMatrix.BT709);
            if (hr.Failed) return hr;
            hr = pmtOptimal.SetTransferFunction(MFVideoTransferFunction.Func709);
            if (hr.Failed) return hr;
            hr = pmtOptimal.SetVideoPrimaries(MFVideoPrimaries.BT709);
            if (hr.Failed) return hr;
            hr = pmtOptimal.SetVideoNominalRange(MFNominalRange.MFNominalRange_16_235);
            if (hr.Failed) return hr;
            hr = pmtOptimal.SetVideoLighting(MFVideoLighting.Dim);
            if (hr.Failed) return hr;

            // Set the target rect dimensions. 
            hr = pmtOptimal.SetFrameDimensions(rcOutput.right, rcOutput.bottom);
            if (hr.Failed) return hr;

            // Set the geometric aperture, and disable pan/scan.
            displayArea = new MFVideoArea(0, 0, rcOutput.right, rcOutput.bottom);

            hr = pmtOptimal.SetPanScanEnabled(false);
            if (hr.Failed) return hr;
            hr = pmtOptimal.SetGeometricAperture(displayArea);
            if (hr.Failed) return hr;
            // Set the pan/scan aperture and the minimum display aperture. We don't care
            // about them per se, but the mixer will reject the type if these exceed the 
            // frame dimentions.
            hr = pmtOptimal.SetPanScanAperture(displayArea);
            if (hr.Failed) return hr;
            hr = pmtOptimal.SetMinDisplayAperture(displayArea);
            if (hr.Failed) return hr;

            // Return the pointer to the caller.
            hr = pmtOptimal.GetMediaType(out ppOptimalType);

            return hr;
        }

        protected HRESULT CalculateOutputRectangle(IMFMediaType pProposedType, out DsRect prcOutput)
        {
            prcOutput = null;
            HRESULT hr = S_OK;
            int srcWidth = 0, srcHeight = 0;

            MFRatio inputPAR = new MFRatio(0, 0);
            MFRatio outputPAR = new MFRatio(0, 0);
            DsRect rcOutput = new DsRect(0, 0, 0, 0);

            MFVideoArea displayArea;

            MFHelper.VideoTypeBuilder pmtProposed = null;

            // Helper object to read the media type.
            hr = MFHelper.MediaTypeBuilder.Create(pProposedType, out pmtProposed);
            if (hr.Failed) return hr;

            // Get the source's frame dimensions.
            hr = pmtProposed.GetFrameDimensions(out srcWidth, out srcHeight);
            if (hr.Failed) return hr;

            // Get the source's display area. 
            hr = pmtProposed.GetVideoDisplayArea(out displayArea);
            if (hr.Failed) return hr;

            // Calculate the x,y offsets of the display area.
            int offsetX = (int)displayArea.OffsetX.GetOffset();
            int offsetY = (int)displayArea.OffsetY.GetOffset();

            // Use the display area if valid. Otherwise, use the entire frame.
            if (displayArea.Area.Width != 0 &&
                displayArea.Area.Height != 0 &&
                offsetX + displayArea.Area.Width <= (srcWidth) &&
                offsetY + displayArea.Area.Height <= (srcHeight))
            {
                rcOutput.left = offsetX;
                rcOutput.right = offsetX + displayArea.Area.Width;
                rcOutput.top = offsetY;
                rcOutput.bottom = offsetY + displayArea.Area.Height;
            }
            else
            {
                rcOutput.left = 0;
                rcOutput.top = 0;
                rcOutput.right = srcWidth;
                rcOutput.bottom = srcHeight;
            }

            // rcOutput is now either a sub-rectangle of the video frame, or the entire frame.

            // If the pixel aspect ratio of the proposed media type is different from the monitor's, 
            // letterbox the video. We stretch the image rather than shrink it.

            inputPAR = pmtProposed.GetPixelAspectRatio();    // Defaults to 1:1

            outputPAR.Denominator = outputPAR.Numerator = 1; // This is an assumption of the sample.

            // Adjust to get the correct picture aspect ratio.
            prcOutput = MFHelper.CorrectAspectRatio(rcOutput, inputPAR, outputPAR);

            return NOERROR;
        }

        protected HRESULT SetMediaType(IMFMediaType pMediaType)
        {
            // Note: pMediaType can be NULL (to clear the type)

            // Clearing the media type is allowed in any state (including shutdown).
            if (pMediaType == null)
            {
                m_pMediaType = null;
                ReleaseResources();
                return S_OK;
            }

            HRESULT hr = S_OK;
            MFRatio fps = new MFRatio(0, 0);

            // Cannot set the media type after shutdown.
            hr = CheckShutdown();

            if (hr.Succeeded)
            {
                // Check if the new type is actually different.
                // Note: This function safely handles NULL input parameters.
                if (MFHelper.AreMediaTypesEqual(m_pMediaType, pMediaType))
                {
                    return S_OK; // Nothing more to do.
                }

                // We're really changing the type. First get rid of the old type.
                m_pMediaType = null;
                ReleaseResources();

                // Initialize the presenter engine with the new media type.
                // The presenter engine allocates the samples. 

                hr = CreateVideoSamples(pMediaType);

                if (hr.Succeeded)
                {
                    // Set the frame rate on the scheduler. 
                    if (SUCCEEDED(MFHelper.GetFrameRate(pMediaType, out fps)) && (fps.Numerator != 0) && (fps.Denominator != 0))
                    {
                        m_scheduler.SetFrameRate(fps);
                    }
                    else
                    {
                        // NOTE: The mixer's proposed type might not have a frame rate, in which case 
                        // we'll use an arbitary default. (Although it's unlikely the video source
                        // does not have a frame rate.)
                        m_scheduler.SetFrameRate(g_DefaultFrameRate);
                    }

                    // Store the media type.
                    ASSERT(pMediaType != NULL);
                    m_pMediaType = pMediaType;
                }
            }

            if (hr.Failed)
            {
                ReleaseResources();
            }
            return hr;
        }

        protected HRESULT IsMediaTypeSupported(IMFMediaType pMediaType)
        {
            MFHelper.VideoTypeBuilder pProposed = null;

            HRESULT hr = S_OK;
            Format d3dFormat = Format.Unknown;
            bool bCompressed = false;
            MFVideoInterlaceMode InterlaceMode = MFVideoInterlaceMode.Unknown;
            MFVideoArea VideoCropArea;
            int width = 0, height = 0;

            // Helper object for reading the proposed type.
            hr = MFHelper.MediaTypeBuilder.Create(pMediaType, out pProposed);
            if (hr.Failed) return hr;

            // Reject compressed media types.
            hr = pProposed.IsCompressedFormat(out bCompressed);
            if (hr.Failed) return hr;
            if (bCompressed)
            {
                return MFHelper.MF_E_INVALIDMEDIATYPE;
            }

            // Validate the format.
            int nFcc;
            hr = pProposed.GetFourCC(out nFcc);
            if (hr.Failed) return hr;
            d3dFormat = (Format)nFcc;

            // The D3DPresentEngine checks whether the format can be used as
            // the back-buffer format for the swap chains.
            hr = CheckFormat(d3dFormat);
            if (hr.Failed) return hr;

            // Reject interlaced formats.
            hr = pProposed.GetInterlaceMode(out InterlaceMode);
            if (hr.Failed) return hr;
            if (InterlaceMode != MFVideoInterlaceMode.Progressive)
            {
                return MFHelper.MF_E_INVALIDMEDIATYPE;
            }

            hr = pProposed.GetFrameDimensions(out width, out height);
            if (hr.Failed) return hr;

            // Validate the various apertures (cropping regions) against the frame size.
            // Any of these apertures may be unspecified in the media type, in which case 
            // we ignore it. We just want to reject invalid apertures.
            if (SUCCEEDED(pProposed.GetPanScanAperture(out VideoCropArea)))
            {
                MFHelper.ValidateVideoArea(VideoCropArea, width, height);
            }
            if (SUCCEEDED(pProposed.GetGeometricAperture(out VideoCropArea)))
            {
                MFHelper.ValidateVideoArea(VideoCropArea, width, height);
            }
            if (SUCCEEDED(pProposed.GetMinDisplayAperture(out VideoCropArea)))
            {
                MFHelper.ValidateVideoArea(VideoCropArea, width, height);
            }

            return hr;
        }

        // Message handlers
        protected HRESULT Flush()
        {
            m_bPrerolled = false;

            // The scheduler might have samples that are waiting for
            // their presentation time. Tell the scheduler to flush.

            // This call blocks until the scheduler threads discards all scheduled samples.
            m_scheduler.Flush();

            return S_OK;
        }

        protected HRESULT RenegotiateMediaType()
        {
            HRESULT hr = S_OK;
            bool bFoundMediaType = false;

            IMFMediaType pMixerType = null;
            IMFMediaType pOptimalType = null;

            if (m_pMixer == IntPtr.Zero)
            {
                return MFHelper.MF_E_INVALIDREQUEST;
            }

            IMFTransform pMixer = (IMFTransform)Marshal.GetObjectForIUnknown(m_pMixer);
            // Loop through all of the mixer's proposed output types.
            int iTypeIndex = 0;
            while (!bFoundMediaType && (hr != MFHelper.MF_E_NO_MORE_TYPES))
            {
                pMixerType = null;
                pOptimalType = null;

                // Step 1. Get the next media type supported by mixer.
                hr = (HRESULT)pMixer.GetOutputAvailableType(0, iTypeIndex++, out pMixerType);
                if (hr.Failed)
                {
                    break;
                }

                // From now on, if anything in this loop fails, try the next type,
                // until we succeed or the mixer runs out of types.

                // Step 2. Check if we support this media type. 
                if (hr.Succeeded)
                {
                    // Note: None of the modifications that we make later in CreateOptimalVideoType
                    // will affect the suitability of the type, at least for us. (Possibly for the mixer.)
                    hr = IsMediaTypeSupported(pMixerType);
                }

                // Step 3. Adjust the mixer's type to match our requirements.
                if (hr.Succeeded)
                {
                    hr = CreateOptimalVideoType(pMixerType, out pOptimalType);
                }

                // Step 4. Check if the mixer will accept this media type.
                if (hr.Succeeded)
                {
                    hr = (HRESULT)pMixer.SetOutputType(0, pOptimalType, MFTSetTypeFlags.TestOnly);
                }

                // Step 5. Try to set the media type on ourselves.
                if (hr.Succeeded)
                {
                    hr = SetMediaType(pOptimalType);
                }

                // Step 6. Set output media type on mixer.
                if (hr.Succeeded)
                {
                    hr = (HRESULT)pMixer.SetOutputType(0, pOptimalType, 0);

                    hr.Assert(); // This should succeed unless the MFT lied in the previous call.

                    // If something went wrong, clear the media type.
                    if (hr.Failed)
                    {
                        SetMediaType(null);
                    }
                }

                if (hr.Succeeded)
                {
                    bFoundMediaType = true;
                }
            }

            pMixerType = null;
            pOptimalType = null;

            return hr;
        }

        protected HRESULT ProcessInputNotify()
        {
            HRESULT hr = S_OK;

            // Set the flag that says the mixer has a new sample.
            m_bSampleNotify = true;

            if (m_pMediaType == NULL)
            {
                // We don't have a valid media type yet.
                hr = MFHelper.MF_E_TRANSFORM_TYPE_NOT_SET;
            }
            else
            {
                // Try to process an output sample.
                ProcessOutputLoop();
            }
            return hr;
        }

        protected HRESULT BeginStreaming()
        {
            // Start the scheduler thread. 
            return m_scheduler.StartScheduler(m_pClock);
        }

        protected HRESULT EndStreaming()
        {
            // Stop the scheduler thread.
            return m_scheduler.StopScheduler();
        }

        protected HRESULT CheckEndOfStream()
        {
            if (!m_bEndStreaming)
            {
                // The EVR did not send the MFVP_MESSAGE_ENDOFSTREAM message.
                return S_OK;
            }

            if (m_bSampleNotify)
            {
                // The mixer still has input. 
                return S_OK;
            }

            if (m_SamplePool.AreSamplesPending())
            {
                // Samples are still scheduled for rendering.
                return S_OK;
            }

            // Everything is complete. Now we can tell the EVR that we are done.
            NotifyEvent(DsEvCode.Complete, IntPtr.Zero, IntPtr.Zero);
            m_bEndStreaming = false;
            return S_OK;
        }

        protected void ProcessOutputLoop()
        {
            HRESULT hr = S_OK;

            // Process as many samples as possible.
            while (hr == S_OK)
            {
                // If the mixer doesn't have a new input sample, break from the loop.
                if (!m_bSampleNotify)
                {
                    hr = MFHelper.MF_E_TRANSFORM_NEED_MORE_INPUT;
                    break;
                }

                // Try to process a sample.
                hr = ProcessOutput();

                // NOTE: ProcessOutput can return S_FALSE to indicate it did not process a sample.
                // If so, we break out of the loop.
            }

            if (hr == MFHelper.MF_E_TRANSFORM_NEED_MORE_INPUT)
            {
                // The mixer has run out of input data. Check if we're at the end of the stream.
                CheckEndOfStream();
            }
        }

        protected HRESULT ProcessOutput()
        {
            try
            {
                //ASSERT(m_bSampleNotify || m_bRepaint);  // See note above.

                HRESULT hr = S_OK;
                ProcessOutputStatus dwStatus = 0;
                long mixerStartTime = 0, mixerEndTime = 0;
                long systemTime = 0;
                bool bRepaint = m_bRepaint; // Temporarily store this state flag.  

                MFTOutputDataBuffer dataBuffer = new MFTOutputDataBuffer();

                MFSample pSample = null;

                // If the clock is not running, we present the first sample,
                // and then don't present any more until the clock starts. 

                if ((m_RenderState != RENDER_STATE.RENDER_STATE_STARTED) &&  // Not running.
                     !m_bRepaint &&                                          // Not a repaint request.
                     m_bPrerolled                                            // At least one sample has been presented.
                     )
                {
                    return S_FALSE;
                }

                // Make sure we have a pointer to the mixer.
                if (m_pMixer == IntPtr.Zero)
                {
                    return MFHelper.MF_E_INVALIDREQUEST;
                }

                // Try to get a free sample from the video sample pool.
                hr = (m_SamplePool.GetBuffer(out pSample, false) ? S_OK : MFHelper.MF_E_SAMPLEALLOCATOR_EMPTY);
                if (hr == MFHelper.MF_E_SAMPLEALLOCATOR_EMPTY)
                {
                    return S_FALSE; // No free samples. We'll try again when a sample is released.
                }
                // Fail on any other error code.
                if (hr.Succeeded)
                {
                    // From now on, we have a valid video sample pointer, where the mixer will
                    // write the video data.
                    ASSERT(pSample != null);

                    // (If the following assertion fires, it means we are not managing the sample pool correctly.)
                    if (m_bRepaint)
                    {
                        // Repaint request. Ask the mixer for the most recent sample.
                        MFHelper.SetDesiredSampleTime(pSample.Sample, m_scheduler.LastSampleTime, m_scheduler.FrameDuration);
                        m_bRepaint = FALSE; // OK to clear this flag now.
                    }
                    else
                    {
                        // Not a repaint request. Clear the desired sample time; the mixer will
                        // give us the next frame in the stream.
                        ClearDesiredSampleTime(pSample);

                        if (m_pClock != null)
                        {
                            // Latency: Record the starting time for the ProcessOutput operation. 
                            m_pClock.GetCorrelatedTime(0, out mixerStartTime, out  systemTime);
                        }
                    }

                    // Now we are ready to get an output sample from the mixer. 
                    dataBuffer.dwStreamID = 0;
                    dataBuffer.pSample = Marshal.GetIUnknownForObject(pSample.Sample);
                    dataBuffer.dwStatus = 0;

                    {
                        IMFTransform pMixer = (IMFTransform)Marshal.GetObjectForIUnknown(m_pMixer);
                        hr = (HRESULT)pMixer.ProcessOutput(0, 1, new MFTOutputDataBuffer[] { dataBuffer }, out dwStatus);
                    }
                    if (hr.Failed)
                    {
                        // Return the sample to the pool.
                        m_SamplePool.ReleaseBuffer(pSample);

                        // Handle some known error codes from ProcessOutput.
                        if (hr == MFHelper.MF_E_TRANSFORM_TYPE_NOT_SET)
                        {
                            // The mixer's format is not set. Negotiate a new format.
                            hr = RenegotiateMediaType();
                        }
                        else if (hr == MFHelper.MF_E_TRANSFORM_STREAM_CHANGE)
                        {
                            // There was a dynamic media type change. Clear our media type.
                            SetMediaType(null);
                        }
                        else if (hr == MFHelper.MF_E_TRANSFORM_NEED_MORE_INPUT)
                        {
                            // The mixer needs more input. 
                            // We have to wait for the mixer to get more input.
                            m_bSampleNotify = false;
                        }
                    }
                    else
                    {
                        
                        // We got an output sample from the mixer.
                        if (m_pClock != null && !bRepaint)
                        {
                            // Latency: Record the ending time for the ProcessOutput operation,
                            // and notify the EVR of the latency. 

                            m_pClock.GetCorrelatedTime(0, out mixerEndTime, out systemTime);

                            long latencyTime = mixerEndTime - mixerStartTime;

                            IntPtr _ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(latencyTime));
                            try
                            {
                                Marshal.WriteInt64(_ptr, latencyTime);
                                NotifyEvent(DsEvCode.ProcessingLatency, _ptr, IntPtr.Zero);
                            }
                            finally
                            {
                                Marshal.FreeCoTaskMem(_ptr);
                            }
                        }
                        
                        if (hr.Succeeded)
                        {
                            // Schedule the sample.
                            hr = DeliverSample(pSample, bRepaint);
                        }
                        if (hr.Succeeded)
                        {
                            m_bPrerolled = true; // We have presented at least one sample now.
                        }
                    }

                }
                // Release any events that were returned from the ProcessOutput method. 
                // (We don't expect any events from the mixer, but this is a good practice.)
                if (dataBuffer.pEvents != null)
                {
                    Marshal.ReleaseComObject(dataBuffer.pEvents);
                }
                pSample = null;

                return hr;
            }
            catch (Exception _exception)
            {
                throw _exception;
            }
        }

        protected HRESULT DeliverSample(MFSample pSample, bool bRepaint)
        {
            ASSERT(pSample != null);

            HRESULT hr = S_OK;

            // If we are not actively playing, OR we are scrubbing (rate = 0) OR this is a 
            // repaint request, then we need to present the sample immediately. Otherwise, 
            // schedule it normally.

            bool bPresentNow = ((m_RenderState != RENDER_STATE.RENDER_STATE_STARTED) || IsScrubbing() || bRepaint);

            hr = m_scheduler.ScheduleSample(pSample, bPresentNow);

            return hr;
        }

        protected void ReleaseResources()
        {
            // Increment the token counter to indicate that all existing video samples
            // are "stale." As these samples get released, we'll dispose of them. 
            //
            // Note: The token counter is required because the samples are shared
            // between more than one thread, and they are returned to the presenter 
            // through an asynchronous callback (OnSampleFree). Without the token, we
            // might accidentally re-use a stale sample after the ReleaseResources
            // method returns.

            Flush();

            m_SamplePool.Clear();
        }

        protected HRESULT ClearDesiredSampleTime(MFSample pSample)
        {
            if (pSample == null || pSample.Sample == null) return E_POINTER;

            IMFDesiredSample pDesired = (IMFDesiredSample)pSample.Sample;

            pSample.PresentationTime = -1;
            pSample.Duration = -1;
            if (pDesired != null)
            {
                pDesired.Clear();
            }
            return NOERROR;
        }

        #endregion

        #region Helper Methods

        protected HRESULT CheckFormat(Format format)
        {
            HRESULT hr = S_OK;

            try
            {
                int uAdapter = 0;
                DeviceType type = DeviceType.Hardware;

                DisplayMode mode;
                CreationParameters _params;

                if (m_Device == null) return E_UNEXPECTED;

                _params = m_Device.CreationParameters;

                uAdapter = _params.AdapterOrdinal;
                type = _params.DeviceType;


                mode = m_Device.Direct3D.GetAdapterDisplayMode(uAdapter);

                if (!m_Device.Direct3D.CheckDeviceType(uAdapter, type, mode.Format, format, true))
                {
                    hr = E_FAIL;
                }
            }
            catch (COMException _exception)
            {
                hr = (HRESULT)_exception.ErrorCode;
            }
            return hr;
        }

        protected int RefreshRate()
        {
            int uAdapter = 0;

            DisplayMode mode;
            CreationParameters _params;

            if (m_Device == null) return 0;

            _params = m_Device.CreationParameters;
            uAdapter = _params.AdapterOrdinal;

            mode = m_Device.Direct3D.GetAdapterDisplayMode(uAdapter);

            return mode.RefreshRate;
        }

        protected DsRect GetDestinationRect()
        {
            return new DsRect();
        }

        protected HRESULT CreateVideoSamples(IMFMediaType pFormat)
        {
            if (pFormat == null)
            {
                return MFHelper.MF_E_UNEXPECTED;
            }

            HRESULT hr = S_OK;
            PresentParameters pp;

            lock (m_ObjectLock)
            {
                hr = GetSwapChainPresentParameters(pFormat, out pp);
                hr.Assert();
                if (hr.Failed)
                {
                    return hr;
                }

                // Create the video samples.
                for (int i = 0; i < PRESENTER_BUFFER_COUNT; i++)
                {
                    Surface _surface = null;
                    if (m_bUseSwapChain)
                    {
                        // Create a new swap chain.
                        SwapChain pSwapChain = new SwapChain(m_Device, pp);
                        if (pSwapChain == null)
                        {
                            return E_UNEXPECTED;
                        }
                        _surface = pSwapChain.GetBackBuffer(0);
                        if (_surface == null) return E_FAIL;
                        m_Device.ColorFill(_surface, new Color4(Color.Black));
                    }
                    else
                    {
                        _surface = Surface.CreateRenderTarget(m_Device, pp.BackBufferWidth, pp.BackBufferHeight, pp.BackBufferFormat, pp.Multisample, pp.MultisampleQuality, true);
                    }
                    // Create the video sample from the swap chain.
                    MFSample pVideoSample = new MFSample();
                    pVideoSample.Target = _surface;

                    hr = (HRESULT)MFHelper.MFCreateVideoSampleFromSurface(Marshal.GetObjectForIUnknown(_surface.ComPointer), out pVideoSample.Sample);

                    // Add it to the list.
                    m_SamplePool.AddSample(pVideoSample);

                }
            }

            return NOERROR;
        }

        protected HRESULT GetSwapChainPresentParameters(IMFMediaType pType, out PresentParameters pPP)
        {
            pPP = null;

            HRESULT hr;
            int width = 0, height = 0;
            UInt32 d3dFormat = 0;

            Guid _subtype;
            hr = (HRESULT)pType.GetGUID(MFAttributesClsid.MF_MT_SUBTYPE, out _subtype);
            hr.Assert();
            if (hr.Succeeded)
            {
                hr = MFHelper.MFGetAttribute2UINT32asUINT64(pType, MFAttributesClsid.MF_MT_FRAME_SIZE, out width, out height);
                hr.Assert();

                if (hr.Succeeded)
                {
                    FOURCC _fourcc = new FOURCC(_subtype);
                    d3dFormat = _fourcc;
                    pPP = new PresentParameters();
                    pPP.BackBufferWidth = width;
                    pPP.BackBufferHeight = height;
                    pPP.Windowed = true;
                    pPP.SwapEffect = SwapEffect.Copy;
                    pPP.BackBufferFormat = (Format)d3dFormat;
                    pPP.DeviceWindowHandle = m_VideoControl != null ? m_VideoControl.Handle : m_Device.CreationParameters.Window;
                    pPP.PresentFlags = PresentFlags.Video;
                    pPP.PresentationInterval = PresentInterval.Default;

                    if (m_Device.CreationParameters.DeviceType != DeviceType.Hardware)
                    {
                        pPP.PresentFlags = pPP.PresentFlags | PresentFlags.LockableBackBuffer;
                    }
                }
            }
            return hr;
        }

        #endregion

        #region SchedulerCallback Members

        public HRESULT PresentSample(object pSample, long llTarget)
        {
            if (pSample != null)
            {
                MFSample _sample = (pSample as MFSample);
                if (OnSurfaceReady != null)
                {
                    OnSurfaceReady(ref _sample.Target);
                }
                m_SamplePool.ReleaseBuffer(_sample);
            }
            ProcessOutputLoop();
            return S_OK;
        }

        #endregion
    }

    #endregion
}