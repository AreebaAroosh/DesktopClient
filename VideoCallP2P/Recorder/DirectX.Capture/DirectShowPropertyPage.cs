using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VideoCallP2P.Recorder.DirectShowNET;

namespace VideoCallP2P.Recorder
{
    /// <summary>
    ///  Property pages for a DirectShow filter (e.g. hardware device). These
    ///  property pages do not support persisting their settings. 
    /// </summary>
    public class DirectShowPropertyPage : PropertyPage
    {

        /// <summary> COM ISpecifyPropertyPages interface </summary>
        protected ISpecifyPropertyPages specifyPropertyPages;

        public DirectShowPropertyPage(string name, ISpecifyPropertyPages specifyPropertyPages)
        {
            base.Name = name;
            base.SupportsPersisting = false;
            this.specifyPropertyPages = specifyPropertyPages;
        }

        /// <summary> 
        ///  Show the property page. Some property pages cannot be displayed 
        ///  while previewing and/or capturing. 
        /// </summary>
        public override void Show(Control owner)
        {
            DsCAUUID pPages = new DsCAUUID();
            try
            {
                int pages = this.specifyPropertyPages.GetPages(out pPages);
                if (pages != 0)
                {
                    Marshal.ThrowExceptionForHR(pages);
                }
                object specifyPropertyPages = this.specifyPropertyPages;
                pages = OleCreatePropertyFrame(owner.Handle, 30, 30, null, 1, ref specifyPropertyPages, pPages.cElems, pPages.pElems, 0, 0, IntPtr.Zero);
            }
            finally
            {
                if (pPages.pElems != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pPages.pElems);
                }
            }
        }

        /// <summary> Release unmanaged resources </summary>
        public void Dispose()
        {
            if (this.specifyPropertyPages != null)
            {
                Marshal.ReleaseComObject(this.specifyPropertyPages);
            }
            this.specifyPropertyPages = null;
        }

        [DllImport("olepro32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int OleCreatePropertyFrame(IntPtr hwndOwner, int x, int y, string lpszCaption, int cObjects, [In, MarshalAs(UnmanagedType.Interface)] ref object ppUnk, int cPages, IntPtr pPageClsID, int lcid, int dwReserved, IntPtr pvReserved);
        

    }
}
