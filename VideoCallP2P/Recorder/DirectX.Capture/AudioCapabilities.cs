using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using VideoCallP2P.Recorder.DirectShowNET;

namespace VideoCallP2P.Recorder
{
    public class AudioCapabilities
    {

        /// <summary> Minimum number of audio channels. </summary>
        public int MinimumChannels;

        /// <summary> Maximum number of audio channels. </summary>
        public int MaximumChannels;

        /// <summary> Granularity of the channels. For example, channels 2 through 4, in steps of 2. </summary>
        public int ChannelsGranularity;

        /// <summary> Minimum number of bits per sample. </summary>
        public int MinimumSampleSize;

        /// <summary> Maximum number of bits per sample. </summary>
        public int MaximumSampleSize;

        /// <summary> Granularity of the bits per sample. For example, 8 bits per sample through 32 bits per sample, in steps of 8. </summary>
        public int SampleSizeGranularity;

        /// <summary> Minimum sample frequency. </summary>
        public int MinimumSamplingRate;

        /// <summary> Maximum sample frequency. </summary>
        public int MaximumSamplingRate;

        /// <summary> Granularity of the frequency. For example, 11025 Hz to 44100 Hz, in steps of 11025 Hz. </summary>
        public int SamplingRateGranularity;

        /// <summary> Retrieve capabilities of an audio device </summary>
        internal AudioCapabilities(IAMStreamConfig audioStreamConfig)
        {
            if (audioStreamConfig == null)
                throw new ArgumentNullException("audioStreamConfig");

            AMMediaType mediaType = null;
            AudioStreamConfigCaps caps = null;
            IntPtr pCaps = IntPtr.Zero;

            try
            {
                // Ensure this device reports capabilities
                IntPtr pMediaType;
                int piCount;
                int piSize;

                int numberOfCapabilities = audioStreamConfig.GetNumberOfCapabilities(out piCount, out piSize);
                if (numberOfCapabilities != 0)
                {
                    Marshal.ThrowExceptionForHR(numberOfCapabilities);
                }
                if (piCount <= 0)
                {
                    throw new NotSupportedException("This audio device does not report capabilities.");
                }
                if (piSize > Marshal.SizeOf(typeof(AudioStreamConfigCaps)))
                {
                    throw new NotSupportedException("Unable to retrieve audio device capabilities. This audio device requires a larger AudioStreamConfigCaps structure.");
                }

                // Alloc memory for structure
                pCaps = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(AudioStreamConfigCaps)));

                // Retrieve first (and hopefully only) capabilities struct
                numberOfCapabilities = audioStreamConfig.GetStreamCaps(0, out pMediaType, pCaps);
                if (numberOfCapabilities != 0)
                {
                    Marshal.ThrowExceptionForHR(numberOfCapabilities);
                }

                // Convert pointers to managed structures
                mediaType = (AMMediaType)Marshal.PtrToStructure(pMediaType, typeof(AMMediaType));
                caps = (AudioStreamConfigCaps)Marshal.PtrToStructure(pCaps, typeof(AudioStreamConfigCaps));

                // Extract info
                this.MinimumChannels = caps.MinimumChannels;
                this.MaximumChannels = caps.MaximumChannels;
                this.ChannelsGranularity = caps.ChannelsGranularity;
                this.MinimumSampleSize = caps.MinimumBitsPerSample;
                this.MaximumSampleSize = caps.MaximumBitsPerSample;
                this.SampleSizeGranularity = caps.BitsPerSampleGranularity;
                this.MinimumSamplingRate = caps.MinimumSampleFrequency;
                this.MaximumSamplingRate = caps.MaximumSampleFrequency;
                this.SamplingRateGranularity = caps.SampleFrequencyGranularity;

            }
            finally
            {
                if (pCaps != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pCaps);
                }
                pCaps = IntPtr.Zero;
                if (mediaType != null)
                {
                    DsUtils.FreeAMMediaType(mediaType);
                }
                mediaType = null;
            }
        }
    }
}
