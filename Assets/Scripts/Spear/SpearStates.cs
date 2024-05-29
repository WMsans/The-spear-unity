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
        cam = spear.cam;
        rd = spear.rd;
    }
    public override void UpdateState(SpearStateManager spear)
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        spear.reachDistance = 0f;
        // If left mb pressed, poke
        if (Input.GetButtonDown("Fire1")){
            spear.SwitchState(spear.pokeState);
        }
    }
    public override void FixedUpdateState(SpearStateManager spear)
    {
        var lookDir = mousePos - spear.spearPosition;
        var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rd.rotation = angle;

    }
    public override void LateUpdateState(SpearStateManager spear)
    {
        spear.transform.position = new Vector3(spear.spearPosition.x + spear.reachDistance * Mathf.Cos((rd.rotation + 90) * Mathf.Deg2Rad), spear.spearPosition.y + spear.reachDistance * Mathf.Sin((rd.rotation + 90) * Mathf.Deg2Rad), spear.transform.position.z);
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

    Vector2 mousePos;
    public override void EnterState(SpearStateManager spear)
    {
        cam = spear.cam;
        rd = spear.rd;
        pokeSpeed = spear.spearPokeSpeed;
        pokeDis = spear.spearPokeDistance;

    }
    public override void UpdateState(SpearStateManager spear)
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        spear.reachDistance += (pokeDis - spear.reachDistance) * pokeSpeed; 
        if (!Input.GetButton("Fire1"))
        {
            spear.SwitchState(spear.normalState);
        }
    }
    public override void FixedUpdateState(SpearStateManager spear)
    {
        var lookDir = mousePos - spear.spearPosition;
        var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rd.rotation = angle;

    }
    public override void LateUpdateState(SpearStateManager spear)
    {
        spear.transform.position = new Vector3(spear.spearPosition.x + spear.reachDistance * Mathf.Cos((rd.rotation + 90) * Mathf.Deg2Rad), spear.spearPosition.y + spear.reachDistance * Mathf.Sin((rd.rotation + 90) * Mathf.Deg2Rad), spear.transform.position.z);
    }
    public override void OnCollisionStay2DState(SpearStateManager spear, Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            bool _flag = false;
            // Calculate the anchor point
            for (float i = 0f; i < 1f; i += 0.01f)
            {
                var _testPoint = spear.spearPosition + (spear.spearHead - spear.spearPosition) * i;
                if (collision.collider.bounds.Contains(new(_testPoint.x , _testPoint.y))) 
                {
                    _flag = true;
                    spear.anchorPoint = _testPoint;
                    break;
                }
            }

            // Switch to the anchor state
            if(_flag) spear.SwitchState(spear.anchorState); 
        }
    }
}

public class SpearAnchorState : SpearBaseState
{
    Rigidbody2D rd;
    float pokeSpeed;
    public override void EnterState(SpearStateManager spear)
    {
        rd = spear.rd;
        pokeSpeed=spear.spearPokeSpeed;

    }
    public override void UpdateState(SpearStateManager spear)
    {
        if (!Input.GetButton("Fire1"))
        {
            spear.SwitchState(spear.normalState);
        }
    }
    public override void FixedUpdateState(SpearStateManager spear)
    {
        // Set the position to fit into Anchor point
        var _targetPoint = rd.position + spear.anchorPoint - spear.spearHead;
        rd.position = _targetPoint;
    }
    public override void LateUpdateState(SpearStateManager spear)
    {
        
    }
    public override void OnCollisionStay2DState(SpearStateManager spear, Collision2D collision)
    {

    }
}