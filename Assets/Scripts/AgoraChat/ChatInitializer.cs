using UnityEngine;
using AgoraChat;
using TMPro;
using System.Collections.Generic;

public class ChatInitializer : MonoBehaviour, IConnectionDelegate
{
    [SerializeField] private string userId = "";
    [SerializeField] private string appKey = "";
    private SDKClient agoraChatClient;
    private bool isJoined = false;
    public Dictionary<string, int> UnreadMessages;

    void Start()
    {
        setupChatSDK();
        UnreadMessages = new Dictionary<string, int>();

    }
    private void Update()
    {
        if (userId == "" && UserDataManager.Instance != null && UserDataManager.Instance.GetUserEmail() != null)
        {
            Debug.LogError(UserDataManager.Instance.GetUserEmail());
            userId = UserDataManager.Instance.GetUserEmail().Split("@")[0];
            if (!isJoined)
            {
                joinAgoraChat();
            }
        }
    }
    private void setupChatSDK()
    {
        if (string.IsNullOrEmpty(appKey))
        {
            Debug.LogError("AppKey is missing!");
            return;
        }

        Options options = new Options(appKey)
        {
            UsingHttpsOnly = true,
            DebugMode = true
        };

        agoraChatClient = SDKClient.Instance;
        agoraChatClient.InitWithOptions(options);
    }

    private void joinAgoraChat()
    {
        if (agoraChatClient == null)
        {
            Debug.LogError("SDK Client not initialized.");
            return;
        }

        agoraChatClient.Login(userId, "user1password", false, callback: new CallBack(
            onSuccess: () =>
            {
                Debug.Log("Login successful");
                isJoined = true;
            },
            onError: (code, desc) =>
            {
                Debug.LogError($"Login failed: code {code}, desc: {desc}");
            }));
    }
    public bool IsJoined => isJoined;
    public string UserId => userId;

    void OnApplicationQuit()
    {
        if (agoraChatClient != null)
        {
            agoraChatClient.Logout(true, new CallBack(
                onSuccess: () => Debug.Log("Logout successful"),
                onError: (code, desc) => Debug.LogError($"Logout failed: code {code}, desc: {desc}")
            ));
        }
    }

    void IConnectionDelegate.OnConnected()
    {
        throw new System.NotImplementedException();
    }

    void IConnectionDelegate.OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    void IConnectionDelegate.OnLoggedOtherDevice(string deviceName)
    {
        throw new System.NotImplementedException();
    }

    void IConnectionDelegate.OnRemovedFromServer()
    {
        throw new System.NotImplementedException();
    }

    void IConnectionDelegate.OnForbidByServer()
    {
        throw new System.NotImplementedException();
    }

    void IConnectionDelegate.OnChangedIMPwd()
    {
        throw new System.NotImplementedException();
    }

    void IConnectionDelegate.OnLoginTooManyDevice()
    {
        throw new System.NotImplementedException();
    }

    void IConnectionDelegate.OnKickedByOtherDevice()
    {
        throw new System.NotImplementedException();
    }

    void IConnectionDelegate.OnAuthFailed()
    {
        throw new System.NotImplementedException();
    }

    void IConnectionDelegate.OnTokenExpired()
    {
        throw new System.NotImplementedException();
    }

    void IConnectionDelegate.OnTokenWillExpire()
    {
        throw new System.NotImplementedException();
    }

    void IConnectionDelegate.OnAppActiveNumberReachLimitation()
    {
        throw new System.NotImplementedException();
    }

    
}
