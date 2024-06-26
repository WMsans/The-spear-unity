using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSideBreakableBlock : BreakableBlock, IDataPersistence
{
    [SerializeField] GameObject face; 
    [SerializeField] string id;
    [ContextMenu("Generate Guid")]
    void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    bool destroyed = false;
    private void Update()
    {
        if (BlockHP <= 0)
        {
            DestroyBlock();
        }
    }
    public new void DestroyBlock()
    {
        destroyed = true;
        // Destroy game object
        Destroy(gameObject);
    }
    public void LoadData(GameData gameData)
    {
        gameData.goldblockCollected.TryGetValue(id, out destroyed);
        if (destroyed)
        {
            Destroy(gameObject);
        }
    }
    public void SaveData(ref GameData gameData)
    {
        if (gameData.goldblockCollected.ContainsKey(id))
        {
            gameData.goldblockCollected.Remove(id);
        }
        gameData.goldblockCollected.Add(id, destroyed);
    }
}
