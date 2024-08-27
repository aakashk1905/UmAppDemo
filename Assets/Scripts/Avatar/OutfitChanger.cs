using UnityEngine;

public class OutfitChanger : MonoBehaviour
{
    public GameObject playerPrefab; // Reference to the player prefab
    public Sprite[] hoodOptions; // Array to store different hood sprites
    public Sprite[] ShirtOptions;
    public Sprite[] faceOptions;
    public Sprite[] pantsOptions;

    public SpriteRenderer hoodRenderer; // Reference to the SpriteRenderer for the hood
    public SpriteRenderer shirtRenderer;
    public SpriteRenderer faceRenderer;
    public SpriteRenderer pantsRenderer;


    private void Start()
    {
        // Ensure the hoodRenderer is set to the correct child object in the player prefab
        if (playerPrefab != null)
        {
            hoodRenderer = playerPrefab.transform.Find("Hood").GetComponent<SpriteRenderer>();
        }
    }

    // This method is triggered when a hood button is clicked
    public void ChangeHood(int hoodIndex)
    {
        if (hoodIndex >= 0 && hoodIndex < hoodOptions.Length && hoodRenderer != null)
        {
            hoodRenderer.sprite = hoodOptions[hoodIndex];
        }
    }
    public void ChangeShirt(int hoodIndex)
    {
        if (hoodIndex >= 0 && hoodIndex < hoodOptions.Length && hoodRenderer != null)
        {
            hoodRenderer.sprite = hoodOptions[hoodIndex];
        }
    }
     public void Changeface(int hoodIndex)
    {
        if (hoodIndex >= 0 && hoodIndex < hoodOptions.Length && hoodRenderer != null)
        {
            hoodRenderer.sprite = hoodOptions[hoodIndex];
        }
    }
     public void Changepants(int hoodIndex)
    {
        if (hoodIndex >= 0 && hoodIndex < hoodOptions.Length && hoodRenderer != null)
        {
            hoodRenderer.sprite = hoodOptions[hoodIndex];
        }
    }
}
