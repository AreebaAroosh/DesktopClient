/******************************************************
                  DirectShow .NET
*******************************************************/
//					DsCore
// Core streaming interfaces, ported from axcore.idl

using System;
using System.Text;
using System.Runtime.InteropServices;

namespace VideoCallP2P.Recorder.DirectShowNET
{

    /*
     * PinDirection
     * PhysicalConnectorType
     * DsHlp
     * IPin
     * IFilterGraph
     * IPersist
     * IPersistStream
     * IMediaFilter
     * IBaseFilter
     * FilterInfo
     * IMediaSeeking
     * SeekingCapabilities
     * SeekingFlags
     * IReferenceClock
     * IEnumFilters
     * IEnumPins
     * AMMediaType
     * PinInfo
     * IMediaSample
     */

    [ComVisible(false)]
    public enum PinDirection
    {
        Input,
        Output
    }

    [ComVisible(false)]
    public enum PhysicalConnectorType
    {
        Audio_1394 = 0x1007,
        Audio_AESDigital = 0x1003,
        Audio_AudioDecoder = 0x1009,
        Audio_AUX = 0x1006,
        Audio_Line = 0x1001,
        Audio_Mic = 0x1002,
        Audio_SCSI = 0x1005,
        Audio_SPDIFDigital = 0x1004,
        Audio_Tuner = 0x1000,
        Audio_USB = 0x1008,
        Video_1394 = 10,
        Video_AUX = 9,
        Video_Composite = 2,
        Video_ParallelDigital = 7,
        Video_RGB = 4,
        Video_SCART = 14,
        Video_SCSI = 8,
        Video_SerialDigital = 6,
        Video_SVideo = 3,
        Video_Tuner = 1,
        Video_USB = 11,
        Video_VideoDecoder = 12,
        Video_VideoEncoder = 13,
        Video_YRYBY = 5
    }

    [ComVisible(false)]
    public class DsHlp
    {
        public const int OAFALSE = 0;
        public const int OATRUE = -1;

        [DllImport("quartz.dll", CharSet = CharSet.Auto)]
        public static extern int AMGetErrorText(int hr, StringBuilder buf, int max);
    }

    [ComImport, Guid("56a86891-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true)]
    public interface IPin
    {
        [PreserveSig]
        int Connect([In] IPin pReceivePin, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);
        [PreserveSig]
        int ReceiveConnection([In] IPin pReceivePin, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);
        [PreserveSig]
        int Disconnect();
        [PreserveSig]
        int ConnectedTo(out IPin ppPin);
        [PreserveSig]
        int ConnectionMediaType([Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);
        [PreserveSig]
        int QueryPinInfo(out PinInfo pInfo);
        [PreserveSig]
        int QueryDirection(out PinDirection pPinDir);
        [PreserveSig]
        int QueryId([MarshalAs(UnmanagedType.LPWStr)] out string Id);
        [PreserveSig]
        int QueryAccept([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);
        [PreserveSig]
        int EnumMediaTypes(IntPtr ppEnum);
        [PreserveSig]
        int QueryInternalConnections(IntPtr apPin, [In, Out] ref int nPin);
        [PreserveSig]
        int EndOfStream();
        [PreserveSig]
        int BeginFlush();
        [PreserveSig]
        int EndFlush();
        [PreserveSig]
        int NewSegment(long tStart, long tStop, double dRate);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("56a8689f-0ad4-11ce-b03a-0020af0ba770"), ComVisible(true)]
    public interface IFilterGraph
    {
        [PreserveSig]
        int AddFilter([In] IBaseFilter pFilter, [In, MarshalAs(UnmanagedType.LPWStr)] string pName);
        [PreserveSig]
        int RemoveFilter([In] IBaseFilter pFilter);
        [PreserveSig]
        int EnumFilters(out IEnumFilters ppEnum);
        [PreserveSig]
        int FindFilterByName([In, MarshalAs(UnmanagedType.LPWStr)] string pName, out IBaseFilter ppFilter);
        [PreserveSig]
        int ConnectDirect([In] IPin ppinOut, [In] IPin ppinIn, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);
        [PreserveSig]
        int Reconnect([In] IPin ppin);
        [PreserveSig]
        int Disconnect([In] IPin ppin);
        [PreserveSig]
        int SetDefaultSyncSource();
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true), Guid("0000010c-0000-0000-C000-000000000046")]
    public interface IPersist
    {
        [PreserveSig]
        int GetClassID(out Guid pClassID);
    }

    [ComImport, ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000010c-0000-0000-C000-000000000046")]
    public interface IPersistStream
    {
        [PreserveSig]
        int GetClassID(out Guid pClassID);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true), Guid("56a86899-0ad4-11ce-b03a-0020af0ba770")]
    public interface IMediaFilter
    {
        [PreserveSig]
        int GetClassID(out Guid pClassID);
        [PreserveSig]
        int Stop();
        [PreserveSig]
        int Pause();
        [PreserveSig]
        int Run(long tStart);
        [PreserveSig]
        int GetState(int dwMilliSecsTimeout, out int filtState);
        [PreserveSig]
        int SetSyncSource([In] IReferenceClock pClock);
        [PreserveSig]
        int GetSyncSource(out IReferenceClock pClock);
    }

    [ComImport, ComVisible(true), Guid("56a86895-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IBaseFilter
    {
        [PreserveSig]
        int GetClassID(out Guid pClassID);
        [PreserveSig]
        int Stop();
        [PreserveSig]
        int Pause();
        [PreserveSig]
        int Run(long tStart);
        [PreserveSig]
        int GetState(int dwMilliSecsTimeout, out int filtState);
        [PreserveSig]
        int SetSyncSource([In] IReferenceClock pClock);
        [PreserveSig]
        int GetSyncSource(out IReferenceClock pClock);
        [PreserveSig]
        int EnumPins(out IEnumPins ppEnum);
        [PreserveSig]
        int FindPin([In, MarshalAs(UnmanagedType.LPWStr)] string Id, out IPin ppPin);
        [PreserveSig]
        int QueryFilterInfo([Out] FilterInfo pInfo);
        [PreserveSig]
        int JoinFilterGraph([In] IFilterGraph pGraph, [In, MarshalAs(UnmanagedType.LPWStr)] string pName);
        [PreserveSig]
        int QueryVendorInfo([MarshalAs(UnmanagedType.LPWStr)] out string pVendorInfo);
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode), ComVisible(false)]
    public class FilterInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x80)]
        public string achName;
        [MarshalAs(UnmanagedType.IUnknown)]
        public object pUnk;
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true), Guid("36b73880-c2c8-11cf-8b46-00805f6cef60")]
    public interface IMediaSeeking
    {
        [PreserveSig]
        int GetCapabilities(out SeekingCapabilities pCapabilities);
        [PreserveSig]
        int CheckCapabilities([In, Out] ref SeekingCapabilities pCapabilities);
        [PreserveSig]
        int IsFormatSupported([In] ref Guid pFormat);
        [PreserveSig]
        int QueryPreferredFormat(out Guid pFormat);
        [PreserveSig]
        int GetTimeFormat(out Guid pFormat);
        [PreserveSig]
        int IsUsingTimeFormat([In] ref Guid pFormat);
        [PreserveSig]
        int SetTimeFormat([In] ref Guid pFormat);
        [PreserveSig]
        int GetDuration(out long pDuration);
        [PreserveSig]
        int GetStopPosition(out long pStop);
        [PreserveSig]
        int GetCurrentPosition(out long pCurrent);
        [PreserveSig]
        int ConvertTimeFormat(out long pTarget, [In] ref Guid pTargetFormat, long Source, [In] ref Guid pSourceFormat);
        [PreserveSig]
        int SetPositions([In, Out, MarshalAs(UnmanagedType.LPStruct)] DsOptInt64 pCurrent, SeekingFlags dwCurrentFlags, [In, Out, MarshalAs(UnmanagedType.LPStruct)] DsOptInt64 pStop, SeekingFlags dwStopFlags);
        [PreserveSig]
        int GetPositions(out long pCurrent, out long pStop);
        [PreserveSig]
        int GetAvailable(out long pEarliest, out long pLatest);
        [PreserveSig]
        int SetRate(double dRate);
        [PreserveSig]
        int GetRate(out double pdRate);
        [PreserveSig]
        int GetPreroll(out long pllPreroll);
    }

    [ComVisible(false), Flags]
    public enum SeekingCapabilities
    {
        CanDoSegments = 0x80,
        CanGetCurrentPos = 8,
        CanGetDuration = 0x20,
        CanGetStopPos = 0x10,
        CanPlayBackwards = 0x40,
        CanSeekAbsolute = 1,
        CanSeekBackwards = 4,
        CanSeekForwards = 2,
        Source = 0x100
    }

    [Flags, ComVisible(false)]
    public enum SeekingFlags
    {
        AbsolutePositioning = 1,
        IncrementalPositioning = 3,
        NoFlush = 0x20,
        NoPositioning = 0,
        PositioningBitsMask = 3,
        RelativePositioning = 2,
        ReturnTime = 8,
        SeekToKeyFrame = 4,
        Segment = 0x10
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true), Guid("56a86897-0ad4-11ce-b03a-0020af0ba770")]
    public interface IReferenceClock
    {
        [PreserveSig]
        int GetTime(out long pTime);
        [PreserveSig]
        int AdviseTime(long baseTime, long streamTime, IntPtr hEvent, out int pdwAdviseCookie);
        [PreserveSig]
        int AdvisePeriodic(long startTime, long periodTime, IntPtr hSemaphore, out int pdwAdviseCookie);
        [PreserveSig]
        int Unadvise(int dwAdviseCookie);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("56a86893-0ad4-11ce-b03a-0020af0ba770"), ComVisible(true)]
    public interface IEnumFilters
    {
        [PreserveSig]
        int Next([In] uint cFilters, out IBaseFilter x, out uint pcFetched);
        [PreserveSig]
        int Skip([In] int cFilters);
        void Reset();
        void Clone(out IEnumFilters ppEnum);
    }

    [ComImport, Guid("56a86892-0ad4-11ce-b03a-0020af0ba770"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumPins
    {
        [PreserveSig]
        int Next([In] int cPins, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] IPin[] ppPins, out int pcFetched);
        [PreserveSig]
        int Skip([In] int cPins);
        void Reset();
        void Clone(out IEnumPins ppEnum);
    }

    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public class AMMediaType
    {
        public Guid majorType;
        public Guid subType;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fixedSizeSamples;
        [MarshalAs(UnmanagedType.Bool)]
        public bool temporalCompression;
        public int sampleSize;
        public Guid formatType;
        public IntPtr unkPtr;
        public int formatSize;
        public IntPtr formatPtr;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1), ComVisible(false)]
    public struct PinInfo
    {
        public IBaseFilter filter;
        public PinDirection dir;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x80)]
        public string name;
    }

    [ComImport, ComVisible(true), Guid("56a8689a-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMediaSample
    {
        [PreserveSig]
        int GetPointer(out IntPtr ppBuffer);
        [PreserveSig]
        int GetSize();
        [PreserveSig]
        int GetTime(out long pTimeStart, out long pTimeEnd);
        [PreserveSig]
        int SetTime([In, MarshalAs(UnmanagedType.LPStruct)] DsOptInt64 pTimeStart, [In, MarshalAs(UnmanagedType.LPStruct)] DsOptInt64 pTimeEnd);
        [PreserveSig]
        int IsSyncPoint();
        [PreserveSig]
        int SetSyncPoint([In, MarshalAs(UnmanagedType.Bool)] bool bIsSyncPoint);
        [PreserveSig]
        int IsPreroll();
        [PreserveSig]
        int SetPreroll([In, MarshalAs(UnmanagedType.Bool)] bool bIsPreroll);
        [PreserveSig]
        int GetActualDataLength();
        [PreserveSig]
        int SetActualDataLength(int len);
        [PreserveSig]
        int GetMediaType([MarshalAs(UnmanagedType.LPStruct)] out AMMediaType ppMediaType);
        [PreserveSig]
        int SetMediaType([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pMediaType);
        [PreserveSig]
        int IsDiscontinuity();
        [PreserveSig]
        int SetDiscontinuity([In, MarshalAs(UnmanagedType.Bool)] bool bDiscontinuity);
        [PreserveSig]
        int GetMediaTime(out long pTimeStart, out long pTimeEnd);
        [PreserveSig]
        int SetMediaTime([In, MarshalAs(UnmanagedType.LPStruct)] DsOptInt64 pTimeStart, [In, MarshalAs(UnmanagedType.LPStruct)] DsOptInt64 pTimeEnd);
    }

} 
