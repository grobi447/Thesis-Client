using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;

    public float moveSpeed = 5f;
    private Vector2 movement;

    public float jumpForce = 15f;
    public float minJumpForce = 10f;
    public float jumpCutMultiplier = 0.3f;
    private bool isJumping = false;

    public bool isGrounded = false;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;
    private bool wasGrounded = false;

    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    public float fallMultiplier = 4.5f;
    public float lowJumpMultiplier = 4f;

    public InputActionReference move;
    public InputActionReference jump;
    public InputActionReference crouch;

    public bool isCrouching = false;
    public Animator animator;
    public BoxCollider2D boxCollider;
    public CapsuleCollider2D capsuleCollider;
    private SpriteRenderer spriteRenderer;
    private bool isDead = false;
    private bool isFinished = false;
    public GameManager gameManager;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
    }
    
    private void Update()
    {
        if (isDead) return;
        
        isCrouching = crouch != null && crouch.action.IsPressed();
        
        if (!isCrouching)
        {
            movement = move.action.ReadValue<Vector2>();
            animator.SetFloat("Speed", Mathf.Abs(movement.x));
        }
        else
        {
            movement = Vector2.zero;
        }
        
        if (animator != null)
        {
            animator.SetBool("IsCrouching", isCrouching);
        }
        
        HandleCrouchCollider();
        CheckGrounded();

        if (jump.action.WasPressedThisFrame())
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        HandleJump();
        ApplyJumpPhysics();
        UpdateAnimator();
        FlipCharacter();
    }

    private void UpdateAnimator()
    {
        if (animator != null)
        {
            if (isGrounded)
            {
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", false);
            }
            else if (rb.velocity.y > 0.1f)
            {
                animator.SetBool("IsJumping", true);
                animator.SetBool("IsFalling", false);
            }
            else
            {
                animator.SetBool("IsJumping", false);
                animator.SetBool("IsFalling", true);
            }
        }
    }

    private void FlipCharacter()
    {
        if (movement.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (movement.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }   
    }

    private void HandleCrouchCollider()
    {
        if (boxCollider != null && capsuleCollider != null)
        {
            if (isCrouching)
            {
                boxCollider.enabled = false;
                capsuleCollider.enabled = true;
            }
            else
            {
                boxCollider.enabled = true;
                capsuleCollider.enabled = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            rb.velocity = new Vector2(movement.x * moveSpeed, rb.velocity.y);
        }
    }

    private void CheckGrounded()
    {
        wasGrounded = isGrounded;
        Vector2 checkPosition = groundCheck != null ? groundCheck.position : (Vector2)transform.position;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPosition, groundCheckRadius, groundLayer);
        
        isGrounded = false;
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Block"))
            {
                isGrounded = true;
                break;
            }
        }
    }

    private void HandleJump()
    {
        if (jumpBufferCounter > 0f && isGrounded && !isJumping)
        {
            PerformJump();
        }

        if (jump.action.WasReleasedThisFrame() && isJumping && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
            isJumping = false;
        }

        if (isGrounded && !wasGrounded)
        {
            isJumping = false;
        }
    }

    private void PerformJump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isJumping = true;
        jumpBufferCounter = 0f;
    }

    private void ApplyJumpPhysics()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !jump.action.IsPressed())
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        jump.action.Enable();
        move.action.Enable();
        crouch.action.Enable();
    }

    private void OnDisable()
    {
        jump.action.Disable();
        move.action.Disable();
        crouch.action.Disable();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trap") || collision.CompareTag("Bullet"))
        {
            Die();
        }

        if (collision.CompareTag("Finish") && !isFinished)
        {
            isFinished = true;
            gameManager.StartCoroutine(gameManager.FinishedLevelRoutine());
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Trap") && collision.collider.gameObject.name == "Blade")
        {
            Die();
        }
    }

    private void Die()
    {
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        isDead = true;
        movement = Vector2.zero;
        rb.velocity = Vector2.zero;
        capsuleCollider.enabled = false;
        boxCollider.enabled = false;
        jump.action.Disable();
        move.action.Disable();
        crouch.action.Disable();
        rb.bodyType = RigidbodyType2D.Static;
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", false);
        animator.SetFloat("Speed", 0f);
        animator.SetBool("IsCrouching", false);
        animator.SetTrigger("Die");
        
        yield return new WaitForSeconds(1f);
        spriteRenderer.enabled = false;
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("Spawn");
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position + new Vector3(0f, 0.5f, 0f);
        }
        gameManager.playerDeathCount++;
        gameManager.UpdateDeathCountUI(gameManager.playerDeathCount);
        gameManager.SaveDeathCount();
        yield return new WaitForSeconds(1f);
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        
        spriteRenderer.enabled = true;
        animator.Rebind();
        animator.Update(0f);
        capsuleCollider.enabled = true;
        boxCollider.enabled = true;
        jump.action.Enable();
        move.action.Enable();
        crouch.action.Enable();
        isDead = false;
    }
}