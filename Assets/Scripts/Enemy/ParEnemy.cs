using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParEnemy : MonoBehaviour
{
    [SerializeField] float attack = 1f;
    [SerializeField] float maxHP = 1f;
    [SerializeField] int coinNumber = 0;
    [SerializeField] GameObject coinPrefab;
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
