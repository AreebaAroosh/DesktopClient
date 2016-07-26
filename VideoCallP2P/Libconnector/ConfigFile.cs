using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoCallP2P.Libconnector
{
    public class ConfigFile
    {
        public static long USER_ID;
        //public static string VOICE_SERVER_IP;
        //public static int VOICE_REGISTER_PORT;
        //public static long USER_ID;
        //public static long FRIEND_ID;
        //public static string VOICE_SERVER_IP;
        //public static int VOICE_REGISTER_PORT;
        //public static int VOICE_BINDING_PORT;
        //public static int VOICE_FRIEND_DEVICE = 1;
        //public static string CALL_ID;
        //public static String BUSY_TEXT = null;
        public static string VIDEO_SERVER_IP = "192.168.1.38";
        //    public static int VIDEO_REGISTER_PORT;
        public static int VIDEO_BINDING_PORT = 1000;

        public static bool stopKeepAlive = true;
        public static bool sendKeepAliveWhileP2P = false;
        public static int numberOfRTPsending = 0;
        public static int numberOfRTPreceiving = 0;
        public static int CALL_WAITING_TIME = 30;//30SEC
        public static int KEEP_ALIVE_INTERVAL = 3000;
        public class MediaType
        {
            public const int IPV_UNKNOWN_MEDIA = 0;
            public const int IPV_MEDIA_AUDIO = 1;
            public const int IPV_MEDIA_VIDEO = 2;
            public const int IPV_MEDIA_CHAT = 3;
            public const int IPV_MEDIA_FILE_TRANSFER = 4;
        }
        public class EventType
        {
            public const int BEST_INTERFACE_DETECTED = 100;
            public const int FIREWALL_DETECTED = 101;
            public const int NETWORK_PROBLEM = 102;
            public const int INTERFACE_CHANGED = 103;
            public const int P2P_COMMUNICATION_ESTABLISHED = 104;
            public const int P2P_COMMUNICATION_FAILED = 105;
            public const int RELAY_COMMUNCATION_ESTABLISHED = 106;
            public const int DATA_REVEIVED = 107;
        }
        //public static void callID(String callid)
        //{
        //    CALL_ID = callid;
        //}
        public static void resetAllParameters()
        {
            // ConfigFile.USER_ID = 0;
            //VOICE_SERVER_IP = null;
            //VOICE_REGISTER_PORT = 0;
            //ConfigFile.USER_ID = 0;
            //FRIEND_ID = 0;
            //VOICE_SERVER_IP = null;
            //VOICE_REGISTER_PORT = 0;
            //VOICE_BINDING_PORT = 0;
            //VOICE_FRIEND_DEVICE = 1;
            //CALL_ID = null;
            //BUSY_TEXT = null;
            stopKeepAlive = true;
            sendKeepAliveWhileP2P = false;
            numberOfRTPsending = 0;
            numberOfRTPreceiving = 0;

        }
    }
    public enum SessionStatus
    {
        FAIL_TO_CREATE_SESSION,
        SESSION_CREATE_SUCCESSFULLY,
        ALREADY_SESSION_EXIST,
        NO_SESSION_AVAILABLE,
        INVALID_MEDIA,
        NONE
    };

}
