/******************************************************
                  DirectShow .NET
*******************************************************/
//					DsDevice
// enumerate directshow devices and moniker handling


using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace VideoCallP2P.Recorder.DirectShowNET
{

    /*
     * DsDev
     * DsDevice
     * ICreateDevEnum
     * IPropertyBag
     */

    [ComVisible(false)]
    public class DsDev
    {
        public static bool GetDevicesOfCat(Guid cat, out ArrayList devs)
        {
            bool flag;
            devs = null;
            object o = null;
            ICreateDevEnum enum2 = null;
            UCOMIEnumMoniker ppEnumMoniker = null;
            UCOMIMoniker[] rgelt = new UCOMIMoniker[1];
            try
            {
                Type typeFromCLSID = Type.GetTypeFromCLSID(Clsid.SystemDeviceEnum);
                if (typeFromCLSID == null)
                {
                    throw new NotImplementedException("System Device Enumerator");
                }
                o = Activator.CreateInstance(typeFromCLSID);
                enum2 = (ICreateDevEnum)o;
                if (enum2.CreateClassEnumerator(ref cat, out ppEnumMoniker, 0) != 0)
                {
                    throw new NotSupportedException("No devices of the category");
                }
                int num3 = 0;
                while (true)
                {
                    int num2;
                    if ((ppEnumMoniker.Next(1, rgelt, out num2) != 0) || (rgelt[0] == null))
                    {
                        break;
                    }
                    DsDevice device = new DsDevice
                    {
                        Name = GetFriendlyName(rgelt[0])
                    };
                    if (devs == null)
                    {
                        devs = new ArrayList();
                    }
                    device.Mon = rgelt[0];
                    rgelt[0] = null;
                    devs.Add(device);
                    device = null;
                    num3++;
                }
                flag = num3 > 0;
            }
            catch (Exception)
            {
                if (devs != null)
                {
                    foreach (DsDevice device2 in devs)
                    {
                        device2.Dispose();
                    }
                    devs = null;
                }
                flag = false;
            }
            finally
            {
                enum2 = null;
                if (rgelt[0] != null)
                {
                    Marshal.ReleaseComObject(rgelt[0]);
                }
                rgelt[0] = null;
                if (ppEnumMoniker != null)
                {
                    Marshal.ReleaseComObject(ppEnumMoniker);
                }
                ppEnumMoniker = null;
                if (o != null)
                {
                    Marshal.ReleaseComObject(o);
                }
                o = null;
            }
            return flag;
        }

        private static string GetFriendlyName(UCOMIMoniker mon)
        {
            object ppvObj = null;
            IPropertyBag bag = null;
            string str2;
            try
            {
                Guid gUID = typeof(IPropertyBag).GUID;
                mon.BindToStorage(null, null, ref gUID, out ppvObj);
                bag = (IPropertyBag)ppvObj;
                object pVar = "";
                int errorCode = bag.Read("FriendlyName", ref pVar, IntPtr.Zero);
                if (errorCode != 0)
                {
                    Marshal.ThrowExceptionForHR(errorCode);
                }
                string str = pVar as string;
                if ((str == null) || (str.Length < 1))
                {
                    throw new NotImplementedException("Device FriendlyName");
                }
                str2 = str;
            }
            catch (Exception)
            {
                str2 = null;
            }
            finally
            {
                bag = null;
                if (ppvObj != null)
                {
                    Marshal.ReleaseComObject(ppvObj);
                }
                ppvObj = null;
            }
            return str2;
        }
    }

    [ComVisible(false)]
    public class DsDevice : IDisposable
    {
        public UCOMIMoniker Mon;
        public string Name;

        public void Dispose()
        {
            if (this.Mon != null)
            {
                Marshal.ReleaseComObject(this.Mon);
            }
            this.Mon = null;
        }
    }

    [ComImport, Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"), ComVisible(true), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface ICreateDevEnum
    {
        [PreserveSig]
        int CreateClassEnumerator([In] ref Guid pType, out UCOMIEnumMoniker ppEnumMoniker, [In] int dwFlags);
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), ComVisible(true), Guid("55272A00-42CB-11CE-8135-00AA004BB851")]
    public interface IPropertyBag
    {
        [PreserveSig]
        int Read([In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName, [In, Out, MarshalAs(UnmanagedType.Struct)] ref object pVar, IntPtr pErrorLog);
        [PreserveSig]
        int Write([In, MarshalAs(UnmanagedType.LPWStr)] string pszPropName, [In, MarshalAs(UnmanagedType.Struct)] ref object pVar);
    }

}
