using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWave : MonoBehaviour
{
    [SerializeField] GameObject spawnSfx;
    [SerializeField] GameObject[] enemies;
    private void Start()
    {
        foreach (var enemy in enemies)
        {
            enemy.SetActive(false);
        }
    }
    public void GenerateEnemies()
    {
        foreach(var enemy in enemies)
        {
            enemy.SetActive(true);
            var enemPos = enemy.transform.position;
            if (spawnSfx != null)
            {
                Instantiate(spawnSfx, transform).transform.position = enemPos;
            }
        }
    }
    public GameObject[] GetEnemies()
    {
        return enemies;
    }
}
