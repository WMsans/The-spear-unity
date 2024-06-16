using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinCounter : MonoBehaviour, IDataPersistence
{
    public static CoinCounter instance;
    int coinCount = 0;
    [SerializeField] Text coinCountText;
    public int CoinCount { get { return coinCount; } set { coinCount = value; } }
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.LogError("Found more than one CoinCounter in the scene.");
        }else 
            instance = this;
    }
    void Update()
    {
        coinCountText.text = coinCount.ToString();
    }
    public void LoadData(GameData gameData)
    {
        coinCount = gameData.coinCount;
    }
    public void SaveData(ref GameData gameData)
    {
        gameData.coinCount = coinCount;
    }
    
}
