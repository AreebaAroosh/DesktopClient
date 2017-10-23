using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace VideoCallP2P.Libconnector
{
    public class SignalModel
    {
        public int iLen;
        public byte[] data;

        public SignalModel(byte[] d, int len)
        {
            data = d;
            iLen = len;
        }
    }
    public class P2PWrapper : OnReceiveListner
    {
        const string RingIDSDK_Path = "RingIDSDK_Desktop.dll";

        public static Queue<byte[]> framesQueue = new Queue<byte[]>();
        public static Queue<SignalModel> signalDataQueue = new Queue<SignalModel>();
        public static int iRenderWidth, iRenderHeight;


        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int StartVideoCall(long llFriendID, int nVideoHeight, int nVideoWidth, int nServiceType, int nEntityType, int packetSizeOfNetwork, int nNetworkType, bool bAudioOnlyLive);

        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int StartAudioCall(long lFriendID, int nServiceType, int entityType, int nAudioSpeakerType);

        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitializeLibrary(long username);
        
        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ipv_Release();
        

        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SendVideoData(long lFriendID, byte[] in_data, int in_size, int orientation_type, int cameraOrient);

        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SendAudioData(long lFriendID, short[] in_data, int in_size);

        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int StopVideoCall(long lFriendID);
        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int StopAudioCall(long lFriendID);

        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CheckDeviceCapability(long lFriendID, int iHeightHigh, int iWidthHigh, int iHeightLow, int iWidthLow);

        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int StartCallInLive(long llFriendID, int iRole, int nCallInLiveType);

        

        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitializeMediaConnectivity(string sServerIP, int iPort, int iLogLevel);

        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern int  ProcessCommand(string sCommand);

        //PORT int InitializeMediaConnectivity(const char* sServerIP, int iPort, int iLogLevel);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Delegate_SetNotifyClientWithPacketCallback(long FriendId, IntPtr packet, int iLen);
        Delegate_SetNotifyClientWithPacketCallback Obj_SetNotifyClientWithPacketCallback;
        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNotifyClientWithPacketCallback([MarshalAs(UnmanagedType.FunctionPtr)] Delegate_SetNotifyClientWithPacketCallback ii);


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Delegate_SetNotifyClientWithVideoDataCallback(long FriendId, int eventType, IntPtr frame, int iLen, int iHeight, int iWidth, int cameraOrient);
        Delegate_SetNotifyClientWithVideoDataCallback Obj_SetNotifyClientWithVideoDataCallback;
        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNotifyClientWithVideoDataCallback([MarshalAs(UnmanagedType.FunctionPtr)] Delegate_SetNotifyClientWithVideoDataCallback ii);


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Delegate_SetNotifyClientWithAudioDataCallback(long FriendId, int eventType, IntPtr data, int in_size);
        Delegate_SetNotifyClientWithAudioDataCallback Obj_SetNotifyClientWithAudioDataCallback;
        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNotifyClientWithAudioDataCallback([MarshalAs(UnmanagedType.FunctionPtr)] Delegate_SetNotifyClientWithAudioDataCallback ii);


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Delegate_SetNotifyClientWithVideoNotificationCallback(long FriendId, int eventType);
        Delegate_SetNotifyClientWithVideoNotificationCallback Obj_SetNotifyClientWithVideoNotificationCallback;

        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNotifyClientWithVideoNotificationCallback([MarshalAs(UnmanagedType.FunctionPtr)] Delegate_SetNotifyClientWithVideoNotificationCallback ii);


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void Delegate_SetNotifynotifyClientMethodWithSignalingDataCallback(IntPtr data, int iLen);
        Delegate_SetNotifynotifyClientMethodWithSignalingDataCallback Obj_SetNotifynotifyClientMethodWithSignalingDataCallback;

        [DllImport(RingIDSDK_Path, CallingConvention = CallingConvention.Cdecl)]
        public static extern void SetNotifynotifyClientMethodWithSignalingDataCallback([MarshalAs(UnmanagedType.FunctionPtr)] Delegate_SetNotifynotifyClientMethodWithSignalingDataCallback ii);





        void notifyClientMethod(int eventType)
        {
        }
        void notifyClientMethodForFriend(int eventType, long friendName, int mediaName)
        {
            uiCommunicator.process_notifyClientMethodForFriend(eventType, friendName, mediaName);
        }
        void notifyClientMethodWithReceivedBytes(int eventType, long friendName, int mediaName, int dataLength, IntPtr data2)
        {
            byte[] data = new byte[dataLength];
            Marshal.Copy(data2, data, 0, dataLength);
            if (mediaName == ConfigFile.MediaType.IPV_MEDIA_AUDIO && eventType == ConfigFile.EventType.DATA_REVEIVED)
            {
                OnVoiceSignalReceive(data, data.Length);
            }
        }


        void PacketsFromLibrary(long FriendId, IntPtr packet, int iLen)
        {
            System.IO.File.WriteAllText("log.txt", "Inside PacketsFromLibrary, iLen = " + iLen + "\r\n");
        }

        int iTemp = 0;
        int iTempAudio = 0;
        byte[] managedArray = new byte[640*480*3];
        short[] managedArrayAudio = new short[2000];
        byte[] signalingDataArray = new byte[10000];

        public static void AppendAllBytes(string path, byte[] bytes)
        {
            //argument-checking here.

            using (var stream = new FileStream(path, FileMode.Append))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        public long GetCurrentTimeStamp()
        {
            long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            return milliseconds;
        }



        long lastFrameFromLibrary = 0;

        void FramesFromLibrary(long FriendId, int eventType, IntPtr frame, int iLen, int iHeight, int iWidth, int cameraOrient)
        {
            Console.WriteLine("FramesFromLibrary: iLen = " + iLen);

             //Console.WriteLine("TheKing--> Receive Diff = " + (GetCurrentTimeStamp() - lastFrameFromLibrary));
             //lastFrameFromLibrary = GetCurrentTimeStamp();

             iRenderWidth = iWidth;
             iRenderHeight = iHeight;
             Marshal.Copy(frame, managedArray, 0, iLen);
             framesQueue.Enqueue(managedArray);
            
            /*if (iLen > 0)
            {
                if (iTemp == 0)
                {
                    //System.IO.File.WriteAllText("log2.txt", "Inside FramesFromLibrary, iLen = " + iLen + ", iHeight = " + iHeight + ", iWidth = " + iWidth + "\r\n");

                    System.IO.File.WriteAllBytes("incoming_RGB24.test", managedArray);

                    iTemp++;
                }
                else
                {
                    //System.IO.File.AppendAllText("log2.txt", "Inside FramesFromLibrary, iLen = " + iLen + ", iHeight = " + iHeight + ", iWidth = " + iWidth + "\r\n");
                    AppendAllBytes("incoming_RGB24.test", managedArray);
                }
            }*/

        }

        void AudioFromLibrary(long FriendId, int eventType,  IntPtr data, int in_size)
        {
            return;

            Marshal.Copy(data, managedArrayAudio, 0, in_size);

            byte[] newAudio = new byte[in_size * 2];
            for (int i = 0; i < in_size;i++)
            {
                newAudio[i*2]       = (byte)(managedArrayAudio[i] >> 8);
                newAudio[i * 2 + 1] = (byte)(managedArrayAudio[i] & 0xFF);
            }



                if (iTempAudio == 0)
                {
                    System.IO.File.WriteAllText("log3.txt", "Inside AudioFromLibrary, iLen = " + in_size + "\r\n");

                    System.IO.File.WriteAllBytes("incoming.pcm", newAudio);

                    iTempAudio++;
                }
                else
                {
                    System.IO.File.AppendAllText("log3.txt", "Inside AudioFromLibrary, iLen = " + in_size + "Managed array size = " + managedArray.Length + "\r\n");
                    AppendAllBytes("incoming.pcm", newAudio);
                }
        }

        void VideoNotification(long FriendId, int eventType)
        {
            Console.WriteLine("TheKing--> VideoNotification eventType = " + eventType);
        }

        void SignalingDataFromLibrary(IntPtr data, int iLen)
        {
            Marshal.Copy(data, signalingDataArray, 0, iLen);
            SignalModel sm = new SignalModel(signalingDataArray, iLen);
            signalDataQueue.Enqueue(sm);
        }
        //EXPORT void SetNotifyClientWithPacketCallback(void(*callBackFunctionPointer)(long long, unsigned char*, int));

        /**/
        public void initDelegateMethods()
        {

            Obj_SetNotifyClientWithPacketCallback = new Delegate_SetNotifyClientWithPacketCallback(PacketsFromLibrary);
            SetNotifyClientWithPacketCallback(Obj_SetNotifyClientWithPacketCallback);

            Obj_SetNotifyClientWithVideoDataCallback = new Delegate_SetNotifyClientWithVideoDataCallback(FramesFromLibrary);
            SetNotifyClientWithVideoDataCallback(Obj_SetNotifyClientWithVideoDataCallback);



            Obj_SetNotifyClientWithAudioDataCallback = new Delegate_SetNotifyClientWithAudioDataCallback(AudioFromLibrary);
            SetNotifyClientWithAudioDataCallback(Obj_SetNotifyClientWithAudioDataCallback);


            Obj_SetNotifyClientWithVideoNotificationCallback = new Delegate_SetNotifyClientWithVideoNotificationCallback(VideoNotification);
            SetNotifyClientWithVideoNotificationCallback(Obj_SetNotifyClientWithVideoNotificationCallback);

            Obj_SetNotifynotifyClientMethodWithSignalingDataCallback = new Delegate_SetNotifynotifyClientMethodWithSignalingDataCallback(SignalingDataFromLibrary);
            SetNotifynotifyClientMethodWithSignalingDataCallback(Obj_SetNotifynotifyClientMethodWithSignalingDataCallback);


        }

        private static P2PWrapper _instance;

        public static P2PWrapper GetInstance()
        {
            if(_instance == null)
            {
                _instance = new P2PWrapper();
            }
            return _instance;
        }
        
        public static P2PWrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new P2PWrapper();

                }
                return _instance;
            }
        }

        public int InitializeLibraryR(long iUserName)
        {
            return InitializeLibrary(iUserName);
        }


        
        public int StartAudioCallR(long lFriendID, int nServiceType, int entityType, int nAudioSpeakerType)
        {
            return StartAudioCall(lFriendID, nServiceType, entityType, nAudioSpeakerType);
        }

        


        public int StartVideoCallR(long llFriendID, int nVideoHeight, int nVideoWidth, int nServiceType, int nEntityType, int packetSizeOfNetwork, int nNetworkType, bool bAudioOnlyLive)
        {
            return StartVideoCall( llFriendID, nVideoHeight, nVideoWidth, nServiceType, nEntityType, packetSizeOfNetwork, nNetworkType, bAudioOnlyLive); 
        }

        public int SendVideoDataR(long lFriendID, byte[] in_data, int in_size, int orientation_type, int cameraOrient)
        {
            return SendVideoData(lFriendID, in_data, in_size, orientation_type, cameraOrient);
        }
        public int StopVideoCallR(long lFriendID)
        {
            return StopVideoCall(lFriendID);
        }
        
        public int StopAudioCallR(long lFriendID)
        {
            return StopAudioCall(lFriendID);
        }

        public int StartCallInLiveR(long llFriendID, int iRole, int nCallInLiveType)
        {
            return StartCallInLive(llFriendID, iRole, nCallInLiveType);
        }

        public int CheckDeviceCapabilityR(long lFriendID, int iHeightHigh, int iWidthHigh, int iHeightLow, int iWidthLow)
        {
            Console.WriteLine("Inside CheckDeviceCapabilityR function");
            return CheckDeviceCapability(lFriendID, iHeightHigh, iWidthHigh, iHeightLow, iWidthLow);
        }

        

        public int InitializeMediaConnectivityR(string sServerIP, int iPort, int iLogLevel)
        {
            return InitializeMediaConnectivity(sServerIP, iPort, iLogLevel);

        }
        public int ProcessCommandR(string sCommand)
        {
            return ProcessCommand(sCommand);
        }

        UiCommunicator uiCommunicator;
        public void LinkWithConnectivityLib(UiCommunicator uiCommunicator2)
        {
            _instance.initDelegateMethods();
            this.uiCommunicator = uiCommunicator2;

        }
        #region SendRTP after encode
        int[] packetNumber = { 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
        Random rand = new Random();
        public int SendRTP(byte[] rowData, long friendID)
        {
            //int frame_size_in_byte = packetNumber[0] * 160;
            //byte[] packet_buffer = new byte[(frame_size_in_byte / 160) * 10];
            //short[] shorts = new short[rowData.Length / 2];
            //Buffer.BlockCopy(rowData, 0, shorts, 0, rowData.Length);
            //codec.Encode(shorts, 0, packet_buffer, frame_size_in_byte / 2);
            //int ran = rand.Next(VoiceConstants.NUMBER_OF_MAX_GARBAGE);
            //byte[] sendingBytes = new byte[packet_buffer.Length + 1 + ran];
            //sendingBytes[0] = (byte)0;
            //Buffer.BlockCopy(packet_buffer, 0, sendingBytes, 1, packet_buffer.Length);
            //return ipv_Send(friendID, ConfigFile.MediaType.IPV_MEDIA_AUDIO, sendingBytes, sendingBytes.Length);

            //Rajib
            return /*ipv_Send(friendID, ConfigFile.MediaType.IPV_MEDIA_AUDIO, rowData, rowData.Length)*/ 0;
        }
        public int SendToVoiceBindingPort(byte[] rowData, long friendID)
        {
            return /*ipv_Send(friendID, ConfigFile.MediaType.IPV_MEDIA_AUDIO, rowData, rowData.Length)*/0;
        }
        #endregion
        #region OnReceiveListner Members
        public void OnVoiceSignalReceive(byte[] receivedBuffer, int length)
        {
            int packetType = receivedBuffer[0];
            VoiceMessageDTO messageDTO = null;
            switch (packetType)
            {
                case VoiceConstants.VOICE_MEDIA: //0
                    int receivedLength = length - 1;
                    length = receivedLength - (receivedLength % 10);
                    byte[] audioData = new byte[length];
                    Buffer.BlockCopy(receivedBuffer, 1, audioData, 0, length);
                    short[] decoded = new short[(length / 10) * 80];
                    //   codec.Decode(audioData, decoded, length);
                    byte[] output_audio_stream = new byte[decoded.Length * 2];
                    Buffer.BlockCopy(decoded, 0, output_audio_stream, 0, output_audio_stream.Length);
                    uiCommunicator.process_RTP(output_audio_stream, output_audio_stream.Length);
                    break;
                case VoiceConstants.VOICE_REGISTER_CONFIRMATION://3
                    messageDTO = VoicePacketGenerator.getRegisterConfirmationPacket(receivedBuffer);
                    uiCommunicator.process_VOICE_REGISTER_CONFIRMATION(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.CALLING://5
                    messageDTO = VoicePacketGenerator.getSignalingPacket(receivedBuffer);
                    uiCommunicator.process_CALLING(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.RINGING://6
                    messageDTO = VoicePacketGenerator.getSignalingPacket(receivedBuffer);
                    uiCommunicator.process_RINGING(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.IN_CALL://7
                    messageDTO = VoicePacketGenerator.getIncallPacket(receivedBuffer);
                    uiCommunicator.process_IN_CALL(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.ANSWER://8
                    messageDTO = VoicePacketGenerator.getSignalingPacketAnwser(receivedBuffer);
                    uiCommunicator.process_ANSWER(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.BUSY://9
                    messageDTO = VoicePacketGenerator.getSignalingPacket(receivedBuffer);
                    uiCommunicator.process_BUSY(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.CANCELED://10
                    messageDTO = VoicePacketGenerator.getSignalingPacket(receivedBuffer);
                    uiCommunicator.process_CANCELED(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.CONNECTED://11
                    messageDTO = VoicePacketGenerator.getSignalingPacket(receivedBuffer);
                    uiCommunicator.process_CONNECTED(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.DISCONNECTED://12
                    messageDTO = VoicePacketGenerator.getSignalingPacket(receivedBuffer);
                    uiCommunicator.process_DISCONNECTED(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.BYE://13
                    messageDTO = VoicePacketGenerator.getSignalingPacket(receivedBuffer);
                    uiCommunicator.process_BYE(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.NO_ANSWER://15
                    messageDTO = VoicePacketGenerator.getSignalingPacket(receivedBuffer);
                    uiCommunicator.process_NO_ANSWER(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.IN_CALL_CONFIRMATION://18
                    messageDTO = VoicePacketGenerator.getIncallPacket(receivedBuffer);
                    uiCommunicator.process_IN_CALL_CONFIRMATION(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.VOICE_REGISTER_PUSH://20
                    messageDTO = VoicePacketGenerator.getPushConfirmationPacket(receivedBuffer);
                    uiCommunicator.process_VOICE_REGISTER_PUSH(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.VOICE_REGISTER_PUSH_CONFIRMATION://21
                    messageDTO = VoicePacketGenerator.getPushConfirmationPacket(receivedBuffer);
                    uiCommunicator.process_VOICE_REGISTER_PUSH_CONFIRMATION(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.VOICE_CALL_HOLD://22
                    messageDTO = VoicePacketGenerator.getSignalingPacket(receivedBuffer);
                    uiCommunicator.process_VOICE_CALL_HOLD(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.VOICE_CALL_HOLD_CONFIRMATION://23
                    messageDTO = VoicePacketGenerator.getSignalingPacket(receivedBuffer);
                    uiCommunicator.process_VOICE_CALL_HOLD_CONFIRMATION(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.VOICE_CALL_UNHOLD://24
                    messageDTO = VoicePacketGenerator.getSignalingPacket(receivedBuffer);
                    uiCommunicator.process_VOICE_CALL_UNHOLD(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.VOICE_UNHOLD_CONFIRMATION://25
                    messageDTO = VoicePacketGenerator.getSignalingPacket(receivedBuffer);
                    uiCommunicator.process_VOICE_UNHOLD_CONFIRMATION(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.VOICE_BUSY_MESSAGE://26
                    messageDTO = VoicePacketGenerator.getVoiceBusyMsg(receivedBuffer);
                    uiCommunicator.process_VOICE_BUSY_MESSAGE(messageDTO);
                    break;
                case VoiceConstants.CALL_STATE.VOICE_BUSY_MESSAGE_CONFIRMATION://27
                    messageDTO = VoicePacketGenerator.getSignalingPacket(receivedBuffer);
                    uiCommunicator.process_VOICE_BUSY_MESSAGE_CONFIRMATION(messageDTO);
                    break;
            }
            if (messageDTO != null && messageDTO.PacketType > 0 && messageDTO.PacketID != null)
            {
                System.Diagnostics.Debug.WriteLine(messageDTO);
            }
        }

        #endregion
    }

}
