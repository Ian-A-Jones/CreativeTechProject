using UnityEngine;
using System.Collections;

public class PlayerCam : MonoBehaviour {

	public GameObject orbit;

	RaycastHit hit;

	bool leftDown, rightDown, forwardDown, backDown;
	
	float x;
	
	float moveForce = 500;

	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Find vector from camera to plane
		Vector3 deltaPosition = orbit.transform.position - transform.position;
		//Debug.Log ("deltaPosition: " + deltaPosition.ToString());
		
		Vector3 direction = Vector3.Normalize(deltaPosition);
		//Debug.Log("direction: " + direction.ToString());

		if(Physics.Raycast(this.transform.position, direction, out hit))
		{
			//Debug.Log("Collided");
			Debug.DrawLine (this.transform.position, hit.point, Color.cyan);
			transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
		}

		if(Input.GetKey(KeyCode.W))
		{
			forwardDown = true;
		}
	
		if (Input.GetKey(KeyCode.A))
		{
			leftDown = true;
		}

		if(Input.GetKey(KeyCode.S))
		{
			backDown = true;
		} 

		if (Input.GetKey(KeyCode.D))
		{
			rightDown = true;
		}
		

//		x += Input.GetAxis("Mouse X");
//		transform.Rotate(0, x, 0, Space.Self);
	}


	// Update is called once per frame
	void FixedUpdate () 
	{
		if(leftDown)
		{
			rigidbody.AddRelativeForce(transform.forward * moveForce * Time.deltaTime);
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

		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 10);

		
		//Debug.Log (rigidbody.velocity);
		
	}
}
