using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterEnemyAI : ParEnemy
{
    private static readonly int CanSeePlayer = Animator.StringToHash("CanSeePlayer");
    private static readonly int Cooling = Animator.StringToHash("Cooling");
    private static readonly int Speed = Animator.StringToHash("Speed");

    [Header("Petrolling")]
    [SerializeField] Collider2D detection;
    [SerializeField] Transform FrontGroundCheck;
    [SerializeField] Transform WallCheck;
    [SerializeField] float checkRadius;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float speed;
    [SerializeField] float accel;
    [SerializeField] float decel;
    float moveDir = 1f;
    float oriDir = 1f;
    bool facingRight = true;
    bool checkingGround = false;
    bool checkingWall = false;
    [Header("Splitting")]
    [SerializeField] float splittHeight;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float splittCoolDownTime;
    [SerializeField] Transform splittPos;
    bool coolingDown = false;
    Transform Target;
    [Header("Detection")]
    [SerializeField] Collider2D Vision;
    bool canSeePlayer;
    [Header("Others")]
    [SerializeField] Animator animator;
    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        Hp = MaxHp;
        Target = CharacterStateManager.Instance.transform;
    }
    private void Update()
    {
        PlayerDetection(detection);
        HandleAnim();
    }

    void HandleAnim()
    {
        animator.SetFloat(Speed, Mathf.Abs(rb.velocity.x));
        animator.SetBool(CanSeePlayer, canSeePlayer);
        animator.SetBool(Cooling, coolingDown);
    }
    private void FixedUpdate()
    {
        // Detection
        checkingGround = Physics2D.OverlapCircle(FrontGroundCheck.position, checkRadius, groundLayer);
        checkingWall = Physics2D.OverlapCircle(WallCheck.position, checkRadius, groundLayer);
        canSeePlayer = Vision.IsTouching(Target.GetComponent<Collider2D>());
        Petrolling();
        if (!canSeePlayer && !coolingDown)
        {
            if (Mathf.Abs(moveDir) < 0.01f)
                moveDir = oriDir;
        }
        else
        {
            if (Mathf.Abs(moveDir) >= 0.01f)
                oriDir = moveDir;
            moveDir = 0f;
        }
    }
    void Petrolling()
    {
        if((!checkingGround || checkingWall) && !(coolingDown || canSeePlayer))
        {
            if(facingRight)
            {
                Flip();
            }else if (!facingRight)
            {
                Flip();
            }
        }
        var dir = moveDir;
        if (Mathf.Abs(rb.velocity.x + dir * accel) <= speed || (!Mathf.Approximately(Mathf.Sign(dir), Mathf.Sign(rb.velocity.x)) && Mathf.Abs(dir) > 0.001f))
        {
            rb.velocity += dir * accel * Vector2.right;
        }
        else if (Mathf.Abs(rb.velocity.x + dir * accel) >= speed && Mathf.Abs(rb.velocity.x) < speed)
        {
            rb.velocity = new(Mathf.Sign(dir) * speed, rb.velocity.y);
        }
        if (Mathf.Abs(dir) <= 0.001f || (Mathf.Abs(rb.velocity.x) > speed))
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
    void FlipTowardsTarget()
    {
        var disToTarget = Target.position - transform.position;
        if(disToTarget.x < 0 &&facingRight)
        {
            Flip();
        }
        else if(disToTarget.x > 0 && !facingRight)
        {
            Flip();
        }
    }
    public void Attack()
    {
        FlipTowardsTarget();
        // Attack
        var bulletRb = Instantiate(bulletPrefab).GetComponent<Rigidbody2D>();
        bulletRb.position = splittPos.position;
        var disToTarget = (Vector2)Target.position - bulletRb.position;
        bulletRb.velocity = Vector2.zero;
        bulletRb.AddForce(new(disToTarget.x * 0.8f, splittHeight), ForceMode2D.Impulse);
        // Cool
        StartCoroutine(AttackCooler());
    }
    IEnumerator AttackCooler ()
    {
        coolingDown = true;
        yield return new WaitForSeconds(splittCoolDownTime);
        coolingDown = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(FrontGroundCheck.position, checkRadius);
        Gizmos.DrawWireSphere(WallCheck.position, checkRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(splittPos.position, checkRadius);
    }
}
