using Unity.Loading;
using Unity.VisualScripting;
using UnityEngine;

public abstract class SpearBaseState
{
    public abstract void EnterState(SpearStateManager spear);
    public abstract void UpdateState(SpearStateManager spear);
    public abstract void FixedUpdateState(SpearStateManager spear);
    public abstract void OnCollisionStay2DState(SpearStateManager spear, Collision2D collision);
    public abstract void LateUpdateState(SpearStateManager spear);
}

public class SpearNormalState : SpearBaseState
{
    Camera cam;
    Rigidbody2D rd;

    Vector2 mousePos;

    public bool AutoPoke { get; set; } = false;
    public override void EnterState(SpearStateManager spear)
    {
        cam = spear.Cam;
        rd = spear.GetComponent<Rigidbody2D>();

        spear.AnchorBlock = null;
        
    }
    public override void UpdateState(SpearStateManager spear)
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        spear.ReachDistance += (0f - spear.ReachDistance) * spear.PokeSpeed;
        // If left mb pressed, poke
        if (((Input.GetButton("Fire1") && AutoPoke) || Input.GetButtonDown("Fire1")) && spear.ReadyToPokeTimer <= 0f && !spear.Player.IsCrouched )
        {
            spear.SwitchState(spear.pokeState);
        }
    }
    public override void FixedUpdateState(SpearStateManager spear)
    {
        spear.ReadyToPokeTimer = Mathf.Max(spear.ReadyToPokeTimer - 1, 0);

        var lookDir = mousePos - spear.SpearPosition;
        var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rd.rotation = angle;

    }
    public override void LateUpdateState(SpearStateManager spear)
    {
        spear.transform.position = new(spear.SpearPosition.x + spear.ReachDistance * Mathf.Cos((rd.rotation + 90) * Mathf.Deg2Rad), spear.SpearPosition.y + spear.ReachDistance * Mathf.Sin((rd.rotation + 90) * Mathf.Deg2Rad));
    }
    public override void OnCollisionStay2DState(SpearStateManager spear, Collision2D collision)
    {
        
    }
}

public class SpearPokeState : SpearBaseState
{
    Camera cam;
    Rigidbody2D rd;
    int m_WhatIsGround;
    float pokeSpeed;
    float pokeDis;
    Transform spearHead;
    int _faceIndex;

    Vector2 mousePos;

    public int FaceIndex { get { return _faceIndex; } }
    public override void EnterState(SpearStateManager spear)
    {
        cam = spear.Cam;
        rd = spear.GetComponent<Rigidbody2D>();
        pokeSpeed = spear.PokeSpeed;
        pokeDis = spear.PokeDistance;
        _faceIndex = -1;
    }
    public override void UpdateState(SpearStateManager spear)
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        spear.ReachDistance += (pokeDis - spear.ReachDistance) * pokeSpeed;

        if (!Input.GetButton("Fire1") || spear.Player.IsCrouched)
        {
            spear.SwitchState(spear.normalState);
        }

    }
    public override void FixedUpdateState(SpearStateManager spear)
    {
        var lookDir = mousePos - spear.SpearPosition;
        var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rd.rotation = angle;

        Collider2D[] cols = new Collider2D[102];
        var colNum = rd.OverlapCollider(new ContactFilter2D().NoFilter(), cols);
        if (colNum > 0)
        {
            for (var i = 0f; i <= 1f; i += 0.01f)
            {
                var _testPoint = spear.SpearPosition + (spear.SpearHead - spear.SpearPosition) * i;
                var _flag = false;
                for (var j = 0; j < colNum; j++)
                {
                    var collision = cols[j];
                    if (collision.OverlapPoint(_testPoint))
                    {
                        _flag = ComparePoint(spear, collision, _testPoint);
                        if (_flag) break;
                    }
                }
                if (_flag) break;
            }
        }
    }
    public override void LateUpdateState(SpearStateManager spear)
    {
        spear.transform.position = new(spear.SpearPosition.x + spear.ReachDistance * Mathf.Cos((rd.rotation + 90) * Mathf.Deg2Rad), spear.SpearPosition.y + spear.ReachDistance * Mathf.Sin((rd.rotation + 90) * Mathf.Deg2Rad));
    }
    bool ComparePoint(SpearStateManager spear, Collider2D collision, Vector2 anchorPoint)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (collision.gameObject.CompareTag("Block"))
            {
                for (var j = 0; j < 4; j++)
                {
                    var _c = collision.gameObject.GetComponent<GroundCollisions>().Colliers[j];
                    if (_c.OverlapPoint(anchorPoint))
                    {
                        spear.AnchorPoint = anchorPoint;
                        _faceIndex = j;

                        collision.gameObject.GetComponent<GroundCollisions>().Anchored = true;
                        spear.AnchorBlock = collision.gameObject;
                        spear.normalState.AutoPoke = false;
                        spear.SwitchState(spear.anchorState);
                        return true;
                    }
                }
                return false;
            }
            else if (collision.gameObject.CompareTag("BouncyBlock"))
            {
                for (var j = 0; j < 4; j++)
                {
                    var _c = collision.gameObject.GetComponent<GroundCollisions>().Colliers[j];
                    if (!_c.enabled) continue;
                    if (_c.OverlapPoint(anchorPoint))
                    {
                        _faceIndex = j;

                        var _playerRd = spear.Player.GetComponent<Rigidbody2D>();

                        var _dir = _playerRd.position - anchorPoint;
                        var num2 = Mathf.Sqrt(_dir.sqrMagnitude);
                        _dir = new Vector2(_dir.x / num2, Mathf.Max(0f, _dir.y / num2));

                        if (_faceIndex == 0 || _faceIndex == 2)// Left or Right face
                        {
                            spear.Player.AllowMoveTimer = 10;
                            _playerRd.velocity = new(25f * Mathf.Sign(_dir.x), _dir.y * 15f);
                        }
                        else if (_faceIndex == 3)// Up face
                        {
                            _playerRd.velocity = new(_playerRd.velocity.x, 15f);
                        }
                        else// Down face
                        {
                            _playerRd.velocity *= Vector2.right;
                            _dir.x = 0f;
                        }
                        spear.Player.BoucedFace = _faceIndex;


                        spear.Player.Bounced = true;
                        spear.ReadyToPokeTimer = spear.ReadyToPokeTime;
                        spear.normalState.AutoPoke = true;
                        spear.SwitchState(spear.normalState);
                        return true;
                    }
                }
                return false;
            }
            else if (collision.gameObject.CompareTag("BreakableBlock"))
            {
                collision.gameObject.GetComponent<BreakableBlock>().DamageBlock();
                spear.normalState.AutoPoke = false;
                spear.SwitchState(spear.normalState);
                return true;
            }
            else if (collision.gameObject.CompareTag("Switch"))
            {
                collision.gameObject.GetComponent<SwitchObj>().Switch();
                spear.normalState.AutoPoke = false;
                spear.SwitchState(spear.normalState);
                return true;
            }
        }else if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.GetComponent<ParEnemy>().Hurt(HPCounter.instance.Attack, anchorPoint);
                spear.ReadyToPokeTimer = spear.ReadyToPokeTime;

                var _playerRd = spear.Player.GetComponent<Rigidbody2D>();
                _playerRd.velocity *= Vector2.right;
                _playerRd.AddForce(new(500f * (_playerRd.position - (Vector2)collision.transform.position).normalized.x, 500f));

                spear.SwitchState(spear.normalState);
                return true;
            }
        }
        return false;
    }

    public override void OnCollisionStay2DState(SpearStateManager spear, Collision2D collision)
    {
        /*if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (collision.gameObject.CompareTag("Block"))
            {
                bool _flag = false;
                Collider2D _face;
                // Calculate the anchor point

                *//*for (var j = 0; j < 4; j++)
                {
                    var _c = collision.gameObject.GetComponent<GroundCollisions>().Colliers[j];
                    if (rd.Distance(_c).distance < minDis)
                    {
                        _flag = true;
                        _face = _c;
                        minDis = rd.Distance(_c).distance;
                        spear.AnchorPoint = _c.ClosestPoint(spear.SpearPosition);
                        _faceIndex = j;
                    }
                }*//*
                for (var i = 0f; i <= 1f; i += 0.005f)
                {
                    var _testPoint = spear.SpearPosition + (spear.SpearHead - spear.SpearPosition) * i;
                    if (collision.collider.bounds.Contains(new(_testPoint.x, _testPoint.y)))
                    {
                        for (var j = 0; j < 4; j++)
                        {
                            var _c = collision.gameObject.GetComponent<GroundCollisions>().Colliers[j];
                            if (_c.bounds.Contains(new(_testPoint.x, _testPoint.y)))
                            {
                                if (spear.AnchorBlock == null || Vector2.Distance(_testPoint, spear.SpearPosition) < Vector2.Distance(spear.AnchorPoint, spear.SpearPosition))
                                {
                                    _flag = true;
                                    _face = _c;
                                    spear.AnchorPoint = _testPoint;
                                    _faceIndex = j;
                                    break;
                                }
                            }
                        }
                        if (_flag) break;
                    }
                }

                // Switch to the anchor state
                if (_flag)
                {
                    collision.gameObject.GetComponent<GroundCollisions>().Anchored = true;
                    spear.AnchorBlock = collision.gameObject;
                    spear.SwitchState(spear.anchorState);
                }
            }
            else if (collision.gameObject.CompareTag("BouncyBlock"))
            {
                bool _flag = false;
                Collider2D _face = new();
                int _faceIndex = -1;
                Vector2 _anchorPoint = new();
                // Calculate the anchor point
                for (var i = 0f; i < 1f; i += 0.01f)
                {
                    var _testPoint = spear.SpearPosition + (spear.SpearHead - spear.SpearPosition) * i;
                    if (collision.collider.bounds.Contains(new(_testPoint.x, _testPoint.y)))
                    {
                        for (var j = 0; j < 4; j++)
                        {
                            var _c = collision.gameObject.GetComponent<GroundCollisions>().Colliers[j];
                            if (!_c.enabled) continue;
                            if (_c.bounds.Contains(new(_testPoint.x, _testPoint.y)))
                            {
                                _flag = true;
                                _face = _c;
                                _anchorPoint = _testPoint;
                                _faceIndex = j;
                                break;
                            }
                        }
                        if (_flag) break;
                    }
                }
                // Add a force to the player
                if (_flag)
                {
                    var _playerRd = spear.Player.GetComponent<Rigidbody2D>();

                    var _dir = _playerRd.position - _anchorPoint;
                    var num2 = Mathf.Sqrt(_dir.sqrMagnitude);
                    _dir = new Vector2(_dir.x / num2, Mathf.Max(0f, _dir.y / num2));

                    if (_faceIndex == 0 || _faceIndex == 2)// Left or Right face
                    {

                        *//*spear.Player.AllowMoveTimer = 10;
                        _playerRd.velocity *= Vector2.right;
                        _dir.x = Mathf.Sign(_dir.x);
                        _playerRd.AddForce(_dir * 400f);*//*

                        spear.Player.AllowMoveTimer = 10;
                        _playerRd.velocity = new(25f * Mathf.Sign(_dir.x), _dir.y * 15f);
                    }
                    else if (_faceIndex == 3)// Up face
                    {

                        *//*_playerRd.velocity *= Vector2.right;
                        _dir = Vector2.up;
                        _playerRd.AddForce(_dir * 600f);*//*

                        _playerRd.velocity = new(_playerRd.velocity.x, 15f);
                    }
                    else// Down face
                    {
                        _playerRd.velocity *= Vector2.right;
                        _dir.x = 0f;
                    }
                    spear.Player.BoucedFace = _faceIndex;


                    spear.Player.Bounced = true;
                    spear.SwitchState(spear.normalState);
                }
            }
            else if (collision.gameObject.CompareTag("BreakableBlock"))
            {
                collision.gameObject.GetComponent<BreakableBlock>().DamageBlock();
                spear.SwitchState(spear.normalState);
            }
            else if (collision.gameObject.CompareTag("Switch"))
            {
                collision.gameObject.GetComponent<DoorSwitch>().Switch();
                spear.SwitchState(spear.normalState);
            }
        }*/
    }
}

public class SpearAnchorState : SpearBaseState
{
    Rigidbody2D rd;
    float pokeSpeed;
    float maxSpinSpeed;
    float spinSpeed;
    float m_Velocity;
    int _faceIndex; 
    public override void EnterState(SpearStateManager spear)
    {
        rd = spear.GetComponent<Rigidbody2D>();
        pokeSpeed =spear.PokeSpeed;
        spear.Anchored = true;
        maxSpinSpeed = spear.SpinSpeed;
        spinSpeed = 0f;
        m_Velocity = 0f;
        _faceIndex = spear.pokeState.FaceIndex;
    }
    public override void UpdateState(SpearStateManager spear)
    {
        if (!Input.GetButton("Fire1"))
        {
            spear.Anchored = false;
            spear.AnchorBlock.GetComponent<GroundCollisions>().Anchored = false;
            
            spear.SwitchState(spear.normalState);
            
        }
        else if (!spear.Anchored)
        {
            spear.AnchorBlock.GetComponent<GroundCollisions>().Anchored = false;
            spear.SwitchState(spear.normalState);
            
        }
    }
    public override void FixedUpdateState(SpearStateManager spear)
    {
        // Rotate the spear
        var _tarSpeed = 0f;
        var _tarClamp = 0f;
        if(_faceIndex == 0 || _faceIndex == 2)// Right and Left
        {
            _tarSpeed = Input.GetAxisRaw("Vertical");
            if (_faceIndex == 2) _tarSpeed *= -1;
        }
        else if(_faceIndex == 1 || _faceIndex == 3) // Down and Up
        {
            _tarSpeed = Input.GetAxisRaw("Horizontal");
            if (_faceIndex == 3) _tarSpeed *= -1;
        }

        if (_faceIndex == 0) _tarClamp = 90f;
        else if (_faceIndex == 1) _tarClamp = 0f;
        else if (_faceIndex == 2) _tarClamp = -90f;
        else if (_faceIndex == 3) _tarClamp = 180f;

        spinSpeed = Mathf.SmoothDamp(spinSpeed, _tarSpeed * maxSpinSpeed, ref m_Velocity, 0.1f);
        rd.rotation += spinSpeed;
        //rd.rotation = Mathf.Clamp(rd.rotation, -75f + _tarClamp, 75f + _tarClamp);
        // Set the position to fit into Anchor point
        var _disHeadToSpear = Vector2.Distance(rd.position, spear.SpearHead);
        var _targetPoint = new Vector2(spear.AnchorPoint.x + _disHeadToSpear * Mathf.Cos((rd.rotation - 90) * Mathf.Deg2Rad), spear.AnchorPoint.y + _disHeadToSpear * Mathf.Sin((rd.rotation - 90) * Mathf.Deg2Rad));
        rd.position = _targetPoint;
    }
    public override void LateUpdateState(SpearStateManager spear)
    {
        
    }
    public override void OnCollisionStay2DState(SpearStateManager spear, Collision2D collision)
    {

    }
}
public class SpearStiffState : SpearBaseState
{
    Rigidbody2D rd;
    public override void EnterState(SpearStateManager spear)
    {
        spear.AnchorBlock = null;
        rd = spear.GetComponent<Rigidbody2D>();
    }
    public override void UpdateState(SpearStateManager spear)
    {
        
    }
    public override void FixedUpdateState(SpearStateManager spear)
    {

    }
    public override void OnCollisionStay2DState(SpearStateManager spear, Collision2D collision)
    {

    }
    public override void LateUpdateState(SpearStateManager spear)
    {
        // Follow the player 
        spear.transform.position = new Vector3(spear.SpearPosition.x + spear.ReachDistance * Mathf.Cos((rd.rotation + 90) * Mathf.Deg2Rad), spear.SpearPosition.y + spear.ReachDistance * Mathf.Sin((rd.rotation + 90) * Mathf.Deg2Rad), spear.transform.position.z);
    }
}