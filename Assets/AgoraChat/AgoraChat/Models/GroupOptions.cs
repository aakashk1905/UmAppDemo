using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    [Preserve]
    public class GroupOptions : BaseModel
    {
        /**
         * The group style. See {@link GroupStyle}.
         */
        public GroupStyle Style;

        /**
         * The maximum number of members allowed in a group.
         */
        public int MaxCount;

        /**
         * Whether to ask for consent when inviting a user to join a group.
         * 
         * Whether automatically accepting the invitation to join a group depends on two settings: 
         * - `inviteNeedConfirm`, an option for group creation.
         * - {@link Options#AutoAcceptGroupInvitation} which determines whether to automatically accept an invitation to join the group.
         * 
         * There are two cases:
         * - If `inviteNeedConfirm` is set to `false`, the SDK adds the invitee directly to the group on the server side, regardless of the setting of {@link Options#AutoAcceptGroupInvitation} on the invitee side.
         * - If `inviteNeedConfirm` is set to `true`, whether the invitee automatically joins the chat group or not depends on the settings of {@link Options#AutoAcceptGroupInvitation}.
         * 
         * {@link Options#AutoAcceptGroupInvitation} is an SDK-level operation. If it is set to `true`, the invitee automatically joins the chat group; if it is set to `false`, the invitee can manually accept or decline the group invitation instead of joining the group automatically.
         */
        public bool InviteNeedConfirm;

        /**
         * The group detail extensions which can be in the JSON format to contain more group information.
         */
        public string Ext;

        /**
         * The group option class constructor.
         */
        [Preserve]
        public GroupOptions(GroupStyle style, int count = 200, bool inviteNeedConfirm = false, string ext = null)
        {
            Style = style;
            MaxCount = count;
            InviteNeedConfirm = inviteNeedConfirm;
            Ext = ext;
        }

        [Preserve]
        internal GroupOptions() { }

        [Preserve]
        internal GroupOptions(string jsonString) : base(jsonString) { }

        [Preserve]
        internal GroupOptions(JSONObject jsonObject) : base(jsonObject) { }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jsonObject = new JSONObject();
            jsonObject.AddWithoutNull("style", (int)Style);
            jsonObject.AddWithoutNull("maxCount", MaxCount);
            jsonObject.AddWithoutNull("inviteNeedConfirm", InviteNeedConfirm);
            if (null != Ext)
            {
                jsonObject.AddWithoutNull("ext", Ext);
            }
            return jsonObject;
        }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            Style = (GroupStyle)jsonObject["style"].AsInt;
            MaxCount = jsonObject["maxCount"].AsInt;
            InviteNeedConfirm = jsonObject["inviteNeedConfirm"].AsBool;
            if (!jsonObject["ext"].IsNull)
            {
                Ext = jsonObject["ext"];
            }
        }
    }
}
