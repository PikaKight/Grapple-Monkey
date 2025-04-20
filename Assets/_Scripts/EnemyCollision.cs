// EnemyCollision.cs
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyCollision : MonoBehaviour
{
    EnemyController _ctrl;

    void Awake()
    {
        // cache reference to our controller
        _ctrl = GetComponent<EnemyController>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;

        // if player is swinging or dashing, die
        if (PlayerMovement.sDown || PlayerMovement.dashing)
        {
            _ctrl.Die();
            return;
        }

        // if player is immune, ignore
        var ih = col.collider.GetComponent<ImmunityHandler>();
        if (ih != null && !ih.shouldTakeDamage())
            return;

        // otherwise respawn player
        col.collider.GetComponent<PlayerMovement>()?.Respawn();
    }
}
