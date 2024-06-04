using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateManager : MonoBehaviour
{
    [SerializeField] float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0f, 1f)][SerializeField] float m_CrouchSpeed = .36f;           // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)][SerializeField] float m_MovementSmoothing = .05f;   // How much to smooth out the movement
    [Range(0f, 50f)][SerializeField] float maxHorizontalSpeed;                  // The walk speed of the player
    [Range(0f, 100f)][SerializeField] float maxBouncedSpeed;
    [SerializeField] bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
    [SerializeField] float m_GroundBuff;                                // Distance above ground is alloed to jump
    [SerializeField] int m_CoyoteTime;                                  // Coyote time 
    [SerializeField] float lowJumpMultiplier = 2.0f;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float maxFallSpeed;
    [SerializeField] SpearStateManager spear;
    

    CharacterBaseState currentState;
    public CharacterNormalState normalState = new();
    public CharacterAnchorState anchorState = new();

    public bool keyJump {  get; private set; }
    public bool keyJumpDown { get; private set; }
    public float keyHor { get; private set; }
    public bool keyCrouch { get; private set; }
    public float MaxHorizontalSpeed { get {return maxHorizontalSpeed; } }
    public float MaxBouncedHorizontalSpeed { get { return maxBouncedSpeed; } }
    public float JumpForce { get { return m_JumpForce; } }
    public float CrouchSpeed { get { return m_CrouchSpeed; } }
    public float MovementSmoothing { get { return m_MovementSmoothing; } }
    public float Crouch_speed { get { return m_CrouchSpeed; } }
    public bool AirControl { get { return m_AirControl; } }
    public LayerMask WhatIsGround { get { return m_WhatIsGround; } }
    public Transform GroundCheck { get { return m_GroundCheck; } }
    public Transform CeilingCheck { get { return m_CeilingCheck; } }
    public Collider2D CrouchDisableCollider { get { return m_CrouchDisableCollider; } }
    public float GroundBuff { get { return m_GroundBuff; } }
    public int CoyoteTime { get { return m_CoyoteTime; } }
    public float LowJumpMultiplier { get { return lowJumpMultiplier; } }
    public float FallMultiplier { get { return fallMultiplier; } }
    public SpearStateManager Spear { get { return spear; } }
    public Rigidbody2D SpearRd { get { return spear.GetComponent<Rigidbody2D>(); } }
    public bool Bounced { get; set; }
    public float MaxFallSpeed { get { return maxFallSpeed; } }

    // Start is called before the first frame update
    void Start()
    {
        SwitchState(normalState);
    }
    void Update()
    {
        currentState.UpdateState(this);
    }
    void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
    }
    public void SwitchState(CharacterBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
    public void GetKeys(bool _keyjump, bool _keyjumpdown, float _keyhor, bool _keycrouch)
    {
        keyJump = _keyjump;
        keyJumpDown = _keyjumpdown;
        keyHor = _keyhor;
        keyCrouch = _keycrouch;
    }
}
