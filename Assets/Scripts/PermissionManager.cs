using UnityEngine;
using UnityEngine.Android;

public class PermissionManager : MonoBehaviour
{
    void Start()
    {
        // Request all permissions at once
        RequestPermissions();
    }

    void RequestPermissions()
    {
        // Check and request Microphone Permission
        bool micPermission = !Permission.HasUserAuthorizedPermission(Permission.Microphone);
        bool cameraPermission = !Permission.HasUserAuthorizedPermission(Permission.Camera);
        bool readStoragePermission = !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead);
        bool writeStoragePermission = !Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite);

        // Request permissions only if not already granted
        if (micPermission || cameraPermission || readStoragePermission || writeStoragePermission)
        {
            // This will trigger the permission dialogs for all denied permissions at once
            if (micPermission) Permission.RequestUserPermission(Permission.Microphone);
            if (cameraPermission) Permission.RequestUserPermission(Permission.Camera);
            if (readStoragePermission) Permission.RequestUserPermission(Permission.ExternalStorageRead);
            if (writeStoragePermission) Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
    }
}
