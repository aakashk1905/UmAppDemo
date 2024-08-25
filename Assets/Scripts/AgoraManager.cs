using Agora.Rtc;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using System.Collections;
using Agora_RTC_Plugin.API_Example;
using UnityEngine.UI;
using WebSocketSharp;
using static Unity.Collections.Unicode;

public class AgoraManager : MonoBehaviour
{
    public static AgoraManager Instance { get; private set; }

    [SerializeField] private string appID;
    [SerializeField] private GameObject canvas;
    [SerializeField] private string tokenBase = "https://agoraapi.vercel.app/token";

    private IRtcEngine RtcEngine;

    private string _token = "";
    public string _channelName = "";

    private PlayerController mainPlayerInfo;
    private Dictionary<string, HashSet<uint>> channelUsers = new Dictionary<string, HashSet<uint>>();


    public CONNECTION_STATE_TYPE connectionState = CONNECTION_STATE_TYPE.CONNECTION_STATE_DISCONNECTED;
    [Networked] public Dictionary<string, int> Bridges { get; set; }
    [Networked]
    public int channelCount { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        
        Bridges = new Dictionary<string, int>();
        InitRtcEngine();
        SetBasicConfiguration();
    }

    #region Configuration Functions

    private void InitRtcEngine()
    {
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngineContext context = new RtcEngineContext();
        context.appId = appID;
        context.channelProfile = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING;
        context.audioScenario = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT;
        context.areaCode = AREA_CODE.AREA_CODE_GLOB;

        RtcEngine.Initialize(context);
        RtcEngine.InitEventHandler(handler);
    }

    private void SetBasicConfiguration()
    {
        RtcEngine.EnableAudio();
        RtcEngine.EnableVideo();

        //Setting up Video Configuration
        VideoEncoderConfiguration config = new VideoEncoderConfiguration();
        config.dimensions = new VideoDimensions(640, 360);
        config.frameRate = 15;
        config.bitrate = 0;
        RtcEngine.SetVideoEncoderConfiguration(config);

        RtcEngine.SetChannelProfile(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING);
        RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
    }

    #endregion

    #region Channel Join/Leave Handler Functions

    public void AddPlayerToChannel(string channelName, PlayerController player)
    {
        if(player.networkTableManager == null)
        {
            Debug.LogError("It is nulll");
        }
       
        player.LogNetworkTable();
        Debug.LogError("Adding " + player._playerID + " " + channelName);
        string tempToken = GetTokenForChannel(channelName, player);
        player.isInChannel = true;

        if (!string.IsNullOrEmpty(tempToken))
        {
            player.RpcSetToken(tempToken);
            JoinAgoraChannel(player, channelName, tempToken);
        }
        else
        {
            StartCoroutine(FetchAndJoinChannel(channelName, player));
        }

        if (!channelUsers.ContainsKey(channelName))
        {
            channelUsers[channelName] = new HashSet<uint>();
        }
        channelUsers[channelName].Add((uint)player._playerID);
    }

    private IEnumerator FetchAndJoinChannel(string channelName, PlayerController player)
    {
        bool isTokenFetched = false;
        string fetchedToken = string.Empty;

        void UpdateToken(string token)
        {
            fetchedToken = token;
            player.tokens[channelName] = token;
            isTokenFetched = true;
        }

        StartCoroutine(HelperClass.FetchToken(tokenBase, channelName, player.GetPlayerId(), UpdateToken));

        yield return new WaitUntil(() => isTokenFetched);

        if (!string.IsNullOrEmpty(fetchedToken))
        {
            player.RpcSetToken(fetchedToken);
            JoinAgoraChannel(player, channelName, fetchedToken);
        }
        else
        {
            Debug.LogError("Failed to fetch token.");
        }
    }

    private void JoinAgoraChannel(PlayerController player, string channelName, string token)
    {
        var result = RtcEngine.JoinChannel(token, channelName, "", (uint)player._playerID);
        Debug.LogError("Results =" + result + " " + player._channelName + " " + (uint)player._playerID);

        RtcEngine.StartPreview();
   
    }
    [Rpc]
    public void UpdateChannelCount(int num)
    {
        channelCount = num;
    }

    public void LeaveChannel(PlayerController player, string channel)
    {
        player.tokens.Remove(channel);

        if (channelUsers.ContainsKey(channel))
        {
            channelUsers[channel].Remove((uint)player._playerID);
            if (channelUsers[channel].Count == 0)
            {
                channelUsers.Remove(channel);
            }
        }

        if (!IsUserInAnyChannel((uint)player._playerID))
        {
            RtcEngine.LeaveChannel();
        }

        player.Rpc_SetChannelName(string.Empty);
        player.RpcSetToken(string.Empty);
        player.isInChannel = false;
        Debug.LogError("Player Left: " + player.name);
    }

    #endregion

    #region Helper Functions

    private void DestroyVideoView(uint uid)
    {
        GameObject videoView = GameObject.Find(uid.ToString());
        if (videoView != null)
        {
            Destroy(videoView);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]

    public string GenerateChannelName(String name)
    { 
        string newChannelName = "user_channel_" + name;
        return newChannelName;
    }

    private string GetTokenForChannel(string channelName, PlayerController player)
    {
        if (player.tokens.ContainsKey(channelName)) return player.tokens[channelName];
        return "";
    }

    public bool IsUserInAnyChannel(uint uid)
    {
        foreach (var channelUsers in channelUsers.Values)
        {
            if (channelUsers.Contains(uid))
            {
                return true;
            }
        }
        return false;
    }

    private VideoSurface MakeImageSurface(string goName)
    {
        GameObject gameObject = new GameObject();

        if (gameObject == null)
        {
            return null;
        }

        gameObject.name = goName;
        gameObject.AddComponent<RawImage>();
        gameObject.tag = "VideoSurface";
        gameObject.AddComponent<UIElementDrag>();
        if (canvas != null)
        {
            gameObject.transform.SetParent(canvas.transform);
        }
        else
        {
            Debug.LogError("Canvas is null for video view");
        }

        gameObject.transform.Rotate(0f, 0.0f, 180.0f);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localScale = new Vector3(2f, 3f, 1f);

        VideoSurface videoSurface = gameObject.AddComponent<VideoSurface>();
        return videoSurface;
    }

    #endregion

    #region User Events
    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private AgoraManager agoraManager;
        private HashSet<uint> joinedUsers = new HashSet<uint>();

        internal UserEventHandler(AgoraManager agoraManager)
        {
            this.agoraManager = agoraManager;
        }

        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            
            joinedUsers.Clear();
            Debug.LogError("Player Left calleddddddd "+ connection.localUid);
            GameObject[] videoViews = GameObject.FindGameObjectsWithTag("VideoSurface");
            foreach (GameObject videoView in videoViews)
            {
                Destroy(videoView);
            }
        }


        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            Debug.LogError("on user joined callleedddd");
            if (!joinedUsers.Contains(uid))
            {
                VideoSurface videoSurface = agoraManager.MakeImageSurface(uid.ToString());
                if (videoSurface != null)
                {
                    videoSurface.SetForUser(uid, connection.channelId, uid == 0 ? VIDEO_SOURCE_TYPE.VIDEO_SOURCE_LOCAL : VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
                    videoSurface.SetEnable(true);
                }
            }
        }

        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            Debug.LogError("User offline: " + uid);

            if (!agoraManager.IsUserInAnyChannel(uid))
            {
                Debug.LogError("Destroying video view for user: " + uid);
                agoraManager.DestroyVideoView(uid);
            }
            else
            {
                Debug.LogError("User " + uid + " is still in a channel, not destroying video view");
            }

            joinedUsers.Remove(uid);
        }
        public override void OnConnectionStateChanged(RtcConnection connection, CONNECTION_STATE_TYPE state, CONNECTION_CHANGED_REASON_TYPE reason)
        {
            agoraManager.connectionState = state;
        }
    }
    #endregion
}