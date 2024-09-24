using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using System.IO;
using System;

[Serializable]
public class SaveData
{
    public int selectedCharacterIndex;
}

public class CharacterChoose : NetworkBehaviour
{
    //Assign the playerAnimator to the local player
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private RuntimeAnimatorController[] characterControllers;

    [Networked] public int selectedCharacterIndex { get; set; }

    private const string SaveFileName = "character_selection.json";

    public override void Spawned()
    {
        // if (Object.HasInputAuthority)
        // {
        //     LoadCharacterSelection();
        //     ShowCharacterSelectionUI();
        // }
        // else
        // {
        //     SetAnimatorController(selectedCharacterIndex);
        // }

        // Assign playerAnimator dynamically if the player has input authority
        if (Object.HasInputAuthority)
        {
            playerAnimator = GetComponent<Animator>();
            
            if (playerAnimator == null)
            {
                Debug.LogError("No Animator component found on the player object.");
            }

            LoadCharacterSelection();
            ShowCharacterSelectionUI();
        }
        else
        {
            SetAnimatorController(selectedCharacterIndex);
        }
    }

    public void OnCharacterSelected(int characterIndex)
    {
        if (Object.HasInputAuthority)
        {
            selectedCharacterIndex = characterIndex;
            SetAnimatorController(characterIndex);
            SaveCharacterSelection(characterIndex);
            Debug.Log("Character " + characterIndex + " selected.");
        }
        else{
            Debug.LogError("Only the local player can select a character.");
        }
    }

    private void SetAnimatorController(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < characterControllers.Length)
        {
            playerAnimator.runtimeAnimatorController = characterControllers[characterIndex];
        }
        else
        {
            Debug.LogError("Invalid character index selected.");
        }
    }

    private void SaveCharacterSelection(int characterIndex)
    {
        SaveData data = new SaveData { selectedCharacterIndex = characterIndex };
        string json = JsonUtility.ToJson(data);
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("Character saved with index: " + characterIndex);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save character selection: " + e.Message);
        }
    }

    private void LoadCharacterSelection()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                selectedCharacterIndex = data.selectedCharacterIndex;
                SetAnimatorController(selectedCharacterIndex);
                Debug.Log("Loaded character index: " + selectedCharacterIndex);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load character selection: " + e.Message);
                selectedCharacterIndex = 0;
                SetAnimatorController(0);
            }
        }
        else
        {
            Debug.Log("No saved character found. Defaulting to first character.");
            selectedCharacterIndex = 0;
            SetAnimatorController(0);
        }
    }

    private void ShowCharacterSelectionUI()
    {
        Button button1 = GameObject.Find("UI Canvas/CharacterSelectPanel/CharacterButton1").GetComponent<Button>();
        Button button2 = GameObject.Find("UI Canvas/CharacterSelectPanel/CharacterButton2").GetComponent<Button>();
        Button button3 = GameObject.Find("UI Canvas/CharacterSelectPanel/CharacterButton3").GetComponent<Button>();
        Button button4 = GameObject.Find("UI Canvas/CharacterSelectPanel/CharacterButton4").GetComponent<Button>();
        Button button5 = GameObject.Find("UI Canvas/CharacterSelectPanel/CharacterButton5").GetComponent<Button>();
        Button button6 = GameObject.Find("UI Canvas/CharacterSelectPanel/CharacterButton6").GetComponent<Button>();
        Button button7 = GameObject.Find("UI Canvas/CharacterSelectPanel/CharacterButton7").GetComponent<Button>();

        button1.onClick.AddListener(() => OnCharacterSelected(0));
        button2.onClick.AddListener(() => OnCharacterSelected(1));
        button3.onClick.AddListener(() => OnCharacterSelected(2));
        button4.onClick.AddListener(() => OnCharacterSelected(3));
        button5.onClick.AddListener(() => OnCharacterSelected(4));
        button6.onClick.AddListener(() => OnCharacterSelected(5));
        button7.onClick.AddListener(() => OnCharacterSelected(6));

        //Show a Debug if button0 is pressed
        button1.onClick.AddListener(() => Debug.Log("Button 0 pressed"));
    }

    private void Update()
    {
        if (Object.HasInputAuthority)
        {
            // Debug testing without UI using number keys to select characters
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                OnCharacterSelected(0); // Select the first character
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                OnCharacterSelected(1); // Select the second character
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                OnCharacterSelected(2); // Select the third character
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                OnCharacterSelected(3); // Select the second character
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                OnCharacterSelected(4); // Select the third character
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                OnCharacterSelected(5); // Select the second character
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                OnCharacterSelected(6); // Select the third character
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                OnCharacterSelected(7); // Select the second character
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                OnCharacterSelected(8); // Select the third character
            }
        }
    }
}
