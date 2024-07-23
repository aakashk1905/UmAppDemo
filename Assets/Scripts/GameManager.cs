using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private TMP_InputField nameInputField;
    internal int _playername;

    private void Awake()
    {
        instance = this;
    }
    public void StoreName()
    {
        int.TryParse(nameInputField.text, out _playername);
    }
}
