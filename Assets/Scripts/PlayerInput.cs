using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour 
{
	public GameObject closetPlanet;
	public float closestPlanetMass;

	bool onGround = false;

	float x;
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
		}

		x += Input.GetAxis("Mouse X");
		transform.Rotate(0, x, 0, Space.Self);
		Debug.Log (x);
	}
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(onGround)
		{
			if(Input.GetKey(KeyCode.W))
			{
				rigidbody.AddRelativeForce(Vector3.forward * Time.deltaTime * 0.03f);
				Debug.Log (rigidbody.velocity.x);
			}

			if(Input.GetKey(KeyCode.S))
			{
				rigidbody.AddRelativeForce(Vector3.back * Time.deltaTime * 0.03f);
			}

			if(Input.GetKey(KeyCode.A))
			{
				rigidbody.AddRelativeForce(Vector3.left * Time.deltaTime * 0.03f);
			}

			if(Input.GetKey(KeyCode.D))
			{
				rigidbody.AddRelativeForce(Vector3.right * Time.deltaTime * 0.03f);
			}
		}

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
