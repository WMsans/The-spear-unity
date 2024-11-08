using UnityEngine;
 
[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;
 
    private float oldPosition;
 
    void Start()
    {
        oldPosition = transform.position.x;
    }
 
    void Update()
    {
        if (!Mathf.Approximately(transform.position.x, oldPosition))
        {
            if (onCameraTranslate != null)
            {
                float delta = oldPosition - transform.position.x;
                onCameraTranslate(delta);
            }
 
            oldPosition = transform.position.x;
        }
    }
}