using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce = 400f;							// Amount of force added when the player jumps.
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A comboBarMask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private float m_CoyoteTime;

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	private bool m_CoyoteJump;
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

    public bool CoyoteJump { get => m_CoyoteJump; set => m_CoyoteJump = value; }

    [System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		//for (int i = 0; i < colliders.Length; i++)
		//{
		//	if (colliders[i].gameObject != gameObject)
		//	{
		//		m_Grounded = true;
		//		//Debug.Log(m_Rigidbody2D.velocity.y);
		//		if (!wasGrounded && m_Rigidbody2D.velocity.y < 0f)
		//			OnLandEvent.Invoke();
		//	}
		//}

		if (colliders.Length > 0) 
		{ 
			
			
			m_Grounded = true;
			//Debug.Log(m_Rigidbody2D.velocity.y);
			if (!wasGrounded && m_Rigidbody2D.velocity.y < 0f)
				OnLandEvent.Invoke();
			
		}
		if (wasGrounded && !m_Grounded)
        {
			StartCoroutine(CoyoteJumpDelay());
			//Debug.Log("Off ground");
        }
	}

	IEnumerator CoyoteJumpDelay()
    {
		CoyoteJump = true;
		yield return new WaitForSeconds(m_CoyoteTime);
		CoyoteJump = false;
    }
    private void Update()
    {
		var maxVelocityY = 100f;
		var velocity = gameObject.GetComponent<Rigidbody2D>().velocity;
		if (velocity.y > maxVelocityY)
			gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x, maxVelocityY);
		//Debug.Log("Velocity: " + velocity);

	}

	public void Move(float move, bool jump)
	{
		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{
			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if ((m_Grounded || m_CoyoteJump) && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_CoyoteJump = false;
			//m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce+(Mathf.Pow(Mathf.Abs(m_Rigidbody2D.velocity.x),2.5f)))); //good idea, work on that
			//m_Rigidbody2D.AddForce(new Vector2(0f+m_Rigidbody2D.velocity.x, m_JumpForce+ m_Rigidbody2D.velocity.y));
		}
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public bool IsGrounded() => m_Grounded;
}
