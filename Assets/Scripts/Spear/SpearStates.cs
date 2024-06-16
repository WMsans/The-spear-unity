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

    public override void EnterState(SpearStateManager spear)
    {
        cam = spear.Cam;
        rd = spear.GetComponent<Rigidbody2D>();

    }
    public override void UpdateState(SpearStateManager spear)
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        spear.ReachDistance = 0f;
        // If left mb pressed, poke
        if (Input.GetButtonDown("Fire1") && spear.ReadyToPokeTimer <= 0f){
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
        spear.transform.position = new Vector3(spear.SpearPosition.x + spear.ReachDistance * Mathf.Cos((rd.rotation + 90) * Mathf.Deg2Rad), spear.SpearPosition.y + spear.ReachDistance * Mathf.Sin((rd.rotation + 90) * Mathf.Deg2Rad), spear.transform.position.z);
    }
    public override void OnCollisionStay2DState(SpearStateManager spear, Collision2D collision)
    {
        
    }
}

public class SpearPokeState : SpearBaseState
{
    Camera cam;
    Rigidbody2D rd;
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
        if (!Input.GetButton("Fire1"))
        {
            spear.SwitchState(spear.normalState);
        }
    }
    public override void FixedUpdateState(SpearStateManager spear)
    {
        var lookDir = mousePos - spear.SpearPosition;
        var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rd.rotation = angle;

    }
    public override void LateUpdateState(SpearStateManager spear)
    {
        spear.transform.position = new Vector3(spear.SpearPosition.x + spear.ReachDistance * Mathf.Cos((rd.rotation + 90) * Mathf.Deg2Rad), spear.SpearPosition.y + spear.ReachDistance * Mathf.Sin((rd.rotation + 90) * Mathf.Deg2Rad), spear.transform.position.z);
    }
    public override void OnCollisionStay2DState(SpearStateManager spear, Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if(collision.gameObject.tag == "Block")
            {
                bool _flag = false;
                Collider2D _face;
                // Calculate the anchor point
                for (var i = 0f; i < 1f; i += 0.01f)
                {
                    var _testPoint = spear.SpearPosition + (spear.SpearHead - spear.SpearPosition) * i;
                    if (collision.collider.bounds.Contains(new(_testPoint.x, _testPoint.y)))
                    {
                        for (var j = 0; j < 4; j++)
                        {
                            var _c = collision.gameObject.GetComponent<GroundCollisions>().Colliers[j];
                            if (_c.bounds.Contains(new(_testPoint.x, _testPoint.y)))
                            {
                                _flag = true;
                                _face = _c;
                                spear.AnchorPoint = _testPoint;
                                _faceIndex = j;
                                break;
                            }
                        }
                        if (_flag) break;
                    }
                }

                // Switch to the anchor state
                if (_flag) spear.SwitchState(spear.anchorState);
            }
            else if(collision.gameObject.tag == "BouncyBlock")
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

                    if(_faceIndex == 0 || _faceIndex == 2)// Left or Right face
                    {
                        /*
                        spear.Player.AllowMoveTimer = 10;
                        _playerRd.velocity *= Vector2.right;
                        _dir.x = Mathf.Sign(_dir.x);
                        _playerRd.AddForce(_dir * 400f);
                        */
                        spear.Player.AllowMoveTimer = 10;
                        _playerRd.velocity = new(25f * Mathf.Sign(_dir.x), _dir.y * 15f);
                    }else if(_faceIndex == 3)// Up face
                    {
                        /*
                        _playerRd.velocity *= Vector2.right;
                        _dir = Vector2.up;
                        _playerRd.AddForce(_dir * 600f);
                        */
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
            else if(collision.gameObject.tag == "BreakableBlock")
            {
                collision.gameObject.GetComponent<Goldblock>().BlockHP--;
            }
        }
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
            spear.SwitchState(spear.normalState);
        }
        else if (!spear.Anchored)
        {
            spear.SwitchState(spear.normalState);
        }
    }
    public override void FixedUpdateState(SpearStateManager spear)
    {
        // Rotate the spear
        var _tarSpeed = 0f;
        if(_faceIndex == 0 || _faceIndex == 2)
        {
            _tarSpeed = Input.GetAxisRaw("Vertical");
            if (_faceIndex == 2) _tarSpeed *= -1;
        }
        else if(_faceIndex == 1 || _faceIndex == 3) 
        {
            _tarSpeed = Input.GetAxisRaw("Horizontal");
            if (_faceIndex == 3) _tarSpeed *= -1;
        }
        spinSpeed = Mathf.SmoothDamp(spinSpeed, _tarSpeed * maxSpinSpeed, ref m_Velocity, 0.1f);
        rd.rotation += spinSpeed;
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