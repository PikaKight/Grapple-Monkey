using UnityEngine;

public class SpikeCollision : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.collider.CompareTag("Player")) return;

        var pm = col.collider.GetComponent<PlayerMovement>();

        if (pm != null) pm.Respawn();
    }
}
