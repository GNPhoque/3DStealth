using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	//SELF COMPONENTS
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    Transform t;
	[SerializeField]
	PlayerMovementStateMachine moveStateMachine;
	[SerializeField]
	Animator animator;
	[SerializeField]
	CapsuleCollider capsule;

	//OTHER REFERENCES
	[SerializeField]
    Inputs inputs;

	//INSPECTOR VARIABLES
	[SerializeField][Range(0f,60f)]
	float slopeLimit;
	[SerializeField]
	float jumpForce;
	[SerializeField]
	float rotationSpeed;
	[SerializeField]
	float walkSpeed;
	[SerializeField]
	float sprintSpeed;
	[SerializeField]
	float sneakSpeed;
	[SerializeField]
	float rayLength;
	[SerializeField]
	LayerMask groundLayer;

	//PRIVATE FIELDS
	[SerializeField]
	bool _isGrounded;
	[SerializeField]
	bool _isJumping;
	float currentSpeed;

	//PUBLIC PROPERTIES
	public Vector3 velocity
	{
		get { return rb.velocity; }
	}

	public bool isGrounded
	{
		get { return _isGrounded; }
		set { _isGrounded = value; }
	}


	private void Start()
	{
		currentSpeed = walkSpeed;
	}

	void Update()
	{
		switch (moveStateMachine.currentState)
		{
			case MovementState.IDLE:
				currentSpeed = walkSpeed;
				break;
			case MovementState.WALK:
				currentSpeed = walkSpeed;
				break;
			case MovementState.SPRINT:
				currentSpeed = sprintSpeed;
				break;
			case MovementState.SNEAK:
				currentSpeed = sneakSpeed;
				break;
			default:
				break;
		}
		//if (moveStateMachine.currentState == MovementState.SPRINT)
		//{
		//	currentSpeed = sprintSpeed;
		//}
		//if (inputs.sneak)
		//{
		//	t.localScale = new Vector3(1f, .75f, 1f);
		//	currentSpeed = sneakSpeed;
		//}
		//if (!inputs.sneak)
		//{
		//	transform.localScale = Vector3.one;
		//	if (!inputs.sprint)
		//	{
		//		currentSpeed = walkSpeed; 
		//	}
		//}
		animator.SetFloat("PlayerDirectionX", inputs.movement.x);
		animator.SetFloat("PlayerDirectionY", inputs.movement.y);
	}

	void FixedUpdate()
	{
		Vector3 moveX = t.right * inputs.movement.x * currentSpeed;
		Vector3 moveZ = t.forward * inputs.movement.y * currentSpeed;
		Vector3 move = moveX + moveZ;
		rb.velocity = new Vector3(move.x, (_isJumping || rb.velocity.y < 0f) ? rb.velocity.y : 0f, move.z);
		if (inputs.movement != Vector2.zero)
		{
			Vector3 cameraForward = Camera.main.transform.forward;
			cameraForward = new Vector3(cameraForward.x, 0f, cameraForward.z);
			rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(cameraForward), rotationSpeed));
		}
		CheckGround();
		if (inputs.jump && isGrounded)
		{
			rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
			isGrounded = false;
			_isJumping = true;
		}
	}

	//private void CheckGround()
	//{
	//	isGrounded = false;
	//	isGrounded = Physics.OverlapBox(t.position, new Vector3(.5f, .05f, .5f), Quaternion.identity, groundLayer).Length > 0;
	//}

	void CheckGround()
	{
		isGrounded = false;
		float capsuleHeight = Mathf.Max(capsule.radius * 2f, capsule.height);
		Vector3 capsuleBottom = transform.TransformPoint(capsule.center - Vector3.up * capsuleHeight / 2f);
		float radius = transform.TransformVector(capsule.radius, 0f, 0f).magnitude;

		Ray ray = new Ray(capsuleBottom + transform.up * .01f, -transform.up);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, radius * 5f))
		{
			float normalAngle = Vector3.Angle(hit.normal, transform.up);
			if (normalAngle < slopeLimit)
			{
				float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - radius + .02f;
				if (hit.distance < maxDist)
				{
					isGrounded = true;
					_isJumping = false;
				}
			}
		}
	}
}
