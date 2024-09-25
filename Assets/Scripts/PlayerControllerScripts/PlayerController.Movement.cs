using UnityEngine;
using Fusion;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;

public partial class PlayerController : NetworkBehaviour
{
    private Vector2 _direction;
    private float lastClickTime = 0f;
    private const float DOUBLE_CLICK_TIME = 0.3f;
    private bool isTeleporting = false;
    private Vector3 _updatedPosition = Vector3.zero;
    private int clickCount = 0;
    public bool isTimeCheckAllowed = true;
    [Networked] private Vector3 _targetPosition { get; set; }
    [Networked] private NetworkBool _isTeleporting { get; set; }

    private void HandleMovement()
    {
        Vector2 moveDirection = Vector2.zero;

        if (GetInput(out NetworkInputData input))
        {
            moveDirection = input.directions;
        }

        if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
        {
            moveDirection.y = 0;
        }
        else
        {
            moveDirection.x = 0;
        }

        moveDirection = moveDirection.normalized;

        _rb.Rigidbody.velocity = moveDirection * moveSpeed;

        UpdateAnimator(moveDirection);

        if (Object.HasInputAuthority)
        {
            CheckForDoubleClick();
        }
    }

    private void UpdateAnimator(Vector2 moveDirection)
    {
        animator.SetFloat("Horizontal", moveDirection.x);
        animator.SetFloat("Vertical", moveDirection.y);
        animator.SetFloat("Speed", moveDirection.magnitude);

        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {
            animator.SetFloat("LastHorizontal", Input.GetAxisRaw("Horizontal"));
            animator.SetFloat("LastVertical", Input.GetAxisRaw("Vertical"));
        }
    }

    private void CheckForDoubleClick()
    {
        if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            HandleTouchForTeleport();
        }
        else
        {
            HandleMouseForTeleport();
        }
    }

    private void HandleTouchForTeleport()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended && touch.tapCount == 2)
            {
                TeleportPlayerToPosition(touch.position);
            }
        }
    }

    private void HandleMouseForTeleport()
    {
        if (Input.GetMouseButtonUp(0))
        {
            clickCount += 1;
        }
        if (clickCount == 1 && isTimeCheckAllowed)
        {
            lastClickTime = Time.time;
            StartCoroutine(DetectDoubleLeftClick());
        }
    }

    private System.Collections.IEnumerator DetectDoubleLeftClick()
    {
        isTimeCheckAllowed = false;
        while (Time.time < lastClickTime + 0.3f)
        {
            if (clickCount == 2)
            {
                TeleportPlayerToPosition(Input.mousePosition);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        clickCount = 0;
        isTimeCheckAllowed = true;
    }

    private void TeleportPlayerToPosition(Vector2 screenPosition)
    {
        if (IsPointerOverUIElement(screenPosition))
        {
            return;
        }

        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector2.zero, Mathf.Infinity, raycastMask);

        if (hits.Length > 0)
        {
            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
            RaycastHit2D hit = hits[0];
            int hitLayerIndex = hit.collider.gameObject.layer;
           
            if (hitLayerIndex == 8 || hitLayerIndex == 3)
            {
                RPC_RequestTeleport(hit.point);
            }
        }
    }

    private bool IsPointerOverUIElement(Vector2 screenPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.layer != 8 && result.gameObject.layer != 3)
            {
                return true;
            }
        }

        return false;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_RequestTeleport(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
        _isTeleporting = true;
    }
    private void HandleTeleportation()
    {
        if (_isTeleporting)
        {
            if (Object.HasStateAuthority)
            {
                Rpc_UpdateTeleportationState(true);
                Rpc_UpdateTargetIndicator();
            }
            else if (Object.HasInputAuthority)
            {
                Rpc_RequestUpdateTeleportationState(true);
                Rpc_RequestUpdateTargetIndicator();
            }

            

            Vector2 direction = _targetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.SetPositionAndRotation(
                Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Runner.DeltaTime),
                Quaternion.AngleAxis(angle - 45, Vector3.forward)
            );

            if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
            {
                EndTeleportation();
            }
        }
    }

    private void EndTeleportation()
    {
        _isTeleporting = false;
        if (Object.HasStateAuthority)
        {
            Rpc_UpdateTeleportationState(false);
            Rpc_UpdateTargetIndicator(false);
        }
        else if (Object.HasInputAuthority)
        {
            Rpc_RequestUpdateTeleportationState(false);
            Rpc_RequestUpdateTargetIndicator(false);
        }
        transform.rotation = Quaternion.identity;
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void Rpc_RequestUpdateTargetIndicator(bool show)
    {
        Rpc_UpdateTargetIndicator(show);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void Rpc_UpdateTargetIndicator(bool show)
    {
        if (show)
        {
            if (targetIndicator == null)
            {
                CreateTargetIndicator();
            }
            targetIndicator.transform.position = _targetPosition;
        }
        else
        {
            DestroyTargetIndicator();
        }
    }

    private void DestroyTargetIndicator()
    {
        if (targetIndicator != null)
        {
            Destroy(targetIndicator);
            targetIndicator = null;
        }
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void Rpc_RequestUpdateTeleportationState(bool isTeleporting)
    {
        Rpc_UpdateTeleportationState(isTeleporting);
    }
        [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void Rpc_UpdateTeleportationState(bool isTeleporting)
    {
       
        trigger.enabled = !isTeleporting;
        Rangetrigger.enabled = !isTeleporting;
        Roomtrigger.enabled = !isTeleporting;
        animator.enabled = !isTeleporting;
        range.GetComponent<SpriteRenderer>().enabled = !isTeleporting;
        TMP_Text nameText = GetComponentInChildren<TMP_Text>();
        nameText.enabled = !isTeleporting;
        
        if (isTeleporting)
        {
            if(Object.HasStateAuthority){
                Rpc_UpdateSprite(0); 
            }else if(Object.HasInputAuthority) { 
                Rpc_RequestUpdateSprite(0); 
            }

        }
        else
        {
            if (Object.HasStateAuthority) {
                Rpc_UpdateSprite(1);
            }
            else if (Object.HasInputAuthority) {
                Rpc_RequestUpdateSprite(1);
            }
        }


    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void Rpc_RequestUpdateSprite(int spriteIndex)
    {
        Rpc_UpdateSprite(spriteIndex);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
   private void Rpc_UpdateSprite(int spriteIndex)
    {
        _player.sprite = _sprites[spriteIndex];
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void Rpc_RequestUpdateTargetIndicator()
    {
        Rpc_UpdateTargetIndicator();
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void Rpc_UpdateTargetIndicator()
    {
        if (targetIndicator == null)
        {
            CreateTargetIndicator();
        }
        else
        {
            Destroy(targetIndicator);
            CreateTargetIndicator();
        }
    }
}