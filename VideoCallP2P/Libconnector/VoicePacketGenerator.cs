using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoCallP2P.Libconnector
{
    public class VoicePacketGenerator
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(VoicePacketGenerator).Name);
        public static byte[] makeRegisterPacket(int packetType, long userIdentity, long friendIdentity, string packetID, int calltype)
        {
            byte[] packetIdByte = Encoding.UTF8.GetBytes(packetID);
            int totalDataLenght = 19 + packetIdByte.Length;
            byte[] data = new byte[totalDataLenght];
            int i = 0;
            data[i++] = (byte)packetType;
            i = addUserIDInArray(userIdentity, data, i);
            i = addUserIDInArray(friendIdentity, data, i);
            data[i++] = (byte)packetIdByte.Length;
            for (int n = 0; n < packetIdByte.Length; n++)
            {
                data[i++] = packetIdByte[n];
            }
            data[i++] = (byte)calltype;
            return data;
        }

        public static byte[] makeUnRegisterPacket(int packetType, long userIdentity)
        {
            byte[] data = new byte[9];
            int i = 0;
            data[i++] = (byte)packetType;
            i = addUserIDInArray(userIdentity, data, i);

            return data;
        }

        public static byte[] makeSignalingPacket(int packetType, string packetID, long userIdentity, long friendIdentity)
        {
            byte[] packetIDByte = Encoding.UTF8.GetBytes(packetID);
            int totalDataLenght = 18 + packetIDByte.Length;
            byte[] data = new byte[totalDataLenght];
            int i = 0;
            data[i++] = (byte)packetType;
            i = addUserIDInArray(userIdentity, data, i);
            i = addUserIDInArray(friendIdentity, data, i);
            data[i++] = (byte)packetIDByte.Length;
            for (int n = 0; n < packetIDByte.Length; n++)
            {
                data[i++] = packetIDByte[n];
            }
            return data;
        }
        public static byte[] makeSignalingPacket(int packetType, string packetID, long userIdentity, long friendIdentity, int callType)
        {
            byte[] packetIDByte = Encoding.UTF8.GetBytes(packetID);
            int totalDataLenght = 19 + packetIDByte.Length;
            byte[] data = new byte[totalDataLenght];
            int i = 0;
            data[i++] = (byte)packetType;
            i = addUserIDInArray(userIdentity, data, i);
            i = addUserIDInArray(friendIdentity, data, i);
            data[i++] = (byte)packetIDByte.Length;
            for (int n = 0; n < packetIDByte.Length; n++)
            {
                data[i++] = packetIDByte[n];
            }
            data[i++] = (byte)callType;
            return data;
        }
        public static VoiceMessageDTO getRegisterConfirmationPacket(
                  byte[] receivedBuffer)
        { //Packet Type + Friend Idenity + PacketID Length+Packet ID + Voice Communication Port + Video Communication Port
            VoiceMessageDTO messageDTO = new VoiceMessageDTO();
            int totalRead = 0;
            int packetType = receivedBuffer[0];
            totalRead++;

            long friendIdentity = getLong(receivedBuffer, totalRead, 8);
            totalRead += 8;

            int packetIDLength = receivedBuffer[totalRead];
            totalRead++;
            //   string packetID = new String(receivedBuffer, totalRead, packetIDLength); E
            string packetID = Encoding.UTF8.GetString(receivedBuffer, totalRead, packetIDLength);
            totalRead += packetIDLength;

            int bindingPort = getIntegerFromByte(totalRead, receivedBuffer);
            totalRead += 4;

            //byte[] address = new byte[4];
            //address[0] = receivedBuffer[totalRead++];
            //address[1] = receivedBuffer[totalRead++];
            //address[2] = receivedBuffer[totalRead++];
            //address[3] = receivedBuffer[totalRead++];

            int vedioComunicationPort = (int)getIntegerFromByte(totalRead, receivedBuffer);
            totalRead += 4;
            messageDTO.PacketType = packetType;
            messageDTO.PacketID = packetID;
            messageDTO.VoiceBindingPort = bindingPort;
            messageDTO.FriendIdentity = friendIdentity;
            messageDTO.VideoCommunicationPort = vedioComunicationPort;
            return messageDTO;
        }

        public static VoiceMessageDTO getSignalingPacket(byte[] recievedBuffer)
        {
            VoiceMessageDTO messageDTO = new VoiceMessageDTO();
            int totalRead = 0;
            int packetType = recievedBuffer[0];
            totalRead++;

            long userIdentity = getLong(recievedBuffer, totalRead, 8);
            totalRead += 8;

            long friendIdentity = getLong(recievedBuffer, totalRead, 8);
            totalRead += 8;

            int packetIDLength = recievedBuffer[totalRead];
            totalRead++;
            string packetID = Encoding.UTF8.GetString(recievedBuffer, totalRead, packetIDLength);
            totalRead += packetIDLength;
            messageDTO.PacketType = packetType;
            messageDTO.PacketID = packetID;
            messageDTO.UserIdentity = friendIdentity;
            messageDTO.FriendIdentity = userIdentity;
            return messageDTO;
        }
        public static VoiceMessageDTO getSignalingPacketAnwser(byte[] recievedBuffer)
        {
            VoiceMessageDTO messageDTO = new VoiceMessageDTO();
            int totalRead = 0;
            int packetType = recievedBuffer[0];
            totalRead++;
            long userIdentity = getLong(recievedBuffer, totalRead, 8);
            totalRead += 8;
            long friendIdentity = getLong(recievedBuffer, totalRead, 8);
            totalRead += 8;
            int packetIDLength = recievedBuffer[totalRead];
            totalRead++;
            string packetID = Encoding.UTF8.GetString(recievedBuffer, totalRead, packetIDLength);
            totalRead += packetIDLength;
            int callType = recievedBuffer[totalRead];
            totalRead++;

            messageDTO.PacketType = packetType;
            messageDTO.PacketID = packetID;
            messageDTO.UserIdentity = friendIdentity;
            messageDTO.FriendIdentity = userIdentity;
            messageDTO.CallType = callType;
            return messageDTO;
        }
        public static VoiceMessageDTO getIncallPacket(byte[] recievedBuffer)
        {

            VoiceMessageDTO messageDTO = new VoiceMessageDTO();
            int totalRead = 0;
            int packetType = recievedBuffer[0];
            totalRead++;

            long userIdentity = getLong(recievedBuffer, totalRead, 8);
            totalRead += 8;

            long friendIdentity = getLong(recievedBuffer, totalRead, 8);
            totalRead += 8;

            int packetIDLength = recievedBuffer[totalRead];
            totalRead++;
            string packetID = Encoding.UTF8.GetString(recievedBuffer, totalRead, packetIDLength);// new String(recievedBuffer, totalRead, packetIDLength);
            totalRead += packetIDLength;

            //messageDTO.setPacketType(packetType);
            //messageDTO.setPacketID(packetID);
            //messageDTO.setUserIdentity(friendIdentity);
            //messageDTO.setFriendIdentity(userIdentity);
            messageDTO.PacketType = packetType;
            messageDTO.PacketID = packetID;
            messageDTO.UserIdentity = friendIdentity;
            messageDTO.FriendIdentity = userIdentity;

            return messageDTO;
        }

        public static int getIntegerFromByte(int stratPoint, byte[] bytes)
        {
            int result = 0;
            result += (bytes[stratPoint++] & 0xFF) << 24;
            result += (bytes[stratPoint++] & 0xFF) << 16;
            result += (bytes[stratPoint++] & 0xFF) << 8;
            result += (bytes[stratPoint++] & 0xFF);
            return result;
        }

        public static byte[] makePushPacket(string packetID, long userIdentity, string fullName, long friendIdentity, int platform, int onlineStatus, int appType, string frinedDeviceToken, int calltype = 1)
        {
            byte[] packetIDByte = Encoding.UTF8.GetBytes(packetID);
            byte[] fullNameByte = Encoding.UTF8.GetBytes(fullName);//fullName.getBytes();
            byte[] frinedDeviceTokenByte = Encoding.UTF8.GetBytes(frinedDeviceToken);// frinedDeviceToken.getBytes();
            int deviceTokenLength = frinedDeviceTokenByte.Length;
            int fullNameLength = fullNameByte.Length;

            int totalDataLenght = 25 + packetIDByte.Length + fullNameLength + deviceTokenLength;
            byte[] data = new byte[totalDataLenght];
            int i = 0;
            data[i++] = (byte)VoiceConstants.CALL_STATE.VOICE_REGISTER_PUSH;

            i = addUserIDInArray(userIdentity, data, i);
            i = addUserIDInArray(friendIdentity, data, i);
            data[i++] = (byte)packetIDByte.Length;
            for (int n = 0; n < packetIDByte.Length; n++)
            {
                data[i++] = packetIDByte[n];
            }

            data[i++] = (byte)fullNameLength;
            for (int n = 0; n < fullNameLength; n++)
            {
                data[i++] = fullNameByte[n];
            }

            data[i++] = (byte)platform;
            data[i++] = (byte)onlineStatus;
            data[i++] = (byte)appType;
            data[i++] = (byte)(deviceTokenLength >> 8);
            data[i++] = (byte)(deviceTokenLength);
            try
            {
                for (int n = 0; n < deviceTokenLength; n++)
                {
                    data[i++] = frinedDeviceTokenByte[n];
                }
            }
            catch (Exception ex)
            {
                logger.Error("makePushPacket==>" + ex.Message + " ==>" + ex.StackTrace);
            }
            data[i++] = (byte)calltype;
            i++;
            return data;
        }

        public static VoiceMessageDTO getPushConfirmationPacket(byte[] recievedBuffer)
        {
            VoiceMessageDTO messageDTO = new VoiceMessageDTO();
            int totalRead = 0;
            int packetType = recievedBuffer[0];
            totalRead++;

            long friendIdentity = getLong(recievedBuffer, totalRead, 8);
            totalRead += 8;

            int packetIDLength = recievedBuffer[totalRead];
            totalRead++;
            string packetID = Encoding.UTF8.GetString(recievedBuffer, totalRead, packetIDLength);
            totalRead += packetIDLength;
            messageDTO.PacketType = packetType;
            messageDTO.PacketID = packetID;
            messageDTO.FriendIdentity = friendIdentity;
            return messageDTO;
        }

        //public static VoiceMessageDTO getAddressPacket(byte[] receivedBuffer)
        //{
        //    VoiceMessageDTO messageDTO = new VoiceMessageDTO();
        //    int totalRead = 0;
        //    int packetType = receivedBuffer[0];
        //    totalRead++;
        //    int packetIDLength = receivedBuffer[totalRead];
        //    totalRead++;
        //    String packetID = Encoding.UTF8.GetString(receivedBuffer, totalRead, packetIDLength);
        //    //new String(receivedBuffer, totalRead, packetIDLength);
        //    totalRead += packetIDLength;

        //    byte[] address = new byte[4];
        //    address[0] = receivedBuffer[totalRead++];
        //    address[1] = receivedBuffer[totalRead++];
        //    address[2] = receivedBuffer[totalRead++];
        //    address[3] = receivedBuffer[totalRead++];
        //    int port = (int)getIntegerFromByte(totalRead, receivedBuffer);
        //    totalRead += 4;
        //    //InetAddress inetAddress = null;
        //    //try
        //    //{
        //    //    inetAddress = InetAddress.getByAddress(address);
        //    //}
        //    //catch (UnknownHostException ex)
        //    //{
        //    //}

        //    //     messageDTO.setPacketType(packetType);
        //    //     messageDTO.setPacketID(packetID);
        //    //     messageDTO.setMapPort(port);
        //    messageDTO.PacketType = packetType;
        //    messageDTO.PacketID = packetID;
        //    messageDTO.MapPort = port;
        //    return messageDTO;
        //}

        //public static byte[] makeAddressRequestPacket(int packetType, string packetID, long userIdentity)
        //{
        //    byte[] packetIdByte = Encoding.UTF8.GetBytes(packetID);
        //    int totalDataLenght = 10 + packetIdByte.Length;
        //    byte[] data = new byte[totalDataLenght];
        //    int i = 0;
        //    data[i++] = (byte)packetType;
        //    i = addUserIDInArray(userIdentity, data, i);
        //    data[i++] = (byte)packetIdByte.Length;
        //    for (int n = 0; n < packetIdByte.Length; n++)
        //    {
        //        data[i++] = packetIdByte[n];
        //    }
        //    return data;
        //}

        public static long getLong(byte[] data, int startPoint, int Length)
        {
            long result = 0;
            for (int i = (Length - 1); i > -1; i--)
            {
                result |= (data[startPoint++] & 0xFFL) << (8 * i);
            }
            return result;
        }

        public static byte[] makeBusyMessage(String packetID, long userIdentity, long friendIdentity, String msg)
        { /* Busy Signal Packet with Specific Message Text(VOICE_BUSY_MESSAGE): Packet Type+PacketID Length+Packet ID+User Identity Length+User Idenity+Friend Identity Length+Friend Idenity+Message Length+Message   Packet Type(1 byte) +PacketID Length(1 byte) +Packet ID (1 byte) +User Identity Length(1 byte) +Freind id Length (1 byte) +Message legth (1 byte)  */

            byte[] packetIDByte = Encoding.UTF8.GetBytes(packetID);
            //Packet Type+User Idenity+Friend Idenity+PacketID Length+Packet ID+Message Length+Message 
            byte[] messageByte = Encoding.UTF8.GetBytes(msg);
            int totalDataLenght = 19 + packetIDByte.Length + messageByte.Length;
            byte[] data = new byte[totalDataLenght];
            int i = 0;
            data[i++] = (byte)VoiceConstants.CALL_STATE.VOICE_BUSY_MESSAGE;

            i = addUserIDInArray(userIdentity, data, i);
            i = addUserIDInArray(friendIdentity, data, i);
            data[i++] = (byte)packetIDByte.Length;
            for (int n = 0; n < packetIDByte.Length; n++)
            {
                data[i++] = packetIDByte[n];
            }
            data[i++] = (byte)(messageByte.Length);
            for (int n = 0; n < messageByte.Length; n++)
            {
                data[i++] = messageByte[n];
            }
            return data;
        }

        public static VoiceMessageDTO getVoiceBusyMsg(byte[] recievedBuffer)
        {
            //Packet Type+User Idenity+Friend Idenity+PacketID Length+Packet ID+Message Length+Message

            VoiceMessageDTO messageDTO = new VoiceMessageDTO();
            int totalRead = 0;
            int packetType = recievedBuffer[0];
            totalRead++;

            long userIdentity = getLong(recievedBuffer, totalRead, 8);
            totalRead += 8;

            long friendIdentity = getLong(recievedBuffer, totalRead, 8);
            totalRead += 8;

            int packetIDLength = recievedBuffer[totalRead];
            totalRead++;
            string packetID = Encoding.UTF8.GetString(recievedBuffer, totalRead, packetIDLength);
            //new String(recievedBuffer, totalRead, packetIDLength);
            totalRead += packetIDLength;

            int pushMsgLength = recievedBuffer[totalRead];
            totalRead++;
            string pushMessage = Encoding.UTF8.GetString(recievedBuffer, totalRead, pushMsgLength);//new String(recievedBuffer, totalRead, pushMsgLength);
            totalRead += pushMsgLength;

            //messageDTO.PacketType(packetType);
            //messageDTO.UserIdentity(userId);
            //messageDTO.PacketID(packetID);
            //messageDTO.FriendIdentity(friendIdentity);
            //messageDTO.setVoiceBusyMessage(pushMessage);

            messageDTO.PacketType = packetType;
            messageDTO.PacketID = packetID;
            messageDTO.UserIdentity = friendIdentity;
            messageDTO.FriendIdentity = userIdentity;
            messageDTO.VoiceBusyMessage = pushMessage;
            return messageDTO;
        }

        public static int addUserIDInArray(long userIdentity, byte[] data, int i)
        {
            data[i++] = (byte)(userIdentity >> 56);
            data[i++] = (byte)(userIdentity >> 48);
            data[i++] = (byte)(userIdentity >> 40);
            data[i++] = (byte)(userIdentity >> 32);
            data[i++] = (byte)(userIdentity >> 24);
            data[i++] = (byte)(userIdentity >> 16);
            data[i++] = (byte)(userIdentity >> 8);
            data[i++] = (byte)(userIdentity);
            return i;
        }

        public static byte[] makeTransferSignalingPacket(int packetType, String packetID, long userIdentity, long friendIdentity, long transferuserId)
        {
            byte[] packetIDByte = Encoding.UTF8.GetBytes(packetID);
            int totalDataLenght = 26 + packetIDByte.Length;
            byte[] data = new byte[totalDataLenght];
            int i = 0;
            data[i++] = (byte)packetType;

            addUserIDInArray(userIdentity, data, i);
            addUserIDInArray(friendIdentity, data, i);
            data[i++] = (byte)packetIDByte.Length;
            for (int n = 0; n < packetIDByte.Length; n++)
            {
                data[i++] = packetIDByte[n];
            }
            addUserIDInArray(transferuserId, data, i);
            return data;
        }

        public static byte[] makeTransferConfirmationOrConnectedSignalings(int packetType, String packetID, long userIdentity, long friendIdentityOrTransferID)
        {
            byte[] packetIDByte = Encoding.UTF8.GetBytes(packetID);
            int totalDataLenght = 18 + packetIDByte.Length;
            byte[] data = new byte[totalDataLenght];
            int i = 0;
            data[i++] = (byte)packetType;
            data[i++] = (byte)packetIDByte.Length;
            for (int n = 0; n < packetIDByte.Length; n++)
            {
                data[i++] = packetIDByte[n];
            }
            addUserIDInArray(userIdentity, data, i);
            addUserIDInArray(friendIdentityOrTransferID, data, i);
            return data;
        }

        public static byte[] makeTransferTransferBsyPacketWithMsg(int packetType, String packetID, long userIdentity, long friendIdentityOrTransferID, String msg)
        {
            //Packet Type+PacketID Length+Packet ID+User Identity Length+User Idenity+Friend Identity Length+Friend Idenity+Message Length+Message
            byte[] packetIDByte = Encoding.UTF8.GetBytes(packetID);
            byte[] msgByte = Encoding.UTF8.GetBytes(msg);
            //msg.getBytes();

            int totalDataLenght = 19 + packetIDByte.Length + msgByte.Length;
            byte[] data = new byte[totalDataLenght];
            int i = 0;
            data[i++] = (byte)packetType;
            data[i++] = (byte)packetIDByte.Length;
            for (int n = 0; n < packetIDByte.Length; n++)
            {
                data[i++] = packetIDByte[n];
            }

            addUserIDInArray(userIdentity, data, i);
            addUserIDInArray(friendIdentityOrTransferID, data, i);
            data[i++] = (byte)msgByte.Length;
            for (int n = 0; n < msgByte.Length; n++)
            {
                data[i++] = msgByte[n];
            }
            return data;
        }
        /*video*/
        public static byte[] CreateVideoMediaPacket(long frameNumber, int total_seq, int seq, byte[] givenImagedata)
        {
            //    byte[] packetIDByte = Encoding.UTF8.GetBytes(packetID);
            int totalDataLenght = 7 + givenImagedata.Length;
            byte[] data = new byte[totalDataLenght];
            int i = 0;
            data[i++] = (byte)VoiceConstants.VIDEO_CALL.VIDEO_MEDIA;

            //*frame number** 1 to 5 (b byte)/
            data[i++] = (byte)(frameNumber >> 24);
            data[i++] = (byte)(frameNumber >> 16);
            data[i++] = (byte)(frameNumber >> 8);
            data[i++] = (byte)(frameNumber);
            //***/
            data[i++] = (byte)total_seq;
            data[i++] = (byte)seq;
            for (int n = 0; n < givenImagedata.Length; n++)
            {
                data[i++] = givenImagedata[n];
            }
            return data;
        }

        //public static VideoPackets getVediomedioPacket(byte[] receivedBuffer)
        //{
        //    VideoPackets pak = new VideoPackets();

        //    // VoiceMessageDTO messageDTO = new VoiceMessageDTO();
        //    int totalRead = 0;
        //    int packetType = receivedBuffer[0];
        //    totalRead++;
        //    int framNumber = (int)getIntegerFromByte(totalRead, receivedBuffer);
        //    pak.FrameNumber = framNumber;
        //    totalRead += 4;
        //    int totalSeq = (int)receivedBuffer[totalRead++];
        //    pak.TotalSeq = totalSeq;
        //    int seqNumber = (int)receivedBuffer[totalRead++];
        //    pak.SequenceNumber = seqNumber;
        //    int length = receivedBuffer.Length - totalRead;
        //    byte[] vedioData = new byte[length];
        //    pak.ImageBytes = vedioData;
        //    Buffer.BlockCopy(receivedBuffer, 7, vedioData, 0, length);
        //    //  //  Console.WriteLine("vedioData==>" + vedioData.Length);
        //    return pak;
        //}
    }
}
