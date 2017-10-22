using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

using DirectShowLib;
using VideoCallP2P.Libconnector;


//#define printg(X,...) _RPT1(0,X,__VA_ARGS__)
//#define printf(...) printg(__VA_ARGS__,"")

namespace VideoCallP2P.lib2005
{
    public class DirectShowDevice : DependencyObject, ISampleGrabberCB, IDisposable
    {
        #region Delegates

        public delegate void FrameReadyEventHandler(object sender, EventArgs e);

        #endregion

        #region Constants

        private const int PixelSize = 3;

        #endregion

        #region Private fields

        public int FrameTime;

        protected ICaptureGraphBuilder2 CaptureGraphBuilder;
        protected byte[] CapturedFrame;
        protected byte[] Frame;
        protected IGraphBuilder GraphBuilder;
        protected IMediaControl MediaControl;
        protected int PreviewDivider;
        protected byte[] PreviewFrame;
        protected ISampleGrabber SampleGrabber;
        protected Thread UpdateThread;
        protected IBaseFilter VideoInput;
        private readonly List<VideoMode> availableVideoModes;
        private bool frameArrived;
        private int height;
        private DsDevice selectedDevice;
        private VideoMode selectedVideoMode;
        private int width;
        private int counter = 0;
        private byte[] TempFrameRGB24;
        #endregion

        #region Properties

        public static ArrayList AvailableDevices
        {
            get
            {
                var devices = new ArrayList();

                foreach (DsDevice d in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
                {
                    if (d.Name.ToUpper().IndexOf("SECONDARY") > -1)
                    {
                    }
                    devices.Add(d);
                }
                return devices;
            }
        }

        public bool CameraAvailable
        {
            get
            {
                if (SampleGrabber != null)
                {
                    Console.WriteLine("TheKing--> Found Camera");
                    return true;
                }
                Console.WriteLine("TheKing--> ERROR... Camera Not Found");
                return false;
            }
        }

        public byte[] CapturedPixelMap
        {
            get { return Frame; }
        }

        public byte[] PreviewPixelMap
        {
            get { return PreviewFrame; }
        }

        public string SelectedDeviceMoniker
        {
            get
            {
                if (selectedDevice != null)
                {
                    return selectedDevice.Name;
                }
                return null;
            }
        }

        public VideoMode SelectedVieoMode
        {
            get { return selectedVideoMode; }
        }

        #endregion

        #region Events

        public event FrameReadyEventHandler FrameReady;

        #endregion

        #region Ctors

        public DirectShowDevice()
        {
            PreviewDivider = 1;
            frameArrived = false;
            availableVideoModes = new List<VideoMode>();
            FrameTime = 200;
        }

        public DirectShowDevice(int width, int height, int previewDivider, int maxFramerate)
        {
            PreviewDivider = previewDivider;
            this.width = width;
            this.height = height;
            frameArrived = false;
            availableVideoModes = new List<VideoMode>();
            FrameTime = (1 / maxFramerate) * 1000;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            if (MediaControl != null)
            {
                MediaControl.StopWhenReady();
                Marshal.ReleaseComObject(MediaControl);
            }

            if (GraphBuilder != null)
            {
                Marshal.ReleaseComObject(GraphBuilder);
            }
            if (CaptureGraphBuilder != null)
            {
                Marshal.ReleaseComObject(CaptureGraphBuilder);
            }

            if (UpdateThread != null)
            {
                UpdateThread.Abort();
            }

            CaptureGraphBuilder = null;
            GraphBuilder = null;
            MediaControl = null;
        }

        #endregion

        #region Event triggers

        private void OnFrameReady(object sender, EventArgs e)
        {
            if (FrameReady != null)
            {
                FrameReady(sender, e);
            }
        }

        #endregion

        #region Public methods

        public IBaseFilter GetVideo()
        {
            IBaseFilter baseDevice;

            var filterGraph = new FilterGraph() as IFilterGraph2;

            filterGraph.AddSourceFilterForMoniker(selectedDevice.Mon, null, selectedDevice.Name, out baseDevice);

            IPin pin = DsFindPin.ByCategory(baseDevice, PinCategory.Capture, 0);
            var streamConfig = pin as IAMStreamConfig;
            AMMediaType media;
            int iC = 0, iS = 0;
            streamConfig.GetNumberOfCapabilities(out iC, out iS);
            IntPtr ptr = Marshal.AllocCoTaskMem(iS);
            for (int i = 0; i < iC; i++)
            {
                streamConfig.GetStreamCaps(i, out media, ptr);
                VideoInfoHeader v;
                v = new VideoInfoHeader();
                Marshal.PtrToStructure(media.formatPtr, v);
            }

            Guid iid = typeof(IBaseFilter).GUID;
            object source;
            selectedDevice.Mon.BindToObject(null, null, ref iid, out source);
            return (IBaseFilter)source;
        }

        public List<VideoMode> SelectVideoInput(string deviceName, int width, int height)
        {
            Console.WriteLine("TheKing--> SelectVideoInput, deviceName = " + deviceName + ", width = " + width + ", height = " + height);
            if (UpdateThread != null)
            {
                UpdateThread.Abort();
            }
            this.width = width;
            this.height = height;
            FindDevice(deviceName);
            VideoInput = GetVideo();
            TempFrameRGB24 = new byte[this.width * this.height * 3];
            ApplyVideoInput();

            return availableVideoModes;
        }

        public List<VideoMode> SelectVideoInput(string deviceName)
        {
            FindDevice(deviceName);
            VideoInput = GetVideo();
            ApplyVideoInput();

            return availableVideoModes;
        }

        public List<VideoMode> SelectVideoInput()
        {
            if (AvailableDevices.Count == 0)
            {
                return new List<VideoMode>();
            }
            selectedDevice = (DsDevice)AvailableDevices[0];
            VideoInput = GetVideo();
            ApplyVideoInput();

            return availableVideoModes;
        }
        byte[] outputYUV420;//= 
        public void SetResolution(int width, int height)
        {
            if (counter == 2) return;
            counter++;
            int iRet;
            this.width = width;
            this.height = height;
            outputYUV420 = new byte[width * height * 3 / 2];
            object o;

            CaptureGraphBuilder.FindInterface(PinCategory.Capture, MediaType.Video, VideoInput,
                typeof(IAMStreamConfig).GUID, out o);
            var videoStreamConfig = o as IAMStreamConfig;
            AMMediaType media;

            



            
            int iC = 0, iS = 0;
            iRet = videoStreamConfig.GetNumberOfCapabilities(out iC, out iS);
            if (iRet != 0) Console.WriteLine("TheKing--> Error Found GetNumberOfCapabilities");

            IntPtr ptr = Marshal.AllocCoTaskMem(iS);
            int streamId = 0;
            var videoInfo = new VideoInfoHeader();

            availableVideoModes.Clear();
            for (int i = 0; i < iC; i++)
            {
                iRet = videoStreamConfig.GetStreamCaps(i, out media, ptr);
                if (iRet != 0) Console.WriteLine("TheKing--> Error Found GetStreamCaps");
                Marshal.PtrToStructure(media.formatPtr, videoInfo);

                if (videoInfo.BmiHeader.Width != 0 && videoInfo.BmiHeader.Height != 0)
                {
                    availableVideoModes.Add(new VideoMode(videoInfo.BmiHeader.Width, videoInfo.BmiHeader.Height));
                }
                if (videoInfo.BmiHeader.Width != width || videoInfo.BmiHeader.Height != height)
                {
                    continue;
                }
                streamId = i;
                selectedVideoMode = availableVideoModes.Last();
                break;
            }


            iRet = videoStreamConfig.GetStreamCaps(streamId, out media, ptr);
            if (iRet != 0) Console.WriteLine("TheKing--> Error Found GetStreamCaps 2");
            Marshal.PtrToStructure(media.formatPtr, videoInfo);
            iRet = videoStreamConfig.SetFormat(media);
            if(iRet!=0) Console.WriteLine("TheKing SetFormat --> " + iRet);
            Marshal.FreeCoTaskMem(ptr);


            media = new AMMediaType();
            media.majorType = MediaType.Video;
            if (counter == 1)
            {
                media.subType = MediaSubType.RGB24; //Input Video Format
            }
            else if (counter == 2)
            {
                media.subType = MediaSubType.RGB24;
            }


            media.formatType = FormatType.VideoInfo;
            media.fixedSizeSamples = true;

            iRet = SampleGrabber.SetMediaType(media);
            if(iRet!=0) Console.WriteLine("TheKing SetMediaType --> " + iRet);
            DsUtils.FreeAMMediaType(media);

            if (counter == 1)
            {
                //InitRingIDSDKLib();

                Thread oThread = new Thread(new ThreadStart(RenderThread));
                oThread.Start();
            }
        }


        byte[] frameForRender = new byte[640 * 480 * 3/2];
        int iFrameReceiveCounter = 0;
        void RenderThread()
        {
            while(true)
            {
                if (P2PWrapper.framesQueue.Count <= 0)
                {
                    //Console.WriteLine("dsifjhsdiufhsdijfhishfiusdhfiudshfishgiurut9udfhijryht8ufijgjrijtyiufh");
                    Thread.Sleep(5);
                    continue;
                }
                iFrameReceiveCounter++;
                if(iFrameReceiveCounter == 1)
                {
                    Frame = new byte[(P2PWrapper.iRenderWidth * P2PWrapper.iRenderHeight) * PixelSize];
                    CapturedFrame = new byte[(P2PWrapper.iRenderWidth * P2PWrapper.iRenderHeight) * PixelSize];
                    PreviewFrame = new byte[(P2PWrapper.iRenderWidth / PreviewDivider * P2PWrapper.iRenderHeight / PreviewDivider) * PixelSize];
                }

                CapturedFrame = P2PWrapper.framesQueue.Dequeue(); 

                //Console.WriteLine("FrameForRender = " + frameForRender.Length);

                //YUV2RGBManaged2(frameForRender, CapturedFrame, width, height);

                UpdateBuffer(CapturedFrame, P2PWrapper.iRenderHeight, P2PWrapper.iRenderWidth);

                frameArrived = true;
               
                
            }
        }
        
        public void InitRingIDSDKLib()
        {
            //string sIP = "192.168.8.29";
            string sIP = "38.127.68.60";
            //string sIP = "60.68.127.38";
            int iFriendPort = 60007;

            int iRet = 0;
            P2PWrapper p2pWrapper = P2PWrapper.GetInstance();

            iRet = p2pWrapper.InitializeLibraryR(100/*UserID*/);
            System.Console.WriteLine("MediaEngineLib==> InitializeLibrary, iRet = " +  iRet);
            //p2pWrapper.CreateSessionR(200/*FriendID*/, 1/*Audio*/, sIP, iFriendPort);
            //p2pWrapper.CreateSessionR(200, 2/*Video*/, sIP, iFriendPort);
            //p2pWrapper.SetRelayServerInformationR(200, 1, sIP, iFriendPort);
            //p2pWrapper.SetRelayServerInformationR(200, 2, sIP, iFriendPort);
            iRet = p2pWrapper.StartAudioCallR(200);
            iRet = p2pWrapper.StartVideoCallR(200, height/*Height*/, width/*Width*/);
            System.Diagnostics.Debug.WriteLine("MediaEngineLib==> StartVideoCall, iRet = " + iRet);
            //p2pWrapper.SetLoggingStateR(true, 5);
            p2pWrapper.LinkWithConnectivityLib(null);

        }

        #endregion

        #region Protected methods

        protected void UpdateBuffer(byte[] ReceivedFrameData, int iHeight, int iWidth)
        {
            //while (true)
            {

                for (int y = 1; y <= iHeight; y++)
                {
                    for (int x = 0; x < iWidth; x++)
                    {
                        int flippedPosition = (((iHeight - y) * iWidth) + (x)) * PixelSize;
                        int position = (((y - 1) * iWidth) + (x)) * PixelSize;

                        Frame[flippedPosition + 0] = ReceivedFrameData[position + 0];
                        Frame[flippedPosition + 1] = ReceivedFrameData[position + 1];
                        Frame[flippedPosition + 2] = ReceivedFrameData[position + 2];

                        if (x % PreviewDivider == 0 && y % PreviewDivider == 0)
                        {
                            int previewPosition = (((iHeight - y) * iWidth / PreviewDivider / PreviewDivider) +
                                (x / PreviewDivider)) * PixelSize;

                            PreviewFrame[previewPosition + 0] = ReceivedFrameData[position + 0];
                            PreviewFrame[previewPosition + 1] = ReceivedFrameData[position + 1];
                            PreviewFrame[previewPosition + 2] = ReceivedFrameData[position + 2];
                        }
                    }
                }

                OnFrameReady(this, EventArgs.Empty);
                /*
                frameArrived = false;
                while (!frameArrived)
                {
                    Thread.Sleep(FrameTime);
                }*/
            }
        }

        #endregion

        #region Private methods

        private void ApplyVideoInput()
        {
            int iRet;
            Dispose();
            
             

            /*Frame = new byte[(width * height) * PixelSize];
            CapturedFrame = new byte[(width * height) * PixelSize];
            PreviewFrame = new byte[(width / PreviewDivider * height / PreviewDivider) * PixelSize];*/




            if (VideoInput == null)
            {
                return;
            }
            
            //Original Code
            GraphBuilder = (IGraphBuilder)new FilterGraph();
            CaptureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            MediaControl = (IMediaControl)GraphBuilder;
            iRet = CaptureGraphBuilder.SetFiltergraph(GraphBuilder);
            if (iRet != 0) Console.WriteLine("TheKing--> Error Found SetFiltergraph");

            SampleGrabber = new SampleGrabber() as ISampleGrabber;
            iRet = GraphBuilder.AddFilter((IBaseFilter)SampleGrabber, "Render");
            if (iRet != 0) Console.WriteLine("TheKing--> Error Found AddFilter 1");

            SetResolution(width, height);
            iRet = GraphBuilder.AddFilter(VideoInput, "Camera");

            if (iRet != 0) Console.WriteLine("TheKing--> Error Found AddFilter 2");
            iRet = SampleGrabber.SetBufferSamples(true);
            if (iRet != 0) Console.WriteLine("TheKing--> Error Found SetBufferSamples");
            iRet = SampleGrabber.SetOneShot(false);
            if (iRet != 0) Console.WriteLine("TheKing--> Error Found SetOneShot");

            iRet = SampleGrabber.SetCallback(this, 1);

            if (iRet != 0) Console.WriteLine("TheKing--> Error Found SetCallback");

            iRet = CaptureGraphBuilder.RenderStream(null, null, VideoInput, null, SampleGrabber as IBaseFilter);
            if (iRet < 0)
            {
                Console.WriteLine("TheKing--> Error Found in  CaptureGraphBuilder.RenderStream, iRet = " + iRet+", Initialization TryNumber = " + counter);
                if(counter == 1)
                    ApplyVideoInput();
            }


            //GraphBuilder.Connect()
            //iRet = CaptureGraphBuilder.RenderStream(null, null, VideoInput, null, null);
            //if (iRet != 0) Console.WriteLine("TheKing--> Error Found RenderStream 1");
            
           
            //iRet = CaptureGraphBuilder.RenderStream(PinCategory.Capture, MediaType.Video, VideoInput, null, SampleGrabber as IBaseFilter);
            //if (iRet != 0) Console.WriteLine("TheKing--> Error Found RenderStream 2, iRet = " + iRet);


            
            if (UpdateThread != null)
            {
                UpdateThread.Abort();
            }
            
            //UpdateThread = new Thread(UpdateBuffer);
            //UpdateThread.Start();

            MediaControl.Run();
            
            Marshal.ReleaseComObject(VideoInput);
            
        }

        private void FindDevice(string deviceName)
        {
            Console.WriteLine("TheKing-->FindDevice, deviceName = " + deviceName);

            if (AvailableDevices.Count == 0)
            {
                Console.WriteLine("TheKing CameraCount = 0");
                return;
            }

            bool deviceFound = false;
            foreach (DsDevice device in AvailableDevices)
            {
                if (device.Name == deviceName)
                {
                    selectedDevice = device;
                    Console.WriteLine("TheKing-->FindDevice, selectedDevice = " + selectedDevice);
                    deviceFound = true;
                }
            }

            if (!deviceFound)
            {
                selectedDevice = (DsDevice)AvailableDevices[0];
            }
        }
        void YUY2ToI420(byte[] input, byte[] output, int _width, int _height)
        {
            Int32 pixels = _width * _height;
            Int32 macropixels = pixels >> 1;

            long mpx_per_row = _width >> 1;

            for (int i = 0, ci = 0; i < macropixels; i++)
            {
                output[i << 1] = input[i << 2];
                output[(i << 1) + 1] = input[(i << 2) + 2];

                long row_number = i / mpx_per_row;
                if (row_number % 2 == 0)
                {
                    output[pixels + ci] = input[(i << 2) + 1];
                    output[pixels + (pixels >> 2) + ci] = input[(i << 2) + 3];
                    ci++;
                }
            }

            return;
        }
        private static unsafe void YUV2RGBManaged2(byte[] YUVData, byte[] RGBData, int width, int height)
        {

            fixed (byte* pRGBs = RGBData, pYUVs = YUVData)
            {
                int yIndex = 0;
                int uIndex = width * height;
                int vIndex = (width * height * 5 ) / 4;


                for (int r = 0; r < height; r++)
                {
                    byte* pRGB = pRGBs + r * width * 3;

                    if (r % 2 != 0)
                    {
                        uIndex -= (width >> 1);
                        vIndex -= (width >> 1);
                    }
                    for (int c = 0; c < width; c += 2)
                    {
                        int C1 = pYUVs[yIndex++] - 16;
                        int C2 = pYUVs[yIndex++] - 16;


                        int D = pYUVs[vIndex++] - 128;
                        int E = pYUVs[uIndex++] - 128;

                        int R1 = (298 * C1 + 409 * E + 128) >> 8;
                        int G1 = (298 * C1 - 100 * D - 208 * E + 128) >> 8;
                        int B1 = (298 * C1 + 516 * D + 128) >> 8;

                        int R2 = (298 * C2 + 409 * E + 128) >> 8;
                        int G2 = (298 * C2 - 100 * D - 208 * E + 128) >> 8;
                        int B2 = (298 * C2 + 516 * D + 128) >> 8;


                        pRGB[0] = (byte)(R1 < 0 ? 0 : R1 > 255 ? 255 : R1);
                        pRGB[1] = (byte)(G1 < 0 ? 0 : G1 > 255 ? 255 : G1);
                        pRGB[2] = (byte)(B1 < 0 ? 0 : B1 > 255 ? 255 : B1);

                        pRGB[3] = (byte)(R2 < 0 ? 0 : R2 > 255 ? 255 : R2);
                        pRGB[4] = (byte)(G2 < 0 ? 0 : G2 > 255 ? 255 : G2);
                        pRGB[5] = (byte)(B2 < 0 ? 0 : B2 > 255 ? 255 : B2);


                        pRGB += 6;

                    }
                }
            }
        }

        private static unsafe byte[] clip = new byte[896];
        private static unsafe bool bInit = false;

        private static unsafe void InitClip()
        {
            //memset(clip, 0, 320);
            for (int i = 0; i < 320; i++) clip[i] = 0;
            
            for (int i = 0; i < 256; ++i) clip[i + 320] = (byte)i;
            
            //memset(clip+320+256, 255, 320);
            for (int i = 320 + 256; i < 320 + 256 + 320; i++) clip[i] = 255;

        }

        private static unsafe byte Clip(int x)
        { 
	        return clip[320 + ((x+0x8000) >> 16)]; 
        }

        
        private static unsafe void TransformRGB24(byte[] lpIndata, byte[] lpOutdata, int m_iWidth, int m_iHeight)
        {

	        if( bInit == false)
	        {
		        bInit = true;
		        InitClip();
	        }

	        const int cyb = (int)(0.114*219/255*65536+0.5);
            const int cyg = (int)(0.587*219/255*65536+0.5);
            const int cyr = (int)(0.299*219/255*65536+0.5);

	        int py = 0;
	        int pu = 0 + m_iWidth*m_iHeight;
	        int pv = pu + m_iWidth*m_iHeight/4;

	        for(int row = 0; row < m_iHeight; ++row) 
	        {
		        //byte* rgb = lpIndata + m_iWidth * 3 * (m_iHeight-1-row);
                fixed(byte* rgbs = lpIndata)
                {
                    byte* rgb = rgbs + m_iWidth * 3 * (m_iHeight-1-row);

		            for(int col = 0; col < m_iWidth; col += 2) 
		            {
			            // y1 and y2 can't overflow
			            int y1 = (cyb*rgb[0] + cyg*rgb[1] + cyr*rgb[2] + 0x108000) >> 16;
			            lpOutdata[py++] = (byte)y1;
			            int y2 = (cyb*rgb[3] + cyg*rgb[4] + cyr*rgb[5] + 0x108000) >> 16;
			            lpOutdata[py++] = (byte)y2;

			            if( (row&1) == 0)
			            {
				            int scaled_y = (y1+y2 - 32) * (int)(255.0/219.0*32768+0.5);
				            int b_y = ((rgb[0]+rgb[3]) << 15) - scaled_y;
				            byte u = lpOutdata[pu++] = Clip((b_y >> 10) * (int)(1/2.018*1024+0.5) + 0x800000);  // u
				            int r_y = ((rgb[2]+rgb[5]) << 15) - scaled_y;
				            byte v = lpOutdata[pv++] = Clip((r_y >> 10) * (int)(1/1.596*1024+0.5) + 0x800000);  // v
			            }
			            rgb += 6;
		            }
                }
	        }
        }

        #endregion
        byte[] capturedYUY2Buffer;
        #region ISampleGrabberCB Members
        int iTemp = 0;
        int iFpsControl = 0;

        long lastFrame = 0;
        public int BufferCB(double sampleTime, IntPtr pBuffer, int bufferLen)
        {
            Console.WriteLine("VampireEngg--> TimeDiff = " + (GetCurrentTimeStamp() - lastFrame));
            lastFrame = GetCurrentTimeStamp();
            iFpsControl++;
            //if (iFpsControl % 2 == 1) return 0;
            Marshal.Copy(pBuffer, TempFrameRGB24, 0, bufferLen);

            //P2PWrapper.iRenderHeight = height;
            //P2PWrapper.iRenderWidth = width;
            //P2PWrapper.framesQueue.Enqueue(TempFrameRGB24);

            int iRet = P2PWrapper.GetInstance().SendVideoDataR(200, TempFrameRGB24, bufferLen, 1, 0);

            //Console.WriteLine("VampireENgg--> Capturing RGB24 Data..... bufferLen = " + bufferLen + ", Returned = " + iRet);
            return 0;
            
            /*
            //Console.WriteLine("VampireENgg--> bufferLen = " + (bufferLen - width * height * 3) );


            byte[] TempFrame = new byte[width * height * 3];
            byte[] YUVI420 = new byte[width * height * 3/2];

            //capturedYUY2Buffer = new byte[bufferLen];
           // Marshal.Copy(pBuffer, capturedYUY2Buffer, 0, bufferLen);

            Marshal.Copy(pBuffer, TempFrame, 0, bufferLen);

            //YUY2ToI420(capturedYUY2Buffer, outputYUV420, width, height);
          
            //P2PWrapper.SendVideoData(200, capturedYUY2Buffer, capturedYUY2Buffer.Length, 1);
            long milliseconds, milliseconds2;

            milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            TransformRGB24(TempFrame, YUVI420, width, height);
            milliseconds2 = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - milliseconds;
            Console.WriteLine("RGB2YUV420Managed2 time = " + milliseconds2);

            if (iTemp == 0)
            {
                System.IO.File.WriteAllBytes("TestYuv420.yuv", YUVI420);
                iTemp = 1;
            }
            else
            {
                P2PWrapper.AppendAllBytes("TestYuv420.yuv", YUVI420);
            }

            
            milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            YUV2RGBManaged2(outputYUV420, TempFrame, width, height);
            milliseconds2 = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - milliseconds;
            Console.WriteLine("YUV2RGBManaged2 time = " + milliseconds2);

            




            Console.WriteLine("TheKing-->BufferCB: " + bufferLen);
            for (int i = 0; i < 10; i++)
            {
                Console.Write(TempFrame[i] + " ");
            }
            Console.WriteLine("");

            P2PWrapper.framesQueue.Enqueue(TempFrame);
            //frameArrived = true;

            return 0;
           */
        }

        public int SampleCB(double sampleTime, IMediaSample pSample)
        {
            Console.WriteLine("TheKing--> Inside SampleCB");
            return 0;
        }

        public long GetCurrentTimeStamp()
        {
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            return milliseconds;
        }

        #endregion
    }
}

