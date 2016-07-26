using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VideoCallP2P.Libconnector;

namespace VideoCallP2P
{
    /// <summary>
    /// Interaction logic for WNLib2005.xaml
    /// </summary>
    public partial class WNLib2005 : Window
    {
        #region "Class By VideoTeam"
        public class Alpha
        {

            // This method that will be called when the thread is started
            public byte[] VideoData;
            public byte[] AudioData;
            public short[] AudioData2;

            public byte[] TestB;
            public short[] TestS;

            public int VideoDataIndx = 0;
            public int AudioDataIndx = 0;
            public bool bStartSending = false;
            int icounttttt = 0;
            public void Beta()
            {
                const int iFixedLength = 640 * 480 * 3 / 2;
                const int iFixedLengthAudio = 160;

                TestB = new byte[20];
                TestS = new short[10];

                for (int i = 0; i < 20; i++)
                {
                    TestB[i] = (byte)i;
                }


                for (int i = 0; i < 10; i++)
                {
                    short right = (short)TestB[2 * i + 1];
                    short left = (short)TestB[2 * i];
                    TestS[i] = (short)(right | (left << 8));
                }

                //Array.Copy(TestB, TestS, TestS.Length);

                byte[] dataToSend = new byte[iFixedLength + 5];
                short[] dataToSendAudio = new short[iFixedLengthAudio + 5];
                ushort[] dataToSendAudio2 = new ushort[iFixedLengthAudio + 5];


                while (true)
                {
                   //  Console.WriteLine("Alpha.Beta is running in its own thread.");
                    if (icounttttt == 0)
                    {
                        System.IO.File.WriteAllText("log.txt", "running\r\n");
                    }
                    else
                    {
                        System.IO.File.AppendAllText("log.txt", "running\r\n");
                    }
                    icounttttt++;

                    //MediaEngineWork


                    for (int i = VideoDataIndx, indx = 0; i < VideoDataIndx + iFixedLength && i < VideoData.Length; i++, indx++)
                        dataToSend[indx] = VideoData[i];

                    VideoDataIndx += iFixedLength;
                    if (VideoDataIndx >= VideoData.Length)
                        VideoDataIndx = 0;
                    Thread.Sleep(33);

                    /*for(int i=AudioDataIndx, indx = 0; i <AudioDataIndx+iFixedLengthAudio && i<AudioData.Length; i+=2, indx++)
                    {
                        short right = (short)AudioData[i + 1];
                        short left = (short)AudioData[i];
                        dataToSendAudio[indx] = (short)(right | (left << 8));

                    }*/

                    if (AudioDataIndx == 0)
                        System.Console.WriteLine("C# audio data for the 1st frame");
                    for (int i = 0; i < iFixedLengthAudio / 2; i++)
                    {
                        ushort left = (ushort)AudioData[AudioDataIndx + 2 * i + 1];
                        ushort right = (ushort)AudioData[AudioDataIndx + 2 * i];
                        dataToSendAudio[i] = (short)(right | (left << 8));
                        dataToSendAudio2[i] = (ushort)(right | (left << 8));

                        if (AudioDataIndx == 0 && i < 30)
                        {
                            System.Console.Write(" " + (uint)dataToSendAudio2[i]);
                        }
                    }

                    AudioDataIndx += iFixedLengthAudio;
                    if (AudioDataIndx >= AudioData.Length)
                        AudioDataIndx = 0;

                    //TestShortData(dataToSendAudio, iFixedLengthAudio / 2);

                    int iRet;
                    iRet = P2PWrapper.SendVideoData(200, dataToSend, iFixedLength, 0, 0);
                    iRet = P2PWrapper.SendAudioData(200, dataToSendAudio, iFixedLengthAudio / 2);
                    System.IO.File.AppendAllText("log.txt", "P2PWrapper.SendAudioData = " + iRet + "\r\n");
                    Thread.Sleep(33);

                }
            }


            byte[] tempByte = new byte[160];
            int tempByteCheck = 0;
            public void TestShortData(short[] dataToSendAudio, int iLen)
            {
                for (int i = 0; i < iLen; i++)
                {
                    byte left = (byte)(dataToSendAudio[i] >> 8);
                    byte right = (byte)(dataToSendAudio[i]);
                    tempByte[i * 2] = left;
                    tempByte[i * 2 + 1] = right;
                }


                if (tempByteCheck == 0)
                {
                    System.IO.File.WriteAllBytes("TestShortData.pcm", tempByte);
                    tempByteCheck++;
                }
                else
                {
                    System.Console.WriteLine("Writing to TestShortData.pcm");
                    P2PWrapper.AppendAllBytes("TestShortData.pcm", tempByte);
                }

            }

        };
        #endregion "Class By VideoTeam"


        long loginId = 123L;
        long friendID = 321L;
        string sessionID = "session";
        string ip = "192.168.1.38";
        int port = 1250;
        public WNLib2005()
        {
            InitializeComponent();
            this.DataContext = this;
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

            //string sIP = "38.127.68.60";
            
            /*string sIP = "192.168.8.28";
            //string sIP = "60.68.127.38";
            int iFriendPort = 60001;

            int iRet;
            iRet = P2PWrapper.InitializeLibrary(200);



            //string text = System.IO.File.ReadAllText(@"E:\dummy.txt");

            Alpha oAlpha = new Alpha();

            oAlpha.VideoData = System.IO.File.ReadAllBytes(@"VideoSample.yuv");
            oAlpha.AudioData = System.IO.File.ReadAllBytes(@"AudioSample.pcm");
            //oAlpha.AudioData = System.IO.File.ReadAllBytes(@"C:\Users\Nayeem\Downloads\a2002011001-e02-16kHz.wav"); 
            oAlpha.bStartSending = true;

            Thread oThread = new Thread(new ThreadStart(oAlpha.Beta));
            oThread.Start();
            */

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

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Inside StartCall Button");

            string sIP = "192.168.8.30";

            //string sIP = "38.127.68.60";
            //string sIP = "60.68.127.38";
            int iFriendPort = 60003;
            int iRet = 0;
            P2PWrapper p2pWrapper = P2PWrapper.GetInstance();
            iRet = p2pWrapper.InitializeLibraryR(100/*UserID*/);
            System.Console.WriteLine("MediaEngineLib==> InitializeLibrary, iRet = " + iRet);
            p2pWrapper.CreateSessionR(200/*FriendID*/, 1/*Audio*/, sIP, iFriendPort);
            p2pWrapper.CreateSessionR(200, 2/*Video*/, sIP, iFriendPort);
            p2pWrapper.SetRelayServerInformationR(200, 1, sIP, iFriendPort);
            p2pWrapper.SetRelayServerInformationR(200, 2, sIP, iFriendPort);
            iRet = p2pWrapper.StartAudioCallR(200);
            iRet = p2pWrapper.StartVideoCallR(200, 288  /*Height*/, 352/*Width*/);
            System.Diagnostics.Debug.WriteLine("MediaEngineLib==> StartVideoCall, iRet = " + iRet);
            p2pWrapper.SetLoggingStateR(true, 5);
            p2pWrapper.LinkWithConnectivityLib(null);

        }

        private void Stop_Click_1(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Inside EndCall Button");
            P2PWrapper p2pWrapper = P2PWrapper.GetInstance();
            p2pWrapper.StopAudioCallR(200);
            p2pWrapper.StopAudioCallR(200);
            p2pWrapper.CloseSessionR(200, 1);
            p2pWrapper.CloseSessionR(200, 2);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            P2PWrapper p2pWrapper = P2PWrapper.GetInstance();
            Console.WriteLine("Inside CheckCap Button");
            p2pWrapper.InitializeLibraryR(100/*UserID*/);
            p2pWrapper.LinkWithConnectivityLib(null);
            //p2pWrapper.CheckDeviceCapabilityR(100, 640, 480, 352, 288);

            //p2pWrapper.CheckDeviceCapabilityR(100, 480, 640, 288, 352);
            
        }
    }
}
