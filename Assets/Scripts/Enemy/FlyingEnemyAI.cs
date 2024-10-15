using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;

public class FlyingEnemyAI : ParEnemy
{
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int PlayerDetected = Animator.StringToHash("PlayerDetected");

    [Header("Player Follow")]
    [SerializeField] Collider2D detection;
    [SerializeField] float detectionDistance;
    [SerializeField] float trackingDistance;
    [SerializeField] Transform target;
    [SerializeField] float speed = 200f;
    [SerializeField] float nextWaypointDistance = 3f;
    [SerializeField] float updateTime = 0.5f;
    [SerializeField] Animator anim;

    Path _path;
    int _currentWaypoint = 0;
    bool _isMoving = false;
    bool _isAttacking = false;
    //bool reachedEndOfPath = false;

    Seeker _seeker;
    Rigidbody2D _rd;
    private void Awake()
    {
        _seeker = GetComponent<Seeker>();
        _rd = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Hp = MaxHp;

        target = CharacterStateManager.Instance.transform;
        InvokeRepeating(nameof(UpdatePath), 0f, updateTime);
    }
    private void Update()
    {
        PlayerDetection(detection);
        HandleAnimation();
    }

    private new void PlayerDetection(Collider2D detectionCollider)
    {
        if (detectionCollider.IsTouching(CharacterStateManager.Instance.GetComponent<Collider2D>()))
        {
            CharacterStateManager.Instance.Hurt(attack, transform.position);
        }
        if(Vector2.Distance(transform.position, target.position) <= 2f)_isAttacking = true;
        else _isAttacking = false;
    }
    void HandleAnimation()
    {
        anim.SetFloat(Speed, _rd.velocity.magnitude);
        anim.SetBool(PlayerDetected, _isAttacking);
    }
    void UpdatePath()
    {
        //if(seeker.IsDone())
            _seeker.StartPath(_rd.position, target.position, OnPathComplete);
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWaypoint = 0;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(_path == null) return;
        if (_currentWaypoint >= _path.vectorPath.Count)
        {
            //reachedEndOfPath=true;
            return;
        }
        else
        {
            //reachedEndOfPath = false;
        }
        
        var dir = ((Vector2)_path.vectorPath[_currentWaypoint] - _rd.position).normalized;
        var force = dir * speed;
        if (Vector2.Distance(target.position, _rd.position) < detectionDistance)
        {
            _isMoving = true;
        }
        if (_isMoving && Vector2.Distance(target.position, _rd.position) < trackingDistance)
        {
            _rd.velocity = force;

            float distance = Vector2.Distance(_rd.position, _path.vectorPath[_currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                _currentWaypoint++;
            }
        }else
        {
            _isMoving = false;
            _rd.velocity = Vector2.zero;
        }

        // Animation
        if(force.x < 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if(force.x > 0f)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }
}
