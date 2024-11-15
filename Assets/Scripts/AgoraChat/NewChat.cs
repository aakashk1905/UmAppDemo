/*using UnityEngine;
using TMPro;
using AgoraChat;
using System.Collections.Generic;
using AgoraChat.MessageBody;
using System.Collections;
using UnityEngine.UI;

public class NewChat : MonoBehaviour, IChatManagerDelegate
{
    private ChatInitializer chatInitializer;
    private TMP_Text messageList;
    private SDKClient agoraChatClient;
    [SerializeField] private GameObject chatMessagePrefab;
    [SerializeField] private Transform chatContent;
    public string currentRecipient = "";
    public string recipient = "";

    public void Load()
    {

        chatInitializer = FindObjectOfType<ChatInitializer>();
        if (chatInitializer != null && chatInitializer.IsJoined)
        {
            agoraChatClient = SDKClient.Instance;
            agoraChatClient.ChatManager.AddChatManagerDelegate(this);
        }
        else
        {
            Debug.LogWarning("Player not logged in. Ensure ChatInitializer has run.");
        }

        GameObject.Find("message/Text Area/Placeholder").GetComponent<TMP_Text>().text = "Message";
        messageList = GameObject.Find("scrollView/Viewport/Content").GetComponent<TextMeshProUGUI>();
        GameObject close = GameObject.Find("CloseChat");
        close.GetComponent<Button>().onClick.AddListener(CloseChat);
        GameObject button = GameObject.Find("sendBtn");
        button.GetComponent<Button>().onClick.AddListener(sendMessage);
        StartWaitAndLoad();
    }
    private void CloseChat()
    {
        recipient = "";
        currentRecipient = "";
    }

    private void newMsgIndicator(string email, int newMsgCount)
    {
        Transform playerInfo = GameObject.Find(email).GetComponent<Transform>();
        foreach (Transform child in playerInfo)
        {
            if (child.name == "NotificationCount")
            {
                child.GetComponent<TextMeshProUGUI>().text = newMsgCount.ToString();
            }
        }
    }

    private IEnumerator waitAndLoad()
    {

        yield return new WaitUntil(() => chatInitializer.IsJoined);
        currentRecipient = recipient;
        LoadMessageHistory(recipient);
    }

    // Method to start the coroutine
    public void StartWaitAndLoad()
    {
        StartCoroutine(waitAndLoad());
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
                Debug.LogError($"Send message succeed");
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
        emptyMsgList(chatContent);
        Debug.LogError(recipient);
        var conversation = agoraChatClient.ChatManager.GetConversation(recipient, ConversationType.Chat);

        if (conversation != null)
        {
            // Load the last 50 messages with a callback to handle the result
            conversation.LoadMessages(null, count: 50, MessageSearchDirection.UP, new ValueCallBack<List<Message>>(
                onSuccess: (List<Message> historyMessages) =>
                {
                    foreach (var msg in historyMessages)
                    {
                        if (msg.Body.Type == MessageBodyType.TXT)
                        {
                            TextBody txtBody = msg.Body as TextBody;
                            Debug.Log(txtBody.Text);
                            //displayMessage($"{msg.From}: {txtBody.Text}", msg.From == userId);
                            displayMessage($" {txtBody.Text}", msg.From == chatInitializer.UserId);
                        }
                    }
                    Debug.Log("Loaded message history successfully.");
                    chatInitializer.UnreadMessages[recipient] = 0;
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


        GameObject messageInstance = Instantiate(chatMessagePrefab, chatContent);
        TMP_Text nameText = messageInstance.transform.Find("NameText").GetComponent<TMP_Text>();
        TMP_Text messageTextComponent = messageInstance.transform.Find("MessageText").GetComponent<TMP_Text>();
        //Image avatarImage = messageInstance.transform.Find("Avatar").GetComponent<Image>();

        nameText.text = isSentMessage ? "You" : recipient;
        messageTextComponent.text = messageText;

        // Customize avatar based on sender if needed
        //avatarImage.sprite = isSentMessage ? yourAvatarSprite : recipientAvatarSprite;

        // Align to left or right if desired
        RectTransform rectTransform = messageInstance.GetComponent<RectTransform>();
        rectTransform.pivot = isSentMessage ? new Vector2(1, 1) : new Vector2(0, 1);
        rectTransform.localScale = Vector3.one; // Ensure it displays correctly
    }
    public void emptyMsgList(Transform chatContent)
    {
        if (chatContent.childCount > 0)
        {
            foreach (Transform child in chatContent)
            {
                Destroy(child.gameObject);
            }
            Debug.Log("All child objects have been deleted.");
        }
        else
        {
            Debug.Log("No child objects to delete.");
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
                chatInitializer.UnreadMessages[msg.From] = chatInitializer.UnreadMessages.GetValueOrDefault(msg.From, 0) + 1;
                Debug.LogError("New Message from " + msg.From);
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
*/