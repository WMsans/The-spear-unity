using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController2D controller;

    float horizontalMove = 0f;

    float maxHorizontalSpeed = 40f;
    bool jumpKeyDown = false;
    bool jumpKey = false;
    bool crouchKey = false; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * maxHorizontalSpeed;


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
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouchKey, jumpKeyDown, jumpKey);
        jumpKeyDown = false;
    }
}
