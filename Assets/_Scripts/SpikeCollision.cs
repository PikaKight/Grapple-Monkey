// SpikeCollision.cs
using UnityEngine;

public class SpikeCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;

        // if player is immune, ignore spikes
        var ih = col.collider.GetComponent<ImmunityHandler>();
        if (ih != null && !ih.shouldTakeDamage())
            return;

        // otherwise send player back to spawn
        col.collider.GetComponent<PlayerMovement>()?.Respawn();
    }
}
