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
    public PlayerController playerController;
    private Animator playerAnimator;
    [SerializeField] private RuntimeAnimatorController[] characterControllers;

    [Networked] public int selectedCharacterIndex { get; set; }

    private const string SaveFileName = "character_selection.json";

    public override void Spawned()
    {
            LoadCharacterSelection();
           //ShowCharacterSelectionUI();
        
    }

    public override void FixedUpdateNetwork()
    {
       if(playerController == null)
        {
            playerController = PlayerController.Instance;
            playerAnimator = playerController.animator;
            SetAnimatorController(selectedCharacterIndex);
        }
    }

    public void OnCharacterSelected(int characterIndex)
    {
       
            selectedCharacterIndex = characterIndex;
            SetAnimatorController(characterIndex);
            SaveCharacterSelection(characterIndex);
            Debug.Log("Character " + characterIndex + " selected.");
       
    }

    private void SetAnimatorController(int characterIndex)
    {
        if (playerAnimator!=null)
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

   /* private void ShowCharacterSelectionUI()
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
    }*/

}
