/******************************************************
                  DirectShow .NET
*******************************************************/
//           WORKAROUND FOR DS BUGs

using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Services;

namespace VideoCallP2P.Recorder.DirectShowNET
{

    public class DsBugWO
    {
        [DllImport("ole32.dll")]
        private static extern int CoCreateInstance(ref Guid clsid, IntPtr pUnkOuter, CLSCTX dwClsContext, ref Guid iid, out IntPtr ptrIf);
        public static object CreateDsInstance(ref Guid clsid, ref Guid riid)
        {
            IntPtr ptr;
            IntPtr ptr2;
            int errorCode = CoCreateInstance(ref clsid, IntPtr.Zero, CLSCTX.Inproc, ref riid, out ptr);
            if ((errorCode != 0) || (ptr == IntPtr.Zero))
            {
                Marshal.ThrowExceptionForHR(errorCode);
            }
            Guid iid = new Guid("00000000-0000-0000-C000-000000000046");
            errorCode = Marshal.QueryInterface(ptr, ref iid, out ptr2);
            object obj2 = EnterpriseServicesHelper.WrapIUnknownWithComObject(ptr);
            Marshal.Release(ptr);
            return obj2;
        }
    }

    [Flags]
    internal enum CLSCTX
    {
        All = 0x17,
        Inproc = 3,
        Server = 0x15
    }

}
