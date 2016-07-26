using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace VideoCallP2P.Recorder
{
    /// <summary>
    ///  A base class for representing property pages exposed by filters. 
    /// </summary>
    public class PropertyPage : IDisposable
    {

        /// <summary> Name of property page. This name may not be unique </summary>
        public string Name;

        /// <summary> Does this property page support saving and loading the user's choices. </summary>
        public bool SupportsPersisting = false;

        /// <summary> 
        ///  Show the property page. Some property pages cannot be displayed 
        ///  while previewing and/or capturing. This method will block until
        ///  the property page is closed by the user.
        /// </summary>
        public virtual void Show(Control owner)
        {
            throw new NotSupportedException("Not implemented. Use a derived class. ");
        }

        public virtual byte[] State
        {
            get
            {
                throw new NotSupportedException("This property page does not support persisting state.");
            }
            set
            {
                throw new NotSupportedException("This property page does not support persisting state.");
            }
        }

        public void Dispose()
        {
        }

    }
}
