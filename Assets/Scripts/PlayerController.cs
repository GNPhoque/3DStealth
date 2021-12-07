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

	//OTHER REFERENCES
	[SerializeField]
	Transform cameraFollow;
    [SerializeField]
    Inputs inputs;

	//INSPECTOR VARIABLES
	[SerializeField]
	float playerWidth;
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
	bool isGrounded;
	float currentSpeed;

	private void Start()
	{
		currentSpeed = walkSpeed;
	}

	void Update()
	{
		if (inputs.sprint)
		{
			currentSpeed = sprintSpeed;
		}
		if (inputs.sneak)
		{
			t.localScale = new Vector3(1f, .75f, 1f);
			currentSpeed = sneakSpeed;
		}
		if (!inputs.sneak)
		{
			transform.localScale = Vector3.one;
			if (!inputs.sprint)
			{
				currentSpeed = walkSpeed; 
			}
		}
		if (inputs.camera != Vector2.zero)
		{
			Vector3 camRotation = cameraFollow.rotation.eulerAngles;
			Vector3 newCamRotation = new Vector3(Mathf.Clamp(camRotation.x + inputs.camera.y, -60f, 60f), camRotation.y + inputs.camera.x, 0f);
			cameraFollow.rotation = Quaternion.Euler(newCamRotation);
			//cameraFollow.rotation *= Quaternion.Euler((Vector3)inputs.camera);
			//cameraFollow.rotation *= Quaternion.AngleAxis(inputs.camera.x, t.up);
			//cameraFollow.rotation *= Quaternion.AngleAxis(inputs.camera.x, Vector3.up);
			//cameraFollow.rotation *= Quaternion.AngleAxis(inputs.camera.y, Vector3.right);
			//cameraFollow.rotation *= Quaternion.Euler((Vector3)inputs.camera);
			//cameraFollow.rotation *= Quaternion.Euler(inputs.camera.x, inputs.camera.y, 0f);
			//cameraFollow.Rotate(cameraFollow.up, inputs.camera.x);
			//cameraFollow.Rotate(cameraFollow.right, inputs.camera.y);
			//cameraFollow.Rotate(new Vector3(inputs.camera.y, inputs.camera.x, 0f));
			//cameraFollow.Rotate(new Vector3(inputs.camera.y, inputs.camera.x, 0f));
		}
		Debug.DrawRay(t.position, Vector3.down * rayLength, Color.red);
	}

	void FixedUpdate()
	{

		rb.velocity = new Vector3(inputs.movement.x * currentSpeed * t.right.x, rb.velocity.y, inputs.movement.y * currentSpeed * t.forward.z);
		if (inputs.movement != Vector2.zero)
		{
			Vector3 cameraForward = Camera.main.transform.forward;
			rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(cameraForward), rotationSpeed));
		}
		if (inputs.camera != Vector2.zero)
		{
			//TODO
		}
		CheckGround();
		if (inputs.jump && isGrounded)
		{
			Debug.Log("Jump");
			rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
			isGrounded = false;
		}
	}

	private void CheckGround()
	{
		isGrounded = false;
		isGrounded = Physics.OverlapBox(t.position, new Vector3(.5f, .05f, .5f), Quaternion.identity, groundLayer).Length > 0;
		//for (float i = -1f; i <= 1f; i++)
		//{
		//	for (float j = -1f; j <= 1f; j++)
		//	{
		//		Debug.DrawRay(t.position + new Vector3(i * playerWidth, 0f, j * playerWidth), Vector3.down * rayLength, Color.red);
		//		isGrounded = Physics.Raycast(t.position + new Vector3(i * playerWidth, 0f, j * playerWidth), Vector3.down, rayLength, groundLayer);
		//		if (isGrounded)
		//		{
		//			return;
		//		}
		//	}
		//}
	}
}
