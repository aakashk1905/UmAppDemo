using System.Collections.Generic;
using AgoraChat.SimpleJSON;

namespace AgoraChat
{
    /**
 * The user information manager for updating and getting user attributes.
 */
    public class UserInfoManager : BaseManager
    {

        internal UserInfoManager(NativeListener listener) : base(listener, SDKMethod.userInfoManager)
        {

        }

        /**
         * Modifies the information of the current user.
         *
         * @param userInfo  The user information to be modified.
         * @param callback	The operation callback. See {@link CallBack}.
         */
        public void UpdateOwnInfo(UserInfo userInfo, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("userInfo", userInfo.ToJsonObject());
            NativeCall(SDKMethod.updateOwnUserInfo, jo_param, callback);
        }

        /**
         * Gets user information by user ID.
         *
         * @param userIds   The list of user IDs.
         * @param callback	The operation callback. If success, the user information dictionary is returned; otherwise, an error is returned. See {@link ValueCallBack}.
         */
        public void FetchUserInfoByUserId(List<string> userIds, ValueCallBack<Dictionary<string, UserInfo>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(userIds));

            Process process = (_, jsonNode) =>
            {
                return Dictionary.BaseModelDictionaryFromJsonObject<UserInfo>(jsonNode);
            };

            NativeCall<Dictionary<string, UserInfo>>(SDKMethod.fetchUserInfoById, jo_param, callback, process);
        }
    }
}
