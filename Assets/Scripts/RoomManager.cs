using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; } 

    public List<GameObject> roomPrefabs; 
    public List<GameObject> wallColliderPrefabs;

    private Dictionary<string, GameObject> roomToColliderMap = new Dictionary<string, GameObject>();
    private string currentRoomName;
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
    public void SetCurrentRoom(string roomName)
    {
        currentRoomName = roomName; 
        Debug.Log("Current room set to: " + currentRoomName);

        //Acessing the PlayerController script and set channel name to roomName
        PlayerController playerController = GameObject.FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("No PlayerController found in the scene.");
        }
        playerController.Rpc_UpdateCharacterinRoom(false);
        playerController.Rpc_SetChannelName(currentRoomName);

        //Turning off the Circle Collider 2D and Sprite Renderer of "Range"
        
    }

    public void ClearCurrentRoom()
    {
        currentRoomName = null;
        Debug.Log("Current room cleared.");

        //Turn on the Circle Collider 2D of the player
        PlayerController playerController = GameObject.FindObjectOfType<PlayerController>();
        playerController.Rpc_LeaveChannel(playerController.Object.InputAuthority, playerController._channelName.Value);
        

        playerController.Rpc_UpdateCharacterinRoom(true);
        playerController.Rpc_SetChannelName("");
        

        //Turing on the sprite Renderer of GameObject "Range"
    

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
