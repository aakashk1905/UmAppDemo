using System.Collections.Generic;
using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    /**
     *  The Reaction operation class.
     */
    [Preserve]
    public class MessageReactionOperation : BaseModel
    {
        /**
         *  The user ID of the operator.
         */
        public string UserId;

        /**
         *  The changed Reaction.
         */
        public string Reaction;

        /**
         *  The Reaction operation.
         */
        public MessageReactionOperate operate;

        [Preserve]
        internal MessageReactionOperation() { }

        [Preserve]
        internal MessageReactionOperation(string jsonString) : base(jsonString) { }

        [Preserve]
        internal MessageReactionOperation(JSONObject jsonObject) : base(jsonObject) { }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            UserId = jsonObject["userId"];
            Reaction = jsonObject["reaction"];
            operate = jsonObject["operate"].AsInt.ToMessageReactionOperate();
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("userId", UserId);
            jo.AddWithoutNull("reaction", Reaction);
            jo.AddWithoutNull("operate", operate.ToInt());
            return jo;
        }
    };

    /**
    * The message Reaction change entity class.
    */
    [Preserve]
    public class MessageReactionChange : BaseModel
    {

        /**
         * The conversation ID to which the Reaction belongs.
         */
        public string ConversationId;
        /**
         * The ID of the parent message of the Reaction.
         */
        public string MessageId;

        /**
         * The Reaction list.
         */
        public List<MessageReaction> ReactionList;

        /**
         * The Reaction operation list.
         */
        public List<MessageReactionOperation> OperationList;

        [Preserve]
        internal MessageReactionChange() { }

        [Preserve]
        internal MessageReactionChange(string jsonString) : base(jsonString) { }

        [Preserve]
        internal MessageReactionChange(JSONObject jsonObject) : base(jsonObject) { }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            ConversationId = jsonObject["convId"];
            MessageId = jsonObject["msgId"];
            ReactionList = List.BaseModelListFromJsonArray<MessageReaction>(jsonObject["reactions"]);
            OperationList = List.BaseModelListFromJsonArray<MessageReactionOperation>(jsonObject["operations"]);
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("convId", ConversationId);
            jo.AddWithoutNull("msgId", MessageId);
            jo.AddWithoutNull("reactions", JsonObject.JsonArrayFromList(ReactionList));
            jo.AddWithoutNull("operations", JsonObject.JsonArrayFromList(OperationList));
            return jo;
        }
    }
}
