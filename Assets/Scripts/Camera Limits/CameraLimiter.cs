using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraLimiter : MonoBehaviour
{
    [SerializeField] CameraFollower cameraFollower;

    Collider2D _col;
    bool _enabled = false;
    void Awake()
    {
        _col = GetComponent<Collider2D>();
        _enabled = false;
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _enabled = true;
            if (_col.bounds.Contains(collision.gameObject.transform.position)){
                cameraFollower.CameraLimiter = this;
                //Make this the limiter
                cameraFollower.MinPoint = _col.bounds.min;
                cameraFollower.MaxPoint = _col.bounds.max;
            }
            else
            {
                if (cameraFollower.CameraLimiter == this)
                {
                    cameraFollower.CameraLimiter = null;
                }
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && _enabled)
        {
            _enabled = false;
            if (cameraFollower.CameraLimiter == this)
            {
                cameraFollower.CameraLimiter = null;
            }
        }
    }
}
