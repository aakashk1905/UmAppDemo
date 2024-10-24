using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    [Preserve]
    public class ChatThread : BaseModel
    {
        /**
         * Gets the message thread ID.
         *
         * @return The message thread ID.
         */
        public string Tid;

        /**
         * Gets the ID of the parent message.
         *
         * @return The ID of the parent message.
         */
        public string MessageId;

        /**
         * Gets the group ID to which the message thread belongs.
         *
         * @return The group ID.
         */
        public string ParentId;

        /**
         * Gets the message thread creator.
         *
         * This message thread creator is returned when you get the message thread details and message thread list.
         *
         * @return The user ID of the message thread creator.
         */

        public string Owner;

        /**
         * Gets the message thread name.
         *
         * @return The message thread name.
         */
        public string Name;

        /**
         * Get the number of messages in a message thread.
         *
         * To get the number of messages in a message thread, you need to first call {@link IChatThreadManager#GetThreadDetail} to get details of the message thread.
         *
         * @return The message count.
         */
        public int MessageCount;

        /**
         * Gets the number of members in the message thread.
         *
         * To get the member count, you need to first call {@link IChatThreadManager#GetThreadDetail} to get details of the message thread.
         *
         * @return  The number of members in the message thread.
         */
        public int MembersCount;

        /**
        * Gets the Unix timestamp when the message thread is created. The unit is millisecond.
        *
        * @return The Unix timestamp when the message thread is created.
        */
        public long CreateAt;

        /**
         * Get the last reply in the message thread.
         *
         * To get the last reply in the message thread, you need to first call {@link IChatThreadManager#GetThreadDetail} to get details of the message thread.
         *
         * @return The last reply in the message thread.
         */
        public Message LastMessage;

        [Preserve]
        internal ChatThread() { }

        [Preserve]
        internal ChatThread(string jsonString) : base(jsonString) { }

        [Preserve]
        internal ChatThread(JSONObject jsonObject) : base(jsonObject) { }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            Tid = jsonObject["threadId"];
            Owner = jsonObject["owner"];
            Name = jsonObject["name"];
            MessageId = jsonObject["msgId"];
            ParentId = jsonObject["parentId"];
            MembersCount = jsonObject["memberCount"];
            MessageCount = jsonObject["msgCount"];
            CreateAt = (long)jsonObject["createAt"].AsDouble;

            // if lastMsg node is not exist, once use AsObeject lastMsg node will be added into json!!
            // So first check it first.
            if (null != jsonObject["lastMsg"])
            {
                LastMessage = ModelHelper.CreateWithJsonObject<Message>(jsonObject["lastMsg"].AsObject);
            }
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("threadId", Tid);
            jo.AddWithoutNull("owner", Owner);
            jo.AddWithoutNull("name", Name);
            jo.AddWithoutNull("msgId", MessageId);
            jo.AddWithoutNull("parentId", ParentId);
            jo.AddWithoutNull("memberCount", MembersCount);
            jo.AddWithoutNull("msgCount", MessageCount);
            jo.AddWithoutNull("createAt", CreateAt);
            jo.AddWithoutNull("lastMsg", LastMessage?.ToJsonObject());

            return jo;
        }
    }
}
