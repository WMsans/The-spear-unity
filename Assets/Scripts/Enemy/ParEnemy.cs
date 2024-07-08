using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParEnemy : MonoBehaviour
{
    [SerializeField] float attack = 1f;
    [SerializeField] float maxHP = 1f;
    [SerializeField] float beatBack = 500f;
    [SerializeField] string[] HurtingEvents;
    public float HP {  get; set; }
    private void Start()
    {
        HP = maxHP;
    }
    public bool PlayerDetection(Collider2D detection)
    {
        if (detection.IsTouching(CharacterStateManager.Instance.GetComponent<Collider2D>()))
        {
            CharacterStateManager.Instance.Hurt(attack, transform.position);
            return true;
        }
        return false;
    }
    public void Hurt(float attack, Vector2 otherPos)
    {
        HP -= attack;
        if( HP <= 0)
        {
            Die();
        }
        var rb = GetComponent<Rigidbody2D>();
        rb.AddForce(beatBack * (rb.position - otherPos));
        foreach (var e in HurtingEvents) 
            Invoke(e, 0f);
    }
    public void Die()
    {
        Destroy(gameObject);
    }
}
