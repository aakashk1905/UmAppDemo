using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInfoPrefabController : MonoBehaviour
{
    public TextMeshProUGUI textComponent; 
    public Button buttonComponent; 
    [SerializeField] public NewChat newChat;
    private ChatInitializer chatInitializer;

    private void Awake()
    {
        newChat = FindAnyObjectByType<NewChat>();
        chatInitializer = FindAnyObjectByType<ChatInitializer>();
        buttonComponent.onClick.AddListener(ReturnTextValue);
    }

    private void Update()
    {
        UpdateAllMessageIndicators();
    }

    private void ReturnTextValue()
    {
        string textValue = textComponent.text;
        newChat.recipient = textValue;
        newChat.Load();
    }

    public void UpdateAllMessageIndicators()
    {
        foreach (var entry in chatInitializer.UnreadMessages)
        {
            string email = entry.Key;
            int newMsgCount = entry.Value;

            newMsgIndicator(email, newMsgCount);
        }
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
}
