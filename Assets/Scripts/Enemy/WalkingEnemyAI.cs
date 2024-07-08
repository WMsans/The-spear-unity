using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemyAI : ParEnemy
{
    [SerializeField] Collider2D detection;
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] float speed;
    [SerializeField] float accel;
    [SerializeField] float decel;
    Rigidbody2D rb;
    Vector2 target;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        target = pointB.position;
    }
    private void Update()
    {
        PlayerDetection(detection);
        var pA = (Vector2)pointA.position;
        var pB = (Vector2)pointB.position;

        if (Vector2.Distance(rb.position, pB) < 0.5f && target == pB)
        {
            target = pA;
        }else if (Vector2.Distance(rb.position, pA) < 0.5f && target == pA)
        {
            target = pB;
        }
        var dir = (target - rb.position).normalized;
        transform.localScale = new(Mathf.Sign(dir.x), 1, 1);
    }
    private void FixedUpdate()
    {
        var dir = (target - rb.position).normalized;
        if (Mathf.Abs(rb.velocity.x + dir.x * accel) <= speed || (Mathf.Sign(dir.x) != Mathf.Sign(rb.velocity.x) && Mathf.Abs(dir.x) > 0.001f))
        {
            rb.velocity += dir.x * accel * Vector2.right;
        }
        else if(Mathf.Abs(rb.velocity.x + dir.x * accel) >= speed && Mathf.Abs(rb.velocity.x) < speed)
        {
            rb.velocity = new (Mathf.Sign(dir.x) * speed, rb.velocity.y);
        }
        if (Mathf.Abs(dir.x) <= 0.001f || (Mathf.Abs(rb.velocity.x) > speed))
        {
            if (Mathf.Abs(rb.velocity.x) <= Mathf.Abs(decel)) rb.velocity *= Vector2.up;
            else if (rb.velocity.x < 0) rb.velocity += decel * Vector2.right;
            else rb.velocity -= decel * Vector2.right;
        }
        //rb.velocity = new(dir.x * speed, rb.velocity.y);

        //rb.position += dir.x * speed * Vector2.right * Time.deltaTime;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointA.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.position, 0.5f);
        Gizmos.DrawLine(pointA.position, pointB.position);
    }
}
