using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyAttack : MonoBehaviour
{
    [Header("Enemy Type")]
    public bool hasWeapon = false;
    
    [Tooltip("Your sword’s trigger collider")]
    public Collider2D hitBox;

    void Start()
    {
        if (hitBox) hitBox.enabled = false;
    }

    // Animation Event at the moment of impact
    public void StartAttackHitbox()
    {
        if (!hasWeapon) return;
        hitBox.enabled = true;
    }
    // Animation Event at the last frame of attack
    public void EndAttackHitbox() { 
        if (!hasWeapon) return;
        hitBox.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if ( hasWeapon && !hitBox.enabled) return;

        var pm = other.GetComponent<PlayerMovement>();
        if (pm != null) pm.Respawn();
    }
}
