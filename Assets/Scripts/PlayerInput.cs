using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour 
{
	public GameObject closetPlanet;
	public float closestPlanetMass;

	bool onGround = false;

	bool leftDown, rightDown, forwardDown, backDown;

	float x;

	float moveForce = 0.015f;
	// Use this for initialization
	void Start () 
	{

	}

	void Update()
	{
		RaycastHit hit;
		
		if(closetPlanet != null)
		{
			//Find vector from first body to second body
			Vector3 deltaPosition = closetPlanet.transform.position - transform.position;
			//Debug.Log ("deltaPosition: " + deltaPosition.ToString());
			
			Vector3 direction = Vector3.Normalize(deltaPosition);
			//Debug.Log("direction: " + direction.ToString());
			
			if(Physics.Raycast(this.transform.position, direction, out hit))
			{
				//Debug.Log("Collided");
				Debug.DrawLine (this.transform.position, hit.point, Color.cyan);
				transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
			}

			if (Input.GetKey(KeyCode.A)&& onGround)
			{
				leftDown = true;
			}
			
			if (Input.GetKey(KeyCode.D)&& onGround)
			{
				rightDown = true;
			}
			
			if(Input.GetKey(KeyCode.W) && onGround)
			{
				forwardDown = true;
			}

			if(Input.GetKey(KeyCode.S) && onGround)
			{
				backDown = true;
			} 
		}

		x += Input.GetAxis("Mouse X");
		transform.Rotate(0, x, 0, Space.Self);
	}
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(onGround)
		{
			if(leftDown)
			{
				rigidbody.AddRelativeForce(Vector3.left * moveForce * Time.deltaTime);
				leftDown = false;
			}

			if(rightDown)
			{
				rigidbody.AddRelativeForce(Vector3.right * moveForce * Time.deltaTime);
				rightDown = false;
			}

			if(forwardDown)
			{
				rigidbody.AddRelativeForce(Vector3.forward * moveForce * Time.deltaTime);
				forwardDown = false;
			}

			if(backDown)
			{
				rigidbody.AddRelativeForce(Vector3.back * moveForce * Time.deltaTime);
				backDown = false;
			}
		}

		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 20);

		Debug.Log (rigidbody.velocity);

	}

	void OnCollisionEnter(Collision collision)
	{
		onGround = true;
	}

	void OnCollisionExit(Collision collisionInfo) 
	{
		onGround = false;
	}
	
}
