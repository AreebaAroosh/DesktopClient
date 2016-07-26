using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge.Video;
using AForge.Video.DirectShow;
using VideoCallP2P.Libconnector;
using VideoCallP2P.Recorder;

namespace VideoCallP2P
{

    using System;
    using System.Threading;

    


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        DateTime time;
        

        WebcamVedioPreview webCamPreview;
        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
        EncoderParameters myEncoderParameters = new EncoderParameters(1);

        ImageCodecInfo jpgEncoder;
        MemoryStream ms;
        //   private StartVideoCapture capture = null;
        private Filters filters = new Filters();
        long loginId = 123L;
        long friendID = 321L;
        string sessionID = "session";
        string ip = "192.168.1.38";
        int port = 1250;
        public static MainWindow mainWindow;
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            mainWindow = this;
          //  this.Loaded += Form1_Load;
            this.Closed += MainWindow_Closed;

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 30L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            jpgEncoder = GetEncoder(ImageFormat.Jpeg);
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            stopWebcamPreview();
        }
        #region "INotifyPropertyChanged"
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        public static BitmapImage convertByteArrayToBitmapImage(byte[] byteArray)
        {
            try
            {
                var image = new BitmapImage();
                using (var ms = new System.IO.MemoryStream(byteArray))
                {
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();
                }
                return image;
            }
            catch (Exception ex)
            {
                //  log.Error("Error: convertByteArrayToBitmapImage()." + ex.Message);
            }
            return null;
        }
        public void ChangeImage(byte[] ImageFinalByte)
        {
            if (ImageFinalByte.Length > 100)
            {
                try
                {
                    BitmapImage image = convertByteArrayToBitmapImage(ImageFinalByte);
                    if (image != null)
                    {
                        MyImageSource = image;
                    }
                    else
                    {
                    }
                }
                catch (Exception es)
                {

                }
            }
        }
        private ImageSource _myImageSource;
        public ImageSource MyImageSource
        {
            get
            {
                return _myImageSource;
            }
            set
            {
                if (value == _myImageSource)
                    return;
                _myImageSource = value;
                OnPropertyChanged("MyImageSource");
            }
        }
        public void stopWebcamPreview()
        {
            if (webCamPreview != null)
                webCamPreview.Dispose();
            //Stop and free the webcam object if application is closing
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource = null;
            }
        }
        // StartVideoCapture webCap;
        public void start2WebCam()
        {
            webCamPreview = new WebcamVedioPreview();
            WinFormsHost.Child = webCamPreview;
            int exception = webCamPreview.StartWebcam();

            if (exception > 0)
            {
                System.Diagnostics.Debug.WriteLine("Webcam runing...");
            }
            else
            {
                webCamPreview.StartStream();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            start2WebCam();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            stopWebcamPreview();
        }

        VideoCaptureDevice videoSource;
        public static int numberofFrame = 0;
        public static int numberofFrameSenidng = 0;
        int modFactor = 5;
        private void Form1_Load(object sender, EventArgs e)
        {
            time = DateTime.Now;
            //List all available video sources. (That can be webcams as well as tv cards, etc)
            FilterInfoCollection videosources = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            //Check if atleast one video source is available
            if (videosources != null)
            {
                //For example use first video device. You may check if this is your webcam.
                videoSource = new VideoCaptureDevice(videosources[0].MonikerString);
                try
                {
                    //Check if the video device provides a list of supported resolutions
                    if (videoSource.VideoCapabilities.Length > 0)
                    {
                        string highestSolution = "0;0";
                        //Search for the highest resolution
                        for (int i = 0; i < videoSource.VideoCapabilities.Length; i++)
                        {
                            if (videoSource.VideoCapabilities[i].FrameSize.Width > Convert.ToInt32(highestSolution.Split(';')[0]))
                                highestSolution = videoSource.VideoCapabilities[i].FrameSize.Width.ToString() + ";" + i.ToString();
                        }
                        //Set the highest resolution as active
                        videoSource.VideoResolution = videoSource.VideoCapabilities[Convert.ToInt32(highestSolution.Split(';')[1])];
                    }
                }
                catch { }

                //Create NewFrame event handler
                //(This one triggers every time a new frame/image is captured
                videoSource.NewFrame += new NewFrameEventHandler(videoSource_NewFrame);

                //Start recording
                videoSource.Start();
            }

        }

        int iCounter = 0;

        void videoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            TimeSpan timeDiff = DateTime.Now - time;
            numberofFrame++;
            if (timeDiff.TotalMilliseconds > 1000)
            {
               //  Console.WriteLine("Frame per Second==>" + numberofFrame);
                if (numberofFrame <= 5)
                { modFactor = 1; }
                else if (numberofFrame <= 10)
                { modFactor = 3; }
                else if (numberofFrame <= 15)
                { modFactor = 4; }
                else if (numberofFrame <= 20)
                { modFactor = 5; }
                else
                { modFactor = 6; }
                numberofFrame = 0;
                time = DateTime.Now;
            }

            /*
           
            if (bStartSending)
            {
                //MediaEngineWork
                const int iFixedLength = 320 * 192 * 3 / 2;
                byte[] dataToSend = new byte[iFixedLength + 5];
                for (int i = VideoDataIndx, indx = 0; i < VideoDataIndx + iFixedLength; i++, indx++)
                    dataToSend[indx] = VideoData[i];

                VideoDataIndx += iFixedLength;
                if (VideoDataIndx >= VideoData.Length)
                    VideoDataIndx = 0;


                P2PWrapper.SendVideoData(200, dataToSend, iFixedLength, 0);

               //  Console.WriteLine("Thread Start/Stop/Join Sample");

                if(iCounter == 0)
                {
                   

                    iCounter++;
                }

               
            }
             * 
             * */
                //
            
          



            //Cast the frame as Bitmap object and don't forget to use ".Clone()" otherwise
            //you'll probably get access violation exceptions
            //    pictureBoxVideo.BackgroundImage = (Bitmap)eventArgs.Frame.Clone();

           

            Bitmap b = (Bitmap)eventArgs.Frame.Clone();
            try
            {
                ms = new MemoryStream();
                b.Save(ms, jpgEncoder, myEncoderParameters);
                byte[] bmpBytes = ms.ToArray();
                //   //  Console.WriteLine("Length==>" + bmpBytes.Length);
                //    CallHelperMethods.AddAndStartThreadToProcessVideoImage(bmpBytes);
                ms.Close();
                MainWindow.mainWindow.ChangeImage(bmpBytes);
                bmpBytes = null;
            }
            catch (Exception ex)
            {
                //log.Error("Vedio image==>" + ex.StackTrace);
            }
        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
        //private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    //Stop and free the webcam object if application is closing
        //    if (videoSource != null && videoSource.IsRunning)
        //    {
        //        videoSource.SignalToStop();
        //        videoSource = null;
        //    }
        //}
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void Button_Connect(object sender, RoutedEventArgs e)
        {
            /*
            int connected = P2PWrapper.InitializeLibrary(loginId);
            System.Diagnostics.Debug.WriteLine("MediaEngineLib==> connected = " + connected);
            int iRet = P2PWrapper.StartVideoCall(200, 288, 352);
            System.Diagnostics.Debug.WriteLine("MediaEngineLib==> iRet = " + iRet);

            P2PWrapper.Instance.LinkWithConnectivityLib(null);
            P2PWrapper.ipv_SetAuthenticationServer(ip, port, sessionID);
            System.Diagnostics.Debug.WriteLine("Connected with Lib==>" + connected);
            */
            //string text = System.IO.File.ReadAllText(@"E:\dummy.txt");

            /*Alpha oAlpha = new Alpha();

            oAlpha.VideoData = System.IO.File.ReadAllBytes(@"VideoSample.yuv");
            oAlpha.AudioData = System.IO.File.ReadAllBytes(@"AudioSample.pcm");
            //oAlpha.AudioData = System.IO.File.ReadAllBytes(@"C:\Users\Nayeem\Downloads\a2002011001-e02-16kHz.wav"); 
            oAlpha.bStartSending = true;

            Thread oThread = new Thread(new ThreadStart(oAlpha.Beta));
            oThread.Start();*/


        }
        private void Button_CreateS(object sender, RoutedEventArgs e)
        {
            SessionStatus sessionStatus = P2PWrapper.ipv_CreateSession(friendID, ConfigFile.MediaType.IPV_MEDIA_AUDIO, ip, port);
            if (sessionStatus == SessionStatus.SESSION_CREATE_SUCCESSFULLY)
            {
                System.Diagnostics.Debug.WriteLine("Session created Successfully");
            }
        }
        private void Button_Disconnect(object sender, RoutedEventArgs e)
        {
            P2PWrapper.ipv_CloseSession(friendID, ConfigFile.MediaType.IPV_MEDIA_AUDIO);
        }
        private void Button_Destroy(object sender, RoutedEventArgs e)
        {
            P2PWrapper.ipv_Release();
        }

    }
}
