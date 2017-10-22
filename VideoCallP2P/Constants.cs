using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoCallP2P
{
    class Constants
    {
        public const int REGISTER_MESSAGE = 101;
        public const int UNREGISTER_MESSAGE = 102;
        public const int INVITE_MESSAGE = 103;
        public const int END_CALL_MESSAGE = 104;
        public const int CANCEL_MESSAGE = 105;
        public const int GET_ONLINE_USER_MESSAGE = 106;
        public const int PRESENCE_MESSAGE = 107;
        public const int PUBLISH_MESSAGE = 108;
        public const int VIEW_MESSAGE = 109;
        public const int PUBLISHER_INVITE_MESSAGE = 110;
        public const int VIEWER_INVITE_MESSAGE = 111;
        public const int ONLINE_PUBLISHER_MESSAGE = 112;
        public const int TERMINATE_ALL_MESSAGE = 199; // BE careful about this.
        public const int STOP_LIVE_CALL_MESSAGE = 113;


        public const int REPLY_REGISTER_MESSAGE = 121;
        public const int REPLY_UNREGISTER_MESSAGE = 122;
        public const int REPLY_INVITE_MESSAGE = 123;
        public const int REPLY_END_CALL_MESSAGE = 124;
        public const int REPLY_CANCEL_MESSAGE = 125;
        public const int REPLY_GET_ONLINE_USER_MESSAGE = 126;
        public const int REPLY_PRESENCE_MESSAGE = 127;
        public const int REPLY_PUBLISH_MESSAGE = 128;
        public const int REPLY_VIEW_MESSAGE = 129;
        public const int REPLY_PUBLISHER_INVITE_MESSAGE = 130;
        public const int REPLY_VIEWER_INVITE_MESSAGE = 131;
        public const int REPLY_ONLINE_PUBLISHER_MESSAGE = 132;
        public const int REPLY_STOP_LIVE_CALL_MESSAGE = 133;

        public const int USER_TYPE_IDLE = 0;
        public const int USER_TYPE_CALLER = 1;
        public const int USER_TYPE_CALLEE = 2;
        public const int USER_TYPE_PUBLISHER = 3;
        public const int USER_TYPE_VIEWER = 4;
        public const int USER_TYPE_PUBLISHER_CALLER = 5;
        public const int USER_TYPE_PUBLISHER_CALLEE = 6;
        public const int USER_TYPE_VIEWER_CALLER = 7;
        public const int USER_TYPE_VIEWER_CALLEE = 8;


        public const int ERRORR_MESSAGE = 12;

        public const int MAX_SERVER_PORT_NUMBER = 40000;
        public const int MIN_SERVER_PORT_NUMBER = 30001;

        public const int MAX_CLIENT_PORT_NUMBER = 30000;
        public const int MIN_CLIENT_PORT_NUMBER = 20001;

        public const int SIGNALING_SOCKET_TYPE = 1;
        public const int MEDIA_SOCKET_TYPE = 2;
    }
}
