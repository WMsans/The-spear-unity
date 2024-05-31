using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpearStateManager : MonoBehaviour
{
    [SerializeField] CharacterStateManager _player;
    [SerializeField] Transform _spearHead; 
    [SerializeField] Camera cam;
    [Range(0f, 1f)][SerializeField] float spearPokeSpeed;
    [Range(0f, 5f)][SerializeField] float spearPokeDistance;
    [Range(0f, 5f)][SerializeField] float spearSpinSpeed; 

    SpearBaseState currentState;
    public SpearNormalState normalState { get; private set; } = new();
    public SpearAnchorState anchorState { get; private set; } = new();
    public SpearPokeState pokeState { get; private set; } = new();

    public float ReachDistance {  get; set; }
    public CharacterStateManager Player { get { return _player; } }
    public Vector2 SpearPosition { get { return _player.GetComponent<Rigidbody2D>().position; } }
    public bool Anchored { get; set; }
    public Vector2 AnchorPoint { get; set; }
    public Vector2 SpearHead { get { return _spearHead.position; } }
    public Camera Cam { get { return cam; } }
    public float PokeSpeed { get { return spearPokeSpeed; } }
    public float PokeDistance { get { return spearPokeDistance; } }
    public float SpinSpeed { get { return spearSpinSpeed; } }
    public float ReadyToPokeTimer { get; set;}

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
