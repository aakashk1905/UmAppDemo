using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;
using static UnityEngine.RuleTile.TilingRuleOutput;
using WebSocketSharp;

#if UNITY_EDITOR
using UnityEditor.ShaderGraph.Internal;
#endif

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private NetworkRigidbody2D _rb;
    [SerializeField] public Sprite[] _sprites;

    private int _playerID;
    public string _channelName;
    public string _token;
    public SpriteRenderer _player;
    private Vector2 _direction;
    private List<PlayerController> neighbours = new List<PlayerController>();
    private AgoraManager _agoraManager;
    public Dictionary<string, string> tokens = new Dictionary<string, string>();
    private void Start()
    {
        _player = GetComponent<SpriteRenderer>();
        _playerID = SetPlayerID();
        _agoraManager = AgoraManager.Instance;
    }

    // Update player movement based on input
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            float moveX = UnityEngine.Input.GetAxis("Horizontal");
            float moveY = UnityEngine.Input.GetAxis("Vertical");

            _direction = new Vector2(moveX, moveY).normalized;

            _rb.Rigidbody.velocity = _direction * _speed;
        }

    }

    // Handle player collision for audio communicationn
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
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && neighbours.Contains(collision.gameObject.GetComponent<PlayerController>()))
        {
            PlayerController otherplayer = collision.gameObject.GetComponent<PlayerController>();
            if (neighbours.Contains(otherplayer))
                neighbours.Remove(otherplayer);
            if (otherplayer.neighbours.Contains(this))
                otherplayer.neighbours.Remove(this);
            
            if (neighbours.Count <= 0)
            {
                Debug.Log("removing myself");
                string channel = GetChannelName();
                _agoraManager.LeaveChannel(this);
                _agoraManager.Rpc_UpdateNetworkTable("Remove",channel,this);
                _player.sprite = _sprites[0];


                // removing neighbour also from the channel if no neighbour
                if(otherplayer.neighbours.Count == 0)
                {
                    _agoraManager.LeaveChannel(otherplayer);
                    _agoraManager.Rpc_UpdateNetworkTable("Remove", channel, otherplayer);
                    otherplayer._player.sprite = _sprites[0];
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
                        Debug.Log("++++++++++++++++++++++++" + player.name +" " +  player.neighbours.Count);
                        string newChannelName = _agoraManager.GenerateChannelName();
                        AddMeAndNeighbours(player, newChannelName,new List<PlayerController>(), checkedPlayers);
                    }
                    else
                    {
                        Debug.Log("=========================" + player.name);
                        _agoraManager.LeaveChannel(player);
                        _agoraManager.Rpc_UpdateNetworkTable("Remove", player.GetChannelName(), player);
                        player._player.sprite = _sprites[0];

                    }

                }

            }
        }
    }
    private void AddMeAndNeighbours(PlayerController player,string channelName,List<PlayerController> listOfNewPlayers,HashSet<PlayerController> checkedPlayers)
    {
        _agoraManager.LeaveChannel(player);
        _agoraManager.AddPlayerToChannel(channelName, player);
        checkedPlayers.Add(player);
        listOfNewPlayers.Add(player);
        foreach(PlayerController neighbours in player.neighbours)
        {
            if (!checkedPlayers.Contains(neighbours))
            {
                AddMeAndNeighbours(neighbours, channelName, listOfNewPlayers, checkedPlayers);
            } 
        }
    }
    public string GetChannelName() { return _channelName; }
    public void SetChannelName(string name) { _channelName = name; }

    public string GetToken() { return _token; }
    public void SetToken(string newToken) 
    {
        if (!newToken.IsNullOrEmpty())
        {
            Debug.Log("Setting Token for " + this.name);
        }
        _token = newToken;
    }

    public int GetPlayerId() { return _playerID; }
    public static int SetPlayerID() => UnityEngine.Random.Range(10000, 99999);
    public void TriggerJoin(PlayerController _playerController) => _agoraManager.JoinChannel(this,_playerController);
}


