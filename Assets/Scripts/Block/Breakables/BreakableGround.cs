using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreakableGround : MonoBehaviour
{
    [SerializeField] GameObject ground;
    [SerializeField] Collider2D playerDetection;
    public bool destroyed = false;
    bool readyToDestroy = false;
    bool readyToGenerate = true;

    bool standed = false;

    void GenerateBlock()
    {
        if (destroyed)
        {
            ground.SetActive(true);
        }

        destroyed = false;
    }
    void DestroyBlock()
    {
        if (!destroyed) {
            ground.SetActive(false);
        }
        destroyed = true;
        
    }
    void Update()
    {
        if(playerDetection.IsTouching(CharacterStateManager.Instance.GetComponent<Collider2D>()))
        {
            standed = true;
        }
        else
        {
            standed = false;
        }
        if (CharacterStateManager.Instance.AbleToReset && readyToGenerate)
        {
            GenerateBlock();
            readyToDestroy = false;
        }
        else if (ground.GetComponent<GroundCollisions>().Anchored || standed)
        {
            readyToDestroy = true;
        }else if (readyToDestroy)
        {
            DestroyBlock();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            readyToGenerate = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            readyToGenerate = true;
        }
    }
}
