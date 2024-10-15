using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ParEnemy : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] protected float attack = 1f;
    [FormerlySerializedAs("maxHP")] [SerializeField] protected float maxHp = 1f;
    [SerializeField] protected int coinNumber = 0;
    [SerializeField] protected GameObject coinPrefab;
    [SerializeField] protected float beatBack = 500f;
    [FormerlySerializedAs("HurtingEvents")] [SerializeField] protected string[] hurtingEvents;
    protected float Hp {  get; set; }
    protected float MaxHp => maxHp;
    public float AttackNum => attack;

    private void Start()
    {
        Hp = maxHp;
    }

    protected bool PlayerDetection(Collider2D detection)
    {
        if (detection.IsTouching(CharacterStateManager.Instance.GetComponent<Collider2D>()))
        {
            CharacterStateManager.Instance.Hurt(attack, transform.position);
            return true;
        }
        return false;
    }
    public bool PlayerDetection(Collider2D[] detections)
    {
        foreach (var detection in detections)
        {
            if (detection.IsTouching(CharacterStateManager.Instance.GetComponent<Collider2D>()))
            {
                CharacterStateManager.Instance.Hurt(attack, transform.position);
                return true;
            }
        }
        return false;
    }
    public void Hurt(float attack, Vector2 otherPos)
    {
        Hp -= attack;
        if( Hp <= 0)
        {
            Die();
        }
        var rb = GetComponent<Rigidbody2D>();
        rb.AddForce(beatBack * (rb.position - otherPos));
        foreach (var e in hurtingEvents) 
            Invoke(e, 0f);
    }
    public void Die()
    {
        CoinGenerate();
        Destroy(gameObject);
    }
    void CoinGenerate()
    {
        for (var i = 1; i <= coinNumber; i++)
        {
            var force = Quaternion.Euler(0f, 0f, Random.Range(-30f, 30f)) * new Vector3(0, 10, 0);
            var coin = Instantiate(coinPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            coin.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        }
    }
}
