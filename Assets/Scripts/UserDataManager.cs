using UnityEngine;
using System.IO;

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
        string filePath = Path.Combine(Application.persistentDataPath, UserDataFileName);
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
        string filePath = Path.Combine(Application.persistentDataPath, UserDataFileName);
        File.WriteAllText(filePath, jsonData);
    }

    public void LogOut()
    {
        CurrentUser = null;
        string filePath = Path.Combine(Application.persistentDataPath, UserDataFileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
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