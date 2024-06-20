using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goldblock : BreakableBlock, IDataPersistence
{
    [SerializeField] int blockHP = 1;
    [SerializeField] int coinNumber = 1;
    [SerializeField] string id;
    [SerializeField] GameObject coinPrefab;
    [ContextMenu("Generate Guid")]
    void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    bool collected = false;
    private void Start()
    {
        BlockHP = blockHP;
    }
    private void Update()
    {
        if (BlockHP <= 0)
        {
            DestroyBlock();
        }
    }
    public new void DestroyBlock()
    {
        // ±¬½ð±Ò
        CoinGenerate();
        // Destroy game object
        Destroy(gameObject);
    }
    public void LoadData(GameData gameData)
    {
        gameData.goldblockCollected.TryGetValue(id, out collected);
        if (collected)
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
        gameData.goldblockCollected.Add(id, collected);
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
