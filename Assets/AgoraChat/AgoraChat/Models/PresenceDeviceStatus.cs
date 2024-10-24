using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    /**
     * The presence device status class
     */
    [Preserve]
    public class PresenceDeviceStatus : BaseModel
    {
        /**
        * The presence device Id.
        */
        public string DeviceId;


        /**
         * The presence device status
         */
        public int Status;

        [Preserve]
        internal PresenceDeviceStatus() { }

        [Preserve]
        internal PresenceDeviceStatus(string jsonString) : base(jsonString) { }

        [Preserve]
        internal PresenceDeviceStatus(JSONObject jsonObject) : base(jsonObject) { }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            DeviceId = jsonObject["device"];
            Status = jsonObject["status"];
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("device", DeviceId);
            jo.AddWithoutNull("status", Status);
            return jo;
        }
    }
}
