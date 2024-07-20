using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollisions : MonoBehaviour 
{
    [SerializeField] Collider2D up;
    [SerializeField] Collider2D down;
    [SerializeField] Collider2D left;
    [SerializeField] Collider2D right;

    public Collider2D Up { get { return up; } }
    public Collider2D Down { get { return down; } }
    public Collider2D Left { get { return left; } }
    public Collider2D Right { get { return right; } }
    public Collider2D[] Colliers {  get { return new Collider2D[] { right, down, left, up }; } }
    public bool Anchored { get; set; } = false;

    
}
