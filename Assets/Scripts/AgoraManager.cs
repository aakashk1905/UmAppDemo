using Agora.Rtc;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using System.Collections;
using Agora_RTC_Plugin.API_Example;
using UnityEngine.UI;
using WebSocketSharp;
using AgoraChat;


public class AgoraManager : MonoBehaviour
{
    public static AgoraManager Instance { get; private set; }

    [SerializeField] private string appID;
    [SerializeField] private GameObject canvas;
    [SerializeField] private string tokenBase = "https://agoraapi.vercel.app/token";
    private bool isMuted = false;
    public string playerId;
    private bool isVideoEnabled = true;
    private bool isSharingScreen = false;
    public bool IsInitialized { get; private set; } = false;

    public IRtcEngine RtcEngine;

    public string _channelName = "";

    private Dictionary<string, HashSet<uint>> channelUsers = new Dictionary<string, HashSet<uint>>();
    public void setPlayer(string pl)
    {
        playerId = pl;
    }

    public CONNECTION_STATE_TYPE connectionState = CONNECTION_STATE_TYPE.CONNECTION_STATE_DISCONNECTED;
    [Networked] public Dictionary<string, int> Bridges { get; set; }
    [Networked]
    public int channelCount { get; set; }

    [SerializeField] private Sprite micOnSprite;
    [SerializeField] private Sprite micOffSprite;
    [SerializeField] private Button muteButton;

    [SerializeField] CarouselVideo carouselVideo;

    [SerializeField] Image DisableVideoImage, DisableMicImage, DisableSSImage;

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
    public void Dispose()
    {
        if (RtcEngine != null)
        {
            RtcEngine.LeaveChannel();
            RtcEngine.Dispose();
            RtcEngine = null;
        }

        Instance = null;
        Destroy(gameObject);
    }

    private void Start()
    {
        
        Bridges = new Dictionary<string, int>();
        InitRtcEngine();
        SetBasicConfiguration();

        carouselVideo.CreateNewPanel();
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
        IsInitialized = true;
    }
    public void ToggleMute()
    {
        Debug.LogError("Mute button Cicked" + isMuted);
        isMuted = !isMuted;
        RtcEngine.MuteLocalAudioStream(isMuted);

        // Update the button sprite based on the mute state
        if (muteButton != null)
        {
            Image buttonImage = muteButton.GetComponent<Image>();
            if (isMuted)
            {
                DisableMicImage.gameObject.SetActive(false);
                buttonImage.sprite = micOnSprite; // Switch to Mic On sprite when muted
                Log.Info("Mic is on");
            }
            else
            {
                DisableMicImage.gameObject.SetActive(true);
                buttonImage.sprite = micOffSprite; // Switch to Mic Off sprite when unmuted
                Log.Info("Mic is off");
            }
        }
        else
        {
            Debug.LogError("Mute button reference not set!");
        }
        //UpdateButtonTexts();
    }

    public void ToggleVideo()
    {
        isVideoEnabled = !isVideoEnabled;

        // Enable/Disable local video
        RtcEngine.EnableLocalVideo(isVideoEnabled);
        RtcEngine.MuteLocalVideoStream(!isVideoEnabled);

        uint localUid = 0; // Assuming local UID is 0; update if needed

        // Update the placeholder for the local user
        UpdateVideoPlaceholder(localUid, !isVideoEnabled);

        // Notify remote users about the change
        RpcSyncVideoState(localUid, isVideoEnabled);

        // Toggle preview for local user
        if (isVideoEnabled)
        {
            DisableVideoImage.gameObject.SetActive(false);
            RtcEngine.StartPreview();
            Log.Info("Video is on");
        }
        else
        {
            DisableVideoImage.gameObject.SetActive(true);
            RtcEngine.StopPreview();
            Log.Info("Video is off");
        }
    }

    public void OnUserMuteVideo(RtcConnection connection, uint uid, bool muted)
    {
        UpdateVideoPlaceholder(uid, muted);
        Debug.Log($"User {uid} video muted: {muted}");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RpcSyncVideoState(uint uid, bool videoState)
    {
        UpdateVideoPlaceholder(uid, !videoState);
    }

    private IEnumerator EnsureVideoSurfaceExistsAndSync(uint uid, bool videoState)
    {
        GameObject videoSurface = null;
        while (videoSurface == null)
        {
            videoSurface = GameObject.Find(uid.ToString());
            yield return null; // Wait for the next frame
        }

        RpcSyncVideoState(uid, videoState);
    }

    private IEnumerator EnsureVideoSurfaceExists(uint uid, bool showPlaceholder)
    {
        GameObject videoSurface = GameObject.Find(uid.ToString());
        if (videoSurface != null)
        {
            VideoOffPlaceHolder placeholder = videoSurface.GetComponent<VideoOffPlaceHolder>();
            if (placeholder != null)
            {
                placeholder.ShowPlaceholder(showPlaceholder);
            }
        }
        else
        {
            Debug.LogWarning($"Video surface not found for user: {uid}, ensuring creation...");
            StartCoroutine(EnsureVideoSurfaceExistsAndSync(uid, showPlaceholder));
        }
        return null;
    }


    private void UpdateVideoPlaceholder(uint uid, bool showPlaceholder)
    {
        GameObject videoSurface = GameObject.Find(uid.ToString());
        if (videoSurface != null)
        {
            VideoOffPlaceHolder placeholder = videoSurface.GetComponent<VideoOffPlaceHolder>();
            if (placeholder != null)
            {
                placeholder.ShowPlaceholder(showPlaceholder);
            }
        }
        else
        {
            Debug.LogError($"Video surface not found for user: {uid}");
        }
    }


    private void SetBasicConfiguration()
    {
        RtcEngine.EnableAudio();
        RtcEngine.EnableVideo();

        VideoEncoderConfiguration config = new VideoEncoderConfiguration();
        config.dimensions = new VideoDimensions(480, 480);
        config.frameRate = 15;
        config.bitrate = 0;
        RtcEngine.SetVideoEncoderConfiguration(config);

        RtcEngine.SetChannelProfile(CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING);
        RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
    }

    public void CreateLocalVideoView()
    {
       
        RtcEngine.StartPreview();

        VideoSurface videoSurface = MakeImageSurface(0.ToString(),"mine");       
        if (videoSurface != null)
        {
            videoSurface.SetForUser(0, "");
            videoSurface.OnTextureSizeModify += (int width, int height) =>
            {
                float scale = (float)height / (float)width;
                videoSurface.transform.localScale = new Vector3(1, scale, 1);
            };
            videoSurface.SetEnable(true);
        }

        ToggleMute();
        ToggleVideo();
    }

   public void ToggleScreenShare()
    {
        if (isSharingScreen)
        {
            DisableSSImage.gameObject.SetActive(true);
            StopScreenShare();
        }
        else
        {
            DisableMicImage.gameObject.SetActive(false);
            StartScreenShare();
        }
    }

    private void StartScreenShare()
    {
       
#if UNITY_ANDROID || UNITY_IPHONE
            var parameters2 = new ScreenCaptureParameters2
            {
                captureAudio = true,
                captureVideo = true
            };
            var nRet = RtcEngine.StartScreenCapture(parameters2);
#else
        var option = PrepareScreenCapture();
        if (RtcEngine == null) return;
        if (option.Contains("ScreenCaptureSourceType_Window"))
        {
            var windowId = option.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1];
            var nRet = RtcEngine.StartScreenCaptureByWindowId(
                (ulong)long.Parse(windowId),
                default(Rectangle),
                default(ScreenCaptureParameters)
            );
        }
        else
        {
            var dispId = uint.Parse(option.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1]);
            var nRet = RtcEngine.StartScreenCaptureByDisplayId(
                dispId,
                default(Rectangle),
                new ScreenCaptureParameters { captureMouseCursor = true, frameRate = 30 }
            );
        }
#endif

        PublishScreenShare();
        isSharingScreen = true;
    }

    private void StopScreenShare()
    {
        RtcEngine.StopScreenCapture();
        UnpublishScreenShare();
        isSharingScreen = false;
    }

    private void PublishScreenShare()
    {
        ChannelMediaOptions options = new ChannelMediaOptions();
        options.publishCameraTrack.SetValue(false);
        options.publishScreenTrack.SetValue(true);

#if UNITY_ANDROID || UNITY_IPHONE
            options.publishScreenCaptureAudio.SetValue(true);
            options.publishScreenCaptureVideo.SetValue(true);
#endif

        var ret = RtcEngine.UpdateChannelMediaOptions(options);
        Debug.Log("UpdateChannelMediaOptions for screen share returns: " + ret);
    }

    private void UnpublishScreenShare()
    {
        ChannelMediaOptions options = new ChannelMediaOptions();
        options.publishCameraTrack.SetValue(true);
        options.publishScreenTrack.SetValue(false);

#if UNITY_ANDROID || UNITY_IPHONE
            options.publishScreenCaptureAudio.SetValue(false);
            options.publishScreenCaptureVideo.SetValue(false);
#endif

        var ret = RtcEngine.UpdateChannelMediaOptions(options);
        Debug.Log("UpdateChannelMediaOptions for camera returns: " + ret);
    }

    private string PrepareScreenCapture()
    {
        SIZE t = new SIZE();
        t.width = 360;
        t.height = 240;
        SIZE s = new SIZE();
        s.width = 360;
        s.height = 240;
        var info = RtcEngine.GetScreenCaptureSources(t, s, true);
        var w = info[0];
        return string.Format("{0}: {1}-{2} | {3}", w.type, w.sourceName, w.sourceTitle, w.sourceId);
    }

    #endregion

    #region Channel Join/Leave Handler Functions

    public void AddPlayerToChannel(string channelName, PlayerController player)
    {
        if(player.networkTableManager == null)
        {
            Debug.LogError("It is nulll");
        }
       
       // player.LogNetworkTable();
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
        RtcEngine.JoinChannel(token, channelName, "", (uint)player._playerID);
        RtcEngine.StartPreview();

        // Notify others about the local user's video state
        RpcSyncVideoState((uint)player._playerID, isVideoEnabled);
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

    public string GenerateChannelName(string name)
    { 
        string newChannelName = "user_" + name;
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

    private VideoSurface MakeImageSurface(string goName,string mine="")
    {
        carouselVideo.ActivateCarousel();
        GameObject gameObject = new GameObject();

        if (gameObject == null)
        {
            return null;
        }

        gameObject.name = goName;
        gameObject.AddComponent<RawImage>();
        gameObject.tag = "VideoSurface"+mine;
        gameObject.layer = 12;
        
        if (canvas != null)
        {
            carouselVideo.currentPanel();
            gameObject.transform.SetParent(carouselVideo.CurrentPanel.transform);
            carouselVideo.UpdateButtonPositions();
        }
        else
        {
            Debug.LogError("Canvas is null for video view");
        }

        gameObject.transform.Rotate(0f, 0.0f, 180.0f);
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(30,30);

        // Add a placeholder image for when the video is off
        GameObject placeholderImageObj = new GameObject("PlaceholderImage");
        placeholderImageObj.transform.SetParent(gameObject.transform);
        Image placeholderImage = placeholderImageObj.AddComponent<Image>();
        placeholderImage.sprite = Resources.Load<Sprite>("PlaceholderImage"); // Load your image from Resources
        placeholderImageObj.GetComponent<RectTransform>().sizeDelta = new Vector2(173, 173);
        placeholderImageObj.transform.localPosition = Vector3.zero;

        //Initialize placeholder state
        VideoOffPlaceHolder placeholder = gameObject.AddComponent<VideoOffPlaceHolder>();
        placeholder.SetPlaceholder(placeholderImageObj);
        placeholder.ShowPlaceholder(true); // Default to visible

        // Add a click event listener to toggle fullscreen
        Button btn = gameObject.AddComponent<Button>(); // Add a Button component for click detection
        btn.onClick.AddListener(() => carouselVideo.ToggleFullscreen(gameObject));

        VideoSurface videoSurface = gameObject.AddComponent<VideoSurface>();
        videoSurface.gameObject.AddComponent<VideoOffPlaceHolder>().SetPlaceholder(placeholderImageObj);

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
            GameObject[] videoViews = GameObject.FindGameObjectsWithTag("VideoSurface");
            foreach (GameObject videoView in videoViews)
            {
                Destroy(videoView);
            }
        }


        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            if (!joinedUsers.Contains(uid))
            {
                VideoSurface videoSurface = agoraManager.MakeImageSurface(uid.ToString());
                if (videoSurface != null)
                {
                    videoSurface.SetForUser(uid, connection.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
                    videoSurface.SetEnable(true);

                    // Show placeholder initially
                    //StartCoroutine(EnsureVideoSurfaceExistsAndSync(uid, true));// Add this here
                }
                else
                {
                    Debug.LogError($"Failed to create video surface for user: {uid}");
                }
                joinedUsers.Add(uid);
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