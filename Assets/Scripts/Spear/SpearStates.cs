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
    Camera _cam;
    Rigidbody2D _rd;

    Vector2 _mousePos;

    public bool AutoPoke { get; set; } = false;
    public override void EnterState(SpearStateManager spear)
    {
        _cam = spear.Cam;
        _rd = spear.GetComponent<Rigidbody2D>();

        spear.AnchorBlock = null;
        
    }
    public override void UpdateState(SpearStateManager spear)
    {
        _mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

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

        var lookDir = _mousePos - spear.SpearPosition;
        var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        _rd.rotation = angle;

    }
    public override void LateUpdateState(SpearStateManager spear)
    {
        spear.transform.position = new(spear.SpearPosition.x + spear.ReachDistance * Mathf.Cos((_rd.rotation + 90) * Mathf.Deg2Rad), spear.SpearPosition.y + spear.ReachDistance * Mathf.Sin((_rd.rotation + 90) * Mathf.Deg2Rad));
    }
    public override void OnCollisionStay2DState(SpearStateManager spear, Collision2D collision)
    {
        
    }
}

public class SpearPokeState : SpearBaseState
{
    Camera _cam;
    Rigidbody2D _rd;
    int _whatIsGround;
    float _pokeSpeed;
    float _pokeDis;
    Transform _spearHead;
    int _faceIndex;

    Vector2 _mousePos;

    public int FaceIndex { get { return _faceIndex; } }
    public override void EnterState(SpearStateManager spear)
    {
        _cam = spear.Cam;
        _rd = spear.GetComponent<Rigidbody2D>();
        _pokeSpeed = spear.PokeSpeed;
        _pokeDis = spear.PokeDistance;
        _faceIndex = -1;
    }
    public override void UpdateState(SpearStateManager spear)
    {
        _mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);

        spear.ReachDistance += (_pokeDis - spear.ReachDistance) * _pokeSpeed;

        if (!Input.GetButton("Fire1") || spear.Player.IsCrouched)
        {
            spear.SwitchState(spear.normalState);
        }

    }
    public override void FixedUpdateState(SpearStateManager spear)
    {
        var lookDir = _mousePos - spear.SpearPosition;
        var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        _rd.rotation = angle;

        Collider2D[] cols = new Collider2D[102];
        var colNum = _rd.OverlapCollider(new ContactFilter2D().NoFilter(), cols);
        if (colNum > 0)
        {
            for (var i = 0f; i <= 1f; i += 0.01f)
            {
                var testPoint = spear.SpearPosition + (spear.SpearHead - spear.SpearPosition) * i;
                var flag = false;
                for (var j = 0; j < colNum; j++)
                {
                    var collision = cols[j];
                    if (collision.OverlapPoint(testPoint))
                    {
                        flag = ComparePoint(spear, collision, testPoint);
                        if (flag) break;
                    }
                }
                if (flag) break;
            }
        }
    }
    public override void LateUpdateState(SpearStateManager spear)
    {
        spear.transform.position = new(spear.SpearPosition.x + spear.ReachDistance * Mathf.Cos((_rd.rotation + 90) * Mathf.Deg2Rad), spear.SpearPosition.y + spear.ReachDistance * Mathf.Sin((_rd.rotation + 90) * Mathf.Deg2Rad));
    }
    bool ComparePoint(SpearStateManager spear, Collider2D collision, Vector2 anchorPoint)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (collision.gameObject.CompareTag("Block"))
            {
                for (var j = 0; j < 4; j++)
                {
                    var c = collision.gameObject.GetComponent<GroundCollisions>().Colliers[j];
                    if(!c.isActiveAndEnabled) continue;
                    if (c.OverlapPoint(anchorPoint))
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
                    var c = collision.gameObject.GetComponent<GroundCollisions>().Colliers[j];
                    if (!c.isActiveAndEnabled) continue;
                    if (c.OverlapPoint(anchorPoint))
                    {
                        _faceIndex = j;

                        var playerRd = spear.Player.GetComponent<Rigidbody2D>();

                        var dir = playerRd.position - anchorPoint;
                        var num2 = Mathf.Sqrt(dir.sqrMagnitude);
                        dir = new Vector2(dir.x / num2, Mathf.Max(0f, dir.y / num2));

                        if (_faceIndex == 0 || _faceIndex == 2)// Left or Right face
                        {
                            spear.Player.AllowMoveTimer = 10;
                            playerRd.velocity = new(25f * Mathf.Sign(dir.x), dir.y * 15f);
                        }
                        else if (_faceIndex == 3)// Up face
                        {
                            playerRd.velocity = new(playerRd.velocity.x, 15f);
                        }
                        else// Down face
                        {
                            playerRd.velocity *= Vector2.right;
                            dir.x = 0f;
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
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.GetComponent<ParEnemy>().Hurt(HPCounter.instance.Attack, anchorPoint);
                spear.ReadyToPokeTimer = spear.ReadyToPokeTime;

                var _playerRd = spear.Player.GetComponent<Rigidbody2D>();
                _playerRd.velocity *= Vector2.right;
                _playerRd.velocity = new(10f * (_playerRd.position - (Vector2)collision.transform.position).normalized.x, 10f);

                spear.SwitchState(spear.normalState);
                return true;
            }
            else if (collision.gameObject.CompareTag("Shield"))
            {
                collision.GetComponent<Shield>().Poked(anchorPoint);
                spear.ReadyToPokeTimer = spear.ReadyToPokeTime;

                var _playerRd = spear.Player.GetComponent<Rigidbody2D>();
                _playerRd.velocity *= Vector2.right;
                _playerRd.velocity = new(10f * (_playerRd.position - (Vector2)collision.transform.position).normalized.x, 15f);

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
    Rigidbody2D _rd;
    float _pokeSpeed;
    float _maxSpinSpeed;
    float _spinSpeed;
    float _velocity;
    int _faceIndex; 
    public override void EnterState(SpearStateManager spear)
    {
        _rd = spear.GetComponent<Rigidbody2D>();
        _pokeSpeed =spear.PokeSpeed;
        spear.Anchored = true;
        _maxSpinSpeed = spear.SpinSpeed;
        _spinSpeed = 0f;
        _velocity = 0f;
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
        var tarSpeed = 0f;
        var tarClamp = 0f;
        if(_faceIndex == 0 || _faceIndex == 2)// Right and Left
        {
            tarSpeed = Input.GetAxisRaw("Vertical");
            if (_faceIndex == 2) tarSpeed *= -1;
        }
        else if(_faceIndex == 1 || _faceIndex == 3) // Down and Up
        {
            tarSpeed = Input.GetAxisRaw("Horizontal");
            if (_faceIndex == 3) tarSpeed *= -1;
        }

        if (_faceIndex == 0) tarClamp = 90f;
        else if (_faceIndex == 1) tarClamp = 0f;
        else if (_faceIndex == 2) tarClamp = -90f;
        else if (_faceIndex == 3) tarClamp = 180f;

        _spinSpeed = Mathf.SmoothDamp(_spinSpeed, tarSpeed * _maxSpinSpeed, ref _velocity, 0.1f);
        _rd.rotation += _spinSpeed;
        //rd.rotation = Mathf.Clamp(rd.rotation, -75f + _tarClamp, 75f + _tarClamp);
        // Set the position to fit into Anchor point
        var disHeadToSpear = Vector2.Distance(_rd.position, spear.SpearHead);
        var targetPoint = new Vector2(spear.AnchorPoint.x + disHeadToSpear * Mathf.Cos((_rd.rotation - 90) * Mathf.Deg2Rad), spear.AnchorPoint.y + disHeadToSpear * Mathf.Sin((_rd.rotation - 90) * Mathf.Deg2Rad));
        _rd.position = targetPoint;
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
    Rigidbody2D _rd;
    public override void EnterState(SpearStateManager spear)
    {
        spear.AnchorBlock = null;
        _rd = spear.GetComponent<Rigidbody2D>();
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
        spear.transform.position = new Vector3(spear.SpearPosition.x + spear.ReachDistance * Mathf.Cos((_rd.rotation + 90) * Mathf.Deg2Rad), spear.SpearPosition.y + spear.ReachDistance * Mathf.Sin((_rd.rotation + 90) * Mathf.Deg2Rad), spear.transform.position.z);
    }
}