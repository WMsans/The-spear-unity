using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashingEnemyAI : ParEnemy
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Dashing = Animator.StringToHash("Dashing");
    private static readonly int PreDashing = Animator.StringToHash("PreDashing");

    [Header("Petrolling")]
    
    [SerializeField] Collider2D detection;
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] float speed;
    [SerializeField] float accel;
    [SerializeField] float decel;
    float moveDir = 1f;
    bool facingRight = true;
    bool checkingGround = false;
    bool checkingWall = false;
    [Header("Dashing")]
    [SerializeField] float vision;
    [SerializeField] float dashingSpeed;
    [SerializeField] float dashingTime;
    [SerializeField] float dashingCoolDown;
    float dashDir;
    bool dashing = false;
    bool preDashing = false;
    bool dashCoolingDown = false;
    [Header("Others")]
    [SerializeField] Animator anim;
    Rigidbody2D rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        PlayerDetection(detection);
        HandleAnimation();
        // Check for the player
        var ray = Physics2D.Raycast(transform.position, Vector2.right * (facingRight ? 1 : -1), vision, groundLayer | playerLayer);
        if(ray && ray.collider.CompareTag("Player") && !dashing && !dashCoolingDown)
        {
            TargetOn();
        }
    }

    void HandleAnimation()
    {
        anim.SetFloat(Speed, Mathf.Abs(rb.velocity.x));
        anim.SetBool(PreDashing, preDashing);
        anim.SetBool(Dashing, dashing);
    }
    private IEnumerator Dash()
    {
        dashing = true;
        if (!Mathf.Approximately(moveDir, dashDir)) Flip();
        yield return new WaitForSeconds(dashingTime);
        dashing = false;
        dashCoolingDown = true;
        yield return new WaitForSeconds(dashingCoolDown);
        dashCoolingDown = false;
    }
    private void FixedUpdate()
    {
        checkingGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        checkingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);
       
        Petrolling();
    }
    void TargetOn()
    {
        //target = CharacterStateManager.Instance.transform;
        dashDir = Mathf.Sign(((Vector2)CharacterStateManager.Instance.transform.position - rb.position).normalized.x);
        preDashing = true;
    }

    public void StartDash()
    {
        preDashing = false;
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
        else if (preDashing) _sp = 0;
        
        if (Mathf.Abs(rb.velocity.x + dir * accel) <= _sp || (!Mathf.Approximately(Mathf.Sign(dir), Mathf.Sign(rb.velocity.x)) && Mathf.Abs(dir) > 0.001f))
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
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * vision);
    }
}
