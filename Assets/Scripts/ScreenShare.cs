using Agora.Rtc;
using Agora_RTC_Plugin.API_Example.Examples.Advanced.ScreenShare;
using io.agora.rtc.demo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShare : MonoBehaviour
{
    public IRtcEngine rtcEngine;
    public string _appId = "1a500d1eb74641d3b46d085049e75f32";
    public string _tokenId = "007eJxTYFjRy/byhpKRR21QQXV8+E17i36n3w8PZKvEmC79UqOUUqnAYJhoamCQYpiaZG5iZmKYYpxkYpZiYGFqYGKZam6aZmz0zX9VWkMgI0NkZjsTIwMEgvg8DGH5mcmpzhmJeXmpOQwMAAdoIWM=";
    private AppIdInput _appIdInput;
    
    private string _channelName;

    public IRtcEngine RtcEngine { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        LoadAssetData();
        InitEngine();
        
        IntializeScreenCapture();
        SetBasicConfiguration();
        Debug.Log("Intialized screen");
        JoinChannel();
        Debug.Log("Joined channel");
    }

    private void SetBasicConfiguration()
    {
        RtcEngine.EnableAudio();
        RtcEngine.EnableVideo();
        RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
    }

    private void InitEngine()
    {
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngineContext context = new RtcEngineContext();
        context.appId = _appId;
        context.channelProfile = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING;
        context.audioScenario = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT;
        context.areaCode = AREA_CODE.AREA_CODE_GLOB;
        RtcEngine.Initialize(context);
        RtcEngine.InitEventHandler(handler);
    }

    private void LoadAssetData()
    {
        if (_appIdInput == null) return;
        _appId = _appIdInput.appID;
        _tokenId = _appIdInput.token;
        _channelName = _appIdInput.channelName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void IntializeScreenCapture()
    {
        ScreenCaptureParameters2 screenCaptureParameters2 = new ScreenCaptureParameters2();
        screenCaptureParameters2.captureVideo = true;
        screenCaptureParameters2.captureAudio = true;
        RtcEngine.StartScreenCapture(screenCaptureParameters2);
    }

    void JoinChannel()
    {
        ChannelMediaOptions channelMediaOptions = new ChannelMediaOptions();
        channelMediaOptions.publishScreenCaptureVideo.SetValue(true);
        channelMediaOptions.publishScreenCaptureAudio.SetValue(true);
        channelMediaOptions.publishCameraTrack.SetValue(false);
        channelMediaOptions.publishMicrophoneTrack.SetValue(false);
        RtcEngine.JoinChannel(_tokenId, _appId, 0, channelMediaOptions);
    }
    public void OnScreenShare()
    {
        GettingScreenCaptureResources();
    }
    void GettingScreenCaptureResources()
    {
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
        Debug.Log("Executed else for Desktop");
        // Set the dimensions for the thumbnail and icon sizes
        SIZE thumbSize = new SIZE { width = 160, height = 90 };
        SIZE iconSize = new SIZE { width = 32, height = 32 };

        var sources = RtcEngine.GetScreenCaptureSources(thumbSize, iconSize, true);

        if (sources != null && sources.Length > 0)
        {
            foreach (var source in sources)
            {
                Debug.Log("Source name: " + source.sourceName);
            }

            // Example: Start screen capture by display ID
            var screenCaptureParameters = new ScreenCaptureParameters();
            screenCaptureParameters.frameRate = 15;
            screenCaptureParameters.bitrate = 0; // Agora will calculate bitrate automatically
            screenCaptureParameters.captureMouseCursor = true;

            var displayId = sources[0].sourceId; // Example: capture the first screen
            var nRet = RtcEngine.StartScreenCaptureByDisplayId((uint)displayId, new Rectangle(), screenCaptureParameters);
            Debug.Log("StartScreenCaptureByDisplayId :" + nRet);
        }
        else
        {
            Debug.LogError("No screen capture sources found.");
        }
#else
    Debug.Log("Executed else for Mobile");
    RtcEngine.StopScreenCapture();
#endif
    }
}
