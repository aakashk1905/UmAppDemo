using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    /**
     * The multi-device information class.
     */
    [Preserve]
    public class DeviceInfo : BaseModel

    {
        /**
         * The information of other login devices.
         * 
         * With the device information, you can distinguish different types of devices.
         */
        public string Resource { get; private set; }

        /**
         * The UUID of the device.
         * 
         */
        public string DeviceUUID { get; private set; }

        /**
         * The device name.
         */
        public string DeviceName { get; private set; }

        [Preserve]
        internal DeviceInfo() { }

        [Preserve]
        internal DeviceInfo(string jsonString) : base(jsonString) { }

        [Preserve]
        internal DeviceInfo(JSONObject jsonObject) : base(jsonObject) { }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            Resource = jsonObject["resource"];
            DeviceUUID = jsonObject["deviceUUID"];
            DeviceName = jsonObject["deviceName"];
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("resource", Resource);
            jo.AddWithoutNull("deviceUUID", DeviceUUID);
            jo.AddWithoutNull("deviceName", DeviceName);
            return jo;
        }
    }
}
