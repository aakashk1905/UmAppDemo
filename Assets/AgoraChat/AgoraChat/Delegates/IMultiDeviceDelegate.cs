using System.Collections.Generic;

namespace AgoraChat
{
    /**
        * The multi-device callback interface.
        * 
        */
    public interface IMultiDeviceDelegate
    {
        /**
         * The callback for a multi-device contact event.
         *
         * @param operation The contact event. See {@link MultiDevicesOperation}.
         * @param target    The user ID of the contact.
         * @param ext       The extension information.
         */
        void OnContactMultiDevicesEvent(MultiDevicesOperation operation, string target, string ext);

        /**
         * The callback for a multi-device group event.
         *
         * @param operation     The group event. See {@link MultiDevicesOperation}.
         * @param target        The group ID.
         * @param usernames     The target user ID(s) of the operation.
         */
        void OnGroupMultiDevicesEvent(MultiDevicesOperation operation, string target, List<string> usernames);

        /**
         * The callback for a multi-device thread event.
         *
         * @param operation     The thread event. See {@link MultiDevicesOperation}.
         * @param target        The thread ID.
         * @param usernames     The target user ID(s) of the operation.
         */
        void OnThreadMultiDevicesEvent(MultiDevicesOperation operation, string target, List<string> usernames);

        /**
         * The callback for a multi-device event for deletion of historical messages in a conversation from the server.
         *
         * @param conversationId    The conversation ID.
         * @param deviceId          The device ID.
         */
        void OnRoamDeleteMultiDevicesEvent(string conversationId, string deviceId);

        /**
        * The callback for a multi-device conversation event.
        *
        * @param operation         The conversation event. See {@link MultiDevicesOperation}.
        * @param conversationId    The conversation ID.
        * @param type              The conversation type.
        */
        void OnConversationMultiDevicesEvent(MultiDevicesOperation operation, string conversationId, ConversationType type);
    }

}
