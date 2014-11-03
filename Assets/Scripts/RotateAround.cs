using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour 
{
	public GameObject closetPlanet;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
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
	            Debug.Log("Collided");
	            Debug.DrawLine (this.transform.position, hit.point, Color.cyan);
	            transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
	        }
		}
	}
}
