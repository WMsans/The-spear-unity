using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyGFX : MonoBehaviour
{
    [SerializeField]AIPath aIPath;

    // Update is called once per frame
    void Update()
    {
        if(aIPath.desiredVelocity.x >= 0.01f)//moving to the right
        {
            transform.localScale = new(-1f,1f,1f );
        }else if(aIPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new(1f, 1f, 1f);
        }
    }
}
