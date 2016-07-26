using System;
using System.Drawing;
using System.Windows.Forms;
using VideoCallP2P.Recorder;

namespace VideoCallP2P
{
    class WebcamVedioPreview : Panel
    {
        //   private BitMapImageCapture capture = null;
        private Capture2 capture = null;
        private Filters filters = null;
        public Bitmap Bitmap = null;

        public const int ERROR_NONE = 0;
        public const int ERROR_WEBCAM_NOT_FOUND = 1;
        public const int ERROR_WEBCAM_ALREADY_USING = 2;
        public const int FRAME_WIDTH = 320;
        public const int FRAME_HEIGHT = 240;
        public static string RINGID_WEBCAM_DEVICE_ID = string.Empty;
        public int StartWebcam()
        {
            try
            {
                filters = new Filters();
                if (filters.VideoInputDevices == null || filters.VideoInputDevices.Count <= 0)
                {
                    return WebcamVedioPreview.ERROR_WEBCAM_NOT_FOUND;
                }
                //capture = new BitMapImageCapture(GetVideoDevice());
                //capture.FrameSize = new System.Drawing.Size(FRAME_WIDTH, FRAME_HEIGHT);
                //capture.PreviewWindow = this;
                //capture.CheckWebcam();

                capture = new Capture2(GetVideoDevice(), null);
                capture.PreviewWindow = this;

                numberofFrame = 0;
            }
            catch (Exception)
            {
                return WebcamVedioPreview.ERROR_WEBCAM_ALREADY_USING;
            }
            return WebcamVedioPreview.ERROR_NONE;
        }

        //public int RestartWebcam()
        //{
        //    Bitmap = null;
        //    try
        //    {
        //        if (capture != null)
        //        {
        //            capture.Dispose();
        //            capture = null;
        //        }

        //        filters = new Filters();
        //        if (filters.VideoInputDevices == null || filters.VideoInputDevices.Count <= 0)
        //        {
        //            return WebcamVedioPreview.ERROR_WEBCAM_NOT_FOUND;
        //        }
        //        capture = new BitMapImageCapture(GetVideoDevice());
        //        capture.FrameSize = new System.Drawing.Size(FRAME_WIDTH, FRAME_HEIGHT);
        //        capture.PreviewWindow = this;
        //    }
        //    catch (Exception ex)
        //    {
        //        return WebcamVedioPreview.ERROR_WEBCAM_ALREADY_USING;
        //    }
        //    return WebcamVedioPreview.ERROR_NONE;
        //}

        public int StartStream()
        {
            try
            {
                if (capture == null)
                {
                    return WebcamVedioPreview.ERROR_WEBCAM_NOT_FOUND;
                }

                if (filters == null || filters.VideoInputDevices == null || filters.VideoInputDevices.Count <= 0)
                {
                    return WebcamVedioPreview.ERROR_WEBCAM_NOT_FOUND;
                }

                //   capture.TakePicture();
                //capture.StartSampleGrabberToSend();
                capture.GrapImg();
            }
            catch (Exception)
            {
                return WebcamVedioPreview.ERROR_WEBCAM_ALREADY_USING;
            }
            return WebcamVedioPreview.ERROR_NONE;
        }

        public new void Dispose()
        {
            Bitmap = null;
            if (capture != null)
            {
                capture.Dispose();
                capture = null;
                base.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        //private void OnTakePicture(Bitmap bitmap)
        //{
        //    //    capture.OnTakePicture -= OnTakePicture;
        //    Bitmap = bitmap;
        //}
        public static int numberofFrame = 0;
        public static long time = ModelUtility.CurrentTimeMillis();
        private void OnStream(Bitmap bmp)
        {
        }

        private Filter GetVideoDevice()
        {
            Filter device = null;
            if (filters.VideoInputDevices.Count > 0)
            {
                device = filters.VideoInputDevices[0];
                if (!String.IsNullOrWhiteSpace(RINGID_WEBCAM_DEVICE_ID))
                {
                    foreach (Filter filter in filters.VideoInputDevices)
                    {
                        if (filter.MonikerString.Equals(RINGID_WEBCAM_DEVICE_ID))
                        {
                            device = filter;
                        }
                    }
                }
            }
            return device;
        }

    }
}
