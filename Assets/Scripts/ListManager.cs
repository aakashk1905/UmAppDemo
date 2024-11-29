using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class PlayerListManager : NetworkBehaviour
{
    private static PlayerListManager _instance;
    public static PlayerListManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerListManager>();
            }
            return _instance;
        }
    }

    [Networked, Capacity(100),OnChangedRender(nameof(OnPlayerListChanged))]
    public NetworkLinkedList<PlayerInfo> playerInfoList { get; }

    public override void Spawned()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Runner.Despawn(Object);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestAddPlayerInfo(string name, string id)
    {
        if (Runner.IsServer) 
        {
            RPC_AddPlayerInfo(name, id); 
        }
        
    }
    /*[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestRemovePlayerInfo(string id)
    {
        Debug.LogError("REquesting to remove");
        if (Runner.IsServer)
        {
            RPC_RemovePlayerInfo(id);
        }

    }*/
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AddPlayerInfo(string name, string id)
    {
        foreach (var playerInfo in playerInfoList)
        {
            if (playerInfo.id == id)
            {
                return;
            }
        }
        playerInfoList.Add(new PlayerInfo { name = name, id = id });
        
    }


    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_RequestRemovePlayerInfo(string id)
    {
        if (Runner.IsServer)
        {
            RPC_RemovePlayerInfo(id);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_RemovePlayerInfo(string id)
    {
        Debug.LogError("Removing player info");
        PlayerInfo playerToRemove = default;
        bool found = false;
        foreach (var playerInfo in playerInfoList)
        {
            if (playerInfo.id.ToString() == id)
            {
                playerToRemove = playerInfo;
                found = true;
                break;
            }
        }
        if (found)
        {
            playerInfoList.Remove(playerToRemove);
            Debug.Log($"Player {id} removed successfully.");
        }
        else
        {
            Debug.LogWarning($"Player {id} not found in the list.");
        }
    }

    public void AddPlayerInfo(string name, string id)
    {
        if (Runner.IsServer)
        {
            RPC_AddPlayerInfo(name, id);
        }
        else
        {
            RPC_RequestAddPlayerInfo(name, id);
        }
        
    }


    public void RemovePlayerInfo(string id)
    {
        Debug.LogError(id);
        if (Runner.IsServer)
        {
            RPC_RemovePlayerInfo(id);
        }
        else
        {
            RPC_RequestRemovePlayerInfo(id);
        }
    }


    public void OnPlayerListChanged()
    {
        if (OnPlayerListUpdated != null)
        {
            OnPlayerListUpdated();
        }
    }

    public static event System.Action OnPlayerListUpdated;

    public List<PlayerInfo> GetPlayerInfoListAsList()
    {
        return new List<PlayerInfo>(playerInfoList);
    }
}

[System.Serializable]
public struct PlayerInfo : INetworkStruct
{
    public NetworkString<_32> name;
    public NetworkString<_32> id;

    public bool Equals(PlayerInfo other)
    {
        return name.ToString() == other.name.ToString() && id.ToString() == other.id.ToString();
    }

    public override bool Equals(object obj)
    {
        return obj is PlayerInfo other && Equals(other);
    }

    public override int GetHashCode()
    {
        return name.GetHashCode() ^ id.GetHashCode();
    }
}