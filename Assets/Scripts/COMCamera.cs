//Camera that orbits Centre of Mass
using UnityEngine;
using System.Collections;

public class COMCamera : MonoBehaviour 
{
	public GameObject target;
	//mouseToOrbit movement
	float x, y;

	public float zoom = -10;
	int zoomSpeed = 10;

	Quaternion rotation;

	void Start () 
	{
	}

	// Update is called once per frame
	void Update () 
	{
		if(this.enabled)
		{
			//Rotation
			if(Input.GetKey(KeyCode.LeftAlt))
			{
				x -= Input.GetAxis("Mouse X");
				y -= Input.GetAxis("Mouse Y");
				
				rotation = Quaternion.Euler(y, x, 0);
				
				transform.rotation = rotation;
			}

			//Zooming
			if(Input.GetAxis("Mouse ScrollWheel") != 0)
			{
				//Uses distance from target to scale zoom speed
				Vector3 difference = this.transform.position - target.transform.position;

				float relDiff = difference.sqrMagnitude;

				relDiff = Mathf.Clamp(relDiff, 0, 100);

				zoom += Input.GetAxis("Mouse ScrollWheel") * relDiff * zoomSpeed * Time.deltaTime;
			}

			transform.position = rotation * new Vector3(0.0f, 0.0f, zoom) + target.transform.position;
		}
	}
}
