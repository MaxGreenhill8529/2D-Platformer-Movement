using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

//Required For Applying forces
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Run Settings")]
    [SerializeField] private KeyCode leftKey;
    [SerializeField] private KeyCode rightKey;
    [Tooltip("The max speed that the player can reach while running(Not a velocity cap!!)")]
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private float acceleration = 7;
    [SerializeField] private float deceleration = 7;
    [SerializeField] private float velPower = 0.9;
    [Tooltip("Friction is applied to help slow the player down once they have let go of the movement buttons")]
    [SerializeField] private float frictionAmount = 0.2f;

    [Header("Jump Settings")]
    [Tooltip("The button used to jump")]
    [SerializeField] private KeyCode jumpKey = KeyCode.UpArrow;
    [Tooltip("Adjust if their is a problem with ground detection")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.4f,0.1f);
    [Tooltip("The force to apply in the upwards direction when the jump key is pressed")]
    [SerializeField] private float jumpForce = 15;
    [Tooltip("A buffer allowing for the player to still jump even after a set amount of time off the ground")]
    [SerializeField] private float jumpCoyoteTime = 0.15f;
    //Not implemented yet
    //[SerializeField] private float jumpBufferTime = 0.1f;
    [Header("Required References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    [Header("Debug / Testing Options")]
    [Tooltip("Disables/Enables coyote time")]
    [SerializeField] private bool coyoteTime = true;

    private float defaultMaxSpeed;
    private float horizontalInput;
    private float coyoteTimeCounter;

    private void Awake()
    {
        defaultMaxSpeed = maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(leftKey))
        {
            horizontalInput = -1;
        }
        if(Input.GetKey(rightKey))
        {
            horizontalInput = 1;
        }
        if(!Input.GetKey(rightKey) && !Input.GetKey(leftKey))
        {
            horizontalInput = 0;
        }


        if(rb.linearVelocityY == 0 && Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            coyoteTimeCounter = jumpCoyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(jumpKey) && coyoteTime == true && coyoteTimeCounter > 0 || Input.GetKeyDown(jumpKey) && Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
        {
            Jump();
        }
    }
    private void FixedUpdate()
    {
        #region Friction
        if (Mathf.Abs(horizontalInput) <= 0.01)
        {
            float amount = Mathf.Min(Mathf.Abs(rb.linearVelocityX), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(rb.linearVelocityX);
            rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
        #endregion

        #region Run
        float targetSpeed = horizontalInput * maxSpeed;
        float speedDif = targetSpeed - rb.linearVelocityX;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
        rb.AddForce(movement * Vector2.right);
        #endregion
    }

    private void Jump()
    {
        coyoteTimeCounter = 0;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void SpeedChange(float value)
    {
        maxSpeed = value * defaultMaxSpeed;
    }
}
