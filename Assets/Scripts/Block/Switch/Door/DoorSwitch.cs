using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwitch : SwitchObj
{
    [SerializeField] GameObject[] doors;    
    void Awake()
    {
        miMethod = OpenDoor;
    }
    void OpenDoor(bool state)
    {
        foreach (var door in doors)
        {
            door.SetActive(!state);
        }
    }
    
}
