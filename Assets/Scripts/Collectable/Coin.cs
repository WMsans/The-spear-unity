using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] float attractorSpeed = 5f;
    bool collected = false;
    private void Awake()
    {
        Destroy(gameObject, 7.5f);

        Physics2D.IgnoreLayerCollision(11, 11);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if(!collected) 
                CoinCounter.instance.CoinCount++;
            collected = true;
            Destroy(gameObject);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            transform.position = Vector3.MoveTowards(transform.position, collision.transform.position, attractorSpeed * (1f / Vector3.Distance(transform.position, collision.transform.position)) * Time.deltaTime);

        }
    }
}
