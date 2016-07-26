using System;
using System.Runtime.InteropServices;
using VideoCallP2P.Recorder.DirectShowNET;

namespace VideoCallP2P.Recorder
{
    /// <summary>
    ///  Represents a physical connector or source on an 
    ///  audio/video device. This class is used on filters that
    ///  support the IAMCrossbar interface such as TV Tuners.
    /// </summary>
    public class CrossbarSource : Source
    {

        internal IAMCrossbar Crossbar;			// crossbar filter (COM object)
        internal int OutputPin;			// output pin number on the crossbar
        internal int InputPin;			// input pin number on the crossbar
        internal PhysicalConnectorType ConnectorType;		// type of the connector

        /// <summary> Constructor. This class cannot be created directly. </summary>
        internal CrossbarSource(IAMCrossbar crossbar, int outputPin, int inputPin, PhysicalConnectorType connectorType)
        {
            this.Crossbar = crossbar;
            this.OutputPin = outputPin;
            this.InputPin = inputPin;
            this.ConnectorType = connectorType;
            this.name = GetName(connectorType);
        }

        /// <summary> Retrieve the friendly name of a connectorType. </summary>
        private string GetName(PhysicalConnectorType connectorType)
        {
            switch (connectorType)
            {
                case PhysicalConnectorType.Video_Tuner:
                    return "Video Tuner";

                case PhysicalConnectorType.Video_Composite:
                    return "Video Composite";

                case PhysicalConnectorType.Video_SVideo:
                    return "Video S-Video";

                case PhysicalConnectorType.Video_RGB:
                    return "Video RGB";

                case PhysicalConnectorType.Video_YRYBY:
                    return "Video YRYBY";

                case PhysicalConnectorType.Video_SerialDigital:
                    return "Video Serial Digital";

                case PhysicalConnectorType.Video_ParallelDigital:
                    return "Video Parallel Digital";

                case PhysicalConnectorType.Video_SCSI:
                    return "Video SCSI";

                case PhysicalConnectorType.Video_AUX:
                    return "Video AUX";

                case PhysicalConnectorType.Video_1394:
                    return "Video Firewire";

                case PhysicalConnectorType.Video_USB:
                    return "Video USB";

                case PhysicalConnectorType.Video_VideoDecoder:
                    return "Video Decoder";

                case PhysicalConnectorType.Video_VideoEncoder:
                    return "Video Encoder";

                case PhysicalConnectorType.Video_SCART:
                    return "Video SCART";

                case PhysicalConnectorType.Audio_Tuner:
                    return "Audio Tuner";

                case PhysicalConnectorType.Audio_Line:
                    return "Audio Line In";

                case PhysicalConnectorType.Audio_Mic:
                    return "Audio Mic";

                case PhysicalConnectorType.Audio_AESDigital:
                    return "Audio AES Digital";

                case PhysicalConnectorType.Audio_SPDIFDigital:
                    return "Audio SPDIF Digital";

                case PhysicalConnectorType.Audio_SCSI:
                    return "Audio SCSI";

                case PhysicalConnectorType.Audio_AUX:
                    return "Audio AUX";

                case PhysicalConnectorType.Audio_1394:
                    return "Audio Firewire";

                case PhysicalConnectorType.Audio_USB:
                    return "Audio USB";

                case PhysicalConnectorType.Audio_AudioDecoder:
                    return "Audio Decoder";
            }
            return "Unknown Connector";
        }

        /// <summary> Enabled or disable this source. </summary>
        public override bool Enabled
        {
            get
            {
                int num;
                if (Crossbar.get_IsRoutedTo(OutputPin, out num) == 0 && InputPin == num)
                {
                    return true;
                }
                return false;
            }

            set
            {
                if (value)
                {
                    // Enable this route
                    int errorCode = this.Crossbar.Route(this.OutputPin, this.InputPin);
                    if (errorCode < 0)
                        Marshal.ThrowExceptionForHR(errorCode);
                }
                else
                {
                    // Disable this route by routing the output
                    // pin to input pin -1
                    int errorCode = this.Crossbar.Route(this.OutputPin, -1);
                    if (errorCode < 0)
                        Marshal.ThrowExceptionForHR(errorCode);
                }
            }
        }

        /// <summary> Release unmanaged resources. </summary>
        public override void Dispose()
        {
            if (this.Crossbar != null)
            {
                Marshal.ReleaseComObject(this.Crossbar);
            }
            this.Crossbar = null;
            base.Dispose();
        }
    }
}
