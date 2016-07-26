using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoCallP2P.Libconnector
{
    public interface OnReceiveListner
    {
        void OnVoiceSignalReceive(byte[] receivedByte, int Length);
    }
}
