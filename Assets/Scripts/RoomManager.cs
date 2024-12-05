using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; } 

    public List<GameObject> roomPrefabs; 
    public List<GameObject> wallColliderPrefabs;

    private Dictionary<string, GameObject> roomToColliderMap = new Dictionary<string, GameObject>();
    public string currentRoomName;
    public Button lockRoom;

    [SerializeField] private TextMeshProUGUI lockRoomText;
    [SerializeField] private GameObject myDashBoarButton;

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
        lockRoom.gameObject.SetActive(false);

        for (int i = 0; i < roomPrefabs.Count; i++)
        {
            string roomName = roomPrefabs[i].GetComponent<RoomDetection>().roomName;

            if (i < wallColliderPrefabs.Count)
            {
                roomToColliderMap.Add(roomName, wallColliderPrefabs[i]);
                wallColliderPrefabs[i].SetActive(false); 
            }
        }

        lockRoom.onClick.AddListener(ToggleRoomLock);
    }

    public bool spriteEnable;
    public void SetCurrentRoom(string roomName, PlayerController playerController)
    {
        myDashBoarButton.SetActive(true);
        string roomNameInitials = TransformString(roomName);
        lockRoomText.text = "Lock Room : " + roomNameInitials;
        lockRoom.gameObject.SetActive(true);
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
        myDashBoarButton?.SetActive(false);
        lockRoomText.text = "Lock Room";
        lockRoom.gameObject.SetActive(false);

        currentRoomName = null;
      
        playerController.Rpc_LeaveChannel(playerController.Object.InputAuthority, playerController._channelName.Value);
        playerController.Rpc_UpdateIsInRoom(false);
        playerController.Rpc_SetChannelName("");
        
    }

    string TransformString(string input)
    {
        // Split the string into words by spaces
        string[] words = input.Split(' ');

        // Initialize an empty string to store the result
        string result = "";

        // Loop through the words (except the last one, which is a number)
        for (int i = 0; i < words.Length - 1; i++)
        {
            // Add the first letter of each word
            result += words[i][0];
        }

        // Add the last part (the number) without change
        result += words[words.Length - 1];

        return result;
    }
    void ToggleRoomLock()
    {
        //if (!roomToColliderMap.TryGetValue(currentRoomName, out GameObject wallCollider))
        //{
        //    Debug.LogWarning("No valid wall collider found to lock/unlock for room: " + currentRoomName);
        //    return;
        //}

        //wallCollider.SetActive(!wallCollider.activeSelf);
        //Debug.Log((wallCollider.activeSelf ? "Locked" : "Unlocked") + " room: " + currentRoomName);

        RoomDetection currentRoom = GameObject.Find(currentRoomName).GetComponent<RoomDetection>();

        currentRoom.LockRoom();
    }
}
