using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGroup : MonoBehaviour
{
    [SerializeField] GameObject[] doors;
    public void OpenDoors()
    {
        foreach(var door in doors)
        {
            door.SetActive(false);
        }
    }
    public void CloseDoors()
    {
        foreach (var door in doors)
        {
            door.SetActive(true);
        }
    }
}
