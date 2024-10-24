using AgoraChat.SimpleJSON;
using System.Collections.Generic;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    [Preserve]
    public class Conversation : BaseModel
    {
        private ConversationManager manager { get => SDKClient.Instance.ConversationManager; }

        /**
         * The conversation ID.
         * 
        */
        public string Id;

        /**
         * The conversation type.
         */
        public ConversationType Type;


        /**
         * Whether a conversation is a thread conversation:
         * - `true`: Yes.
         * - `false`: No.
         */
        public bool IsThread;

        /**
         * Whether a conversation is pinned:
         * - `true`: Yes.
         * - `false`: No.
         *
         */
        public bool IsPinned;

        /**
         * The timestamp when the conversation is pinned. The unit is millisecond.
         *
         * If `isPinned` is `false`, `0` is returned.
         */
        public long PinnedTime;

        /**
        * Gets the latest message in the conversation.
        * 
        * A call of this method has no impact on the unread message count of the conversation.
        * 
        * The SDK first retrieves the latest message from the memory. If no message is found, the SDK retrieves it from the local database and loads it.
        *
        * @return  The message instance.
        */
        public Message LastMessage
        {
            get => manager.LastMessage(Id, Type);
        }

        /**
         * Gets the latest message received in the conversation.
         * 
         * @return  The message instance.
         */
        public Message LastReceivedMessage
        {
            get => manager.LastReceivedMessage(Id, Type);
        }

        /**
         * Gets the extension information of the conversation.
         * 
         * @return  The extension information of the conversation.
         */
        public Dictionary<string, string> Ext
        {
            get => _Ext;
            set
            {
                if (manager.SetExt(Id, Type, value))
                {
                    _Ext = Ext;
                }
            }
        }

        /**
         * Gets the count of unread messages in the conversation.
         * 
         * @return The count of unread messages.
         */
        public int UnReadCount
        {
            get => manager.UnReadCount(Id, Type);
        }

        /**
         * Marks a message as read.
         * 
         * @param messageId The message ID.
         */
        public void MarkMessageAsRead(string messageId)
        {
            manager.MarkMessageAsRead(Id, Type, messageId);
        }

        /**
         * Marks all unread messages in the conversation as read.
         */
        public void MarkAllMessageAsRead()
        {
            manager.MarkAllMessageAsRead(Id, Type);
        }

        /**
         * Inserts a message into a conversation in the local database.
         * 
         * To insert the message correctly, ensure that the conversation ID of the message is the same as that of the conversation. 
         * 
         * The message will be inserted based on the Unix timestamp included in it. Upon message insertion, the SDK will automatically update attributes of the conversation, including `latestMessage`.
         *
         * @param message  The message instance.
         * 
         * @return Whether the message is successfully inserted.
         *           - `true`: Yes.
         *           - `false`: No.
         * 
         */
        public bool InsertMessage(Message message)
        {
            return manager.InsertMessage(Id, Type, message);
        }

        /**
         * Inserts a message to the end of a conversation in the local database.
         * 
         * The conversation ID of the message should be the same as that of the conversation to make sure that the message is correctly inserted.
         * 
         * After a message is inserted, the SDK will automatically update attributes of the conversation, including `latestMessage`.
         *
         * @param message The message instance.
         * 
         * @return Whether the message is successfully inserted.
         *         - `true`: Yes.
         *         - `false`: No.
         *
         */
        public bool AppendMessage(Message message)
        {
            return manager.AppendMessage(Id, Type, message);
        }

        /**
         * Updates a message in the local database.
         * 
         * The ID of the message remains unchanged during message updates.
         * 
         * After a message is updated, the SDK will automatically update attributes of the conversation, including `latestMessage`.
         *
         * @param message  The message to be updated.
         * 
         * @return Whether this message is successfully updated.
         *         - `true`: Yes.
         *         - `false`: No.
         */
        public bool UpdateMessage(Message message)
        {
            return manager.UpdateMessage(Id, Type, message);
        }

        /**
         * Deletes a message from the local database.
         *
         * @param messageId    The ID of the message to be deleted.
         * 
         * @return Whether the message is successfully deleted.
         *           - `true`: Yes.
         *           - `false`: No.
         */
        public bool DeleteMessage(string messageId)
        {
            return manager.DeleteMessage(Id, Type, messageId);
        }

        /**
         * Deletes messages sent or received in a certain period from the local database.
         *
         * @param startTime    The starting Unix timestamp for message deletion. The unit is millisecond.
         * @param endTime      The ending Unix timestamp for message deletion. The unit is millisecond.
         *
         * @return Whether the message is successfully deleted.
         *           - `true`: Yes.
         *           - `false`: No.
         */
        public bool DeleteMessages(long startTime, long endTime)
        {
            return manager.DeleteMessages(Id, Type, startTime, endTime);
        }

        /**
         * Deletes all the messages in the conversation.
         * 
         * This method deletes all messages in the conversation from both the memory and the local database.
         * 
         * @return Whether messages are successfully deleted.
         *         - `true`: Yes.
         *         - `false`: No.
         */
        public bool DeleteAllMessages()
        {
            return manager.DeleteAllMessages(Id, Type);
        }

        /**
         * Loads a message.
         * The SDK first retrieves the message from the memory. If no message is found, the SDK retrieves it from the local database and loads it.
         *
         * @param messageId         The ID of the message to load.
         * @return                  The message instance. If the message is not found in both the local memory and local database, the SDK returns `null`.
         */
        public Message LoadMessage(string messageId)
        {
            return manager.LoadMessage(Id, Type, messageId);
        }

        /**
         * Loads the messages of a specific type.
         * 
         * The SDK first retrieves the messages from the memory. If no message is found, the SDK will retrieve them from the local database and load them.
         *
         * @param type              The type of messages to load. Ensure that you set this parameter.
         * @param sender            The user ID of the message sender. Ensure that you set this parameter.
         * @param timestamp         The starting Unix timestamp for query. The default value is `-1`, indicating the current Unix timestamp.
         * @param count             The maximum number of messages to load. The default value is `20`.
         * @param direction         The message loading direction. By default, the SDK loads messages in the reverse chronological order of the Unix timestamp ({@link SortMessageByServerTime}) in the message. See {@link MessageSearchDirection}.
         * @param callback          The loading result callback. If success, a list of loaded messages are returned; otherwise, an error is returned. See {@link ValueCallBack}.
         */
        public void LoadMessagesWithMsgType(MessageBodyType type, string sender = null, long timestamp = -1, int count = 20, MessageSearchDirection direction = MessageSearchDirection.UP, ValueCallBack<List<Message>> callback = null)
        {
            manager.LoadMessagesWithMsgType(Id, Type, type, sender, timestamp, count, direction, callback);
        }

        /**
         * Loads the messages, starting from a specific message ID.
         *
         * The SDK first retrieves the messages from the memory. If no message is found, the SDK will retrieve them from the local database and load them.
         *
         * @param startMessageId    The starting message ID for loading. If this parameter is set as "" or `null`, the SDK will load from the latest message.
         * @param count             The maximum number of messages to load. The default value is `20`.
           @param direction         The message loading direction. By default, the SDK loads messages in the reverse chronological order of the Unix timestamp ({@link SortMessageByServerTime}) in the messages. See {@link MessageSearchDirection}.
         * @param callback            The loading result callback. If success, a list of loaded messages are returned; otherwise, an error is returned. See {@link ValueCallBack}.
         */
        public void LoadMessages(string startMessageId = null, int count = 20, MessageSearchDirection direction = MessageSearchDirection.UP, ValueCallBack<List<Message>> callback = null)
        {
            manager.LoadMessages(Id, Type, startMessageId ?? "", count, direction, callback);
        }

        /**
         * Loads the messages by keywords.
         * 
         * The SDK first retrieves the messages from the memory. If no message is found, the SDK will retrieve them from the local database and load them.
         *
         * @param keywords          The keywords for query.
         * @param sender            The user ID of the message sender.
         * @param timestamp         The starting Unix timestamp for query. The default value is `-1`, indicating the current Unix timestamp.
         * @param count             The maximum number of messages to load. The default value is `20`.
         * @param direction         The message loading direction. By default, the SDK loads messages in the reverse chronological order of the Unix timestamp ({@link SortMessageByServerTime}) in the messages. See {@link MessageSearchDirection}.
         * @param callback          The loading result callback. If success, a list of loaded messages are returned; otherwise, an error is returned. See {@link ValueCallBack}.
         */
        public void LoadMessagesWithKeyword(string keywords, string sender = null, long timestamp = -1, int count = 20, MessageSearchDirection direction = MessageSearchDirection.UP, ValueCallBack<List<Message>> callback = null)
        {
            manager.LoadMessagesWithKeyword(Id, Type, keywords, sender, timestamp, count, direction, callback);
        }

        /**
         * Loads the messages within a period.
         * 
         * **Note**
         * 
         * The SDK first retrieves the messages from the memory. If no message is found, the SDK will retrieve them from the local database and load them.
         * 
         * Pay attention to the memory usage when you load a great number of messages.
         *
         * @param startTimeStamp    The starting Unix timestamp for query.
         * @param endTimeStamp      The ending Unix timestamp for query.
         * @param count             The maximum number of messages to load. The default value is `20`.
         * @param callback            The loading result callback. If success, a list of loaded messages are returned; otherwise, an error is returned. See {@link ValueCallBack}.
         */
        public void LoadMessagesWithTime(long startTime, long endTime, int count = 20, ValueCallBack<List<Message>> callback = null)
        {
            manager.LoadMessagesWithTime(Id, Type, startTime, endTime, count, callback);
        }

        /**
         * Gets the count of all messages in this conversation in the local database.
         * @return The count of all the messages in this conversation.
         */
        public int MessagesCount()
        {
            return manager.MessagesCount(Id, Type);
        }

        [Preserve]
        internal Conversation() { }

        [Preserve]
        internal Conversation(string json) : base(json) { }

        [Preserve]
        internal Conversation(JSONObject jo) : base(jo) { }

        private Dictionary<string, string> _Ext;

        internal override void FromJsonObject(JSONObject jo)
        {
            if (!jo.IsNull)
            {
                Id = jo["convId"];
                Type = jo["type"].AsInt.ToConversationType();
                IsThread = jo["isThread"];
                IsPinned = jo["isPinned"];
                PinnedTime = (long)jo["pinnedTime"].AsDouble;
                _Ext = Dictionary.StringDictionaryFromJsonObject(jo["ext"]);
            }
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("convId", Id);
            jo.AddWithoutNull("type", Type.ToInt());
            jo.AddWithoutNull("isThread", IsThread);
            return jo;
        }

    }
}


