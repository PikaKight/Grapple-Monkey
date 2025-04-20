// SpikeCollision.cs
using UnityEngine;

public class SpikeCollision : MonoBehaviour
{
    public int damage = 5;
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        // if player is immune, ignore spikes
        var ih = col.gameObject.GetComponent<ImmunityHandler>();
        if (ih != null && !ih.shouldTakeDamage())
            return;

        // otherwise send player back to spawn
        col.gameObject.GetComponent<PlayerMovement>()?.changeHealth(-1 * damage);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        // if player is immune, ignore spikes
        var ih = col.gameObject.GetComponent<ImmunityHandler>();
        if (ih != null && !ih.shouldTakeDamage())
            return;

        // otherwise send player back to spawn
        col.gameObject.GetComponent<PlayerMovement>()?.changeHealth(-1 * damage);
    }
}
