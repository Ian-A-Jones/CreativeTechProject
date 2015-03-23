using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceManager : MonoBehaviour 
{
	//How many objects you want to start with
	public int planetsToSpawn;

	//How far planets should spawn
	public float posRange;

	//Gravity Scalar
	public static float gForceAmp = 100;

	//Prefab used for spawning planets
	public SpaceObject planetTemplate;
	public SpaceObject sunTemplate;
	public SpaceObject ringTemplate;
	public SpaceObject BlackHoleTemplate;
	public PlayerCam camPlayerTemplate;

	//Reference to player
	public SpaceObject player;

	//Reference for Centre of mass position
	public GameObject CentreOfMassRef;
	public GameObject spawningPositionRef;
	public GameObject spawningVeloctiyRef;

	//Scale the planets to make them larger
	public float massRange;

	public static float planetSizeScale = 1000;

	public float timeScale = 1;

	//Reference to PlayerCam on planet;
	PlayerCam camPlayerInstance;

	//Variables for dealing with Centre of Mass
	Vector3 COMPos;
	float totalMass = 0;

	//Can the user pause the sim
	bool canPause = true;
	
	//Whether user is spawning a planet
	bool spawningPlanet = false;
	bool spawningMultPlanets = false;

	string spawnButton = "";
	string spawnMultButton = "";

	string spawnPosX = "0";
	string spawnPosY = "0";
	string spawnPosZ = "0";

	string spawnMass = "1";

	string spawnVelocityX = "0";
	string spawnVelocityY = "0";
	string spawnVelocityZ = "0";

	string spawnPosRange = "100";
	string spawnMassRange = "50";
	string spawnNumberOfPlanets = "50";

	Vector3 spawnPos;
	Vector3 spawnVelocity;

	List<SpaceObject> bodies;
	List<SpaceObject> Rings;
	
	//Variables for Inter-planetary interaction
	Vector3 deltaPosition;
	Vector3 direction;	
	Vector3 edgeOfFirstBody;
	Vector3 edgeOfSecondBody;	
	float relDistance;
	float forceDueToGrav;

	//Variables for player as part of the simulation
	SpaceObject closestPlanetToPlayer = null;
	float clossetPlanetMass;
	//Relative distance of closes planet to player
	float relDistPlanetToPlayer = 0;

	//Strings for GUI
	string toggleAbsorb = "Absorbing On?";

	//Height and width relating to strings
	float sH = 22;
	float sW = 8;

	//Variables for spawning Rings
	float randAngle;
	float randDist;
	float xPos;
	float zPos;

	int sB;
	int cB = 0;

	int start = 0, 
		finish, 
		step = 50;
	
	// Use this for initialization
	void Start () 
	{

		bodies = new List<SpaceObject>();
		Rings = new List<SpaceObject>();

		spawnBody("Sun", SpaceObject.bodyType.Sun, Vector3.zero, PStats.SunMass, PStats.SunDiam, null, 1);

		spawnBody("Mercury", SpaceObject.bodyType.planet, new Vector3(PStats.MercuryDist, 0, 0), PStats.MercuryMass, PStats.MercuryDiam, bodies[0], 1);

		spawnBody("Venus", SpaceObject.bodyType.planet, new Vector3(PStats.VenusDist, 0, 0), PStats.VenusMass, PStats.VenusDiam, bodies[0], 1);

		spawnBody("Earth", SpaceObject.bodyType.planet, new Vector3(PStats.EarthDist, 0, 0), PStats.EarthMass, PStats.EarthDiam, bodies[0], 1);

		spawnBody("Moon", SpaceObject.bodyType.planet, new Vector3(PStats.MoonDist, 0, 0), PStats.MoonMass, PStats.MoonDiam, bodies[bodies.Count-1], 10);

		spawnBody("Mars", SpaceObject.bodyType.planet, new Vector3(PStats.MarsDist, 0, 0), PStats.MarsMass, PStats.MarsDiam, bodies[0], 1);

		spawnBody("Deimos", SpaceObject.bodyType.planet, new Vector3(PStats.DeimosDist, 0, 0), PStats.DeimosMass, PStats.DeimosDiam, bodies[bodies.Count-1], 5);

		spawnRing("Asteroids", Vector3.zero, PStats.AstMass, PStats.EarthDiam, bodies[0], 50, 336, 200);

		spawnBody("Jupiter", SpaceObject.bodyType.planet, new Vector3(PStats.JupiterDist, 0, 0), PStats.JupiterMass, PStats.JupiterDiam, bodies[0], 1);

		spawnRing("Asteroids2", new Vector3(PStats.JupiterDist, 0, 0), PStats.AstMass, PStats.EarthDiam, bodies[bodies.Count-1], 50, 80, 50);

		spawnBody("Saturn", SpaceObject.bodyType.planet, new Vector3(PStats.SaturnDist, 0, 0), PStats.SaturnMass, PStats.SaturnDiam, bodies[0], 1);

		spawnRing("Asteroids3", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.EarthDiam, bodies[bodies.Count-1], 50, 60, 50);

		spawnBody("Uranus", SpaceObject.bodyType.planet, new Vector3(PStats.UranusDist, 0, 0), PStats.UranusMass, PStats.UranusDiam, bodies[0], 1);

		spawnRing("Asteroids4", new Vector3(PStats.UranusDist, 0, 0), PStats.AstMass, PStats.EarthDiam, bodies[bodies.Count-1], 100, 40, 50);

		spawnBody("Neptune", SpaceObject.bodyType.planet, new Vector3(PStats.NeptuneDist, 0, 0), PStats.NeptuneMass, PStats.NeptuneDiam, bodies[0], 1);

		spawnRing("Asteroids5", new Vector3(PStats.NeptuneDist, 0, 0), PStats.AstMass, PStats.EarthDiam, bodies[bodies.Count-1], 50, 30, 50);

		spawnBody("Pluto", SpaceObject.bodyType.planet, new Vector3(PStats.PlutoDist, 0, 0), PStats.PlutoMass, PStats.PlutoDiam, bodies[0], 1);


//		foreach(SpaceObject sO in bodies)
//		{
//			sO.rigidbody.AddForce(0,0, sO.avgOrbitVelocity);
//		}

		Time.timeScale = timeScale;

		if(bodies.Count < step)
		{
			finish = bodies.Count;
		}
		else
		{
			finish = step;
		}
	}

	IEnumerator spawnBodies()
	{
		Time.timeScale = 0;

		canPause = false;

		AbsorbOnCollision.absorbOn = false;

		for(int i = 0; i < planetsToSpawn; i++)
		{
			float randX = Random.Range(-posRange, posRange);
			float randY = Random.Range(-posRange, posRange);
			float randZ = Random.Range(-posRange, posRange);	

			bodies.Add(Instantiate(planetTemplate, new Vector3(randX, randY, randZ), Quaternion.identity) as SpaceObject);

			bodies[bodies.Count-1].name = "body" + (bodies.Count-1);

			shuffleMass(bodies.Count-1);
//			setMass(bodies.Count-1, 0.9f);

//			float velRange = 2500;
//
//			float randomVelX = Random.Range(-velRange, velRange);
//			float randmoVelY = Random.Range(-velRange, velRange);
//			float arndomVelZ = Random.Range(-velRange, velRange);
//
//			bodies[bodies.Count-1].rigidbody.AddForce(new Vector3(randomVelX, randmoVelY , arndomVelZ));

			Debug.Log("Object Added");
			yield return null;
		}

		Time.timeScale = 1;

		canPause = true;

		//Start with absorbing off
		AbsorbOnCollision.absorbOn = false;
	}

	void bodyType(SpaceObject.bodyType bType, Vector3 pos)
	{

	}

	void spawnBody(string _Name, SpaceObject.bodyType bType, Vector3 pos, float mass, float diam, SpaceObject _OrbitTarget, float orbitPeriod)
	{
		switch(bType)
		{
		case SpaceObject.bodyType.planet:
			
			bodies.Add(Instantiate(planetTemplate, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity) 
			           as SpaceObject);	
			
			break;
			
		case SpaceObject.bodyType.Sun:
			
			bodies.Add(Instantiate(sunTemplate, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity) 
			           as SpaceObject);	
			
			break;
			
		case SpaceObject.bodyType.Ring:

			Debug.Log (pos.x);

			for(int i = 0; i < 100; i ++)
			{
				addAsteroid(pos, 80);

				bodies[bodies.Count-1].init(_Name, bType, mass, diam, _OrbitTarget, orbitPeriod);
			}
			
			break;

		case SpaceObject.bodyType.BlackHole:

			bodies.Add(Instantiate(BlackHoleTemplate, pos, Quaternion.identity) as SpaceObject);

			break;
		}

		if(bType != SpaceObject.bodyType.Ring)
		{
			bodies[bodies.Count-1].init(_Name, bType, mass, diam, _OrbitTarget, orbitPeriod);
		}
	}

	//Spawn body from camera at speed
	void spawnBody(float mass, float diam, float velocity)
	{
		bodies.Add(Instantiate(planetTemplate, Camera.main.transform.position, Quaternion.identity) 
		           as SpaceObject);	

		bodies[bodies.Count-1].init("Thing", SpaceObject.bodyType.planet, mass, diam, velocity);

	}

	void spawnRing(string _Name, Vector3 orbit, float mass, float diam, SpaceObject _OrbitTarget, float orbitPeriod, 
	               float distance, float amount)
	{
		for(int i = 0; i < amount; i ++)
		{
			addAsteroid(orbit, distance);
			
			bodies[bodies.Count-1].init(_Name, SpaceObject.bodyType.Ring, mass, diam, _OrbitTarget, orbitPeriod);
		}
	}

	void addAsteroid(Vector3 pos, float _Distance)
	{
		randAngle = Random.value * 360;
		
		//				Debug.Log (randAngle);
		
		randDist = Random.Range(0,_Distance/2) - (_Distance)/4;


		xPos = (_Distance + randDist) * Mathf.Cos(randAngle);
		
//						Debug.Log (xPos);
		
		zPos = (_Distance + randDist) * Mathf.Sin(randAngle);
		
//						Debug.Log (zPos);
		
		bodies.Add(Instantiate(ringTemplate, pos + new Vector3(xPos, 0, zPos), Quaternion.identity) 
		           as SpaceObject);
		
		//				mass += Random.Range(0, mass/4) - mass/8;
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Y))
		{
			Debug.Log ("Spawning Black Hole");

			spawnBody("BlackHole", SpaceObject.bodyType.BlackHole, Vector3.up*100, 90000000, PStats.SunDiam, null, 0);

//			addAsteroid(PStats.ABelt);

//			bodies[bodies.Count-1].init(name, SpaceObject.bodyType.Ring, PStats.AstMass, PStats.EarthDiam, bodies[0], 50);
		}

		if(Input.GetKeyDown(KeyCode.F))
		{
			Debug.Log ("Spawning");

//			spawnBody(1000, PStats.NeptuneDiam, 9000000);
			spawnBody(100, PStats.NeptuneDiam, 300000);

		}

		if(Input.GetKeyDown(KeyCode.O))
		{
			SpaceObject.bMaintainOrbit = !SpaceObject.bMaintainOrbit;
		}

		if(closestPlanetToPlayer != null)
		{
			if(bodies[0].GetComponent<PlayerInput>().closetPlanet != closestPlanetToPlayer)
			{
				Debug.Log("Setting closest Planet");
				bodies[0].GetComponent<PlayerInput>().closetPlanet = closestPlanetToPlayer;
				bodies[0].GetComponent<PlayerInput>().closestPlanetMass = clossetPlanetMass;
			}
		}

//		Debug.Log (Vector3.Magnitude(bodies[2].rigidbody.velocity));

		if(Input.GetKeyDown(KeyCode.P) && canPause)
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

		if(Input.GetKeyDown(KeyCode.R))
		{
			if(bodies.Count > 0)
			{
				foreach(SpaceObject body in bodies.ToArray())
				{
					Destroy(body);
				}

				bodies.Clear();
			}
		}

		if(Input.GetKeyDown(KeyCode.C))
		{
			if(CentreOfMassRef.activeSelf)
			{
				CentreOfMassRef.SetActive(false);
			}
			else
			{
				CentreOfMassRef.SetActive(true);
			}
		}

		spawningPositionRef.SetActive(spawningPlanet);
		spawningVeloctiyRef.SetActive(spawningPlanet);
		
		if(spawningPlanet)
		{
			spawnPos.x = stringCheck(spawnPosX);
			spawnPos.y = stringCheck(spawnPosY);
			spawnPos.z = stringCheck(spawnPosZ);
			
			spawningPositionRef.transform.position = spawnPos;

			int scale  = stringCheck(spawnMass);

			spawningPositionRef.transform.localScale = new Vector3(scale, scale, scale);

			spawnVelocity.x = stringCheck(spawnVelocityX);
			spawnVelocity.y = stringCheck(spawnVelocityY);
			spawnVelocity.z = stringCheck(spawnVelocityZ);
			
			spawningVeloctiyRef.transform.localScale = new Vector3(0.1f, spawnVelocity.magnitude, 0.1f);
			
			spawningVeloctiyRef.transform.position = spawnPos + spawnVelocity;
			
			spawningVeloctiyRef.transform.rotation = Quaternion.FromToRotation(Vector3.up, spawnVelocity.normalized);
		}
	}

	void OnGUI()
	{
		AbsorbOnCollision.absorbOn = GUI.Toggle(new Rect(0, Screen.height - sH, sW * toggleAbsorb.Length, sH), AbsorbOnCollision.absorbOn, toggleAbsorb);

		GUI.Label(new Rect(Screen.width/2, 0, 200, 30), "Total bodies: " + bodies.Count);

		if(Time.timeScale == 0)
		{
			GUI.Label(new Rect(Screen.width/2, Screen.height/2, 120, 22), "Simulation Paused");
		}

		#region spawning single plant

		Rect spawningPos;

		if(spawningPlanet)
		{
			spawningPos = new Rect(Screen.width - 200, 4, 200, 200);
			spawnButton = "<";
		}
		else
		{
			spawningPos = new Rect(Screen.width - 18, 4, 200, 200);
			spawnButton = ">";
		}

		GUI.BeginGroup(spawningPos);

		if(GUI.Button(new Rect(0,0, 18, 22), spawnButton))
		{
			if(spawningPlanet)
			{
				spawningPlanet = false;
			}
			else
			{
				spawningPlanet = true;
			}
		}

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
		spawnMass = GUI.TextField(new Rect(0, 132, 72, 22), spawnMass);
		GUI.Label (new Rect(76, 132, 16 * sW, 22), "(max Mass : " + SpaceObject.maxMass + ")");

		if(GUI.Button(new Rect(0, 154, 120, 22), "Spawn Planet"))
		{

			bodies.Add(Instantiate(planetTemplate, spawnPos, Quaternion.identity) as SpaceObject);
			bodies[bodies.Count -1].rigidbody.AddForce(spawnVelocity);
			bodies[bodies.Count -1].name = "body" + (bodies.Count-1);
			bodies[bodies.Count -1].setMassAndSize(int.Parse(spawnMass),int.Parse(spawnMass));
		}

		GUI.EndGroup();

		GUI.EndGroup();

		#endregion

		#region spawning mutliple planets

		Rect spawningMultPos;
		
		if(spawningMultPlanets)
		{
			spawningMultPos = new Rect(Screen.width - 200, 204, 200, 200);
			spawnMultButton = "<";
		}
		else
		{
			spawningMultPos = new Rect(Screen.width - 18, 204, 200, 200);
			spawnMultButton = ">";
		}
		
		GUI.BeginGroup(spawningMultPos);
		
		if(GUI.Button(new Rect(0,0, 18, 22), spawnMultButton))
		{
			if(spawningMultPlanets)
			{
				spawningMultPlanets = false;
			}
			else
			{
				spawningMultPlanets = true;
			}
		}
		
		GUI.BeginGroup(new Rect(18, 0, 200, 200));
		
		GUI.Box(new Rect(0, 0, 200, 200), "Multiple Planet Spawning options");
		
		GUI.Label(new Rect(0, 22, 160, 22), "Total Planets");

		spawnNumberOfPlanets = GUI.TextField(new Rect(0, 44, 36, 22), spawnNumberOfPlanets);

		GUI.Label(new Rect(0, 66, 160, 22), "Position Range");

		spawnPosRange = GUI.TextField(new Rect(0, 88, 36, 22), spawnPosRange);

		GUI.Label(new Rect(0, 110, 60, 22), "Mass Range");
		spawnMassRange = GUI.TextField(new Rect(0, 132, 72, 22), spawnMassRange);
		GUI.Label (new Rect(76, 132, 16 * sW, 22), "(max Mass : " + SpaceObject.maxMass + ")");
		
		if(GUI.Button(new Rect(0, 154, 120, 22), "Spawn Planets"))
		{
			planetsToSpawn = int.Parse(spawnNumberOfPlanets);
			posRange = int.Parse(spawnPosRange);
			massRange = Mathf.Clamp(int.Parse(spawnMassRange), 0, SpaceObject.maxMass);

			StartCoroutine("spawnBodies");
		}
		
		GUI.EndGroup();
		
		GUI.EndGroup();

		#endregion
	}
	
	void FixedUpdate () 
	{
		//unoptimisedFDTG();
		optimisedFDTG();
	}

	void optimisedFDTG()
	{
		if(bodies == null)
		{
			Debug.LogError("List is null");
		}

		COMPos = Vector3.zero;

		totalMass = 0;


		for(int fB = 0; fB < bodies.Count; fB++)
		{
			if(fB >= bodies.Count)
			{
				fB = bodies.Count-1;
			}

			if(bodies[fB] == null)
			{
				bodies.Remove(bodies[fB]);
				Debug.LogWarning("bodies[fB] is null");
				break;
			}		

			//			fB = Mathf.Clamp(fB, 0, bodies.Count);

//			bodies[fB].rigidbody.mass = Mathf.Clamp(bodies[fB].rigidbody.mass, 0, SpaceObject.maxMass);

			COMPos.x += bodies[fB].rigidbody.mass * bodies[fB].transform.position.x;
			COMPos.y += bodies[fB].rigidbody.mass * bodies[fB].transform.position.y;
			COMPos.z += bodies[fB].rigidbody.mass * bodies[fB].transform.position.z;

			totalMass += bodies[fB].rigidbody.mass;

			for(sB = start; sB < finish; sB++)
			{
//				sB = Mathf.Clamp(sB, 0, bodies.Count-1);

				if(sB >= bodies.Count)
				{
					sB = bodies.Count-1;
				}

				if(bodies[sB] == null)
				{
					bodies.Remove(bodies[sB]);
					Debug.LogWarning("bodies[sB] is null");
					break;
				}

				if(fB < sB && (bodies[fB].canOrbit(bodies[sB]) || bodies[sB].canOrbit(bodies[fB])))
				{	
//					Debug.Log (bodies[fB].name + " " + bodies[sB].name);
					//					Debug.Log ("fB + sB: " + fB + sB);
					//Find vector from first body to second body

					deltaPosition = bodies[sB].transform.position - bodies[fB].transform.position;
					//Debug.Log ("deltaPosition: " + deltaPosition.ToString());

					if(deltaPosition.sqrMagnitude > (bodies[fB].transform.localScale.x/2 + bodies[sB].transform.localScale.x/2))
					{
						direction = Vector3.Normalize(deltaPosition);
						//Debug.Log("direction: " + direction.ToString());
						
						//Find the position of the edge of the body to draw the ray
						edgeOfFirstBody = direction * (bodies[fB].transform.localScale.x/2);
						//Debug.DrawRay(bodies[fB].transform.position + edgeOfFirstBody, direction, Color.red);
						
						//Find the position of the edge of the body for the second body;
						edgeOfSecondBody = direction * (bodies[sB].transform.localScale.x/2);
						//Debug.DrawRay(bodies[sB].transform.position + edgeOfSecondBody, direction, Color.red);
						
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

						forceDueToGrav = (bodies[fB].rigidbody.mass * bodies[sB].rigidbody.mass) 
							* gForceAmp;

						forceDueToGrav/= (relDistance);
						
						if(bodies[fB].canOrbit(bodies[sB]))
						{
							if(bodies[fB].orbitTarget && bodies[fB].orbitTarget == bodies[sB])
							{
								bodies[fB].directionToOrbitTarget = direction;
							}
							bodies[fB].rigidbody.AddForce(direction * forceDueToGrav * Time.deltaTime);
						}


						if(bodies[sB].canOrbit(bodies[fB]))
						{
							if(bodies[sB].orbitTarget && bodies[sB].orbitTarget == bodies[fB])
							{
								bodies[sB].directionToOrbitTarget = direction * -1;
							}

							bodies[sB].rigidbody.AddForce(direction * -1 * forceDueToGrav * Time.deltaTime);
						}
					}
					else
					{
						Debug.Log("Error");
					}

				}
			}

			start = finish;

			if(finish >= bodies.Count) //If at the end of list
			{
				start = 0;

				if(bodies.Count < step)
				{
					finish = bodies.Count;
				}
				else
				{
					finish = step;
				}
			}
			else
			{
				if(finish + step < bodies.Count)
				{
					finish += step;
				}
				else
				{
					finish = bodies.Count;
				}
			}
		}

		if(bodies.Count > 0 && totalMass != 0)
		{
			COMPos /= totalMass;

			CentreOfMassRef.transform.localScale = new Vector3(totalMass, totalMass, totalMass)/100;
			CentreOfMassRef.transform.position = COMPos;
		}
		else
		{
			CentreOfMassRef.SetActive(false);
		}
	}

	public void addPlayerSim(Vector3 spawn)
	{
		bodies.Insert(0,(Instantiate(player, spawn, Quaternion.identity) as SpaceObject));
	}

	public void addPlayerCam(GameObject planet, Vector3 spawn)
	{
		camPlayerInstance = Instantiate(camPlayerTemplate, spawn, Quaternion.identity) as PlayerCam;

		camPlayerInstance.orbit = planet;
	}

	void shuffleMass(int body)
	{
		float randomMass = Random.Range(1, massRange);
//		if(randomMass > 10)
//		{
//			Debug.LogError("Mass greater than 10, unstability in simulation may occur");
//		}
//		else
//		{
			bodies[body].rigidbody.mass = randomMass;
//		}

		bodies[body].transform.localScale = new Vector3((randomMass), (randomMass), (randomMass));
	}

	public void removeBodyAt(int iD)
	{
//		SpaceObject body = bodies.Find(f => f.GetInstanceID() == iD);
		foreach(SpaceObject body in bodies.ToArray())
		{
//			Debug.Log (iD + " " + body.transform.GetInstanceID());
			if(body.transform.GetInstanceID() == iD)
			{
//				Debug.Log ("Bodies match");
				bodies.Remove(body);

				Destroy(body.gameObject);
				finish--;

				return;
			}
		}

//		Debug.LogError("No object of that ID found");

	}

	public void removePlayer()
	{
		Destroy(bodies[0]);
		bodies.RemoveAt(0);

		closestPlanetToPlayer = null;
	}

	int stringCheck(string value)
	{
		if(value != "")
		{
			if(value.StartsWith("-") && value.Length == 1)
		  	{
				return 0;
			}
			else
			{
				return int.Parse(value);
			}
		}
		else
		{
			return 0;
		}
	}
}
	
