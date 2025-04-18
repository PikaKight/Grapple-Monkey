using UnityEngine;

public class SpikeCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.collider.gameObject.transform.position = new Vector3(-18, -8, 0);
            PlayerMovement.canSwing = 0;
        }
        
        
    }
}
