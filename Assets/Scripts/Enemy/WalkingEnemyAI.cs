using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class WalkingEnemyAI : ParEnemy
{
    [SerializeField] Collider2D detection;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float checkRadius;
    [SerializeField] float speed;
    [SerializeField] float accel;
    [SerializeField] float decel;
    Rigidbody2D rb;
    float moveDir = 1f;
    bool facingRight = true;
    bool checkingGround = false;
    bool checkingWall = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        PlayerDetection(detection);

    }
    private void FixedUpdate()
    {
        checkingGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        checkingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);

        Petrolling();
    }
    void Petrolling()
    {
        if (!checkingGround || checkingWall)
        {
            if (facingRight)
            {
                Flip();
            }
            else if (!facingRight)
            {
                Flip();
            }
        }
        var dir = moveDir;
        var _sp = speed;
        if (Mathf.Abs(rb.velocity.x + dir * accel) <= _sp || (Mathf.Sign(dir) != Mathf.Sign(rb.velocity.x) && Mathf.Abs(dir) > 0.001f))
        {
            rb.velocity += dir * accel * Vector2.right;
        }
        else if (Mathf.Abs(rb.velocity.x + dir * accel) >= _sp && Mathf.Abs(rb.velocity.x) < _sp)
        {
            rb.velocity = new(Mathf.Sign(dir) * _sp, rb.velocity.y);
        }
        if (Mathf.Abs(dir) <= 0.001f || (Mathf.Abs(rb.velocity.x) > _sp))
        {
            if (Mathf.Abs(rb.velocity.x) <= Mathf.Abs(decel)) rb.velocity *= Vector2.up;
            else if (rb.velocity.x < 0) rb.velocity += decel * Vector2.right;
            else rb.velocity -= decel * Vector2.right;
        }
    }
    void Flip()
    {
        moveDir *= -1;
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
    }
}
