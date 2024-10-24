using System.Collections.Generic;
using AgoraChat.SimpleJSON;

namespace AgoraChat
{

    /**
     * The abstract class for the chat manager.
     * 
     */
    public class RoomManager : BaseManager
    {
        internal List<IRoomManagerDelegate> delegater;

        internal RoomManager(NativeListener listener) : base(listener, SDKMethod.roomManager)
        {
            listener.RoomManagerEvent += NativeEventcallback;
            delegater = new List<IRoomManagerDelegate>();
        }

        /**
	     * Adds a chat room admin.
		 * 
	     * Only the chat room owner can call this method.
	     *
	     * This is an asynchronous method.
	     *
	     * @param chatRoomId	The chat room ID.
	     * @param memberId		The user ID of the chat room admin to be added.
	     * @param callback        The operation result callback. See {@link CallBack}.
	     */
        public void AddRoomAdmin(string roomId, string memberId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("userId", memberId);
            NativeCall(SDKMethod.addChatRoomAdmin, jo_param, callback);
        }

        /**
		 * Adds members to the block list of the chat room.
		 * 
		 * Only the chat room owner or admin can call this method.
		 * 
		 * **Note**
		 * 
		 * - A member, once added to block list of the chat room, will be removed from the chat room by the server.
		 * - After being added to the chat room block list, the member is notified via the {@link IRoomManagerDelegate#OnRemovedFromRoom( String, String, String)} callback.
		 * - Members on the block list cannot rejoin the chat room.
		 *
		 * This is an asynchronous method.
		 *
		 * @param roomId	    The chat room ID.
		 * @param members		The list of members to be added to block list of the chat room.
		 * @param callback        The operation result callback. See {@link CallBack}.
		 */
        public void BlockRoomMembers(string roomId, List<string> members, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            NativeCall(SDKMethod.blockChatRoomMembers, jo_param, callback);
        }

        /**
		 * Transfers the chat room ownership.
		 * 
		 * Only the chat room owner can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param chatRoomId	The chat room ID.
		 * @param newOwner		The user ID of the new chat room owner.
		 * @param callback        The operation result callback. See {@link CallBack}.
		 */
        public void ChangeRoomOwner(string roomId, string newOwner, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("userId", newOwner);
            NativeCall(SDKMethod.changeChatRoomOwner, jo_param, callback);
        }

        /**
		 * Modifies the chat room description.
		 * 
		 * Only the chat room owner can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param roomId		    The chat room ID.
		 * @param newDescription	The new description of the chat room.
		 * @param callback            The operation result callback. See {@link CallBack}.
		 */
        public void ChangeRoomDescription(string roomId, string newDescription, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("desc", newDescription);
            NativeCall(SDKMethod.changeChatRoomDescription, jo_param, callback);
        }

        /**
		 * Changes the chat room name.
		 * 
		 * Only the chat room owner can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param roomId	    The chat room ID.
		 * @param newName	    The new name of the chat room.
		 * @param callback        The operation result callback. See {@link CallBack}.
		 */
        public void ChangeRoomName(string roomId, string newName, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("name", newName);
            NativeCall(SDKMethod.changeChatRoomSubject, jo_param, callback);
        }

        /**
		 * Creates a chat room.
		 *
		 * This is an asynchronous method.
		 *
		 * @param name              The chat room name.
		 * @param description       The chat room description.
		 * @param welcomeMsg        A welcome message inviting members to join the chat room.
		 * @param maxUserCount      The maximum number of members allowed to join the chat room.
		 * @param members           The list of members invited to join the chat room.
		 * @param callback			The operation result callback. See {@link CallBack}.
		 */
        public void CreateRoom(string name, string descriptions = null, string welcomeMsg = null, int maxUserCount = 300, List<string> members = null, ValueCallBack<Room> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("name", name);
            jo_param.AddWithoutNull("desc", descriptions);
            jo_param.AddWithoutNull("msg", welcomeMsg);
            jo_param.AddWithoutNull("count", maxUserCount);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));

            Process process = (_, jsonNode) =>
            {
                return ModelHelper.CreateWithJsonObject<Room>(jsonNode);
            };

            NativeCall<Room>(SDKMethod.createChatRoom, jo_param, callback, process);
        }

        /**
		 * Destroys a chat room.
		 * 
		 * Only the chat room owner can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param roomId		The chat room ID.
		 * @param callback		The operation result callback. See {@link CallBack}.
		 */
        public void DestroyRoom(string roomId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            NativeCall(SDKMethod.destroyChatRoom, jo_param, callback);
        }

        /**
		 * Gets chat room data from the server with pagination.
		 * 
		 * For a large but unknown quantity of data, you can get data with pagination by specifying `pageNum` and `pageSize`.
		 *
		 * This is an asynchronous method.
		 *
		 * @param pageNum 	The page number, starting from 1.
		 * @param pageSize 	The number of records that you expect to get on each page. For the last page, the number of returned records is less than the parameter value.
		 * @param callback	The operation result callback. See {@link CallBack}.
		 * 
		 */
        public void FetchPublicRoomsFromServer(int pageNum = 1, int pageSize = 200, ValueCallBack<PageResult<Room>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("pageNum", pageNum);
            jo_param.AddWithoutNull("pageSize", pageSize);
            Process process = (_, jsonNode) =>
            {
                PageResult<Room> cursor_msg = new PageResult<Room>(_, (jn) =>
                {
                    return ModelHelper.CreateWithJsonObject<Room>(jn);
                });

                cursor_msg.FromJsonObject(jsonNode.AsObject);
                return cursor_msg;
            };
            NativeCall<PageResult<Room>>(SDKMethod.fetchPublicChatRoomsFromServer, jo_param, callback, process);
        }

        /**
		 * Gets the chat room announcement from the server.
		 * 
		 * This is an asynchronous method.
		 * 
		 * @param roomId 		The chat room ID.
		 * @param callback		The operation result callback. See {@link CallBack}.
		 */
        public void FetchRoomAnnouncement(string roomId, ValueCallBack<string> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            Process process = (_, jsonNode) =>
            {
                return jsonNode["ret"].IsString ? jsonNode["ret"].Value : null;
            };

            NativeCall<string>(SDKMethod.fetchChatRoomAnnouncement, jo_param, callback, process);
        }

        /**
		 * Gets the block list of the chat room with pagination.
		 *
		 * For a large but unknown quantity of data, you can get data with pagination by specifying `pageSize` and `cursor`.
		 *
		 * Only the chat room owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param roomId		The chat room ID.
		 * @param pageNum		The page number, starting from 1. 
		 * @param pageSize		The number of users on the block list that you expect to get on each page. For the last page, the number of returned users is less than the parameter value.
		 * @param callback		The operation result callback. See {@link CallBack}.
		 *
		 */
        public void FetchRoomBlockList(string roomId, int pageNum = 1, int pageSize = 200, ValueCallBack<List<string>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("pageNum", pageNum);
            jo_param.AddWithoutNull("pageSize", pageSize);
            Process process = (_, jsonNode) =>
            {
                return List.StringListFromJsonArray(jsonNode);
            };

            NativeCall<List<string>>(SDKMethod.fetchChatRoomBlockList, jo_param, callback, process);
        }

        /**
		 * Gets details of a chat room from the server, excluding the member list by default.
		 *
		 * This is an asynchronous method.
		 *
		 * @param roomId	The chat room ID.
		 * @param callback	The operation callback. If success, the chat room instance is returned; otherwise, an error is returned. See {@link ValueCallBack}.
		 */
        public void FetchRoomInfoFromServer(string roomId, ValueCallBack<Room> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.Add("roomId", roomId);

            Process process = (_, jsonNode) =>
            {
                return ModelHelper.CreateWithJsonObject<Room>(jsonNode);
            };

            NativeCall<Room>(SDKMethod.fetchChatRoomInfoFromServer, jo_param, callback, process);
        }
        /**
         * Gets the chat room member list with pagination.
         * For a large but unknown quantity of data, you can get data with pagination by specifying `pageSize` and `cursor`.
         *
         * This is an asynchronous method.
         *
         * @param roomId		The chat room ID.
         * @param cursor		The cursor position from which to start to get data.
         *                      At the first method call, if you set `cursor` as `null`, the SDK gets the data in the reverse chronological order of when users joined the chat room. 
         *                      Amid the returned data (CursorResult), `cursor` is a field saved locally and the updated cursor can be passed as the position from which to start to get data for the next query.
         * @param pageSize		The number of members that you expect to get on each page. For the last page, the number of returned members is less than the parameter value.
         * @param callback		The operation callback. If success, the chat room member list is returned; otherwise, an error is returned. See {@link ValueCallBack}.
         * 
         */
        public void FetchRoomMembers(string roomId, string cursor = "", int pageSize = 200, ValueCallBack<CursorResult<string>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("cursor", cursor);
            jo_param.AddWithoutNull("pageSize", pageSize);
            Process process = (_, jsonNode) =>
            {
                CursorResult<string> cursor_msg = new CursorResult<string>(_, (jn) =>
                {
                    return jn.IsString ? jn.Value : null;
                });

                cursor_msg.FromJsonObject(jsonNode.AsObject);
                return cursor_msg;

            };
            NativeCall<CursorResult<string>>(SDKMethod.fetchChatRoomMembers, jo_param, callback, process);
        }

        /**
         * Gets the list of members who are muted in the chat room.
         *
         * For a large but unknown quantity of data, you can get data with pagination by specifying `pageSize` and `cursor`.
         * 
         * Only the chat room owner or admin can call this method.
         *
         * This is an asynchronous method.
         *
         * @param roomId		The chat room ID.
         * @param pageNum		The page number, starting from 1.
         * @param pageSize		The number of muted members that you expect to get on each page. For the last page, the actual number of returned members is less than the parameter value.
         * @param callback		The operation callback. If success, the chat room mute list is returned; otherwise, an error is returned. See {@link ValueCallBack}.
         * 
         */
        public void FetchRoomMuteList(string roomId, int pageSize = 200, int pageNum = 1, ValueCallBack<Dictionary<string, long>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("pageNum", pageNum);
            jo_param.AddWithoutNull("pageSize", pageSize);

            Process process = (_, jsonNode) =>
            {
                return Dictionary.SimpleTypeDictionaryFromJsonObject<long>(jsonNode);
            };

            NativeCall<Dictionary<string, long>>(SDKMethod.fetchChatRoomMuteList, jo_param, callback, process);
        }

        /**
         * Joins the chat room.
         * 
         * To exit the chat room, you can call {@link #LeaveRoom(String, CallBack)}.
         *
         * This is an asynchronous method.
         *
         * @param roomId 	The ID of the chat room to join.
         * @param callback	The operation callback. If success, the chat room instance is returned; otherwise, an error is returned. See {@link ValueCallBack}.
         */
        public void JoinRoom(string roomId, ValueCallBack<Room> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);

            Process process = (_, jsonNode) =>
            {
                return ModelHelper.CreateWithJsonObject<Room>(jsonNode);
            };

            NativeCall<Room>(SDKMethod.joinChatRoom, jo_param, callback, process);
        }

        /**
         * Leaves a chat room.
         * 
         * After joining a chat room via {@link #JoinRoom(String, ValueCallBack)}, the member can call `LeaveRoom` to leave the chat room.
         *
         * This is an asynchronous method.
         *
         * @param roomId 	The ID of the chat room to leave.
         * @param callback	The operation callback. See {@link CallBack}.
         * 
         */
        public void LeaveRoom(string roomId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            NativeCall(SDKMethod.leaveChatRoom, jo_param, callback);
        }

        /**
         * Mutes members in a chat room.
         * 
         * Only the chat room owner or admin can call this method.
         *
         * This is an asynchronous method.
         *
         * @param roomId	The chat room ID.
         * @param members 	The list of members to be muted.
         * @param muteMilliseconds Muted time duration in millisecond, -1 stand for eternity.
         * @param callback	The operation callback. See {@link CallBack}.
         */
        public void MuteRoomMembers(string roomId, List<string> members, long muteMilliseconds = -1, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            jo_param.AddWithoutNull("expireTime", muteMilliseconds);
            NativeCall(SDKMethod.muteChatRoomMembers, jo_param, callback);
        }

        /**
         * Removes the administrative privileges of a chat room admin.
         * 
         * Only the chat room owner can call this method.
         *
         * This is an asynchronous method.
         *
         * @param roomId		The chat room ID.
         * @param adminId		The user ID of the admin whose administrative privileges are to be removed.
         * @param callback		The operation callback. See {@link CallBack}.
         */
        public void RemoveRoomAdmin(string roomId, string adminId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("userId", adminId);
            NativeCall(SDKMethod.removeChatRoomAdmin, jo_param, callback);
        }

        /**
         * Removes members from a chat room.
         * 
         * Only the chat room owner or admin can call this method.
         *
         * This is an asynchronous method.
         *
         * @param roomId		The chat room ID.
         * @param members		The list of members to be removed from the chat room.
         * @param callback		The operation callback. See {@link CallBack}.
         */
        public void DeleteRoomMembers(string roomId, List<string> members, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            NativeCall(SDKMethod.removeChatRoomMembers, jo_param, callback);
        }

        /**
         * Removes members from the block list of the chat room.
         * 
         * Only the chat room owner or admin can call this method.
         *
         * This is an asynchronous method.
         *
         * @param roomId		The chat room ID.
         * @param members		The list of members to be removed from the block list of the chat room.
         * @param callback		The operation callback. See {@link CallBack}.
         */
        public void UnBlockRoomMembers(string roomId, List<string> members, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            NativeCall(SDKMethod.unBlockChatRoomMembers, jo_param, callback);
        }

        /**
         * Unmutes members in a chat room.
         * 
         * Only the chat room owner or admin can call this method.
         *
         * This is an asynchronous method.
         *
         * @param roomId		The chat room ID.
         * @param members		The list of members to be unmuted.
         * @param callback		The operation callback. See {@link CallBack}.
         * 
         */
        public void UnMuteRoomMembers(string roomId, List<string> members, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            NativeCall(SDKMethod.unMuteChatRoomMembers, jo_param, callback);
        }

        /**
         * Updates the chat room announcement.
         * 
         * Only the chat room owner or admin can call this method.
         *
         * This is an asynchronous method.
         *
         * @param roomId 		The chat room ID.
         * @param announcement 	The announcement content.
         * @param callback		The operation callback. See {@link CallBack}.
         * 
         */
        public void UpdateRoomAnnouncement(string roomId, string announcement, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("announcement", announcement);
            NativeCall(SDKMethod.updateChatRoomAnnouncement, jo_param, callback);
        }

        /**
         * Mutes all members.
         * Only the chat room owner or admin can call this method.
         * This method does not work for the chat room owner, admin, and members added to the allow list.
         *
         * This is an asynchronous method.
         *
         * @param roomId    The chat room ID.
         * @param callback    The completion callback. 
         *                  - If this call succeeds, the SDK calls {@link ValueCallBack#onSuccess(Object)};
         *                  - If this call fails, the SDK calls {@link ValueCallBack#onError(int, String)}.
         */
        public void MuteAllRoomMembers(string roomId, ValueCallBack<Room> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);

            Process process = (_, jsonNode) =>
            {
                return ModelHelper.CreateWithJsonObject<Room>(jsonNode);
            };

            NativeCall<Room>(SDKMethod.muteAllChatRoomMembers, jo_param, callback, process);
        }

        /**
         * Unmutes all members.
         * 
         * Only the chat room owner or admin can call this method.
         *
         * This is an asynchronous method.
         *
         * @param roomId    The chat room ID.
         * @param callback    The completion callback. 
         *                  - If this call succeeds, the SDK calls {@link ValueCallBack#onSuccess(Object)};
         *                  - If this call fails, the SDK calls {@link ValueCallBack#onError(int, String)}.
         */
        public void UnMuteAllRoomMembers(string roomId, ValueCallBack<Room> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);

            Process process = (_, jsonNode) =>
            {
                return ModelHelper.CreateWithJsonObject<Room>(jsonNode);
            };

            NativeCall<Room>(SDKMethod.unMuteAllChatRoomMembers, jo_param, callback, process);
        }

        /**
         * Adds members to the allow list.
         * 
         * Only the chat room owner or admin can call this method.
         * 
         * A call to the {@link #MuteAllMembers} method by the chat room owner or admin does not affect members on the allow list.
         *
         * This is an asynchronous method.
         *
         * @param roomId       The chat room ID.
         * @param members      The list of members to be added to the allow list.
         * @param callback       The completion callback. 
         *                     - If this call succeeds, the SDK calls {@link ValueCallBack#onSuccess(Object)};
         *                     - If this call fails, the SDK calls {@link ValueCallBack#onError(int, String)}.
         */
        public void AddAllowListMembers(string roomId, List<string> members, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            NativeCall(SDKMethod.addMembersToChatRoomAllowList, jo_param, callback);
        }

        /**
         * Removes members from the block list.
         * 
         * Only the chat room owner or admin can call this method.
         * 
         * When members are removed from the block list, a call to the method {@link #MuteAllMembers(String, EMValueCallBack)} will also mute them.
         *
         * This is an asynchronous method.
         *
         * @param roomId        The chat room ID.
         * @param members       The list of members to be removed from the block list.
         * @param callback        The completion callback. 
         *                      - If this call succeeds, the SDK calls {@link ValueCallBack#onSuccess(Object)};
         *                      - If this call fails, the SDK calls {@link ValueCallBack#onError(int, String)}.
         */
        public void RemoveAllowListMembers(string roomId, List<string> members, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            NativeCall(SDKMethod.removeMembersFromChatRoomAllowList, jo_param, callback);
        }

        /**
         * Gets the chat room allow list from the server.
         * 
         * Only the chat room owner or admin can call this method.
         *
         * This is an asynchronous method.
         *
         * @param chatRoomId 	The chat room ID.
         * @param callBack		The completion callback. If this call succeeds, the SDK calls {@link ValueCallBack#OnSuccessValue(Object)};
         * 						If this call fails, the SDK calls {@link ValueCallBack#onError(int, String)}.
         */
        public void FetchAllowListFromServer(string roomId, ValueCallBack<List<string>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);

            Process process = (_, jsonNode) =>
            {
                return List.StringListFromJsonArray(jsonNode);
            };

            NativeCall<List<string>>(SDKMethod.fetchChatRoomAllowListFromServer, jo_param, callback, process);
        }

        /**
         * Checks whether the current member is on the chat room block list.
         *
         * This is an asynchronous method.
         *
         * @param roomId 	The chat room ID.
         * @param callBack		The completion callback. If this call succeeds, the SDK calls {@link ValueCallBack#OnSuccessValue(Object)} to show whether the member is on the block list;
         * 						if this call fails, the SDK calls {@link ValueCallBack#onError(int, String)}.
         */
        public void CheckIfInRoomAllowList(string roomId, ValueCallBack<bool> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            Process process = (_, jsonNode) =>
            {
                return jsonNode.IsBoolean ? jsonNode.AsBool : false;
            };

            NativeCall<bool>(SDKMethod.isMemberInChatRoomAllowListFromServer, jo_param, callback, process);
        }

        /**
         * Gets the chat room in the memory.
         *
         * @param roomId		The chat room ID.
         * @return 				The chat room instance. The SDK creates a new chat room instance based on the chat room ID (roomId) and returns it if the chat room is not found in the memory.
         */
        public Room GetChatRoom(string roomId)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);

            JSONNode jn = NativeGet<Room>(SDKMethod.getChatRoom, jo_param).GetReturnJsonNode();
            return ModelHelper.CreateWithJsonObject<Room>(jn);
        }

        /*
         * @deprecated
        public void FetchAllRoomsFromServer(ValueCallBack<List<Room>> callback = null)
        {
            Process process = (_, jsonNode) =>
            {
                return List.BaseModelListFromJsonArray<Room>(jsonNode);
            };

            NativeCall<List<Room>>(SDKMethod.getAllChatRooms, null, callback, process);
        }
        */

        /**
         * Sets custom chat room attributes.
         * All members in the chat room owner can call this method.
         *
         * This is an asynchronous method.
         *
         * @param roomId        The chat room ID.
         * @param kv            The chat room attributes to add. The attributes are in key-value format. 
         *                      In a key-value pair, the key is the attribute name that can contain 128 characters at most; the value is the attribute value that cannot exceed 4096 characters. 
         *                      A chat room can have a maximum of 100 custom attributes and the total length of custom chat room attributes cannot exceed 10 GB for each app. Attribute keys support the following character sets:
         *						 - 26 lowercase English letters (a-z)
         *						 - 26 uppercase English letters (A-Z)
         *						 - 10 numbers (0-9)
         *						 - "_", "-", "."
         * @deleteWhenExit      Whether to delete the chat room attributes set by the member when he or she exits the chat room.
         * 						- (Default)`true`: Yes.
         *						- `false`: No.
         * @forced              Whether to overwrite the attributes with same key set by others.
         * 						- `true`: Yes.
         *						- (Default)`false`: No.
         * @param callback        The completion callback. If this call succeeds, the SDK calls {@link ValueCallBack#OnSuccessValue(Dictionary<string, int>)};
         *                      if this call fails, the SDK calls {@link CallBackResult#onError(int, String)}.
         */
        public void AddAttributes(string roomId, Dictionary<string, string> kv, bool deleteWhenExit = true, bool forced = false, ValueCallBack<Dictionary<string, int>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("kv", JsonObject.JsonObjectFromDictionary(kv));
            jo_param.AddWithoutNull("deleteWhenExit", deleteWhenExit);
            jo_param.AddWithoutNull("forced", forced);

            Process process = (_, jsonNode) =>
            {
                return Dictionary.SimpleTypeDictionaryFromJsonObject<int>(jsonNode);
            };

            NativeCall<Dictionary<string, int>>(SDKMethod.setChatRoomAttributes, jo_param, callback, process);
        }

        /**
         * Gets the list of custom chat room attributes based on the attribute key list.
         * 
         * All members in the chat room owner can call this method.
         *
         * This is an asynchronous method.
         *
         * @param roomId        The chat room ID.
         * @param keys			The key list of attributes to get. If you set it as `null` or leave it empty, this method retrieves all custom attributes.
         * @param callback        The completion callback. If this call succeeds, the SDK calls {@link ValueCallBack#OnSuccessValue(Dictionary<string, string>)};
         *                      if this call fails, the SDK calls {@link ValueCallBack#onError(int, String)}.
         */
        public void FetchAttributes(string roomId, List<string> keys = null, ValueCallBack<Dictionary<string, string>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            if (keys != null)
            {
                jo_param.AddWithoutNull("list", JsonObject.JsonArrayFromStringList(keys));
            }

            Process process = (_, jsonNode) =>
            {
                return Dictionary.StringDictionaryFromJsonObject(jsonNode);
            };

            NativeCall<Dictionary<string, string>>(SDKMethod.fetchChatRoomAttributes, jo_param, callback, process);
        }

        /**
         * Removes custom chat room attributes by chat room ID and attribute key list.
         * 
         * All members in the chat room can call this method.
         *
         * This is an asynchronous method.
         *
         * @param roomId        The chat room ID.
         * @param keys			The keys of custom chat room attributes to remove.
         * @forced              Whether to remove attributes with same key set by others.
         * @param callback        The completion callback. If this call succeeds, the SDK calls {@link CallBackResult#OnSuccessResult(Dictionary<string, int>)};
         *                      if this call fails, the SDK calls {@link CallBackResult#onError(int, String)}.
         */
        public void RemoveAttributes(string roomId, List<string> keys, bool forced = false, ValueCallBack<Dictionary<string, int>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("roomId", roomId);
            jo_param.AddWithoutNull("forced", forced);
            jo_param.AddWithoutNull("list", JsonObject.JsonArrayFromStringList(keys));

            Process process = (_, jsonNode) =>
            {
                return Dictionary.SimpleTypeDictionaryFromJsonObject<int>(jsonNode);
            };

            NativeCall<Dictionary<string, int>>(SDKMethod.removeChatRoomAttributes, jo_param, callback, process);
        }

        /**
         * Adds a chat room listener.
         *
         * @param roomManagerDelegate 		The chat room listener to add. It is inherited from {@link IRoomManagerDelegate}.
         * 
         */
        public void AddRoomManagerDelegate(IRoomManagerDelegate roomManagerDelegate)
        {
            if (!delegater.Contains(roomManagerDelegate))
            {
                delegater.Add(roomManagerDelegate);
            }
        }

        /**
         * Removes a chat room listener.
         *
         * @param roomManagerDelegate 		The chat room listener to remove. It is inherited from {@link IRoomManagerDelegate}.
         * 
         */
        public void RemoveRoomManagerDelegate(IRoomManagerDelegate roomManagerDelegate)
        {
            delegater.Remove(roomManagerDelegate);
        }

        internal void ClearDelegates()
        {
            delegater.Clear();
        }


        internal void NativeEventcallback(string method, JSONNode jsonNode)
        {
            if (delegater.Count == 0) return;

            string roomId = jsonNode["roomId"];
            string roomName = jsonNode["name"];

            foreach (IRoomManagerDelegate it in delegater)
            {
                switch (method)
                {
                    case SDKMethod.onDestroyedFromRoom:
                        {
                            it.OnDestroyedFromRoom(roomId, roomName);
                        }
                        break;
                    case SDKMethod.onRemoveFromRoomByOffline:
                        {
                            it.OnRemoveFromRoomByOffline(roomId, roomName);
                        }
                        break;
                    case SDKMethod.onMemberJoinedFromRoom:
                        {
                            string userId = jsonNode["userId"];
                            it.OnMemberJoinedFromRoom(roomId, userId);
                        }
                        break;
                    case SDKMethod.onMemberExitedFromRoom:
                        {
                            string userId = jsonNode["userId"];
                            it.OnMemberExitedFromRoom(roomId, roomName, userId);
                        }
                        break;
                    case SDKMethod.onRemovedFromRoom:
                        {
                            string userId = jsonNode["userId"];
                            it.OnRemovedFromRoom(roomId, roomName, userId);
                        }
                        break;
                    case SDKMethod.onMuteListAddedFromRoom:
                        {
                            List<string> list = List.StringListFromJsonArray(jsonNode["userIds"]);
                            long muteExpire = (long)jsonNode["expireTime"].AsDouble;
                            it.OnMuteListAddedFromRoom(roomId, list, muteExpire);
                        }
                        break;
                    case SDKMethod.onMuteListRemovedFromRoom:
                        {
                            List<string> list = List.StringListFromJsonArray(jsonNode["userIds"]);
                            it.OnMuteListRemovedFromRoom(roomId, list);
                        }
                        break;
                    case SDKMethod.onAdminAddedFromRoom:
                        {
                            string userId = jsonNode["userId"];
                            it.OnAdminAddedFromRoom(roomId, userId);
                        }
                        break;
                    case SDKMethod.onAdminRemovedFromRoom:
                        {
                            string userId = jsonNode["userId"];
                            it.OnAdminRemovedFromRoom(roomId, userId);
                        }
                        break;
                    case SDKMethod.onOwnerChangedFromRoom:
                        {
                            string newOwner = jsonNode["newOwner"];
                            string oldOwner = jsonNode["oldOwner"];
                            it.OnOwnerChangedFromRoom(roomId, newOwner, oldOwner);
                        }
                        break;
                    case SDKMethod.onAnnouncementChangedFromRoom:
                        {
                            string announcement = jsonNode["announcement"];
                            it.OnAnnouncementChangedFromRoom(roomId, announcement);
                        }
                        break;
                    case SDKMethod.onAttributesChangedFromRoom:
                        {
                            Dictionary<string, string> kv = Dictionary.StringDictionaryFromJsonObject(jsonNode["kv"]);
                            string userId = jsonNode["userId"];
                            it.OnChatroomAttributesChanged(roomId, kv, userId);
                        }
                        break;
                    case SDKMethod.onAttributesRemovedFromRoom:
                        {
                            List<string> list = List.StringListFromJsonArray(jsonNode["list"]);
                            string userId = jsonNode["userId"];
                            it.OnChatroomAttributesRemoved(roomId, list, userId);
                        }
                        break;
                    case SDKMethod.onSpecificationChangedFromRoom:
                        {
                            Room room = ModelHelper.CreateWithJsonObject<Room>(jsonNode["room"]);
                            it.OnSpecificationChangedFromRoom(room);
                        }
                        break;
                    case SDKMethod.onAddAllowListMembersFromRoom:
                        {
                            List<string> list = List.StringListFromJsonArray(jsonNode["userIds"]);
                            it.OnAddAllowListMembersFromChatroom(roomId, list);
                        }
                        break;
                    case SDKMethod.onRemoveAllowListMembersFromRoom:
                        {
                            List<string> list = List.StringListFromJsonArray(jsonNode["userIds"]);
                            it.OnRemoveAllowListMembersFromChatroom(roomId, list);
                        }
                        break;
                    case SDKMethod.onAllMemberMuteChangedFromRoom:
                        {
                            bool isAllMuted = jsonNode["isAllMuted"].AsBool;
                            it.OnAllMemberMuteChangedFromChatroom(roomId, isAllMuted);
                        }
                        break;
                }
            }
        }
    }



}
