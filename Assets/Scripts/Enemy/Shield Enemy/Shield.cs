using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject enemy;
    public Transform enemyCenter;
    public Transform shieldCenter;
    Transform target;
    Rigidbody2D rb;
    float angle = 0f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        target = CharacterStateManager.Instance.transform;
    }
    private void Update()
    {
        var lookDir = target.position - shieldCenter.position;
        angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
    }
    private void LateUpdate()
    {
        rb.rotation = angle;
        transform.position += enemyCenter.position - shieldCenter.position;
    }
    public void Poked(Vector2 pokePoint)
    {
        enemy.GetComponent<ShieldEnemyAI>().ShieldPoked = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStateManager>().Hurt(enemy.GetComponent<ParEnemy>().AttackNum, rb.position);
        }
    }
}
