using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    SpriteRenderer sprRend;
    Collider2D col;
    private void Awake()
    {
        sprRend = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }
    public void DoSwitch(bool switched)
    {
        if (switched)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
    public void Open()
    {
        sprRend.enabled = false;
        col.enabled = false;
    }
    public void Close()
    {
        sprRend.enabled=true;
        col.enabled = true;
    }
}
