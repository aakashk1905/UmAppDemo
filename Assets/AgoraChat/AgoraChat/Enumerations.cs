using System;

namespace AgoraChat
{
    /**
    * Area Code.
    */
    public enum AreaCode
    {
        CN = 1,

        NA = 2,

        EU = 4,

        AS = 8,

        JP = 16,

        IN = 32,

        GLOB = -1,
    }

    /**
    * The reason of disconnection.
    */
    public enum DisconnectReason
    {
        /**
        * The SDK is disconnected from the server with no reason.
        */
        Reason_Disconnected,

        /**
        * The user ID or password is wrong.
        */
        Reason_AuthenticationFailed,

        /**
        * The user has logged in to another device.
        */
        Reason_LoginFromOtherDevice,

        /**
        * The user is removed from the server.
        */
        Reason_RemoveFromServer,

        /**
        * The user has logged in to too many devices.
        */
        Reason_LoginTooManyDevice,

        /**
        * The user has changed the password.
        */
        Reason_ChangePassword,

        /**
        * The user is kicked by another device or on the console.
        */
        Reason_KickedByOtherDevice,

        /**
        * The service is disabled.
        */
        Reason_ForbidByServer,
    }

    public enum ChatThreadOperation
    {
        /**
        * Unknown operation, default value.
        */
        UnKnown = 0,
        /**
        * Create a thread.
        */
        Create,
        /**
        * Update a thread.
        */
        Update,
        /**
        * Delete a thread.
        */
        Delete,
        /**
        * Update a thread message.
        */
        Update_Msg,
    }

    /**
    * The group member roles.
    *
    */
    public enum GroupPermissionType
    {
        /**
         * The regular group member.
         */
        Member,

        /**
         * The group admin.
         */
        Admin,

        /**
         * The group owner.
         */
        Owner,

        /**
         * Unknown.
         */
        Unknown = -1,
        Default = Unknown,
        None = Unknown
    }

    /**
         * The group styles.
         */
    public enum GroupStyle
    {
        /**
         * Private groups where only the group owner can invite users to join.
         */
        PrivateOnlyOwnerInvite,

        /**
         * Private groups where each group member can invite users to join.
         */
        PrivateMemberCanInvite,

        /**
         * Public groups where only the owner can invite users to join. 
         * 
         * A user can join a group only after getting approval from the group owner or admins.
         */
        PublicJoinNeedApproval,

        /**
         * Public groups where a user can join a group without the approval from the group owner or admins.
         */
        PublicOpenJoin,
    }

    /**
         *  The chat room member roles.
         */
    public enum RoomPermissionType
    {
        /**
         * The regular chat room member.
         */
        Member,

        /**
         * The chat room admin.
         */
        Admin,

        /**
         * The chat room owner.
         */
        Owner,

        /**
         * Unknown.
         */
        Unknown = -1,
        Default = Unknown,
        None = Unknown
    }

    /**
        * The chat types.
        */
    public enum ConversationType
    {
        /**
        * The one-to-one chat.
        */
        Chat,

        /**
        * The group chat.
        */
        Group,

        /**
        * The chat room.
        */
        Room,
    }

    /**
    * The message types.
    */
    public enum MessageBodyType
    {
        /**
          * The text message.
          */
        TXT,

        /**
          * The image message.
          */
        IMAGE,

        /**
          * The video message.
          */
        VIDEO,

        /**
          * The location message.
          */
        LOCATION,

        /**
          * The voice message.
          */
        VOICE,

        /**
         * The file message.
         */
        FILE,

        /**
          * The command message.
          */
        CMD,

        /**
          * The custom message.
          */
        CUSTOM,

        /**
          * The combined message.
          */
        COMBINE
    };

    /**
    * The chat room message priorities.
    */
    public enum RoomMessagePriority
    {
        /**
        * High priority.
        */
        High = 0,

        /**
        * Normal priority.
        */
        Normal,

        /**
        * Low priority.
        */
        Low
    };

    /**
    * The message query directions. 
    */
    public enum MessageSearchDirection
    {
        /**
        * Messages are retrieved in the reverse chronological order of their Unix timestamp ({@link SortMessageByServerTime}). 
        */
        UP,

        /**
        * Messages are retrieved in the chronological order of their Unix timestamp ({@link SortMessageByServerTime}). 
        */
        DOWN
    };

    /**
    * The message status.
    */
    public enum MessageStatus
    {
        /**
        * The message is created.
        */
        CREATE,

        /**
        * The message is being delivered.
        */
        PROGRESS,

        /**
        * The message is successfully delivered.
        */
        SUCCESS,

        /**
        * The message fails to be delivered.
        */
        FAIL,
    };

    /**
    * The chat types.
    */
    public enum MessageType
    {
        /**
        * The one-to-one chat.
        */
        Chat = 0,

        /**
        * The group chat.
        */
        Group,

        /**
        * The chat room.
        */
        Room,
    };

    /**
    * The message directions.
    */
    public enum MessageDirection
    {
        /**
        * This message is sent from the current user.
        */
        SEND,

        /**
        * The message is received by the current user.
        */
        RECEIVE,
    };

    /**
     *  The extension attribute types of messages.
     */
    public enum AttributeValueType : byte
    {
        /**
         *  Boolean.
         */
        BOOL = 0,

        /**
         *  Signed 32-bit int.
         */
        INT32,

        /**
         *  Unsigned 32-bit int.
         */
        [Obsolete]
        UINT32,

        /**
         *  Signed 64-bit int.
         */
        INT64,

        /**
         *  Float.
         */
        FLOAT,

        /**
         *  Double.
         */
        DOUBLE,

        /**
         *  String.
         */
        STRING,
        //STRVECTOR,
        /**
         *  JSON string.
         */
        JSONSTRING,
        //ATTRIBUTEVALUE,
        NULLOBJ
    };

    namespace MessageBody
    {
        /**
        * The message download status.
        */
        public enum DownLoadStatus
        {
            /**
            * The message is being downloaded.
            */
            DOWNLOADING,

            /**
            * The message is successfully downloaded.
            */
            SUCCESS,

            /**
            * The message fails to be downloaded.
            */
            FAILED,

            /**
            * The download is pending.
            */
            PENDING
        };
    };

    public enum MultiDevicesOperation
    {
        UNKNOWN = -1,
        /**
         * The current user removed a contact on another device.
         */
        CONTACT_REMOVE = 2,

        /**
         * The current user accepted a friend request on another device.
         */
        CONTACT_ACCEPT = 3,

        /**
         * The current user declined a friend request on another device.
         */
        CONTACT_DECLINE = 4,

        /**
         * The current user added a contact to the block list on another device.
         */
        CONTACT_BAN = 5,

        /**
         * The current user removed a contact from the block list on another device.
         */
        CONTACT_ALLOW = 6,

        /**
         * The current user created a group on another device.
         */
        GROUP_CREATE = 10,

        /**
         * The current user destroyed a group on another device.
         */
        GROUP_DESTROY = 11,

        /**
         * The current user joined a group on another device.
         */
        GROUP_JOIN = 12,

        /**
         * The current user left a group on another device.
         */
        GROUP_LEAVE = 13,

        /**
         * The current user requested to join a group on another device.
         */
        GROUP_APPLY = 14,

        /**
         * The current user accepted a group request on another device.
         */
        GROUP_APPLY_ACCEPT = 15,

        /**
         * The current user declined a group request on another device.
         */
        GROUP_APPLY_DECLINE = 16,

        /**
         * The current user invited a user to join the group on another device.
         */
        GROUP_INVITE = 17,

        /**
         * The current user accepted a group invitation on another device.
         */
        GROUP_INVITE_ACCEPT = 18,

        /**
         * The current user declined a group invitation on another device.
         */
        GROUP_INVITE_DECLINE = 19,

        /**
         * The current user kicked a member out of a group on another device.
         */
        GROUP_KICK = 20,

        /**
         * The current user added a member to a group block list on another device.
         */
        GROUP_BAN = 21,

        /**
         * The current user removed a member from a group block list on another device.
         */
        GROUP_ALLOW = 22,

        /**
         * The current user blocked a group on another device.
         */
        GROUP_BLOCK = 23,

        /**
         * The current user unblocked a group on another device.
         */
        GROUP_UNBLOCK = 24,

        /**
         * The current user transferred the group ownership on another device.
         */
        GROUP_ASSIGN_OWNER = 25,

        /**
         * The current user added an admin on another device.
         */
        GROUP_ADD_ADMIN = 26,

        /**
         * The current user removed an admin on another device.
         */
        GROUP_REMOVE_ADMIN = 27,

        /**
         * The current user muted a member on another device.
         */
        GROUP_ADD_MUTE = 28,

        /**
         * The current user unmuted a member on another device.
         */
        GROUP_REMOVE_MUTE = 29,

        /**
         * The current user added a group member to the allow list on another device.
         */
        GROUP_ADD_USER_WHITE_LIST = 30,

        /**
         * The current user removed a group member from the allow list on another device.
         */
        GROUP_REMOVE_USER_WHITE_LIST = 31,

        /**
         * The current user added all other group members to the group mute list on another device.
         */
        GROUP_ALL_BAN = 32,

        /**
         * The current user removed all other group members from the group mute list on another device.
         */
        GROUP_REMOVE_ALL_BAN = 33,

        /**
         * A thread was created on another device.
         */
        THREAD_CREATE = 40,

        /**
        * A thread was destroyed on another device.
        */
        THREAD_DESTROY = 41,

        /**
        * The user joined a thread on another device.
        */
        THREAD_JOIN = 42,

        /**
        * The user left a thread on another device.
        */
        THREAD_LEAVE = 43,

        /**
        * The thread was updated on another device.
        */
        THREAD_UPDATE = 44,

        /**
        * The user was kicked from a thread on another device.
        */
        THREAD_KICK = 45,

        /**
        * The custom attribute(s) of a group member(s) is/are set on other devices.
        */
        SET_METADATA = 50,

        /**
        * The custom attribute(s) of a group member(s) is/are deleted on other devices.
        */
        DELETE_METADATA = 51,

        /**
        * The custom attribute(s) of a group member(s) is/are changed.
        */
        GROUP_MEMBER_METADATA_CHANGED = 52,

        /**
        * A conversation is pinned.
        */
        CONVERSATION_PINNED = 60,

        /**
        * A conversation is unpinned.
        */
        CONVERSATION_UNPINNED = 61,

        /**
        * A conversation is deleted.
        */
        CONVERSATION_DELETED = 62
    };

    public enum MessageReactionOperate
    {
        /**
        * A Reaction is deleted.
        */
        MessageReactionOperateRemove = 0,

        /**
        * A Reaction is added.
        */
        MessageReactionOperateAdd = 1,
    }
}
