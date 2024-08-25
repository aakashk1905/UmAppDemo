using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;
using TMPro;

using WebSocketSharp;
using System;

using System.Linq;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private NetworkRigidbody2D _rb;
    [SerializeField] public Sprite[] _sprites;

    public NetworkTableManager networkTableManager;
    public NetworkedDSU _networkedDSU;


    public bool IamBridge = false;
    private ChangeDetector _changeDetector;
    public string PlayerName;
    [Networked, OnChangedRender(nameof(OnPlayerIdChanged))]
    public int _playerID { get; set; }
    [Networked, OnChangedRender(nameof(OnChannelChanged))]
    public NetworkString<_128> _channelName { get; set; } = "";
    public string myChannel  = "";
    public string prevChannel  = "";
    public string _token = "";
    public bool isInChannel = false;
    public SpriteRenderer _player;
    private Vector2 _direction;
    public List<PlayerRef> neighbours;
    public AgoraManager _agoraManager;
    public Dictionary<string, string> tokens = new Dictionary<string, string>();

    public override void Spawned()
    {
        _player = GetComponent<SpriteRenderer>();
        if (Object.HasStateAuthority)
        {
            if (_playerID == 0)
            {
                int id = UnityEngine.Random.Range(0, 1000);
                RPC_SetNickname(id);
            }
           
        }

        transform.name = "Player" + _playerID;
        GetComponentInChildren<TMP_Text>().text = "Player" + _playerID;

        _agoraManager = AgoraManager.Instance;
        _agoraManager = AgoraManager.Instance;
        _rb = GetComponent<NetworkRigidbody2D>();
        _changeDetector = new ChangeDetector();
        _channelName = "";
        networkTableManager = NetworkTableManager.Instance;
        _networkedDSU = NetworkedDSU.Instance;
        if (Object.HasStateAuthority)
        {
            if (_networkedDSU != null)
            {
                _networkedDSU.MakeSet(Object.InputAuthority);
            }
            else
            {
                Debug.LogError("NetworkedDSU is not initialized!");
            }
        }

    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            _rb.Rigidbody.velocity = input.directions * moveSpeed;
        }
    }

    public void OnPlayerIdChanged()
    {
        if (_playerID != 0)
        {
            transform.name = "Player" + _playerID;
            GetComponentInChildren<TMP_Text>().text = "Player" + _playerID;
        }
    }

    public void OnChannelChanged()
    {
        if (Object.HasInputAuthority)
        {
            prevChannel = myChannel;
            myChannel = _channelName.Value;
            Debug.LogError(_channelName.Value + " =====" + _playerID + " ==========" + isInChannel);
            if (!_channelName.Value.IsNullOrEmpty() && _channelName.Value != "dummy" && !isInChannel)
            {
                _agoraManager.AddPlayerToChannel(_channelName.Value, this);
                if (Object.HasStateAuthority)
                {
                    networkTableManager.Rpc_UpdateNetworkTable("add", _channelName.Value, this.Object.InputAuthority);
                }
                else
                {
                    Rpc_RequestTableUpdate("add", _channelName.Value, this.Object.InputAuthority);
                }
               
            }
            Runner.StartCoroutine(LogNetworkTableWithDelay());

        }
    }
    private IEnumerator LogNetworkTableWithDelay()
    {
        yield return new WaitForSeconds(1f);
        LogNetworkTable();
    }

    private bool IsnullorDummy(String name)
    {
      
        return String.IsNullOrEmpty(name) || name == "dummy";
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

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void Rpc_RequestTableUpdate(string action, string channelName,PlayerRef player)
    {
        HandleTableUpdate(action, channelName, player);
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
    public void HandleTableUpdate(string action, string channel, PlayerRef player)
    {
        networkTableManager.Rpc_UpdateNetworkTable(action, channel, player);
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


    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    private void Rpc_UpdateChannelsAfterUnion(PlayerController otherPlayer)
    {
        if (IsnullorDummy(_channelName.Value) && IsnullorDummy(otherPlayer._channelName.Value))
        {
            string newChannel = _agoraManager.GenerateChannelName("" + _playerID);
            Rpc_SetChannelName(newChannel);
        }
        else if (IsnullorDummy(_channelName.Value) && !IsnullorDummy(otherPlayer._channelName.Value))
        {
            Rpc_SetChannelName(otherPlayer._channelName.Value);
        }
        else if (!IsnullorDummy(_channelName.Value) && !IsnullorDummy(otherPlayer._channelName.Value) &&
            _channelName.Value != otherPlayer._channelName.Value)
        {
            RpcMergeChannels(_channelName.Value, otherPlayer._channelName.Value);
        }

        _player.sprite = _sprites[1];
    
    }


    private void HandleCollisionExit(PlayerController otherPlayer)
    {
        Debug.LogError($"Exit calledddd {_playerID}  {otherPlayer._playerID}");
        RPC_HandleOnTriggerExit(otherPlayer);
        
    }


    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_JoinChannelWithPlayer(PlayerController otherPlayer)
    {
        neighbours.Add(otherPlayer.Object.InputAuthority);
        Debug.LogError(_playerID + " "+neighbours.Count);
        if (IsnullorDummy(_channelName.Value) && IsnullorDummy(otherPlayer._channelName.Value))
        {
            Rpc_SetChannelName(_agoraManager.GenerateChannelName("" + _playerID));
        }
        else if (IsnullorDummy(_channelName.Value) && !IsnullorDummy(otherPlayer._channelName.Value))
        {
            Rpc_SetChannelName(otherPlayer._channelName.Value);
        }
        else if (!IsnullorDummy(_channelName.Value) && !IsnullorDummy(otherPlayer._channelName.Value) &&
            _channelName.Value != otherPlayer._channelName.Value)
        {
            IamBridge = true;
            if (_agoraManager.Bridges.TryGetValue(_channelName.Value, out int currentValue))
            {

                _agoraManager.Bridges[_channelName.Value] = currentValue + 1;
            }
            else
            {
                _agoraManager.Bridges.Add(_channelName.Value, 1);
            }
            RpcMergeChannels(_channelName.Value, otherPlayer._channelName.Value);
        }

        _player.sprite = _sprites[1];
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

   
    private void ReorganizeGroupAfterExit(PlayerController exitingPlayer)
    {
        if (_networkedDSU == null)
        {
            Debug.LogError("NetworkedDSU is not initialized!");
            return;
        }

        string currentChannel = _channelName.Value;
        _networkedDSU.DisconnectPlayer(exitingPlayer.Object.InputAuthority);

        if (neighbours.Count == 0)
        {
            HandleChannelChange(this, currentChannel, "");
            return;
        }


        // Get the updated groups
        var groups = _networkedDSU.GetCurrentGroups();



        if (groups.Count == 1)
        {
            return;
        }

        var largestGroup = groups.Values.OrderByDescending(g => g.Count).First();
        foreach (var group in groups.Values)
        {
            if (group == largestGroup)
            {
                continue;
            }

            string newChannel = "Split" + group.First();
            foreach (var playerRef in group)
            {
                PlayerController playerController = Runner.GetPlayerObject(playerRef).GetComponent<PlayerController>();
                HandleChannelChange(playerController, currentChannel, newChannel);
            }
        }

        UpdatePlayerSprite();
        LogNetworkTable();
    }


    private void HandleChannelChange(PlayerController player, string oldChannel, string newChannel)
    { 
        networkTableManager.Rpc_UpdateNetworkTable("remove", oldChannel, player.Object.InputAuthority);
        if(!string.IsNullOrEmpty(newChannel))
            networkTableManager.Rpc_UpdateNetworkTable("add", newChannel, player.Object.InputAuthority);
        player.Rpc_LeaveChannel(player.Object.InputAuthority, oldChannel);
        player.Rpc_SetIsInChannel(false);
        player.Rpc_SetChannelName(newChannel);
    }


    
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void Rpc_LeaveChannel(PlayerRef playerRef, string channel)
    {
        if (Object.HasInputAuthority)
        {
            PlayerController player = Runner.GetPlayerObject(playerRef).GetComponent<PlayerController>();
            _agoraManager.LeaveChannel(player, channel);
        }
    }

    private void UpdatePlayerSprite()
    {
        Rpc_UpdatePlayerSprite(neighbours.Count <= 0);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void Rpc_UpdatePlayerSprite(bool isAlone)
    {
        _player.sprite = isAlone ? _sprites[0] : _sprites[1];
    }


    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RpcMergeChannels(string channel1, string channel2)
    {
        Debug.LogError("Merging channels: " + channel1 + " and " + channel2);
        Dictionary<string, List<PlayerRef>> networkTable = networkTableManager.GetNetworkTableAsDictionary();

        LogNetworkTable();

        if (!networkTable.ContainsKey(channel1) || !networkTable.ContainsKey(channel2)) return;

        List<PlayerRef> channel1Players = networkTable[channel1];
        List<PlayerRef> channel2Players = networkTable[channel2];
        string targetChannel = channel1Players.Count >= channel2Players.Count ? channel1 : channel2;
        string sourceChannel = channel1Players.Count >= channel2Players.Count ? channel2 : channel1;

        Debug.LogError($"Target channel: {targetChannel}, Source channel: {sourceChannel}");

        List<PlayerRef> playersToMove = networkTable[sourceChannel];

        foreach (var playerRef in playersToMove)
        {
            networkTableManager.Rpc_UpdateNetworkTable("remove", sourceChannel, playerRef);
            PlayerController player = Runner.GetPlayerObject(playerRef).GetComponent<PlayerController>();
            player.Rpc_LeaveChannel(playerRef, sourceChannel);
            player.Rpc_SetIsInChannel(false);
            player.Rpc_SetChannelName(targetChannel);
        }

    }

    public void LogNetworkTable()
    {
        Dictionary<string, List<PlayerRef>> networkTable = networkTableManager.GetNetworkTableAsDictionary();
        Debug.Log("Network Table:"+ networkTable.Count);
        foreach (var kvp in networkTable)
        {
            Debug.LogError($"Channel: {kvp.Key}");
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                PlayerController player = Runner.GetPlayerObject(kvp.Value[i]).GetComponent<PlayerController>();
                Debug.LogError($"  Player {i + 1}: {player._playerID}");
            }
        }
    }
    public string GetChannelName() { return _channelName.Value; }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void Rpc_SetChannelName(string name)
    {
        _channelName = name;
    }

    public string GetToken() { return _token; }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RpcSetToken(string newToken)
    {
        _token = newToken;
    }

    public int GetPlayerId() { return _playerID; }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_SetNickname(int nick)
    {
        _playerID = nick;
    }
    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void Rpc_SetIsInChannel(bool value)
    {
        isInChannel = value;
    }

}