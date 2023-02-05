using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class PlayerMovement : MonoBehaviour
{

    private BouncyTowerPC _playerInputActions;

    private CharacterController2D controller;

    public Animator animator;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;

    private void Awake()
    {
        _playerInputActions = new BouncyTowerPC();

        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Jump.performed += Jump;
        _playerInputActions.Player.Move.started += Move;
        _playerInputActions.Player.Move.performed += Move;
        _playerInputActions.Player.Move.canceled += Move;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (controller.IsGrounded())
        {
            FindObjectOfType<AudioManager>().Play("Jump");
            jump = true;
        }
        else if (controller.CoyoteJump)
        {
            jump = true;
        }
    }

    private void Move(InputAction.CallbackContext context)
    {
        if (controller.IsGrounded())
        {
            // Find a way to trigger only once otherwise it's bad idea
            FindObjectOfType<AudioManager>().Play("Steps");
        }

        horizontalMove = context.ReadValue<Vector2>().x * runSpeed;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController2D>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        animator.SetBool("IsJumping", jump);
        
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
        Debug.Log("Landed");
        //jump = false;
    }

    public void OnMove(InputValue value)
    {
        
    }

    public void OnJump(InputValue value)
    {
        
        
    }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove*Time.fixedDeltaTime, jump);
        jump = false;
    }

    public void OnCancel(InputValue value)
    {
        Debug.Log("Esc");
        GameManager.Instance.PauseMenu();
    }
}
