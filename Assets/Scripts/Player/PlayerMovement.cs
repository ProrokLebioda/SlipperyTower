using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController2D controller;

    public Animator animator;

    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;

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
        jump = false;
    }

    public void OnMove(InputValue value)
    {
        if (controller.IsGrounded())
        {
            // Find a way to trigger only once otherwise it's bad idea
            FindObjectOfType<AudioManager>().Play("Steps");
        }
        horizontalMove = value.Get<Vector2>().x * runSpeed;
    }

    public void OnJump(InputValue value)
    {
        if (controller.IsGrounded())
        {
            FindObjectOfType<AudioManager>().Play("Jump");
        }
        jump = true;
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
