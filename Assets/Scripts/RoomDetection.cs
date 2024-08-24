using UnityEngine;

public class RoomDetection : MonoBehaviour
{
    public string roomName; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RoomManager.Instance.SetCurrentRoom(roomName);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RoomManager.Instance.ClearCurrentRoom();
        }
    }
}
