using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public abstract class CharacterBaseState
{
    public abstract void EnterState(CharacterStateManager chara);
    public abstract void UpdateState(CharacterStateManager chara);
    public abstract void FixedUpdateState(CharacterStateManager chara);
}

public class CharacterNormalState : CharacterBaseState
{
    float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    float m_CrouchSpeed = .36f;                        // Amount of maxSpeed applied to crouching movement. 1 = 100%
    float m_MovementSmoothing = .05f;                  // How much to smooth out the movement
    float maxHorizontalSpeed;                          // The walk speed of the player
    bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
    float m_GroundBuff;                                // Distance above ground is alloed to jump
    int m_CoyoteTime;                                  // Coyote time 
    float lowJumpMultiplier = 2.0f;
    float fallMultiplier = 2.5f;

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector2 m_Velocity = Vector2.zero;
    private int _coyoteTimer = 0;
    public int CoyoteTimer { get { return _coyoteTimer; } set {  _coyoteTimer = value; } }

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    public override void EnterState(CharacterStateManager chara)
    {
        m_JumpForce = chara.JumpForce;                          
        m_CrouchSpeed = chara.CrouchSpeed;                      
        m_MovementSmoothing = chara.MovementSmoothing;          
        maxHorizontalSpeed = chara.MaxHorizontalSpeed;            
        m_AirControl = chara.AirControl;                        
        m_WhatIsGround = chara.WhatIsGround;                    
        m_GroundCheck = chara.GroundCheck;                      
        m_CeilingCheck = chara.CeilingCheck;                    
        m_CrouchDisableCollider = chara.CrouchDisableCollider;  
        m_GroundBuff = chara.GroundBuff;                        
        m_CoyoteTime = chara.CoyoteTime;                        
        lowJumpMultiplier = chara.LowJumpMultiplier;
        fallMultiplier = chara.FallMultiplier;

        m_Rigidbody2D = chara.GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();

    }
    public override void UpdateState(CharacterStateManager chara)
    {
        if (chara.Spear.Anchored)
        {
            chara.SwitchState(chara.anchorState);
        }
        
    }
    public override void FixedUpdateState(CharacterStateManager chara)
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(m_GroundCheck.position.x, m_GroundCheck.position.y - m_GroundBuff), k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != chara.gameObject)
            {
                m_Grounded = true;
                _coyoteTimer = m_CoyoteTime;
                chara.Bounced = false;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
        _coyoteTimer = Mathf.Max(_coyoteTimer - 1, 0); // debug
        

        Move(chara.keyHor * maxHorizontalSpeed * Time.fixedDeltaTime, chara.keyCrouch, chara.keyJumpDown, chara.keyJump, chara);
    }
    private void Move(float move, bool crouch, bool jumpDown, bool jump, CharacterStateManager chara)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch && m_Grounded)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new();
            if (chara.Bounced && !m_Grounded && Mathf.Sign(move) == Mathf.Sign(m_Rigidbody2D.velocity.x))
            {
                targetVelocity = new(Mathf.Min( Mathf.Max(Mathf.Abs( m_Rigidbody2D.velocity.x), Mathf.Abs( move*10f)), chara.MaxBouncedHorizontalSpeed) * Mathf.Sign(move), m_Rigidbody2D.velocity.y);
            }
            else
                targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
            
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector2.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip(chara);
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip(chara);
            }
        }
        // Fall faster
        if (m_Rigidbody2D.velocity.y < 0)
        {
            m_Rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // Fall faster after releasing jump key
        else if (m_Rigidbody2D.velocity.y > 0 && !chara.Bounced && !jump)
        {
            m_Rigidbody2D.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        // If the player should jump
        if ((m_Grounded || _coyoteTimer > 0) && jumpDown)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            if (m_Rigidbody2D.velocity.y < 0) m_Rigidbody2D.velocity *= Vector2.right;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }
    private void Flip(CharacterStateManager chara)
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = chara.transform.localScale;
        theScale.x *= -1;
        chara.transform.localScale = theScale;
    }
}

public class CharacterAnchorState : CharacterBaseState
{
    Rigidbody2D rd;
    Rigidbody2D spearRd;
    float _oriGrav;
    float m_JumpForce;
    float m_Speed;
    public override void EnterState(CharacterStateManager chara)
    {
        spearRd = chara.Spear.GetComponent<Rigidbody2D>();

        rd = chara.GetComponent<Rigidbody2D>();
        rd.velocity = Vector2.zero;
        m_JumpForce = chara.JumpForce;

        _oriGrav = rd.gravityScale;
        rd.gravityScale = 0f;

        m_Speed = chara.MaxHorizontalSpeed;
    }
    public override void UpdateState(CharacterStateManager chara)
    {
        if (!chara.Spear.Anchored)
        {
            rd.gravityScale = _oriGrav;
            rd.velocity = new Vector2(Mathf.Min(rd.velocity.x, m_Speed), 0f);
            chara.normalState.CoyoteTimer = chara.CoyoteTime;
            chara.SwitchState(chara.normalState);
        }
    }
    public override void FixedUpdateState(CharacterStateManager chara)
    {
        // Move with the spear
        rd.velocity = (chara.SpearRd.position - rd.position) * 20f;
        // Jump while anchored
        if (chara.keyJumpDown)
        {
            rd.gravityScale = _oriGrav;
            rd.velocity *= Vector2.right;
            rd.AddForce(new Vector2(0f, m_JumpForce));
            chara.Spear.Anchored = false;
            //chara.Spear.ReadyToPokeTimer = 50f;
            chara.SwitchState(chara.normalState);
        }
    }
}