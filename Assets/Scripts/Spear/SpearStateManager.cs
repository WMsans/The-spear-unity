using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpearStateManager : MonoBehaviour
{
    [SerializeField] Rigidbody2D _playerRd;
    [SerializeField] Transform _spearHead; 
    public Camera cam;
    public Rigidbody2D rd;
    [Range(0f, 1f)] public float spearPokeSpeed;
    [Range(0f, 5f)] public float spearPokeDistance; 

    SpearBaseState currentState;
    public SpearNormalState normalState = new();
    public SpearAnchorState anchorState = new();
    public SpearPokeState pokeState = new();

    public float reachDistance {  get; set; }
    public Vector2 spearPosition { get { return _playerRd.position; } }
    public Vector2 anchorPoint { get; set; }
    public Vector2 spearHead { get { return _spearHead.position; } }

    // Start is called before the first frame update
    void Start()
    {
        currentState = normalState;

        currentState.EnterState(this);
    }

    void Update()
    {
        currentState.UpdateState(this);
    }
    void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
    }
    void LateUpdate()
    {
        currentState.LateUpdateState(this);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        currentState.OnCollisionStay2DState(this, collision);
    }

    public void SwitchState(SpearBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
}
