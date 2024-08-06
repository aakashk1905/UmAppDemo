using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSp : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;
    private Dictionary<PlayerRef, NetworkObject> currentSpawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            foreach (var item in Runner.ActivePlayers)
            {
                if (item != Runner.LocalPlayer)

                {
                    SpawnPlayer(item);
                }

            }
        }
    }

    public void AddToEntry(PlayerRef player, NetworkObject obj)
    {
        currentSpawnedPlayers.TryAdd(player, obj);
    }


    private void SpawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            var playerObject = Runner.Spawn(playerNetworkPrefab, Vector3.zero, Quaternion.identity, playerRef);
            Runner.SetPlayerObject(playerRef, playerObject);
            AddToEntry(playerRef, playerObject);
        }
    }
    private void DeSpawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            if (currentSpawnedPlayers.TryGetValue(playerRef, out var playerNetworkObject))
            {
                Runner.Despawn(playerNetworkObject);
            }
        }
        Runner.SetPlayerObject(playerRef, null);
    }
    public void PlayerJoined(PlayerRef player)
    {
        SpawnPlayer(player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        DeSpawnPlayer(player);
    }
}
