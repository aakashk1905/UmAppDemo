using UnityEngine;
using Fusion;
using TMPro;

public partial class PlayerController : NetworkBehaviour
{
    private GameObject targetIndicator;

    private void InitializeVisuals()
    {
    }

    private void RenderName(string name)
    {
        GameObject textObject = new GameObject("TextMeshPro");
        textObject.transform.SetParent(NameListCanvas.transform, false);
        TextMeshProUGUI textMeshPro = textObject.AddComponent<TextMeshProUGUI>();
        textMeshPro.text = name;
        textMeshPro.fontSize = 18;
        textMeshPro.alignment = TextAlignmentOptions.Center;
        textMeshPro.enableAutoSizing = true;
    }

    private void CreateTargetIndicator()
    {
        targetIndicator = new GameObject("TeleportTargetIndicator");
        SpriteRenderer spriteRenderer = targetIndicator.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = _sprites[2];
        targetIndicator.transform.position = _targetPosition;
        targetIndicator.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
    }

    public void OnNameChanged()
    {
        if (!string.IsNullOrEmpty(PlayerName.Value))
        {
            transform.name = PlayerName.Value;
            TMP_Text nameText = GetComponentInChildren<TMP_Text>();
            if (nameText != null)
            {
                nameText.text = PlayerName.Value;
            }
            else
            {
                Debug.LogError("TMP_Text component not found on player object.");
            }
            
                StartCoroutine(WaitForPlayerListManager());
            
        }
    }
   

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void Rpc_UpdatePlayerSpriteTeleport(bool teleporting)
    {
        Rpc_UpdatePlayerSprite(teleporting);
    }
}