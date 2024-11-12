using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HongYin : MonoBehaviour
{
    [SerializeField] private Rigidbody2D spearRb;
    [SerializeField] private Transform point;

    private void FixedUpdate()
    {
        transform.localRotation = Quaternion.Euler(0, 0, -spearRb.rotation);
        transform.position = point.position;
    }
}
