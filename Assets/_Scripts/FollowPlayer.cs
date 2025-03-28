using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    // Update is called once per frame
    void Update()
    {
        if ((player.position.x > transform.position.x || player.position.x < transform.position.x) && player.position.x > -10)
        {
            transform.position = new Vector3(player.position.x, 0, -10);
        }

        if (player.position.x < -10)
        {
            transform.position = new Vector3(-10, 0, -10);
        }
    }
}
