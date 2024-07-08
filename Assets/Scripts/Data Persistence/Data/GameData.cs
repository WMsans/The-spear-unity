using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData
{
    public int coinCount;
    public Vector3 playerPosition;
    public float maxHP;
    public float Attack;
    public Vector3 cameraPosition;
    public SerializableDictionary<string, bool> goldblockCollected;
    public SerializableDictionary<string, bool> switchOpened;
    public SerializableDictionary<string, float> blockPositionX;
    public SerializableDictionary<string, float> blockPositionY;

    public GameData()
    {
        coinCount = 0;
        playerPosition = new(0f, 0f);
        cameraPosition = playerPosition;
        maxHP = 5f;
        Attack = 4f;
        goldblockCollected = new();
        switchOpened = new();
        blockPositionX = new();
        blockPositionY = new();
    }
}
