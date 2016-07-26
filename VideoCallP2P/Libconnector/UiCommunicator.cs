using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoCallP2P.Libconnector
{
    public interface UiCommunicator
    {
        void process_RTP(byte[] receivedBuffer, int Length);

        void process_VOICE_REGISTER_CONFIRMATION(VoiceMessageDTO voiceMessageDTO);

        void process_CALLING(VoiceMessageDTO voiceMessageDTO);

        void process_RINGING(VoiceMessageDTO voiceMessageDTO);

        void process_IN_CALL(VoiceMessageDTO voiceMessageDTO);

        void process_IN_CALL_CONFIRMATION(VoiceMessageDTO voiceMessageDTO);

        void process_ANSWER(VoiceMessageDTO voiceMessageDTO);

        void process_CONNECTED(VoiceMessageDTO voiceMessageDTO);

        void process_BUSY(VoiceMessageDTO voiceMessageDTO);

        void process_CANCELED(VoiceMessageDTO voiceMessageDTO);

        void process_BYE(VoiceMessageDTO voiceMessageDTO);

        void process_DISCONNECTED(VoiceMessageDTO voiceMessageDTO);

        void process_NO_ANSWER(VoiceMessageDTO voiceMessageDTO);

        void process_VOICE_REGISTER_PUSH(VoiceMessageDTO voiceMessageDTO);

        void process_VOICE_REGISTER_PUSH_CONFIRMATION(VoiceMessageDTO voiceMessageDTO);

        void process_VOICE_CALL_HOLD(VoiceMessageDTO voiceMessageDTO);

        void process_VOICE_CALL_HOLD_CONFIRMATION(VoiceMessageDTO voiceMessageDTO);

        void process_VOICE_CALL_UNHOLD(VoiceMessageDTO voiceMessageDTO);

        void process_VOICE_UNHOLD_CONFIRMATION(VoiceMessageDTO voiceMessageDTO);

        void process_VOICE_BUSY_MESSAGE(VoiceMessageDTO voiceMessageDTO);

        void process_VOICE_BUSY_MESSAGE_CONFIRMATION(VoiceMessageDTO voiceMessageDTO);

        void process_TRANSFER_HOLD(VoiceMessageDTO voiceMessageDTO);

        void process_TRANSFER_HOLD_CONFIRMATION(VoiceMessageDTO voiceMessageDTO);

        void process_TRANSFER_BUSY(VoiceMessageDTO voiceMessageDTO);

        void process_TRANSFER_BUSY_CONFIRMATION(VoiceMessageDTO voiceMessageDTO);

        void process_TRANSFER_SUCESS(VoiceMessageDTO voiceMessageDTO);

        void process_TRANSFER_SUCESS_CONFIRMATION(VoiceMessageDTO voiceMessageDTO);

        void process_TRANSFER_CONNECTED(VoiceMessageDTO voiceMessageDTO);

        void process_TRANSFER_CONNECTED_CONFIRMATION(VoiceMessageDTO voiceMessageDTO);

        void process_TRANSFER_UNHOLD(VoiceMessageDTO voiceMessageDTO);

        void process_TRANSFER_UNHOLD_CONFIRMATION(VoiceMessageDTO voiceMessageDTO);
        /*p2p*/
        void process_notifyClientMethodForFriend(int eventType, long friendName, int mediaName);
        /**/
    }
}
