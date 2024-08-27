using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;



namespace Agora_RTC_Plugin.API_Example
{
    [Serializable]
    public class TokenObject
    {
        public string token;
    }


    public static class HelperClass
    {
        public static IEnumerator FetchToken(string url, string channel, long userId, Action<string> callback = null)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(string.Format("{0}?channel={1}&userId={2}", url, channel, userId)))
            {
                
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("Error: " + request.error);
                    callback?.Invoke(null);
                    yield break;
                }

                TokenObject tokenInfo = JsonUtility.FromJson<TokenObject>(request.downloadHandler.text);
                callback?.Invoke(tokenInfo.token);
            }
        }
    }
}