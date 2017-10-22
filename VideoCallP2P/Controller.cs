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
                //char cConvertedCharArray[12];
                //sprintf(cConvertedCharArray, "%d", userID);
                //string sValue = "UserID = " + std::string(cConvertedCharArray);
                //[[TestCameraViewController GetInstance] UpdateUserID:sValue];

            }
            else if (buffer[0] == Constants.INVITE_MESSAGE)
            {
                //[[TestCameraViewController GetInstance] StartAllThreads];
                //[[TestCameraViewController GetInstance] InitializeCameraAndMicrophone];
                //[[TestCameraViewController GetInstance] InitializeAudioVideoEngineForCall];

            }
            else if (buffer[0] == Constants.PUBLISHER_INVITE_MESSAGE)
            {
                int publisherID = ByteToInt(buffer, startIndex); startIndex += 4;
                int myViewerID = ByteToInt(buffer, startIndex); startIndex += 4;
                int targetServerMediaPort = ByteToInt(buffer, startIndex); startIndex += 4;
                int iInsetID = buffer[startIndex++];
                //printf("TestCamera--> PUBLISHER_INVITE_MESSAGE, publisherID --> videwerID = %d --> %d, insetID = %d VIEWER_IN_CALL\n", publisherID, myViewerID, iInsetID);
                //CVideoAPI::GetInstance()->StartCallInLive(200, VIEWER_IN_CALL, CALL_IN_LIVE_TYPE_AUDIO_VIDEO/*, iInsetID*/);
            }
            else if (buffer[0] == Constants.REPLY_PUBLISHER_INVITE_MESSAGE)
            {

                int publisherID = ByteToInt(buffer, startIndex); startIndex += 4;
                int myViewerID = ByteToInt(buffer, startIndex); startIndex += 4;
                int targetServerMediaPort = ByteToInt(buffer, startIndex); startIndex += 4;
                int iInsetID = buffer[startIndex++];
                //printf("TestCamera--> REPLY_PUBLISHER_INVITE_MESSAGE, publisherID --> videwerID = %d --> %d, insetID = %d PUBLISHER_IN_CALL\n", publisherID, myViewerID, iInsetID);
                //CVideoAPI::GetInstance()->StartCallInLive(200, PUBLISHER_IN_CALL, CALL_IN_LIVE_TYPE_AUDIO_VIDEO/*, iInsetID*/);
            }
            else if (buffer[0] == Constants.VIEWER_INVITE_MESSAGE)
            {
                //CVideoAPI::GetInstance()->StartCallInLive(200, PUBLISHER_IN_CALL, CALL_IN_LIVE_TYPE_AUDIO_VIDEO);
            }
            else if (buffer[0] == Constants.TERMINATE_ALL_MESSAGE)
            {
                //[[TestCameraViewController GetInstance] UnInitializeAudioVideoEngine];
                //[[TestCameraViewController GetInstance] UnInitializeCameraAndMicrophone];
                //[[TestCameraViewController GetInstance] CloseAllThreads];
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



    }
}
