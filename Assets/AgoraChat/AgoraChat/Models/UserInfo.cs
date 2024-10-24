using AgoraChat.SimpleJSON;
#if !_WIN32
using UnityEngine.Scripting;
#endif

namespace AgoraChat
{
    /**
    * The user information class.
    */
    [Preserve]
    public class UserInfo : BaseModel
    {

        /**
         * The nickname of the user.
         */
        public string NickName = "";
        /**
         * The avatar URL of the user.
         */
        public string AvatarUrl = "";
        /**
         * The email address of the user.
         */
        public string Email = "";
        /**
        * The phone number of the user.
        */
        public string PhoneNumber = "";
        /**
         * The signature of the user.
         */
        public string Signature = "";
        /**
        * The birthday of the user.
        */
        public string Birth = "";
        /**
         * The user ID.
         */
        public string UserId = "";
        /**
        * The extension information of the user. 
        * 
        * You can specify either an empty string or the custom information encapsulated as the JSON string.
        */
        public string Ext = "";
        /**
         * The gender of the user.
         * - (Default) `0`: Unknown.
         * - `1`: Male.
         * - `2`: Female.
         */
        public int Gender = 0;

        [Preserve]
        public UserInfo() { }

        [Preserve]
        internal UserInfo(string jsonString) : base(jsonString) { }

        [Preserve]
        internal UserInfo(JSONObject jsonObject) : base(jsonObject) { }

        internal override void FromJsonObject(JSONObject jsonObject)
        {
            if (!jsonObject["nickName"].IsNull)
            {
                NickName = jsonObject["nickName"].Value;
            }

            if (!jsonObject["avatarUrl"].IsNull)
            {
                AvatarUrl = jsonObject["avatarUrl"].Value;
            }

            if (!jsonObject["mail"].IsNull)
            {
                Email = jsonObject["mail"].Value;
            }

            if (!jsonObject["phone"].IsNull)
            {
                PhoneNumber = jsonObject["phone"].Value;
            }

            if (!jsonObject["sign"].IsNull)
            {
                Signature = jsonObject["sign"].Value;
            }

            if (!jsonObject["birth"].IsNull)
            {
                Birth = jsonObject["birth"].Value;
            }

            if (!jsonObject["userId"].IsNull)
            {
                UserId = jsonObject["userId"].Value;
            }

            if (!jsonObject["gender"].IsNull)
            {
                Gender = jsonObject["gender"].AsInt;
            }

            if (!jsonObject["ext"].IsNull)
            {
                Ext = jsonObject["ext"].Value;
            }
        }

        internal override JSONObject ToJsonObject()
        {
            JSONObject jo = new JSONObject();
            jo.AddWithoutNull("nickName", NickName);
            jo.AddWithoutNull("avatarUrl", AvatarUrl);
            jo.AddWithoutNull("mail", Email);
            jo.AddWithoutNull("phone", PhoneNumber);
            jo.AddWithoutNull("sign", Signature);
            jo.AddWithoutNull("birth", Birth);
            jo.AddWithoutNull("gender", Gender);
            jo.AddWithoutNull("userId", UserId);
            jo.AddWithoutNull("ext", Ext);

            return jo;
        }
    }
}
