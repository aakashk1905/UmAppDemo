using System.Collections.Generic;

namespace AgoraChat
{
    /**
	     * The group manager callback interface.
		 *
	     */
    public interface IGroupManagerDelegate
    {
        /**
	     * Occurs when the user receives a group invitation.
		 * 
	     * @param groupId		The group ID.
	     * @param groupName		The group name.
	     * @param inviter		The user ID of the inviter.
	     * @param reason		The reason for invitation.
	     */
        void OnInvitationReceivedFromGroup(
            string groupId, string groupName, string inviter, string reason);

        /**
         * Occurs when the group owner or admin receives a join request from a user.
	     *
	     * @param groupId		The group ID.
	     * @param groupName		The group name.
	     * @param applicant		The ID of the user requesting to join the group.
	     * @param reason		The reason for requesting to join the group.
	     */
        void OnRequestToJoinReceivedFromGroup(
            string groupId, string groupName, string applicant, string reason);

        /**
	     * Occurs when a join request is accepted.
	     *
	     * @param groupId 		The group ID.
	     * @param groupName 	The group name.
	     * @param accepter 		The ID of the user that accepts the join request.
	     */
        void OnRequestToJoinAcceptedFromGroup(
            string groupId, string groupName, string accepter);

        /**
         * Occurs when a join request is declined.
	     *
	     * @param groupId 		The group ID.
	     * @param reason 		The reason for declining the join request.
	     */
        void OnRequestToJoinDeclinedFromGroup(
            string groupId, string reason);

        /**
         * Occurs when a group invitation is accepted.
	     *
	     * @param groupId 		The group ID.
	     * @param invitee 		The user ID of the invitee.
	     */
        void OnInvitationAcceptedFromGroup(string groupId, string invitee);

        /**
         * Occurs when a group invitation is declined.
	     *
	     * @param groupId 		The group ID.
	     * @param invitee		The user ID of the invitee.
	     * @param reason 		The reason for declining the invitation.
	     */
        void OnInvitationDeclinedFromGroup(string groupId, string invitee, string reason);

        /**
         * Occurs when the current user is removed from the group by the group owner or group admin.
	     *
	     * @param groupId 		The group ID.
	     * @param groupName		The group name.
	     */
        void OnUserRemovedFromGroup(string groupId, string groupName);

        /**
         * Occurs when a group is destroyed.
		 * 
         * The SDK will remove the group from the local database and local memory before notifying the app of the group removal via the callback method.
	     *
	     * @param groupId		The group ID.
	     * @param groupName 	The group name.
         */
        void OnDestroyedFromGroup(string groupId, string groupName);

        /**
         * Occurs when the group invitation is accepted automatically.
		 * 
	     * The SDK will add the user to the group before notifying the app of the acceptance of the group invitation via the callback method.
         * 
		 * For settings, see {@link com.hyphenate.chat.EMOptions#setAutoAcceptGroupInvitation(boolean value)}.
	     *
	     * @param groupId			The group ID.
	     * @param inviter			The user ID of the inviter.
	     * @param inviteMessage		The invitation message.
         */
        void OnAutoAcceptInvitationFromGroup(
            string groupId, string inviter, string inviteMessage);

        /**
	     * Occurs when one or more group members are muted.
		 * 
	     * **Note**
		 * 
		 *  A user, when muted, can still see group messages, but cannot send messages in the group. However, a user on the block list can neither see nor send group messages.
	     *
	     * @param groupId		The group ID.
	     * @param mutes 		The member(s) added to the mute list.
	     * @param muteExpire    The mute duration in milliseconds.
	     */
        void OnMuteListAddedFromGroup(string groupId, List<string> mutes, long muteExpire);

        /**
	     * Occurs when one or more group members are unmuted.
		 * 
	     * **Note**
		 * 
		 * A user, when muted, can still see group messages, but cannot send messages in the group. However, a user on the block list can neither see nor send group messages.
	     *
	     * @param groupId		The group ID.
	     * @param mutes 		The member(s) removed from the mute list.
	     */
        void OnMuteListRemovedFromGroup(string groupId, List<string> mutes);

        /**
	     * Occurs when a member is set as an admin.
	     *
	     * @param groupId		The group ID.
	     * @param administrator The user ID of the member that is set as an admin.
	     */
        void OnAdminAddedFromGroup(string groupId, string administrator);

        /**
		 * Occurs when the admin privileges of a member are removed..
		 *
		 * @param groupId 		The group ID.
		 * @param administrator The user ID of the member whose admin privileges are removed.
		 */
        void OnAdminRemovedFromGroup(string groupId, string administrator);

        /**
		 * Occurs when the group ownership is transferred.
		 * 
		 * @param groupId 		The group ID.
		 * @param newOwner 		The new group owner.
		 * @param oldOwner 		The previous group owner.
		 */
        void OnOwnerChangedFromGroup(string groupId, string newOwner, string oldOwner);

        /**
		 * Occurs when a user joins a group.
		 * 
		 * @param groupId  The group ID.
		 * @param member   The ID of the new member.
		 */
        void OnMemberJoinedFromGroup(string groupId, string member);

        /**
		 * Occurs when a member voluntarily leaves the group.
		 * 
		 * @param groupId   The group ID.
		 * @param member  	The user ID of the member who has left the group.
		 */
        void OnMemberExitedFromGroup(string groupId, string member);

        /**
		 * Occurs when the group announcement is updated.
		 * 
		 * @param groupId  The group ID.
		 * @param announcement  The updated announcement content.
		 */
        void OnAnnouncementChangedFromGroup(string groupId, string announcement);

        /**
		 * Occurs when a shared file is added to a group.
		 * 
		 * @param groupId The group ID.
		 * @param sharedFile The new shared file.
		 */
        void OnSharedFileAddedFromGroup(string groupId, GroupSharedFile sharedFile);

        /**
		 * Occurs when a shared file is removed from a group.
		 * 
		 * @param groupId The group ID.
		 * @param fileId  The ID of the removed shared file.
		 */
        void OnSharedFileDeletedFromGroup(string groupId, string fileId);

        /**
		 * Occurs when one or more group members are added to the group allow list.
		 *
		 * @param groupId       The group ID.
		 * @param allowList     The member(s) added to the group allow list.
		 */
        void OnAddAllowListMembersFromGroup(string groupId, List<string> allowList);

        /**
		 * Occurs when one or more members are removed from the group allow list.
		 *
		 * @param groupId       The group ID.
		 * @param allowList     Member(s) removed from the group allow list.
		 */
        void OnRemoveAllowListMembersFromGroup(string groupId, List<string> allowList);

        /**
		 * Occurs when all group members are muted or unmuted.
		 *
		 * @param groupId       The group ID.
		 * @param isAllMuted    Whether all group members are muted or unmuted. 
		 * - `true`: All group members are muted; 
		 * - `false`: All group members are unmuted.
		 */

        void OnAllMemberMuteChangedFromGroup(string groupId, bool isAllMuted);


        /**
		 *  Occurs when the disabled state of group changes.
		 *
		 *  @param groupId          The group ID.
		 *  @param isDisable        Whether the group is disabled.
		 */
        void OnStateChangedFromGroup(string groupId, bool isDisable);

        /**
		 *  Occurs when the group specification(s) is/are changed.
		 *
		 *  Once the group specifications are changed, you need to call {@link GetGroupSpecificationFromServer(string, ValueCallBack<Group>)} to get the latest group information from the server.
		 *
		 *  @param group      The group instance.
		 */
        void OnSpecificationChangedFromGroup(Group group);

        /**
		 *  Occurs when a custom attribute(s) of a group member is/are changed.
		 *
         *  @param groupId           The group ID.
         *  @param userId            The user ID of the group member whose custom attribute(s) is/are changed.
         *  @param attributes        The modified custom attribute(s).
         *  @param from              The user ID of the operator.
		 */
        void OnUpdateMemberAttributesFromGroup(string groupId, string userId, Dictionary<string, string> attributes, string from);

    }
}
