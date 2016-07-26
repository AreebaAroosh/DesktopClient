using System;
using System.Runtime.InteropServices;
using VideoCallP2P.Recorder.DirectShowNET;

namespace VideoCallP2P.Recorder
{

    /// <summary>
    ///  Control and query a hardware TV Tuner.
    /// </summary>
    public class Tuner : IDisposable
    {

        protected IAMTVTuner tvTuner = null;

        public Tuner(IAMTVTuner tuner)
        {
            tvTuner = tuner;
        }

        /// <summary>
        ///  Get or set the TV Tuner channel.
        /// </summary>
        public int Channel
        {
            get
            {
                int channel;
                int v; 
                int a;
                int errorCode = this.tvTuner.get_Channel(out channel, out v, out a);
                if (errorCode != 0)
                {
                    Marshal.ThrowExceptionForHR(errorCode);
                }
                return channel;
            }

            set
            {
                int errorCode = tvTuner.put_Channel(value, AMTunerSubChannel.Default, AMTunerSubChannel.Default);
                if (errorCode != 0) 
                { 
                    Marshal.ThrowExceptionForHR(errorCode); 
                }
            }
        }

        /// <summary>
        ///  Get or set the tuner frequency (cable or antenna).
        /// </summary>
        public TunerInputType InputType
        {
            get
            {
                TunerInputType type;
                int errorCode = this.tvTuner.get_InputType(0, out type);
                if (errorCode != 0)
                {
                    Marshal.ThrowExceptionForHR(errorCode);
                }
                return (TunerInputType)type;
            }
            set
            {
                TunerInputType inputType = (TunerInputType)value;
                int errorCode = this.tvTuner.put_InputType(0, inputType);
                if (errorCode != 0)
                {
                    Marshal.ThrowExceptionForHR(errorCode);
                }
            }
        }

        /// <summary>
        ///  Indicates whether a signal is present on the current channel.
        ///  If the signal strength cannot be determined, a NotSupportedException
        ///  is thrown.
        /// </summary>
        public bool SignalPresent
        {
            get
            {
                AMTunerSignalStrength strength;
                int errorCode = this.tvTuner.SignalPresent(out strength);
                if (errorCode != 0)
                {
                    Marshal.ThrowExceptionForHR(errorCode);
                }
                if (strength == AMTunerSignalStrength.NA)
                {
                    throw new NotSupportedException("Signal strength not available.");
                }
                return (strength == AMTunerSignalStrength.SignalPresent);
            }
        }

        public void Dispose()
        {
            if (tvTuner != null)
            {
                Marshal.ReleaseComObject(tvTuner);
            }
            tvTuner = null;
        }
    }
}
