namespace AgoraChat
{
    /**
     * The contact manager callback interface.
     * 
     */
    public interface IContactManagerDelegate
    {
        /**
         * Occurs when a user is added as a contact.
         *
         * @param userId   The user ID of the new contact.
         */
        void OnContactAdded(string userId);

        /**
         * Occurs when a user is removed from a contact list.
         * 
         * @param userId    The user ID of the removed contact.
         */
        void OnContactDeleted(string userId);

        /**
         * Occurs when a user receives a friend request.
         *
         * @param userId    The ID of the user who initiates the friend request.
         * @param reason      The invitation message. 
         */
        void OnContactInvited(string userId, string reason);

        /**
        * Occurs when a friend request is approved.
        *
        * @param userId The ID of the user who initiates the friend request.
        */
        void OnFriendRequestAccepted(string userId);

        /**
         * Occurs when a friend request is declined.
         *
         * @param userId The ID of the user who initiates the friend request.
         */
        void OnFriendRequestDeclined(string userId);
    }
}
