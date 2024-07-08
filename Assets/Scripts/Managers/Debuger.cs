using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debuger : MonoBehaviour
{
    public static Debuger instance;
    public bool DebugMode = false;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.LogError("Found more than one Debuger in the scene.");
        }
        else
            instance = this;
    }
}
