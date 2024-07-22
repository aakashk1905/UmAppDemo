using Agora.Rtc;
using Agora_RTC_Plugin.API_Example;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using WebSocketSharp;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor.Timeline.Actions;
#endif
public class AgoraManager : MonoBehaviour
{
    public static AgoraManager Instance { get; private set; }

    [SerializeField] private string appID;
    [SerializeField] private GameObject canvas;
    [SerializeField] private string tokenBase = "https://agoraapi.vercel.app/token";

    private IRtcEngine RtcEngine;

    private string _token = "";
    private string _channelName ="";

    private PlayerController mainPlayerInfo;

    public CONNECTION_STATE_TYPE connectionState = CONNECTION_STATE_TYPE.CONNECTION_STATE_DISCONNECTED;
    [Networked]
    public Dictionary<string, List<PlayerController>> networkTable { get; set; }
    [Networked]
    public int channelCount { get; set; }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        networkTable=new Dictionary<string, List<PlayerController>>();
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

    #region Channel Join/Leave Handler FunctionsgetToken
    public void JoinChannel(PlayerController player1, PlayerController player2)
    {
       
        if (string.IsNullOrEmpty(player1.GetChannelName()) && string.IsNullOrEmpty(player2.GetChannelName()))
        {
            string newChannelName = GenerateChannelName();
           
            AddPlayerToChannel(newChannelName, player1);
        }
        else if (!string.IsNullOrEmpty(player1.GetChannelName()) && string.IsNullOrEmpty(player2.GetChannelName()))
        {
           
            AddPlayerToChannel(player1.GetChannelName(), player2);
        }
        else if (string.IsNullOrEmpty(player1.GetChannelName()) && !string.IsNullOrEmpty(player2.GetChannelName()))
        {
           
            AddPlayerToChannel(player2.GetChannelName(), player1);
        }
        else if (player1.GetChannelName() != player2.GetChannelName())
        {
           
           
            MergeChannels(player1.GetChannelName(), player2.GetChannelName());
        }
    }
  
    public void AddPlayerToChannel(string channelName, PlayerController player)
    {
        Rpc_UpdateNetworkTable("add", channelName, player);
        player._channelName = channelName;

        string tempToken = GetTokenForChannel(channelName,player);
        
       
        if (!string.IsNullOrEmpty(tempToken))
        {
            player.SetToken(tempToken);
            JoinAgoraChannel(player, channelName, tempToken);
        }
        else
        {
            StartCoroutine(FetchAndJoinChannel(channelName, player));
        }
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
            player.SetToken(fetchedToken);
            JoinAgoraChannel(player, channelName, fetchedToken);
        }
        else
        {
            Debug.LogError("Failed to fetch token.");
        }
    }

    private void JoinAgoraChannel(PlayerController player, string channelName, string token)
    {

        player.SetChannelName(channelName);
        RtcEngine.JoinChannel(token, channelName, "", (uint)player.GetPlayerId());
        Debug.Log(player.name + " joined");
        RtcEngine.StartPreview();
        player._player.sprite = player._sprites[1];
    }

    private void MergeChannels(string channel1, string channel2)
    {
        if (!networkTable.ContainsKey(channel1) || !networkTable.ContainsKey(channel2)) return;

        List<PlayerController> channel1Players = new List<PlayerController>(networkTable[channel1]);
        List<PlayerController> channel2Players = new List<PlayerController>(networkTable[channel2]);
        string targetChannel = channel1Players.Count >= channel2Players.Count ? channel1 : channel2;
        string sourceChannel = channel1Players.Count >= channel2Players.Count ? channel2 : channel1;

        List<PlayerController> playersToMove = new List<PlayerController>(networkTable[sourceChannel]);

        foreach (var player in playersToMove)
        {
            Rpc_UpdateNetworkTable("remove", sourceChannel,player);
            LeaveChannel(player);
            AddPlayerToChannel(targetChannel, player);
        }
    }
    [Rpc]
    public void Rpc_UpdateNetworkTable(string action, string channelName, PlayerController player = null, string sourceChannel = null)
    {
        if (action == "add")
        {
            if (!networkTable.ContainsKey(channelName))
            {
                networkTable[channelName] = new List<PlayerController>();
                networkTable[channelName].Add(player);
            }
            else if (!networkTable[channelName].Contains(player))
            {
                networkTable[channelName].Add(player);
            }
        }
        else if (action == "remove")
        {
            if (networkTable.ContainsKey(channelName))
            {
                if(player!= null)
                    networkTable[channelName].Remove(player);
                if (networkTable[channelName].Count == 0)
                {
                    networkTable.Remove(channelName);
                    channelCount--;
                }
            }
        }
        else if (action == "merge")
        {
            if (!networkTable.ContainsKey(channelName) || !networkTable.ContainsKey(sourceChannel)) return;

            List<PlayerController> sourcePlayers = new List<PlayerController>(networkTable[sourceChannel]);

            foreach (var playerToMove in sourcePlayers)
            {
                Rpc_UpdateNetworkTable("add", channelName, playerToMove);
            }

            Rpc_UpdateNetworkTable("remove", sourceChannel);
        }
    }
    public void LeaveChannel(PlayerController player)
    {
        String channel = player._channelName;
        player.tokens.Remove(channel);
        RtcEngine.StopPreview();
        DestroyVideoView(0);
        RtcEngine.LeaveChannel();
        player.SetChannelName(string.Empty);
        player.SetToken(string.Empty);
        Debug.Log("Player Left" + player.name);
    }
 
 
    #endregion

    #region Helper Functions

    /// <summary>
    /// Responsible for destroying the video view of a user
    /// </summary>
    /// <param name="uid">User Id of the user whose video view is to be destroyed</param>
    private void DestroyVideoView(uint uid)
    {
        GameObject videoView = GameObject.Find(uid.ToString());
        if (videoView != null)
        {
            Destroy(videoView);
        }
    }
    /// <summary>
    /// Generate a channel name at the runtime
    /// </summary>
    /// <returns></returns>
    public string GenerateChannelName()
    {
        return "user_channel_" + (++channelCount);
    }
    private string GetTokenForChannel(string channelName, PlayerController player)
    {
        if (player.tokens.ContainsKey(channelName)) return player.tokens[channelName];
        else return "";
    }

    /// <summary>
    /// Responsible for updating channel name and token of a player with the given values
    /// </summary>
    /// <param name="player">The player whose valeus are to be updated</param>
    /// <param name="channelName">The name of the new channel</param>
    /// <param name="token">The new token</param>
    private void UpdatePropertiesForPlayer(PlayerController player, string channelName, string token)
    {
        player.SetChannelName(channelName);
        player.SetToken(token);

        if (player == mainPlayerInfo)
        {
            this._channelName = channelName;
            this._token = token;
        }
    }

    #endregion

    #region Video View Rendering Logic
    private void MakeVideoView(uint uid, string channelId = "")
    {
        GameObject videoView = GameObject.Find(uid.ToString());
        if (videoView != null)
        {
            //Video view for this user id already exists
            return;
        }

        // create a video surface game object and assign it to the user
        VideoSurface videoSurface = MakeImageSurface(uid.ToString());
        if (videoSurface == null) return;

        // configure videoSurface
        if (uid == 0)
        {
            videoSurface.SetForUser(uid, channelId);
        }
        else
        {
            videoSurface.SetForUser(uid, channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
        }


        videoSurface.OnTextureSizeModify += (int width, int height) =>
        {
            RectTransform transform = videoSurface.GetComponent<RectTransform>();
            if (transform)
            {
                //If render in RawImage. just set rawImage size.
                transform.sizeDelta = new Vector2(width / 2, height / 2);
                transform.localScale = Vector3.one;
            }
            else
            {
                //If render in MeshRenderer, just set localSize with MeshRenderer
                float scale = (float)height / (float)width;
                videoSurface.transform.localScale = new Vector3(-1, 1, scale);
            }
            Debug.LogError("OnTextureSizeModify: " + width + "  " + height);
        };

        videoSurface.SetEnable(true);
    }

    private VideoSurface MakeImageSurface(string goName)
    {
        GameObject gameObject = new GameObject();

        if (gameObject == null)
        {
            return null;
        }

        gameObject.name = goName;
        // to be renderered onto
        gameObject.AddComponent<RawImage>();
        // make the object draggable
        gameObject.AddComponent<UIElementDrag>();
        if (canvas != null)
        {
            //Add the video view as a child of the canvas
            gameObject.transform.parent = canvas.transform;
        }
        else
        {
            Debug.LogError("Canvas is null video view");
        }

        // set up transform
        gameObject.transform.Rotate(0f, 0.0f, 180.0f);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localScale = new Vector3(2f, 3f, 1f);

        // configure videoSurface
        VideoSurface videoSurface = gameObject.AddComponent<VideoSurface>();
        return videoSurface;
    }

    #endregion

    #region User Events
    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private AgoraManager agoraManager;
        internal UserEventHandler(AgoraManager agoraManager)
        {
            this.agoraManager = agoraManager;
        }

        /// <summary>
        /// Responsible for deleting all the views that are present on a user's screen, when the user leaves a channel
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="stats"></param>
        public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
        {
            if (!agoraManager.networkTable.ContainsKey(connection.channelId)) return;

            foreach (PlayerController uid in agoraManager.networkTable[connection.channelId])
            {
                agoraManager.DestroyVideoView((uint)uid.GetPlayerId());
            }
        }

        /// <summary>
        /// Responsible for adding the newly joined user to the channel's uid pool
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="uid"></param>
        /// <param name="elapsed"></param>
        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            agoraManager.MakeVideoView(uid, connection.channelId);

            //agoraManager.UpdateUsersInAChannelTable(connection.channelId, uid);
        }

        /// <summary>
        /// Responsible for removing a remote user's video view, if the user leaves the channel
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="uid"></param>
        /// <param name="reason"></param>
        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            agoraManager.DestroyVideoView(uid);

            string userChannel = connection.channelId;

            //if (agoraManager.usersJoinedInAChannel.ContainsKey(userChannel))
            //{
            //    agoraManager.usersJoinedInAChannel[userChannel].Remove(uid);
            //}
        }

        public override void OnConnectionStateChanged(RtcConnection connection, CONNECTION_STATE_TYPE state, CONNECTION_CHANGED_REASON_TYPE reason)
        {
            agoraManager.connectionState = state;
        }
    }
    #endregion
}
