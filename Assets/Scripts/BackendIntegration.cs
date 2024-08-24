using UnityEngine;
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

    public InputField loginEmailInput;
    public InputField loginPasswordInput;

    public GameObject _logInPage, _signUpPage;


    private void Start()
    {
        _logInPage.SetActive(true);
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
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(jsonResponse);
            // Login successful, handle response
            Debug.Log("Login successful. Response: " + www.downloadHandler.text);
            Debug.Log("Name ye Hai :" + loginResponse.user.name);
            //_playerNameLoginTime = loginResponse.user.name;
            //PhotonNetwork.NickName = loginResponse.user.name;

            //PlayerPrefs.SetString("PlayerName", loginResponse.user.name);
            //SceneManager.LoadScene("Lobby");
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

    // Class to represent user data in the login response JSON
    [Serializable]
    private class UserData
    {
        public string name;
        // Add other user data fields here if needed
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
        //PlayerPrefs.SetString("PlayerName", name);
        // Create JSON payload
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
