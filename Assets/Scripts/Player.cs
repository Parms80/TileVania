using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
	// Config
	[SerializeField] private float runSpeed = 1.0f;
	[SerializeField] private float jumpSpeed = 5.0f;
	[SerializeField] private float climbSpeed = 4.0f;

	// State
	bool isAlive = true;

	// Cached component references
	private Rigidbody2D myRigidBody;
	private Animator myAnimator;
	private float startingGravityScale;

    void Start()
    {
		myRigidBody = GetComponent<Rigidbody2D> ();
		myAnimator = GetComponent<Animator> ();
		startingGravityScale = myRigidBody.gravityScale;
    }

    void Update()
    {
		Run ();
		Jump ();
		FlipSprite ();
		ClimbLadder ();
    }

	private void Run()
	{
		float controlThrow = CrossPlatformInputManager.GetAxis ("Horizontal");	// Value is between -1 and +1
		Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
		myRigidBody.velocity = playerVelocity;
		myAnimator.SetBool ("Running", PlayerHasHorizontalSpeed());
	}

	private void Jump()
	{
		if (CrossPlatformInputManager.GetButtonDown ("Jump") && IsOnGround())			
		{
			Vector2 jumpVelocityToAdd = new Vector2 (0f, jumpSpeed);
			myRigidBody.velocity += jumpVelocityToAdd;
		}
	}

	private bool IsOnGround() {
		Collider2D coll = GetComponent<Collider2D> ();
		return coll.IsTouchingLayers(LayerMask.GetMask ("Ground"));
	}

	private void FlipSprite()
	{
		if (PlayerHasHorizontalSpeed()) 
		{
			float localScale = Mathf.Sign (myRigidBody.velocity.x);
			transform.localScale = new Vector2 (localScale, 1f);
		}
	}

	private bool PlayerHasHorizontalSpeed()
	{
		return Mathf.Abs (myRigidBody.velocity.x) > Mathf.Epsilon;
	}

	private void ClimbLadder()
	{
		if (IsTouchingLadder ()) {
			float controlThrow = CrossPlatformInputManager.GetAxis ("Vertical");	// Value is between -1 and +1
			Vector2 climbVelocity = new Vector2 (myRigidBody.velocity.x, controlThrow * climbSpeed);
			myRigidBody.velocity = climbVelocity;
			myAnimator.SetBool ("Climbing", PlayerHasVerticalSpeed ());
			myRigidBody.gravityScale = 0f;
		} else {
			myAnimator.SetBool ("Climbing", false);
			myRigidBody.gravityScale = startingGravityScale;
		}
	}

	private bool IsTouchingLadder()
	{
		Collider2D coll = GetComponent<Collider2D> ();
		return coll.IsTouchingLayers(LayerMask.GetMask ("Ladder"));		
	}

	private bool PlayerHasVerticalSpeed()
	{
		return Mathf.Abs (myRigidBody.velocity.y) > Mathf.Epsilon;
	}
}
