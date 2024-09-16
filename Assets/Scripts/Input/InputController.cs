using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class InputController : NetworkBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Joystick joystick;
    private Canvas joystickCanvas;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Runner.AddCallbacks(this);

            // Find the Canvas and Joystick
            joystickCanvas = GetComponentInChildren<Canvas>();
            if (joystickCanvas != null)
            {
                joystick = joystickCanvas.GetComponentInChildren<Joystick>();
                if (joystick == null)
                {
                    Debug.LogError("Joystick not found in Canvas children!");
                }
            }
            else
            {
                Debug.LogError("Canvas not found in player prefab children!");
            }
        }
        else
        {
            Canvas nonLocalCanvas = GetComponentInChildren<Canvas>();
            if (nonLocalCanvas != null)
            {
                nonLocalCanvas.gameObject.SetActive(false);
            }
        }
    }


    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (Object.HasInputAuthority)
        {
           
            var data = new NetworkInputData();

            Vector2 direction = Vector2.zero;
           

            if (joystick != null)
            {
                direction.x = joystick.Horizontal;
                direction.y = joystick.Vertical;
            }



            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                direction += Vector2.up;

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                direction += Vector2.down;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                direction += Vector2.left;

            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                direction += Vector2.right;

            if (direction.magnitude > 1)
                direction.Normalize();

            data.directions = direction;
            input.Set(data);

        }
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
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

    public void OnDisconnectedFromServer(NetworkRunner runner)
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

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }
}