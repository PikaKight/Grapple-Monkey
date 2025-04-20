using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyAttack : MonoBehaviour
{
    [Header("weapon settings")]
    public bool hasWeapon = true;
    [Tooltip("your sword’s trigger collider")]
    public Collider2D hitBox;

    void Start()
    {
        if (hitBox)
            hitBox.enabled = false;
    }

    // called via Animation Event at the moment of impact
    public void StartAttackHitbox()
    {
        if (hasWeapon && hitBox)
            hitBox.enabled = true;
    }

    // called via Animation Event on the last frame
    public void EndAttackHitbox()
    {
        if (hasWeapon && hitBox)
            hitBox.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // only hurt when the hitBox is active
        if (!hasWeapon || hitBox == null || !hitBox.enabled) return;
        if (!other.CompareTag("Player")) return;

        var pm = other.GetComponent<PlayerMovement>();
        var ih = other.GetComponent<ImmunityHandler>();

        // if immune, skip damage
        if (ih != null && !ih.ShouldTakeDamage())
            return;

        pm?.Respawn();
    }
}
