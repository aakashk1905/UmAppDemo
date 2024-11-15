using UnityEngine;
using AgoraChat;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using AgoraChat.MessageBody;
using Newtonsoft.Json;

public class ChatInitializer : MonoBehaviour, IConnectionDelegate, IChatManagerDelegate
{
    [SerializeField] private string userId = "";
    [SerializeField] private string appKey = "";
    [SerializeField] private GameObject leftMessagePrefab;
    [SerializeField] private GameObject rightMessagePrefab;
    [SerializeField] private Transform chatContent;
    [SerializeField] private Transform NotifOuter;

    private SDKClient agoraChatClient;
    private bool isJoined = false;
    public Dictionary<string, int> UnreadMessages = new Dictionary<string, int>();
    private TMP_Text messageList;

    public string currentRecipient = "";
    public string recipient = "";

    void Start()
    {
        LoadUnreadMessages();
        setupChatSDK();
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
                agoraChatClient.ChatManager.AddChatManagerDelegate(this);
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
        SaveUnreadMessages();
        if (agoraChatClient != null)
        {
            agoraChatClient.Logout(true, new CallBack(
                onSuccess: () => Debug.Log("Logout successful"),
                onError: (code, desc) => Debug.LogError($"Logout failed: code {code}, desc: {desc}")
            ));
        }
    }

    public void Load()
    {
        if (isJoined)
        {
            GameObject.Find("message/Text Area/Placeholder").GetComponent<TMP_Text>().text = "Message";
            messageList = GameObject.Find("scrollView/Viewport/Content").GetComponent<TextMeshProUGUI>();
            GameObject.Find("CloseChat").GetComponent<Button>().onClick.AddListener(CloseChat);
            GameObject.Find("sendBtn").GetComponent<Button>().onClick.AddListener(sendMessage);
            StartWaitAndLoad();
        }
        else
        {
            Debug.LogWarning("Player not logged in. Ensure ChatInitializer has run.");
        }
    }

    private void CloseChat()
    {
        recipient = "";
        currentRecipient = "";
    }

    private void SaveUnreadMessages()
    {
        string json = JsonConvert.SerializeObject(UnreadMessages);
        PlayerPrefs.SetString("UnreadMessages", json);
        PlayerPrefs.Save();
        Debug.LogWarning("Unread messages saved.");
    }
    private void LoadUnreadMessages()
    {
        if (PlayerPrefs.HasKey("UnreadMessages"))
        {
            string json = PlayerPrefs.GetString("UnreadMessages");
            UnreadMessages = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            Debug.LogWarning("Unread messages loaded.");
            UpdateOuterNotif();
        }
        
    }


    private IEnumerator waitAndLoad()
    {
        yield return new WaitUntil(() => isJoined);
        currentRecipient = recipient;
        LoadMessageHistory(recipient);
    }

    public void StartWaitAndLoad()
    {
        if (isJoined)
        {
            // Immediately load the message history if already joined
            LoadMessageHistory(recipient);
        }
        else
        {
            // Otherwise, wait for `isJoined` to be true
            StartCoroutine(waitAndLoad());
        }
    }

    public void sendMessage()
    {
        string Msg = GameObject.Find("message").GetComponent<TMP_InputField>().text;

        if (string.IsNullOrEmpty(Msg) || string.IsNullOrEmpty(recipient))
        {
            Debug.LogError("You did not type your message");
            return;
        }

        Message msg = Message.CreateTextSendMessage(recipient, Msg);

        agoraChatClient.ChatManager.SendMessage(ref msg, new CallBack(
            onSuccess: () =>
            {
                Debug.LogError("Send message succeed");
                displayMessage(Msg, true);
                GameObject.Find("message").GetComponent<TMP_InputField>().text = "";
            },
            onError: (code, desc) =>
            {
                Debug.LogError($"Send message failed, code: {code}, desc: {desc}");
            }));
    }

    public void LoadMessageHistory(string recipient)
    {
        if (string.IsNullOrEmpty(recipient))
        {
            Debug.LogError("Recipient is empty.");
            return;
        }

        emptyMsgList(chatContent);
        Debug.LogError(recipient);
        var conversation = agoraChatClient.ChatManager.GetConversation(recipient, ConversationType.Chat);

        if (conversation != null)
        {
            conversation.LoadMessages(null, count: 50, MessageSearchDirection.UP, new ValueCallBack<List<Message>>(
                onSuccess: (List<Message> historyMessages) =>
                {
                    foreach (var msg in historyMessages)
                    {
                        if (msg.Body.Type == MessageBodyType.TXT)
                        {
                            TextBody txtBody = msg.Body as TextBody;
                            displayMessage(txtBody.Text, msg.From == userId);
                        }
                    }
                   
                    Debug.Log("Loaded message history successfully.");
                    UnreadMessages[recipient] = 0;
                    UpdateOuterNotif();
                    ResetNotificationCounter(recipient);
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
    private float cumulativeHeight = 0f;

    public void displayMessage(string messageText, bool isSentMessage)
    {
        GameObject messagePrefab = isSentMessage ? rightMessagePrefab : leftMessagePrefab;

        GameObject messageInstance = Instantiate(messagePrefab, chatContent);

        TMP_Text nameText = messageInstance.transform.Find("NameText").GetComponent<TMP_Text>();
        TMP_Text messageTextComponent = messageInstance.transform.Find("MessageText").GetComponent<TMP_Text>();
        nameText.text = isSentMessage ? "You" : recipient;
        messageTextComponent.text = messageText;

        RectTransform rectTransform = messageInstance.GetComponent<RectTransform>();

        if (isSentMessage)
        {
            rectTransform.pivot = new Vector2(1, 1);
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(-10, -cumulativeHeight); 
            messageTextComponent.alignment = TextAlignmentOptions.Right;
        }
        else
        {
            rectTransform.pivot = new Vector2(0, 1); 
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(10, -cumulativeHeight); 
            messageTextComponent.alignment = TextAlignmentOptions.Left;
        }

        cumulativeHeight += rectTransform.sizeDelta.y + 10f; 

        LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent.GetComponent<RectTransform>());

        ScrollRect scrollRect = messageInstance.gameObject.GetComponentInParent<ScrollRect>();
        ScrollToBottom(scrollRect );
    }

    public void ScrollToBottom(ScrollRect scrollRect)
    {
        Canvas.ForceUpdateCanvases();  // Ensure layout is updated
        scrollRect.verticalNormalizedPosition = 0f;  // Scroll to bottom
    }
    public void emptyMsgList(Transform chatContent)
    {
        foreach (Transform child in chatContent)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnMessagesReceived(List<Message> messages)
    {
        foreach (Message msg in messages)
        {
            if (msg.Body.Type == MessageBodyType.TXT && msg.From == currentRecipient)
            {
                TextBody txtBody = msg.Body as TextBody;
                displayMessage(txtBody.Text, false);
            }
            if (msg.From != currentRecipient)
            {
                UnreadMessages[msg.From] = UnreadMessages.GetValueOrDefault(msg.From, 0) + 1;
                UpdateOuterNotif();
                Debug.LogError("New Message from " + msg.From);
            }
        }
    }

    public void ResetNotificationCounter(string userId)
    {
        if (UnreadMessages.ContainsKey(userId))
        {
            // Reset the unread message count for the user
            UnreadMessages[userId] = 0;

            // Update the UI to reflect the reset count
            GameObject playerObject = GameObject.Find(userId);

            if (playerObject != null)
            {
                Transform msgBtn = playerObject.transform.Find("msgBtn");
                if (msgBtn != null)
                {
                    Transform imgTransform = msgBtn.Find("img");
                    if (imgTransform != null)
                    {
                       
                        Transform notificationCounter = imgTransform.Find("NotificationCount");
                        if (notificationCounter != null)
                        {
                            TextMeshProUGUI notificationText = notificationCounter.GetComponent<TextMeshProUGUI>();
                            if (notificationText != null)
                            {
                               
                                notificationText.text = "0";
                            }
                            
                        }
                        imgTransform.gameObject.SetActive(false);
                    }

                    
                }
                
            }
            
        }
    }
    public void UpdateNotificationCounter()
    {
        foreach (var entry in UnreadMessages)
        {
            string userId = entry.Key;
            int messageCount = entry.Value;
            if (messageCount > 0)
            {
                GameObject playerObject = GameObject.Find(userId);

                if (playerObject != null)
                {
                    Transform msgBtn = playerObject.transform.Find("msgBtn");
                    if (msgBtn != null)
                    {
                        Transform imgTransform = msgBtn.Find("img");
                        if (imgTransform != null)
                        {
                            Transform notificationCounter = imgTransform.Find("NotificationCount");
                            if (notificationCounter != null)
                            {
                                TextMeshProUGUI notificationText = notificationCounter.GetComponent<TextMeshProUGUI>();
                                if (notificationText != null)
                                {
                                    notificationText.text = messageCount.ToString();
                                    imgTransform.gameObject.SetActive(true);
                                }
                                
                            }
                            
                        }
                        
                    }
                   
                }
                
            }
        }
    }

    public void UpdateOuterNotif()
    {
        StartCoroutine(UpdateOuterNotifCoroutine());
    }

    private IEnumerator UpdateOuterNotifCoroutine()
    {
        // Wait until isJoined is true
        yield return new WaitUntil(() => isJoined);

        int count = UnreadMessages.Count;
        if (NotifOuter != null) // Check if NotifOuter is assigned
        {
            Transform Notif = NotifOuter.Find("Notif");
            if (Notif != null)
            {
                Transform msgBtn = Notif.transform.Find("NotifCount");
                if (msgBtn != null)
                {
                    TextMeshProUGUI notificationText = msgBtn.GetComponent<TextMeshProUGUI>();
                    if (notificationText != null)
                    {
                        notificationText.text = count.ToString();
                        Notif.gameObject.SetActive(true);
                    }
                }
                else
                {
                    Debug.LogWarning("NotifCount Not Found");
                }
            }
            else
            {
                Debug.LogWarning("Notif Not Found");
            }
        }
        else
        {
            Debug.LogWarning("NotifOuter Not Found");
        }
    }


    /*public void UpdateOuterNotif()
    {
        int count = UnreadMessages.Count;
        if (!NotifOuter)
        {
            Transform Notif = NotifOuter.Find("Notif");
            if (Notif != null)
            {
                Transform msgBtn = Notif.transform.Find("NotifCount");
                if (msgBtn != null)
                {
                    TextMeshProUGUI notificationText = msgBtn.GetComponent<TextMeshProUGUI>();
                    if (notificationText != null)
                    {
                        notificationText.text = count.ToString();
                        Notif.gameObject.SetActive(true);
                    }
                }
                else
                {
                    Debug.LogWarning("NotifCount Not FOund");
                }
            }
            else
            {
                Debug.LogWarning("Notif Not FOund");
            }
        }
        else
        {
            Debug.LogWarning("NotifOuter Not Found");
        }
    }*/
    public void OnConnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnLoggedOtherDevice(string deviceName)
    {
        throw new System.NotImplementedException();
    }

    public void OnRemovedFromServer()
    {
        throw new System.NotImplementedException();
    }

    public void OnForbidByServer()
    {
        throw new System.NotImplementedException();
    }

    public void OnChangedIMPwd()
    {
        throw new System.NotImplementedException();
    }

    public void OnLoginTooManyDevice()
    {
        throw new System.NotImplementedException();
    }

    public void OnKickedByOtherDevice()
    {
        throw new System.NotImplementedException();
    }

    public void OnAuthFailed()
    {
        throw new System.NotImplementedException();
    }

    public void OnTokenExpired()
    {
        throw new System.NotImplementedException();
    }

    public void OnTokenWillExpire()
    {
        throw new System.NotImplementedException();
    }

    public void OnAppActiveNumberReachLimitation()
    {
        throw new System.NotImplementedException();
    }

    public void OnCmdMessagesReceived(List<Message> messages)
    {
        throw new System.NotImplementedException();
    }

    public void OnMessagesRead(List<Message> messages)
    {
        throw new System.NotImplementedException();
    }

    public void OnMessagesDelivered(List<Message> messages)
    {
        throw new System.NotImplementedException();
    }

    public void OnMessagesRecalled(List<Message> messages)
    {
        throw new System.NotImplementedException();
    }

    public void OnReadAckForGroupMessageUpdated()
    {
        throw new System.NotImplementedException();
    }

    public void OnGroupMessageRead(List<GroupReadAck> list)
    {
        throw new System.NotImplementedException();
    }

    public void OnConversationsUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void OnConversationRead(string from, string to)
    {
        throw new System.NotImplementedException();
    }

    public void MessageReactionDidChange(List<MessageReactionChange> list)
    {
        throw new System.NotImplementedException();
    }

    public void OnMessageContentChanged(Message msg, string operatorId, long operationTime)
    {
        throw new System.NotImplementedException();
    }
}
