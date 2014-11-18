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
	//Reference for Centre of mass position
	public GameObject CentreOfMass;

	//Variables for dealing with Centre of Mass
	Vector3 COMPos;
	float totalMass = 0;

	//Whether the sim is active or not
	bool simActive = false;
	//Whether user is spawning a planet
	bool spawningPlanet = true;

	string spawnPosX = "0";
	string spawnPosY = "0";
	string spawnPosZ = "0";

	string spawnMass = "0";

	string spawnVelocityX = "0";
	string spawnVelocityY = "0";
	string spawnVelocityZ = "0";

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

//		bodies.Add(Instantiate(body, new Vector3(0, 0.0f, 0.0f), Quaternion.identity) as GameObject);
//		bodies[bodies.Count-1].name = "body" + (bodies.Count-1);
////
//		bodies.Add(Instantiate(body, new Vector3(0, 0.0f, 20f), Quaternion.identity) as GameObject);
//		bodies[bodies.Count-1].name = "body" + (bodies.Count-1);

		StartCoroutine("spawnBodies");

		setMass(0, 2);

	}

	IEnumerator spawnBodies()
	{
		for(int i = 0; i < startingObjects; i++)
		{
			float randX = Random.Range(-range, range);
			float randY = Random.Range(-range, range);
			float randZ = Random.Range(-range, range);
			
			bodies.Add(Instantiate(body, new Vector3(randX, randY, randZ), Quaternion.identity) as GameObject);
			bodies[bodies.Count-1].name = "body" + (bodies.Count-1);
			shuffleMass(bodies.Count-1);
			Debug.Log("Object Added");
			yield return null;
		}

		simActive = true;
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

		if(Input.GetKeyDown(KeyCode.P))
		{
			if(Time.timeScale == 1)
			{
				Time.timeScale = 0;
			}
			else
			{
				Time.timeScale = 1;
			}
		}
	}

	void OnGUI()
	{
		if(Time.timeScale == 0)
		{
			GUI.Label(new Rect(Screen.width/2, Screen.height/2, 120, 22), "Simulation Paused");
		}

		if(spawningPlanet)
		{
			GUI.BeginGroup(new Rect(Screen.width - 200, 4, 200, 200));

			GUI.Button(new Rect(0,0, 18, 22), "<");

			GUI.BeginGroup(new Rect(18, 0, 200, 200));

			GUI.Box(new Rect(0, 0, 200, 200), "Planet Spawning options");

			GUI.Label(new Rect(0, 22, 160, 22), "Position");

			GUI.Label(new Rect(0, 44, 22, 22), "X: ");
			spawnPosX = GUI.TextField(new Rect(14, 44, 36, 22), spawnPosX);

			GUI.Label(new Rect(56, 44, 22, 22), "Y: ");
			spawnPosY = GUI.TextField(new Rect(70, 44, 36, 22), spawnPosY);

			GUI.Label(new Rect(112, 44, 22, 22), "Z: ");
			spawnPosZ = GUI.TextField(new Rect(126, 44, 36, 22), spawnPosZ);

			GUI.Label(new Rect(0, 66, 160, 22), "Velocity");
			
			GUI.Label(new Rect(0, 88, 22, 22), "X: ");
			spawnVelocityX = GUI.TextField(new Rect(14, 88, 36, 22), spawnVelocityX);
			
			GUI.Label(new Rect(56, 88, 22, 22), "Y: ");
			spawnVelocityY = GUI.TextField(new Rect(70, 88, 36, 22), spawnVelocityY);
			
			GUI.Label(new Rect(112, 88, 22, 22), "Z: ");
			spawnVelocityZ = GUI.TextField(new Rect(126, 88, 36, 22), spawnVelocityZ);

			GUI.Label(new Rect(0, 110, 60, 22), "Mass");
			spawnMass = GUI.TextField(new Rect(0, 132, 18, 22), spawnMass);

			if(GUI.Button(new Rect(0, 154, 120, 22), "Spawn Planet"))
			{
				Vector3 spawnPos;
				spawnPos.x = int.Parse(spawnPosX);
				spawnPos.y = int.Parse(spawnPosY);
				spawnPos.z = int.Parse(spawnPosZ);

				Vector3 spawnVelocity;
				spawnVelocity.x = int.Parse(spawnVelocityX);
				spawnVelocity.y = int.Parse(spawnVelocityY);
				spawnVelocity.z = int.Parse(spawnVelocityZ);

				bodies.Add(Instantiate(body, spawnPos, Quaternion.identity) as GameObject);
				bodies[bodies.Count -1].rigidbody.AddForce(spawnVelocity);
				bodies[bodies.Count -1].name = "body" + (bodies.Count-1);
				setMass(bodies.Count-1, int.Parse(spawnMass));

			}

			GUI.EndGroup();

			GUI.EndGroup();

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

		COMPos = Vector3.zero;

		totalMass = 0;

		for(int fB = 0; fB < bodies.Count ; fB++)
		{

			COMPos.x += bodies[fB].rigidbody.mass * bodies[fB].transform.position.x;
			COMPos.y += bodies[fB].rigidbody.mass * bodies[fB].transform.position.y;
			COMPos.z += bodies[fB].rigidbody.mass * bodies[fB].transform.position.z;

			totalMass += bodies[fB].rigidbody.mass;

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

		if(bodies.Count > 0)
		{
			COMPos /= totalMass;

			CentreOfMass.transform.position = COMPos;
		}
	}

	public void addPlayer(Vector3 spawn)
	{
		bodies.Insert(0,(Instantiate(player, spawn, Quaternion.identity) as GameObject));
	}

	void shuffleMass(int body)
	{
		float randomMass = Random.value + 0.1f;

		if(randomMass > 10)
		{
			Debug.LogError("Mass greater than 10, unstability in simulation may occur");
		}
		else
		{
			bodies[body].rigidbody.mass = randomMass;
		}

		bodies[body].transform.localScale = new Vector3((randomMass * 10f), (randomMass * 10f), (randomMass * 10f));
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
	