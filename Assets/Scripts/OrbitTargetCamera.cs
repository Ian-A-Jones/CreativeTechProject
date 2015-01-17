using UnityEngine;
using System.Collections;

public class OrbitTargetCamera : MonoBehaviour 
{

	public GameObject target;

	//mouseToOrbit movement
	float x, y;

	public float zoom = -10;
	int zoomSpeed = 10;

	Quaternion rotation;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(this.enabled)
		{

			if(Input.GetKey(KeyCode.LeftAlt))
			{
				x -= Input.GetAxis("Mouse X");
				y -= Input.GetAxis("Mouse Y");
				
				rotation = Quaternion.Euler(y, x, 0);
				
				transform.rotation = rotation;

			}

			if(Input.GetAxis("Mouse ScrollWheel") != 0)
			{
				Vector3 temp = this.transform.position - target.transform.position;

				float temp2 = temp.sqrMagnitude;

				temp2 = Mathf.Clamp(temp2, 0, 100);

//				Debug.Log(temp2);

				zoom += Input.GetAxis("Mouse ScrollWheel") * temp2 * zoomSpeed * Time.deltaTime;
			}

			transform.position = rotation * new Vector3(0.0f, 0.0f, zoom) + target.transform.position;

			if(Input.GetMouseButtonDown(0))
			{
				Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				
				if(Physics.Raycast(ray, out hit))
				{	
					if(target != GameObject.Find(hit.transform.name))
					{
						target = GameObject.Find(hit.transform.name);
					}
				}
			}

		}


	}
}
