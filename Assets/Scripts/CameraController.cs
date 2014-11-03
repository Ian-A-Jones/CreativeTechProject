using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	Vector3 DEFAULTTARGETVECTOR = new Vector3(0,0,10);

	public GameObject target;
	public SpaceManager sManager;
	public int zoomSpeed;

	float x, y;
	float zoom = 10;

	Vector3 targetVector;

	bool choosingSpawn;

	// Use this for initialization
	void Start () 
	{
		targetVector  = DEFAULTTARGETVECTOR;
	}

	void OnGUI()
	{
		if(target != null)
		{
			GUI.Box(new Rect(0,0,160,90), "Target Info");
			GUI.Label(new Rect(5,15,100,90), "Name: " + target.name);
			GUI.Label(new Rect(5,30,100,90), "Size: " + target.transform.localScale.x.ToString("0.00"));
			GUI.Label(new Rect(5,45,100,90), "Mass: " + target.rigidbody.mass.ToString("0.00"));
			GUI.Label(new Rect(5,60,150,90), "Velocity" + target.rigidbody.velocity.ToString());

		}

		if(choosingSpawn)
		{
			GUI.Label(new Rect(Screen.width/2 - 50, 20, 100, 20), "Select spawn");
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit))
			{	
				if(choosingSpawn)
				{
					Vector3 spawn = hit.point;
					spawn +=  hit.normal * 2;

					sManager.addPlayer(spawn);

					choosingSpawn = false;
				}
				else
				{
					if(target != GameObject.Find(hit.transform.name))
					{
						target = GameObject.Find(hit.transform.name);
					}
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
		else
		{

			if(Input.GetKey(KeyCode.W))
			{
				rigidbody.AddForce(Vector3.forward * 2);
			}

			if(Input.GetKey(KeyCode.S))
			{
				rigidbody.AddForce(Vector3.back * 2);
			}

			if(Input.GetKey(KeyCode.A))
			{
				rigidbody.AddForce(Vector3.left * 2);
			}

			if(Input.GetKey(KeyCode.D))
			{
				rigidbody.AddForce(Vector3.right * 2);
			}

		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(choosingSpawn)
			{
				choosingSpawn = false;
			}
			else
			{
				choosingSpawn = true;
			}
		}

		//TODO:Make Orbit
//		if(Input.GetKey(KeyCode.LeftAlt))
//		{
//			if(target == null)
//			{
//				x += Input.GetAxis("Mouse X");
//				y -= Input.GetAxis("Mouse Y");
//
//				Quaternion rotation = Quaternion.Euler(y, x, 0);
//				Vector3 position = rotation * (new Vector3(0,0,10) + targetVector);
//
//				var rotation = Quaternion.Euler(y, x, 0);
//				var position = rotation * Vector3(0.0, 0.0, -distance) + target.position;
//
//				
//				transform.rotation = rotation;
//				transform.position = position;
//			}
//
//		}

		if(Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;

			transform.position = new Vector3(transform.position.x,transform.position.y, zoom);
		}

		if(target != null)
		{
			transform.position = target.transform.position + new Vector3(0, 0, -zoom);
		}
		else
		{
			transform.position = new Vector3(0,0, - zoom);
		}

	}
}
