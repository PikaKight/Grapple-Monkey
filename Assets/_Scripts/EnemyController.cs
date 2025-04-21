using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float detectionRange = 10f;

    [Header("Attack")]
    public int damage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;

    public bool isAttacking { get; private set; }
    public bool isDead { get; private set; }

    private Transform _player;
    private Rigidbody2D _rb;
    private Animator _anim;
    private float _lastAttackTime;

    const string ANIM_WALK = "isRunning";
    const string ANIM_ATTACK = "attack";
    const string ANIM_DEATH = "death";

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (_player == null || isDead) return;
        float dist = Vector2.Distance(transform.position, _player.position);

        if (dist <= detectionRange)
        {
            if (dist <= attackRange)
            {
                if (Time.time >= _lastAttackTime + attackCooldown)
                {
                    StartAttack();
                    _lastAttackTime = Time.time;
                }
                _rb.linearVelocity = Vector2.zero;
                _anim.SetBool(ANIM_WALK, false);
            }
            else
            {
                MoveTowardsPlayer();
                _anim.SetBool(ANIM_WALK, true);
            }
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
            _anim.SetBool(ANIM_WALK, false);
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 dir = (_player.position - transform.position).normalized;
        _rb.linearVelocity = new Vector2(dir.x * moveSpeed, _rb.linearVelocity.y);

        if (dir.x > 0 && transform.localScale.x < 0) Flip();
        if (dir.x < 0 && transform.localScale.x > 0) Flip();
    }

    void Flip()
    {
        var s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    void StartAttack()
    {
        isAttacking = true;
        _anim.SetTrigger(ANIM_ATTACK);
    }

    // animation event
    public void EndAttack()
    {
        isAttacking = false;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        _anim.SetTrigger(ANIM_DEATH);
        GetComponent<Collider2D>().enabled = false;
        _rb.simulated = false;
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
