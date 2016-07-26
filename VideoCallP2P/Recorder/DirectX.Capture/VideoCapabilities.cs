// ------------------------------------------------------------------
// DirectX.Capture
//
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using VideoCallP2P.Recorder.DirectShowNET;

namespace VideoCallP2P.Recorder
{
    /// <summary>
    ///  Capabilities of the video device such as 
    ///  min/max frame size and frame rate.
    /// </summary>
    public class VideoCapabilities
    {

        /// <summary> Native size of the incoming video signal. This is the largest signal the filter can digitize with every pixel remaining unique. Read-only. </summary>
        public Size InputSize;

        /// <summary> Minimum supported frame size. Read-only. </summary>
        public Size MinFrameSize;

        /// <summary> Maximum supported frame size. Read-only. </summary>
        public Size MaxFrameSize;

        /// <summary> Granularity of the output width. This value specifies the increments that are valid between MinFrameSize and MaxFrameSize. Read-only. </summary>
        public int FrameSizeGranularityX;

        /// <summary> Granularity of the output height. This value specifies the increments that are valid between MinFrameSize and MaxFrameSize. Read-only. </summary>
        public int FrameSizeGranularityY;

        /// <summary> Minimum supported frame rate. Read-only. </summary>
        public double MinFrameRate;

        /// <summary> Maximum supported frame rate. Read-only. </summary>
        public double MaxFrameRate;


        internal VideoCapabilities(IAMStreamConfig videoStreamConfig)
        {
            if (videoStreamConfig == null)
                throw new ArgumentNullException("videoStreamConfig");

            AMMediaType mediaType = null;
            VideoStreamConfigCaps caps = null;
            IntPtr pCaps = IntPtr.Zero;
            IntPtr pMediaType;
            try
            {
                // Ensure this device reports capabilities
                int c, size;
                int numberOfCapabilities = videoStreamConfig.GetNumberOfCapabilities(out c, out size);
                if (numberOfCapabilities != 0) 
                { 
                    Marshal.ThrowExceptionForHR(numberOfCapabilities); 
                }
                if (c <= 0)
                {
                    throw new NotSupportedException("This video device does not report capabilities.");
                }
                if (size > Marshal.SizeOf(typeof(VideoStreamConfigCaps)))
                {
                    throw new NotSupportedException("Unable to retrieve video device capabilities. This video device requires a larger VideoStreamConfigCaps structure.");
                }
  
                // Alloc memory for structure
                pCaps = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(VideoStreamConfigCaps)));

                // Retrieve first (and hopefully only) capabilities struct
                numberOfCapabilities = videoStreamConfig.GetStreamCaps(0, out pMediaType, pCaps);
                if (numberOfCapabilities != 0)
                {
                    Marshal.ThrowExceptionForHR(numberOfCapabilities);
                }

                // Convert pointers to managed structures
                mediaType = (AMMediaType)Marshal.PtrToStructure(pMediaType, typeof(AMMediaType));
                caps = (VideoStreamConfigCaps)Marshal.PtrToStructure(pCaps, typeof(VideoStreamConfigCaps));

                // Extract info
                this.InputSize = caps.InputSize;
                this.MinFrameSize = caps.MinOutputSize;
                this.MaxFrameSize = caps.MaxOutputSize;
                this.FrameSizeGranularityX = caps.OutputGranularityX;
                this.FrameSizeGranularityY = caps.OutputGranularityY;
                this.MinFrameRate = 10000000.0 / ((double)caps.MaxFrameInterval);
                this.MaxFrameRate = 10000000.0 / ((double)caps.MinFrameInterval);
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
