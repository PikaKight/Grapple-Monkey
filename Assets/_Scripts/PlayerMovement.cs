// PlayerMovement.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Status")]
    public int maxHealth = 100;
    public float timeInvincible = 2.0f;

    [Header("Movement")]
    public Rigidbody2D body;
    public BoxCollider2D playerCollider;
    public float moveSpeed = 8f;
    public float jumpForce = 600f;
    public GameObject ground;

    [Header("Grapple Settings")]
    [Range(0.01f, 0.5f)]
    public float lineWidth = 0.05f;
    public float maxSwingTime = 1.5f, maxDashCooldown = 5f;

    [Header("Dash Settings")]
    public bool enableDash = true;
    public float dashCooldownMax = 5f;
    public float dashSpeed = 20f;

    [Header("Respawn Settings")]
    [Tooltip("delay in seconds before teleporting back")]
    public float respawnDelay = 1f;

    [Header("UI Texts")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI flamesText;

    // Health
    public float health { get { return currentHealth; } }
    float currentHealth = 0;
    bool isInvincible;
    float damageCooldown = 0;
    
    // Flames
    public float flames { get { return collectedFlames; } }
    int collectedFlames = 0;

    // internals
    private LineRenderer lineRenderer;
    private Vector3 swingAnchor;
    private Vector3 spawnPoint;
    private Vector2 startingPoint;
    private Vector3 originalPos;

    private float swingRadius = 10f;
    private float swingSpeed = 2f;
    private float swingAngle;
    private float swingTimer;
    private bool swinging;
    // 0 is left, 1 is right
    private bool swingDirection;

    // state
    private Vector3 _spawnPoint;
    private bool isDead = false;

    public static bool sDown, dashing;
    public static float canSwing, dashCooldown;
    private int jumpCount;

    private bool canDash, canDoubleJump, facingRight = true;
    void Start()
    {

        currentHealth = maxHealth;
        healthText.text = $"Health: {currentHealth} HP";

        spawnPoint = transform.position;

        // set up the grapple rope
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

        // Unlock dash/double?jump by scene name
        var lvl = SceneManager.GetActiveScene().name;
        canDash = lvl == "Level 2" || lvl == "Level 3";
        canDoubleJump = lvl == "Level 3";
    }

    void Update()
    {

        if (currentHealth <= 0)
        {
            Respawn();
        }

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown <= 0)
            {
                isInvincible = false;
            }
        }


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
        swingDirection = facingRight;
        sDown = true;
        startingPoint = body.position;
        originalPos = swingDirection ? new Vector2(body.position.x + swingRadius, 20.0f) : new Vector2(body.position.x - swingRadius, 20.0f);
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
        body.MovePosition(swingDirection ? new Vector2(originalPos.x - x, startingPoint.y - y) : new Vector2(originalPos.x + x, startingPoint.y - y));
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
            else if (canDoubleJump && jumpCount < 1)
            {
                jumpCount++;
                DoJump();
            }
        }

        // dash
        if (canDash && dashCooldown <= 0f && Input.GetMouseButtonDown(0))
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

        Vector2 playerScreenPosition = Camera.main.WorldToScreenPoint(body.transform.position);
        Vector2 mouseScreenPosition = Input.mousePosition;

        Vector2 playerToMouseVector = (mouseScreenPosition - playerScreenPosition).normalized;

        body.linearVelocity = playerToMouseVector * dashSpeed;
    }

    void EndDash() => dashing = false;

    bool IsGrounded()
    {
        EndDash();

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

        // play death animation
        GetComponent<Animator>().SetTrigger("death");
        StartCoroutine(respawnRoutine());
    }

    IEnumerator respawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        currentHealth = maxHealth;

        // bring player back
        transform.position = spawnPoint;
        body.linearVelocity = Vector2.zero;
        body.gravityScale = 3f;

        // reset states
        sDown = dashing = false;
        canSwing = dashCooldown = 0f;

        // go back to idle
        var anim = GetComponent<Animator>();
        anim.ResetTrigger("death");
        anim.Play("MonkeyIdle");


        isDead = false;
    }

    // used by npc to boost speed for a while
    public IEnumerator BoostSpeed(float amount, float duration)
    {
        moveSpeed += amount;
        yield return new WaitForSeconds(duration);
        moveSpeed -= amount;
    }


    public void changeHealth(float dHealth)
    {
        if (dHealth < 0)
        {
            if (isInvincible)
            {
                return;
            }

            isInvincible = true;
            damageCooldown = timeInvincible;
        }

        currentHealth = Mathf.Clamp(currentHealth + dHealth, 0, maxHealth);
        healthText.text = $"Health: {currentHealth} HP";
    }

    public void changeFlames(int flame)
    {
        collectedFlames += flame;
        flamesText.text = $"Sacred Flames: {flames}";
    }
}