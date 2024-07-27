using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private NetworkRigidbody2D _rb;
    [SerializeField] public Sprite[] _sprites;
    public NetworkString<_16> PlayerName { get; set; }

    public int _playerID;
    public string _channelName;
    public string _token;
    public SpriteRenderer _player;
    private Vector2 _direction;
    public List<PlayerController> neighbours = new List<PlayerController>();
    private AgoraManager _agoraManager;
    public Dictionary<string, string> tokens = new Dictionary<string, string>();

    public override void Spawned()
    {
        _player = GetComponent<SpriteRenderer>();
        if (Object.HasInputAuthority)
        {
            if(_playerID == 0)
            {
                RPC_SetNickname(GameManager.instance._playername);
            }
        }
        transform.name = "player" + _playerID.ToString();
        GetComponentInChildren<TMP_Text>().text = _playerID.ToString();
        _agoraManager = AgoraManager.Instance;
        _rb = GetComponent<NetworkRigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            _rb.Rigidbody.velocity = input.directions * moveSpeed;
        }
    }
    #region Collison Management
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !neighbours.Contains(collision.gameObject.GetComponent<PlayerController>()))
        {
            neighbours.Add(collision.gameObject.GetComponent<PlayerController>());
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            _agoraManager.JoinChannel(this, otherPlayer);
            _player.sprite = _sprites[1];
        }
    }

    /*private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && neighbours.Contains(collision.gameObject.GetComponent<PlayerController>()))
        {
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            neighbours.Remove(otherPlayer);
            otherPlayer.neighbours.Remove(this);

            if (neighbours.Count <= 0)
            {
                string channel = GetChannelName();
                _agoraManager.LeaveChannel(this);
                _agoraManager.Rpc_UpdateNetworkTable("remove", channel, this);
                _player.sprite = _sprites[0];

                if (otherPlayer.neighbours.Count == 0)
                {
                    _agoraManager.LeaveChannel(otherPlayer);
                    _agoraManager.Rpc_UpdateNetworkTable("remove", channel, otherPlayer);
                    otherPlayer._player.sprite = _sprites[0];
                }
            }
            else
            {
                List<PlayerController> connectedPlayers = new List<PlayerController>(_agoraManager.networkTable[_channelName]);
                HashSet<PlayerController> checkedPlayers = new HashSet<PlayerController>();

                foreach (PlayerController player in connectedPlayers)
                {
                    if (!checkedPlayers.Contains(player) && player.neighbours.Count >= 1)
                    {
                        string newChannelName = _agoraManager.GenerateChannelName();
                        AddMeAndNeighbours(player, newChannelName, new List<PlayerController>(), checkedPlayers);
                    }
                    else
                    {
                        _agoraManager.LeaveChannel(player);
                        _agoraManager.Rpc_UpdateNetworkTable("remove", player.GetChannelName(), player);
                        player._player.sprite = _sprites[0];
                    }
                }
            }
        }
    }*/


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && neighbours.Contains(collision.gameObject.GetComponent<PlayerController>()))
        {
            HandleOnTriggerExit(collision.gameObject.GetComponent<PlayerController>());
        }

    }
    public void HandleOnTriggerExit(PlayerController otherPlayer)
    {

        if (neighbours.Contains(otherPlayer))
            neighbours.Remove(otherPlayer);
        if (otherPlayer.neighbours.Contains(this))
            otherPlayer.neighbours.Remove(this);

        if (neighbours.Count <= 0)
        {
            string channel = GetChannelName();
            _agoraManager.LeaveChannel(this);
            _agoraManager.Rpc_UpdateNetworkTable("remove", channel, this);
            _player.sprite = _sprites[0];

            if (otherPlayer.neighbours.Count == 0)
            {
                _agoraManager.LeaveChannel(otherPlayer);
                _agoraManager.Rpc_UpdateNetworkTable("remove", channel, otherPlayer);
                otherPlayer._player.sprite = _sprites[0];
            }
        }
        else
        {
            List<PlayerController> connectedPlayers = new List<PlayerController>(_agoraManager.networkTable[_channelName]);
            HashSet<PlayerController> checkedPlayers = new HashSet<PlayerController>();

            foreach (PlayerController player in connectedPlayers)
            {
                if (!checkedPlayers.Contains(player) && player.neighbours.Count >= 1)
                {
                    string newChannelName = _agoraManager.GenerateChannelName();
                    AddMeAndNeighbours(player, newChannelName, new List<PlayerController>(), checkedPlayers);
                }
                else
                {
                    _agoraManager.LeaveChannel(player);
                    _agoraManager.Rpc_UpdateNetworkTable("remove", player.GetChannelName(), player);
                    player._player.sprite = _sprites[0];
                }
            }
        }

    }


    private void AddMeAndNeighbours(PlayerController player, string channelName, List<PlayerController> listOfNewPlayers, HashSet<PlayerController> checkedPlayers)
    {
        _agoraManager.LeaveChannel(player);
        _agoraManager.AddPlayerToChannel(channelName, player);
        checkedPlayers.Add(player);
        listOfNewPlayers.Add(player);
        foreach (PlayerController neighbour in player.neighbours)
        {
            if (!checkedPlayers.Contains(neighbour))
            {
                AddMeAndNeighbours(neighbour, channelName, listOfNewPlayers, checkedPlayers);
            }
        }
    }
    #endregion
    public string GetChannelName() { return _channelName; }
    public void SetChannelName(string name) { _channelName = name; }

    public string GetToken() { return _token; }
    public void SetToken(string newToken)
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
