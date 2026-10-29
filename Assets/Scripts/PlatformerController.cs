
using System;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

public class PlatformerController : MonoBehaviour
{
    Animator animator;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] CircleCollider2D collider;
    SpriteResolver spriteResolver;
    [SerializeField] float moveSpeed; 
    [SerializeField] float jumpForce = 10;
    [SerializeField] public float dashForce = 15;
    [SerializeField] public float maxGroundingDistance = 20f; //Determines how far above the ground player can be and still press space to ground themselves.
    [SerializeField] DashLineRenderer dashLine;
    [SerializeField] GroundLineRenderer groundLine;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] public RespawnPoint respawnPoint;

    [SerializeField] public AudioSource audioSource;
    [SerializeField] AudioClip jumpClip;

    public bool invertDash = true;

    bool isDashing = false;
    bool isJumping = false;
    bool isRebounding = false;
    bool canDash = true;
    bool canGround = false;
    bool isMovingRight = true;
    float movementX;
    float defaultDeltaTime;
    float initialTimeScale;
    float dashTimeScale = 0.5f;

    public int points = 0;
    [NonSerialized] public List<GameObject> coinsCollected;

    float rayPositionOffset; //! might break if obj is a child component
    Vector3 dashFacingVector;
    string dashRaycastHitName = "";
    
    void Awake()
    {
        dashLine = GetComponentInChildren<DashLineRenderer>();
        dashLine.gameObject.SetActive(true);
        dashLine.enabled = false;

        groundLine = GetComponentInChildren<GroundLineRenderer>();
        groundLine.gameObject.SetActive(true);
        groundLine.enabled = false;

        animator = gameObject.GetComponent<Animator>();
        spriteResolver = GetComponent<SpriteResolver>();
        spriteResolver.SetCategoryAndLabel("cat", "cat");

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        defaultDeltaTime = Time.fixedDeltaTime;
        rayPositionOffset = transform.lossyScale.y * collider.radius * 0.8f;

        coinsCollected = new List<GameObject>();
    }

    void Update()
    {
        animator.SetBool("isJumping", !IsOnGround());
        animator.SetBool("isRunning", !Mathf.Approximately(rb.linearVelocityX, 0f));
        animator.SetBool("canDash", canDash);
        if (isMovingRight && movementX < 0)
        {
            isMovingRight = false;
            spriteRenderer.flipX = true;
        }
        else if  (!isMovingRight && movementX > 0)
        {
            isMovingRight = true;
            spriteRenderer.flipX = false;
        }
    }

    void FixedUpdate()
    {
        if (Math.Abs(rb.linearVelocityX) < Math.Abs(movementX * moveSpeed) || (Mathf.Approximately(movementX, 0) && !isDashing))
        {
             rb.linearVelocityX = movementX * moveSpeed;
             isDashing = false;
        }
    }

    void OnMove(InputValue value)
    {
        movementX = value.Get<Vector2>().x;
    }

    void OnJump(InputValue value)
    {
        if (IsOnGround() && value.isPressed)
        {
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
            isJumping = true;
            audioSource.PlayOneShot(jumpClip);
            return;
        } 
        RaycastHit2D groundRay = IsOnGround(maxGroundingDistance);
        if(groundRay)
        {
            if (value.isPressed)
            {
                canGround = true;
                groundLine.enabled = true;
            }
            else if (canGround)
            {
                rb.linearVelocity = Vector2.down * dashForce;
                groundLine.enabled = false;
                canGround = false;
            }
            return;
        }
    }

    //Propels the player in direction of mouse. If close to a wall, will rebound with extra force.
    void OnDash(InputValue value)
    {   
        if (!canDash)
        {
            return;
        }

        //Slow time when pressed, dash on release
        if (value.isPressed)
        {
            dashLine.enabled = true;
            spriteResolver.SetCategoryAndLabel("cat", "cat_mad");
            initialTimeScale = Time.timeScale;
            Time.timeScale = dashTimeScale;
            Time.fixedDeltaTime = defaultDeltaTime * Time.timeScale;
            return;
        }
        dashLine.enabled = false;
        spriteResolver.SetCategoryAndLabel("cat", "cat");
        Time.timeScale = initialTimeScale;
        Time.fixedDeltaTime = defaultDeltaTime;


        float reboundDistance = dashForce / rb.mass / 3.5f;
        //Dash in direction of mouse
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 mouseDirection = ((Vector2)(mousePosition - transform.position)).normalized;
        dashFacingVector = mouseDirection * (invertDash ? -1 : 1);

        //(For dash in direction of character orientation)
        //Vector3 facingVector = transform.rotation * Vector3.right; //Assumes initial "forward" direction is right
        
        //Search for reboundable objects
        int numRays = 7;
        RaycastHit2D shortestHit = new RaycastHit2D();
        for (int i = 0; i < numRays; i++)
        {   
            //Calculate points along front of circle as ray origins
            float offsetAngle = 85 * (1 - 2.0f*i/(numRays-1));
            Vector3 offset = Quaternion.AngleAxis(offsetAngle, Vector3.forward) * dashFacingVector * rayPositionOffset;
            Vector3 rayPosition = transform.position + offset;

            Debug.DrawRay(rayPosition, dashFacingVector * reboundDistance, Color.yellow, 1);
            RaycastHit2D hit = Physics2D.Raycast(rayPosition, dashFacingVector, reboundDistance, LayerMask.GetMask("Wall")); //! Only rebounds off walls
            if (hit.collider != null && (shortestHit.collider == null || (shortestHit.collider != null && hit.distance < shortestHit.distance)))
            {
                shortestHit = hit;
            }
        }

        //Dash, rebound against nearby walls
        rb.linearVelocity = dashFacingVector * dashForce;
        if (shortestHit.collider != null)
        {
            isRebounding = true;
            rb.linearVelocity = dashFacingVector * 2 * dashForce;
            Debug.DrawRay(shortestHit.point, shortestHit.normal, Color.darkViolet, 3);
            dashRaycastHitName = shortestHit.collider.name;
        }
        isDashing = true;
        canDash = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsOnGround())
        {
            canDash = true;
        }
        if (isRebounding && collision.collider.name == dashRaycastHitName)
        {
            Rebound(collision);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isRebounding && collision.collider.name == dashRaycastHitName)
        {
            Rebound(collision);
        }
    }

    void Rebound(Collision2D collision)
    {
        Vector2 reflection = Vector2.Reflect(dashFacingVector, collision.GetContact(0).normal);
        rb.linearVelocity = 1.5f * dashForce * reflection;
        isDashing = true;
        canDash = true;     //!Regains dash after rebounding off *any* object
        isRebounding = false;
    }


    /// <summary>
    /// Check if player is on ground using linear raycast by feet. Only counts "Wall" layer.
    /// </summary>
    /// <param name="castLength">Determines length of the downward ray originating just below the player.</param>
    /// <returns>RaycastHit2D returned from the Physics.Raycast</returns>
    public RaycastHit2D IsOnGround(float castLength = 0.2f)
    {
        Vector3 footPosition = transform.position + rayPositionOffset * Vector3.down;
        return Physics2D.Raycast(footPosition ,Vector3.down, castLength, LayerMask.GetMask("Wall"));
    }

    public float GetRayOffset()
    {
        return rayPositionOffset;
    }

    public void Respawn()
    {
        transform.position = respawnPoint.transform.position + Vector3.up;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0;
        canDash = true;
    }
}
