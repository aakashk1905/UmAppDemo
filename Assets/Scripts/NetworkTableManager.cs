using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTableManager : NetworkBehaviour
{

    private static NetworkTableManager _instance;
    public static NetworkTableManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkTableManager>();
            }
            return _instance;
        }
    }

    [Networked,Capacity(20)]
    public NetworkDictionary<string, NetworkedPlayerRefList> NetworkTable { get; }

    public override void Spawned()
    {
        _instance = this;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void Rpc_UpdateNetworkTable(string action, string channel, PlayerRef playerRef)
    {
        if (action == "add")
        {
            if (!NetworkTable.TryGet(channel, out var playerList))
            {
                playerList = new NetworkedPlayerRefList();
            }

            if (!playerList.Contains(playerRef))
            {
                playerList.Add(playerRef);
                NetworkTable.Set(channel, playerList);
            }
        }
        else if (action == "remove")
        {
            if (NetworkTable.TryGet(channel, out var playerList))
            {
                playerList.Remove(playerRef);
                if (playerList.Count == 0)
                {
                    NetworkTable.Remove(channel);
                }
                else
                {
                    NetworkTable.Set(channel, playerList);
                }
            }
        }
    }
    public Dictionary<string, List<PlayerRef>> GetNetworkTableAsDictionary()
    {
        var result = new Dictionary<string, List<PlayerRef>>();
        foreach (var kvp in NetworkTable)
        {
            result[kvp.Key] = kvp.Value.ToList();
        }
        return result;
    }
}