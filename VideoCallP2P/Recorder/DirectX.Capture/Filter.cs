// ------------------------------------------------------------------
// DirectX.Capture
//
// History:
//	2003-Jan-24		BL		- created
//
// Copyright (c) 2003 Brian Low
// ------------------------------------------------------------------

using System;
using System.Collections;
using System.Runtime.InteropServices;

using VideoCallP2P.Recorder.DirectShowNET;

namespace VideoCallP2P.Recorder
{
    /// <summary>
    ///  Represents a DirectShow filter (e.g. video capture device, 
    ///  compression codec).
    /// </summary>
    /// <remarks>
    ///  To save a chosen filer for later recall
    ///  save the MonikerString property on the filter: 
    ///  <code><div style="background-color:whitesmoke;">
    ///   string savedMonikerString = myFilter.MonikerString;
    ///  </div></code>
    ///  
    ///  To recall the filter create a new Filter class and pass the 
    ///  string to the constructor: 
    ///  <code><div style="background-color:whitesmoke;">
    ///   Filter mySelectedFilter = new Filter( savedMonikerString );
    ///  </div></code>
    /// </remarks>
    public class Filter : IComparable
    {
        /// <summary> Human-readable name of the filter </summary>
        public string Name;

        /// <summary> Unique string referencing this filter. This string can be used to recreate this filter. </summary>
        public string MonikerString;


        public Filter(string monikerString)
        {
            this.Name = GetName(monikerString);
            this.MonikerString = monikerString;
        }

        /// <summary> Create a new filter from its moniker </summary>
        internal Filter(UCOMIMoniker moniker)
        {
            this.Name = this.GetName(moniker);
            this.MonikerString = this.GetMonikerString(moniker);
        }

        /// <summary>
        ///  This method gets a UCOMIMoniker object.
        /// 
        ///  HACK: The only way to create a UCOMIMoniker from a moniker 
        ///  string is to use UCOMIMoniker.ParseDisplayName(). So I 
        ///  need ANY UCOMIMoniker object so that I can call 
        ///  ParseDisplayName(). Does anyone have a better solution?
        /// 
        ///  This assumes there is at least one video compressor filter
        ///  installed on the system.
        /// </summary>
        protected UCOMIMoniker getAnyMoniker()
        {
            Guid videoCompressorCategory = FilterCategory.VideoCompressorCategory;
            int hr;
            object comObj = null;
            ICreateDevEnum enumDev = null;
            UCOMIEnumMoniker enumMon = null;
            UCOMIMoniker[] mon = new UCOMIMoniker[1];

            try
            {
                // Get the system device enumerator
                Type typeFromCLSID = Type.GetTypeFromCLSID(Clsid.SystemDeviceEnum);
                if (typeFromCLSID == null)
                {
                    throw new NotImplementedException("System Device Enumerator");
                }
                comObj = Activator.CreateInstance(typeFromCLSID);
                enumDev = (ICreateDevEnum)comObj;

                // Create an enumerator to find filters in category
                hr = enumDev.CreateClassEnumerator(ref videoCompressorCategory, out enumMon, 0);
                if (hr != 0)
                {
                    throw new NotSupportedException("No devices of the category");
                }

                // Get first filter
                int f;
                hr = enumMon.Next(1, mon, out f);
                if ((hr != 0))
                {
                    mon[0] = null;
                }
                return mon[0];
            }
            finally
            {
                enumDev = null;
                if (enumMon != null)
                {
                    Marshal.ReleaseComObject(enumMon);
                }
                enumMon = null;
                if (comObj != null)
                {
                    Marshal.ReleaseComObject(comObj);
                }
                comObj = null;
            }
        }

        /// <summary> Retrieve the a moniker's display name (i.e. it's unique string) </summary>
        protected string GetMonikerString(UCOMIMoniker moniker)
        {
            string str;
            moniker.GetDisplayName(null, null, out str);
            return str;
        }

        /// <summary> Retrieve the human-readable name of the filter </summary>
        protected string GetName(UCOMIMoniker moniker)
        {
            object ppvObj = null;
            IPropertyBag bag = null;
            string str2;
            try
            {
                Guid gUID = typeof(IPropertyBag).GUID;
                moniker.BindToStorage(null, null, ref gUID, out ppvObj);
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
                str2 = "";
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

        /// <summary> Get a moniker's human-readable name based on a moniker string. </summary>
        protected string GetName(string monikerString)
        {
            UCOMIMoniker moniker = null;
            UCOMIMoniker ppmkOut = null;
            string str;
            try
            {
                int num;
                moniker = this.getAnyMoniker();
                moniker.ParseDisplayName(null, null, monikerString, out num, out ppmkOut);
                str = this.GetName(moniker);
            }
            finally
            {
                if (moniker != null)
                {
                    Marshal.ReleaseComObject(moniker);
                }
                moniker = null;
                if (ppmkOut != null)
                {
                    Marshal.ReleaseComObject(ppmkOut);
                }
                ppmkOut = null;
            }
            return str;
        }
        

        /// <summary>
        ///  Compares the current instance with another object of 
        ///  the same type.
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            Filter filter = (Filter)obj;
            return this.Name.CompareTo(filter.Name);
        }

    }
}
