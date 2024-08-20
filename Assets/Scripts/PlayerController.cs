/*using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using WebSocketSharp;
using System;
using ExitGames.Client.Photon.StructWrapping;


public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private NetworkRigidbody2D _rb;
    [SerializeField] public Sprite[] _sprites;

    public bool IamBridge = false;
    private ChangeDetector _changeDetector;

    public NetworkString<_16> PlayerName { get; set; }
    [Networked, OnChangedRender(nameof(OnPlayerIdChanged))]
    public int _playerID { get; set; }
    [Networked, OnChangedRender(nameof(OnChannelChanged))]
    public NetworkString<_128> _channelName { get; set; } = "dummy";
    public string myChannel = "";
    public string prevChannel = "";
    public string _token;
    public bool isInChannel = false;
    public SpriteRenderer _player;
    private Vector2 _direction;
    public List<PlayerController> neighbours = new List<PlayerController>();
    public AgoraManager _agoraManager;
    public Dictionary<string, string> tokens = new Dictionary<string, string>();
    public override void Spawned()
    {
        _player = GetComponent<SpriteRenderer>();
        if (Object.HasInputAuthority)
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
        _rb = GetComponent<NetworkRigidbody2D>();
        _changeDetector = new ChangeDetector();
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
        
            prevChannel = myChannel;
            myChannel = _channelName.Value;
            Debug.LogError(_channelName.Value + " =====" + _playerID);
            if (!_channelName.Value.IsNullOrEmpty() && _channelName.Value != "dummy" && !isInChannel)
            {
                _agoraManager.AddPlayerToChannel(_channelName.Value, this);
            }
        
    }
    private bool isnullorDummy(String name)
    {
        return name.IsNullOrEmpty() || name == "dummy";
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !neighbours.Contains(collision.gameObject.GetComponent<PlayerController>()))
        {
            if (Object.HasStateAuthority)
            {
                PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
                RPC_JoinChannelWithPlayer(otherPlayer);
            }
            else
            {
                PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
                RPC_RequestJoinChannelWithPlayer(otherPlayer);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (Object.HasStateAuthority)
            {
                PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
                RPC_HandleOnTriggerExit(otherPlayer);
            }
            else
            {
                PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
                RPC_RequestHandleOnTriggerExit(otherPlayer);
            }
        }
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_RequestJoinChannelWithPlayer(PlayerController otherPlayer)
    {
        RPC_JoinChannelWithPlayer(otherPlayer);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_RequestHandleOnTriggerExit(PlayerController otherPlayer)
    {
        RPC_HandleOnTriggerExit(otherPlayer);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void RPC_JoinChannelWithPlayer(PlayerController otherPlayer)
    {
        if (Object.HasInputAuthority)
        {
            neighbours.Add(otherPlayer);
            Debug.LogError("Collision happened" + _playerID + otherPlayer._playerID);

            if (isnullorDummy(_channelName.Value) && isnullorDummy(otherPlayer._channelName.Value))
            {
                Rpc_SetChannelName(_agoraManager.GenerateChannelName("" + _playerID));

            }
            else if (isnullorDummy(_channelName.Value) && !isnullorDummy(otherPlayer._channelName.Value))
            {
                Rpc_SetChannelName(otherPlayer._channelName.Value);
            }
            else if (!isnullorDummy(_channelName.Value) && !isnullorDummy(otherPlayer._channelName.Value))
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
                _agoraManager.MergeChannels(_channelName.Value, otherPlayer._channelName.Value);
            }
        }
        
        _player.sprite = _sprites[1];
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_HandleOnTriggerExit(PlayerController otherPlayer)
    {
        Debug.LogError("lving === " + _playerID);
        bool change = false;
        if (Object.HasInputAuthority)
        {
            if (neighbours.Contains(otherPlayer))
                neighbours.Remove(otherPlayer);

            if (neighbours.Count <= 0)
            {
                string channel = _agoraManager._channelName;
                _agoraManager.LeaveChannel(this);
                _agoraManager.Rpc_UpdateNetworkTable("remove", channel, this);
                change = true;
            }
            else
            {
                string channel = myChannel;
                if (IamBridge)
                {
                    IamBridge = false;
                    if (_agoraManager.Bridges.TryGetValue(_channelName.Value, out int currentValue))
                    {
                        _agoraManager.Bridges[_channelName.Value] = currentValue - 1;
                        if (currentValue == 1)
                        {
                            HashSet<PlayerController> processedPlayers = new HashSet<PlayerController>();
                            ChangeChannelForPlayerAndNeighboursRecursively(otherPlayer, processedPlayers);
                        }
                    }
                }
            }
        }
        if(change)
           _player.sprite = _sprites[0];

    }

    private void ChangeChannelForPlayerAndNeighboursRecursively(PlayerController otherPlayer, HashSet<PlayerController> processedPlayers)
    {
        processedPlayers.Add(otherPlayer);
        string tempPreChannel = otherPlayer.prevChannel;
        _agoraManager.LeaveChannel(otherPlayer);
        otherPlayer._channelName = tempPreChannel;

        foreach (PlayerController neighbour in otherPlayer.neighbours)
        {
            if (!processedPlayers.Contains(neighbour))
            {
                ChangeChannelForPlayerAndNeighboursRecursively(neighbour, processedPlayers);
            }
        }
    }

    public string GetChannelName() { return _channelName.Value; }

    public void Rpc_SetChannelName(string name)
    {
        _channelName = name;
        Debug.LogError("Change Channel called");
    }


    public string GetToken() { return _token; }
    public void RpcSetToken(string newToken)
    {
        if (!string.IsNullOrEmpty(newToken))
        {
            Debug.Log("Setting Token for " + this.name);
        }
        _token = newToken;
    }

    public int GetPlayerId() { return _playerID; }

    
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    public void RPC_SetNickname(int nick)
    {
        _playerID = nick;

    }
    public void TriggerJoin(PlayerController _playerController) => _agoraManager.JoinChannel(this, _playerController);
}




*/




using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using WebSocketSharp;
using System;
using ExitGames.Client.Photon.StructWrapping;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private NetworkRigidbody2D _rb;
    [SerializeField] public Sprite[] _sprites;

    public bool IamBridge = false;
    private ChangeDetector _changeDetector;
    public string PlayerName;
    [Networked, OnChangedRender(nameof(OnPlayerIdChanged))]
    public int _playerID { get; set; }
    [Networked, OnChangedRender(nameof(OnChannelChanged))]
    public NetworkString<_128> _channelName { get; set; } = "dummy";
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
        _rb = GetComponent<NetworkRigidbody2D>();
        _changeDetector = new ChangeDetector();
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
            Debug.LogError(_channelName.Value + " =====" + _playerID);
            if (!_channelName.Value.IsNullOrEmpty() && _channelName.Value != "dummy" && !isInChannel)
            {
                _agoraManager.AddPlayerToChannel(_channelName.Value, this);
            }
        }
    }

    private bool isnullorDummy(String name)
    {
        return name.IsNullOrEmpty() || name == "dummy";
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
                    // Server-side logic
                    HandleCollisionEnter(otherPlayer);
                }
                else
                {
                    // Client-side request
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
                    // Server-side logic
                    HandleCollisionExit(otherPlayer);
                }
                else
                {
                    // Client-side request
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
            RPC_JoinChannelWithPlayer(otherPlayer);
        }
    }

    private void HandleCollisionExit(PlayerController otherPlayer)
    {
        RPC_HandleOnTriggerExit(otherPlayer);
    }


    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void RPC_JoinChannelWithPlayer(PlayerController otherPlayer)
    {
        neighbours.Add(otherPlayer.Object.InputAuthority);
        Debug.LogError("Collision happened" + _playerID + otherPlayer._playerID);

        if (isnullorDummy(_channelName.Value) && isnullorDummy(otherPlayer._channelName.Value))
        {
            Rpc_SetChannelName(_agoraManager.GenerateChannelName("" + _playerID));
        }
        else if (isnullorDummy(_channelName.Value) && !isnullorDummy(otherPlayer._channelName.Value))
        {
            Rpc_SetChannelName(otherPlayer._channelName.Value);
        }
        else if (!isnullorDummy(_channelName.Value) && !isnullorDummy(otherPlayer._channelName.Value))
        {
            Debug.Log("testttt === " + _channelName.Value + otherPlayer._channelName.Value);
            IamBridge = true;
            if (_agoraManager.Bridges.TryGetValue(_channelName.Value, out int currentValue))
            {
                _agoraManager.Bridges[_channelName.Value] = currentValue + 1;
            }
            else
            {
                _agoraManager.Bridges.Add(_channelName.Value, 1);
            }
            _agoraManager.RpcMergeChannels(_channelName.Value, otherPlayer._channelName.Value);
        }

        _player.sprite = _sprites[1];
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RPC_HandleOnTriggerExit(PlayerController otherPlayer)
    {
        Debug.LogError("leaving === " + _playerID);
        bool change = false;

        if (neighbours.Contains(otherPlayer.Object.InputAuthority))
            neighbours.Remove(otherPlayer.Object.InputAuthority);

        if (neighbours.Count <= 0)
        {
            string channel = _agoraManager._channelName;
            _agoraManager.LeaveChannel(this);
            _agoraManager.Rpc_UpdateNetworkTable("remove", channel, this);
            change = true;
        }
        else
        {
            string channel = myChannel;
            if (IamBridge)
            {
                IamBridge = false;
                if (_agoraManager.Bridges.TryGetValue(_channelName.Value, out int currentValue))
                {
                    _agoraManager.Bridges[_channelName.Value] = currentValue - 1;
                    if (currentValue == 1)
                    {
                        HashSet<PlayerController> processedPlayers = new HashSet<PlayerController>();
                        ChangeChannelForPlayerAndNeighboursRecursively(otherPlayer, processedPlayers);
                    }
                }
            }
        }

        if (change)
            _player.sprite = _sprites[0];
    }

    private void ChangeChannelForPlayerAndNeighboursRecursively(PlayerController otherPlayer, HashSet<PlayerController> processedPlayers)
    {
        processedPlayers.Add(otherPlayer);
        string tempPreChannel = otherPlayer.prevChannel;
        _agoraManager.LeaveChannel(otherPlayer);
        otherPlayer._channelName = tempPreChannel;

        foreach (PlayerRef neighbourRef in otherPlayer.neighbours)
        {
            PlayerController neighbour = Runner.GetPlayerObject(neighbourRef).GetComponent<PlayerController>();
            if (!processedPlayers.Contains(neighbour))
            {
                ChangeChannelForPlayerAndNeighboursRecursively(neighbour, processedPlayers);
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
}