using UnityEngine;
using Fusion;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public partial class PlayerController : NetworkBehaviour
{
    private GameObject targetIndicator;
 

    private void InitializeVisuals()
    {
    }

    private void RenderName(string name, string email)
    {
        string currentUserEmail = UserDataManager.Instance.GetUserEmail();
        string currentUsername = currentUserEmail?.Split('@')[0];

        Debug.LogError(email + "email full" + currentUsername);

        GameObject PlayerInfoForList = Instantiate(playerInfoPrefab, NameListCanvas.transform);
        TextMeshProUGUI[] children = PlayerInfoForList.GetComponentsInChildren<TextMeshProUGUI>(true);
        PlayerInfoForList.name = email;

        Button teleportBtn = PlayerInfoForList.transform.Find("TeleportBtn").GetComponent<Button>();

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

                if (email == currentUsername)
                {
                    btnMsg.gameObject.SetActive(false);
                    teleportBtn.gameObject.SetActive(false);

                }

                btnMsg.onClick.RemoveAllListeners();
                btnMsg.onClick.AddListener(uiManager.EnableDmPage);
                
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