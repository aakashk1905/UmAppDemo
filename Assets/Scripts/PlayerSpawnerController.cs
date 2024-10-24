using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSp : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;
    private Dictionary<PlayerRef, NetworkObject> currentSpawnedPlayers = new Dictionary<PlayerRef, NetworkObject>();
    private string roomName;

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            roomName = GetRoomFromURL();

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
            GameObject roomObject = GameObject.Find(roomName);
            Vector3 spawnPosition = Vector3.zero;

            if (roomObject != null)
            {
                spawnPosition = roomObject.transform.position; 
            }
           

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

   
    string GetRoomFromURL()
    {
        string roomName = "defaultRoom";

#if UNITY_WEBGL
        string url = Application.absoluteURL;
        roomName = ParseRoomFromURL(url);
#elif UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
        roomName = GetPlatformSpecificRoomName(); 
#endif
        return roomName;
    }

    string ParseRoomFromURL(string url)
    {
        Uri myUri = new Uri(url);
        string room = System.Web.HttpUtility.ParseQueryString(myUri.Query).Get("room");
        return room ?? "defaultRoom";
    }

    string GetPlatformSpecificRoomName()
    {
        string roomName = "defaultRoom"; // Default fallback if no room is found

#if UNITY_IOS || UNITY_ANDROID
       
        if (!string.IsNullOrEmpty(Application.absoluteURL))
        {
            roomName = ParseRoomFromURL(Application.absoluteURL); 
        }
        else
        {
            Debug.LogWarning("No deep link URL found, using default room.");
        }

#elif UNITY_STANDALONE

    string[] args = System.Environment.GetCommandLineArgs();
    foreach (string arg in args)
    {
        if (arg.StartsWith("--room="))
        {
            roomName = arg.Substring(7); 
            break;
        }
    }
    if (roomName == "defaultRoom")
    {
        Debug.LogWarning("No command-line argument found for room, using default room.");
    }

#else
    Debug.LogWarning("Unsupported platform, using default room.");
#endif

        return roomName;
    }
}
