using System.Collections.Generic;

namespace AgoraChat
{
    /**
         * 
         * The chat manager callback interface.
         */
    public interface IRoomManagerDelegate
    {
        /**
         * Occurs when the chat room is destroyed.
         * 
         * @param roomId        The chat room ID.
         * @param roomName      The chat room name.
         */
        void OnDestroyedFromRoom(string roomId, string roomName);


        /**
        * Occurs when a user joins the chat room.
        * 
        * @param roomId        The chat room ID.
        * @param participant   The user ID of the new member.
        */
        void OnMemberJoinedFromRoom(string roomId, string participant);

        /**
         * Occurs when a member voluntarily leaves the chat room.
         * 
         * @param roomId       The chat room ID.
         * @param roomName     The name of the chat room.
         * @param participant  The user ID of the member who leaves the chat room.
         */
        void OnMemberExitedFromRoom(string roomId, string roomName, string participant);


        /**
         * Occurs when a member is removed from a chat room.
         *
         * @param roomId        The chat room ID.
         * @param roomName      The name of the chat room.
         * @param participant   The user ID of the member that is removed from a chat room.
         */
        void OnRemovedFromRoom(string roomId, string roomName, string participant);



        /**
         * Occurs when a member is removed from a chat room because he or she gets offline.
         *
         * @param roomId        The chat room ID.
         * @param roomName      The name of the chat room.
         */
        void OnRemoveFromRoomByOffline(string roomId, string roomName);
        /**
         * Occurs when a chat room member is added to the mute list.
         *
         * The muted members cannot send messages during the mute duration.
         *
         * @param chatRoomId    The chat room ID.
         * @param mutes         The user ID(s) of the muted member(s).
         * @param expireTime    The mute duration in milliseconds.
         */
        void OnMuteListAddedFromRoom(string roomId, List<string> mutes, long expireTime);

        /**
         * Occurs when a chat room member is removed from the mute list.
         *
         * @param chatRoomId    The chat room ID.
         * @param mutes         The user ID(s) of member(s) that is removed from the mute list of the chat room.
         */
        void OnMuteListRemovedFromRoom(string roomId, List<string> mutes);

        /**
         * Occurs when a regular chat room member is set as an admin.
         *
         * @param  roomId       The chat room ID.
         * @param  admin        The user ID of the member who is set as an admin.
         */
        void OnAdminAddedFromRoom(string roomId, string admin);

        /**
         * Occurs when an admin is demoted to a regular member.
         *
         * @param  roomId       The chat room ID.
         * @param  admin        The user ID of the admin whose administrative permissions are removed.
         */
        void OnAdminRemovedFromRoom(string roomId, string admin);

        /**
         * Occurs when the chat room ownership is transferred.
         *
         * @param roomId        The chat room ID.
         * @param newOwner      The user ID of the new chat room owner.
         * @param oldOwner      The user ID of the previous chat room owner.
         */
        void OnOwnerChangedFromRoom(string roomId, string newOwner, string oldOwner);

        /**
         * Occurs when the chat room announcement is updated.
         *
         * @param roomId        The chat room ID.
         * @param announcement  The updated announcement.
         */
        void OnAnnouncementChangedFromRoom(string roomId, string announcement);

        /**
         * The custom chat room attribute(s) is/are updated.
         *
         * All chat room members receive this event.
         *
         * @param chatRoomId   The chat room ID.
         * @param kv           The map of custom chat room attributes that are updated.
         * @param from         The user ID of the operator.
         */
        void OnChatroomAttributesChanged(string roomId, Dictionary<string, string> kv, string from);

        /**
         * The custom chat room attribute(s) is/are removed.
         *
         * All chat room members receive this event.
         *
         * @param chatRoomId   The chat room ID.
         * @param keys         The list of keys of custom chat room attributes that are removed.
         * @param from         The user ID of the operator.
         */
        void OnChatroomAttributesRemoved(string roomId, List<string> keys, string from);

        /**
        * Occurs when the chat room specifications are changed.
        *
        * All chat room members receive this event.
        *
        * @param room The chat room instance.
        */
        void OnSpecificationChangedFromRoom(Room room);

        /**
         * Occurs when the chat room member(s) is/are added to the allow list.
         *
         * The members added to the allow list receive this event.
         *
         * @param roomId The chat room ID.
         * @param members  The member(s) added to the allow list.
         */
        void OnAddAllowListMembersFromChatroom(string roomId, List<string> members);

        /**
         * Occurs when the chat room member(s) is/are removed from the allow list.
         *
         * The members that are removed from the allow list receive this event.
         *
         * @param roomId The chat room ID.
         * @param members  The member(s) removed from the allow list.
         */
        void OnRemoveAllowListMembersFromChatroom(string roomId, List<string> members);

        /**
         * Occurs when all members in the chat room are muted or unmuted.
         *
         * All chat room members receive this event.
         *
         * @param roomId The chat room ID.
         * @param isAllMuted    Whether all chat room members are muted or unmuted.
         */
        void OnAllMemberMuteChangedFromChatroom(string roomId, bool isAllMuted);
    }
}
