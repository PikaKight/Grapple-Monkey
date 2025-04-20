// RushingEnemyController.cs
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class RushingEnemyController : MonoBehaviour
{
    public float detectionRange = 15f;  // how close before we start
    public float rushDelay = 0.5f;      // wait before lunging
    public float cooldownTime = 2f;     // seconds between rushes
    public float rushSpeed = 25f;       // rush velocity

    bool _onCooldown;
    public bool isRushing { get; private set; }

    Transform _player;
    Rigidbody2D _rb;
    Animator _anim;

    void Awake()
    {
        _player = GameObject.FindWithTag("Player")?.transform;
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (_player == null || isRushing || _onCooldown) return;

        float dist = Vector2.Distance(transform.position, _player.position);
        if (dist <= detectionRange)
            StartCoroutine(doRush(_player.position));
    }

    IEnumerator doRush(Vector3 targetPos)
    {
        _onCooldown = true;
        yield return new WaitForSeconds(rushDelay);

        // begin rush
        isRushing = true;
        _anim.SetBool("isRunning", true);

        Vector2 dir = (targetPos - transform.position).normalized;
        float traveled = 0f;
        float distance = Vector2.Distance(transform.position, targetPos);

        while (traveled < distance)
        {
            _rb.linearVelocity = dir * rushSpeed;
            traveled += rushSpeed * Time.deltaTime;
            yield return null;
        }

        // end rush
        _rb.linearVelocity = Vector2.zero;
        isRushing = false;
        _anim.SetBool("isRunning", false);

        // wait before next rush
        yield return new WaitForSeconds(cooldownTime);
        _onCooldown = false;
    }
}
