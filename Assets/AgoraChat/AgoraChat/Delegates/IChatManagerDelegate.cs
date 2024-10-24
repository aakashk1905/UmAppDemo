using System.Collections.Generic;

namespace AgoraChat
{
	/**
	     * The chat manager callback interface.
	     * 
	     */
	public interface IChatManagerDelegate
	{
		/**
	     * Occurs when a messages is received.
		 * 
	     * This callback is triggered to notify the user when a message such as a text, image, video, voice, geographical location, or file is received.
	     * 
	     * @param messages  The received message(s).
	     */
		void OnMessagesReceived(List<Message> messages);
		/**
	     * Occurs when a command message is received.
		 *
	     * Unlike {@link #onMessageReceived(List)}, this callback is triggered only by the reception of a command message that is usually invisible to users.
	     * 
	     * @param messages  The received command message(s).
	     *
	     */
		void OnCmdMessagesReceived(List<Message> messages);

		/**
         * Occurs when a read receipt is received for a message. 
         * 
         * @param messages  The read message(s).
         */
		void OnMessagesRead(List<Message> messages);

		/**
         * Occurs when a delivery receipt is received.
         * 
         * @param messages  The delivered message(s).
         */
		void OnMessagesDelivered(List<Message> messages);

		/**
 	     * Occurs when a received message is recalled.
 	     * 
 	     * @param messages  The recalled message(s).
 	     */
		void OnMessagesRecalled(List<Message> messages);

		/**
	     * Occurs when the read status updates of a group message is received.
	     */
		void OnReadAckForGroupMessageUpdated();

		/**
	     * Occurs when a read receipt is received for a group message.
	     * 
	     * @param list The read receipt(s) for group message(s).
	     * 
	     */
		void OnGroupMessageRead(List<GroupReadAck> list);

		/**
        * Occurs when the number of conversations changes.
        * 
        */
		void OnConversationsUpdate();

		/**
	     * Occurs when the read receipt is received for a conversation.
	     *
	     * This callback occurs in either of the following scenarios:
		 *
	     * - The message is read by the recipient (The read receipt for the conversation is sent).
		 *
	     *   Upon receiving this event, the SDK sets the `isAcked` attribute of the messages in the conversation to `true` in the local database.
	     * 
		 * - In the multi-device login scenario, when one device sends a read receipt for a conversation, the server will set the number of unread messages of this conversation to `0`.
		 *  In this case, the callback occurs on the other devices where the SDK will set `isRead` attribute of the messages in the conversation to `true` in the local database.
	     * 
		 * @param from The ID of the user who sends the read receipt.
	     * @param to   The ID of the user who receives the read receipt.
	     */
		void OnConversationRead(string from, string to);

		/**
         * Occurs when the reactions changed.
         *
         * @param list The changed reaction list.
         */
		void MessageReactionDidChange(List<MessageReactionChange> list);

        /**
         * Occurs when a sent message is modified.
         *
         * @param Message       The modified message object, where the message body contains the information such as the number of message modifications, the operator of the last modification, and the last modification time.
	     * 	Also, you can get the operator of the last message modification and the last modification time via the `onMessageContentChanged` method.
         * @param operatorId    The user ID of the operator that modified the message last time.
         * @param operationTime The last message modification time. It is a UNIX timestamp in milliseconds.
         */
        void OnMessageContentChanged(Message msg, string operatorId, long operationTime);
    }
}
