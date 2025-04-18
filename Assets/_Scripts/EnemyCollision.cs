using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.tag == "Player")
        {
            if (PlayerMovement.sDown || PlayerMovement.dashing)
            {
                Destroy(gameObject);
            }
            else
            {
                collision.collider.gameObject.transform.position = new Vector3(-18, -8, 0);
                PlayerMovement.canSwing = 0;
            }
        }
        
    }
}
