using System.Collections.Generic;
using AgoraChat.SimpleJSON;

namespace AgoraChat
{
    /**
    * The contact manager class, which manages chat contacts such as adding, retrieving, and deleting contacts.
    */
    public class ContactManager : BaseManager
    {
        internal List<IContactManagerDelegate> delegater;

        internal ContactManager(NativeListener listener) : base(listener, SDKMethod.contactManager)
        {
            listener.ContactManagerEvent += NativeEventHandle;
            delegater = new List<IContactManagerDelegate>();
        }

        ~ContactManager()
        {

        }

        /**
        * Adds a new contact.
        *
        * This is an asynchronous method.
        * 
        * @param userId		    The user ID of the contact to add.
        * @param reason     	The invitation message. This parameter is optional and can be set to `null` or "".
        * @param callback	        The result callback. See {@link CallBack}.
        */
        public void AddContact(string userId, string reason = null, CallBack callback = null)
        {

            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("userId", userId);
            jo_param.AddWithoutNull("msg", reason);

            NativeCall(SDKMethod.addContact, jo_param, callback);

        }

        /**
         * Deletes a contact and all the related conversations.
         *
         * This is an asynchronous method.
         *
         * @param userId            The user ID of the contact to delete.
         * @param keepConversation  Whether to retain conversations of the contact to delete.
         *                          - `true`: Yes.
         *                          - (Default) `false`: No.
         * @param callback	        The result callback. See {@link CallBack}.
         */
        public void DeleteContact(string userId, bool keepConversation = false, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("userId", userId);
            jo_param.AddWithoutNull("keepConversation", keepConversation);

            NativeCall(SDKMethod.deleteContact, jo_param, callback);
        }

        /**
        * Gets the contact list from the server.
        * 
        * This is an asynchronous method.
        *
        * @param callback The result callback. If success, the SDK returns the list of contacts; if a failure occurs, the SDK returns the error information. See {@link ValueCallBack}.
        */
        public void GetAllContactsFromServer(ValueCallBack<List<string>> callback = null)
        {
            Process process = (_, jsonNode) =>
            {
                return List.StringListFromJsonArray(jsonNode);
            };

            NativeCall<List<string>>(SDKMethod.getAllContactsFromServer, null, callback, process);
        }

        /**
         * Gets the contact list from the local database.
         *
         * @return  If success, the SDK returns the list of contacts; if a failure occurs, the SDK returns an empty list.
         
         */
        public List<string> GetAllContactsFromDB()
        {
            JSONNode jn = NativeGet(SDKMethod.getAllContactsFromDB).GetReturnJsonNode();
            return List.StringListFromJsonArray(jn);
        }

        /**
         * Adds a contact to the block list.
         * You can send messages to the users on the block list, but cannot receive messages from them.
         * 
         * This is an asynchronous method.
         *
         * @param userId      The user ID of the contact to be added to the block list.
         * @param callback	The result callback. See {@link CallBack}.
         */
        public void AddUserToBlockList(string userId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("userId", userId);

            NativeCall(SDKMethod.addUserToBlockList, jo_param, callback);
        }

        /**
         * Removes the contact from the block list.
         * 
         * This is an asynchronous method.
         *
         * @param userId    The user ID of the contact to be removed from the block list.
         * @param callback	The result callback. See {@link CallBack}.
         */
        public void RemoveUserFromBlockList(string userId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("userId", userId);

            NativeCall(SDKMethod.removeUserFromBlockList, jo_param, callback);
        }

        /**
         * Gets the block list from the server.
         * 
         * This is an asynchronous method.
         *
         * @param callback    The result callback. If success, the SDK returns the block list; if a failure occurs, the SDK returns the error information. See {@link ValueCallBack}.
         */
        public void GetBlockListFromServer(ValueCallBack<List<string>> callback = null)
        {
            Process process = (_, jsonNode) =>
            {
                return List.StringListFromJsonArray(jsonNode);
            };

            NativeCall<List<string>>(SDKMethod.getBlockListFromServer, null, callback, process);
        }

        /**
         * Gets the local blocklist.
         *
         * @return The blocklist.
         */
        public List<string> GetBlockListFromDB()
        {
            JSONNode jn = NativeGet(SDKMethod.getBlockListFromDB).GetReturnJsonNode();
            return List.StringListFromJsonArray(jn);
        }

        /**
         * Accepts a friend invitation.
         *
         * This an asynchronous method.
         *
         * @param userId    The user who initiates the friend invitation.
         * @param callback  The result callback. See {@link CallBack}.
         */
        public void AcceptInvitation(string userId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("userId", userId);

            NativeCall(SDKMethod.acceptInvitation, jo_param, callback);
        }

        /**
         * Declines a friend invitation.
         *      
         * This an asynchronous method.
         *
         * @param userId      The user who initiates the friend invitation.
         * @param callback    The result callback. See {@link CallBack}.
         */
        public void DeclineInvitation(string userId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("userId", userId);

            NativeCall(SDKMethod.declineInvitation, jo_param, callback);
        }

        /**
         * Gets the unique IDs of the current user on the other devices. 
         *
         * The ID is in the format of user ID + "/" + resource.
         *      
         * This is an asynchronous method.
         *
         * @param callback   The result callback. If success, the SDK returns the unique IDs of the current user on the other devices; if a failure occurs, the SDK returns the error information. See {@link ValueCallBack}.
                            
         * 
         */
        public void GetSelfIdsOnOtherPlatform(ValueCallBack<List<string>> callback = null)
        {
            Process process = (_, jsonNode) =>
            {
                return List.StringListFromJsonArray(jsonNode);
            };

            NativeCall<List<string>>(SDKMethod.getSelfIdsOnOtherPlatform, null, callback, process);
        }

        /**
		 * Adds a contact listener.
		 *
		 * @param contactManagerDelegate 		The contact listener to add. It is inherited from {@link IContactManagerDelegate}.
		 * 
		 */
        public void AddContactManagerDelegate(IContactManagerDelegate contactManagerDelegate)
        {
            if (!delegater.Contains(contactManagerDelegate))
            {
                delegater.Add(contactManagerDelegate);
            }
        }

        /**
		 * Removes a contact listener.
		 *
		 * @param contactManagerDelegate 		The contact listener to remove. It is inherited from {@link IContactManagerDelegate}.
		 * 
		 */
        public void RemoveContactManagerDelegate(IContactManagerDelegate contactManagerDelegate)
        {
            delegater.Remove(contactManagerDelegate);

        }

        internal void ClearDelegates()
        {
            delegater.Clear();
        }

        internal void NativeEventHandle(string method, JSONNode jsonNode)
        {
            if (delegater.Count == 0) return;

            string reason = jsonNode["msg"];
            string userId = jsonNode["userId"];

            foreach (IContactManagerDelegate it in delegater)
            {
                switch (method)
                {
                    case SDKMethod.onContactAdded:
                        it.OnContactAdded(userId);
                        break;
                    case SDKMethod.onContactDeleted:
                        it.OnContactDeleted(userId);
                        break;
                    case SDKMethod.onContactInvited:
                        it.OnContactInvited(userId, reason);
                        break;
                    case SDKMethod.onFriendRequestAccepted:
                        it.OnFriendRequestAccepted(userId);
                        break;
                    case SDKMethod.onFriendRequestDeclined:
                        it.OnFriendRequestDeclined(userId);
                        break;
                    default:
                        break;
                }
            }
        }
    }

}
