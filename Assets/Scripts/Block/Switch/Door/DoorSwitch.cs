using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DoorSwitch : SwitchObj
{
    [SerializeField] GameObject[] doors;
    [SerializeField] List<Animator> anim;
    void Awake()
    {
        miMethod = OpenDoor;
    }
    void OpenDoor(bool state)
    {
        foreach (var t in doors)
        {
            //t.SetActive(!state);
        }

        foreach (var a in anim)
        {
            a.SetBool("Opened", state);
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
