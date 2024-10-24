using UnityEngine;
using System.Collections;
using UnityEngine.Android;
using System.Collections.Generic;

public class PermissionsManager : MonoBehaviour
{
    private static readonly string[] RequiredPermissions = new string[]
    {
        Permission.Camera,
        Permission.Microphone,
        Permission.ExternalStorageWrite,
        Permission.ExternalStorageRead
    };

    private void Start()
    {
        StartCoroutine(RequestAllPermissions());
    }

    private IEnumerator RequestAllPermissions()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            var permissionsToRequest = new List<string>();

            foreach (string permission in RequiredPermissions)
            {
                if (!Permission.HasUserAuthorizedPermission(permission))
                {
                    permissionsToRequest.Add(permission);
                }
            }

            if (permissionsToRequest.Count > 0)
            {
                var permissionsArray = permissionsToRequest.ToArray();
                Permission.RequestUserPermissions(permissionsArray);

                // Wait for permissions to be granted
                while (true)
                {
                    bool allGranted = true;
                    foreach (string permission in permissionsArray)
                    {
                        if (!Permission.HasUserAuthorizedPermission(permission))
                        {
                            allGranted = false;
                            break;
                        }
                    }

                    if (allGranted)
                    {
                        break;
                    }

                    yield return new WaitForSeconds(0.5f);
                }

                Debug.Log("All permissions have been granted.");
            }
            else
            {
                Debug.Log("All required permissions are already granted.");
            }
        }
        else
        {
            Debug.Log("Not running on Android. Permissions are assumed to be granted.");
        }
    }

    public bool HasCameraPermission()
    {
        return Application.platform != RuntimePlatform.Android || Permission.HasUserAuthorizedPermission(Permission.Camera);
    }

    public bool HasMicrophonePermission()
    {
        return Application.platform != RuntimePlatform.Android || Permission.HasUserAuthorizedPermission(Permission.Microphone);
    }

    public bool HasStoragePermission()
    {
        return Application.platform != RuntimePlatform.Android ||
               (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) &&
                Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead));
    }
}