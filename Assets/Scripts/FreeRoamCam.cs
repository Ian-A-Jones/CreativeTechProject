//Camear controller script for Free roam and targetting
using UnityEngine;
using System.Collections;

public class FreeRoamCam : MonoBehaviour 
{
	public GameObject target;
	public float zoomSpeed;

	float x, y;
	public float zoom = 0;

	float moveForce = 1;

	Quaternion orbitRotation;
	
	void OnGUI()
	{
		if(target != null)
		{
			GUI.BeginGroup(new Rect(0,Screen.height/2 - 45,160,90));

			GUI.Box(new Rect(0,0,160,90), "Target Info");
			GUI.Label(new Rect(5,15,150,90), "Name: " + target.name);
			GUI.Label(new Rect(5,30,150,90), "Size: " + target.transform.localScale.x.ToString("0.00"));
			GUI.Label(new Rect(5,45,150,90), "Mass: " + target.rigidbody.mass.ToString("0.00"));
			GUI.Label(new Rect(5,60,150,90), "Velocity: " + target.rigidbody.velocity.magnitude.ToString("0.00"));

			GUI.EndGroup();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0) && !SpaceManager.choosingOrbitTarget)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit))
			{	
				if(target != GameObject.Find(hit.transform.name))
				{
					target = GameObject.Find(hit.transform.name);
					zoom = target.transform.localScale.x*3;
				}
			}
			else
			{
				target = null;
			}
		}

		if(Input.GetMouseButton(1))
		{
			
			x += Input.GetAxis("Mouse X");
			y -= Input.GetAxis("Mouse Y");
			
			Quaternion rotation = Quaternion.Euler(y, x, 0);
			
			transform.rotation = rotation;
		}

		if(Input.GetMouseButton(2))
		{
			x -= Input.GetAxis("Mouse X") ;
			y -= Input.GetAxis("Mouse Y");

			transform.position = new Vector3(x,y,0);
		}

		if(Input.GetKey(KeyCode.LeftShift))
		{
			moveForce = 10;
		}

		if(Input.GetKeyUp(KeyCode.LeftShift))
		{
			moveForce = 1;
		}

		if(Input.GetKey(KeyCode.LeftControl))
		{
			moveForce = 0.25f;
		}
		
		if(Input.GetKeyUp(KeyCode.LeftControl))
		{
			moveForce = 1;
		}

		if(Input.GetKey(KeyCode.W))
		{
			transform.position +=(transform.forward * moveForce);
		}

		if(Input.GetKey(KeyCode.S))
		{
			transform.position +=(transform.forward * -moveForce);
		}

		if(Input.GetKey(KeyCode.A))
		{
			transform.position +=(transform.right * -moveForce);
		}

		if(Input.GetKey(KeyCode.D))
		{
			transform.position +=(transform.right * moveForce);
		}

		if(Input.GetKey(KeyCode.Q))
		{
			transform.position +=(transform.up * moveForce);
		}

		if(Input.GetKey(KeyCode.E))
		{
			transform.position +=(transform.up * -moveForce);
		}

		if(Input.GetAxis("Mouse ScrollWheel") != 0)
		{
//			Debug.Log ("Zooming");
//			Debug.Log (zoom);
			zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

			if(target)
			{
				zoomSpeed = Mathf.Sqrt(Vector3.Distance(transform.position, target.transform.position));
				zoom = Mathf.Clamp(zoom, target.transform.localScale.x *1.2f, zoom);
			}
//			Debug.Log (zoom);
//			transform.position +=(transform.forward * zoom);
		}

		if(Input.GetKey(KeyCode.LeftAlt))
		{
			x -= Input.GetAxis("Mouse X");
			y -= Input.GetAxis("Mouse Y");
			
			orbitRotation = Quaternion.Euler(y, x, 0);
			
			transform.rotation = orbitRotation;
			
		}

		if(target)
		{
			transform.position = transform.rotation * new Vector3(0.0f, 0.0f, -zoom) + target.transform.position;
		}
		else
		{
			transform.position = new Vector3(transform.position.x,transform.position.y, transform.position.z);
		}

	}
}
