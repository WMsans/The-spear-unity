using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreakableGround : MonoBehaviour
{
    [SerializeField] GameObject ground;
    public bool destroyed = false;
    bool readyToDestroy = false;
    

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
        if (CharacterStateManager.Instance.AbleToReset)
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
}
