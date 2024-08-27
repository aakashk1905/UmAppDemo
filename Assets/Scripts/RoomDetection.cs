using UnityEngine;

public class RoomDetection : MonoBehaviour
{
    public string roomName; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.LogError("collision detected"+ collision.name);
        if (collision.CompareTag("Range"))
        {
            RoomManager.Instance.SetCurrentRoom(roomName);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Range"))
        {
            RoomManager.Instance.ClearCurrentRoom();
        }
    }
}
