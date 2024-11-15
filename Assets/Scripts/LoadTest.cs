using UnityEngine.UI;
using UnityEngine;
#if(UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
using UnityEngine.Android;
#endif
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadTest : MonoBehaviour
{

    [SerializeField] private Image _micIcon,_videoIcon;
    [SerializeField] private Sprite _micOn, _micOff, _videoOff, _videoOn;
    private bool micOn, videoOn;


#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
    private ArrayList permissionList = new ArrayList();
#endif

    void Awake()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        permissionList.Add(Permission.Microphone);
        permissionList.Add(Permission.Camera);
#endif

        // keep this alive across scenes
        DontDestroyOnLoad(this.gameObject);
        micOn = true;
        videoOn = true;
    }
    void Update()
    {
        CheckPermissions();
    }
    private void CheckPermissions()
    {
#if (UNITY_2018_3_OR_NEWER && UNITY_ANDROID)
        foreach (string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                Permission.RequestUserPermission(permission);
            }
        }
#endif
    }

    public void MicBtn()
    {
        if (micOn)
        {
            micOn = false;
            _micIcon.sprite = _micOff;
        }
        else
        {
            micOn = true;
            _micIcon.sprite = _micOn;
       }
    }

    public void VideoBtn()
    {
        if (videoOn)
        {
            videoOn = false;
            _videoIcon.sprite = _videoOff;
        }
        else
        {
            videoOn = true;
            _videoIcon.sprite = _videoOn;
        }
    }
     public void LoadScene()
    {
        string sceneName = "PlayMobile";
        SceneManager.LoadScene(sceneName);
    }
}
