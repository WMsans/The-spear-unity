using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterStateManager manager;

    float horizontalMove = 0f;

    bool jumpKeyDown = false;
    bool jumpKey = false;
    bool crouchKey = false; 

    // Update is called once per frame
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
