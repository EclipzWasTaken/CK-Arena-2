using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] TrailRenderer dashTrail;
    

    // Dashing Fields
    private bool canDash = true;
    private bool isDashing = false;
    private float dashSpeed = 200f;
    private float dashTime = 0.1f;
    private float dashCooldown = 3f;

    // Movement Fields
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 12f;
    private bool isFacingRight = true;
    private int numberOfJumps = 2;
    private float jumpCooldown = 0.5f;

    void Update()
    {
        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0f)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        // dashing
        if (isDashing)
        {
            rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0f);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded() && numberOfJumps > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            numberOfJumps--;
        }
        else if (context.performed && !IsGrounded() && numberOfJumps > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            numberOfJumps--;
        }

        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        if (IsGrounded())
        {
            // wait for a short time before allowing the player to jump again

            numberOfJumps = 1;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 1f, groundLayer);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }


    // Dash Input Action
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    /**
        * Coroutine to perform the dash
        The actual code for the dash is in this coroutine
        */
    private IEnumerator PerformDash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        //rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0f);
        dashTrail.emitting = true;
        yield return new WaitForSeconds(dashTime);
        dashTrail.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}