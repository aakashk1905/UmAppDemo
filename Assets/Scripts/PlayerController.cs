using Fusion;
using Fusion.Addons.Physics;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public partial class PlayerController : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody2D _rb;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private float moveSpeed = 1.2f;
    public Sprite[] _sprites;

    [Networked, OnChangedRender(nameof(OnNameChanged))] public NetworkString<_128> PlayerName { get; set; } = "";
    [Networked] public int _playerID { get; set; }
    public PlayerListManager playerListManager;
    public Animator animator;
    public CharacterChoose choose;
    public SpriteRenderer _player;
    public AgoraManager _agoraManager;
    public NetworkTableManager networkTableManager;
    public NetworkedDSU _networkedDSU;
    [SerializeField] public GameObject movementJoystick;
    public GameObject NameListCanvas;
    public GameObject playerInfoPrefab;
    public static PlayerController Instance { get;set; }

    private UiManager uiManager;

    private void Awake()
    {
        InitializeComponents();
        uiManager = FindAnyObjectByType<UiManager>();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        PlayerListManager.Instance.RemovePlayerInfo(_playerID.ToString());
        base.Despawned(runner, hasState);

    }
    public override void Spawned()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        moveSpeed = 1.2f;

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
        playerListManager = PlayerListManager.Instance;

        if (Object.HasStateAuthority && _networkedDSU != null)
        {
            _networkedDSU.MakeSet(Object.InputAuthority);
        }
        //NameListCanvas = GameObject.FindGameObjectWithTag("PlayerNameList");
        NameListCanvas = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.CompareTag("PlayerNameList"));
        playerInfoPrefab = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.CompareTag("PlayerInfo"));
        TextMeshProUGUI PlayerNametxt;

        Debug.LogError(NameListCanvas);
        UpdateSprite();
        if (Object.HasInputAuthority)
        {
            _agoraManager.CreateLocalVideoView();
        }
        StartCoroutine(SetList());
    }

    private IEnumerator WaitForPlayerListManager()
    {
        while (playerListManager == null)
        {
            yield return null;
        }
        if(Object.HasInputAuthority)
         PlayerListManager.Instance.AddPlayerInfo(PlayerName.Value, UserDataManager.Instance.GetUserEmail().Split("@")[0].ToString());
        
    }

        private IEnumerator SetList()
    {
        while (playerListManager == null)
        {
            yield return null;
        }

        if (Object.HasInputAuthority)
        {
            PlayerListManager.OnPlayerListUpdated += UpdateNameList;
        }
    }

    private void UpdateNameList()
    {
        if (!Object.HasInputAuthority) return;

        foreach (Transform child in NameListCanvas.transform)
        {
            Destroy(child.gameObject);
        }

        if (playerListManager != null)
        {
            
            foreach (var playerInfo in playerListManager.playerInfoList)
            {
                RenderName(playerInfo.name.Value,playerInfo.id.Value);
            }
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