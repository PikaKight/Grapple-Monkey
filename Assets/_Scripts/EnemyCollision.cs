using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyCollision : MonoBehaviour
{
    private EnemyController _ctrl;

    void Awake()
    {
        _ctrl = GetComponent<EnemyController>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;

        // if player is mid‑swing or dashing, kill the enemy
        if (PlayerMovement.sDown || PlayerMovement.dashing)
            _ctrl.Die();
    }
}
