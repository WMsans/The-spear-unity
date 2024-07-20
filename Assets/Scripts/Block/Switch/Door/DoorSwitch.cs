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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        foreach (var door in doors)
        {
            Gizmos.DrawLine(transform.position, door.transform.position);
        }
    }
}
