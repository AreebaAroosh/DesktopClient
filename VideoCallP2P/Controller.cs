using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using VideoCallP2P.Libconnector;

namespace VideoCallP2P
{
    class Controller
    {
        public int MEDIA_TYPE_AUDIO = 1;
        public int MEDIA_TYPE_VIDEO = 2;
        public int MEDIA_TYPE_LIVE_STREAM = 3;

        public int SERVICE_TYPE_CALL = 11;
        public int SERVICE_TYPE_SELF_CALL = 13;

        public int ENTITY_TYPE_CALLER = 31;

        public int SERVICE_TYPE_LIVE_STREAM = 12;
        public int SERVICE_TYPE_SELF_STREAM = 14;

        public int ENTITY_TYPE_PUBLISHER = 31;
        public int ENTITY_TYPE_VIEWER = 32;
        public int ENTITY_TYPE_VIEWER_CALLEE = 2;
        public int ENTITY_TYPE_PUBLISHER_CALLER = 1;

        public int PUBLISHER_IN_CALL = 1;
        public int VIEWER_IN_CALL = 2;
        public int CALL_NOT_RUNNING = 4;

        public int CALL_IN_LIVE_TYPE_AUDIO_ONLY = 1;
        public int CALL_IN_LIVE_TYPE_VIDEO_ONLY = 2;
        public int CALL_IN_LIVE_TYPE_AUDIO_VIDEO = 3;

        public void SignalingMessageProcessor()
        {
            SignalModel signalingModel;
            Console.WriteLine("Inside SignalingMessgeProcessor");
            while (true)
            {
                if (P2PWrapper.signalDataQueue.Count <= 0)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    signalingModel = P2PWrapper.signalDataQueue.Dequeue();

                    Console.Write("SignalingMessageProcessor:   ");
                    for (int i = 0; i < signalingModel.iLen; i++)
                    {
                        Console.Write(" " + signalingModel.data[i]);
                    }
                    Console.WriteLine("");

                    Handle_Signaling_Message(signalingModel.data, signalingModel.iLen);

                }
            }
        }

        public void Handle_Signaling_Message(byte[] buffer, int iLen)
        {
            int startIndex = 1;

            Console.WriteLine("");
            Console.WriteLine("Handle_Signaling_Message -->");
            for (int i = 0; i < iLen; i++)
            {
                Console.Write(buffer[i] + " ");
            }
            Console.WriteLine("");

            if (buffer[0] == Constants.REPLY_REGISTER_MESSAGE)
            {
                int userID = ByteToInt(buffer, startIndex); startIndex += 4;
                string sValue = "UserID: " + userID.ToString();
                DesktopClientGUI.Instance.UpdateUserLabel(sValue);

            }
            else if (buffer[0] == Constants.INVITE_MESSAGE)
            {
                //[[TestCameraViewController GetInstance] StartAllThreads];
                //[[TestCameraViewController GetInstance] InitializeCameraAndMicrophone];
                //[[TestCameraViewController GetInstance] InitializeAudioVideoEngineForCall];
                InitializeAudioVideoEngineForCall(288, 352);

            }
            else if (buffer[0] == Constants.PUBLISHER_INVITE_MESSAGE)
            {
                int publisherID = ByteToInt(buffer, startIndex); startIndex += 4;
                int myViewerID = ByteToInt(buffer, startIndex); startIndex += 4;
                int targetServerMediaPort = ByteToInt(buffer, startIndex); startIndex += 4;
                int iInsetID = buffer[startIndex++];
                //printf("TestCamera--> PUBLISHER_INVITE_MESSAGE, publisherID --> videwerID = %d --> %d, insetID = %d VIEWER_IN_CALL\n", publisherID, myViewerID, iInsetID);
                P2PWrapper.GetInstance().StartCallInLiveR(200, VIEWER_IN_CALL, CALL_IN_LIVE_TYPE_AUDIO_VIDEO/*, iInsetID*/);
            }
            else if (buffer[0] == Constants.REPLY_PUBLISHER_INVITE_MESSAGE)
            {

                int publisherID = ByteToInt(buffer, startIndex); startIndex += 4;
                int myViewerID = ByteToInt(buffer, startIndex); startIndex += 4;
                int targetServerMediaPort = ByteToInt(buffer, startIndex); startIndex += 4;
                int iInsetID = buffer[startIndex++];
                //printf("TestCamera--> REPLY_PUBLISHER_INVITE_MESSAGE, publisherID --> videwerID = %d --> %d, insetID = %d PUBLISHER_IN_CALL\n", publisherID, myViewerID, iInsetID);
                P2PWrapper.GetInstance().StartCallInLiveR(200, PUBLISHER_IN_CALL, CALL_IN_LIVE_TYPE_AUDIO_VIDEO/*, iInsetID*/);
            }
            else if (buffer[0] == Constants.VIEWER_INVITE_MESSAGE)
            {
                //CVideoAPI::GetInstance()->StartCallInLive(200, PUBLISHER_IN_CALL, CALL_IN_LIVE_TYPE_AUDIO_VIDEO);
                P2PWrapper.GetInstance().StartCallInLiveR(200, PUBLISHER_IN_CALL, CALL_IN_LIVE_TYPE_AUDIO_VIDEO);

            }
            else if (buffer[0] == Constants.TERMINATE_ALL_MESSAGE)
            {
                //[[TestCameraViewController GetInstance] UnInitializeAudioVideoEngine];
                //[[TestCameraViewController GetInstance] UnInitializeCameraAndMicrophone];
                //[[TestCameraViewController GetInstance] CloseAllThreads];

                UnInitializeAudioVideoEngine();
            }
        }

        public int ByteToInt(byte[] data, int startIndex)
        {
            int ret = 0;

            ret += (int)(data[startIndex++] & 0xFF) << 24;
            ret += (int)(data[startIndex++] & 0xFF) << 16;
            ret += (int)(data[startIndex++] & 0xFF) << 8;
            ret += (int)(data[startIndex++] & 0xFF);

            return ret;
        }

        public int InitializeAudioVideoEngineForLive(int iHeight, int iWidth, bool isPublisher)
        {
            //If We need Live
            P2PWrapper p2pWrapper;
            p2pWrapper = P2PWrapper.GetInstance();

            int iRet;
            long sessionID = 200;

            if (isPublisher)
                iRet = p2pWrapper.StartAudioCallR(sessionID, SERVICE_TYPE_LIVE_STREAM, ENTITY_TYPE_PUBLISHER, 1);
            else
                iRet = p2pWrapper.StartAudioCallR(sessionID, SERVICE_TYPE_LIVE_STREAM, ENTITY_TYPE_VIEWER, 1);


            if (isPublisher)
                iRet = p2pWrapper.StartVideoCallR(sessionID, iHeight, iWidth, SERVICE_TYPE_LIVE_STREAM, ENTITY_TYPE_PUBLISHER,1000/*packetSize*/, /*NetworkType*/ 0, /*bIsAudioOnlyLive*/false);
            else
                iRet = p2pWrapper.StartVideoCallR(sessionID, iHeight, iWidth, SERVICE_TYPE_LIVE_STREAM, ENTITY_TYPE_VIEWER, 1000/*PacketSize*/, /*NetworkType*/ 0, /*bIsAudioOnlyLive*/false);

            return iRet;
        }

        public int InitializeAudioVideoEngineForCall(int iHeight, int iWidth)
        {
            //If We need Call
    
            int iRet = 1;
            long sessionID = 200;
            int iDeviceCapability = 207;
            P2PWrapper p2pWrapper;
            p2pWrapper = P2PWrapper.GetInstance();
    
            //p2pWrapper.SetDeviceCapabilityResultsR(iDeviceCapability, 640, 480, 352, 288);

            p2pWrapper.StartAudioCallR(sessionID, SERVICE_TYPE_CALL, ENTITY_TYPE_CALLER, 1);
            p2pWrapper.StartVideoCallR(sessionID, iHeight, iWidth, SERVICE_TYPE_CALL, ENTITY_TYPE_CALLER, 1000/*PacketSize*/, /*NetworkType*/ 0, /*bIsAudioOnlyLive*/false);
            return iRet;
        }

        public int UnInitializeAudioVideoEngine()
        {
            P2PWrapper.GetInstance().StopAudioCallR(200);
            P2PWrapper.GetInstance().StopVideoCallR(200);
            return 1;
    
        }


        private static Controller controllerInstance;

        public static Controller GetInstance()
        {
            if (controllerInstance == null)
            {
                controllerInstance = new Controller();
            }
            return controllerInstance;
        }

    }
}
