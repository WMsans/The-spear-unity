using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;

public class TrapBlock : MonoBehaviour
{
    [SerializeField] float damage = 1f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var other = collision.gameObject;
        if (other.CompareTag("Player"))
        {
            other.GetComponent<CharacterStateManager>().Hurt(damage, other.transform.position);
            other.GetComponent<CharacterStateManager>().SwitchState(other.GetComponent<CharacterStateManager>().stiffState);
        }
        else if (other.CompareTag("Enemy"))
        {
            other.GetComponent<ParEnemy>().Die();
        }
        
    }
}
