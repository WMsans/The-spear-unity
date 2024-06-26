using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour, IDataPersistence
{
    public static CameraFollower instance;
    [SerializeField] Transform follow;
    [SerializeField] Camera cam;
    [SerializeField] float followSpeed;
    [SerializeField] float lookForward;

    Vector3 _moveDamp = new();
    Vector3 _lastFollowPos = new();
    public Vector2 MinPoint { get; set; } = Vector2.zero;
    public Vector2 MaxPoint { get; set; } = Vector2.zero;
    public CameraLimiter CameraLimiter { get; set; } = null;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            Debug.LogError("Found more than one Virtual Camera in the scene.");
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
            
    }
    void Start()
    {
        _lastFollowPos = follow.position;
        CameraLimiter = null;
    }
    void Update()
    {
        var _targetPos = follow.position + (follow.position - _lastFollowPos) * lookForward;
        
        if (Vector2.Distance(MinPoint, MaxPoint) > 0f && CameraLimiter != null)
        {
            _targetPos.x = Mathf.Clamp(_targetPos.x, MinPoint.x + Extents(cam).x, MaxPoint.x - Extents(cam).x);
            _targetPos.y = Mathf.Clamp(_targetPos.y, MinPoint.y + Extents(cam).y, MaxPoint.y - Extents(cam).y);
        }
        transform.position = Vector3.SmoothDamp(transform.position, _targetPos, ref _moveDamp, followSpeed);
        _lastFollowPos = follow.position;

        cam.transform.position = new(transform.position.x, transform.position.y, cam.transform.position.z);
    }
    private Vector2 Extents(Camera cam)
    {
        if (cam.orthographic)
            return new(cam.orthographicSize * Screen.width / Screen.height, cam.orthographicSize);
        else
        {
            Debug.LogError("Camera is not orthographic!", cam);
            return new();
        }
    }
    private Vector2 BoundsMin(Camera cam)
    {
        Vector2 pos = cam.transform.position;
        return pos - Extents(cam);
    }
    private Vector2 BoundsMax(Camera cam)
    {
        Vector2 pos = cam.transform.position;
        return pos + Extents(cam);
    }
    // Load and Save system
    public void LoadData(GameData gameData)
    {
        transform.position = gameData.cameraPosition;
    }
    public void SaveData(ref GameData gameData)
    {
        gameData.cameraPosition = transform.position;
    }
}
