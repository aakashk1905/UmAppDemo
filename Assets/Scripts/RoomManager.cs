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

    
    public void SetCurrentRoom(string roomName)
    {
        currentRoomName = roomName; 
        Debug.Log("Current room set to: " + currentRoomName);
    }

    public void ClearCurrentRoom()
    {
        currentRoomName = null;
        Debug.Log("Current room cleared.");
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
