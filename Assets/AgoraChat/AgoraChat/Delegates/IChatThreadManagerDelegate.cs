namespace AgoraChat
{
    public interface IChatThreadManagerDelegate
    {
        /**
        * Occurs when a message thread is created.
        *
        * Each member of the group to which the message thread belongs can receive the callback.
        */
        void OnChatThreadCreate(ChatThreadEvent threadEvent);

        /**
        * Occurs when a message thread is updated.
		*
	    * This callback is triggered when the message thread name is changed or a threaded reply is added or recalled.
        *
        * Each member of the group to which the message thread belongs can receive the callback.
        */
        void OnChatThreadUpdate(ChatThreadEvent threadEvent);

        /**
        * Occurs when a message thread is destoryed.
        *
        * Each member of the group to which the message thread belongs can receive the callback.
        */
        void OnChatThreadDestroy(ChatThreadEvent threadEvent);

        /**
	    * Occurs when the current user is removed from the message thread by the group owner or a group admin to which the message thread belongs.
        *
        * Each member of the group to which the message thread belongs can receive the callback.
        */
        void OnUserKickOutOfChatThread(ChatThreadEvent threadEvent);
    }
}
