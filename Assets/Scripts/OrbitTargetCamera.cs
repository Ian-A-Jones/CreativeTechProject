using UnityEngine;
using System.Collections;

public class OrbitTargetCamera : MonoBehaviour 
{

	public GameObject target;

	//mouseToOrbit movement
	float x, y;

	public float zoom = -10;
	int zoomSpeed = 200;

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
				zoom += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
			}

			transform.position = rotation * new Vector3(0.0f, 0.0f, zoom) + target.transform.position;

		}


	}
}
