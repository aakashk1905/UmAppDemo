using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInfoPrefabController : MonoBehaviour
{
    public TextMeshProUGUI textComponent; 
    public Button buttonComponent; 
    [SerializeField] NewChat newChat;

    private void Awake()
    {
        buttonComponent.onClick.AddListener(ReturnTextValue);
    }

    private void Start()
    {
        //buttonComponent.onClick.AddListener(ReturnTextValue);
    }

    private void ReturnTextValue()
    {
        string textValue = textComponent.text;
        Debug.Log("Button clicked! Text Value: " + textValue);
        Debug.Log( newChat.enabled ? "New chat enabled" : "New Chat Disabled");  
        newChat.recipient = textValue;
        newChat.LoadMessageHistory(textValue);
    }
}
