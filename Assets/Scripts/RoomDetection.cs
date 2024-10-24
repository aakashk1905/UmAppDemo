using UnityEngine;

public class RoomDetection : MonoBehaviour
{
    public string roomName;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        BoxCollider2D mybox = GetComponent<BoxCollider2D>();
        Debug.LogError(mybox.isTrigger);
        if (!mybox.isTrigger)
        {
            return;
        }
        if (collision.CompareTag("Range") )
        {
            PlayerController playerController = GetPlayerControllerFromRange(collision);
            if (playerController != null && !playerController.IsInRoom )
            {
                RoomManager.Instance.SetCurrentRoom(roomName,playerController);
                
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        BoxCollider2D mybox = GetComponent<BoxCollider2D>();

        if (!mybox.isTrigger)
        {
            return;
        }

        if (collision.CompareTag("Range") && RoomManager.Instance.currentRoomName == roomName)
        {
            PlayerController playerController = GetPlayerControllerFromRange(collision);
            if (playerController != null)
            {
                RoomManager.Instance.ClearCurrentRoom(playerController);
            }
        }
    }

    private PlayerController GetPlayerControllerFromRange(Collider2D rangeCollider)
    {
        Transform playerTransform = rangeCollider.transform.parent;

        if (playerTransform != null)
        {
            return playerTransform.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogError("Could not find parent of Range object");
            return null;
        }
    }
}