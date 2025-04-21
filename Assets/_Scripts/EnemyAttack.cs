// EnemyAttack.cs
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyAttack : MonoBehaviour
{
    public Collider2D hitBox;  // sword trigger
    public bool hasWeapon = true;

    void Start()
    {
        // make sure hitbox is off at start
        if (hitBox) hitBox.enabled = false;
    }

    // animation event: turn on sword hitbox
    public void startAttackHitbox()
    {
        if (hasWeapon && hitBox) hitBox.enabled = true;
    }

    // animation event: turn off sword hitbox
    public void endAttackHitbox()
    {
        if (hasWeapon && hitBox) hitBox.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasWeapon || hitBox == null || !hitBox.enabled) return;
        if (!other.CompareTag("Player")) return;

        // if player is immune, do nothing
        var ih = other.GetComponent<ImmunityHandler>();
        if (ih != null && !ih.shouldTakeDamage())
            return;

        // otherwise respawn player
        other.GetComponent<PlayerMovement>()?.changeHealth(-1 * gameObject.GetComponent<EnemyController>().damage);
    }
}
