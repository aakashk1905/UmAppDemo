using System;
using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine;
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    /**
     * The chat setting class that defines parameters and options of the SDK, including whether to automatically accept friend invitations and whether to automatically download the thumbnail.
     */
    [Preserve]
    public class Options : BaseModel
    {
        /**
	     * The App Key you get from the console when creating a chat app. It is the unique identifier of your app.
	     */
        public string AppKey = "";

        /**
	     * The URL of the DNS server.
	     */
        public string DNSURL = "";

        /**
	     * The address of the IM server.
         *
         * This address is used when you implement data isolation and data security during private deployment.
         *
         * If you need the address, contact our business manager.
         *
	     */
        public string IMServer = "";

        /**
	     * The address of the REST server.
         *
         * This address is used when you implement data isolation and data security during private deployment.
         *
         * If you need the address, contact our business manager.
	     */
        public string RestServer = "";

        /**
	     * The custom port of the IM server.
         *
         * This port is used when you implement data isolation and data security during private deployment.
         *
         * If you need the port, contact our business manager.
	     */
        public int IMPort = 0;

        /**
	     * Whether to enable DNS.
	     * - (Default) `true`: Yes.
	     * - `false`: No. DNS should be disabled for private deployment.
	     */
        public bool EnableDNSConfig = true;

        /**
         * Whether to output the debug information as logs.
         * - `true`: Yes.
         * - (Default) `false`: No.
         */
        public bool DebugMode = false;

        /**
	     * Whether to enable automatic login.
	     * -  `true`: Yes.
	     * -  (Default)`false`: No.
	     */
        public bool AutoLogin = false;

        /**
	     * Whether to automatically accept friend invitations from other users.
         * - `true`: Yes.
	     * - (Default) `false`: No.
	     */
        public bool AcceptInvitationAlways = false;

        /**
	     * Whether to accept group invitations automatically.
         * - (Default) `true`: Yes.
	     * - `false`: No.
	     */
        public bool AutoAcceptGroupInvitation = false;

        /**
	     * Whether to require the read receipt.
	     * - (Default) `true`: Yes;
	     * - `false`: No.
	     */
        public bool RequireAck = true;

        /**
	     * Whether to require the delivery receipt.
	     * - (Default) `true`: Yes;
	     * - `false`: No.
	     */
        public bool RequireDeliveryAck = false;

        /**
	     * Whether to delete the historical group messages in the memory and local database when leaving the group (either voluntarily or passively).
	     * - (Default) `true`: Yes;
	     * - `false`: No.
         */
        public bool DeleteMessagesAsExitGroup = true;

        /**
	     * Whether to delete the historical messages of the chat room in the memory and local database when leaving the chat room (either voluntarily or passively).
	     * - (Default) `true`: Yes.
	     * - `false`: No.
         */
        public bool DeleteMessagesAsExitRoom = true;

        /**
	     * Whether to allow the chat room owner to leave the chat room.
	     * - (Default) `true`: Yes. When leaving the chat room, the chat room owner still has all privileges, except for receive messages in the chat room.
	     * - `false`: No.
         */
        public bool IsRoomOwnerLeaveAllowed = true;

        /**
	     * Whether to sort the messages in the reverse chronological order of the time when they are received by the server.
	     * - (Default) `true`: Yes;
	     * - `false`: No. Messages are sorted in the reverse chronological order of the time when they are created.
	     */
        public bool SortMessageByServerTime = true;

        /**
	     * Whether only HTTPS is used for REST operations.
	     * - (Default) `true`: Only HTTPS is supported.
	     * - `false`: Both HTTP and HTTPS are allowed.
	     */
        public bool UsingHttpsOnly = true;

        /**
	     * Whether to upload the message attachments automatically to the chat server.
	     * - (Default) `true`: Yes;
	     * - `false`: No.
	     */
        public bool ServerTransfer = true;

        /**
	     * Whether to automatically download the thumbnail.
	     * - (Default) `true`: Yes;
	     * - `false`: No.
	     *
	     */
        public bool IsAutoDownload = true;

        /**
	     * Whether to include empty conversations while loading conversations from the database.
	     * -`true`: Yes;
	     * -(Default) `false`: No.
	     *
	     */
        public bool EnableEmptyConversation = false;

        /**
         *  The custom system type.
         */
        public int CustomOSType = -1;

        /**
         *  The custom device name.
         *
         */
        public string CustomDeviceName = "";

        /**
	     * The area code.
         *
         * Restrictions of the area should be followed when edge nodes are used.
         *
	     * (Default) `GLOB`: No restrictions will be applied when you try to access the server node.
	     */
        public AreaCode AreaCode = AreaCode.GLOB;

        /**
        * The UUID for the current device.
        */
        public string MyUUID = "";

        /**
        * The underlying storage path for SDK data. The storage path is used only for MacOS and Windows platforms.
        *
        * If this parameter is not set, the SDK will set the default value.
        *
        * Note:
        * For the Unity SDK, prior to v1.1.2, SDKDataPath was set to the current path ".".
        * Starting from v1.1.2, this path has been changed to the persistent directory Application.persistentDataPath.
        * If you are upgrading from an SDK version earlier than v1.1.2 and need to retain the local historical messages, there are two methods:
        * - Method 1: Set SDKDataPath to the current path ".".
        * - Method 2: Copy the sdkdata folder in the original current path to the persistent directory Application.persistentDataPath.
        * For the Windows SDK, SDKDataPath still uses the current path ".".
        *
        * For example:
        * MacOS: /Users/UserName/Library/Application Support/DefaultCompany/xxx
        * Windows: C:/Users/UserName/AppData/LocalLow/DefaultCompany/xxx
        *
        * If the data storage path ends with a folder name, it is unnecessary to appended "/" to the end of the path.
        *
        * Note: For MacOS, if you set `SDKDatapath` to a relative path, the path must start with ".", for example "./sdkdatapath".
        */
        public string SDKDataPath = "";

        /**
        * The options constructor.
        *
        * @param appKey  The App Key.
        */
        [Preserve]
        public Options(string appKey)
        {
            AppKey = appKey;
        }

        /*
        // MeiZu
        private string mZAppId = "", mZAppKey = "";
        private bool enableMZPush = false;

        // OPPO
        private string oPPOAppKey = "", oPPOAppSecret = "";
        private bool enableOPPOPush = false;

        // XiaoMi
        private string miAppId = "", miAppKey = "";
        private bool enableMiPush = false;

        private bool enableVivoPush = false;

        // Google
        private string fCMId = "";
        private bool enableFCMPush = false;

        // Apple
        private string aPNsCerName = "";
        private bool enableAPNs = false;

        // HuaWei
        private bool enableHWPush = false;
        */
        [Preserve]
        internal Options() { }

        [Preserve]
        internal Options(bool is_json, string json) : base(json) { }

        [Preserve]
        internal Options(JSONObject jo) : base(jo) { }

        internal override void FromJsonObject(JSONObject jo) { }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("appKey", AppKey);
            jo.AddWithoutNull("debugMode", DebugMode);
            jo.AddWithoutNull("autoLogin", AutoLogin);
            jo.AddWithoutNull("acceptInvitationAlways", AcceptInvitationAlways);
            jo.AddWithoutNull("autoAcceptGroupInvitation", AutoAcceptGroupInvitation);
            jo.AddWithoutNull("requireAck", RequireAck);
            jo.AddWithoutNull("requireDeliveryAck", RequireDeliveryAck);
            jo.AddWithoutNull("deleteMessagesAsExitGroup", DeleteMessagesAsExitGroup);
            jo.AddWithoutNull("deleteMessagesAsExitRoom", DeleteMessagesAsExitRoom);
            jo.AddWithoutNull("isRoomOwnerLeaveAllowed", IsRoomOwnerLeaveAllowed);
            jo.AddWithoutNull("sortMessageByServerTime", SortMessageByServerTime);
            jo.AddWithoutNull("usingHttpsOnly", UsingHttpsOnly);
            jo.AddWithoutNull("serverTransfer", ServerTransfer);
            jo.AddWithoutNull("isAutoDownload", IsAutoDownload);
            jo.AddWithoutNull("areaCode", (int)AreaCode);
            jo.AddWithoutNull("enableDnsConfig", EnableDNSConfig);
            jo.AddWithoutNull("myUUID", MyUUID);
            jo.AddWithoutNull("enableEmptyConversation", EnableEmptyConversation);
            jo.AddWithoutNull("customOSType", CustomOSType);
            jo.AddWithoutNull("customDeviceName", CustomDeviceName);


            if (SDKDataPath.Length == 0)
            {
#if !_WIN32
                jo.AddWithoutNull("sdkDataPath", Application.persistentDataPath);
#endif
            }
            else
            {
                jo.AddWithoutNull("sdkDataPath", SDKDataPath);
            }

            if (RestServer != null)
            {
                jo.AddWithoutNull("restServer", RestServer);
            }

            if (IMServer != null)
            {
                jo.AddWithoutNull("imServer", IMServer);
            }

            if (IMPort != 0)
            {
                jo.AddWithoutNull("imPort", IMPort);
            }

            if (DNSURL != null)
            {
                jo.AddWithoutNull("dnsUrl", DNSURL);
            }


            /* 暂不支持推送
            JSONObject pushConfig = new JSONObject();
            pushConfig.Add("mzAppId", mZAppId);
            pushConfig.Add("mzAppKey", mZAppKey);
            pushConfig.Add("enableMzPush", enableMZPush);

            pushConfig.Add("oppoAppKey", oPPOAppKey);
            pushConfig.Add("oppoAppSecret", oPPOAppSecret);
            pushConfig.Add("enableOppoPush", enableOPPOPush);

            pushConfig.Add("miAppId", miAppId);
            pushConfig.Add("miAppKey", miAppKey);
            pushConfig.Add("enableMiPush", enableMiPush);

            pushConfig.Add("fcmId", fCMId);
            pushConfig.Add("enableFcmPush", enableFCMPush);

            pushConfig.Add("apnsCerName", aPNsCerName);
            pushConfig.Add("enableApns", enableAPNs);

            pushConfig.Add("enableHwPush", enableHWPush);

            pushConfig.Add("enableVivoPush", enableVivoPush);

            jo.AddWithoutNull("pushConfig", pushConfig);
            */

            return jo;
        }
    }
}

