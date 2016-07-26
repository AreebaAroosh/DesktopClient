using System;
using VideoCallP2P.Recorder.DirectShowNET;

namespace VideoCallP2P.Recorder
{
    public class Filters
    {
        public FilterCollection AudioCompressors;
        public FilterCollection AudioInputDevices;
        public FilterCollection VideoCompressors;
        public FilterCollection VideoInputDevices;

        public Filters()
        {
            try
            {
                this.VideoInputDevices = new FilterCollection(FilterCategory.VideoInputDevice);
            }
            catch
            {
            }
            try
            {
                this.AudioInputDevices = new FilterCollection(FilterCategory.AudioInputDevice);
            }
            catch
            {
            }
            try
            {
                this.VideoCompressors = new FilterCollection(FilterCategory.VideoCompressorCategory);
            }
            catch
            {
            }
            try
            {
                this.AudioCompressors = new FilterCollection(FilterCategory.AudioCompressorCategory);
            }
            catch
            {
            }
        }

    }
}
