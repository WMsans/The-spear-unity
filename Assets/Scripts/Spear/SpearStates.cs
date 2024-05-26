using UnityEngine;

public abstract class SpearBaseState
{
    public abstract void EnterState(SpearStateManager spear);
    public abstract void UpdateState(SpearStateManager spear);
    public abstract void FixedUpdateState(SpearStateManager spear);
    public abstract void OnCollisionEnter(SpearStateManager spear, Collision collision);
}

public class SpearNormalState : SpearBaseState
{
    Camera cam;
    Transform tr;

    Vector2 mousePos;
    public override void EnterState(SpearStateManager spear)
    {
        cam = spear.cam;
        tr = spear.transform;
    }
    public override void UpdateState(SpearStateManager spear)
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }
    public override void FixedUpdateState(SpearStateManager spear)
    {
        var lookDir = mousePos - new Vector2(tr.position.x, tr.position.y);
        var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        tr.eulerAngles = new Vector3(0f, 0f, angle);

    }
    public override void OnCollisionEnter(SpearStateManager spear, Collision collision)
    {

    }
}

public class SpearPokeState : SpearBaseState
{
    Camera cam;
    Transform tr;

    Vector2 mousePos;
    public override void EnterState(SpearStateManager spear)
    {
        cam = spear.cam;
        tr = spear.transform;
    }
    public override void UpdateState(SpearStateManager spear)
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }
    public override void FixedUpdateState(SpearStateManager spear)
    {
        var lookDir = mousePos - new Vector2(tr.position.x, tr.position.y);
        var angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        tr.eulerAngles = new Vector3(0f, 0f, angle);

    }
    public override void OnCollisionEnter(SpearStateManager spear, Collision collision)
    {

    }
}

public class SpearAnchorState : SpearBaseState
{
    public override void EnterState(SpearStateManager spear)
    {

    }
    public override void UpdateState(SpearStateManager spear)
    {

    }
    public override void FixedUpdateState(SpearStateManager spear)
    {
        
    }
    public override void OnCollisionEnter(SpearStateManager spear, Collision collision)
    {

    }
}