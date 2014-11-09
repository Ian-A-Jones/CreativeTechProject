using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceManager : MonoBehaviour 
{
	//How many objects you want to start with
	public int startingObjects;
	//How far planets should spawn
	public float range;
	//how strong gravity should be
	public int gForceAmp = 50;
	//Prefab used for spawning planets
	public GameObject body;
	public GameObject player;
	
	List<GameObject> bodies;
	
	//Variables for Inter-planetary interaction
	Vector3 deltaPosition;
	Vector3 direction;	
	Vector3 edgeOfFirstBody;
	Vector3 edgeOfSecondBody;	
	float relDistance;
	float forceDueToGrav;

	GameObject closestPlanetToPlayer = null;
	float clossetPlanetMass;
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

		for(int i = 0; i < startingObjects; i++)
		{
			float randX = Random.Range(-range, range);
			float randY = Random.Range(-range, range);
			float randZ = Random.Range(-range, range);

			bodies.Add(Instantiate(body, new Vector3(randX, randY, randZ), Quaternion.identity) as GameObject);
			bodies[bodies.Count-1].name = "body" + (bodies.Count-1);
			Debug.Log("Object Added");
		}

		shuffleMass();

		setMass(0, 10);

	}

	void Update()
	{
		if(closestPlanetToPlayer != null)
		{
			if(bodies[0].GetComponent<PlayerInput>().closetPlanet != closestPlanetToPlayer)
			{
				Debug.Log("Setting closest Planet");
				bodies[0].GetComponent<PlayerInput>().closetPlanet = closestPlanetToPlayer;
				bodies[0].GetComponent<PlayerInput>().closestPlanetMass = clossetPlanetMass;
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

//	void unoptimisedFDTG()
//	{
//		foreach(GameObject firstBody in bodies)
//		{
//			if(firstBody == null)
//			{
//				Debug.LogError("I is null");
//			}
//			foreach(GameObject secondBody in bodies)
//			{
//				if(secondBody == null)
//				{
//					Debug.LogError("J is null");
//				}
//				
//				if(firstBody.transform.position != secondBody.transform.position)
//				{	
//					//Find vector from first body to second body
//					deltaPosition = secondBody.transform.position - firstBody.transform.position;
//					//Debug.Log ("deltaPosition: " + deltaPosition.ToString());
//					
//					direction = Vector3.Normalize(deltaPosition);
//					//Debug.Log("direction: " + direction.ToString());
//
//					edgeOfFirstBody = direction * (firstBody.transform.localScale.x/2);
//					Debug.DrawRay(firstBody.transform.position + edgeOfFirstBody, direction, Color.red);
//					
//					relDistance = deltaPosition.sqrMagnitude;
//					//Debug.Log ("Square Radius: " + relDistance);
//					
//					forceDueToGrav = (firstBody.rigidbody.mass * secondBody.rigidbody.mass)/relDistance;
//					//Debug.Log ("Force Due To Gravity: " + forcedueToGrav * gForceAmp);
//					
//					firstBody.rigidbody.AddForce(direction * forceDueToGrav * gForceAmp);
//										
//				}
//			}
//		}
//	}

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

			bodies[fB].rigidbody.mass = Mathf.Clamp(bodies[fB].rigidbody.mass, 0, 10);

			for(int sB = 0; sB < bodies.Count ; sB++)
			{
				if(bodies[sB] == null)
				{
					Debug.LogError("bodies[sB] is null");
				}
				
				if(fB < sB)
				{	
					//Find vector from first body to second body
					deltaPosition = bodies[sB].transform.position - bodies[fB].transform.position;
					//Debug.Log ("deltaPosition: " + deltaPosition.ToString());

					direction = Vector3.Normalize(deltaPosition);
					//Debug.Log("direction: " + direction.ToString());
					
					//Find the position of the edge of the body to draw the ray
					edgeOfFirstBody = direction * (bodies[fB].transform.localScale.x/2);
					Debug.DrawRay(bodies[fB].transform.position + edgeOfFirstBody, direction, Color.red);
					
					//Find the position of the edge of the body for the second body;
					edgeOfSecondBody = direction * (bodies[sB].transform.localScale.x/2);
					Debug.DrawRay(bodies[sB].transform.position + edgeOfSecondBody, direction, Color.red);
					
					relDistance = deltaPosition.sqrMagnitude;
					//Debug.Log ("Square Radius: " + relDistance);

					if(bodies[fB].tag == "Player")
					{
						if((closestPlanetToPlayer == null || bodies[sB].name != closestPlanetToPlayer.name) &&
						   (relDistance < relDistPlanetToPlayer || relDistPlanetToPlayer == 0))
						{
							relDistPlanetToPlayer = relDistance;
							closestPlanetToPlayer = bodies[sB];
							clossetPlanetMass = bodies[sB].rigidbody.mass;
							Debug.Log (bodies[sB].name);
						}
					}
					
					forceDueToGrav = (bodies[fB].rigidbody.mass * bodies[sB].rigidbody.mass)/(relDistance/10);
					
					bodies[fB].rigidbody.AddForce(direction * forceDueToGrav * gForceAmp);
					bodies[sB].rigidbody.AddForce(direction * -1 * forceDueToGrav * gForceAmp);
														
				}
			}
		}
	}

	public void addPlayer(Vector3 spawn)
	{
		bodies.Insert(0,(Instantiate(player, spawn, Quaternion.identity) as GameObject));
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

			body.transform.localScale = new Vector3((randomMass * 10f), (randomMass * 10f), (randomMass * 10f));

		}
	}

	void setMass(int BodiesMassToSet, float massToSet)
	{
		bodies[BodiesMassToSet].rigidbody.mass = massToSet;

		bodies[BodiesMassToSet].transform.localScale = new Vector3((massToSet * 10f), (massToSet * 10f), (massToSet * 10f));
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
	