using System;
using System.Collections.Generic;
using AgoraChat.SimpleJSON;

namespace AgoraChat
{
    /**
		* The abstract class for the group manager.
		*
		*/
    public class GroupManager : BaseManager
    {
        internal List<IGroupManagerDelegate> delegater;

        internal GroupManager(NativeListener listener) : base(listener, SDKMethod.groupManager)
        {
            listener.GroupManagerEvent += NativeEventHandle;
            delegater = new List<IGroupManagerDelegate>();
        }

        /**
	     * Requests to join a group.
	     *
	     * This is an asynchronous method.
	     *
	     * @param groupId	The group ID.
	     * @param reason    The reason for requesting to join the group.
	     * @param callback    The callback of application. See {@link CallBack}.
	     */
        public void applyJoinToGroup(string groupId, string reason = "", CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("msg", reason);
            NativeCall(SDKMethod.requestToJoinGroup, jo_param, callback);
        }

        /**
	     * Accepts a group invitation.
	     *
	     * This is an asynchronous method.
	     *
	     * @param groupId 	The group ID.
	     * @param callback    The callback of acceptance. Returns the group instance which the user has accepted the invitation to join. See {@link ValueCallBack}.
	     */
        public void AcceptGroupInvitation(string groupId, ValueCallBack<Group> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);

            Process process = (_, jsonNode) =>
            {
                return ModelHelper.CreateWithJsonObject<Group>(jsonNode);
            };

            NativeCall<Group>(SDKMethod.acceptInvitationFromGroup, jo_param, callback, process);
        }

        /**
	     * Approves a group request.
		 * 
         * Only the group owner or admin can call this method.
	     *
	     * This is an asynchronous method.
	     *
	     * @param groupId  	The group ID.
	     * @param userId 	The ID of the user who sends the request to join the group.
	     * @param callback    The callback of approval. See {@link CallBack}.
	     */
        public void AcceptGroupJoinApplication(string groupId, string userId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userId", userId);
            NativeCall(SDKMethod.acceptJoinApplication, jo_param, callback);
        }

        /**
         * Adds a group admin.
	     * Only the group owner, not the group admin, can call this method.
	     *
	     * This is an asynchronous method.
         *
	     * @param groupId	The group ID.
	     * @param memberId	The new admin ID.
	     * @param callback    The callback of addition. See {@link CallBack}.
	     */
        public void AddGroupAdmin(string groupId, string memberId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userId", memberId);
            NativeCall(SDKMethod.addAdmin, jo_param, callback);
        }

        /**
	     * Adds users to the group.
		 * 
	     * Only the group owner or admin can call this method.
	     *
	     * This is an asynchronous method.
	     *
	     * @param groupId		The group ID.
	     * @param newmembers	The list of new members to add.
	     * @param callback        The operation callback. See {@link CallBack}.
	     */
        public void AddGroupMembers(string groupId, List<string> newmembers, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(newmembers));
            NativeCall(SDKMethod.addMembers, jo_param, callback);
        }

        /**
	     * Adds members to the allow list.
		 * 
	     * Only the group owner or admin can call this method.
	     *
	     * This is an asynchronous method.
	     *
	     * @param groupId 	The group ID.
	     * @param members 	The members to be added to the allow list.
	     * @param callback    The operation callback. See {@link CallBack}.
	     */
        public void AddGroupAllowList(string groupId, List<string> members, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            NativeCall(SDKMethod.addAllowList, jo_param, callback);
        }

        /**
	     * Blocks group messages.
		 * 
         * The user that blocks group messages is still a group member. They just cannot receive group messages.
	     *
	     * This is an asynchronous method.
	     *
	     * @param groupId	The group ID.
	     * @param callback    The operation callback. See {@link CallBack}.
	     */
        public void BlockGroup(string groupId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            NativeCall(SDKMethod.blockGroup, jo_param, callback);
        }

        /**
         * Adds the user to the group block list.
		 * 
	     * After this method call succeeds, the user is first removed from the chat group, and then added to the group block list. Users on the chat group block list cannot send or receive group messages, nor can they re-join the chat group.
	     * 
		 * Only the group owner or admin can call this method.
	     *
	     * This is an asynchronous method.
	     *
	     * @param groupId	The group ID.
	     * @param members 	The users to be added to the block list.
	     * @param callback    The operation callback. See {@link CallBack}.
	     */
        public void BlockGroupMembers(string groupId, List<string> members, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            NativeCall(SDKMethod.blockMembers, jo_param, callback);
        }

        /**
	     * Changes the group description.
		 * 
	     * Only the group owner or admin can call this method.
	     *
	     * This is an asynchronous method.
	     *
	     * @param groupId 	The group ID.
	     * @param desc 	    The new group description.
         * @param callback    The operation callback. See {@link CallBack}.
	     */
        public void ChangeGroupDescription(string groupId, string desc, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("desc", desc);
            NativeCall(SDKMethod.updateDescription, jo_param, callback);
        }

        /**
	     * Changes the group name.
		 * 
	     * Only the group owner or admin can call this method.
	     *
	     * This is an asynchronous method.
	     *
	     * @param groupId 	The ID of group whose name is to be changed.
	     * @param name 	    The new group name.
	     * @param callback    The operation callback. See {@link CallBack}.
	     */
        public void ChangeGroupName(string groupId, string name, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("name", name);
            NativeCall(SDKMethod.updateGroupSubject, jo_param, callback);
        }

        /**
	     * Transfers the group ownership.
		 * 
	     * Only the group owner can call this method.
	     *
	     * This is an asynchronous method.
         *
	     * @param groupId	The group ID.
	     * @param newOwner	The user ID of the new owner.
	     * @param callback    The operation callback. See {@link CallBack}.
	     */
        public void ChangeGroupOwner(string groupId, string newOwner, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userId", newOwner);
            NativeCall(SDKMethod.updateGroupOwner, jo_param, callback);
        }

        /**
	     * Gets whether the current user is on the allow list of the group.
	     *
	     * This is an asynchronous method.
	     *
	     * @param groupId 	The group ID.
	     * @param callback    The operation callback. See {@link CallBack}.
	     */
        public void CheckIfInGroupAllowList(string groupId, ValueCallBack<bool> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            Process process = (_, jsonNode) =>
            {
                return jsonNode["ret"].IsBoolean ? jsonNode["ret"].AsBool : false;
            };
            NativeCall<bool>(SDKMethod.isMemberInAllowListFromServer, jo_param, callback, process);
        }

        /**
	     * Creates a group instance.
		 * 
         * After the group is created, the data in the memory and database will be updated and multiple devices will receive the notification event and update the group to the memory and database. 
		 * 
		 * You can set {@link IMultiDeviceDelegate} to listen for the event. 
		 * 
		 * If an event occurs, the callback function {@link onGroupMultiDevicesEvent((MultiDevicesOperation, string, List<string>)} will be triggered.
	     * 
	     * This is an asynchronous method.
	     *
	     * @param groupName     The group name. It is optional. Pass `null` if you do not want to set this parameter.
	     * @param options       The options for creating a group. They are optional and cannot be `null`. See {@link GroupOptions}.
	     *                      The options are as follows:
	     *                      - The maximum number of members allowed in the group. The default value is 200.
	     *                      - The group style. See {@link GroupStyle}. 
	     *                      - Whether to ask for permission when inviting a user to join the group. The default value is `false`, indicating that invitees are automaticall added to the group without their permission.
	     *                      - The extension of group details.
         * @param desc          The group description. It is optional. Pass `null` if you do not want to set this parameter.
	     * @param inviteMembers The group member array. The group owner ID is optional. This parameter cannot be `null`.
	     * @param inviteReason  The group joining invitation. It is optional. Pass `null` if you do not want to set this parameter.
         * @param callback        The operation callback. See {@link CallBack}.
         */
        public void CreateGroup(string groupName, GroupOptions options, string desc = null, List<string> inviteMembers = null, string inviteReason = null, ValueCallBack<Group> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("name", groupName);
            if (null != options)
            {
                jo_param.AddWithoutNull("options", options.ToJsonObject());
            }
            jo_param.AddWithoutNull("desc", desc);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(inviteMembers));
            jo_param.AddWithoutNull("msg", inviteReason);

            Process process = (_, jsonNode) =>
            {
                return ModelHelper.CreateWithJsonObject<Group>(jsonNode.AsObject);
            };


            NativeCall<Group>(SDKMethod.createGroup, jo_param, callback, process);
        }

        /**
	     * Declines a group invitation.
	     *
	     * This is an asynchronous method.
	     *
	     * @param groupId 	The group ID.
	     * @param reason	The reason for declining the group invitation.
	     * @param callback    The operation callback. See {@link CallBack}.
	     */
        public void DeclineGroupInvitation(string groupId, string reason = null, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("msg", reason);
            NativeCall(SDKMethod.declineInvitationFromGroup, jo_param, callback);
        }

        /**
		 * Declines a group request.
		 * 
		 * Only the group owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId  	The group ID.
		 * @param userId 	The ID of the user who sends the request to join the group.
		 * @param reason   	The reason for declining the group request.
		 * @param callback    The operation callback. See {@link CallBack}.
		 */
        public void DeclineGroupJoinApplication(string groupId, string userId, string reason = null, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userId", userId);
            jo_param.AddWithoutNull("msg", reason);
            NativeCall(SDKMethod.declineJoinApplication, jo_param, callback);
        }

        /**
		 * Destroys the group instance.
		 * 
		 * Only the group owner can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId	The group ID.
		 * @param callback    The operation callback. See {@link CallBack}.
		 */
        public void DestroyGroup(string groupId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            NativeCall(SDKMethod.destroyGroup, jo_param, callback);
        }

        /**
		 * Downloads the shared file of the group.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId   The group ID.
		 * @param fileId    The ID of the shared file.
		 * @param savePath  The path to save the downloaded file.
		 * @param callback    The operation callback. See {@link CallBack}.
		 */
        public void DownloadGroupSharedFile(string groupId, string fileId, string savePath, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("fileId", fileId);
            jo_param.AddWithoutNull("savePath", savePath);
            NativeCall(SDKMethod.downloadGroupSharedFile, jo_param, callback);
        }

        /**
		 * Gets the group announcement from the server.
		 *
		 * Group members can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId	The group ID.
		 * @param callback    The operation callback. See {@link ValueCallBack}.
		 */
        public void GetGroupAnnouncementFromServer(string groupId, ValueCallBack<string> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);

            Process process = (_, jsonNode) =>
            {
                return jsonNode["ret"].IsString ? jsonNode["ret"].Value : null;
            };

            NativeCall<string>(SDKMethod.getGroupAnnouncementFromServer, jo_param, callback, process);
        }

        /**
		 * Gets the group block list from the server with pagination.
		 * 
		 * Only the group owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId               The group ID.
		 * @param pageNum            	The page number, starting from 1.
		 * @param pageSize              The number of members on the block list that you expect to get on each page.
		 * @param callback				The operation callback. If success, the obtained block list will be returned; otherwise, an error will be returned. See {@link ValueCallBack}.
		 */
        public void GetGroupBlockListFromServer(string groupId, int pageNum = 1, int pageSize = 200, ValueCallBack<List<string>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("pageNum", pageNum);
            jo_param.AddWithoutNull("pageSize", pageSize);

            Process process = (_, jsonNode) =>
            {
                return List.StringListFromJsonArray(jsonNode);
            };

            NativeCall<List<string>>(SDKMethod.getGroupBlockListFromServer, jo_param, callback, process);
        }

        /**
		 * Gets the shared files of the group from the server.
         *
         * For a large but unknown quantity of data, the server will return data with pagination as specified by `pageSize` and `pageNum`.
		 * 
		 * This is an asynchronous method.
		 *
		 * @param groupId 	The group ID.
		 * @param pageNum 	The page number, starting from 1.
		 *                  For the last page, the actual number of returned shared files is less than the value of `pageSize`.     
		 * @param pageSize 	The number of shared files that you expect to get on each page.
		 * @param callback	The operation callback. If success, the SDK returns the list of shared files; otherwise, an error will be returned. See {@link ValueCallBack}.
		 *
		 */
        public void GetGroupFileListFromServer(string groupId, int pageNum = 1, int pageSize = 200, ValueCallBack<List<GroupSharedFile>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("pageNum", pageNum);
            jo_param.AddWithoutNull("pageSize", pageSize);

            Process process = (_, jsonNode) =>
            {
                return List.BaseModelListFromJsonArray<GroupSharedFile>(jsonNode);
            };

            NativeCall<List<GroupSharedFile>>(SDKMethod.getGroupFileListFromServer, jo_param, callback, process);
        }

        /**
		 * Gets the group member list from the server.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId 	The group ID.
		 * @param pageSize 	The number of members that you expect to get on each page.
		   @param cursor    The position from which to start getting data. At the first method call, if you set `cursor` as `null`, the SDK gets the data in the reverse chronological order of when users joined the group.
		 * @param callback	The operation callback. If success, the SDK returns the obtained group member list and the cursor for the next query; otherwise, an error will be returned. See {@link ValueCallBack}.
		 *
		 */
        public void GetGroupMemberListFromServer(string groupId, int pageSize = 200, string cursor = "", ValueCallBack<CursorResult<string>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
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

            NativeCall<CursorResult<string>>(SDKMethod.getGroupMemberListFromServer, jo_param, callback, process);
        }

        /**
		 * Gets the mute list of the group from the server.
		 * 
		 * Only the group owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId	The group ID.
		 * @param pageNum	The page number, starting from 1.
		 * @param pageSize	The number of muted members that you expect to get on each page.
		 * @param callback	The operation callback. If success, the SDK returns the obtained mute list; otherwise, an error will be returned. See {@link ValueCallBack}.
		 *
		 */
        public void GetGroupMuteListFromServer(string groupId, int pageNum = 1, int pageSize = 200, ValueCallBack<Dictionary<string, long>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("pageNum", pageNum);
            jo_param.AddWithoutNull("pageSize", pageSize);

            Process process = (_, jsonNode) =>
            {
                return Dictionary.SimpleTypeDictionaryFromJsonObject<long>(jsonNode);
            };

            NativeCall<Dictionary<string, long>>(SDKMethod.getGroupMuteListFromServer, jo_param, callback, process);
        }

        /**
		 * Gets group details.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId	The group ID.
		 * @param callback	The operation callback. If success, the SDK returns the group instance; otherwise, an error will be returned. See {@link ValueCallBack}.
		 *
		 */
        public void GetGroupSpecificationFromServer(string groupId, ValueCallBack<Group> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);

            Process process = (_, jsonNode) =>
            {
                return ModelHelper.CreateWithJsonObject<Group>(jsonNode);
            };

            NativeCall<Group>(SDKMethod.getGroupSpecificationFromServer, jo_param, callback, process);
        }

        /**
		 * Gets the allow list of the group from the server.
		 * 
		 * Only the group owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId 	The group ID.
		 * @param callback	The operation callback. If success, the SDK returns the obtained allow list; otherwise, an error will be returned. See {@link ValueCallBack}.
		 */
        public void GetGroupAllowListFromServer(string groupId, ValueCallBack<List<string>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);

            Process process = (_, jsonNode) =>
            {
                return List.StringListFromJsonArray(jsonNode);
            };

            NativeCall<List<string>>(SDKMethod.getGroupAllowListFromServer, jo_param, callback, process);
        }

        /**
		 * Gets the group instance from the memory by group ID.
		 *
		 * @param groupId	The group ID.
		 * @return 			The group instance. Returns `null` if the group does not exist.
		 */
        public Group GetGroupWithId(string groupId)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            JSONNode jn = NativeGet(SDKMethod.getGroupWithId, jo_param).GetReturnJsonNode();
            return ModelHelper.CreateWithJsonObject<Group>(jn);
        }

        /**
		 * Gets the list of groups that the user has joined.
		 * 
		 * This method gets the groups from the local memory and database.
		 * 
		 * This is an asynchronous method.
		 *
		 * @return 			The group list. 
		 */
        public List<Group> GetJoinedGroups()
        {
            JSONObject jo_param = new JSONObject();
            JSONNode jn = NativeGet(SDKMethod.getJoinedGroups).GetReturnJsonNode();
            return List.BaseModelListFromJsonArray<Group>(jn);
        }

        /**
		 * Gets the list of groups with pagination.
		 * 
		 * This method gets a group list from the server, which does not contain member information. 
		 * 
		 * This is an asynchronous method and blocks the current thread.
		 *
		 * @param pageNum 		The page number, starting from 0.
		 * @param pageSize		The number of groups that you expect to get on each page. The value range is [1,20].
		 * @param needAffiliations Get member count or not.
		 * @param needRole 		Get role or current user in joined groups.
		 * @param callback		The operation callback. If success, the SDK returns the obtained group list; otherwise, an error will be returned. See {@link ValueCallBack}. 
		 */
        public void FetchJoinedGroupsFromServer(int pageNum = 0, int pageSize = 20, bool needAffiliations = false, bool needRole = false, ValueCallBack<List<Group>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("pageNum", pageNum);
            jo_param.AddWithoutNull("pageSize", pageSize);
            jo_param.AddWithoutNull("needAffiliations", needAffiliations);
            jo_param.AddWithoutNull("needRole", needRole);

            Process process = (_, jsonNode) =>
            {
                return List.BaseModelListFromJsonArray<Group>(jsonNode);
            };

            NativeCall<List<Group>>(SDKMethod.getJoinedGroupsFromServer, jo_param, callback, process);
        }

        /**
        * Gets the list of groups with pagination.
        *
        * This method gets a group list from the server, which does not contain member information.
        *
        * This is an asynchronous method and blocks the current thread.
        *
        * @param pageNum 		The page number, starting from 0.
        * @param pageSize		The number of groups that you expect to get on each page. Default num is 20.
        * @param callback		The operation callback. If success, the SDK returns the obtained group list; otherwise, an error will be returned. See {@link ValueCallBack}. 
        */
        [Obsolete]
        public void FetchJoinedGroupsFromServer(int pageNum = 0, int pageSize = 20, ValueCallBack<List<Group>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("pageNum", pageNum);
            jo_param.AddWithoutNull("pageSize", pageSize);

            Process process = (_, jsonNode) =>
            {
                return List.BaseModelListFromJsonArray<Group>(jsonNode);
            };

            NativeCall<List<Group>>(SDKMethod.getJoinedGroupsFromServerSimple, jo_param, callback, process);
        }

        /**
		 * Gets public groups from the server with pagination.
		 *
		 * This is an asynchronous method.
		 *
		 * @param pageSize  	The number of public groups that you expect to get on each page.
		 * @param cursor    	The position from which to start getting data. During the first call to this method, if `null` is passed to `cursor`, the SDK will get data in the reverse chronological order.
		 * @param callback		The operation callback. If success, the SDK returns the obtained public group list and the cursor used for the next query; otherwise, an error will be returned. See {@link ValueCallBack}.
		 */
        public void FetchPublicGroupsFromServer(int pageSize = 200, string cursor = "", ValueCallBack<CursorResult<GroupInfo>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("pageSize", pageSize);
            jo_param.AddWithoutNull("cursor", cursor);

            Process process = (_, jsonNode) =>
            {
                CursorResult<GroupInfo> cursor_msg = new CursorResult<GroupInfo>(_, (jn) =>
                {
                    return ModelHelper.CreateWithJsonObject<GroupInfo>(jn);
                });

                cursor_msg.FromJsonObject(jsonNode.AsObject);
                return cursor_msg;

            };

            NativeCall<CursorResult<GroupInfo>>(SDKMethod.getPublicGroupsFromServer, jo_param, callback, process);
        }

        /**
		 * Joins a public group.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId	  	The ID of the public group.
		 * @param callback		The operation callback. See {@link CallBack}.
		 */
        public void JoinPublicGroup(string groupId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            NativeCall(SDKMethod.joinPublicGroup, jo_param, callback);
        }

        /**
		 * Leaves a group.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId	The group ID.
		 * @param callback    The operation callback. See {@link CallBack}.
		 */
        public void LeaveGroup(string groupId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            NativeCall(SDKMethod.leaveGroup, jo_param, callback);
        }

        /**
		 * Mutes all members in the group.
		 * 
		 * Only the group owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId		The group ID.
		 * @param callback		The operation callback. See {@link CallBack}.
		 */
        public void MuteGroupAllMembers(string groupId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            NativeCall(SDKMethod.muteAllMembers, jo_param, callback);
        }

        /**
		 * Mutes the specified group members.
		 * Only the group owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId		The group ID.
		 * @param members 		The list of members to be muted.
		 * @param muteMilliseconds Muted time duration in millisecond, -1 stand for eternity.
		 * @param callback		The operation callback. See {@link CallBack}.
		 */
        public void MuteGroupMembers(string groupId, List<string> members, long muteMilliseconds = -1, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            jo_param.AddWithoutNull("expireTime", muteMilliseconds);
            NativeCall(SDKMethod.muteMembers, jo_param, callback);
        }

        /**
		 * Removes a group admin.
		 * Only the group owner can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId	The group ID.
		 * @param memberId	The ID of the admin to remove.
		 * @param callback	The operation callback. See {@link CallBack}.
		 */
        public void RemoveGroupAdmin(string groupId, string memberId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userId", memberId);
            NativeCall(SDKMethod.removeAdmin, jo_param, callback);
        }

        /**
		 * Removes the shared file of the group.
		 * 
		 * Group members can only delete the shared files uploaded by themselves. The group owner and admins can delete all the shared files.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId 	The group ID.
		 * @param fileId 	The ID of the shared file.
		 * @param callback	The operation callback. See {@link CallBack}.
		 */
        public void DeleteGroupSharedFile(string groupId, string fileId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("fileId", fileId);
            NativeCall(SDKMethod.removeGroupSharedFile, jo_param, callback);
        }

        /**
		 * Removes members from the group.
		 * 
		 * Only the group owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId	The group ID.
		 * @param members   The members to remove.
		 * @param callback	The operation callback. See {@link CallBack}.
		 */
        public void DeleteGroupMembers(string groupId, List<string> members, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            NativeCall(SDKMethod.removeMembers, jo_param, callback);
        }

        /**
		 * Removes members from the group allow list.
		 * 
		 * Only the group owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId 	The group ID.
		 * @param members 	The list of members to be removed from the group allow list.
		 * @param callback	The operation callback. See {@link CallBack}.
		 */
        public void RemoveGroupAllowList(string groupId, List<string> members, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            NativeCall(SDKMethod.removeAllowList, jo_param, callback);
        }

        /**
		 * Unblocks group messages.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId	The group ID
		 * @param callback	The operation callback. See {@link CallBack}.
		 */
        public void UnBlockGroup(string groupId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            NativeCall(SDKMethod.unblockGroup, jo_param, callback);
        }

        /**
		 * Removes users from the group block list.
		 * 
		 * Only the group owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId	The group ID.
		 * @param members	The list of users to be removed from the block list.
		 * @param callback	The operation callback. See {@link CallBack}.
		 */
        public void UnBlockGroupMembers(string groupId, List<string> members, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            NativeCall(SDKMethod.unblockMembers, jo_param, callback);
        }

        /**
		 * Unmutes all members in the group.
		 * 
		 * Only the group owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId	The group ID.
		 * @param callback	The operation callback. See {@link CallBack}.
		 */
        public void UnMuteGroupAllMembers(string groupId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            NativeCall(SDKMethod.unMuteAllMembers, jo_param, callback);
        }

        /**
		 * Unmutes the specified group members.
		 * 
		 * Only the group owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId	The group ID.
		 * @param members	The list of members to be unmuted.
		 * @param callback	The operation callback. See {@link CallBack}.
		 */
        public void UnMuteGroupMembers(string groupId, List<string> members, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(members));
            NativeCall(SDKMethod.unMuteMembers, jo_param, callback);
        }

        /**
		 * Updates the group announcement.
		 * 
		 * Only the group owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId 		The group ID.
		 * @param announcement 	The group announcement.
		 * @param callback	The operation callback. See {@link CallBack}.
		 */
        public void UpdateGroupAnnouncement(string groupId, string announcement, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("announcement", announcement);
            NativeCall(SDKMethod.updateGroupAnnouncement, jo_param, callback);
        }

        /**
		 * Updates the group extension field.
		 * 
		 * Only the group owner or admin can call this method.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId       The group ID.
		 * @param ext     		The group extension field.
		 * @param callback		The operation callback. See {@link CallBack}.
		 */
        public void UpdateGroupExt(string groupId, string ext, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("ext", ext);
            NativeCall(SDKMethod.updateGroupExt, jo_param, callback);
        }

        /**
		 * Uploads the shared file to the group.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId 	The group ID.
		 * @param filePath 	The local path of the shared file.
		 * @param callback	The operation callback. See {@link CallBack}.
		 */
        public void UploadGroupSharedFile(string groupId, string filePath, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("filePath", filePath);
            NativeCall(SDKMethod.uploadGroupSharedFile, jo_param, callback);
        }

        /**
        * Sets custom attributes of a group member.
        *
        * This is an asynchronous method.
        *
        * @param groupId 	The group ID.
        * @param userId 	The user ID of the group member for whom the custom attributes are set.
        * @param callback	The operation callback. See {@link CallBack}.
        */
        public void SetMemberAttributes(string groupId, string userId, Dictionary<string, string> attrs, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userId", userId);
            jo_param.AddWithoutNull("attrs", JsonObject.JsonObjectFromDictionary(attrs));
            NativeCall(SDKMethod.setMemberAttributes, jo_param, callback);
        }

        /**
		 * Fetch group members custom attributes.
		 *
		 * This is an asynchronous method.
		 *
		 * @param groupId 	    The group ID.
		 * @param userIds    	User name ID list.
		 * @param attrs         The custom attribute list.
		 * @param callback	    The operation callback. See {@link ValueCallBack}.
		 */
        public void FetchMemberAttributes(string groupId, List<string> userIds, List<string> attrs, ValueCallBack<Dictionary<string, Dictionary<string, string>>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("userIds", JsonObject.JsonArrayFromStringList(userIds));
            jo_param.AddWithoutNull("attrs", JsonObject.JsonArrayFromStringList(attrs));

            Process process = (_, jsonNode) =>
            {
                return Dictionary.NestedStringDictionaryFromJsonObject(jsonNode);
            };

            NativeCall<Dictionary<string, Dictionary<string, string>>>(SDKMethod.fetchMemberAttributes, jo_param, callback, process);
        }

        /**
		 * Adds a group manager listener.
		 *
		 * @param groupManagerDelegate 		The group manager listener to add. It is inherited from {@link IGroupManagerDelegate}.
		 * 
		 */
        public void AddGroupManagerDelegate(IGroupManagerDelegate groupManagerDelegate)
        {
            if (!delegater.Contains(groupManagerDelegate))
            {
                delegater.Add(groupManagerDelegate);
            }
        }

        /**
		 * Removes a group manager listener.
		 *
		 * @param groupManagerDelegate 		The group manager listener to remove. It is inherited from {@link IGroupManagerDelegate}.
		 * 
		 */
        public void RemoveGroupManagerDelegate(IGroupManagerDelegate groupManagerDelegate)
        {
            delegater.Remove(groupManagerDelegate);
        }

        internal void ClearDelegates()
        {
            delegater.Clear();
        }

        internal void NativeEventHandle(string method, JSONNode jsonNode)
        {
            if (delegater.Count == 0) return;

            string groupId = jsonNode["groupId"];
            string groupName = jsonNode["name"];

            foreach (IGroupManagerDelegate it in delegater)
            {
                switch (method)
                {
                    case SDKMethod.onInvitationReceivedFromGroup:
                        {
                            string userId = jsonNode["userId"];
                            string reason = jsonNode["msg"];
                            it.OnInvitationReceivedFromGroup(groupId, groupName, userId, reason);
                        }
                        break;
                    case SDKMethod.onRequestToJoinReceivedFromGroup:
                        {
                            string reason = jsonNode["msg"];
                            string userId = jsonNode["userId"];
                            it.OnRequestToJoinReceivedFromGroup(groupId, groupName, userId, reason);
                        }
                        break;
                    case SDKMethod.onRequestToJoinAcceptedFromGroup:
                        {
                            string userId = jsonNode["userId"];
                            it.OnRequestToJoinAcceptedFromGroup(groupId, groupName, userId);
                        }
                        break;
                    case SDKMethod.onRequestToJoinDeclinedFromGroup:
                        {
                            string reason = jsonNode["msg"];
                            //string userId = jsonNode["userId"];
                            it.OnRequestToJoinDeclinedFromGroup(groupId, reason);
                        }
                        break;
                    case SDKMethod.onInvitationAcceptedFromGroup:
                        {
                            string userId = jsonNode["userId"];
                            it.OnInvitationAcceptedFromGroup(groupId, userId);
                        }
                        break;
                    case SDKMethod.onInvitationDeclinedFromGroup:
                        {
                            string reason = jsonNode["msg"];
                            string userId = jsonNode["userId"];
                            it.OnInvitationDeclinedFromGroup(groupId, userId, reason);
                        }
                        break;
                    case SDKMethod.onUserRemovedFromGroup:
                        {
                            it.OnUserRemovedFromGroup(groupId, groupName);
                        }
                        break;
                    case SDKMethod.onDestroyedFromGroup:
                        {
                            it.OnDestroyedFromGroup(groupId, groupName);
                        }
                        break;
                    case SDKMethod.onAutoAcceptInvitationFromGroup:
                        {
                            string userId = jsonNode["userId"];
                            string inviteMsg = jsonNode["msg"];
                            it.OnAutoAcceptInvitationFromGroup(groupId, userId, inviteMsg);
                        }
                        break;
                    case SDKMethod.onMuteListAddedFromGroup:
                        {
                            List<string> list = List.StringListFromJsonArray(jsonNode["userIds"]);
                            long muteExpire = (long)jsonNode["expireTime"].AsDouble;
                            it.OnMuteListAddedFromGroup(groupId, list, muteExpire);
                        }
                        break;
                    case SDKMethod.onMuteListRemovedFromGroup:
                        {
                            List<string> list = List.StringListFromJsonArray(jsonNode["userIds"]);
                            it.OnMuteListRemovedFromGroup(groupId, list);
                        }
                        break;
                    case SDKMethod.onAdminAddedFromGroup:
                        {
                            string userId = jsonNode["userId"];
                            it.OnAdminAddedFromGroup(groupId, userId);
                        }
                        break;
                    case SDKMethod.onAdminRemovedFromGroup:
                        {
                            string userId = jsonNode["userId"];
                            it.OnAdminRemovedFromGroup(groupId, userId);
                        }
                        break;
                    case SDKMethod.onOwnerChangedFromGroup:
                        {
                            string newOwner = jsonNode["newOwner"];
                            string oldOwner = jsonNode["oldOwner"];
                            it.OnOwnerChangedFromGroup(groupId, newOwner, oldOwner);
                        }
                        break;
                    case SDKMethod.onMemberJoinedFromGroup:
                        {
                            string userId = jsonNode["userId"];
                            it.OnMemberJoinedFromGroup(groupId, userId);
                        }
                        break;
                    case SDKMethod.onMemberExitedFromGroup:
                        {
                            string userId = jsonNode["userId"];
                            it.OnMemberExitedFromGroup(groupId, userId);
                        }
                        break;
                    case SDKMethod.onAnnouncementChangedFromGroup:
                        {
                            string announcement = jsonNode["announcement"];
                            it.OnAnnouncementChangedFromGroup(groupId, announcement);
                        }
                        break;
                    case SDKMethod.onSharedFileAddedFromGroup:
                        {
                            GroupSharedFile file = ModelHelper.CreateWithJsonObject<GroupSharedFile>(jsonNode["file"]);
                            it.OnSharedFileAddedFromGroup(groupId, file);
                        }
                        break;
                    case SDKMethod.onSharedFileDeletedFromGroup:
                        {
                            string fileId = jsonNode["fileId"];
                            it.OnSharedFileDeletedFromGroup(groupId, fileId);
                        }
                        break;
                    case SDKMethod.onAddAllowListMembersFromGroup:
                        {
                            List<string> list = List.StringListFromJsonArray(jsonNode["userIds"]);
                            it.OnAddAllowListMembersFromGroup(groupId, list);
                        }
                        break;
                    case SDKMethod.onRemoveAllowListMembersFromGroup:
                        {
                            List<string> list = List.StringListFromJsonArray(jsonNode["userIds"]);
                            it.OnRemoveAllowListMembersFromGroup(groupId, list);
                        }
                        break;
                    case SDKMethod.onAllMemberMuteChangedFromGroup:
                        {
                            bool isMuteAll = jsonNode["isMuteAll"];
                            it.OnAllMemberMuteChangedFromGroup(groupId, isMuteAll);
                        }
                        break;
                    case SDKMethod.onStateChangedFromGroup:
                        {
                            bool isDisable = jsonNode["isDisabled"].AsBool;
                            it.OnStateChangedFromGroup(groupId, isDisable);
                        }
                        break;
                    case SDKMethod.onSpecificationChangedFromGroup:
                        {
                            Group group = ModelHelper.CreateWithJsonObject<Group>(jsonNode["group"]);
                            it.OnSpecificationChangedFromGroup(group);
                        }
                        break;
                    case SDKMethod.onUpdateMemberAttributesFromGroup:
                        {
                            string userId = jsonNode["userId"];
                            string from = jsonNode["from"];
                            Dictionary<string, string> attributes = Dictionary.StringDictionaryFromJsonObject(jsonNode["attrs"]);
                            it.OnUpdateMemberAttributesFromGroup(groupId, userId, attributes, from);
                        }
                        break;
                }
            }
        }
    }

}
