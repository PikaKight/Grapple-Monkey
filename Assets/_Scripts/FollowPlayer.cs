using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        // keep camera (or UI) following the player’s X
        if (player == null) return;
        float px = player.position.x;
        transform.position = new Vector3(Mathf.Max(px, -10f), 0f, -10f);
    }
}
