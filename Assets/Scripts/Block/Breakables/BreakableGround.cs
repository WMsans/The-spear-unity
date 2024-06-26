using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreakableGround : MonoBehaviour
{
    [SerializeField] GameObject ground;
    public bool destroyed = false;
    bool readyToDestroy = false;
    bool readyToGenerate = true;

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
        if (CharacterStateManager.Instance.AbleToReset && readyToGenerate)
        {
            GenerateBlock();
            readyToDestroy = false;
        }
        else if (ground.GetComponent<GroundCollisions>().Anchored)
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
