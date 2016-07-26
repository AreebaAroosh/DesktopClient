/******************************************************
                  DirectShow .NET
*******************************************************/
//					DsExtend
// Extended streaming interfaces, ported from axextend.idl

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace VideoCallP2P.Recorder.DirectShowNET
{
    /*
     * ICaptureGraphBuilder2
     * IGraphBuilder
     * IFileSinkFilter
     * IFileSinkFilter2
     * IAMCopyCaptureFileProgress
     * IVideoFrameStep
     * IAMStreamConfig
     * AMTunerSubChannel
     * AMTunerSignalStrength
     * AMTunerModeType
     * AMTunerEventType
     * IAMTuner
     * IAMTunerNotification
     * AnalogVideoStandard
     * TunerInputType
     * IAMTVTuner
     * IAMCrossbar
     * IAMAudioInputMixer
     * VfwCompressDialogs
     * IAMVfwCompressDialogs
     * VideoStreamConfigCaps
     * AudioStreamConfigCaps
     */

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("93E5A4E0-2D50-11d2-ABFA-00A0C9C6E38D"), ComVisible(true)]
    public interface ICaptureGraphBuilder2
    {
        [PreserveSig]
        int SetFiltergraph([In] IGraphBuilder pfg);
        [PreserveSig]
        int GetFiltergraph(out IGraphBuilder ppfg);
        [PreserveSig]
        int SetOutputFileName([In] ref Guid pType, [In, MarshalAs(UnmanagedType.LPWStr)] string lpstrFile, out IBaseFilter ppbf, out IFileSinkFilter ppSink);
        [PreserveSig]
        int FindInterface([In] ref Guid pCategory, [In] ref Guid pType, [In] IBaseFilter pbf, [In] ref Guid riid, [MarshalAs(UnmanagedType.IUnknown)] out object ppint);
        [PreserveSig]
        int RenderStream([In] ref Guid pCategory, [In] ref Guid pType, [In, MarshalAs(UnmanagedType.IUnknown)] object pSource, [In] IBaseFilter pfCompressor, [In] IBaseFilter pfRenderer);
        [PreserveSig]
        int ControlStream([In] ref Guid pCategory, [In] ref Guid pType, [In] IBaseFilter pFilter, [In] long pstart, [In] long pstop, [In] short wStartCookie, [In] short wStopCookie);
        [PreserveSig]
        int AllocCapFile([In, MarshalAs(UnmanagedType.LPWStr)] string lpstrFile, [In] long dwlSize);
        [PreserveSig]
        int CopyCaptureFile([In, MarshalAs(UnmanagedType.LPWStr)] string lpwstrOld, [In, MarshalAs(UnmanagedType.LPWStr)] string lpwstrNew, [In] int fAllowEscAbort, [In] IAMCopyCaptureFileProgress pFilter);
        [PreserveSig]
        int FindPin([In] object pSource, [In] int pindir, [In] ref Guid pCategory, [In] ref Guid pType, [In, MarshalAs(UnmanagedType.Bool)] bool fUnconnected, [In] int num, out IPin ppPin);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("56a868a9-0ad4-11ce-b03a-0020af0ba770"), ComVisible(true)]
    public interface IGraphBuilder
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
        [PreserveSig]
        int Connect([In] IPin ppinOut, [In] IPin ppinIn);
        [PreserveSig]
        int Render([In] IPin ppinOut);
        [PreserveSig]
        int RenderFile([In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFile, [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrPlayList);
        [PreserveSig]
        int AddSourceFilter([In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFileName, [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFilterName, out IBaseFilter ppFilter);
        [PreserveSig]
        int SetLogFile(IntPtr hFile);
        [PreserveSig]
        int Abort();
        [PreserveSig]
        int ShouldOperationContinue();
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true), Guid("a2104830-7c70-11cf-8bce-00aa00a3f1a6")]
    public interface IFileSinkFilter
    {
        [PreserveSig]
        int SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);
        [PreserveSig]
        int GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string pszFileName, [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);
    }

    [ComImport, ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00855B90-CE1B-11d0-BD4F-00A0C911CE86")]
    public interface IFileSinkFilter2
    {
        [PreserveSig]
        int SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);
        [PreserveSig]
        int GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string pszFileName, [Out, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);
        [PreserveSig]
        int SetMode([In] int dwFlags);
        [PreserveSig]
        int GetMode(out int dwFlags);
    }

    [ComImport, ComVisible(true), Guid("670d1d20-a068-11d0-b3f0-00aa003761c5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMCopyCaptureFileProgress
    {
        [PreserveSig]
        int Progress(int iProgress);
    }

    [ComImport, Guid("e46a9787-2b71-444d-a4b5-1fab7b708d6a"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true)]
    public interface IVideoFrameStep
    {
        [PreserveSig]
        int Step(int dwFrames, [In, MarshalAs(UnmanagedType.IUnknown)] object pStepObject);
        [PreserveSig]
        int CanStep(int bMultiple, [In, MarshalAs(UnmanagedType.IUnknown)] object pStepObject);
        [PreserveSig]
        int CancelStep();
    }

    [ComImport, Guid("C6E13340-30AC-11d0-A18C-00A0C9118956"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true)]
    public interface IAMStreamConfig
    {
        [PreserveSig]
        int SetFormat([In, MarshalAs(UnmanagedType.LPStruct)] AMMediaType pmt);
        [PreserveSig]
        int GetFormat(out IntPtr pmt);
        [PreserveSig]
        int GetNumberOfCapabilities(out int piCount, out int piSize);
        [PreserveSig]
        int GetStreamCaps(int iIndex, out IntPtr pmt, [In] IntPtr pSCC);
    }

    [ComVisible(false)]
    public enum AMTunerSubChannel
    {
        Default = -1,
        NoTune = -2
    }

    [ComVisible(false)]
    public enum AMTunerSignalStrength
    {
        NA = -1,
        NoSignal = 0,
        SignalPresent = 1
    }

    [Flags, ComVisible(false)]
    public enum AMTunerModeType
    {
        AMRadio = 4,
        Default = 0,
        Dss = 8,
        FMRadio = 2,
        TV = 1
    }

    [ComVisible(false)]
    public enum AMTunerEventType
    {
        Changed = 1
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("211A8761-03AC-11d1-8D13-00AA00BD8339"), ComVisible(true)]
    public interface IAMTuner
    {
        [PreserveSig]
        int put_Channel(int lChannel, AMTunerSubChannel lVideoSubChannel, AMTunerSubChannel lAudioSubChannel);
        [PreserveSig]
        int get_Channel(out int plChannel, out int plVideoSubChannel, out int plAudioSubChannel);
        [PreserveSig]
        int ChannelMinMax(out int lChannelMin, out int lChannelMax);
        [PreserveSig]
        int put_CountryCode(int lCountryCode);
        [PreserveSig]
        int get_CountryCode(out int plCountryCode);
        [PreserveSig]
        int put_TuningSpace(int lTuningSpace);
        [PreserveSig]
        int get_TuningSpace(out int plTuningSpace);
        [PreserveSig]
        int Logon(IntPtr hCurrentUser);
        [PreserveSig]
        int Logout();
        [PreserveSig]
        int SignalPresent(out AMTunerSignalStrength plSignalStrength);
        [PreserveSig]
        int put_Mode(AMTunerModeType lMode);
        [PreserveSig]
        int get_Mode(out AMTunerModeType plMode);
        [PreserveSig]
        int GetAvailableModes(out AMTunerModeType plModes);
        [PreserveSig]
        int RegisterNotificationCallBack(IAMTunerNotification pNotify, AMTunerEventType lEvents);
        [PreserveSig]
        int UnRegisterNotificationCallBack(IAMTunerNotification pNotify);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true), Guid("211A8760-03AC-11d1-8D13-00AA00BD8339")]
    public interface IAMTunerNotification
    {
        [PreserveSig]
        int OnEvent(AMTunerEventType Event);
    }

    [ComVisible(false), Flags]
    public enum AnalogVideoStandard
    {
        None = 0,
        NTSC_433 = 4,
        NTSC_M = 1,
        NTSC_M_J = 2,
        PAL_60 = 0x800,
        PAL_B = 0x10,
        PAL_D = 0x20,
        PAL_G = 0x40,
        PAL_H = 0x80,
        PAL_I = 0x100,
        PAL_M = 0x200,
        PAL_N = 0x400,
        PAL_N_COMBO = 0x100000,
        SECAM_B = 0x1000,
        SECAM_D = 0x2000,
        SECAM_G = 0x4000,
        SECAM_H = 0x8000,
        SECAM_K = 0x10000,
        SECAM_K1 = 0x20000,
        SECAM_L = 0x40000,
        SECAM_L1 = 0x80000
    }

    [ComVisible(false)]
    public enum TunerInputType
    {
        Cable,
        Antenna
    }

    [ComImport, ComVisible(true), Guid("211A8766-03AC-11d1-8D13-00AA00BD8339"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMTVTuner
    {
        [PreserveSig]
        int put_Channel(int lChannel, AMTunerSubChannel lVideoSubChannel, AMTunerSubChannel lAudioSubChannel);
        [PreserveSig]
        int get_Channel(out int plChannel, out int plVideoSubChannel, out int plAudioSubChannel);
        [PreserveSig]
        int ChannelMinMax(out int lChannelMin, out int lChannelMax);
        [PreserveSig]
        int put_CountryCode(int lCountryCode);
        [PreserveSig]
        int get_CountryCode(out int plCountryCode);
        [PreserveSig]
        int put_TuningSpace(int lTuningSpace);
        [PreserveSig]
        int get_TuningSpace(out int plTuningSpace);
        [PreserveSig]
        int Logon(IntPtr hCurrentUser);
        [PreserveSig]
        int Logout();
        [PreserveSig]
        int SignalPresent(out AMTunerSignalStrength plSignalStrength);
        [PreserveSig]
        int put_Mode(AMTunerModeType lMode);
        [PreserveSig]
        int get_Mode(out AMTunerModeType plMode);
        [PreserveSig]
        int GetAvailableModes(out AMTunerModeType plModes);
        [PreserveSig]
        int RegisterNotificationCallBack(IAMTunerNotification pNotify, AMTunerEventType lEvents);
        [PreserveSig]
        int UnRegisterNotificationCallBack(IAMTunerNotification pNotify);
        [PreserveSig]
        int get_AvailableTVFormats(out AnalogVideoStandard lAnalogVideoStandard);
        [PreserveSig]
        int get_TVFormat(out AnalogVideoStandard lAnalogVideoStandard);
        [PreserveSig]
        int AutoTune(int lChannel, out int plFoundSignal);
        [PreserveSig]
        int StoreAutoTune();
        [PreserveSig]
        int get_NumInputConnections(out int plNumInputConnections);
        [PreserveSig]
        int put_InputType(int lIndex, TunerInputType inputType);
        [PreserveSig]
        int get_InputType(int lIndex, out TunerInputType inputType);
        [PreserveSig]
        int put_ConnectInput(int lIndex);
        [PreserveSig]
        int get_ConnectInput(out int lIndex);
        [PreserveSig]
        int get_VideoFrequency(out int lFreq);
        [PreserveSig]
        int get_AudioFrequency(out int lFreq);
    }

    [ComImport, Guid("C6E13380-30AC-11d0-A18C-00A0C9118956"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true)]
    public interface IAMCrossbar
    {
        [PreserveSig]
        int get_PinCounts(out int OutputPinCount, out int InputPinCount);
        [PreserveSig]
        int CanRoute([In] int OutputPinIndex, [In] int InputPinIndex);
        [PreserveSig]
        int Route([In] int OutputPinIndex, [In] int InputPinIndex);
        [PreserveSig]
        int get_IsRoutedTo([In] int OutputPinIndex, out int InputPinIndex);
        [PreserveSig]
        int get_CrossbarPinInfo([In, MarshalAs(UnmanagedType.Bool)] bool IsInputPin, [In] int PinIndex, out int PinIndexRelated, out PhysicalConnectorType PhysicalType);
    }

    [ComImport, ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("54C39221-8380-11d0-B3F0-00AA003761C5")]
    public interface IAMAudioInputMixer
    {
        int put_Enable([In] bool fEnable);
        int get_Enable(out bool pfEnable);
        int put_Mono([In] bool fMono);
        int get_Mono(out bool pfMono);
        int put_MixLevel([In] double Level);
        int get_MixLevel(out double pLevel);
        int put_Pan([In] double Pan);
        int get_Pan(out double pPan);
        int put_Loudness([In] bool fLoudness);
        int get_Loudness(out bool pfLoudness);
        int put_Treble([In] double Treble);
        int get_Treble(out double pTreble);
        int get_TrebleRange(out double pRange);
        int put_Bass([In] double Bass);
        int get_Bass(out double pBass);
        int get_BassRange(out double pRange);
    }

    public enum VfwCompressDialogs
    {
        About = 2,
        Config = 1,
        QueryAbout = 8,
        QueryConfig = 4
    }

    [ComImport, ComVisible(true), Guid("D8D715A3-6E5E-11D0-B3F0-00AA003761C5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAMVfwCompressDialogs
    {
        [PreserveSig]
        int ShowDialog([In] VfwCompressDialogs iDialog, [In] IntPtr hwnd);
        int GetState([Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pState, ref int pcbState);
        int SetState([In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pState, [In] int cbState);
        int SendDriverMessage(int uMsg, long dw1, long dw2);
    }

    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public class VideoStreamConfigCaps
    {
        public System.Guid Guid;
        public AnalogVideoStandard VideoStandard;
        public Size InputSize;
        public Size MinCroppingSize;
        public Size MaxCroppingSize;
        public int CropGranularityX;
        public int CropGranularityY;
        public int CropAlignX;
        public int CropAlignY;
        public Size MinOutputSize;
        public Size MaxOutputSize;
        public int OutputGranularityX;
        public int OutputGranularityY;
        public int StretchTapsX;
        public int StretchTapsY;
        public int ShrinkTapsX;
        public int ShrinkTapsY;
        public long MinFrameInterval;
        public long MaxFrameInterval;
        public int MinBitsPerSecond;
        public int MaxBitsPerSecond;
    }

    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public class AudioStreamConfigCaps
    {
        public System.Guid Guid;
        public int MinimumChannels;
        public int MaximumChannels;
        public int ChannelsGranularity;
        public int MinimumBitsPerSample;
        public int MaximumBitsPerSample;
        public int BitsPerSampleGranularity;
        public int MinimumSampleFrequency;
        public int MaximumSampleFrequency;
        public int SampleFrequencyGranularity;
    }

}
