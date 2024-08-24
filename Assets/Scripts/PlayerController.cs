using System.Collections.Generic;
using System.Collections;
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

    [Networked]
    private int _playerID { get; set; }

    public string _channelName;
    public string _token;
    public SpriteRenderer _player;
    public List<PlayerController> neighbours = new List<PlayerController>();
    private AgoraManager _agoraManager;
    public Dictionary<string, string> tokens = new Dictionary<string, string>();
    private float lastClickTime;
    private float doubleClickTime = 0.3f;
    private bool isTeleporting = false;
    private Vector2 _direction;
    private Vector3 _updatedPosition = Vector3.zero;
    public override void Spawned()
    {
       
        _player = GetComponent<SpriteRenderer>();
       
        if (Object.HasInputAuthority)
        { 
            if (_playerID == 0)
            {
                int id = Random.Range(0, 1000);
                RPC_SetNickname(id);

            }
           
        }
        transform.name = "Player" + _playerID;
        GetComponentInChildren<TMP_Text>().text = "Player" + _playerID;
        _agoraManager = AgoraManager.Instance;
        _rb = GetComponent<NetworkRigidbody2D>();
    }

public override void FixedUpdateNetwork()
{   
    if (Object.HasInputAuthority)
    {

        if (GetInput(out NetworkInputData input))
        {
            if (_updatedPosition != Vector3.zero)
            {
                if (isTeleporting)
                {
                    _rb.Rigidbody.velocity = Vector2.zero;
                    return;
                }
                Debug.Log($"Teleporting to position: {_updatedPosition}");
                _rb.Rigidbody.position = _updatedPosition;

            
                UpdateNetworkPosition(_updatedPosition);
                _updatedPosition = Vector3.zero; // Reset 
            }
            else
            {
                _rb.Rigidbody.velocity = input.directions * moveSpeed;
            }
        }
    }

    if (_playerID != 0)
    {
        transform.name = "Player" + _playerID;
        GetComponentInChildren<TMP_Text>().text = "Player" + _playerID;
    }
}
private void Update()
{
    if (isTeleporting) return;
    if (Input.GetMouseButtonDown(0))
    {
        if (Time.time - lastClickTime < doubleClickTime)
        {
            Debug.Log("Double click detected. Teleporting player.");
            TeleportPlayerToMousePosition();
        }
        lastClickTime = Time.time;
    }
}

private void TeleportPlayerToMousePosition()
{
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

    if (hit.collider != null && hit.collider.CompareTag("Plane"))
    {
        Vector3 targetPosition = hit.point;
        Debug.Log($"Hit detected on Plane. Target Position: {targetPosition}");
        _updatedPosition = targetPosition;

        StartCoroutine(SmoothTeleport(targetPosition));
    }
    else
    {
        Debug.Log("No hit detected or hit object is not Plane.");
    }
}

private IEnumerator SmoothTeleport(Vector3 targetPosition)
    {
        isTeleporting = true;
        Debug.Log($"Smooth teleport started to {targetPosition}.");

        float duration = 1.0f;
        float elapsed = 0f;
        Vector3 startPosition = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        // Update immediately
        UpdateNetworkPosition(targetPosition);

        isTeleporting = false;
    }
    
    private void UpdateNetworkPosition(Vector3 newPosition)
    {
        if (Object.HasStateAuthority)
        {
            transform.position = newPosition;
            Debug.Log($"Position synchronized across the network: {newPosition}");
        }
    }


    #region Collison Management
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !neighbours.Contains(collision.gameObject.GetComponent<PlayerController>()))
        {
            PlayerController otherPlayer = collision.gameObject.GetComponent<PlayerController>();
            neighbours.Add(otherPlayer);
            JoinChannelWithPlayer(otherPlayer);
            _player.sprite = _sprites[1];
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && neighbours.Contains(collision.gameObject.GetComponent<PlayerController>()))
        {
            HandleOnTriggerExit(collision.gameObject.GetComponent<PlayerController>());
        }

    }
    private void JoinChannelWithPlayer(PlayerController otherPlayer)
    {
        if (_agoraManager == null)
        {
        Debug.LogError("AgoraManager is not initialized!");
        return;
        }
    
        _agoraManager.JoinChannel(this, otherPlayer);
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

