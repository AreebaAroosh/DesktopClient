using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoCallP2P.Libconnector
{
    public class VoiceConstants
    {
        public static readonly int CALL_TYPE_VOICE = 1;
        public static readonly int CALL_TYPE_VIDEO = 2;
        public const int REG_PACKET_SIZE = 512;
        public const int VOICE_MEDIA = 0;
        public const int VOICE_REGISTER = 1;
        public const int VOICE_UNREGISTERED = 2;
        public const int VOICE_REGISTER_CONFIRMATION = 3;
        public const int NUMBER_OF_RESEND = 5;
        public const int REGISTER_RESEND_TIME = 30000;
        public static int NUMBER_OF_MAX_GARBAGE = 5;
        public static int PLATFORM_DESKTOP = 1;
        public static int PLATFORM_ANDROID = 2;
        public static int PLATFORM_IPHONE = 3;
        public static int PLATFORM_WINDOWS = 4;
        public static int PLATFORM_WEB = 5;
        public static int PROCESSING_INTERVAL = 500;

        public class CALL_STATE
        {
            public const byte KEEPALIVE = 4;
            public const byte CALLING = 5;
            public const byte RINGING = 6;
            public const byte IN_CALL = 7;
            public const byte ANSWER = 8;
            public const byte BUSY = 9;
            public const byte CANCELED = 10;
            public const byte CONNECTED = 11;
            public const byte DISCONNECTED = 12;
            public const byte BYE = 13;
            public const byte IDEL = 14;
            public const byte NO_ANSWER = 15;
            public const byte USER_AVAILABLE = 16;
            public const byte USER_NOT_AVAILABLE = 17;
            public const byte IN_CALL_CONFIRMATION = 18;
            public const byte VIDEO_MEDIA = 19;
            public const int VOICE_REGISTER_PUSH = 20;
            public const int VOICE_REGISTER_PUSH_CONFIRMATION = 21;
            public const int VOICE_CALL_HOLD = 22;
            public const int VOICE_CALL_HOLD_CONFIRMATION = 23;
            public const int VOICE_CALL_UNHOLD = 24;
            public const int VOICE_UNHOLD_CONFIRMATION = 25;
            public const int VOICE_BUSY_MESSAGE = 26;
            public const int VOICE_BUSY_MESSAGE_CONFIRMATION = 27;

            //Video Constants
        };
        public class VIDEO_CALL
        {

            public const int VIDEO_MEDIA = 39;
            public const int BINDING_PORT = 40;
            public const int BINDING_PORT_CONFIRMATION = 41;
            public const int START = 42;
            public const int START_CONFIRMATION = 43;
            public const int KEEPALIVE = 44;
            public const int END = 45;
            public const int END_CONFIRMATION = 46;
        }
        public class TRANSFER_CALL
        {
            public const byte HOLD = 28;
            public const byte HOLD_CONFIRMATION = 29;
            public const byte BUSY = 30;
            public const byte BUSY_CONFIRMATION = 31;
            public const byte SUCCESS = 32;
            public const byte SUCCESS_CONFIRMATION = 33;
            public const byte CONNECTED = 34;
            public const byte CONNECTED_CONFIRMATION = 35;
            public const byte UNHOLD = 36;
            public const byte UNHOLD_CONFIRMATION = 37;
        }
    }
}
