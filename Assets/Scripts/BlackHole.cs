//Simple script for removing objects that touch a black hole
using UnityEngine;
using System.Collections;

public class BlackHole : MonoBehaviour {

	SpaceManager sM;
	// Use this for initialization
	void Start () {

		//Find spaceManager using Tags
		sM = GameObject.FindGameObjectWithTag("Space Manager").GetComponent<SpaceManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void collisionLogic(Collision collision)
	{
		//Removed collided object
		sM.removeBodyAt(collision.transform.GetInstanceID());
	}

	//Events called when objects collide with Black Hole
	void OnCollisionEnter(Collision collision)
	{
		collisionLogic(collision);
	}
	
	void OnCollisionStay(Collision collision)
	{
		collisionLogic(collision);
	}
}
