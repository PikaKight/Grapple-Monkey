using UnityEngine;
using UnityEngine.Windows;
using Input = UnityEngine.Input;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D body;
    public CircleCollider2D collider;
    public float moveSpeed;
    public GameObject ground;

    private LineRenderer lineRenderer;
    Vector3 originalPos;

    public static bool sDown = false;
    public static bool dashing = false;
    public static float canSwing = 0;
    public static float dashCooldown = 0;

    // added max swing time and dash cooldown in seconds - michael
    private const float maxSwingTime = 2f; // max grapple time in seconds - michael
    private const float maxDashCooldown = 5f; // dash cooldown in seconds - michael

    // start is called once before the first execution of update after the monobehaviour is created - michael
    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;
    }

    // update is called once per frame - michael
    void Update()
    {
        // swinging - michael
        if (Input.GetKey(KeyCode.S) && !sDown && canSwing == 0)
        {
            sDown = true;
            originalPos = new Vector2(body.position.x + 4, 9.5f);
            body.gravityScale = 0;
            lineRenderer.enabled = true;
            body.Sleep();
        }
        else if (Input.GetKey(KeyCode.S) && sDown && canSwing < maxSwingTime)
        {
            canSwing += Time.deltaTime; // add delta time for consistent timing - michael
            lineRenderer.SetPosition(0, body.position);
            lineRenderer.SetPosition(1, originalPos);
            Physics.gravity = new Vector2(0, 0);
            body.position = new Vector2(transform.position.x + 0.2f, body.position.y);
        }
        else
        {
            canSwing -= Time.deltaTime; // subtract delta time for consistent timing - michael
            if (canSwing < 0)
            {
                canSwing = 0;
            }

            sDown = false;
            body.gravityScale = 3;
            lineRenderer.enabled = false;

            // left/right movement - michael
            if (!dashing)
            {
                body.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, body.linearVelocity.y); // using velocity property - michael
            }
            if (IsGrounded())
            {
                dashing = false;
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
            }
        }
        return false;
    }
}