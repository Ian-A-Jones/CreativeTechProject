using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceManager : MonoBehaviour 
{
	//How many objects you want to start with
	public int startingObjects = 5;
	//Prefab used for spawning planets
	public GameObject body;
	public GameObject player;
	
	List<GameObject> bodies;

	int gForceAmp = 50;


	GameObject closestPlanetToPlayer = null;
	//Relative distance of closes planet to player
	float relDistPlanetToPlayer = 0;
	// Use this for initialization
	void Start () 
	{
		bodies = new List<GameObject>();

		bodies.Add(Instantiate(body, new Vector3(0, 0.0f, 0.0f), Quaternion.identity) as GameObject);
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

	void Update()
	{
		if(closestPlanetToPlayer != null)
		{
			if(bodies[0].GetComponent<RotateAround>().closetPlanet != closestPlanetToPlayer)
			{
				Debug.Log("Setting closest Planet");
				bodies[0].GetComponent<RotateAround>().closetPlanet = closestPlanetToPlayer;
			}
		}
	}
	// Update is called once per frame
	void FixedUpdate () 
	{
		//unoptimisedFDTG();
		optimisedFDTG();

		if(Input.GetKeyDown(KeyCode.Q))
		{
			bodies.Add(Instantiate(body, new Vector3(5, 5, 5), Quaternion.identity) as GameObject);
			bodies[bodies.Count -1].rigidbody.AddForce(new Vector3(200, 0, 0));
			bodies[bodies.Count -1].name = "body" + (bodies.Count-1);
		}
	}

	void unoptimisedFDTG()
	{
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
				
				if(firstBody.transform.position != secondBody.transform.position)
				{	
					//Find vector from first body to second body
					Vector3 deltaPosition = secondBody.transform.position - firstBody.transform.position;
					//Debug.Log ("deltaPosition: " + deltaPosition.ToString());
					
					Vector3 direction = Vector3.Normalize(deltaPosition);
					//Debug.Log("direction: " + direction.ToString());

					Vector3 edgeOfFirstBody = direction * (firstBody.transform.localScale.x/2);
					Debug.DrawRay(firstBody.transform.position + edgeOfFirstBody, direction, Color.red);
					
					float sqrRadius = deltaPosition.sqrMagnitude;
					//Debug.Log ("Square Radius: " + sqrRadius);
					
					float forcedueToGrav = (firstBody.rigidbody.mass * secondBody.rigidbody.mass)/sqrRadius;
					//Debug.Log ("Force Due To Gravity: " + forcedueToGrav * gForceAmp);
					
					firstBody.rigidbody.AddForce(direction * forcedueToGrav * gForceAmp);
										
				}
			}
		}
	}

	void optimisedFDTG()
	{
		if(bodies == null)
		{
			Debug.LogError("List is null");
		}
		
		for(int fB = 0; fB < bodies.Count ; fB++)
		{
			if(bodies[fB] == null)
			{
				Debug.LogError("bodies[fB] is null");
			}		
			
			for(int sB = 0; sB < bodies.Count ; sB++)
			{
				if(bodies[sB] == null)
				{
					Debug.LogError("bodies[sB] is null");
				}
				
				if(fB < sB)
				{	
					//Find vector from first body to second body
					Vector3 deltaPosition = bodies[sB].transform.position - bodies[fB].transform.position;
					//Debug.Log ("deltaPosition: " + deltaPosition.ToString());

					Vector3 direction = Vector3.Normalize(deltaPosition);
					//Debug.Log("direction: " + direction.ToString());
					
					//Find the position of the edge of the body to draw the ray
					Vector3 edgeOfFirstBody = direction * (bodies[fB].transform.localScale.x/2);
					Debug.DrawRay(bodies[fB].transform.position + edgeOfFirstBody, direction, Color.red);
					
					//Find the position of the edge of the body for the second body;
					Vector3 edgeOfSecondBody = direction * (bodies[sB].transform.localScale.x/2);
					Debug.DrawRay(bodies[sB].transform.position + edgeOfSecondBody, direction, Color.red);
					
					float relDistance = deltaPosition.sqrMagnitude;
					//Debug.Log ("Square Radius: " + sqrRadius);

					if(bodies[fB].tag == "Player")
					{
						if((closestPlanetToPlayer == null || bodies[sB].name != closestPlanetToPlayer.name) &&
						   (relDistance < relDistPlanetToPlayer || relDistPlanetToPlayer == 0))
						{
							relDistPlanetToPlayer = relDistance;
							closestPlanetToPlayer = bodies[sB];
							Debug.Log (bodies[sB].name);
						}
					}
					
					float forcedueToGrav = (bodies[fB].rigidbody.mass * bodies[sB].rigidbody.mass)/relDistance;
					//Debug.Log ("Force Due To Gravity: " + forcedueToGrav * gForceAmp);
					
					bodies[fB].rigidbody.AddForce(direction * forcedueToGrav * gForceAmp);
					bodies[sB].rigidbody.AddForce(direction * -1 * forcedueToGrav * gForceAmp);
														
				}
			}
		}
	}

	public void addPlayer(Vector3 spawn)
	{
		bodies.Insert(0,(Instantiate(player, spawn, Quaternion.identity) as GameObject));
		bodies[0].AddComponent("RotateAround");
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

			body.transform.localScale *= (randomMass * 10f);

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
	