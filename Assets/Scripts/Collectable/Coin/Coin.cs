using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using Unity.VisualScripting;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] float attractorSpeed = 5f;
    [SerializeField] float destroyTime = 7.5f;
    [SerializeField] Collider2D Detecter;
    [SerializeField] Collider2D Collider;
    bool collected = false;
    bool attracted = false;

    Rigidbody2D rb;
    Rigidbody2D playerRb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, destroyTime);

        Physics2D.IgnoreLayerCollision(11, 11);
    }
    private void FixedUpdate()
    {
        if (attracted)
        {
            //rb.position = Vector2.MoveTowards(rb.position, playerRb.position, attractorSpeed * (1f / Vector2.Distance(rb.position, playerRb.position)) * Time.deltaTime);
            rb.gravityScale = 0f;
            rb.AddForce((playerRb.position - rb.position).normalized * attractorSpeed * (1f / Vector2.Distance(rb.position, playerRb.position)) );//* Time.deltaTime
            if (Vector2.Distance(rb.position, playerRb.position) <= 1)
            {
                if (!collected)
                    CoinCounter.instance.CoinCount++;
                collected = true;
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            attracted = true;
            Collider.enabled = false;
            playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
        }
    }
}
