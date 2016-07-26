using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VideoCallP2P.Libconnector;


using Image = System.Windows.Controls.Image;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace VideoCallP2P.lib2005
{
    public class CameraViewer : Image, IDisposable
    {
        #region Constants

        private const int PixelSize = 3;

        #endregion

        #region Private fields

        public static readonly DependencyProperty AvailableVideoModesProperty =
            DependencyProperty.Register("AvailableVideoModes", typeof (List<VideoMode>), typeof (CameraViewer),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty CaptureHeightProperty =
            DependencyProperty.Register("CaptureHeight", typeof (int), typeof (CameraViewer),
                new FrameworkPropertyMetadata(480, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty CaptureWidthProperty =
            DependencyProperty.Register("CaptureWidth", typeof (int), typeof (CameraViewer),
                new FrameworkPropertyMetadata(640, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty CapturedImageProperty =
            DependencyProperty.Register("CapturedImage", typeof (ImageSource), typeof (CameraViewer),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty MaxFramerateProperty =
            DependencyProperty.Register("MaxFramerate", typeof (int), typeof (CameraViewer),
                new FrameworkPropertyMetadata(20, FrameworkPropertyMetadataOptions.None,
                    OnMaxFramerateChanged));

        public static readonly DependencyProperty PreviewDividerProperty =
            DependencyProperty.Register("PreviewDivider", typeof (int), typeof (CameraViewer),
                new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty SelectedDeviceProperty =
            DependencyProperty.Register("SelectedDevice", typeof (string), typeof (CameraViewer),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.None,
                    OnSelectedDeviceChanged));

        public static readonly DependencyProperty SelectedVieoModeProperty =
            DependencyProperty.Register("SelectedVideoMode", typeof (VideoMode), typeof (CameraViewer),
                new FrameworkPropertyMetadata(new VideoMode(), FrameworkPropertyMetadataOptions.None,
                    OnSelectedVideoModeChanged));

        private DirectShowDevice device;
        private int height;
        private bool isCopyingFrame;
        private int width;

        #endregion

        #region Properties

        public List<VideoMode> AvailableVideoModes
        {
            get { return (List<VideoMode>)GetValue(AvailableVideoModesProperty); }
            private set { SetValue(AvailableVideoModesProperty, value); }
        }

        public bool CameraFound
        {
            get { return device.CameraAvailable; }
        }

        public int CaptureHeight
        {
            get { return (int)GetValue(CaptureHeightProperty); }
            private set { SetValue(CaptureHeightProperty, value); }
        }

        public int CaptureWidth
        {
            get { return (int)GetValue(CaptureWidthProperty); }
            private set { SetValue(CaptureWidthProperty, value); }
        }

        public ImageSource CapturedImage
        {
            get { return (ImageSource)GetValue(CapturedImageProperty); }
            set { SetValue(CapturedImageProperty, value); }
        }

        public BitmapSource CurrentBitmap
        {
            get { return (BitmapSource)Source; }
        }

        public DirectShowDevice Device
        {
            get { return device; }
        }

        public int MaxFramerate
        {
            get { return (int)GetValue(MaxFramerateProperty); }
            set { SetValue(MaxFramerateProperty, value); }
        }

        public int PreviewDivider
        {
            get { return (int)GetValue(PreviewDividerProperty); }
            set { SetValue(PreviewDividerProperty, value); }
        }

        public string SelectedDevice
        {
            get { return (string)GetValue(SelectedDeviceProperty); }
            set { SetValue(SelectedDeviceProperty, value); }
        }

        public VideoMode SelectedVieoMode
        {
            get { return (VideoMode)GetValue(SelectedVieoModeProperty); }
            set { SetValue(SelectedVieoModeProperty, value); }
        }

        #endregion

        #region Ctors

        public CameraViewer()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                var src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri("Images/camera.png", UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();
                Source = src;
                return;
            }
            CommandBindings.Add(new CommandBinding(CameraCommands.TakePhoto,
                TakePhoto_Executed, TakePhoto_CanExecute));

            Application.Current.Exit += OnApplicationExit;
            Initialized += CameraViewerInitialized;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            if (device != null)
            {
                device.Dispose();
                device = null;
            }
        }

        #endregion

        #region Event triggers

        private static void OnMaxFramerateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = d as CameraViewer;
            if (viewer != null && viewer.device != null)
            {
                viewer.device.FrameTime = (int)e.NewValue;
            }
        }

        private static void OnSelectedDeviceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = d as CameraViewer;
            if (viewer != null)
            {
                viewer.AvailableVideoModes = viewer.device.SelectVideoInput((string)e.NewValue, viewer.CaptureWidth,
                    viewer.CaptureHeight);
            }
        }

        private static void OnSelectedVideoModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mode = (VideoMode)e.NewValue;
            var viewer = d as CameraViewer;
            if (viewer != null)
            {
                viewer.CaptureWidth = mode.Width;
                viewer.CaptureHeight = mode.Height;
                viewer.ApplyVideoMode();
                viewer.AvailableVideoModes = viewer.device.SelectVideoInput(viewer.SelectedDevice, viewer.CaptureWidth,
                    viewer.CaptureHeight);
            }
        }

        void OnApplicationExit(object sender, ExitEventArgs e)
        {
            Dispose();
        }

        #endregion

        #region Event handlers

        private void CameraViewerInitialized(object sender, EventArgs e)
        {
            width = CaptureWidth;
            height = CaptureHeight;
            device = new DirectShowDevice(CaptureWidth, CaptureHeight, PreviewDivider, MaxFramerate);
            device.FrameReady += DeviceFrameReady;
        }

        void DeviceFrameReady(object sender, EventArgs e)
        {
            //Console.WriteLine("DeviceFrameReadyCalled");
            Dispatcher.BeginInvoke(
                DispatcherPriority.Render,
                new Action(() =>
                {
                    if (isCopyingFrame == false)
                    {
                        isCopyingFrame = true;
                        var bitmap = new Bitmap(P2PWrapper.iRenderWidth / PreviewDivider, P2PWrapper.iRenderHeight / PreviewDivider,
                            PixelFormat.Format24bppRgb);

                        BitmapData bmpData = bitmap.LockBits(
                            new Rectangle(0, 0, bitmap.Width / PreviewDivider, bitmap.Height / PreviewDivider),
                            ImageLockMode.ReadOnly, bitmap.PixelFormat);

                        Marshal.Copy(device.PreviewPixelMap, 0, bmpData.Scan0,
                            P2PWrapper.iRenderWidth * P2PWrapper.iRenderHeight * PixelSize / PreviewDivider / PreviewDivider);

                        bitmap.UnlockBits(bmpData);

                        IntPtr hBitmap = bitmap.GetHbitmap();

                        BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                            hBitmap,
                            IntPtr.Zero,
                            Int32Rect.Empty,
                            BitmapSizeOptions.FromEmptyOptions());

                        Source = bitmapSource;

                        DeleteObject(hBitmap);

                        isCopyingFrame = false;
                    }
                }));
        }

        #endregion

        #region Public methods

        public void ApplyResolutionChange()
        {
            width = CaptureWidth;
            height = CaptureHeight;
            device.SetResolution(width, height);
        }

        public void ApplyVideoMode()
        {
            width = CaptureWidth;
            height = CaptureHeight;
        }

        public BitmapSource GetCapturedBitmap()
        {
            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, bitmap.PixelFormat);

            Marshal.Copy(device.CapturedPixelMap, 0, bmpData.Scan0, width * height * PixelSize);

            bitmap.UnlockBits(bmpData);

            IntPtr hBitmap = bitmap.GetHbitmap();

            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            DeleteObject(hBitmap);

            return bitmapSource;
        }

        public void SetDefaultDevice()
        {
            device.SelectVideoInput();
        }

        #endregion

        #region Private methods

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private static void TakePhoto_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var viewer = sender as CameraViewer;
            if (viewer != null)
            {
                e.CanExecute = viewer.Device.CameraAvailable;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private static void TakePhoto_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var viewer = sender as CameraViewer;
            if (viewer != null)
            {
                viewer.CapturedImage = viewer.GetCapturedBitmap();
            }
        }

        #endregion
    }
}