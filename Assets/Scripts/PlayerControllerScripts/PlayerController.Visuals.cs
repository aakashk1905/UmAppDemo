using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;

public partial class PlayerController : NetworkBehaviour
{
    private GameObject targetIndicator;
 

    private void InitializeVisuals()
    {
    }

    private void RenderName(string name, string email)
    {
        
        /*NewChat newChat = new NewChat();
        newChat.recipient = email;*/
        GameObject PlayerInfoForList = Instantiate(playerInfoPrefab, NameListCanvas.transform);
        TextMeshProUGUI[] children = PlayerInfoForList.GetComponentsInChildren<TextMeshProUGUI>(true);
        PlayerInfoForList.name = email;

        foreach (TextMeshProUGUI child in children)
        {
            if (child.CompareTag("PlayerName"))
            {
                child.text = name;
                break;
            }
        }

        Button[] buttons = PlayerInfoForList.GetComponentsInChildren<Button>(true);
        bool listenerAdded = false;

        foreach (Button btnMsg in buttons)
        {
            if (btnMsg.CompareTag("msgBtn"))
            {
                btnMsg.onClick.RemoveAllListeners();
                btnMsg.onClick.AddListener(uiManager.EnableDmPage);
                
               /* Debug.Log("Listener added to button with tag 'msgBtn'");*/
                listenerAdded = true;
                break;
            }
        }

        if (!listenerAdded)
        {
            Debug.LogWarning("Button with tag 'msgBtn' not found or listener not added.");
        }

        PlayerInfoForList.transform.Find("Email").GetComponent<TextMeshProUGUI>().text = email;
        //PlayerName.text = playerInfo.name.Value;
        PlayerInfoForList.SetActive(true);
    }

    public void OnTextClicked(string s)
    {
        if (s != UserDataManager.Instance.GetUserEmail().Split("@")[0])
        {
            Debug.LogError(s);
        }
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