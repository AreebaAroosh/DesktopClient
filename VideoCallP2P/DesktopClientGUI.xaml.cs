using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class DesktopClientGUI : Window
    {
        #region "Class By VideoTeam"

        public ObservableCollection<string> MyOperationCmbContent { get; private set; }

        public class AudioSender
        {
            public byte[] AudioData;
            public int AudioDataIndx = 0;
            public void StartSendingAudio()
            {
                while(true)
                {
                    const int iFixedLengthAudio = 1920;
                    short[] dataToSendAudio = new short[iFixedLengthAudio + 5];
                    ushort[] dataToSendAudio2 = new ushort[iFixedLengthAudio + 5];

                    if (AudioDataIndx == 0)
                        System.Console.WriteLine("C# audio data for the 1st frame");
                    for (int i = 0; i < iFixedLengthAudio / 2; i++)
                    {
                        if((AudioDataIndx+2*i +1) >= iFixedLengthAudio/2)
                        {
                            break;
                        }
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

                    int iRet = P2PWrapper.SendAudioData(200, dataToSendAudio, iFixedLengthAudio / 2);
                    System.IO.File.AppendAllText("log.txt", "P2PWrapper.SendAudioData = " + iRet + "\r\n");
                    Thread.Sleep(100);

                }
            }
        }
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

        public void UpdateUserLabel(string sValue)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {//this refer to form in WPF application 
                useridLbl.Content = sValue;
            }));


        }
        #endregion "Class By VideoTeam"

        public DesktopClientGUI()
        {
            MyOperationCmbContent = new ObservableCollection<string>
            {
                "register",
                "invite", 
                "publish",
                "view",
                "terminate-all"
            };

            InitializeComponent();
            this.DataContext = this;
            Instance = this;
        }


        public static DesktopClientGUI Instance { get; private set; }


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

        }
        private void Button_Disconnect(object sender, RoutedEventArgs e)
        {
            //P2PWrapper.ipv_CloseSession(friendID, ConfigFile.MediaType.IPV_MEDIA_AUDIO);
        }
        private void Button_Destroy(object sender, RoutedEventArgs e)
        {
            P2PWrapper.ipv_Release();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Inside StartCall Button");
            useridLbl.Content = "UserID: [Trying to Find One....]";

            //Controller oController = new Controller();
            Thread oControllerThread = new Thread(new ThreadStart(Controller.GetInstance().SignalingMessageProcessor));
            oControllerThread.Start();



            //string sIP = "127.0.0.1";
            string sIP = "192.168.8.22";

            int iRet = 0;
            P2PWrapper p2pWrapper = P2PWrapper.GetInstance();
            iRet = p2pWrapper.InitializeLibraryR(200/*UserID*/);
            System.Console.WriteLine("MediaEngineLib==> InitializeLibrary, iRet = " + iRet);

            p2pWrapper.InitializeMediaConnectivityR(sIP /*Server IP*/, 6060 /* Server Signaling Port*/, 1);

            p2pWrapper.LinkWithConnectivityLib(null);

            //AudioSender oAlpha = new AudioSender();
            //oAlpha.AudioData = System.IO.File.ReadAllBytes(@"AudioSending(1).pcm");
            //oAlpha.bStartSending = true;

            //Thread oThread = new Thread(new ThreadStart(oAlpha.StartSendingAudio));
            //oThread.Start();
        }

        private void Stop_Click_1(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Inside EndCall Button");
            P2PWrapper p2pWrapper = P2PWrapper.GetInstance();
            //p2pWrapper.StopAudioCallR(200);
            //p2pWrapper.StopAudioCallR(200);
            //p2pWrapper.CloseSessionR(200, 1);
            //p2pWrapper.CloseSessionR(200, 2);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            P2PWrapper p2pWrapper = P2PWrapper.GetInstance();
            Console.WriteLine("Inside CheckCap Button");
            p2pWrapper.InitializeLibraryR(100/*UserID*/);
            p2pWrapper.LinkWithConnectivityLib(null);
            //p2pWrapper.CheckDeviceCapabilityR(100, 640, 480, 352, 288);

            p2pWrapper.CheckDeviceCapabilityR(100, 480, 640, 288, 352);
            
        }

        private void Process_Click(object sender, RoutedEventArgs e)
        {
            //"register",
            //"invite", 
            //"publish",
            //"view",
            //"terminate-all"
            Console.WriteLine("RajibTheKing--> Inside Process_Click Button");
            string sOperation = "";
            if(operationCmb.SelectedItem != null)
            {
                sOperation = operationCmb.SelectedItem.ToString();
            }
            Console.WriteLine("RajibTheKing--> Selected Operation:  " + sOperation);

            if(sOperation == "")
            {
                MessageBox.Show("You Must Select an Operation");
            }
            else if(sOperation == "register")
            {
                P2PWrapper.GetInstance().ProcessCommandR(sOperation);
            }
            else if (sOperation == "invite" || sOperation == "publish" || sOperation == "view")
            {
                string sTargetUser = targetuserIDBox.Text;
                if(sTargetUser == "")
                {
                    MessageBox.Show("You Must give input a Target User");
                }
                else
                {
                    Console.WriteLine("You are ready to start: "+sOperation);

                    P2PWrapper.GetInstance().ProcessCommandR(sOperation + " " + sTargetUser);
                    if (sOperation == "invite")
                        Controller.GetInstance().InitializeAudioVideoEngineForCall(288, 352);
                    else if (sOperation == "publish")
                        Controller.GetInstance().InitializeAudioVideoEngineForLive(288, 352, true);
                    else if (sOperation == "view")
                        Controller.GetInstance().InitializeAudioVideoEngineForLive(288, 352, false);
                    else
                        Console.WriteLine("Something is wrong");

                }
            }
            else if(sOperation == "terminate-all")
            {
                P2PWrapper.GetInstance().ProcessCommandR(sOperation);
            }
            else
            {
                MessageBox.Show("Something is wrong!!, You have selected : " + sOperation);
            }

            //MessageBox.Show("You have selected : " + sOperation);

        }


    }
}
