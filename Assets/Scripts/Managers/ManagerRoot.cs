using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerRoot : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
