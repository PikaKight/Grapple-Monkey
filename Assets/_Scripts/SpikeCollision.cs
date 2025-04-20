using UnityEngine;

public class SpikeCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        var pm = collision.collider.GetComponent<PlayerMovement>();
        var ih = collision.collider.GetComponent<ImmunityHandler>();

        // if Immune, skip damage
        if (ih != null && !ih.ShouldTakeDamage())
            return;

        // otherwise respawn
        pm?.Respawn();
    }
}
