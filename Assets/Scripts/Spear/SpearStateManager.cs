using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearStateManager : MonoBehaviour
{
    public Camera cam;

    SpearBaseState currentState;
    public SpearNormalState normalState = new();
    public SpearAnchorState anchorState = new();
    public SpearPokeState pokeState = new();

    // Start is called before the first frame update
    void Start()
    {
        currentState = normalState;

        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    void FixedUpdate()
    {
        currentState.FixedUpdateState(this);
    }

    void OnCollisionEnter(Collision collision)
    {
        currentState.OnCollisionEnter(this, collision);
    }

    public void SwitchState(SpearBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
}
