using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableChain : SwitchObj
{
    void Cut(bool state)
    {
        if(state)
            Destroy(gameObject);
    }
    private void Awake()
    {
        miMethod = Cut;
    }
}
