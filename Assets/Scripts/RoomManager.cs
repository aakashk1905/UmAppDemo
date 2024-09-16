using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; } 

    public List<GameObject> roomPrefabs; 
    public List<GameObject> wallColliderPrefabs;

    private Dictionary<string, GameObject> roomToColliderMap = new Dictionary<string, GameObject>();
    public string currentRoomName;
    public Button lockUnlockButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        for (int i = 0; i < roomPrefabs.Count; i++)
        {
            string roomName = roomPrefabs[i].GetComponent<RoomDetection>().roomName;

            if (i < wallColliderPrefabs.Count)
            {
                roomToColliderMap.Add(roomName, wallColliderPrefabs[i]);
                wallColliderPrefabs[i].SetActive(false); 
            }
        }

        lockUnlockButton.onClick.AddListener(ToggleRoomLock);
    }

    public bool spriteEnable;
    public void SetCurrentRoom(string roomName, PlayerController playerController)
    {
        currentRoomName = roomName; 

        if (playerController == null)
        {
            Debug.LogWarning("No PlayerController found in the scene.");
        }
        playerController.Rpc_UpdateIsInRoom(true);
        playerController.Rpc_SetChannelName(currentRoomName);

    }

    public void ClearCurrentRoom(PlayerController playerController)
    {
        currentRoomName = null;
        Debug.Log("Current room cleared. for " + playerController._playerID );
        playerController.Rpc_LeaveChannel(playerController.Object.InputAuthority, playerController._channelName.Value);
        playerController.Rpc_UpdateIsInRoom(false);
        playerController.Rpc_SetChannelName("");
        
    }
    void ToggleRoomLock()
    {
        if (!roomToColliderMap.TryGetValue(currentRoomName, out GameObject wallCollider))
        {
            Debug.LogWarning("No valid wall collider found to lock/unlock for room: " + currentRoomName);
            return;
        }

        wallCollider.SetActive(!wallCollider.activeSelf);
        Debug.Log((wallCollider.activeSelf ? "Locked" : "Unlocked") + " room: " + currentRoomName);
    }
}
