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

    // State
    private Vector3 _spawnPoint;
    private bool isDead = false;         // BLOCKS Update until respawn

    public static bool sDown, dashing;
    public static float canSwing, dashCooldown;

    private bool canDash, canDoubleJump, facingRight = true;
    private int jumpCount;
    private const float maxSwingTime = 2f, maxDashCooldown = 5f;

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
            return;
        }
        if (Input.GetKey(KeyCode.S) && sDown && canSwing < maxSwingTime)
        {
            ContinueGrapple();
            return;
        }
        EndGrappleCleanup();

        // 3) Movement & Abilities
        HandleMovement();
    }

    void StartGrapple()
    {
        sDown = true;
        originalPos = new Vector3(body.position.x + 4f, 9.5f, 0f);
        body.gravityScale = 0f;
        lineRenderer.enabled = true;
        body.Sleep();
    }

    void ContinueGrapple()
    {
        canSwing += Time.deltaTime;
        lineRenderer.SetPosition(0, body.position);
        lineRenderer.SetPosition(1, originalPos);
        Physics.gravity = Vector2.zero;
        body.position = new Vector2(transform.position.x + 0.2f, body.position.y);
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
        if (canDash && dashCooldown <= 0f && Input.GetKeyDown(KeyCode.LeftShift))
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

            // jumping - michael
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                body.AddForce(Vector2.up * 1000);
            }
            // if on second level, dashing is enabled - michael
            if (SceneManager.GetActiveScene().name == "Level 2" && dashCooldown <= 0 && Input.GetMouseButtonDown(0))
            {
                dashing = true;
                dashCooldown = maxDashCooldown; // set cooldown with delta time based value - michael
                float dashSpeed = 20f;

                Vector2 playerScreenPosition = Camera.main.WorldToScreenPoint(body.transform.position);
                Vector2 mouseScreenPosition = Input.mousePosition;

                Vector2 playerToMouseVector = (mouseScreenPosition - playerScreenPosition).normalized;
                // debug: log player to mouse vector - michael

                body.linearVelocity = playerToMouseVector * dashSpeed; // using velocity property - michael
            }
        }
        dashCooldown -= Time.deltaTime; // subtract delta time for cooldown - michael
        if (dashCooldown < 0) dashCooldown = 0; // clamp to zero - michael
    }

    // check if user is touching ground - michael
    bool IsGrounded()
    {
        foreach (Collider2D groundCollider in ground.GetComponentsInChildren<Collider2D>())
        {
            if (collider.IsTouching(groundCollider))
            {
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
