using Fusion;
using Fusion.Addons.Physics;
using System.Collections;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public partial class PlayerController : NetworkBehaviour
{
    [SerializeField] private NetworkRigidbody2D _rb;
    [SerializeField] private LayerMask raycastMask;
    [SerializeField] private float moveSpeed = 1.2f;
    [SerializeField] private Button despawnButton;
    public Sprite[] _sprites;

    [Networked, OnChangedRender(nameof(OnNameChanged))] public NetworkString<_128> PlayerName { get; set; } = "";
    [Networked] public int _playerID { get; set; }
    public PlayerListManager playerListManager;
    [Networked] public NetworkString<_64> myId { get; set; }

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
    private ChatInitializer chatInitializer;

    private void Awake()
    {
        InitializeComponents();
        uiManager = FindAnyObjectByType<UiManager>();
        
    }

    private void OnApplicationQuit()
    {
         BeforeDespawn();
    }

  
     public void BeforeDespawn()
     {
         if (chatInitializer != null && Object.HasInputAuthority)
         {
             chatInitializer.SaveAndLogout();
         }
         if (Instance == this && Object.HasInputAuthority)
         {
             Instance = null;
         }
     }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_RemovePlayerInfo(string playerId)
    {
        if (PlayerListManager.Instance != null)
        {  
            playerListManager.RemovePlayerInfo(playerId);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
    }
    public override void Spawned()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _agoraManager = AgoraManager.Instance;
       
        

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

        
        networkTableManager = NetworkTableManager.Instance;
        _networkedDSU = NetworkedDSU.Instance;
        playerListManager = PlayerListManager.Instance;

        if (Object.HasStateAuthority && _networkedDSU != null)
        {
            _networkedDSU.MakeSet(Object.InputAuthority);
        }
        NameListCanvas = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.CompareTag("PlayerNameList"));
       
        playerInfoPrefab = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(obj => obj.CompareTag("PlayerInfo"));

       /* if (despawnButton != null && Object.HasInputAuthority)
        {
            despawnButton.gameObject.SetActive(true);
            despawnButton.onClick.RemoveListener(BeforeDespawn);
            despawnButton.onClick.AddListener(this.BeforeDespawn);
  
        }*/

        UpdateSprite();
        if (Object.HasInputAuthority)
        {
            _agoraManager.CreateLocalVideoView();
        }
        StartCoroutine(SetList());
        chatInitializer = GameObject. Find("ChatInitializer").GetComponent<ChatInitializer>();
    }

    private IEnumerator WaitForPlayerListManager()
    {
        while (playerListManager == null)
        {
            yield return null;
        }
        if(Object.HasInputAuthority)
         playerListManager.AddPlayerInfo(PlayerName.Value, UserDataManager.Instance.GetUserEmail().Split("@")[0].ToString());
        
    }

    private IEnumerator SetList()
    {
        while (playerListManager == null && NameListCanvas == null)
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
        if (NameListCanvas == null)return;
        
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
            _agoraManager.setPlayer(Object.InputAuthority.PlayerId.ToString());
            RPC_RequestSetPlayerInfo(Object.InputAuthority.PlayerId, name, UserDataManager.Instance.GetUserEmail().Split("@")[0].ToString());
            
        }
    }

    public override void FixedUpdateNetwork()
    {
        HandleMovement();
        HandleTeleportation();
        
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetPlayerInfo(PlayerRef playerRef, int id, string nickname, string my)
    {
        if (Object.InputAuthority == playerRef)
        {
            _playerID = id;
            PlayerName = nickname;
            myId = my;
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