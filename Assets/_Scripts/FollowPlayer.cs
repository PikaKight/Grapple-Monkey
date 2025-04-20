using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        // keep camera (or UI) following the player’s X
        if (player == null) return;
        float px = player.position.x;
        float py = player.position.y;
        transform.position = new Vector3(Mathf.Max(px, -10f), Mathf.Max(py - 2f, 0f), -10f);
    }
}
