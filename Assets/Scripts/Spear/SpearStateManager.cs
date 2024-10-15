using UnityEngine;

public class SpearStateManager : MonoBehaviour
{
    [SerializeField] CharacterStateManager _player;
    [SerializeField] Transform _spearHead; 
    [SerializeField] Camera cam;
    [Range(0f, 1f)][SerializeField] float spearPokeSpeed;
    [Range(0f, 5f)][SerializeField] float spearPokeDistance;
    [Range(0f, 5f)][SerializeField] float spearSpinSpeed;
    [SerializeField] float spearPokeCoolDown;

    public SpearBaseState currentState { get; private set; }
    public SpearNormalState normalState { get; private set; } = new();
    public SpearAnchorState anchorState { get; private set; } = new();
    public SpearPokeState pokeState { get; private set; } = new();
    public SpearStiffState stiffState { get; private set; } = new();
    public float ReachDistance {  get; set; }
    public CharacterStateManager Player { 
        get => _player;
        set => _player = value;
    }
    public Vector2 SpearPosition => _player.spearPoint.position;
    public bool Anchored { get; set; }
    public Vector2 AnchorPoint { get; set; }
    public GameObject AnchorBlock { get; set; }
    public Vector2 SpearHead => _spearHead.position;
    public Camera Cam { get { return cam; } set { cam = value; } }
    public float PokeSpeed { get { return spearPokeSpeed; } }
    public float PokeDistance { get { return spearPokeDistance; } }
    public float SpinSpeed { get { return spearSpinSpeed; } }
    public float ReadyToPokeTimer { get; set;}
    public float ReadyToPokeTime { get { return spearPokeCoolDown; } }

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
