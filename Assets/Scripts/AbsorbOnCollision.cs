using UnityEngine;
using System.Collections;

public class AbsorbOnCollision : MonoBehaviour 
{
	public SpaceManager spaceManagerObj;
	public SpaceManager spaceManagerScript;

	public static bool absorbOn;

	// Use this for initialization
	void Start () 
	{
		GameObject spaceManagerObj = GameObject.Find("SpaceManager");
		spaceManagerScript = spaceManagerObj.GetComponent<SpaceManager>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void collisionLogic(Collision collision)
	{
		if(this.tag != "Player" && collision.gameObject.tag != "Player" && absorbOn)
		{
			Debug.Log("Bodies collieded");
			
			float otherBMass = collision.rigidbody.mass;
			if(this.rigidbody.mass >= otherBMass)
			{
				Debug.Log ("This larger than that");
				this.rigidbody.mass += otherBMass;
				this.transform.localScale += new Vector3(otherBMass, otherBMass, otherBMass);
				spaceManagerScript.removeBodyAt(collision.transform.position);
			}
			else
			{
				Debug.Log ("that larger than this");
				collision.rigidbody.mass += this.rigidbody.mass;
				collision.transform.localScale += new Vector3(this.rigidbody.mass, this.rigidbody.mass, this.rigidbody.mass);
				
				spaceManagerScript.removeBodyAt(transform.position);
			}
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		collisionLogic(collision);
	}

	void OnCollisionStay(Collision collision)
	{
		collisionLogic(collision);
	}
}
