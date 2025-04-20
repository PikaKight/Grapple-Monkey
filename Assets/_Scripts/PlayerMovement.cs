using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public Rigidbody2D body;
    public BoxCollider2D playerCollider;
    public float moveSpeed = 8f;
    public float jumpForce = 600f;
    public GameObject ground;

    [Header("Grapple Settings")]
    [Range(0.01f, 0.5f)]
    public float lineWidth = 0.05f;
    public float maxSwingTime = 1.5f;

    [Header("Dash Settings")]
    public bool enableDash = true;
    public float dashCooldownMax = 5f;
    public float dashSpeed = 20f;

    [Header("Respawn Settings")]
    [Tooltip("delay in seconds before teleporting back")]
    public float respawnDelay = 1f;

    // internals
    private LineRenderer lineRenderer;
    private Vector3 swingAnchor;
    private Vector3 spawnPoint;
    private float swingTimer;
    private bool swinging;
    private bool facingRight = true;
    private bool isDead;

    // state
    public static bool sDown, dashing;
    public static float canSwing, dashCooldown;
    private int jumpCount;

    void Start()
    {
        //testing REMOVE BEFORE HANDIN

        PlayerPrefs.SetInt("Flames", 5);


        spawnPoint = transform.position;

        // grapple line setup
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
    }

    void Update()
    {
        if (isDead) return;

        HandleGrapple();
        HandleMovement();
    }

    void HandleGrapple()
    {
        // start swing
        if (Input.GetKey(KeyCode.S) && !sDown && canSwing <= 0f)
        {
            sDown = true;
            swinging = true;
            swingTimer = 0f;
            lineRenderer.enabled = true;
            swingAnchor = transform.position + Vector3.right * 2f;
            body.gravityScale = 0f;
            body.Sleep();
        }
        // continue swing
        if (Input.GetKey(KeyCode.S) && swinging && swingTimer < maxSwingTime)
        {
            swingTimer += Time.deltaTime;
            canSwing = swingTimer;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, swingAnchor);
            Physics.gravity = Vector2.zero;
            return; // skip normal movement
        }
        // cleanup
        if (!Input.GetKey(KeyCode.S) || swingTimer >= maxSwingTime)
        {
            swinging = false;
            sDown = false;
            canSwing = Mathf.Max(0f, canSwing - Time.deltaTime);
            body.gravityScale = 3f;
            lineRenderer.enabled = false;
            Physics.gravity = Vector2.down * 9.81f;
        }
    }

    void HandleMovement()
    {
        bool grounded = IsGrounded();
        GetComponent<Animator>().SetBool("isGrounded", grounded);
        if (grounded) jumpCount = 0;

        float h = Input.GetAxis("Horizontal");
        GetComponent<Animator>().SetBool("isRunning", Mathf.Abs(h) > 0.1f);

        // horizontal
        if (!dashing)
            body.linearVelocity = new Vector2(h * moveSpeed, body.linearVelocity.y);

        // flip
        if (h > 0f && !facingRight) Flip();
        if (h < 0f && facingRight) Flip();

        // jump & double
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded) DoJump();
            else if (jumpCount == 0) { jumpCount++; DoJump(); }
        }

        // dash
        if (enableDash && dashCooldown <= 0f && Input.GetKeyDown(KeyCode.LeftShift))
            StartDash();

        dashCooldown = Mathf.Max(0f, dashCooldown - Time.deltaTime);
    }

    void DoJump()
    {
        var anim = GetComponent<Animator>();
        anim.SetTrigger("jump");
        body.linearVelocity = new Vector2(body.linearVelocity.x, 0f);
        body.AddForce(Vector2.up * jumpForce);
    }

    void StartDash()
    {
        dashing = true;
        dashCooldown = dashCooldownMax;
        GetComponent<Animator>().SetTrigger("dash");
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        body.linearVelocity = dir * dashSpeed;
        Invoke(nameof(EndDash), 0.1f);
    }

    void EndDash() => dashing = false;

    bool IsGrounded()
    {
        foreach (var gc in ground.GetComponentsInChildren<Collider2D>())
            if (playerCollider.IsTouching(gc)) return true;
        return false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        var s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    public void Respawn()
    {
        if (isDead) return;
        isDead = true;
        GetComponent<Animator>().SetTrigger("dead");
        StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        // teleport back to spawn
        transform.position = spawnPoint;
        body.linearVelocity = Vector2.zero;
        body.gravityScale = 3f;

        // reset swing/dash state
        sDown = dashing = false;
        canSwing = dashCooldown = 0f;

        // reset animation
        var anim = GetComponent<Animator>();
        anim.ResetTrigger("dead");
        anim.Play("Idle");
        isDead = false;
    }

    /// <summary>
    /// called by NPC to give temporary speed boost
    /// </summary>
    public IEnumerator BoostSpeed(float amount, float duration)
    {
        moveSpeed += amount;
        yield return new WaitForSeconds(duration);
        moveSpeed -= amount;
    }
}