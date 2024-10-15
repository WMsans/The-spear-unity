using System;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterStateManager : MonoBehaviour, IDataPersistence
{
    [SerializeField] float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0f, 1f)][SerializeField] float m_CrouchSpeed = .36f;           // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)][SerializeField] float m_MovementSmoothing = .05f;   // How much to smooth out the movement
    [Range(0f, 50f)][SerializeField] float maxHorizontalSpeed;                  // The walk speed of the player
    [Range(0f, 100f)][SerializeField] float maxBouncedSpeed;
    [SerializeField] float accel;
    [SerializeField] float decel; 
    [SerializeField] bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] Transform m_LeftCheck;
    [SerializeField] Transform m_RightCheck;
    [SerializeField] Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
    [SerializeField] float m_GroundBuff;                                // Distance above ground is alloed to jump
    [SerializeField] int m_CoyoteTime;                                  // Coyote time 
    [SerializeField] float lowJumpMultiplier = 2.0f;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float maxFallSpeed;
    [SerializeField] float stiff;
    [SerializeField] float invincible;
    [SerializeField] Camera m_Camera;
    [SerializeField] SpearStateManager spear;
    [SerializeField] GameObject m_SpearObject;
    public Transform spearPoint;
    public Animator animator;
    public static CharacterStateManager Instance { get; private set; }


    public CharacterBaseState currentState;
    public CharacterNormalState normalState = new();
    public CharacterAnchorState anchorState = new();
    public CharacterStiffState stiffState = new();


    public bool keyJump {  get; private set; }
    public bool keyJumpDown { get; private set; }
    public float keyHor { get; private set; }
    public bool keyCrouch { get; private set; }
    public float MaxHorizontalSpeed { get {return maxHorizontalSpeed; } }
    public float MaxBouncedHorizontalSpeed { get { return maxBouncedSpeed; } }
    public float Accel {  get { return accel; } }
    public float Decel { get { return decel; } }
    public float JumpForce { get { return m_JumpForce; } }
    public float CrouchSpeed { get { return m_CrouchSpeed; } }
    public float MovementSmoothing { get { return m_MovementSmoothing; } }
    public float Crouch_speed { get { return m_CrouchSpeed; } }
    public bool AirControl { get { return m_AirControl; } }
    public LayerMask WhatIsGround { get { return m_WhatIsGround; } }
    public Transform GroundCheck { get { return m_GroundCheck; } }
    public Transform CeilingCheck { get { return m_CeilingCheck; } }
    public Transform LeftCheck {  get { return m_LeftCheck; } }
    public Transform RightCheck { get { return m_RightCheck; } }
    public Collider2D CrouchDisableCollider { get { return m_CrouchDisableCollider; } }
    public float GroundBuff { get { return m_GroundBuff; } }
    public int CoyoteTime { get { return m_CoyoteTime; } }
    public float LowJumpMultiplier { get { return lowJumpMultiplier; } }
    public float FallMultiplier { get { return fallMultiplier; } }
    public SpearStateManager Spear { get { return spear; } }
    public Rigidbody2D SpearRd { get { return spear.GetComponent<Rigidbody2D>(); } }
    public bool Bounced { get; set; } = false;
    public bool Grounded { get; set; } = false;
    public bool AbleToReset { get; set; } = true;
    public int BoucedFace { get; set; } = -1;
    public int AllowMoveTimer { get; set; } = 0;
    public float MaxFallSpeed { get { return maxFallSpeed; } }
    public Vector2 SavePosition { get; set; } = new();
    public float Stiff { get { return stiff; } }
    public float Invincible {  get { return invincible; } }
    public float InvincibleTimer { get; set; }
    public bool IsCrouched { get; set; } = false;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            Debug.LogError("Found more than one Player in the scene.");
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        SwitchState(normalState);
    }
    void Update()
    {
        if(spear == null)
        {
            spear = Instantiate(m_SpearObject).GetComponent<SpearStateManager>();
            spear.Player = this;
            spear.Cam = m_Camera;
        }
        currentState.UpdateState(this);
    }
    void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
    }
    public void SwitchState(CharacterBaseState state)
    {
        if(currentState !=  null)   
            currentState.ExitState(this);
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
    public void Hurt(float damage, Vector2 enemyPos)
    {
        if(InvincibleTimer <= 0)
        {
            HPCounter.instance.HP -= damage;
            if (HPCounter.instance.HP <= 0) Die();

            StartCoroutine(FlashAfterHurt());
        }
    }
    private IEnumerator FlashAfterHurt()
    {
        var flashDelay = 0.0833f;
        var sprite = GetComponent<SpriteRenderer>();
        for (InvincibleTimer = Invincible; InvincibleTimer > 0f; InvincibleTimer--)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(flashDelay);
            sprite.enabled = true;
            yield return new WaitForSeconds(flashDelay);
        }
    }
    public void Die()
    {
        Debug.Log("Player Died");
        if(Debuger.instance.DebugMode) HPCounter.instance.HP = HPCounter.instance.maxHP;
    }
    public void LoadData(GameData gameData)
    {
        if(transform.position == new Vector3())
            transform.position = gameData.playerPosition;

    }
    public void SaveData(ref GameData gameData)
    {
        gameData.playerPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(m_GroundCheck.position, .2f);
        Gizmos.DrawWireSphere(m_CeilingCheck.position, .2f);
        Gizmos.DrawWireSphere(m_LeftCheck.position, .2f);
        Gizmos.DrawWireSphere(m_RightCheck.position, .2f);
    }
}
