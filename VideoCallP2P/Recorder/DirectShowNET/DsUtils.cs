/******************************************************
                  DirectShow .NET
*******************************************************/
//					DsUtils
// DirectShow utility classes, partial from the SDK Common sources

using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace VideoCallP2P.Recorder.DirectShowNET
{

    /*
     * DsUtils
     * DsPOINT
     * DsRECT
     * BitmapInfoHeader
     * DsROT
     * ISpecifyPropertyPages
     * DsCAUUID
     * DsOptInt64
     * DsOptIntPtr
     */

    [ComVisible(false)]
    public class DsUtils
    {
        public static void FreeAMMediaType(AMMediaType mediaType)
        {
            if (mediaType.formatSize != 0)
            {
                Marshal.FreeCoTaskMem(mediaType.formatPtr);
            }
            if (mediaType.unkPtr != IntPtr.Zero)
            {
                Marshal.Release(mediaType.unkPtr);
            }
            mediaType.formatSize = 0;
            mediaType.formatPtr = IntPtr.Zero;
            mediaType.unkPtr = IntPtr.Zero;
        }

        public int GetPin(IBaseFilter filter, PinDirection dirrequired, int num, out IPin ppPin)
        {
            IEnumPins pins;
            ppPin = null;
            int num2 = filter.EnumPins(out pins);
            if ((num2 >= 0) && (pins != null))
            {
                IPin[] ppPins = new IPin[1];
                do
                {
                    int num3;
                    num2 = pins.Next(1, ppPins, out num3);
                    if ((num2 != 0) || (ppPins[0] == null))
                    {
                        break;
                    }
                    PinDirection pPinDir = (PinDirection)3;
                    num2 = ppPins[0].QueryDirection(out pPinDir);
                    if ((num2 == 0) && (pPinDir == dirrequired))
                    {
                        if (num == 0)
                        {
                            ppPin = ppPins[0];
                            ppPins[0] = null;
                            break;
                        }
                        num--;
                    }
                    Marshal.ReleaseComObject(ppPins[0]);
                    ppPins[0] = null;
                }
                while (num2 == 0);
                Marshal.ReleaseComObject(pins);
                pins = null;
            }
            return num2;
        }

        public static bool IsCorrectDirectXVersion()
        {
            return File.Exists(Path.Combine(Environment.SystemDirectory, "dpnhpast.dll"));
        }

        [DllImport("olepro32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int OleCreatePropertyFrame(IntPtr hwndOwner, int x, int y, string lpszCaption, int cObjects, [In, MarshalAs(UnmanagedType.Interface)] ref object ppUnk, int cPages, IntPtr pPageClsID, int lcid, int dwReserved, IntPtr pvReserved);
        
        public static bool ShowCapPinDialog(ICaptureGraphBuilder2 bld, IBaseFilter flt, IntPtr hwnd)
        {
            object ppint = null;
            ISpecifyPropertyPages pages = null;
            bool flag;
            DsCAUUID pPages = new DsCAUUID();
            try
            {
                Guid capture = PinCategory.Capture;
                Guid interleaved = MediaType.Interleaved;
                Guid gUID = typeof(IAMStreamConfig).GUID;
                if (bld.FindInterface(ref capture, ref interleaved, flt, ref gUID, out ppint) != 0)
                {
                    interleaved = MediaType.Video;
                    if (bld.FindInterface(ref capture, ref interleaved, flt, ref gUID, out ppint) != 0)
                    {
                        return false;
                    }
                }
                pages = ppint as ISpecifyPropertyPages;
                if (pages == null)
                {
                    return false;
                }
                int num = pages.GetPages(out pPages);
                num = OleCreatePropertyFrame(hwnd, 30, 30, null, 1, ref ppint, pPages.cElems, pPages.pElems, 0, 0, IntPtr.Zero);
                flag = true;
            }
            catch (Exception exception)
            {
                Trace.WriteLine("!Ds.NET: ShowCapPinDialog " + exception.Message);
                flag = false;
            }
            finally
            {
                if (pPages.pElems != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pPages.pElems);
                }
                pages = null;
                if (ppint != null)
                {
                    Marshal.ReleaseComObject(ppint);
                }
                ppint = null;
            }
            return flag;
        }

        public static bool ShowTunerPinDialog(ICaptureGraphBuilder2 bld, IBaseFilter flt, IntPtr hwnd)
        {
            object ppint = null;
            ISpecifyPropertyPages pages = null;
            bool flag;
            DsCAUUID pPages = new DsCAUUID();
            try
            {
                Guid capture = PinCategory.Capture;
                Guid interleaved = MediaType.Interleaved;
                Guid gUID = typeof(IAMTVTuner).GUID;
                if (bld.FindInterface(ref capture, ref interleaved, flt, ref gUID, out ppint) != 0)
                {
                    interleaved = MediaType.Video;
                    if (bld.FindInterface(ref capture, ref interleaved, flt, ref gUID, out ppint) != 0)
                    {
                        return false;
                    }
                }
                pages = ppint as ISpecifyPropertyPages;
                if (pages == null)
                {
                    return false;
                }
                int num = pages.GetPages(out pPages);
                num = OleCreatePropertyFrame(hwnd, 30, 30, null, 1, ref ppint, pPages.cElems, pPages.pElems, 0, 0, IntPtr.Zero);
                flag = true;
            }
            catch (Exception exception)
            {
                Trace.WriteLine("!Ds.NET: ShowCapPinDialog " + exception.Message);
                flag = false;
            }
            finally
            {
                if (pPages.pElems != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pPages.pElems);
                }
                pages = null;
                if (ppint != null)
                {
                    Marshal.ReleaseComObject(ppint);
                }
                ppint = null;
            }
            return flag;
        }
    }

    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public struct DsPOINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public struct DsRECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2), ComVisible(false)]
    public struct BitmapInfoHeader
    {
        public int Size;
        public int Width;
        public int Height;
        public short Planes;
        public short BitCount;
        public int Compression;
        public int ImageSize;
        public int XPelsPerMeter;
        public int YPelsPerMeter;
        public int ClrUsed;
        public int ClrImportant;
    }

    [ComVisible(false)]
    public class DsROT
    {
        private const int ROTFLAGS_REGISTRATIONKEEPSALIVE = 1;

        public static bool AddGraphToRot(object graph, out int cookie)
        {
            bool flag;
            cookie = 0;
            int errorCode = 0;
            UCOMIRunningObjectTable pprot = null;
            UCOMIMoniker ppmk = null;
            try
            {
                errorCode = GetRunningObjectTable(0, out pprot);
                if (errorCode < 0)
                {
                    Marshal.ThrowExceptionForHR(errorCode);
                }
                int currentProcessId = GetCurrentProcessId();
                IntPtr iUnknownForObject = Marshal.GetIUnknownForObject(graph);
                int num3 = (int)iUnknownForObject;
                Marshal.Release(iUnknownForObject);
                string item = string.Format("FilterGraph {0} pid {1}", num3.ToString("x8"), currentProcessId.ToString("x8"));
                errorCode = CreateItemMoniker("!", item, out ppmk);
                if (errorCode < 0)
                {
                    Marshal.ThrowExceptionForHR(errorCode);
                }
                pprot.Register(1, graph, ppmk, out cookie);
                flag = true;
            }
            catch (Exception)
            {
                flag = false;
            }
            finally
            {
                if (ppmk != null)
                {
                    Marshal.ReleaseComObject(ppmk);
                }
                ppmk = null;
                if (pprot != null)
                {
                    Marshal.ReleaseComObject(pprot);
                }
                pprot = null;
            }
            return flag;
        }

        [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int CreateItemMoniker(string delim, string item, out UCOMIMoniker ppmk);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern int GetCurrentProcessId();

        [DllImport("ole32.dll", ExactSpelling = true)]
        private static extern int GetRunningObjectTable(int r, out UCOMIRunningObjectTable pprot);

        public static bool RemoveGraphFromRot(ref int cookie)
        {
            UCOMIRunningObjectTable pprot = null;
            bool flag;
            try
            {
                int runningObjectTable = GetRunningObjectTable(0, out pprot);
                if (runningObjectTable < 0)
                {
                    Marshal.ThrowExceptionForHR(runningObjectTable);
                }
                pprot.Revoke(cookie);
                cookie = 0;
                flag = true;
            }
            catch (Exception)
            {
                flag = false;
            }
            finally
            {
                if (pprot != null)
                {
                    Marshal.ReleaseComObject(pprot);
                }
                pprot = null;
            }
            return flag;
        }
    }

    [ComImport, ComVisible(true), Guid("B196B28B-BAB4-101A-B69C-00AA00341D07"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ISpecifyPropertyPages
    {
        [PreserveSig]
        int GetPages(out DsCAUUID pPages);
    }

    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public struct DsCAUUID
    {
        public int cElems;
        public IntPtr pElems;
    }

    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public class DsOptInt64
    {
        public long Value;
        public DsOptInt64(long Value)
        {
            this.Value = Value;
        }
    }

    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public class DsOptIntPtr
    {
        public IntPtr Pointer;
    }

}
