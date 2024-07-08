using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FallingBullets : ParEnemy
{
    [SerializeField] Collider2D detection;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))// If touches player
        {
            if(PlayerDetection(detection)) Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerDetection(detection);
        Destroy(gameObject);
    }
}
