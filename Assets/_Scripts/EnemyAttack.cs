using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyAttack : MonoBehaviour
{
    [Tooltip("Your sword’s trigger collider")]
    public Collider2D hitBox;

    void Start()
    {
        if (hitBox) hitBox.enabled = false;
    }

    // Animation Event at the moment of impact
    public void StartAttackHitbox() => hitBox.enabled = true;

    // Animation Event at the last frame of attack
    public void EndAttackHitbox() => hitBox.enabled = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hitBox.enabled) return;
        if (!other.CompareTag("Player")) return;

        var pm = other.GetComponent<PlayerMovement>();
        if (pm != null) pm.Respawn();
    }
}
