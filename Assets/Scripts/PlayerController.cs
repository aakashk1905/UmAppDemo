using UnityEngine;
using Fusion;
using TMPro;
using Fusion.Addons.Physics;

public partial class PlayerController : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody2D _rb;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private float moveSpeed = 1.2f;
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
        moveSpeed = 1.2f;
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