using System.Collections.Generic;
using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    /**
    * The message Reaction instance class, which has the following attributes:
    *
    *   Reaction: The message Reaction.
    *   UserCount: The count of users that added the Reaction.
    *   UserList: The list of users that added the Reaction.
    *   State: Whether the current user added this Reaction.
    */
    [Preserve]
    public class MessageReaction : BaseModel
    {
        /**
         * Gets the Reaction.
         */
        public string Reaction;

        /**
         * Gets the count of users that added this Reaction.
         */
        public int Count;

        /**
         * Gets the list of users that added this Reaction.
         *
         * **Note**
         * {@link IChatManager#GetReactionDetail} can return the entire list of users that added this Reaction with pagination, whereas other methods such as {@link IChatManager#GetReactionList} can only return the first three users.
         * @return  The list of users that added this Reaction.
         */
        public List<string> UserList;

        /**
         * Gets whether the current user has added the Reaction.
         *
         *  - `true`: Yes.
         *  - `false`: No.
         */
        public bool State;

        [Preserve]
        internal MessageReaction() { }

        [Preserve]
        internal MessageReaction(string jsonString) : base(jsonString) { }

        [Preserve]
        internal MessageReaction(JSONObject jsonObject) : base(jsonObject) { }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            Reaction = jsonObject["reaction"];
            Count = jsonObject["count"].AsInt;
            UserList = List.StringListFromJsonArray(jsonObject["userList"]);
            State = jsonObject["isAddedBySelf"].AsBool;
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("reaction", Reaction);
            jo.AddWithoutNull("count", Count);
            jo.AddWithoutNull("userList", JsonObject.JsonArrayFromStringList(UserList));
            jo.AddWithoutNull("isAddedBySelf", State);
            return jo;
        }
    }
}
