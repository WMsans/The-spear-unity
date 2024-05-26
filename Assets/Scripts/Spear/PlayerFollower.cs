using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerRd;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = playerRd.position;
    }
}
