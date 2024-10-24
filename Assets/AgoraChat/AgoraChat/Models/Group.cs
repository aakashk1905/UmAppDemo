using System.Collections.Generic;
using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    [Preserve]
    public class Group : BaseModel
    {
        /**
         * The group ID.
         */
        public string GroupId { get; internal set; }

        /**
         * The group name.
         */
        public string Name { get; internal set; }

        /**
         * The group description.
         */
        public string Description { get; internal set; }

        /**
         * The group owner.
         * 
         */
        public string Owner { get; internal set; }

        /**
         * The group announcement.
         * 
         * To get the group announcement from the server, you can call {@link IGroupManager#GetGroupAnnouncementFromServer(String, ValueCallBack)}.
         */
        public string Announcement { get; internal set; }

        /**
         * The number of members in the group.
         * 
         */
        public int MemberCount { get; internal set; }

        /**
         * The member list of the group.
         * 
         */
        public List<string> MemberList { get; internal set; }

        /**
         * The admin list of the group.
         * 
         * To get the admin list of the group from the server, you can call {@link IGroupManager#GetGroupSpecificationFromServer(String, ValueCallBack)} to get group details.
         * 
         */
        public List<string> AdminList { get; internal set; }

        /**
         * Gets the block list of the group.
         * 
         * To get the block list of the group from the server, you can call {@link IGroupManager#GetGroupBlockListFromServer(String, int, int, ValueCallBack)}.
         * 
         */
        public List<string> BlockList { get; internal set; }

        /**
         * Gets the mute list of the group.
         * 
         * To get the mute list of the group from the server, you can call {@link IGroupManager#GetGroupMuteListFromServer(String, int, int, ValueCallBack)}.
         *
         */
        public List<string> MuteList { get; internal set; }

        /**
         * Whether push notifications are enabled.
         * - `true`: Yes.
         * - `false`: No.
         * 
         */
        public bool NoticeEnabled { get; internal set; }

        /**
         * Whether group messages are blocked.
         * - `true`: Yes.
         * - `false`: No.
         
         * To block group messages, you can call {@link IGroupManager#BlockGroup(String，CallBack}.
         * 
         * To unblock group messages, you can call {@link IGroupManager#UnBlockGroup(String, CallBack)}.
         * 
         */
        public bool MessageBlocked { get; internal set; }

        /**
         * Whether all members are muted.
         * - `true`: Yes.
         * - `false`: No.
         * 
         * The mute state of the in-memory object is updated when the mute callback {@link OnMuteListAddedFromGroup} or unmuting callback {@link OnMuteListRemovedFromGroup} is received.
         *
         *  After the in-memory object is collected and pulled again from the database or server, the state becomes untrustworthy.
         */
        public bool IsAllMemberMuted { get; internal set; }

        /**
         * The group options.
         * 
         */
        internal GroupOptions Options;

        /**
         * The role of the current user in the group.
         * 
         */
        public GroupPermissionType PermissionType { get; internal set; }

        /**
         * Whether users can join a group only via a join request or a group invitation:
         * - `true`: Yes.
         * - `false`: No. Users can join a group freely, without a join request or a group invitation.
         * 
         * When `GroupStyle` is set to `PrivateOnlyOwnerInvite`, `PrivateMemberCanInvite`, or `PublicJoinNeedApproval`, the attribute value is `true`. 
         * 
         *
         */
        public bool IsMemberOnly { get; internal set; }

        /**
         * Whether other group members than the group owner and admins can invite users to join the group:
         * - `true`: Yes. All group members can invite other users to join the group.
         * - `false`: No. Only the group owner and admins can invite other users to join the group.
         */
        public bool IsMemberAllowToInvite { get; internal set; }

        /**
         * The maximum number of members allowed in a group. 
         * 
         * The attribute is set during group creation. 
         
         * To get the correct attribute value, you need to first get the details of the group from the server by calling {@link IGroupManager#GetGroupSpecificationFromServer(String, ValueCallBack)}. Otherwise, the SDK returns `0`.
         */
        public int MaxUserCount { get; internal set; }


        /**
        *  Whether the group is disabled:
        *  - `true`: Yes. The group is disabled. Group members cannot send or receive messages, nor perform group and member management operations. This is also the case for threads in this group.
        *  - `false`: No. The group is in the normal state. Group members can send and receive messages, as well as perform group and member management operations. This is also the case for threads in this group.
        * 
        *  This attribute is not stored in the local database.
        *  
        * For groups loaded from the local database, the default value of this attribute is `NO`.
         */
        public bool IsDisabled { get; internal set; }

        /**
         * Gets the custom extension information of the group.
         * 
         * @return  The custom extension information of the group.
         */
        public string Ext { get; internal set; }

        [Preserve]
        internal Group() { }

        [Preserve]
        internal Group(string jsonString) : base(jsonString) { }

        [Preserve]
        internal Group(JSONObject jsonObject) : base(jsonObject) { }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            GroupId = jsonObject["groupId"];
            Name = jsonObject["name"];
            Description = jsonObject["desc"];
            Owner = jsonObject["owner"];
            Announcement = jsonObject["announcement"];
            MemberCount = jsonObject["memberCount"];
            MemberList = List.StringListFromJsonArray(jsonObject["memberList"]);
            AdminList = List.StringListFromJsonArray(jsonObject["adminList"]);
            BlockList = List.StringListFromJsonArray(jsonObject["blockList"]);
            MuteList = List.StringListFromJsonArray(jsonObject["muteList"]);
            MessageBlocked = jsonObject["block"];
            IsAllMemberMuted = jsonObject["isMuteAll"];
            //Options = ModelHelper.CreateWithJsonObject<GroupOptions>(jsonObject["options"]);
            MaxUserCount = jsonObject["maxUserCount"].AsInt;
            IsMemberOnly = jsonObject["isMemberOnly"].AsBool;
            IsMemberAllowToInvite = jsonObject["isMemberAllowToInvite"].AsBool;
            Ext = jsonObject["ext"];
            PermissionType = (GroupPermissionType)jsonObject["permissionType"].AsInt;
            IsDisabled = jsonObject["isDisabled"].AsBool;
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("groupId", GroupId);
            jo.AddWithoutNull("name", Name);
            jo.AddWithoutNull("desc", Description);
            jo.AddWithoutNull("owner", Owner);
            jo.AddWithoutNull("announcement", Announcement);
            jo.AddWithoutNull("memberCount", MemberCount);
            jo.AddWithoutNull("memberList", JsonObject.JsonArrayFromStringList(MemberList));
            jo.AddWithoutNull("adminList", JsonObject.JsonArrayFromStringList(AdminList));
            jo.AddWithoutNull("blockList", JsonObject.JsonArrayFromStringList(BlockList));
            jo.AddWithoutNull("muteList", JsonObject.JsonArrayFromStringList(MuteList));
            jo.AddWithoutNull("block", MessageBlocked);
            jo.AddWithoutNull("isMuteAll", IsAllMemberMuted);
            jo.AddWithoutNull("permissionType", PermissionType.ToInt());
            // jo.AddWithoutNull("options", Options.ToJsonObject());
            // jo.AddWithoutNull("isDisabled", IsDisabled);
            return jo;
        }
    }
}
