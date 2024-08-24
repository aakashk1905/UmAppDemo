using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarSelectionManager : MonoBehaviour
{
    public Image avatarPreview; 
    public TMP_InputField playerNameInput; 
    public Button saveButton;
    public Sprite defaultAvatar;
    private Image selectedAvatar; 

    void Start()
    {
       if (defaultAvatar != null)
        {
            avatarPreview.sprite = defaultAvatar;
        }
        foreach (Transform avatarOption in transform.Find("AvatarGrid"))    
        {
            Button avatarButton = avatarOption.GetComponent<Button>();
            avatarButton.onClick.AddListener(() => OnAvatarSelected(avatarButton));
        }
        saveButton.onClick.AddListener(SaveAvatarSelection);
    }

    void OnAvatarSelected(Button avatarButton)
    {
        selectedAvatar = avatarButton.GetComponent<Image>();
        avatarPreview.sprite = selectedAvatar.sprite;
    }

    void SaveAvatarSelection()
    {
        string playerName = playerNameInput.text;
        if (selectedAvatar != null && !string.IsNullOrEmpty(playerName))
        {
            Debug.Log($"Player Name: {playerName}, Selected Avatar: {selectedAvatar.sprite.name}");
        }
        else
        {
            Debug.LogWarning("Please select an avatar and enter a name.");
        }
    }
}
