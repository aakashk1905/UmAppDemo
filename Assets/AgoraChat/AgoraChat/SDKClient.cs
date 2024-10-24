using System;
using System.Collections.Generic;

namespace AgoraChat
{
    /**
     * The SDK client class, the entry of the chat SDK, defines how to log in to and log out of the chat app and how to manage the connection between the SDK and the chat server.
     */
    public class SDKClient
    {
        private static SDKClient _instance;
        internal IClient _clientImpl;

        public static SDKClient Instance
        {
            get
            {
                return _instance = _instance ?? new SDKClient();
            }
        }

        private SDKClient()
        {

            _clientImpl = new IClient(new NativeListener());
        }


        /**
         * The chat manager instance.
         */
        public ChatManager ChatManager
        {
            get => _clientImpl.chatManager;
        }

        /**
         * The contact manager instance.
         */
        public ContactManager ContactManager
        {
            get => _clientImpl.contactManager;
        }

        /**
         * The group manager instance.
         */
        public GroupManager GroupManager
        {
            get => _clientImpl.groupManager;
        }

        /**
         * The chat room manager instance.
         */
        public RoomManager RoomManager
        {
            get => _clientImpl.roomManager;
        }

        /**
         * The user information manager instance.
         */
        public UserInfoManager UserInfoManager
        {
            get => _clientImpl.userInfoManager;
        }

        /**
         * The presence manager instance.
         */
        public PresenceManager PresenceManager
        {
            get => _clientImpl.presenceManager;
        }

        /**
         * The thread manager instance.
         */
        public ChatThreadManager ThreadManager
        {
            get => _clientImpl.chatThreadManager;
        }

        internal ConversationManager ConversationManager
        {
            get => _clientImpl.conversationManager;
        }

        internal MessageManager MessageManager
        {
            get => _clientImpl.messageManager;
        }

        /**
         * The SDK version.
         */
        public string SdkVersion { get => "1.2.1"; }


        /**
         * The ID of the current login user.
         */
        public string CurrentUsername { get => _clientImpl.CurrentUsername(); }

        /**
         * Whether the current user is logged into the chat app.
         * - `true`: Yes.
         * - `false`: No. The current user is not logged into the chat app yet.
         */
        public bool IsLoggedIn { get => _clientImpl.IsLoggedIn(); }

        /**
         * Whether the SDK is connected to the server.
         * - `true`: Yes.
         * - `false`: No.
         */
        public bool IsConnected { get => _clientImpl.IsConnected(); }

        /**
         * The token of the current user.
         */
        public string AccessToken { get => _clientImpl.AccessToken(); }

        /**
        * Initializes the SDK.
        * 
        * Make sure that the SDK initialization is complete before you call any methods.
        *
        * @param options The options for SDK initialization. Ensure that you set the options. See {@link Options}.
        * @return The return result:
        * - `0`: Success;
        * - `100`: The App key is invalid.
        */
        public int InitWithOptions(Options options)
        {
            return _clientImpl.InitWithOptions(options);
        }

        /**
         * Creates a new user.
         *
         * This method is not recommended and you are advised to call the RESTful API.
         *
         * This is an asynchronous method.
         *
         * @param userId The user ID. Ensure that you set this parameter.
         * 
         * The user ID can contain a maximum of 64 characters of the following types:
         * - 26 lowercase English letters (a-z);
         * - 26 uppercase English letters (A-Z);
         * - 10 numbers (0-9);
         * - "_", "-", "."
         * 
         * The user ID is case-insensitive, so Aa and aa are the same user ID. 
         * 
         * The email address or the UUID of the user cannot be used as the user ID.
         * 
         * You can also set the user ID using a regular expression in the format of ^[a-zA-Z0-9_-]+$. 
         * 
         * @param password The password. The password can contain a maximum of 64 characters. Ensure that you set this parameter.
         * @param callback  The creation result callback. See {@link CallBack}.          
         *             
         */
        public void CreateAccount(string userId, string password, CallBack callback = null)
        {
            _clientImpl.CreateAccount(userId, password, callback);
        }

        /**
         * Logs in to the chat server with a password or token.
         *
         * This is an asynchronous method.
         *
         * @param userId 		The user ID. Ensure that you set this parameter.
         * @param pwdOrToken 	The password or token of the user. Ensure that you set this parameter.
         * @param isToken       Whether to log in with a token or a password.
         *                      - `true`：Log in with a token.
         *                      - (Default) `false`：Log in with a password.
         * @param callback 	    The login result callback. See {@link CallBack}.
         *
         */
        public void Login(string userId, string pwdOrToken, bool isToken = false, CallBack callback = null)
        {
            _clientImpl.Login(userId, pwdOrToken, isToken, callback);
        }

        /**
          * Logs you out of the chat service.
          *
          * This is an asynchronous method.
          *
          * @param unbindToken Whether to unbind the device with the token upon logout. This parameter is valid only for mobile platforms.
          * - `true`: Yes.
          * - `false`: No.
          *
          * @param callback 	    The logout result callback. See {@link CallBack}.
          *
          */
        public void Logout(bool unbindDeviceToken = true, CallBack callback = null)
        {
            _clientImpl.Logout(unbindDeviceToken, callback);
        }

        /**
        * Logs in to the chat server with the user ID and an Agora token.
        * 
        * You can also log in to the chat server with the user ID and a password. See {@link #Login(string, string, bool, CallBack)}.
        *
        * This an asynchronous method.
        *
        * @param userId        The user ID. Ensure that you set this parameter.
        * @param token         The Agora token. Ensure that you set this parameter.
        * @param callback      The login result callback. See {@link CallBack}.
        *
        */
        public void LoginWithAgoraToken(string userId, string token, CallBack callback = null)
        {
            _clientImpl.LoginWithAgoraToken(userId, token, callback);
        }

        /**
         * Renews the Agora token.
         * 
         * If you log in with an Agora token and are notified by a callback method {@link IConnectionDelegate} that the token is to be expired, you can call this method to update the token to avoid unknown issues caused by an invalid token.
         *
         * @param token The new Agora token.
         */
        public void RenewAgoraToken(string token)
        {
            _clientImpl.RenewAgoraToken(token);
        }

        /**
         * Gets the list of currently logged-in devices of a specified account.
         *
         * This is an asynchronous method.
         *
         * @param userId        The user ID.
         * @param password      The password.
         * @param callBack		The completion callback. If this call succeeds, calls {@link ValueCallBack#OnSuccessValue(Object)} to show device information list;
         * 						if this call fails, calls {@link ValueCallBack#onError(int, String)}.
         */
        public void GetLoggedInDevicesFromServer(string userId, string password, ValueCallBack<List<DeviceInfo>> callback = null)
        {
            _clientImpl.GetLoggedInDevicesFromServer(userId, password, callback);
        }

        /**
         * Gets the list of currently logged-in devices of a specified account.
         *
         * This is an asynchronous method.
         *
         * @param userId        The user ID.
         * @param token         The token.
         * @param callBack		The completion callback. If this call succeeds, calls {@link ValueCallBack#OnSuccessValue(Object)} to show device information list;
         * 						if this call fails, calls {@link ValueCallBack#onError(int, String)}.
         */
        public void GetLoggedInDevicesFromServerWithToken(string userId, string token, ValueCallBack<List<DeviceInfo>> callback = null)
        {
            _clientImpl.GetLoggedInDevicesFromServerWithToken(userId, token, callback);
        }

        /**
         * Forces the specified account to log out from the specified device.
         *
         * You can call {@link GetLoggedInDevicesFromServer()} to get the device information {@link DeviceInfo}.
         *
         * This is an asynchronous method.
         *
         * @param userId   The user ID.
         * @param password The password.
         * @param resource The device ID. See {@link DeviceInfo#Resource}.
         */
        public void KickDevice(string userId, string password, string resource, CallBack callback = null)
        {
            _clientImpl.KickDevice(userId, password, resource, callback);
        }

        /**
        * Forces the specified account to log out from the specified device.
        *
        * You can call {@link GetLoggedInDevicesFromServer()} to get the device information {@link DeviceInfo}.
        *
        * This is an asynchronous method.
        *
        * @param userId   The user ID.
        * @param token    The token.
        * @param resource The device ID. See {@link DeviceInfo#Resource}.
        */
        public void KickDeviceWithToken(string userId, string token, string resource, CallBack callback = null)
        {
            _clientImpl.KickDeviceWithToken(userId, token, resource, callback);
        }

        /**
         * Forces the specified account to log out from all devices.
         *
         * This is an asynchronous method.
         *
         * @param userId   The user ID.
         * @param password The password.
         * @param callback The operation callback. See {@link CallBack}.
         */
        public void KickAllDevices(string userId, string password, CallBack callback = null)
        {
            _clientImpl.KickAllDevices(userId, password, callback);
        }

        /**
         * Forces the specified account to log out from all devices.
         *
         * This is an asynchronous method.
         *
         * @param userId   The user ID.
         * @param token    The token.
         * @param callback The operation callback. See {@link CallBack}.
         */
        public void KickAllDevicesWithToken(string userId, string token, CallBack callback = null)
        {
            _clientImpl.KickAllDevicesWithToken(userId, token, callback);
        }

        /**
		 * Adds a connection status listener.
		 *
		 * @param connectionDelegate 		The connection status listener to add. It is inherited from {@link IConnectionDelegate}.
		 *
		 */
        public void AddConnectionDelegate(IConnectionDelegate connectionDelegate)
        {
            _clientImpl.AddConnectionDelegate(connectionDelegate);
        }

        /**
		 * Removes a connection status listener.
		 *
		 * @param connectionDelegate 		The connection status listener to remove. It is inherited from {@link IConnectionDelegate}.
		 *
		 */
        public void DeleteConnectionDelegate(IConnectionDelegate connectionDelegate)
        {
            _clientImpl.DeleteConnectionDelegate(connectionDelegate);
        }

        /**
          * Adds a connection listener.
          *
          * @param multiDeviceDelegate 		The multi-device listener to add. It is inherited from {@link IMultiDeviceDelegate}.
          *
          */

        public void AddMultiDeviceDelegate(IMultiDeviceDelegate multiDeviceDelegate)
        {
            _clientImpl.AddMultiDeviceDelegate(multiDeviceDelegate);
        }

        /**
		 * Removes a connection listener.
		 *
		 * @param multiDeviceDelegate 		The multi-device listener to remove. It is inherited from {@link IMultiDeviceDelegate}.
		 *
		 */
        public void DeleteMultiDeviceDelegate(IMultiDeviceDelegate multiDeviceDelegate)
        {
            _clientImpl.DeleteMultiDeviceDelegate(multiDeviceDelegate);
        }

        [Obsolete]
        public void DeInit()
        {
            _clientImpl.CleanUp();
        }

        internal void ClearResource()
        {
            _clientImpl.ClearResource();
        }

        internal void DelegateTester()
        {
            _clientImpl.DelegateTesterRun();
        }
    }
}
