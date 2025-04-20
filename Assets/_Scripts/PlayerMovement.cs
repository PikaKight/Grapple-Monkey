using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public Rigidbody2D body;
    public BoxCollider2D playerCollider;
    public float moveSpeed = 8f;
    public GameObject ground;

    [Header("Animation")]
    public Animator animator;

    [Header("Grapple Settings")]
    [Range(0.01f, 0.5f)] public float lineWidth = 0.05f;

    [Header("Jump Settings")]
    [Range(100f, 2000f)] public float jumpForce = 600f;

    [Header("Respawn Settings")]
    [Tooltip("Delay in seconds before actually teleporting back")]
    public float respawnDelay = 1f;

    // Grapple internals
    private LineRenderer lineRenderer;
    private Vector3 originalPos;
    private float swingRadius = 10f;
    private float swingSpeed = 2f;
    private float swingAngle;
    private Vector2 startingPoint;

    // State
    private Vector3 _spawnPoint;
    private bool isDead = false;         // BLOCKS Update until respawn

    public static bool sDown, dashing;
    public static float canSwing, dashCooldown;

    private bool canDash, canDoubleJump, facingRight = true;
    private int jumpCount;
    private const float maxSwingTime = 1.5f, maxDashCooldown = 5f;

    void Start()
    {
        _spawnPoint = transform.position;

        // — Grapple line setup —
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
        lineRenderer.widthMultiplier = lineWidth;

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        lineRenderer.enabled = false;

        // Unlock dash/double‑jump by scene name
        var lvl = SceneManager.GetActiveScene().name;
        canDash = lvl == "Level 2" || lvl == "Level 3";
        canDoubleJump = lvl == "Level 3";
    }

    void Update()
    {
        // 1) Block all input while “dead”
        if (isDead) return;

        // 2) Grapple
        if (Input.GetKey(KeyCode.S) && !sDown && canSwing == 0f)
        {
            StartGrapple();
        }
        if (Input.GetKey(KeyCode.S) && sDown && canSwing < maxSwingTime)
        {
            ContinueGrapple();
        }
        else
        {
            EndGrappleCleanup();
        }

        // 3) Movement & Abilities
        HandleMovement();
    }

    void StartGrapple()
    {
        sDown = true;
        startingPoint = body.position;
        originalPos = new Vector2(body.position.x + swingRadius, 20.0f);
        body.gravityScale = 0;
        lineRenderer.enabled = true;
        swingAngle = 0f;
        body.Sleep();
    }

    void ContinueGrapple()
    {
        canSwing += Time.deltaTime;
        lineRenderer.SetPosition(0, body.position);
        lineRenderer.SetPosition(1, originalPos);
        Physics.gravity = new Vector2(0, 0);
        swingAngle += Time.deltaTime * swingSpeed;
        float x = Mathf.Cos(swingAngle) * swingRadius;
        float y = Mathf.Sin(swingAngle) * swingRadius * 0.1f;
        body.MovePosition(new Vector2(originalPos.x - x, startingPoint.y - y));
    }

    void EndGrappleCleanup()
    {
        canSwing -= Time.deltaTime;
        if (canSwing < 0f) canSwing = 0f;
        sDown = false;
        body.gravityScale = 3f;
        lineRenderer.enabled = false;
        Physics.gravity = new Vector2(0, -9.81f);
    }

    void HandleMovement()
    {
        bool grounded = IsGrounded();
        animator.SetBool("isGrounded", grounded);

        if (grounded) jumpCount = 0;

        float h = Input.GetAxis("Horizontal");
        animator.SetBool("isRunning", Mathf.Abs(h) > 0.1f);

        if (!dashing)
            body.linearVelocity = new Vector2(h * moveSpeed, body.linearVelocity.y);

        // Flip sprite
        if (h > 0 && !facingRight) Flip();
        if (h < 0 && facingRight) Flip();

        // Jump & Double‑Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded) DoJump();
            else if (canDoubleJump && jumpCount < 1)
            {
                jumpCount++;
                DoJump();
            }
        }

        // Dash
        if (canDash && dashCooldown <= 0f && Input.GetMouseButtonDown(0))
            StartDash();

        dashCooldown = Mathf.Max(0f, dashCooldown - Time.deltaTime);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        var s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    private void DoJump()
    {
        animator.SetTrigger("jump");
        body.linearVelocity = new Vector2(body.linearVelocity.x, 0f);
        body.AddForce(Vector2.up * jumpForce);
    }

    private void StartDash()
    {
        dashing = true;
        dashCooldown = maxDashCooldown;
        animator.SetTrigger("dash");
        float dashSpeed = 20f;

        Vector2 playerScreenPosition = Camera.main.WorldToScreenPoint(body.transform.position);
        Vector2 mouseScreenPosition = Input.mousePosition;

        Vector2 playerToMouseVector = (mouseScreenPosition - playerScreenPosition).normalized;

        body.linearVelocity = playerToMouseVector * dashSpeed;
    }

    private void EndDash() => dashing = false;

    private bool IsGrounded()
    {
        EndDash();
        foreach (var gc in ground.GetComponentsInChildren<Collider2D>())
            if (playerCollider.IsTouching(gc))
                return true;
        return false;
    }

    /// <summary>
    /// Call this to trigger death+respawn.
    /// </summary>
    public void Respawn()
    {
        if (isDead) return;
        isDead = true;
        animator.SetTrigger("dead");
        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        // teleport back
        transform.position = _spawnPoint;
        body.linearVelocity = Vector2.zero;
        body.gravityScale = 3f;

        // reset all state
        sDown = false;
        dashing = false;
        canSwing = 0f;
        dashCooldown = 0f;

        // clear death trigger, animator will fall back to Idle
        animator.ResetTrigger("dead");
        animator.SetBool("isRunning", false);
        animator.SetBool("isGrounded", true);

        isDead = false;
    }
}