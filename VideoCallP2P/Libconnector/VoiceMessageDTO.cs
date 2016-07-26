using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoCallP2P.Libconnector
{
    public class VoiceMessageDTO
    {

        public int PacketType
        {
            set;
            get;
        }
        public string PacketID
        {
            set;
            get;
        }
        public long UserIdentity
        {
            set;
            get;
        }
        public long FriendIdentity
        {
            set;
            get;
        }
        public long TransferID
        {
            set;
            get;
        }
        public int VoiceBindingPort
        {
            set;
            get;
        }
        public string VoiceBusyMessage
        {
            set;
            get;
        }
        //public int MapPort
        //{
        //    set;
        //    get;
        //}
        public int VideoCommunicationPort
        {
            set;
            get;
        }
        public int CallType
        {
            set;
            get;
        }
        public override string ToString()
        {
            return "VoiceMessageDTO{" + "packetType=" + PacketType + ", packetID=" + PacketID + ", userIdentity=" + UserIdentity + ", friendIdentity=" + FriendIdentity + ", transferID=" + TransferID + ", voiceBindingPort=" + VoiceBindingPort + ", voiceBusyMessage=" + VoiceBusyMessage + "," + ", VedioPort=" + VideoCommunicationPort + '}';
        }
    }
}
