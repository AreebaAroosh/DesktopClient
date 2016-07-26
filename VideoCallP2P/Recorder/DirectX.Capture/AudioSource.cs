using System;
using System.Runtime.InteropServices;
using VideoCallP2P.Recorder.DirectShowNET;

namespace VideoCallP2P.Recorder
{
    /// <summary>
    ///  Represents a physical connector or source on an 
    ///  audio device. This class is used on filters that
    ///  support the IAMAudioInputMixer interface such as 
    ///  source cards.
    /// </summary>
    public class AudioSource : Source
    {

        /// <summary> audio mixer interface (COM object) </summary>
        internal IPin Pin;

        internal AudioSource(IPin pin)
        {
            if (!(pin is IAMAudioInputMixer))
            {
                throw new NotSupportedException("The input pin does not support the IAMAudioInputMixer interface");
            }
            this.Pin = pin;
            base.name = this.GetName(pin);
        }

        /// <summary> Enable or disable this source. For audio sources it is 
        /// usually possible to enable several sources. When setting Enabled=true,
        /// set Enabled=false on all other audio sources. </summary>
        public override bool Enabled
        {
            get
            {
                bool flag;
                ((IAMAudioInputMixer)this.Pin).get_Enable(out flag);
                return flag;
            }
            set
            {
                ((IAMAudioInputMixer)this.Pin).put_Enable(value);
            }
        }

        /// <summary> Retrieve the friendly name of a connectorType. </summary>
        private string GetName(IPin pin)
        {
            string str = "Unknown pin";
            PinInfo pinInfo = new PinInfo();

            // Direction matches, so add pin name to listbox
            int errorCode = pin.QueryPinInfo(out pinInfo);
            if (errorCode == 0)
            {
                str = pinInfo.name ?? "";
            }
            else
            {
                Marshal.ThrowExceptionForHR(errorCode);
            }

            // The pininfo structure contains a reference to an IBaseFilter,
            // so you must release its reference to prevent resource a leak.
            if (pinInfo.filter != null)
            {
                Marshal.ReleaseComObject(pinInfo.filter);
            }
            pinInfo.filter = null;
            return str;
        }

        /// <summary> Release unmanaged resources. </summary>
        public override void Dispose()
        {
            if (Pin != null)
            {
                Marshal.ReleaseComObject(Pin);
            }
            Pin = null;
            base.Dispose();
        }
    }
}
