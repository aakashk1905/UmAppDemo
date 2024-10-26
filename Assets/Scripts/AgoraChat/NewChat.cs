using UnityEngine.UI;
using TMPro;
using AgoraChat;
using AgoraChat.MessageBody;
using UnityEngine;
using System.Collections.Generic;
using System;
using Agora_RTC_Plugin.API_Example;
using System.Collections;
using UnityEngine.Networking;

public class NewChat : MonoBehaviour, IChatManagerDelegate, IConnectionDelegate 
{
    private TMP_Text messageList;
    [SerializeField] private string userId = "";
    [SerializeField] private string appKey = "";
    private bool isJoined = false;
    private string tokenBase = "https://agoraapi.vercel.app/chattoken";
    SDKClient agoraChatClient;
    private string currentRecipient = "";
    public string recipient = "";


    public void joinLeave()
    {
        if (isJoined)
        {
            agoraChatClient.Logout(true, callback: new CallBack(
            onSuccess: () =>
            {
                Debug.LogError("Logout succeed");
                isJoined = false;
                GameObject.Find("joinBtn").GetComponentInChildren<TextMeshProUGUI>().text = "Join";
            },
            onError: (code, desc) =>
            {
                Debug.LogError($"Logout failed, code: {code}, desc: {desc}");
            }));
        }
        else
        {
            agoraChatClient.Login(userId, "user1password",false, callback: new CallBack(
            onSuccess: () =>
            {
                Debug.LogError("Login succeed");
                isJoined = true;
                GameObject.Find("joinBtn").GetComponentInChildren<TextMeshProUGUI>().text = "Leave";
            },
            onError: (code, desc) =>
            {
                Debug.LogError($"Login failed, code: {code}, desc: {desc}");
            }));
        }
    }

    /*public void sendMessage()
    {
        string recipient = GameObject.Find("userName").GetComponent<TMP_InputField>().text;
        string Msg = GameObject.Find("message").GetComponent<TMP_InputField>().text;
        if (Msg == "" || recipient == "")
        {
            Debug.LogError("You did not type your message");
            return;
        }
        Message msg = Message.CreateTextSendMessage(recipient, Msg);
        displayMessage(Msg, true);
        agoraChatClient.ChatManager.SendMessage(ref msg, new CallBack(
            onSuccess: () =>
            {
                Debug.LogError($"Send message succeed");
                GameObject.Find("message").GetComponent<TMP_InputField>().text = "";
            },
            onError: (code, desc) =>
            {
                Debug.LogError($"Send message failed, code: {code}, desc: {desc}");
            }));
    }*/


    public void sendMessage()
    {
        
        string Msg = GameObject.Find("message").GetComponent<TMP_InputField>().text;

        if (string.IsNullOrEmpty(Msg) || string.IsNullOrEmpty(recipient))
        {
            Debug.LogError("You did not type your message");
            return;
        }

        if (recipient != currentRecipient)
        {
            currentRecipient = recipient;
            LoadMessageHistory(currentRecipient); 
        }

        Message msg = Message.CreateTextSendMessage(recipient, Msg);
        displayMessage(Msg, true);
        agoraChatClient.ChatManager.SendMessage(ref msg, new CallBack(
            onSuccess: () =>
            {
                Debug.LogError($"Send message succeed");
                GameObject.Find("message").GetComponent<TMP_InputField>().text = "";
            },
            onError: (code, desc) =>
            {
                Debug.LogError($"Send message failed, code: {code}, desc: {desc}");
            }));
    }

    public void LoadMessageHistory(string recipient)
    {
        messageList.text = ""; // Clear current chat display

        // Get or create a conversation with the specific recipient
        var conversation = agoraChatClient.ChatManager.GetConversation(recipient, ConversationType.Chat);

        if (conversation != null)
        {
            // Load the last 50 messages with a callback to handle the result
            conversation.LoadMessages(null,count: 50, MessageSearchDirection.UP,new ValueCallBack<List<Message>>(
                onSuccess: (List<Message> historyMessages) =>
                {
                    foreach (var msg in historyMessages)
                    {
                        if (msg.Body.Type == MessageBodyType.TXT)
                        {
                            TextBody txtBody = msg.Body as TextBody;
                            displayMessage($"{msg.From}: {txtBody.Text}", msg.From == userId);
                        }
                    }
                    Debug.Log("Loaded message history successfully.");
                },
                onError: (code, desc) =>
                {
                    Debug.LogError($"Failed to load message history, code: {code}, desc: {desc}");
                }));
        }
        else
        {
            Debug.LogError("No conversation found with the specified recipient.");
        }
    }


    public void displayMessage(string messageText, bool isSentMessage)
    {
        if (isSentMessage)
        {
            messageList.text += "<align=\"right\"><color=black><mark=#dcf8c655 padding=\"10, 10, 0, 0\">" + messageText + "</color></mark>\n";
        }
        else
        {
            messageList.text += "<align=\"left\"><color=black><mark=#ffffff55 padding=\"10, 10, 0, 0\">" + messageText + "</color></mark>\n";
        }
    }


    void Start()
    {
        GameObject.Find("userName/Text Area/Placeholder").GetComponent<TMP_Text>().text = "Enter recipient name";
        GameObject.Find("message/Text Area/Placeholder").GetComponent<TMP_Text>().text = "Message";
        messageList = GameObject.Find("scrollView/Viewport/Content").GetComponent<TextMeshProUGUI>();
        messageList.fontSize = 14;
        messageList.text = "";
        GameObject button = GameObject.Find("joinBtn");
        button.GetComponent<Button>().onClick.AddListener(joinLeave);
        button = GameObject.Find("sendBtn");
        button.GetComponent<Button>().onClick.AddListener(sendMessage);
        setupChatSDK();
    }
    private void Update()
    {
       
        if (userId == "" && UserDataManager.Instance!=null && UserDataManager.Instance.GetUserEmail() != null)
        {
            Debug.LogError(UserDataManager.Instance.GetUserEmail());
            userId = UserDataManager.Instance.GetUserEmail().Split("@")[0];
            joinLeave();
            //StartCoroutine(FetchAndJoinChannel());
        }
    }
    void OnApplicationQuit()
    {
        agoraChatClient.ChatManager.RemoveChatManagerDelegate(this);
        agoraChatClient.Logout(true, callback: new CallBack(
            onSuccess: () => 
            {
                Debug.LogError("Logout succeed");
            },
            onError: (code, desc) => 
            {
                Debug.LogError($"Logout failed, code: {code}, desc: {desc}");
            }));
    }

    void setupChatSDK()
    {
        if (appKey == "")
        {
            Debug.LogError("You should set your appKey first!");
            return;
        }

        Options options = new Options(appKey);
        options.UsingHttpsOnly = true;
        options.DebugMode = true;
        agoraChatClient = SDKClient.Instance;
        agoraChatClient.InitWithOptions(options);
        agoraChatClient.ChatManager.AddChatManagerDelegate(this);
    }

    public void OnMessagesReceived(List<Message> messages)
    {
        foreach (Message msg in messages)
        {
            if (msg.Body.Type == MessageBodyType.TXT && msg.From == currentRecipient)
            {
                TextBody txtBody = msg.Body as TextBody;
                displayMessage($"{msg.From}: {txtBody.Text}", false);
            }
        }
    }

    public void OnCmdMessagesReceived(List<Message> messages)
    {

    }

    public void OnMessagesRead(List<Message> messages)
    {

    }

    public void OnMessagesDelivered(List<Message> messages)
    {

    }

    public void OnMessagesRecalled(List<Message> messages)
    {

    }

    public void OnReadAckForGroupMessageUpdated()
    {

    }

    public void OnGroupMessageRead(List<GroupReadAck> list)
    {

    }

    public void OnConversationsUpdate()
    {

    }

    public void OnConversationRead(string from, string to)
    {

    }

    public void MessageReactionDidChange(List<MessageReactionChange> list)
    {

    }

    public void OnMessageContentChanged(Message msg, string operatorId, long operationTime)
    {

    }

    public void OnConnected()
    {

    }

    public void OnDisconnected()
    {

    }

    public void OnLoggedOtherDevice(string deviceName)
    {

    }

    public void OnRemovedFromServer()
    {

    }

    public void OnForbidByServer()
    {

    }

    public void OnChangedIMPwd()
    {

    }

    public void OnLoginTooManyDevice()
    {

    }

    public void OnKickedByOtherDevice()
    {

    }

    public void OnAuthFailed()
    {

    }

    public void OnTokenExpired()
    {

    }

    public void OnTokenWillExpire()
    {

    }

    public void OnAppActiveNumberReachLimitation()
    {

    }
}
