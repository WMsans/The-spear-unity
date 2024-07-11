using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class SlashEnemyAI : ParEnemy
{
    [Header("Player Detection")]
    [SerializeField] Collider2D detection;
    [SerializeField] Collider2D vision;
    [SerializeField] Transform rayCast;
    [SerializeField] LayerMask rayCastMask;
    [SerializeField] float rayCastLength;
    [SerializeField] float attackDistance;
    [SerializeField] float moveDistance; // Minimum distance for attack
    [SerializeField] float attackCooldown;
    RaycastHit2D hit;
    GameObject target;
    Animator anim;
    float tarDistance;
    bool attacking;
    bool inRange;
    bool cooling;
    float attackCooldownTimer;
    [Header("Petrolling")]
    [SerializeField] Transform groundCheck;
    [SerializeField] Transform wallCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float checkRadius = 0.2f;
    [SerializeField] float speed = 3f;
    [SerializeField] float accel = 1.5f;
    [SerializeField] float decel = 1.2f;
    float moveDir = 1f;
    bool facingRight = true;
    bool checkingGround = false;
    bool checkingWall = false;
    [Header("Others")]
    Rigidbody2D rb;
    private void Awake()
    {
        attackCooldownTimer = attackCooldown;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        PlayerDetection(detection);
        // Detecting player
        if (vision.IsTouching(CharacterStateManager.Instance.GetComponent<Collider2D>()))
        {
            TargetOn();
        }
        
    }
    private void TargetOn()
    {
        target = CharacterStateManager.Instance.gameObject;
        inRange = true;
    }
    private void FixedUpdate()
    {
        checkingGround = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        checkingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, groundLayer);

        // If the player is in vision
        if (inRange)
        {
            var dir = Mathf.Sign(target.transform.position.x - transform.position.x);
            hit = Physics2D.Raycast(rayCast.position, dir * Vector2.right, rayCastLength, rayCastMask);
            RayCastDebugger();
        }
        // If the player is detected
        if (hit.collider != null)
        {
            EnemyLogic();
        }
        else
        {
            inRange = false;
        }
        if (!inRange)
        {
            anim.SetBool("canWalk", false);
            StopAttack();
        }
    }
    void EnemyLogic()
    {
        tarDistance = Vector2.Distance(rb.position, target.transform.position);
        
        if(tarDistance > attackDistance)
        {
            Move();
            StopAttack();
        }
        else if(!cooling)
        {
            Attack();
        }
        if (cooling)
        {
            anim.SetBool("attack", false);
        }
    }
    void Move()
    {
        anim.SetBool("canWalk", true);
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("EliteAttack"))
        {
            var dir = Mathf.Sign(target.transform.position.x - transform.position.x);
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
    }
    void Attack()
    {
        attackCooldown = attackCooldownTimer;
        attacking = true;
        anim.SetBool("canWalk", false);
        anim.SetBool("attack", true);
    }
    void StopAttack()
    {
        cooling = false;
        attacking= false;
        anim.SetBool("attack", false);
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
    void RayCastDebugger()
    {
        var dir = Mathf.Sign(target.transform.position.x - transform.position.x);
        if (tarDistance > attackDistance)
        {
            Debug.DrawRay(rayCast.position, dir * Vector2.right * rayCastLength, Color.red);
        }
        else
        {
            Debug.DrawRay(rayCast.position, dir * Vector2.right * rayCastLength, Color.green);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
    }
}
