using UnityEngine;
using UnityEngine.UI;

public class CharacterSwitch : MonoBehaviour
{
    [SerializeField] private Image characterDisplay; // The UI Image displaying the character
    [SerializeField] private Sprite femaleSprite; // The female character sprite
    [SerializeField] private Sprite maleSprite; // The male character sprite
    [SerializeField] private Button switchButton; // The button to switch characters

    private bool isFemale = true; // Track the current character, default is female

    void Start()
    {
        // Set the default character to female
        characterDisplay.sprite = femaleSprite;

        // Add a listener to the button to handle switching characters
        switchButton.onClick.AddListener(SwitchCharacter);
    }

    void SwitchCharacter()
    {
        // Toggle the character
        isFemale = !isFemale;

        // Update the character sprite
        if (isFemale)
        {
            characterDisplay.sprite = femaleSprite;
        }
        else
        {
            characterDisplay.sprite = maleSprite;
        }
    }
}
