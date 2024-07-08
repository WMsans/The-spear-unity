using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class DashingEnemyAI : ParEnemy
{
    [Header("Petrolling")]
    
    [SerializeField] Collider2D detection;
    [SerializeField] Transform GroundCheck;
    [SerializeField] Transform WallCheck;
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float speed;
    [SerializeField] float accel;
    [SerializeField] float decel;
    [Header("Dashing")]
    [SerializeField] Collider2D vision;
    [SerializeField] float dashingSpeed;
    [SerializeField] float dashingTime;
    [SerializeField] float dashingCoolDown;
    float dashDir;
    bool dashing = false;
    bool dashCoolingDown = false;
    float moveDir = 1f;
    bool facingRight = true;
    bool checkingGround = false;
    bool checkingWall = false;
    [Header("Others")]
    Rigidbody2D rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        PlayerDetection(detection);
        // Check for the player
        if(vision.IsTouching(CharacterStateManager.Instance.GetComponent<Collider2D>()) && !dashing && !dashCoolingDown)
        {
            TargetOn();
        }
    }
    private IEnumerator Dash()
    {
        dashing = true;
        if (moveDir != dashDir) Flip();
        yield return new WaitForSeconds(dashingTime);
        dashing = false;
        if (moveDir != dashDir) Flip();
        dashCoolingDown = true;
        yield return new WaitForSeconds(dashingCoolDown);
        dashCoolingDown = false;
    }
    private void FixedUpdate()
    {
        checkingGround = Physics2D.OverlapCircle(GroundCheck.position, checkRadius, groundLayer);
        checkingWall = Physics2D.OverlapCircle(WallCheck.position, checkRadius, groundLayer);
       
        Petrolling();
    }
    void TargetOn()
    {
        //target = CharacterStateManager.Instance.transform;
        dashDir = Mathf.Sign(((Vector2)CharacterStateManager.Instance.transform.position - rb.position).normalized.x);
        
        StartCoroutine(Dash());
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
        if (dashing) dir = dashDir;
        var _sp = speed;
        if (dashing) _sp = dashingSpeed;
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(GroundCheck.position, checkRadius);
        Gizmos.DrawWireSphere(WallCheck.position, checkRadius);
    }
}
