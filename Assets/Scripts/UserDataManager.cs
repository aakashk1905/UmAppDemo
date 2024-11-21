using System.IO;
using UnityEngine;

public class UserDataManager : MonoBehaviour
{
    public static UserDataManager Instance { get; private set; }

    public BackendIntegration.UserData CurrentUser { get; private set; }

    private const string UserDataFileName = "user_data.json";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUserData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadUserData()
    {
        string filePath = GetUserDataFilePath();
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            CurrentUser = JsonUtility.FromJson<BackendIntegration.UserData>(jsonData);
        }
    }


    public void SaveUserData(BackendIntegration.UserData userData)
    {
        CurrentUser = userData;
        string jsonData = JsonUtility.ToJson(userData);
        string filePath = GetUserDataFilePath();
        File.WriteAllText(filePath, jsonData);
    }

    public void LogOut()
    {
        CurrentUser = null;
        string filePath = GetUserDataFilePath();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    // Use a unique file path depending on whether we're in the Editor or Build
    private string GetUserDataFilePath()
    {
        string platformSuffix = Application.isEditor ? "Editor_" : "Build_";
        return Path.Combine(Application.persistentDataPath, platformSuffix + UserDataFileName);
    }

    public string GetUserName() => CurrentUser?.name;
    public string GetUserEmail() => CurrentUser?.email;
    public string GetUserMobile() => CurrentUser?.mobile;
    public string GetUserRole() => CurrentUser?.role;
    public int GetUserGems() => CurrentUser?.userDetails?.gems ?? 0;
    public int GetUserPoints() => CurrentUser?.userDetails?.points ?? 0;
    public string GetUserLevel() => CurrentUser?.userDetails?.level;
    public int GetUserStreak() => CurrentUser?.userDetails?.streakData?.streak ?? 0;
}
