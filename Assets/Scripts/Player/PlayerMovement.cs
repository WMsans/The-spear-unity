using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(CharacterStateManager))]
public class PlayerMovement : MonoBehaviour
{
    CharacterStateManager manager;

    float horizontalMove = 0f;

    bool jumpKeyDown = false;
    bool jumpKey = false;
    bool crouchKey = false;
    void Awake()
    {
        manager = GetComponent<CharacterStateManager>();
        if(manager == null)
        {
            Debug.LogError("Player doesn't have a manager");
        }
    }
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");


        if (Input.GetButtonDown("Jump"))
        {
            jumpKeyDown = true;
            jumpKey = true;
        }else if (Input.GetButtonUp("Jump"))
        {
            jumpKey = false;
        }
        if (Input.GetButtonDown("Crouch"))
        {
            crouchKey = true;
        }else if (Input.GetButtonUp("Crouch"))
        {
            crouchKey = false;
        }

    }

    private void FixedUpdate()
    {
        manager.GetKeys(jumpKey, jumpKeyDown, horizontalMove, crouchKey);
        jumpKeyDown = false;
    }
}
