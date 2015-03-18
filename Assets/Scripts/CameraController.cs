using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	Vector3 DEFAULTTARGETVECTOR = new Vector3(0,0,10);

	public GameObject target;
	public SpaceManager sManager;
	public int zoomSpeed;

	float x, y;
	public float zoom = 0;

	float moveForce = 1;
	Vector3 targetVector;

	bool choosingSpawn;
	bool playerActive;

	Quaternion orbitRotation;

	// Use this for initialization
	void Start () 
	{
		targetVector  = DEFAULTTARGETVECTOR;
	}

	void OnGUI()
	{
		if(target != null)
		{
			GUI.BeginGroup(new Rect(0,Screen.height/2 - 45,160,90));

			GUI.Box(new Rect(0,0,160,90), "Target Info");
			GUI.Label(new Rect(5,15,100,90), "Name: " + target.name);
			GUI.Label(new Rect(5,30,100,90), "Size: " + target.transform.localScale.x.ToString("0.00"));
			GUI.Label(new Rect(5,45,100,90), "Mass: " + target.rigidbody.mass.ToString("0.00"));
			GUI.Label(new Rect(5,60,150,90), "Velocity" + target.rigidbody.velocity.ToString());

			GUI.EndGroup();
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
					spawn +=  hit.normal * 1.5f;

					sManager.addPlayerSim(spawn);
//					sManager.addPlayerCam(GameObject.Find(hit.transform.name), spawn);

					choosingSpawn = false;
					playerActive = true;
				}
				else
				{
					if(target != GameObject.Find(hit.transform.name))
					{
						target = GameObject.Find(hit.transform.name);
						zoom = target.transform.localScale.x*3;
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
//			Debug.Log (zoom);
//			transform.position +=(transform.forward * zoom);
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			if(!playerActive)
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
			else
			{
				sManager.removePlayer();
				playerActive = false;
			}
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
