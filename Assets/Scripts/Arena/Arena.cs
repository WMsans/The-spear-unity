using System;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    [SerializeField] DoorGroup dg;
    [SerializeField] GameObject[] wavesObject;
    int waveNum = 0;
    List<GameObject> enemies = new();
    bool challenging = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartChallenge();
        }
    }
    private void Start()
    {
        dg.OpenDoors();
    }
    void Update()
    {
        for (var i = 0; i < enemies.Count; i++)
        {
            var e = enemies[i];
            if(e == null) enemies.Remove(e);
        }
        // Check for next wave
        if (enemies.Count <= 0 && challenging)
        {
            if(waveNum >= wavesObject.Length)
                EndChallenge();
            else 
                GenerateWave(waveNum++);
        }
    }
    void StartChallenge()
    {
        dg.CloseDoors();
        challenging = true;
    }
    void EndChallenge()
    {
        dg.OpenDoors();
        challenging = false;
        foreach(var e in enemies)
        {
            Destroy(e);
        }
    }
    void GenerateWave(int nowWaveNum)
    {
        var _newWave = wavesObject[nowWaveNum];
        _newWave.GetComponent<Wave>().GenerateEnemies();
        var _newEnemies = _newWave.GetComponent<Wave>().GetEnemies();
        foreach (var newE in _newEnemies)
        {
            enemies.Add(newE);
        }
    }
}