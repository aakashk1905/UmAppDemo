using System.Collections.Generic;
using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    /**
     *  The parameter configuration class for pulling historical messages from the server.
     */
    [Preserve]
    public class FetchServerMessagesOption : BaseModel
    {
        /**
         *  Whether to save the obtained messages to the database:
         *  -`true`: Yes;
         *  -(Default) `false`：No.
         */
        public bool IsSave;

        /**
         *  The message search direction. See {@link MessageSearchDirection}.
         */
        public MessageSearchDirection Direction = MessageSearchDirection.UP;

        /**
         *  The user ID of the message sender.
         *
         *  This attribute is used only for group message.
         */
        public string From;

        /**
         *  The list of message types for query. The default is empty, indicating that all types of messages are retrieved.
         */
        public List<MessageBodyType> MsgTypes;

        /**
         *  The start time for message query. The time is a UNIX timestamp in milliseconds.
         *
         *  The default value is `-1`, indicating that this parameter is ignored during message query. 
         *
         *  If the start time is set to a specific time spot and the end time uses the default value `-1`, the SDK returns messages that are sent and received in the period that is from the start time to the current time.
         *
         *  If the start time uses the default value `-1` and the end time is set to a specific time spot, the SDK returns messages that are sent and received in the period that is from the timestamp of the first message to the current time.
         */
        public long StartTime = -1;

        /**
         *  The end time for message query. The time is a UNIX time stamp in milliseconds. 
         *
         *  The default value is `-1`, indicating that this parameter is ignored during message query. 
         *
         *  If the start time is set to a specific time spot and the end time uses the default value `-1`, the SDK returns messages that are sent and received in the period that is from the start time to the current time. 
         *
         *  If the start time uses the default value `-1` and the end time is set to a specific time spot, the SDK returns messages that are sent and received in the period that is from the timestamp of the first message to the current time.
         */
        public long EndTime = -1;

        [Preserve]
        public FetchServerMessagesOption() { }

        [Preserve]
        internal FetchServerMessagesOption(string jsonString) : base(jsonString) { }

        [Preserve]
        internal FetchServerMessagesOption(JSONObject jsonObject) : base(jsonObject) { }

        internal override void FromJsonObject(JSONObject jsonObject) { }

        internal List<int> GetListFromMsgTypes()
        {
            List<int> list = new List<int>();
            foreach(var it in MsgTypes)
            {
                list.Add(it.ToInt());
            }
            return list;
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("isSave", IsSave);
            jo.AddWithoutNull("direction", Direction.ToInt());
            jo.AddWithoutNull("from", From);
            jo.AddWithoutNull("types", JsonObject.JsonArrayFromIntList(GetListFromMsgTypes()));
            jo.AddWithoutNull("startTime", StartTime);
            jo.AddWithoutNull("endTime", EndTime);
            return jo;
        }
    };
}
