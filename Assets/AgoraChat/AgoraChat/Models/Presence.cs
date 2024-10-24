using System.Collections.Generic;
using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    /**
     * The presence property class that contains presence properties, including the publisher's user ID and current presence state, and the platform used by the online device, as well as the presence's extension information, update time, and subscription expiration time.
     */
    [Preserve]
    public class Presence : BaseModel
    {

        /**
         * Gets the user ID of the presence publisher.
         *
         * @return The user ID of the presence publisher.
         */

        public string Publisher { get; internal set; }

        /**
         * Gets the details of the current presence state. The presence state details are a key-value structure, where the key can be "ios", "android", "linux", "win", or "webim", and the value is the current presence state of the publisher.
         *
         * @return The details of the current presence state.
         */

        public List<PresenceDeviceStatus> StatusList { get; internal set; }


        /**
         * Gets the presence status description information.
         *
         * @return The presence status description information.
         */

        public string StatusDescription { get; internal set; }

        /**
         * Gets the presence update time.
         *
         *  @return The Unix timestamp when the presence state is updated. The unit is second.
         */
        public long LatestTime { get; internal set; }

        /**
         * Gets the expiration time of the presence subscription.
         *
         * @return  The Unix timestamp when the presence subscription expires. The unit is second.
         */

        public long ExpiryTime { get; internal set; }

        [Preserve]
        internal Presence() { }

        [Preserve]
        internal Presence(string jsonString) : base(jsonString) { }

        [Preserve]
        internal Presence(JSONObject jsonObject) : base(jsonObject) { }

        internal override void FromJsonObject(JSONObject jo)
        {
            Publisher = jo["publisher"];
            StatusDescription = jo["desc"];
            LatestTime = (long)jo["lastTime"].AsDouble;
            ExpiryTime = (long)jo["expiryTime"].AsDouble;
            StatusList = List.BaseModelListFromJsonArray<PresenceDeviceStatus>(jo["detail"]);
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("publisher", Publisher);
            jo.AddWithoutNull("desc", StatusDescription);
            jo.AddWithoutNull("lastTime", LatestTime);
            jo.AddWithoutNull("expiryTime", ExpiryTime);
            jo.AddWithoutNull("detail", JsonObject.JsonArrayFromList(StatusList));
            return jo;
        }
    }
}
