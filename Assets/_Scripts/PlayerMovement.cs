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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    // Update is called once per frame
    void Update()
    { 
        //Swinging
        if (Input.GetKey(KeyCode.S) && !sDown && canSwing == 0)
        {
            sDown = true;
            originalPos = new Vector2(body.position.x + 4, 9.5f);
            body.gravityScale = 0;
            lineRenderer.enabled = true;
            body.Sleep();
        }
        else if (Input.GetKey(KeyCode.S) && sDown && canSwing < 200)
        { 
            canSwing += 0.1f;
            lineRenderer.SetPosition(0, body.position);
            lineRenderer.SetPosition(1, originalPos);
            Physics.gravity = new Vector2(0, 0);
            body.position = new Vector2(transform.position.x + 0.2f, body.position.y);
        }
        else
        {
            canSwing -= 0.1f;
            if (canSwing < 0)
            {
                canSwing = 0;
            }
            
            sDown = false;
            body.gravityScale = 3;
            lineRenderer.enabled = false;
            
            //Left/right movement
            if (!dashing)
            {
                body.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, body.linearVelocity.y);
            }
            if (IsGrounded())
            {
                dashing = false;
            }

            //Jumping
            if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
            {
                body.AddForce(Vector2.up * 1000);
            }
            //If on second level, dashing is enabled
            if (SceneManager.GetActiveScene().name == "Level 2" && dashCooldown <= 0 && Input.GetMouseButtonDown(0))
            {
                dashing = true;
                dashCooldown = 5000f;
                float dashSpeed = 20f;

                Vector2 playerScreenPosition = Camera.main.WorldToScreenPoint(body.transform.position);
                Vector2 mouseScreenPosition = Input.mousePosition;

                Vector2 playerToMouseVector = (mouseScreenPosition - playerScreenPosition).normalized;
                //Debug.Log("playerToMouseVector is " + playerToMouseVector);
    
                body.linearVelocity = playerToMouseVector * dashSpeed;
            }
        }
        dashCooldown--;
    }
    
    //Check if user is touching ground
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
