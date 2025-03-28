using UnityEngine;

public class SpikeCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        collision.collider.gameObject.transform.position = new Vector3(-18, -8, 0);
        PlayerMovement.canSwing = 0;
    }
}
