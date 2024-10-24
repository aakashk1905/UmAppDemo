using System.Collections.Generic;
using AgoraChat.SimpleJSON;

namespace AgoraChat
{
    /**
     * The presence manager class that defines methods of managing the presence state.
     */
    public class PresenceManager : BaseManager
    {

        List<IPresenceManagerDelegate> delegater;

        internal PresenceManager(NativeListener listener) : base(listener, SDKMethod.presenceManager)
        {

            listener.PresenceManagerEvent += NativeEventcallback;
            delegater = new List<IPresenceManagerDelegate>();
        }

        /**
         * Publishes a custom presence state.
         *
         * @param ext            The description information of the presence state. It can be an empty string.
         * @param callBack       The result callback which contains the error message if this method fails.
         */
        public void PublishPresence(string description, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("desc", description);
            NativeCall(SDKMethod.presenceWithDescription, jo_param, callback);
        }

        /**
         * Subscribes to a user's presence state. If the subscription succeeds, the subscriber will receive the onPresenceUpdated callback when the user's presence state changes.
         *
         * @param members  The array of user IDs whose presence states you want to subscribe to.
         * @param expiry   The subscription duration in seconds. The duration cannot exceed 2,592,000 (30×24×3600) seconds, i.e., 30 days.
         * @param callBack The result callback which contains the error message if the method fails. Returns the current presence state of subscribed users if this method executes successfully.
         */
        public void SubscribePresences(List<string> members, long expiry, ValueCallBack<List<Presence>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            jo_param.AddWithoutNull("expiry", expiry);

            Process process = (_, jsonNode) =>
            {
                return List.BaseModelListFromJsonArray<Presence>(jsonNode);
            };

            NativeCall<List<Presence>>(SDKMethod.presenceSubscribe, jo_param, callback, process);
        }

        /**
         * Unsubscribes from the presence state of the unspecified users.
         *
         * @param members  The array of user IDs whose presence state you want to unsubscribe from.
         * @param callBack The result callback, which contains the error message if the method fails.
         */
        public void UnsubscribePresences(List<string> members, CallBack callback = null)
        {
            JSONObject jo_paran = new JSONObject();
            jo_paran.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            NativeCall<List<Presence>>(SDKMethod.presenceUnsubscribe, jo_paran, callback);
        }

        /**
         * Uses pagination to get the list of users whose presence state you have subscribed to.
         *
         * @param pageNum  The current page number, starting from 1.
         * @param pageSize The number of subscribed users displayed on each page.
         * @param callBack The result callback, which contains user IDs whose presence state you have subscribed to. Returns an empty list if you do not subscribe to any user's presence state.
         */
        public void FetchSubscribedMembers(int pageNum, int pageSize, ValueCallBack<List<string>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("pageNum", pageNum);
            jo_param.AddWithoutNull("pageSize", pageSize);

            Process process = (_, jsonNode) =>
            {
                return List.StringListFromJsonArray(jsonNode);
            };

            NativeCall<List<string>>(SDKMethod.fetchSubscribedMembersWithPageNum, jo_param, callback, process);
        }

        /**
         * Gets the current presence state of the specified users.
         *
         * @param members  The array of user IDs whose current presence state you want to get.
         * @param callBack The result callback, which contains the current presence state of users you have subscribed to.
         */
        public void FetchPresenceStatus(List<string> members, ValueCallBack<List<Presence>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            Process process = (_, jsonNode) =>
            {
                return List.BaseModelListFromJsonArray<Presence>(jsonNode);
            };

            NativeCall<List<Presence>>(SDKMethod.fetchPresenceStatus, jo_param, callback, process);
        }

        /**
         * Adds a presence state listener.
         *
         * @param listener {@link IPresenceManagerDelegate} The presence state listener to add.
         */
        public void AddPresenceManagerDelegate(IPresenceManagerDelegate presenceManagerDelegate)
        {
            if (!delegater.Contains(presenceManagerDelegate))
            {
                delegater.Add(presenceManagerDelegate);
            }
        }

        /**
         * Removes a presence listener.
         *
         * @param listener {@link IPresenceManagerDelegate} The presence state listener to remove.
         */
        public void RemovePresenceManagerDelegate(IPresenceManagerDelegate presenceManagerDelegate)
        {
            delegater.Remove(presenceManagerDelegate);
        }

        internal void ClearDelegates()
        {
            delegater.Clear();
        }

        internal void NativeEventcallback(string method, JSONNode jsonNode)
        {

            if (delegater.Count == 0) return;

            List<Presence> list = List.BaseModelListFromJsonArray<Presence>(jsonNode);
            if (list != null)
            {
                foreach (IPresenceManagerDelegate it in delegater)
                {
                    switch (method)
                    {
                        case SDKMethod.onPresenceUpdated:
                            it.OnPresenceUpdated(list);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
