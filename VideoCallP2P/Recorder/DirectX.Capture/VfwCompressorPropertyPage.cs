using System.Runtime.InteropServices;
using System.Windows.Forms;
using VideoCallP2P.Recorder.DirectShowNET;

namespace VideoCallP2P.Recorder
{
    /// <summary>
    ///  The property page to configure a Video for Windows compliant
    ///  compression codec. Most compressors support this property page
    ///  rather than a DirectShow property page. Also, most compressors
    ///  do not support the IAMVideoCompression interface so this
    ///  property page is the only method to configure a compressor. 
    /// </summary>
    public class VfwCompressorPropertyPage : PropertyPage
    {
        /// <summary> Video for Windows compression dialog interface </summary>
        protected IAMVfwCompressDialogs vfwCompressDialogs = null;

        public VfwCompressorPropertyPage(string name, IAMVfwCompressDialogs compressDialogs)
        {
            base.Name = name;
            base.SupportsPersisting = true;
            this.vfwCompressDialogs = compressDialogs;
        }

        /// <summary> 
        ///  Get or set the state of the property page. This is used to save
        ///  and restore the user's choices without redisplaying the property page.
        ///  This property will be null if unable to retrieve the property page's
        ///  state.
        /// </summary>
        /// <remarks>
        ///  After showing this property page, read and store the value of 
        ///  this property. At a later time, the user's choices can be 
        ///  reloaded by setting this property with the value stored earlier. 
        ///  Note that some property pages, after setting this property, 
        ///  will not reflect the new state. However, the filter will use the
        ///  new settings.
        /// </remarks>
        public override byte[] State
        {
            get
            {
                byte[] pState = null;
                int pcbState = 0;
                if ((this.vfwCompressDialogs.GetState(null, ref pcbState) == 0) && (pcbState > 0))
                {
                    pState = new byte[pcbState];
                    if (this.vfwCompressDialogs.GetState(pState, ref pcbState) != 0)
                    {
                        pState = null;
                    }
                }
                return pState;
            }
            set
            {
                int errorCode = this.vfwCompressDialogs.SetState(value, value.Length);
                if (errorCode != 0)
                {
                    Marshal.ThrowExceptionForHR(errorCode);
                }
            }
        }

        /// <summary> 
        ///  Show the property page. Some property pages cannot be displayed 
        ///  while previewing and/or capturing. 
        /// </summary>
        public override void Show(Control owner)
        {
            this.vfwCompressDialogs.ShowDialog(VfwCompressDialogs.Config, owner.Handle);
        }

    }
}
