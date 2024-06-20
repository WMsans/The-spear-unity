using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBlock : MonoBehaviour
{
    public int BlockHP;
    private void Update()
    {
        if (BlockHP <= 0)
        {
            DestroyBlock();
        }
    }
    public void DestroyBlock()
    {
        // Destroy game object
        Destroy(gameObject);
    }
    public void DamageBlock()
    {
        BlockHP--;
    }
    public void DamageBlock(int damage)
    {
        BlockHP -= damage;
    }
}
