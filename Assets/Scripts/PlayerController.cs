/*using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;
using TMPro;
using WebSocketSharp;
using System;
using System.Linq;
using UnityEngine.EventSystems;


public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private NetworkRigidbody2D _rb;
    public BoxCollider2D trigger;
    private CircleCollider2D Rangetrigger;
    private CircleCollider2D Roomtrigger;
    private GameObject targetIndicator;


    [SerializeField] private LayerMask raycastMask;
    public Sprite[] _sprites;

    public NetworkTableManager networkTableManager;
    public NetworkedDSU _networkedDSU;

    public bool IamBridge = false;
    private ChangeDetector _changeDetector;
    [Networked, OnChangedRender(nameof(OnNameChanged))]
    public NetworkString<_128> PlayerName { get; set; } = "";
    [Networked, OnChangedRender(nameof(OnRoomChanged))]
    public NetworkBool IsInRoom { get; set; } = false;
    [Networked]
    public int _playerID { get; set; }
    [Networked, OnChangedRender(nameof(OnChannelChanged))]
    public NetworkString<_128> _channelName { get; set; } = "";
    public string myChannel  = "";
    public string prevChannel  = "";
    public string _token = "";
    public bool isInChannel = false;
    public SpriteRenderer _player;
    public List<PlayerRef> neighbours = new List<PlayerRef>();
    private AgoraManager _agoraManager;
    private Vector2 _direction;
    public Dictionary<string, string> tokens = new Dictionary<string, string>();
    private float lastClickTime = 0f;
    private float DOUBLE_CLICK_TIME = 0.3f;
    private bool isTeleporting = false;
    private Vector3 _updatedPosition = Vector3.zero;
    public float _speed = 5f;
    private int clickCount = 0;
    public GameObject range;

    private Vector2 lastClickPosition;
    private float MAX_CLICK_DISTANCE = 10f;
    public bool isTimeCheckAllowed = true;
    public Animator animator;
    [SerializeField] public GameObject movementJoystick;
    [Networked] private Vector3 _targetPosition { get; set; }
    [Networked] private NetworkBool _isTeleporting { get; set; }




    public override void Spawned()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        Destroy(movementJoystick);

#endif
        if (Object.HasInputAuthority)
        {
            LoadPlayerData();
        }
        raycastMask = ~0;
        _player = GetComponent<SpriteRenderer>();
       
        _speed = 5f;
        transform.name = PlayerName.Value;
        GetComponentInChildren<TMP_Text>().text = PlayerName.Value;
        range = transform.Find("Range").gameObject;
        Transform roomTransform = transform.Find("RoomTrigger");
        Roomtrigger = roomTransform.GetComponent<CircleCollider2D>();
        if (range != null)
        {
            Rangetrigger = range.GetComponent<CircleCollider2D>();   
        }
        _agoraManager = AgoraManager.Instance;
        _rb = GetComponent<NetworkRigidbody2D>();
        trigger = GetComponent<BoxCollider2D>();
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
        UpdateSprite();
        if (Object.HasInputAuthority)
        {
            _agoraManager.CreateLocalVideoView();
        }
    }

    void LoadPlayerData()
    {
        if (UserDataManager.Instance != null && UserDataManager.Instance.CurrentUser != null)
        {
            string name = UserDataManager.Instance.GetUserName();
           RPC_RequestSetPlayerInfo(Object.InputAuthority.PlayerId, name);
  
        }
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_RequestSetPlayerInfo(int id, string nickname)
    {
        RPC_SetPlayerInfo(Object.InputAuthority, id, nickname);
    }

    public override void FixedUpdateNetwork()
    {
        Vector2 moveDirection = Vector2.zero;

        if (GetInput(out NetworkInputData input))
        {
            moveDirection = input.directions;
        }


         if (Object.HasInputAuthority)
         {
             CheckForDoubleClick();
         }

        if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
        {
            moveDirection.y = 0;
        }
        else
        {
            moveDirection.x = 0;
        }

        moveDirection = moveDirection.normalized;

        _rb.Rigidbody.velocity = moveDirection * moveSpeed;

        animator.SetFloat("Horizontal", moveDirection.x);
        animator.SetFloat("Vertical", moveDirection.y);
        animator.SetFloat("Speed", moveDirection.magnitude);

        if (_isTeleporting)
        {
            if (targetIndicator == null)
            {
                CreateTargetIndicator();
            }

            trigger.enabled = false;
            Rangetrigger.enabled = false;
            Roomtrigger.enabled = false;
            animator.enabled = false;
            _player.sprite = _sprites[0];
            range.GetComponent<SpriteRenderer>().enabled = false;

            Vector2 direction = _targetPosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.SetPositionAndRotation(
                Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Runner.DeltaTime),
                Quaternion.AngleAxis(angle - 45, Vector3.forward)
            );

            if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
            {
                EndTeleportation();
            }
        }
    }

    private void CreateTargetIndicator()
    {
        targetIndicator = new GameObject("TeleportTargetIndicator");
        SpriteRenderer spriteRenderer = targetIndicator.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = _sprites[2]; 
        targetIndicator.transform.position = _targetPosition;
        targetIndicator.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
    }

    private void EndTeleportation()
    {
        _isTeleporting = false;
        trigger.enabled = true;
        Rangetrigger.enabled = true;
        Roomtrigger.enabled = true;
        transform.rotation = Quaternion.identity;
        animator.enabled = true;
        range.GetComponent<SpriteRenderer>().enabled = true;

        if (targetIndicator != null)
        {
            Destroy(targetIndicator);
            targetIndicator = null;
        }
    }

    private void CheckForDoubleClick()
    {
        if (Input.touchSupported && Application.platform != RuntimePlatform.WebGLPlayer)
        {
            HandleTouchForTeleport();
        }
        else
        {
            HandleMouseForTeleport();
        }
    }

    private void HandleTouchForTeleport()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Ended && touch.tapCount == 2)
            {
                TeleportPlayerToMousePosition(touch.position);
            }
        }
    }

    private void HandleMouseForTeleport()
    {
        if (Input.GetMouseButtonUp(0))
        {
            clickCount += 1;
        }
        if (clickCount == 1 && isTimeCheckAllowed)
        {
            lastClickTime = Time.time;
            StartCoroutine(DetectDoubleLeftClick());
        }
    }


    private IEnumerator DetectDoubleLeftClick()
    {
        isTimeCheckAllowed = false;
        while (Time.time < lastClickTime + 0.3f)
        {
            if (clickCount == 2)
            {
                TeleportPlayerToMousePosition(Input.mousePosition);
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        clickCount = 0;
        isTimeCheckAllowed = true;
    }

    private void TeleportPlayerToMousePosition(Vector2 screenPosition)
    {
        if (IsPointerOverUIElement(screenPosition))
        {
            return;
        }

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(screenPosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity, raycastMask);

        if (hits.Length > 0)
        {
            System.Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));
            RaycastHit2D hit = hits[0];
            int hitLayerIndex = hit.collider.gameObject.layer;
            if(hitLayerIndex == 8)
            {
                RPC_RequestTeleport(hit.point);
            }else
            {
                return;
            }
 
        }

    }


    private bool IsPointerOverUIElement(Vector2 screenPosition)
    {
        PointerEventData eventData = new(EventSystem.current);
        eventData.position = screenPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.layer != 8)
            {
                return true;
            }
        }

        return false;
    }


    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void RPC_RequestTeleport(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
        _isTeleporting = true;
    }

    #region Collison Management

    public void OnNameChanged()
    {
        if (!string.IsNullOrEmpty(PlayerName.Value))
        {
            transform.name = PlayerName.Value;
            TMP_Text nameText = GetComponentInChildren<TMP_Text>();
            if (nameText != null)
            {
                nameText.text = PlayerName.Value;
            }
            else
            {
                Debug.LogError("TMP_Text component not found on player object.");
            }
        }
    }
    public void OnRoomChanged()
    {
        UpdateSprite(); 
    }
    public void UpdateSprite()
    {
        
        range.GetComponent<SpriteRenderer>().enabled = !IsInRoom;
        trigger.enabled = !IsInRoom;
    }


    public void OnChannelChanged()
    {
        if (Object.HasInputAuthority)
        {
            prevChannel = myChannel;
            myChannel = _channelName.Value;
            
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
        }
    }
   

    private bool IsnullorDummy(String name)
    {
      
        return String.IsNullOrEmpty(name) || name == "dummy";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" )
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
        //LogNetworkTable();
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
    public void Rpc_LeaveChannel(PlayerRef playerRef, string channel)
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


    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void Rpc_UpdatePlayerSpriteTeleport(bool teleporting)
    {
        Rpc_UpdatePlayerSprite(teleporting);
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

        //LogNetworkTable();

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

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void Rpc_UpdateIsInRoom(bool value)
    {
        IsInRoom = value;
    }

    public string GetToken() { return _token; }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void RpcSetToken(string newToken)
    {
        _token = newToken;
    }

    public int GetPlayerId() { return _playerID; }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SetPlayerInfo(PlayerRef playerRef, int id, string nickname)
    {
        if (Object.InputAuthority == playerRef)
        {
            _playerID = id;
            PlayerName = nickname;
            Debug.Log($"Setting player info for {playerRef}: ID = {id}, Name = {nickname}");

        }
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    public void Rpc_SetIsInChannel(bool value)
    {
        isInChannel = value;
    }

}
#endregion*/


using UnityEngine;
using Fusion;
using TMPro;
using Fusion.Addons.Physics;

public partial class PlayerController : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody2D _rb;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private float moveSpeed = 2f;
    public Sprite[] _sprites;

    [Networked, OnChangedRender(nameof(OnNameChanged))] public NetworkString<_128> PlayerName { get; set; } = "";
    [Networked] public int _playerID { get; set; }

    public Animator animator;
    public CharacterChoose choose;
    public SpriteRenderer _player;
    public AgoraManager _agoraManager;
    public NetworkTableManager networkTableManager;
    public NetworkedDSU _networkedDSU;
    [SerializeField] public GameObject movementJoystick;
    public static PlayerController Instance { get;set; }


    private void Awake()
    {
        InitializeComponents();
        
    }

    public override void Spawned()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        choose = GetComponent<CharacterChoose>();
        if (choose != null) ;
        choose.playerController = this;
        if (Object.HasInputAuthority)
        {
            LoadPlayerData();
        }
#if !UNITY_ANDROID && !UNITY_IOS
        Destroy(movementJoystick);

#endif

        raycastMask = ~0;
        _player = GetComponent<SpriteRenderer>();
        transform.name = PlayerName.Value;
        GetComponentInChildren<TMP_Text>().text = PlayerName.Value;

        InitializeNetworking();
        InitializeCollision();
        InitializeChannelManager();
        InitializeVisuals();

        _agoraManager = AgoraManager.Instance;
        networkTableManager = NetworkTableManager.Instance;
        _networkedDSU = NetworkedDSU.Instance;

        if (Object.HasStateAuthority && _networkedDSU != null)
        {
            _networkedDSU.MakeSet(Object.InputAuthority);
        }

        UpdateSprite();

        if (Object.HasInputAuthority)
        {
            _agoraManager.CreateLocalVideoView();
        }
    }

    private void InitializeComponents()
    {
        _rb = GetComponent<NetworkRigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void LoadPlayerData()
    {
        if (UserDataManager.Instance != null && UserDataManager.Instance.CurrentUser != null)
        {
            string name = UserDataManager.Instance.GetUserName();
            RPC_RequestSetPlayerInfo(Object.InputAuthority.PlayerId, name);
        }
    }

    public override void FixedUpdateNetwork()
    {
        HandleMovement();
        HandleTeleportation();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetPlayerInfo(PlayerRef playerRef, int id, string nickname)
    {
        if (Object.InputAuthority == playerRef)
        {
            _playerID = id;
            PlayerName = nickname;
        }
    }

    public void UpdateSprite()
    {
        range.GetComponent<SpriteRenderer>().enabled = !IsInRoom;
        trigger.enabled = !IsInRoom;
    }
    public int GetPlayerId()
    {
        return _playerID;
    }

}