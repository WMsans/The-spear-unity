using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData
{
    public int coinCount;
    public Vector3 playerPosition;
    public Vector3 cameraPosition;
    public SerializableDictionary<string, bool> goldblockCollected;
    public SerializableDictionary<string, bool> switchOpened;
    
    public GameData()
    {
        coinCount = 0;
        playerPosition = new(0f, 0f);
        cameraPosition = playerPosition;
        goldblockCollected = new();
        switchOpened = new();
    }
}
