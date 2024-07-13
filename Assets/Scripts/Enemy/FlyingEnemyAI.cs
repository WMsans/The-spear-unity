using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;

public class FlyingEnemyAI : ParEnemy
{
    [Header("Player Follow")]
    [SerializeField] Collider2D detection;
    [SerializeField] float detectionDistance;
    [SerializeField] Transform target;
    [SerializeField] float speed = 200f;
    [SerializeField] float nextWaypointDistance = 3f;
    [SerializeField] float updateTime = 0.5f;

    Path path;
    int currentWaypoint = 0;
    //bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rd;
    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        rd = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        HP = MaxHP;

        target = CharacterStateManager.Instance.transform;
        InvokeRepeating("UpdatePath", 0f, updateTime);
    }
    private void Update()
    {
        PlayerDetection(detection);
        
    }
    void UpdatePath()
    {
        //if(seeker.IsDone())
            seeker.StartPath(rd.position, target.position, OnPathComplete);
    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(path == null) return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            //reachedEndOfPath=true;
            return;
        }
        else
        {
            //reachedEndOfPath = false;
        }
        
        var dir = ((Vector2)path.vectorPath[currentWaypoint] - rd.position).normalized;
        var force = dir * speed;
        if (Vector2.Distance(target.position, rd.position) < detectionDistance)
        {
            rd.velocity = force;

            float distance = Vector2.Distance(rd.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }
        }

        // Animation
        if(force.x < 0f)
        {
            transform.localScale = new(-1, 1, 1);  
        }
        else if(force.x > 0f)
        {
            transform.localScale = new(1, 1, 1);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);
    }
}
