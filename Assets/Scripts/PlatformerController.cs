
using System;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;

public class PlatformerController : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] CircleCollider2D collider;
    SpriteLibrary spriteLibrary;
    SpriteResolver spriteResolver;
    Sprite sprite;
    [SerializeField] float moveSpeed; 
    [SerializeField] float jumpForce = 10;
    [SerializeField] public float dashForce = 15;
    [SerializeField] DashLineRenderer dashLine;

    bool isRebounding = false;
    bool isDashHeld = true;
    bool canDash = true;
    float movementX;
    float defaultDeltaTime;
    float initialTimeScale;
    float dashTimeScale = 0.5f;

    float rayPositionOffset; //! might break if obj is a child component
    Vector3 dashFacingVector;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        dashLine = GetComponentInChildren<DashLineRenderer>();
        spriteLibrary = GetComponent<SpriteLibrary>();
        spriteResolver = GetComponent<SpriteResolver>();
        spriteResolver.SetCategoryAndLabel("cat", "cat");
        dashLine.gameObject.SetActive(true);
        defaultDeltaTime = Time.fixedDeltaTime;
        rayPositionOffset = transform.lossyScale.y * collider.radius * 0.8f;
    }

    void FixedUpdate()
    {
        rb.angularVelocity = -movementX * moveSpeed;
    }

    void OnMove(InputValue value)
    {
        movementX = value.Get<Vector2>().x;
    }

    void OnJump()
    {
        if (IsOnGround())
        {
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
        } 
        else if(IsOnGround(20f))
        {
            rb.linearVelocity = Vector2.down * dashForce;
        }
    }

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
            isDashHeld = true;
            spriteResolver.SetCategoryAndLabel("cat", "cat_mad");
            initialTimeScale = Time.timeScale;
            Time.timeScale = dashTimeScale;
            Time.fixedDeltaTime = defaultDeltaTime * Time.timeScale;
            return;
        }
        dashLine.enabled = false;
        isDashHeld = false;
        spriteResolver.SetCategoryAndLabel("cat", "cat");
        Time.timeScale = initialTimeScale;
        Time.fixedDeltaTime = defaultDeltaTime;


        float reboundDistance = dashForce / rb.mass / 3.5f;
        //Dash in direction of mouse
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 mouseDirection = ((Vector2)(mousePosition - transform.position)).normalized;
        dashFacingVector = mouseDirection;

        //(For dash in direction of character orientation)
        //Vector3 facingVector = transform.rotation * Vector3.right; //Assumes initial "forward" direction is right
        
        //Search for reboundable objects
        int numRays = 7;
        RaycastHit2D shortestHit = new RaycastHit2D();
        Vector3 normal = new Vector3(dashFacingVector.y, -dashFacingVector.x, 0);
        for (int i = 0; i < numRays; i++)
        {   
            //Calculate points along front of circle as ray origins
            float offsetAngle = 85 * (1 - 2.0f*i/(numRays-1));
            Vector3 offset = Quaternion.AngleAxis(offsetAngle, Vector3.forward) * dashFacingVector * rayPositionOffset;
            Vector3 rayPosition = transform.position + offset;

            Debug.DrawRay(rayPosition, dashFacingVector * reboundDistance, Color.yellow, 1);
            RaycastHit2D hit = Physics2D.Raycast(rayPosition, dashFacingVector, reboundDistance, LayerMask.GetMask("Wall")); //! Only rebounds off walls
            if (shortestHit.collider == null || (shortestHit.collider != null && hit.distance < shortestHit.distance))
            {
                shortestHit = hit;
                Debug.DrawRay(hit.point, hit.normal, Color.purple, 3);
            }
        }

        //Dash, rebound against nearby walls
        rb.linearVelocity = dashFacingVector * dashForce;
        if (shortestHit.collider != null)
        {
            isRebounding = true;
            rb.linearVelocity = dashFacingVector * 2 * dashForce;
        }
        canDash = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsOnGround())
        {
            canDash = true;
        }
        if (isRebounding)
        {
            Rebound(collision);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isRebounding)
        {
            Rebound(collision);
        }
    }

    void Rebound(Collision2D collision)
    {
        Vector2 reflection = Vector2.Reflect(dashFacingVector, collision.GetContact(0).normal);
        rb.linearVelocity = 1.5f * dashForce * reflection;
        canDash = true;     //!Regains dash after rebounding off *any* object
        isRebounding = false;
    }

    //Check if player is on ground using raycast. Only counts "Wall" layer.
    bool IsOnGround(float castLength = 0.2f)
    {
        Vector3 footPosition = transform.position + rayPositionOffset * Vector3.down;
        return Physics2D.Raycast(footPosition ,Vector3.down, castLength, LayerMask.GetMask("Wall"));
    }

    public float GetRayOffset()
    {
        return rayPositionOffset;
    }
}
