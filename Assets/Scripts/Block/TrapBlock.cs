using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;

public class TrapBlock : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var other = collision.gameObject;
        if (other.CompareTag("Player"))
        {
            other.GetComponent<CharacterStateManager>().SwitchState(other.GetComponent<CharacterStateManager>().stiffState);
        }else if (other.CompareTag("Enemy"))
        {
            other.GetComponent<ParEnemy>().Die();
        }
        
    }
}
