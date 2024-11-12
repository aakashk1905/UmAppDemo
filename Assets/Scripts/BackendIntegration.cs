/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class BackendIntegration : MonoBehaviour
{
    private string baseURL = "https://api.upskillmafia.com/api/v1/user";
    [HideInInspector] public string _playerNameLoginTime;
    public InputField registerNameInput;
    public InputField registerEmailInput;
    public InputField registerPasswordInput;
    public InputField registerPhoneInput;
    public Canvas loader;

    public InputField loginEmailInput;
    public InputField loginPasswordInput;

    public GameObject _logInPage, _signUpPage;


    private void Start()
    {
        _logInPage.SetActive(true);
        loader.enabled = false;
        _signUpPage.SetActive(false);
    }

    public void Login()
    {
        string email = loginEmailInput.text;
        string password = loginPasswordInput.text;
        StartCoroutine(LoginRequest(email, password));
    }

    private IEnumerator LoginRequest(string email, string password)
    {
        loader.enabled = true;
        string loginURL = baseURL + "/login";

        // Create JSON payload
        string jsonPayload = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\"}";

        // Convert JSON string to byte array
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        // Create UnityWebRequest
        UnityWebRequest www = UnityWebRequest.PostWwwForm(loginURL, "POST");

        // Set upload handler
        www.uploadHandler = new UploadHandlerRaw(jsonBytes);
        www.uploadHandler.contentType = "application/json";

        // Set download handler
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Login request failed: " + www.error);
            loader.enabled = false;
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(jsonResponse);
           
            Debug.Log("Login successful. Response: " + www.downloadHandler.text);
            Debug.Log("Name ye Hai :" + loginResponse.user.name);
            SceneManager.LoadScene("Play");
        
        }
    }


  


    // Class to represent the login response JSON
    [Serializable]
    private class LoginResponse
    {
        public bool success;
        public string message;
        public UserData user;
    }

    [Serializable]
    private class UserData
    {
        public string name;
    }

    public void Register()
    {
        string name = registerNameInput.text;
        string email = registerEmailInput.text;
        string password = registerPasswordInput.text;
        string phone = registerPhoneInput.text;
        StartCoroutine(RegisterRequest(name, email, password, phone));
    }

    private IEnumerator RegisterRequest(string name, string email, string password, string mobile)
    {
        string registerURL = baseURL + "/register";
        string jsonPayload = "{\"name\":\"" + name + "\",\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"mobile\":\"" + mobile + "\"}";

        // Convert JSON string to byte array
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        // Create UnityWebRequest
        UnityWebRequest www = UnityWebRequest.PostWwwForm(registerURL, "POST");

        // Set upload handler
        www.uploadHandler = new UploadHandlerRaw(jsonBytes);
        www.uploadHandler.contentType = "application/json";

        // Set download handler
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Register request failed: " + www.error);
        }
        else
        {
            // Registration successful, handle response
            Debug.Log("Registration successful. Response: " + www.downloadHandler.text);
            _logInPage.SetActive(true);
            _signUpPage.SetActive(false);
        }
    }
}
*/




using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BackendIntegration : MonoBehaviour
{
    private const string baseURL = "https://api.upskillmafia.com/api/v1/user";
    private const string PlayerPrefsLoginKey = "UserLoginData";

    [HideInInspector] public string _playerNameLoginTime;
    public InputField registerNameInput;
    public InputField registerEmailInput;
    public InputField registerPasswordInput;
    public InputField registerPhoneInput;
    public Canvas loader;
   
    public InputField loginEmailInput;
    public InputField loginPasswordInput;

    public GameObject _logInPage, _signUpPage;

    void Start()
    {
       
        if (IsUserLoggedIn())
        {
            SceneManager.LoadScene("PlayMobile");
        }
        else
        {
            _logInPage.SetActive(true);
            loader.enabled = false;
            _signUpPage.SetActive(false);
        }
    }

    public void Login()
    {
        string email = loginEmailInput.text;
        string password = loginPasswordInput.text;
        StartCoroutine(LoginRequest(email, password));
    }
    

    private IEnumerator LoginRequest(string email, string password)
    {
        loader.enabled = true;
        string loginURL = baseURL + "/login";

        string jsonPayload = JsonUtility.ToJson(new LoginPayload { email = email, password = password });
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(loginURL, jsonPayload))
        {
            www.uploadHandler = new UploadHandlerRaw(jsonBytes);
            www.uploadHandler.contentType = "application/json";
            www.downloadHandler = new DownloadHandlerBuffer();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Login request failed: " + www.error);
                loader.enabled = false;
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(jsonResponse);

                Debug.Log("Login successful. Response: " + jsonResponse);
                Debug.Log("Name: " + loginResponse.user.name);

                UserDataManager.Instance.SaveUserData(loginResponse.user);

                Debug.LogError(UserDataManager.Instance.GetUserMobile());

                SceneManager.LoadScene("PlayMobile");
            }
        }
    }


    public static bool IsUserLoggedIn()
    {
        return !string.IsNullOrEmpty(UserDataManager.Instance.GetUserEmail());
    }

   /* public static UserData GetLoggedInUserData()
    {
        if (IsUserLoggedIn())
        {
            string jsonData = PlayerPrefs.GetString(PlayerPrefsLoginKey);
            return JsonUtility.FromJson<UserData>(jsonData);
        }
        return null;
    }*/

  

    [Serializable]
    private class LoginPayload
    {
        public string email;
        public string password;
    }

    [Serializable]
    private class LoginResponse
    {
        public bool success;
        public UserData user;
    }

    [Serializable]
    public class UserData
    {
        public string _id;
        public string name;
        public string email;
        public string mobile;
        public int closed;
        public string role;
        public UserDetails userDetails;
        public string lastLogin;
        public string createdAt;
        public string updatedAt;
    }

    [Serializable]
    public class UserDetails
    {
        public StreakData streakData;
        public string email;
        public string level;
        public int points;
        public int gems;
        public string lastSubmission;
        public string resume;
        public int credits;
        public List<string> refferals;
        public int pointsThisWeek;
        public List<string> streaksClaimed;
        public bool tutorialShown;
        public Redemptions redemptions;
    }

    [Serializable]
    public class StreakData
    {
        public int streak;
        public List<string> streakDates;
        public int longestStreak;
    }

    [Serializable]
    public class Redemptions
    {
        public List<RedemptionItem> gemsRedeemed;
        public List<RedemptionItem> creditsRedeemed;
    }

    [Serializable]
    public class RedemptionItem
    {
        public string product;
        public int gemsSpent;
        public int creditsSpent;
        public string redeemedOn;
    }
}