using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision){
        Debug.Log("Collision");
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
