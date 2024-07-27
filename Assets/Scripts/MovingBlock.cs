using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class MovingBlock : MonoBehaviour
{
    public float moveDistance = 3f;

    void Update()
    {
        // Check if the "J" key is pressed
        if (Input.GetKeyDown(KeyCode.J))
        {
            // Move the player 3 units to the right
            transform.Translate(Vector3.right * moveDistance);
        }

        // Check if the "K" key is pressed
        if (Input.GetKeyDown(KeyCode.K))
        {
            // Move the player 3 units to the left
            transform.Translate(Vector3.left * moveDistance);
        }
    }

    public void MoveRight()
    {
        transform.Translate(Vector3.right * moveDistance);
    }
    public void MoveLeft()
    {
        transform.Translate(Vector3.left * moveDistance);
    }
}
