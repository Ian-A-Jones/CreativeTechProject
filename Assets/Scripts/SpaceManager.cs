using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceManager : MonoBehaviour 
{
	//How many objects you want to start with
	public int startingObjects = 5;
	//Prefab used for spawning planets
	public GameObject body;

	List<GameObject> bodies;

	int gForceAmp = 50;
	// Use this for initialization
	void Start () 
	{
		bodies = new List<GameObject>();

		bodies.Add(Instantiate(body, new Vector3(0, 0.0f, 0.0f), Quaternion.Euler(90,0,0)) as GameObject);
		bodies[bodies.Count-1].name = "body" + (bodies.Count-1);
		//bodies.Add(Instantiate(body, new Vector3(-5, 0.0f, 0.0f), Quaternion.identity) as GameObject);
		//bodies.Add(Instantiate(body, new Vector3(0, 5f, 0.0f), Quaternion.identity) as GameObject);

//		for(int i = 0; i < startingObjects; i++)
//		{
//			CBodies.Add(Instantiate(CBody, new Vector3(i * 5, 0.0f, 0.0f), Quaternion.identity) as GameObject);
//			Debug.Log("Object Added");
//		}

		shuffleMass();

	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		if(bodies == null)
		{
			Debug.LogError("List is null");
		}

		foreach(GameObject firstBody in bodies)
		{
			if(firstBody == null)
			{
				Debug.LogError("I is null");
			}
			foreach(GameObject secondBody in bodies)
			{
				if(secondBody == null)
				{
					Debug.LogError("J is null");
				}

				if(firstBody.name != secondBody.name)
				{	
					//Find vector from first body to second body
					Vector3 deltaPosition = secondBody.transform.position - firstBody.transform.position;
					Debug.Log ("deltaPosition: " + deltaPosition.ToString());

					Vector3 direction = Vector3.Normalize(deltaPosition);
					Debug.Log("direction: " + direction.ToString());

					Debug.DrawRay(firstBody.transform.position, direction, Color.red);

					float sqrRadius = deltaPosition.sqrMagnitude;
					Debug.Log ("Square Radius: " + sqrRadius);

					float forcedueToGrav = (firstBody.rigidbody.mass * secondBody.rigidbody.mass)/sqrRadius;
					Debug.Log ("Force Due To Gravity: " + forcedueToGrav * gForceAmp);

					firstBody.rigidbody.AddForce(direction * forcedueToGrav * gForceAmp);

				}
			}
		}

		if(Input.GetKeyDown(KeyCode.Q))
		{
			bodies.Add(Instantiate(body, new Vector3(5, 5, 5), Quaternion.identity) as GameObject);
			bodies[bodies.Count -1].rigidbody.AddForce(new Vector3(200, 0, 0));
			bodies[bodies.Count -1].name = "body" + (bodies.Count-1);
		}

	}

	void shuffleMass()
	{
		foreach(GameObject body in bodies)
		{
			float randomMass = Random.value + 0.1f;

			if(randomMass > 10)
			{
				Debug.LogError("Mass greater than 10, unstability in simulation may occur");
			}
			else
			{
				body.rigidbody.mass = randomMass;
			}

			body.transform.localScale *= (randomMass * 25f);

		}
	}

	public void removeBodyAt(Vector3 position)
	{
		foreach(GameObject body in bodies.ToArray())
		{
			if(body.transform.position == position)
			{
				bodies.Remove(body);
				Destroy(body);
			}

		}
	}
}
	