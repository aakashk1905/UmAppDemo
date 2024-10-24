using System.Collections.Generic;
using AgoraChat.SimpleJSON;

namespace AgoraChat
{
    public class ChatThreadManager : BaseManager
    {
        internal List<IChatThreadManagerDelegate> delegater;

        internal ChatThreadManager(NativeListener listener) : base(listener, SDKMethod.threadManager)
        {
            listener.ChatThreadManagerEvent += NativeEventHandle;
            delegater = new List<IChatThreadManagerDelegate>();
        }

        /**
        * Creates a message thread.
        *
        * Each member of the chat group where the message thread belongs can call this method.
        *
        * Upon the creation of a message thread, the following will occur:
        *
        * - In a single-device login scenario, each member of the group to which the message thread belongs will receive the {@link IChatThreadManagerDelegate#OnChatThreadCreate(ChatThreadEvent)} callback.
        *   You can listen for message thread events by setting {@link EMChatThreadChangeListener}.
        *
        * - In a multi-device login scenario, the devices will receive the {@link IMultiDeviceDelegate#onThreadMultiDevicesEvent(MultiDevicesOperation, String, List)} callback.
        *   In this callback method, the first parameter indicates a message thread event, for example, {@link MultiDevicesOperation#THREAD_CREATE} for a message thread creation event.
        *   You can listen for message thread events by setting {@link IMultiDeviceDelegate}.
        *
        * @param threadName The name of the new message thread. It can contain a maximum of 64 characters.
        * @param msgId     The ID of the parent message.
        * @param groupId              The parent ID, which is the group ID.
        * 
        * 
        * @param callback      The result callback:
        *                    - If success, {@link EMValueCallBack#onSuccess(Object)} is triggered to return the new message thread object;
        *                    - If a failure occurs, {@link EMValueCallBack#onError(int, String)} is triggered to return an error.
        */
        public void CreateThread(string threadName, string msgId, string groupId, ValueCallBack<ChatThread> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("threadName", threadName);
            jo_param.AddWithoutNull("msgId", msgId);
            jo_param.AddWithoutNull("groupId", groupId);

            Process process = (_, jsonNode) =>
            {
                return ModelHelper.CreateWithJsonObject<ChatThread>(jsonNode);
            };

            NativeCall<ChatThread>(SDKMethod.createChatThread, jo_param, callback, process);
        }

        /**
         * Joins a message thread.
         *
         * Each member of the group where the message thread belongs can call this method.
         *
         * In a multi-device login scenario, note the following:
         *
         * - The devices will receive the {@link IMultiDeviceDelegate#onThreadMultiDevicesEvent(MultiDevicesOperation, String, List)} callback.
         *
         * - In this callback method, the first parameter indicates a message thread event, for example, {@link MultiDevicesOperation#THREAD_JOIN} for a message thread join event.
         *
         * - You can listen for message thread events by setting {@link IMultiDeviceDelegate}.
         *
         * @param threadId   The message thread ID.
         * @param callback     The result callback:
         *                     - If success, {@link ValueCallBack#onSuccess(Object)} is triggered to return the message thread details {@link ChatThread} which do not include the member count.
         *                     - If a failure occurs, {@link ValueCallBack#onError(int, String)} is triggered to return an error.
         *                       If you join a message thread repeatedly, the SDK will return an error related to USER_ALREADY_EXIST.
         */
        public void JoinThread(string threadId, ValueCallBack<ChatThread> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("threadId", threadId);
            Process process = (_, jsonNode) =>
            {
                return ModelHelper.CreateWithJsonObject<ChatThread>(jsonNode);
            };

            NativeCall<ChatThread>(SDKMethod.joinChatThread, jo_param, callback, process);
        }

        /**
        * Leaves a message thread.
        *
        * Each member in the message thread can call this method.
        *
        * In a multi-device login scenario, note the following:
        *
        * - The devices will receive the {@link IMultiDeviceDelegate#onThreadMultiDevicesEvent(MultiDevicesOperation, String, List)} callback.
        *
        * - In this callback method, the first parameter indicates a message thread event, for example, {@link MultiDevicesOperation#THREAD_LEAVE} for a message thread leaving event.
        *
        * - You can listen for message thread events by setting {@link IMultiDeviceDelegate}.
        *
        * @param threadId     The ID of the message thread that the current user wants to leave.
        * @param callback       The result callback:
        *                     - If success, {@link CallBack#onSuccess()} is triggered;
        *                     - If a failure occurs, {@link CallBack#onError(int, String)} is triggered to return an error.
        */
        public void LeaveThread(string threadId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("threadId", threadId);
            NativeCall<ChatThread>(SDKMethod.leaveChatThread, jo_param, callback);
        }

        /**
        * Destroys the message thread.
        *
        * Only the owner or admins of the group where the message thread belongs can call this method.
        *
        * **Note**
        *
        *
        * - In a multi-device login scenario, The devices will receive the {@link IMultiDeviceDelegate#onThreadMultiDevicesEvent(MultiDevicesOperation, String, List)} callback.
        *   In this callback method, the first parameter indicates a message thread event, for example, {@link MultiDevicesOperation#THREAD_DESTROY} for a message thread destruction event.
        *   You can listen for message thread events by setting {@link IMultiDeviceDelegate}.
        *
        * @param threadId  The message thread ID.
        * @param callback    The result callback:
        *                  - If success, {@link CallBack#onSuccess()} is triggered.
        *                   - If a failure occurs, {@link CallBack#onError(int, String)} is triggered to return an error.
        */

        public void DestroyThread(string threadId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("threadId", threadId);
            NativeCall<ChatThread>(SDKMethod.destroyChatThread, jo_param, callback);
        }

        /**
        * Removes a member from the message thread.
        *
        * Only the owner or admins of the group where the message thread belongs and the message thread creator can call this method.
        *
        * The removed member will receive the {@link IChatThreadManagerDelegate#OnUserKickOutOfChatThread(ChatThreadEvent)} callback.
        *
        * You can listen for message thread events by setting {@link IChatThreadManagerDelegate}.
        *
        * @param threadId   The message thread ID.
        * @param userId     The user ID of the member to be removed from the message thread.
        * @param callback     The result callback.
        *                     - If success, {@link CallBack#onSuccess()} is triggered.
        *                     - If a failure occurs, {@link CallBack#onError(int, String)} is triggered to return an error.
        */
        public void RemoveThreadMember(string threadId, string userId, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("threadId", threadId);
            jo_param.AddWithoutNull("userId", userId);
            NativeCall<ChatThread>(SDKMethod.removeMemberFromChatThread, jo_param, callback);
        }

        /**
         * Changes the name of the message thread.
         *
         * Only the owner or admins of the group where the message thread belongs and the message thread creator can call this method.
         *
         * Each member of the group to which the message thread belongs will receive the {@link IChatThreadManagerDelegate#OnChatThreadUpdate(ChatThreadEvent)} callback.
         *
         * You can listen for message thread events by setting {@link IChatThreadManagerDelegate}.
         *
         * @param threadId      The message thread ID.
         * @param newName       The new message thread name. It can contain a maximum of 64 characters.
         * @param callback        The result callback:
         *                      - If success, {@link CallBack#onSuccess()} is triggered.
         *                      - If a failure occurs, {@link CallBack#onError(int, String)} is triggered to return an error.
         */
        public void ChangeThreadName(string threadId, string newName, CallBack callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("threadId", threadId);
            jo_param.AddWithoutNull("newName", newName);
            NativeCall<ChatThread>(SDKMethod.updateChatThreadSubject, jo_param, callback);

        }

        /**
         */
        public void FetchThreadMembers(string threadId, string cursor = null, int pageSize = 20, ValueCallBack<CursorResult<string>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("threadId", threadId);
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

            NativeCall<CursorResult<string>>(SDKMethod.fetchChatThreadMember, jo_param, callback, process);
        }

        /**
        * Use the pagination to get the list of message threads in the specified group.
        *
        * This method gets data from the server.
        *
        * @param groupId   The parent ID, which is the group ID.       
        * @param joined    Whether the current user has joined the thread.
        * @param cursor    The position from which to start getting data. At the first method call, if you set `cursor` to `null` or an empty string, the SDK will get data in the reverse chronological order of when message threads are created.
        * @param pageSize  The number of message threads that you expect to get on each page. The value range is [1,50].
        * @param callBack  The result callback:
        *                  - If success, {@link ValueCallBack#onSuccess(Object)} is triggered to return the result {@link CursorResult}, including the message thread list and the cursor for the next query.
        *                  - If a failure occurs, {@link ValueCallBack#onError(int, String)} is triggered to return an error.
        */

        public void FetchThreadListOfGroup(string groupId, bool joined, string cursor = null, int pageSize = 20, ValueCallBack<CursorResult<ChatThread>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("groupId", groupId);
            jo_param.AddWithoutNull("joined", joined);
            jo_param.AddWithoutNull("cursor", cursor);
            jo_param.AddWithoutNull("pageSize", pageSize);

            Process process = (_, jsonNode) =>
            {
                CursorResult<ChatThread> cursor_msg = new CursorResult<ChatThread>(_, (jn) =>
                {
                    return ModelHelper.CreateWithJsonObject<ChatThread>(jn);
                });

                cursor_msg.FromJsonObject(jsonNode.AsObject);
                return cursor_msg;

            };

            NativeCall<CursorResult<ChatThread>>(SDKMethod.fetchChatThreadsWithParentId, jo_param, callback, process);
        }

        /**
         * Uses the pagination to get the list of message threads that the current user has joined.
         *
         * This method gets data from the server.
         *         
         * @param cursor    The position from which to start getting data. At the first method call, if you set `cursor` to `null` or an empty string, the SDK will get data in the reverse chronological order of when the user joins the message threads.
         * @param pageSize     The number of message threads that you expect to get on each page. The value range is [1,50].
         * @param callBack  The result callback:
         *                  - If success, {@link ValueCallBack#onSuccess(Object)} is triggered to return the result {@link CursorResult}, including the message thread list and the cursor for the next query.
         *                  - If a failure occurs, {@link ValueCallBack#onError(int, String)} is triggered to return an error.
         */

        public void FetchMineJoinedThreadList(string cursor = null, int pageSize = 20, ValueCallBack<CursorResult<ChatThread>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("cursor", cursor);
            jo_param.AddWithoutNull("pageSize", pageSize);

            Process process = (_, jsonNode) =>
            {
                CursorResult<ChatThread> cursor_msg = new CursorResult<ChatThread>(_, (jn) =>
                {
                    return ModelHelper.CreateWithJsonObject<ChatThread>(jn);
                });

                cursor_msg.FromJsonObject(jsonNode.AsObject);
                return cursor_msg;

            };

            NativeCall<CursorResult<ChatThread>>(SDKMethod.fetchJoinedChatThreads, jo_param, callback, process);
        }

        /**
         * Gets the details of the message thread from the server.
         *
         * @param threadId   The message thread ID.
         * @param callback       The result callback:
         *                       - If success, {@link ValueCallBack#onSuccess(Object)} is triggered to return the message thread details;
         *                       - If a failure occurs, {@link ValueCallBack#onError(int, String)} is triggered to return an error.
         */
        public void GetThreadDetail(string threadId, ValueCallBack<ChatThread> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("threadId", threadId);
            Process process = (_, jsonNode) =>
            {
                return ModelHelper.CreateWithJsonObject<ChatThread>(jsonNode);
            };

            NativeCall<ChatThread>(SDKMethod.fetchChatThreadDetail, jo_param, callback, process);
        }

        /**
         */
        public void GetLastMessageAccordingThreads(List<string> threadIds, ValueCallBack<Dictionary<string, Message>> callback = null)
        {
            JSONObject jo_param = new JSONObject();
            jo_param.AddWithoutNull("threadIds", JsonObject.JsonArrayFromStringList(threadIds));
            Process process = (_, jsonNode) =>
            {
                return Dictionary.BaseModelDictionaryFromJsonObject<Message>(jsonNode);
            };

            NativeCall<Dictionary<string, Message>>(SDKMethod.fetchLastMessageWithChatThreads, jo_param, callback, process);
        }

        /**
         * Adds the message thread event listener, which listens for message thread changes, such as the message thread creation and destruction.
         *
         * You can call {@link #RemoveThreadManagerDelegate} to remove an unnecessary message thread event listener.
         *
         * @param threadManagerDelegate The message thread event listener to add.
         */
        public void AddThreadManagerDelegate(IChatThreadManagerDelegate threadManagerDelegate)
        {
            if (!delegater.Contains(threadManagerDelegate))
            {
                delegater.Add(threadManagerDelegate);
            }
        }

        /**
         * Removes the message thread event listener.
         *
         * After a message thread event listener is added with {@link #AddThreadManagerDelegate}, you can call this method to remove it when it is not required.
         *
         * @param listener  The message thread event listener to remove.
         */
        public void RemoveThreadManagerDelegate(IChatThreadManagerDelegate threadManagerDelegate)
        {
            delegater.Remove(threadManagerDelegate);
        }

        internal void ClearDelegates()
        {
            delegater.Clear();
        }

        internal void NativeEventHandle(string method, JSONNode jsonNode)
        {
            if (delegater.Count == 0) return;

            ChatThreadEvent threadEvent = ModelHelper.CreateWithJsonObject<ChatThreadEvent>(jsonNode);
            if (threadEvent != null)
            {
                foreach (IChatThreadManagerDelegate it in delegater)
                {
                    switch (method)
                    {
                        case SDKMethod.onChatThreadCreate:
                            it.OnChatThreadCreate(threadEvent);
                            break;
                        case SDKMethod.onChatThreadUpdate:
                            it.OnChatThreadUpdate(threadEvent);
                            break;
                        case SDKMethod.onChatThreadDestroy:
                            it.OnChatThreadDestroy(threadEvent);
                            break;
                        case SDKMethod.onUserKickOutOfChatThread:
                            it.OnUserKickOutOfChatThread(threadEvent);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
