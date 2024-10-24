using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{

    /**
     * The class that defines basic information of public groups.
     * 
     * To get basic information of a group from the server, you can call {@link IGroupManager#FetchPublicGroupsFromServer(int, String, ValueCallBack)}.
     */
    [Preserve]
    public class GroupInfo : BaseModel
    {
        /**
         * The group ID.
         */
        public string GroupId { get; internal set; }

        /**
	     * The group name.
	     */
        public string GroupName { get; internal set; }

        [Preserve]
        internal GroupInfo() { }

        [Preserve]
        internal GroupInfo(string jsonString) : base(jsonString) { }

        [Preserve]
        internal GroupInfo(JSONObject jsonObject) : base(jsonObject) { }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            GroupId = jsonObject["groupId"];
            GroupName = jsonObject["name"];
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("groupId", GroupId);
            jo.AddWithoutNull("name", GroupName);
            return jo;
        }
    }
}
