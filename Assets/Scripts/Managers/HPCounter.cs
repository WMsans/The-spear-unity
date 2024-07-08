using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPCounter : MonoBehaviour, IDataPersistence
{
    public static HPCounter instance;
    public float maxHP = 5f;
    public float HP = 5f;
    public float Attack = 4f;
    [SerializeField] Text HPCountText;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.LogError("Found more than one HPCounter in the scene.");
        }
        else
            instance = this;
    }
    void Update()
    {
        HPCountText.text = "HP: " + HP.ToString();
    }
    public void LoadData(GameData gameData)
    {
        maxHP = gameData.maxHP;
        Attack = gameData.Attack;
        HP = maxHP;
    }
    public void SaveData(ref GameData gameData)
    {
        gameData.maxHP = maxHP;
        gameData.Attack = Attack;
    }
}
