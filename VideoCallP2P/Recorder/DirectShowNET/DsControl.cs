/******************************************************
                  DirectShow .NET
*******************************************************/
//					DsControl
// basic Quartz control interfaces, ported from control.odl

using System;
using System.Runtime.InteropServices;

namespace VideoCallP2P.Recorder.DirectShowNET
{

    /*
     * IMediaControl
     * IMediaEvent
     * IMediaEventEx
     * IBasicVideo2
     * IVideoWindow
     * IMediaPosition
     * IBasicAudio
     * IAMCollection
     * DsEvCode
     */

    [ComImport, Guid("56a868b1-0ad4-11ce-b03a-0020af0ba770"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaControl
    {
        [PreserveSig]
        int Run();
        [PreserveSig]
        int Pause();
        [PreserveSig]
        int Stop();
        [PreserveSig]
        int GetState(int msTimeout, out int pfs);
        [PreserveSig]
        int RenderFile(string strFilename);
        [PreserveSig]
        int AddSourceFilter([In] string strFilename, [MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);
        [PreserveSig]
        int get_FilterCollection([MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);
        [PreserveSig]
        int get_RegFilterCollection([MarshalAs(UnmanagedType.IDispatch)] out object ppUnk);
        [PreserveSig]
        int StopWhenReady();
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), ComVisible(true), Guid("56a868b6-0ad4-11ce-b03a-0020af0ba770")]
    public interface IMediaEvent
    {
        [PreserveSig]
        int GetEventHandle(out IntPtr hEvent);
        [PreserveSig]
        int GetEvent(out DsEvCode lEventCode, out int lParam1, out int lParam2, int msTimeout);
        [PreserveSig]
        int WaitForCompletion(int msTimeout, out int pEvCode);
        [PreserveSig]
        int CancelDefaultHandling(int lEvCode);
        [PreserveSig]
        int RestoreDefaultHandling(int lEvCode);
        [PreserveSig]
        int FreeEventParams(DsEvCode lEvCode, int lParam1, int lParam2);
    }

    [ComImport, ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("56a868c0-0ad4-11ce-b03a-0020af0ba770")]
    public interface IMediaEventEx
    {
        [PreserveSig]
        int GetEventHandle(out IntPtr hEvent);
        [PreserveSig]
        int GetEvent(out DsEvCode lEventCode, out int lParam1, out int lParam2, int msTimeout);
        [PreserveSig]
        int WaitForCompletion(int msTimeout, out int pEvCode);
        [PreserveSig]
        int CancelDefaultHandling(int lEvCode);
        [PreserveSig]
        int RestoreDefaultHandling(int lEvCode);
        [PreserveSig]
        int FreeEventParams(DsEvCode lEvCode, int lParam1, int lParam2);
        [PreserveSig]
        int SetNotifyWindow(IntPtr hwnd, int lMsg, IntPtr lInstanceData);
        [PreserveSig]
        int SetNotifyFlags(int lNoNotifyFlags);
        [PreserveSig]
        int GetNotifyFlags(out int lplNoNotifyFlags);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("329bb360-f6ea-11d1-9038-00a0c9697298"), ComVisible(true)]
    public interface IBasicVideo2
    {
        [PreserveSig]
        int AvgTimePerFrame(out double pAvgTimePerFrame);
        [PreserveSig]
        int BitRate(out int pBitRate);
        [PreserveSig]
        int BitErrorRate(out int pBitRate);
        [PreserveSig]
        int VideoWidth(out int pVideoWidth);
        [PreserveSig]
        int VideoHeight(out int pVideoHeight);
        [PreserveSig]
        int put_SourceLeft(int SourceLeft);
        [PreserveSig]
        int get_SourceLeft(out int pSourceLeft);
        [PreserveSig]
        int put_SourceWidth(int SourceWidth);
        [PreserveSig]
        int get_SourceWidth(out int pSourceWidth);
        [PreserveSig]
        int put_SourceTop(int SourceTop);
        [PreserveSig]
        int get_SourceTop(out int pSourceTop);
        [PreserveSig]
        int put_SourceHeight(int SourceHeight);
        [PreserveSig]
        int get_SourceHeight(out int pSourceHeight);
        [PreserveSig]
        int put_DestinationLeft(int DestinationLeft);
        [PreserveSig]
        int get_DestinationLeft(out int pDestinationLeft);
        [PreserveSig]
        int put_DestinationWidth(int DestinationWidth);
        [PreserveSig]
        int get_DestinationWidth(out int pDestinationWidth);
        [PreserveSig]
        int put_DestinationTop(int DestinationTop);
        [PreserveSig]
        int get_DestinationTop(out int pDestinationTop);
        [PreserveSig]
        int put_DestinationHeight(int DestinationHeight);
        [PreserveSig]
        int get_DestinationHeight(out int pDestinationHeight);
        [PreserveSig]
        int SetSourcePosition(int left, int top, int width, int height);
        [PreserveSig]
        int GetSourcePosition(out int left, out int top, out int width, out int height);
        [PreserveSig]
        int SetDefaultSourcePosition();
        [PreserveSig]
        int SetDestinationPosition(int left, int top, int width, int height);
        [PreserveSig]
        int GetDestinationPosition(out int left, out int top, out int width, out int height);
        [PreserveSig]
        int SetDefaultDestinationPosition();
        [PreserveSig]
        int GetVideoSize(out int pWidth, out int pHeight);
        [PreserveSig]
        int GetVideoPaletteEntries(int StartIndex, int Entries, out int pRetrieved, IntPtr pPalette);
        [PreserveSig]
        int GetCurrentImage(ref int pBufferSize, IntPtr pDIBImage);
        [PreserveSig]
        int IsUsingDefaultSource();
        [PreserveSig]
        int IsUsingDefaultDestination();
        [PreserveSig]
        int GetPreferredAspectRatio(out int plAspectX, out int plAspectY);
    }

    [ComImport, Guid("56a868b4-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsDual), ComVisible(true)]
    public interface IVideoWindow
    {
        [PreserveSig]
        int put_Caption(string caption);
        [PreserveSig]
        int get_Caption(out string caption);
        [PreserveSig]
        int put_WindowStyle(int windowStyle);
        [PreserveSig]
        int get_WindowStyle(out int windowStyle);
        [PreserveSig]
        int put_WindowStyleEx(int windowStyleEx);
        [PreserveSig]
        int get_WindowStyleEx(out int windowStyleEx);
        [PreserveSig]
        int put_AutoShow(int autoShow);
        [PreserveSig]
        int get_AutoShow(out int autoShow);
        [PreserveSig]
        int put_WindowState(int windowState);
        [PreserveSig]
        int get_WindowState(out int windowState);
        [PreserveSig]
        int put_BackgroundPalette(int backgroundPalette);
        [PreserveSig]
        int get_BackgroundPalette(out int backgroundPalette);
        [PreserveSig]
        int put_Visible(int visible);
        [PreserveSig]
        int get_Visible(out int visible);
        [PreserveSig]
        int put_Left(int left);
        [PreserveSig]
        int get_Left(out int left);
        [PreserveSig]
        int put_Width(int width);
        [PreserveSig]
        int get_Width(out int width);
        [PreserveSig]
        int put_Top(int top);
        [PreserveSig]
        int get_Top(out int top);
        [PreserveSig]
        int put_Height(int height);
        [PreserveSig]
        int get_Height(out int height);
        [PreserveSig]
        int put_Owner(IntPtr owner);
        [PreserveSig]
        int get_Owner(out IntPtr owner);
        [PreserveSig]
        int put_MessageDrain(IntPtr drain);
        [PreserveSig]
        int get_MessageDrain(out IntPtr drain);
        [PreserveSig]
        int get_BorderColor(out int color);
        [PreserveSig]
        int put_BorderColor(int color);
        [PreserveSig]
        int get_FullScreenMode(out int fullScreenMode);
        [PreserveSig]
        int put_FullScreenMode(int fullScreenMode);
        [PreserveSig]
        int SetWindowForeground(int focus);
        [PreserveSig]
        int NotifyOwnerMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        [PreserveSig]
        int SetWindowPosition(int left, int top, int width, int height);
        [PreserveSig]
        int GetWindowPosition(out int left, out int top, out int width, out int height);
        [PreserveSig]
        int GetMinIdealImageSize(out int width, out int height);
        [PreserveSig]
        int GetMaxIdealImageSize(out int width, out int height);
        [PreserveSig]
        int GetRestorePosition(out int left, out int top, out int width, out int height);
        [PreserveSig]
        int HideCursor(int hideCursor);
        [PreserveSig]
        int IsCursorHidden(out int hideCursor);
    }

    [ComImport, ComVisible(true), Guid("56a868b2-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMediaPosition
    {
        [PreserveSig]
        int get_Duration(out double pLength);
        [PreserveSig]
        int put_CurrentPosition(double llTime);
        [PreserveSig]
        int get_CurrentPosition(out double pllTime);
        [PreserveSig]
        int get_StopTime(out double pllTime);
        [PreserveSig]
        int put_StopTime(double llTime);
        [PreserveSig]
        int get_PrerollTime(out double pllTime);
        [PreserveSig]
        int put_PrerollTime(double llTime);
        [PreserveSig]
        int put_Rate(double dRate);
        [PreserveSig]
        int get_Rate(out double pdRate);
        [PreserveSig]
        int CanSeekForward(out int pCanSeekForward);
        [PreserveSig]
        int CanSeekBackward(out int pCanSeekBackward);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("56a868b3-0ad4-11ce-b03a-0020af0ba770"), ComVisible(true)]
    public interface IBasicAudio
    {
        [PreserveSig]
        int put_Volume(int lVolume);
        [PreserveSig]
        int get_Volume(out int plVolume);
        [PreserveSig]
        int put_Balance(int lBalance);
        [PreserveSig]
        int get_Balance(out int plBalance);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("56a868b9-0ad4-11ce-b03a-0020af0ba770"), ComVisible(true)]
    public interface IAMCollection
    {
        [PreserveSig]
        int get_Count(out int plCount);
        [PreserveSig]
        int Item(int lItem, [MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);
        [PreserveSig]
        int get_NewEnum([MarshalAs(UnmanagedType.IUnknown)] out object ppUnk);
    }

    public enum DsEvCode
    {
        Activate = 0x13,
        BufferingData = 0x11,
        ClockChanged = 13,
        Complete = 1,
        DisplayChanged = 0x16,
        DvdAngleChange = 0x106,
        DvdAnglesAvail = 0x113,
        DvdAudioStChange = 260,
        DvdButtonAActivated = 0x115,
        DvdButtonChange = 0x107,
        DvdChaptAutoStop = 270,
        DvdChaptStart = 0x103,
        DvdCmdEnd = 0x117,
        DvdCmdStart = 0x116,
        DvdCurrentHmsfTime = 0x11a,
        DvdCurrentTime = 0x10b,
        DvdDiscEjected = 280,
        DvdDiscInserted = 0x119,
        DvdDomChange = 0x101,
        DvdError = 0x10c,
        DvdKaraokeMode = 0x11b,
        DvdNoFpPgc = 0x10f,
        DvdParentalLChange = 0x111,
        DvdPeriodAStop = 0x114,
        DvdPlaybRateChange = 0x110,
        DvdPlaybStopped = 0x112,
        DvdStillOff = 0x10a,
        DvdStillOn = 0x109,
        DvdSubPicStChange = 0x105,
        DvdTitleChange = 0x102,
        DvdValidUopsChange = 0x108,
        DvdWarning = 0x10d,
        ErrorAbort = 3,
        ErrorStPlaying = 8,
        FullScreenLost = 0x12,
        NeedRestart = 20,
        None = 0,
        NotifyWindow = 0x19,
        OleEvent = 0x18,
        OpeningFile = 0x10,
        PaletteChanged = 9,
        Paused = 14,
        QualityChange = 11,
        Repaint = 5,
        ShuttingDown = 12,
        Starvation = 0x17,
        StErrStopped = 6,
        StErrStPlaying = 7,
        Time = 4,
        UserAbort = 2,
        VideoSizeChanged = 10,
        WindowDestroyed = 0x15
    }


}
