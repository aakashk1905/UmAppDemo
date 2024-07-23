using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion.Sockets;
using Unity.Mathematics;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    [SerializeField] private GameObject _player;
    private NetworkObject localPlayer;

    public void Host()
    {
        StartGame(GameMode.Shared);
    }


    async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "1234",
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (player == runner.LocalPlayer)
        {
            localPlayer = _runner.Spawn(_player, GenerateRandomPosition(), Quaternion.identity);
            StartCoroutine(DelayedNameSetting());
        }
    }

    private IEnumerator DelayedNameSetting()
    {
       
        yield return new WaitForSeconds(0.1f);
        RPC_SettingName();
    }

    [Rpc]
    private void RPC_SettingName()
    {
        localPlayer.name = "Player" + UnityEngine.Random.Range(1, 1000);
    }


    public static Vector2 GenerateRandomPosition()
    {
        Vector2 min = new Vector2(30, 15);
        Vector2 max = new Vector2(-30, -15);
        float randomX = UnityEngine.Random.Range(min.x, max.x);
        float randomY = UnityEngine.Random.Range(min.y, max.y);
        return new Vector2(randomX, randomY);
    }
    public void Exit()
    {
        Application.Quit();
    }
    #region UnwantedCallBacks
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        PlayerController _player = localPlayer.GetComponent<PlayerController>();
        foreach (PlayerController playerController in _player.neighbours)
        {
            playerController.HandleOnTriggerExit(_player);
            _player.HandleOnTriggerExit(playerController);
        }
        runner.Despawn(localPlayer);
    }


    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }
    #endregion
}
