using UnityEngine;
using System.Collections;

public class BlackHole : MonoBehaviour {

	SpaceManager sM;
	// Use this for initialization
	void Start () {

		sM = GameObject.FindGameObjectWithTag("Space Manager").GetComponent<SpaceManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void collisionLogic(Collision collision)
	{
		Debug.Log ("Collided");
		sM.removeBodyAt(collision.transform.GetInstanceID());
		Debug.Log (collision.transform.GetInstanceID());
	}

	void OnCollisionEnter(Collision collision)
	{
//		Debug.Log ("Collided");

		collisionLogic(collision);
	}
	
	void OnCollisionStay(Collision collision)
	{
//		Debug.Log ("Collided");

		collisionLogic(collision);
	}
}
