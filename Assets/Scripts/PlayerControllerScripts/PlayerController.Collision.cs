using UnityEngine;
using Fusion;
using System.Collections.Generic;

public partial class PlayerController : NetworkBehaviour
{
    public BoxCollider2D trigger;
    private CircleCollider2D Rangetrigger;
    private CircleCollider2D Roomtrigger;
    public List<PlayerRef> neighbours = new List<PlayerRef>();
    public GameObject range;

    private void InitializeCollision()
    {
        trigger = GetComponent<BoxCollider2D>();
        range = transform.Find("Range").gameObject;
        Transform roomTransform = transform.Find("RoomTrigger");
        Roomtrigger = roomTransform.GetComponent<CircleCollider2D>();
        if (range != null)
        {
            Rangetrigger = range.GetComponent<CircleCollider2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            if (otherPlayer != null)
            {
                if (Object.HasStateAuthority)
                {
                    HandleCollisionEnter(otherPlayer);
                }
                else
                {
                    RPC_RequestHandleCollisionEnter(otherPlayer.Object.InputAuthority);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            if (otherPlayer != null)
            {
                if (Object.HasStateAuthority)
                {
                    HandleCollisionExit(otherPlayer);
                }
                else
                {
                    RPC_RequestHandleCollisionExit(otherPlayer.Object.InputAuthority);
                }
            }
        }
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_RequestHandleCollisionEnter(PlayerRef otherPlayerRef)
    {
        PlayerController otherPlayer = Runner.GetPlayerObject(otherPlayerRef).GetComponent<PlayerController>();
        HandleCollisionEnter(otherPlayer);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_RequestHandleCollisionExit(PlayerRef otherPlayerRef)
    {
        PlayerController otherPlayer = Runner.GetPlayerObject(otherPlayerRef).GetComponent<PlayerController>();
        HandleCollisionExit(otherPlayer);
    }

    private void HandleCollisionEnter(PlayerController otherPlayer)
    {
        if (!neighbours.Contains(otherPlayer.Object.InputAuthority))
        {
            neighbours.Add(otherPlayer.Object.InputAuthority);

            if (_networkedDSU != null)
            {
                _networkedDSU.Union(Object.InputAuthority, otherPlayer.Object.InputAuthority);
                Rpc_UpdateChannelsAfterUnion(otherPlayer);
            }
            else
            {
                Debug.LogError("NetworkedDSU is not initialized!");
            }
        }
    }

    private void HandleCollisionExit(PlayerController otherPlayer)
    {
        RPC_HandleOnTriggerExit(otherPlayer);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_HandleOnTriggerExit(PlayerController otherPlayer)
    {
        if (neighbours.Contains(otherPlayer.Object.InputAuthority))
        {
            neighbours.Remove(otherPlayer.Object.InputAuthority);

            if (_networkedDSU != null)
            {
                ReorganizeGroupAfterExit(otherPlayer);
            }
            else
            {
                Debug.LogError("NetworkedDSU is not initialized!");
            }
        }

        UpdatePlayerSprite();
    }

    private void UpdatePlayerSprite()
    {
        Rpc_UpdatePlayerSprite(neighbours.Count <= 0);
    }
}