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
	public static float gForceAmp = 200;

	//Prefab used for spawning planets
	public SpaceObject planetTemplate;
	public SpaceObject sunTemplate;
	public SpaceObject ringTemplate;
	public SpaceObject BlackHoleTemplate;
	public PlayerCam camPlayerTemplate;

	//erence to player
	public SpaceObject player;

	//erence for Centre of mass position
	public GameObject CentreOfMass;
	public GameObject spawningPosition;
	public GameObject spawningVeloctiy;

	public GameObject SpawnPosSphere, SpawnPosCylinder;

	//Scale the planets to make them larger
	public float massRange;

	public static float planetSizeScale = 1000;

	public float timeScale = 1;

	//erence to PlayerCam on planet;
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
	string spawnDiameter = "1";
	string spawnSpeedMultiplier = "1";

	string spawnPosRange = "100";
	string spawnMassRange = "50";
	string spawnNumberOfPlanets = "50";

	string spawnRingWidth = "0";
	string spawnTotal = "1";
	string spawnDistance = "1";

	Vector3 spawnPos;
	float orbitMultiplier;

	BodyType spawnBType = BodyType.planet;

	public static bool choosingOrbitTarget;

	SpaceObject spawnOrbitTarget;

	List<SpaceObject> bodies;
	public List<SpaceObject> inVoid; //Bodies that have been placed into void via wormHole
	
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

	int fB, sB;
	int cB = 0;

	int start = 0, 
		finish, 
		step = 50;

	int fStart = 0, sStart = 0, comparisons = 0;

	// Use this for initialization
	void Start () 
	{
		Object[] temp = Resources.LoadAll("");

		SpawnPosSphere = temp[1] as GameObject;
		SpawnPosCylinder = temp[0] as GameObject;

		bodies = new List<SpaceObject>();
		inVoid = new List<SpaceObject>();

		spawnBody("Sun", BodyType.Sun, Vector3.zero, PStats.SunMass, PStats.SunDiam, null, 1);

		spawnBody("Mercury", BodyType.planet, new Vector3(PStats.MercuryDist, 0, 0), PStats.MercuryMass, PStats.MercuryDiam, bodies[0], 1);

		spawnBody("Venus", BodyType.planet, new Vector3(PStats.VenusDist, 0, 0), PStats.VenusMass, PStats.VenusDiam, bodies[0], 1);

		spawnBody("Earth", BodyType.planet, new Vector3(PStats.EarthDist, 0, 0), PStats.EarthMass, PStats.EarthDiam, bodies[0], 1);

		spawnBody("Moon", BodyType.planet, new Vector3(PStats.MoonDist, 0, 0), PStats.MoonMass, PStats.MoonDiam, bodies[bodies.Count-1], 10);

		spawnBody("Mars", BodyType.planet, new Vector3(PStats.MarsDist, 0, 0), PStats.MarsMass, PStats.MarsDiam, bodies[0], 1);

		spawnBody("Deimos", BodyType.planet, new Vector3(PStats.DeimosDist, 0, 0), PStats.DeimosMass, PStats.DeimosDiam, bodies[bodies.Count-1], 5);

		spawnBody("Jupiter", BodyType.planet, new Vector3(PStats.JupiterDist, 0, 0), PStats.JupiterMass, PStats.JupiterDiam, bodies[0], 1);

		spawnBody("Saturn", BodyType.planet, new Vector3(PStats.SaturnDist, 0, 0), PStats.SaturnMass, PStats.SaturnDiam, bodies[0], 1);
		
		spawnBody("Uranus", BodyType.planet, new Vector3(PStats.UranusDist, 0, 0), PStats.UranusMass, PStats.UranusDiam, bodies[0], 1);

		spawnBody("Neptune", BodyType.planet, new Vector3(PStats.NeptuneDist, 0, 0), PStats.NeptuneMass, PStats.NeptuneDiam, bodies[0], 1);

		spawnBody("Pluto", BodyType.planet, new Vector3(PStats.PlutoDist, 0, 0), PStats.PlutoMass, PStats.PlutoDiam, bodies[0], 1);

		spawnRing("AstBelt", Vector3.zero, PStats.AstMass, PStats.EarthDiam, bodies[0], 1, 336, 100, 150);

		spawnRing("JupitersBelt", new Vector3(PStats.JupiterDist, 0, 0), PStats.AstMass, PStats.EarthDiam, bodies[7], 3, 30, 25, 10);

		//Saturns Rings
		spawnRing("Ring1", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 1, 6, 30, 0);
		
		spawnRing("Ring2", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 1, 6.5f, 30, 0);
		
		spawnRing("Ring3", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 1, 7, 30, 0);
		
		spawnRing("Ring4", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 1, 7.5f, 30, 0);
		
		spawnRing("Ring5", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 1, 8, 30, 0);
		
		spawnRing("Ring6", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 1, 8.5f, 30, 0);

		spawnRing("Ring7", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 1, 9, 30, 0);

		spawnRing("Ring8", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 1, 9.5f, 30, 0);

		spawnRing("Ring9", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 1, 10, 30, 0);

		spawnRing("Ring10", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 1, 10.5f, 30, 0);
		
		spawnRing("UranusRings", new Vector3(PStats.UranusDist, 0, 0), PStats.AstMass, PStats.EarthDiam, bodies[9], 50, 40, 50, 30);

		spawnRing("NeptuneRings", new Vector3(PStats.NeptuneDist, 0, 0), PStats.AstMass, PStats.EarthDiam, bodies[10], 50, 30, 50, 20);

		
//		foreach(SpaceObject sO in bodies)
//		{
//			sO.rigidbody.AddForce(0,0, sO.avgOrbitVelocity);
//		}

		Time.timeScale = timeScale;

//		if(bodies.Count < step)
//		{
//			finish = bodies.Count;
//		}
//		else
//		{
//			finish = step;
//		}
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

	void bodyType(BodyType bType, Vector3 pos)
	{

	}

	void spawnBody(string _Name, BodyType bType, Vector3 pos, float mass, float diam, SpaceObject _OrbitTarget, 
	               float orbitPeriod)
	{
		switch(bType)
		{
		case BodyType.planet:
			
			bodies.Add(Instantiate(planetTemplate, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity) 
			           as SpaceObject);	
			
			break;
			
		case BodyType.Sun:
			
			bodies.Add(Instantiate(sunTemplate, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity) 
			           as SpaceObject);	
			
			break;
			
		case BodyType.Ring:

			Debug.Log ("nope");
			
			break;

		case BodyType.BlackHole:

			Debug.Log ("Spawning Black Hole");
			bodies.Add(Instantiate(BlackHoleTemplate, pos, Quaternion.identity) as SpaceObject);
			break;
		}

		if(bType != BodyType.Ring)
		{
			bodies[bodies.Count-1].init(_Name, bType, mass, diam, _OrbitTarget, orbitPeriod);
		}
	}

	//Spawn body from camera at speed
	void spawnBody(float mass, float diam, float velocity)
	{
		bodies.Add(Instantiate(planetTemplate, Camera.main.transform.position, Quaternion.identity) 
		           as SpaceObject);	

		bodies[bodies.Count-1].init("Thing", BodyType.planet, mass, diam, velocity);

	}

	void spawnRing(string _Name, Vector3 orbit, float mass, float diam, SpaceObject _OrbitTarget, float orbitPeriod, 
	               float distance, float amount, float ringWidth)
	{
		for(int i = 0; i < amount; i ++)
		{
			randAngle = Random.value * 360;
			
			//				Debug.Log (randAngle);
			
			randDist = Random.Range(0, ringWidth*2) - ringWidth;
			
			xPos = (distance + randDist) * Mathf.Cos(randAngle);
			
			//						Debug.Log (xPos);
			
			zPos = (distance + randDist) * Mathf.Sin(randAngle);
			
			//						Debug.Log (zPos);
			
			bodies.Add(Instantiate(ringTemplate, orbit + new Vector3(xPos, 0, zPos), Quaternion.identity) 
			           as SpaceObject);
			
			bodies[bodies.Count-1].init(_Name, BodyType.Ring, mass, diam, _OrbitTarget, orbitPeriod);
		}
	}

//	void addAsteroid(Vector3 pos, float _Distance)
//	{
//		
//		//				Debug.Log (randAngle);
//
//		if(_Distance == 0)
//		{
//			randDist = Random.Range(0,_Distance/2) - (_Distance)/4;
//		}
//		else
//		{
//			randDist = Random.Range(0, _Distance);
//		}
//
//		xPos = (_Distance + randDist) * Mathf.Cos(randAngle);
//		
////						Debug.Log (xPos);
//		
//		zPos = (_Distance + randDist) * Mathf.Sin(randAngle);
//		
////						Debug.Log (zPos);
//		
//		bodies.Add(Instantiate(ringTemplate, pos + new Vector3(xPos, 0, zPos), Quaternion.identity) 
//		           as SpaceObject);
//	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Y))
		{
			Debug.Log ("Spawning Black Hole");

			spawnBody("BlackHole", BodyType.BlackHole, Vector3.up*100, 1000000, PStats.JupiterDiam, null, 0);

//			addAsteroid(PStats.ABelt);

//			bodies[bodies.Count-1].init(name, BodyType.Ring, PStats.AstMass, PStats.EarthDiam, bodies[0], 50);
		}

		if(Input.GetKeyDown(KeyCode.F))
		{
			Debug.Log ("Spawning");
			spawnBody(100, PStats.NeptuneDiam, 150000);

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
					Destroy(body.gameObject);
				}

				bodies.Clear();
			}
		}

		if(Input.GetKeyDown(KeyCode.C))
		{
			if(CentreOfMass.activeSelf)
			{
				CentreOfMass.SetActive(false);
			}
			else
			{
				CentreOfMass.SetActive(true);
			}
		}

		spawningPosition.SetActive(spawningPlanet);
//		spawningVeloctiy.SetActive(spawningPlanet);
		
		if(spawningPlanet)
		{
			orbitMultiplier = stringCheck(spawnSpeedMultiplier);

			switch(spawnBType)
			{
			case BodyType.planet:
			case BodyType.Sun:

				spawningPosition.GetComponent<MeshFilter>().sharedMesh = SpawnPosSphere.GetComponent<MeshFilter>().sharedMesh;

				if(spawnBType == BodyType.planet)
				{
					spawningPosition.renderer.material.color = new Color(0,1,0,0.5f);
				}
				else
				{
					spawningPosition.renderer.material.color = new Color(1,0.3f,0,0.5f);

				}

				spawnPos.x = stringCheck(spawnPosX);
				spawnPos.y = stringCheck(spawnPosY);
				spawnPos.z = stringCheck(spawnPosZ);
				
				spawningPosition.transform.position = spawnPos;

				float val = stringCheck(spawnDiameter);
				
				spawningPosition.transform.localScale = new Vector3(val, val, val);

				spawningVeloctiy.transform.localScale = new Vector3(0.1f, orbitMultiplier, 0.1f);
				
				spawningVeloctiy.transform.position = spawnPos;
				
				spawningVeloctiy.transform.rotation = Quaternion.FromToRotation(Vector3.up, new Vector3(orbitMultiplier, 0,0));

				break;

			case BodyType.Ring:

				spawningPosition.GetComponent<MeshFilter>().sharedMesh = SpawnPosCylinder.GetComponent<MeshFilter>().sharedMesh;

				if(spawnOrbitTarget)
				{
					spawningPosition.transform.position = spawnOrbitTarget.transform.position + Vector3.right * stringCheck(spawnDistance);
				}
				else
				{
					spawningPosition.transform.position = Vector3.zero;
				}

				spawningPosition.transform.eulerAngles = Vector3.forward*90;	
				spawningPosition.transform.localScale = new Vector3(0.5f, stringCheck(spawnDistance), 0.5f);

				spawningPosition.renderer.material.color = new Color(0,1,0,0.5f);

				break;
			}
		}

		if(Input.GetMouseButtonDown(0) && choosingOrbitTarget)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit))
			{
				spawnOrbitTarget = hit.transform.GetComponent<SpaceObject>();
				choosingOrbitTarget = false;
			}
		}
	}

	void OnGUI()
	{
		AbsorbOnCollision.absorbOn = GUI.Toggle(new Rect(0, Screen.height - sH, sW * toggleAbsorb.Length, sH), AbsorbOnCollision.absorbOn, toggleAbsorb);

		GUI.Label(new Rect(Screen.width/2, 0, 200, 30), "Total bodies: " + bodies.Count);

		if(choosingOrbitTarget)
		{
			GUI.Label(new Rect(Screen.width/2, 40, 200, 30), "Select orbit target");
		}

		if(Time.timeScale == 0)
		{
			GUI.Label(new Rect(Screen.width/2, Screen.height/2, 120, 22), "Simulation Paused");
		}

		#region spawning single plant

		Rect spawningPos;

		if(spawningPlanet)
		{
			spawningPos = new Rect(Screen.width - 220, 4, 220, 224);
			spawnButton = "<";
		}
		else
		{
			spawningPos = new Rect(Screen.width - 18, 4, 220, 200);
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

		//Single planet GUI
		GUI.BeginGroup(new Rect(18, 0, 220, 224));

		GUI.Box(new Rect(0, 0, 220, 224), "Body Spawning options");

		if(GUI.Button(new Rect(0, 22, 50, 22), "Planet"))
		{
			spawnBType =  BodyType.planet;
		}

		if(GUI.Button(new Rect(52, 22, 50, 22), "Sun"))
		{
			spawnBType = BodyType.Sun;
		}
		
		if(GUI.Button(new Rect(102, 22, 50, 22), "Ring"))
		{
			spawnBType = BodyType.Ring;
		}

		GUI.Label(new Rect(154, 22, 40, 22), getBodyType(spawnBType));

		switch(spawnBType)
		{
		case BodyType.planet:

			GUI.Label(new Rect(0, 44, 160, 22), "Position");

			GUI.Label(new Rect(0, 66, 22, 22), "X: ");
			spawnPosX = GUI.TextField(new Rect(14, 66, 36, 22), spawnPosX);

			GUI.Label(new Rect(56, 66, 22, 22), "Y: ");
			spawnPosY = GUI.TextField(new Rect(70, 66, 36, 22), spawnPosY);

			GUI.Label(new Rect(112, 66, 22, 22), "Z: ");
			spawnPosZ = GUI.TextField(new Rect(126, 66, 36, 22), spawnPosZ);

			break;

		case BodyType.Sun:

			GUI.Label(new Rect(0, 44, 160, 22), "Position");
			
			GUI.Label(new Rect(0, 66, 22, 22), "X: ");
			spawnPosX = GUI.TextField(new Rect(14, 66, 36, 22), spawnPosX);
			
			GUI.Label(new Rect(56, 66, 22, 22), "Y: ");
			spawnPosY = GUI.TextField(new Rect(70, 66, 36, 22), spawnPosY);
			
			GUI.Label(new Rect(112, 66, 22, 22), "Z: ");
			spawnPosZ = GUI.TextField(new Rect(126, 66, 36, 22), spawnPosZ);

			break;

		case BodyType.Ring:

			GUI.Label(new Rect(0, 44, 70, 22), "Ring Width");
			spawnRingWidth = GUI.TextField(new Rect(70, 44, 36, 22), spawnRingWidth);

			GUI.Label(new Rect(112, 44, 50, 22), "Total");
			spawnTotal = GUI.TextField(new Rect(162, 44, 36, 22), spawnTotal);

			GUI.Label(new Rect(0, 66, 70, 22), "Distance: ");
			spawnDistance = GUI.TextField(new Rect(70, 66, 36, 22), spawnDistance);

			break;
		}

		GUI.Label(new Rect(0, 88, 160, 22), "Orbit Speed multiplier");

		spawnSpeedMultiplier = GUI.TextField(new Rect(0, 110, 72, 22), spawnSpeedMultiplier);

		GUI.Label(new Rect(0, 132, 60, 22), "Mass");
		spawnMass = GUI.TextField(new Rect(62, 132, 72, 22), spawnMass);

		GUI.Label(new Rect(0, 154, 60, 22), "Diameter");
		spawnDiameter = GUI.TextField(new Rect(62, 154, 72, 22), spawnDiameter);
		
		if(GUI.Button(new Rect(0, 176, 130, 22), "Select Orbit Target"))
		{
			if(bodies.Count > 0 && !choosingOrbitTarget)
			{
		   		choosingOrbitTarget = true;
			}
			else
			{
				choosingOrbitTarget = false;
			}
		}

		if(spawnOrbitTarget)
		{
			GUI.Label(new Rect(132, 176, 75, 22), spawnOrbitTarget.name);
		}
		else
		{
			GUI.Label(new Rect(132, 176, 75, 22), "None");
		}

		if(GUI.Button(new Rect(0, 198, 60, 22), "Spawn"))
		{
			switch(spawnBType)
			{
			case BodyType.planet:

				spawnBody("body" + (bodies.Count-1), spawnBType, spawnPos, int.Parse(spawnMass), float.Parse(spawnDiameter), 
			          	spawnOrbitTarget, float.Parse(spawnSpeedMultiplier)); 
				break;

			case BodyType.Sun:

				spawnBody("body" + (bodies.Count-1), spawnBType, spawnPos, int.Parse(spawnMass), float.Parse(spawnDiameter), 
				          spawnOrbitTarget, float.Parse(spawnSpeedMultiplier)); 

				break;

			case BodyType.Ring:

				if(spawnOrbitTarget)
				{
					spawnRing("rings", spawnOrbitTarget.transform.position, float.Parse(spawnMass), 
					          PStats.EarthDiam, spawnOrbitTarget, float.Parse(spawnSpeedMultiplier),
					          float.Parse(spawnDistance), float.Parse(spawnTotal), float.Parse(spawnRingWidth));
				}
				else
				{
					spawnRing("rings", Vector3.zero, float.Parse(spawnMass), 
					          float.Parse(spawnDiameter), null, float.Parse(spawnSpeedMultiplier),
					          float.Parse(spawnDistance), float.Parse(spawnTotal), float.Parse(spawnRingWidth));
				}

				break;
			}
		}

		GUI.EndGroup();

		GUI.EndGroup();

		#endregion

		#region spawning mutliple planets

		Rect spawningMultPos;
		
		if(spawningMultPlanets)
		{
			spawningMultPos = new Rect(Screen.width - 226, 226, 224, 200);
			spawnMultButton = "<";
		}
		else
		{
			spawningMultPos = new Rect(Screen.width - 18, 226, 224, 200);
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

		for(fB = fStart; fB < bodies.Count; fB++)
		{
			if(fB >= bodies.Count)
			{
				fB = bodies.Count-1;
			}

			if(!bodies[fB])
			{
				bodies.Remove(bodies[fB]);
				Debug.LogWarning("bodies[fB] is null");
				break;
			}		

			if(bodies[fB].name == "Earth")
			{

			}

//			fB = Mathf.Clamp(fB, 0, bodies.Count);

//			bodies[fB].rigidbody.mass = Mathf.Clamp(bodies[fB].rigidbody.mass, 0, SpaceObject.maxMass);

			COMPos.x += bodies[fB].rigidbody.mass * bodies[fB].transform.position.x;
			COMPos.y += bodies[fB].rigidbody.mass * bodies[fB].transform.position.y;
			COMPos.z += bodies[fB].rigidbody.mass * bodies[fB].transform.position.z;

			totalMass += bodies[fB].rigidbody.mass;

			for(sB = sStart; sB < bodies.Count; sB++)
			{
//				sB = Mathf.Clamp(sB, 0, bodies.Count-1);

				if(sB >= bodies.Count)
				{
					sB = bodies.Count-1;
				}

				if(!bodies[sB])
				{
					bodies.Remove(bodies[sB]);
					Debug.LogWarning("bodies[sB] is null");
					break;
				}

				if(fB < sB && (bodies[fB].canOrbit(bodies[sB], relDistance) 
				               || bodies[sB].canOrbit(bodies[fB], relDistance)))
				{	
					comparisons ++;

					deltaPosition = bodies[sB].transform.position - bodies[fB].transform.position;
					//Debug.Log ("deltaPosition: " + deltaPosition.ToString());

//					if(deltaPosition.sqrMagnitude > (bodies[fB].transform.localScale.x/2 + bodies[sB].transform.localScale.x/2))
//					{
						direction = Vector3.Normalize(deltaPosition);
						//Debug.Log("direction: " + direction.ToString());
						
						//Find the position of the edge of the body to draw the ray
//						edgeOfFirstBody = direction * (bodies[fB].transform.localScale.x/2);
						//Debug.DrawRay(bodies[fB].transform.position + edgeOfFirstBody, direction, Color.red);
						
						//Find the position of the edge of the body for the second body;
//						edgeOfSecondBody = direction * (bodies[sB].transform.localScale.x/2);
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
						
						if(bodies[fB].canOrbit(bodies[sB], relDistance))
						{
							if(bodies[fB].orbitTarget && bodies[fB].orbitTarget == bodies[sB])
							{
								bodies[fB].directionToOrbitTarget = direction;
							}
							bodies[fB].rigidbody.AddForce(direction * forceDueToGrav * Time.deltaTime);
						}


						if(bodies[sB].canOrbit(bodies[fB], relDistance))
						{
							if(bodies[sB].orbitTarget && bodies[sB].orbitTarget == bodies[fB])
							{
								bodies[sB].directionToOrbitTarget = direction * -1;
							}

							bodies[sB].rigidbody.AddForce(direction * -1 * forceDueToGrav * Time.deltaTime);
						}
//					}
//					else
//					{
////						Debug.Log("Error");
//					}

					if(comparisons >= step)
					{
//						Debug.Log ("Exiting second loop");

						fStart = fB;
						sStart = sB;

//						Debug.Log (fStart);
//						Debug.Log (sStart);

						comparisons = 0;

						return;
					}
				}
			}

			sStart = 0;

//			start = finish;
//
//			if(finish >= bodies.Count) //If at the end of list
//			{
//				start = 0;
//
//				if(bodies.Count < step)
//				{
//					finish = bodies.Count;
//				}
//				else
//				{
//					finish = step;
//				}
//			}
//			else
//			{
//				if(finish + step < bodies.Count)
//				{
//					finish += step;
//				}
//				else
//				{
//					finish = bodies.Count;
//				}
//			}
		}

		fStart = 0;

		if(bodies.Count > 0 && totalMass != 0)
		{
			COMPos /= totalMass;

			CentreOfMass.transform.localScale = new Vector3(totalMass, totalMass, totalMass)/100;
			CentreOfMass.transform.position = COMPos;
		}
		else
		{
			CentreOfMass.SetActive(false);
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
		foreach(SpaceObject body in bodies.ToArray())
		{
			if(body.transform.GetInstanceID() == iD)
			{
//				Debug.Log ("Bodies match");
				bodies.Remove(body);

				Destroy(body.GetComponent<SpaceObject>());

				body.gameObject.AddComponent<DestroySlowly>();

//				finish--;

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

	float stringCheck(string value)
	{
		if(value != "")
		{
			if((value.StartsWith("-") || value.StartsWith("0.") || value.StartsWith(".")) && value.Length == 1)
		  	{
				return 0;
			}
			else
			{
				float result;
				if(float.TryParse(value, out result))
				{
					return result;
				}
				else
				{
					return 0;
				}
			}
		}
		else
		{
			return 0;
		}
	}

	string getBodyType(BodyType bType)
	{
		switch(bType)
		{
		case BodyType.planet:

			return "Planet";

		case BodyType.Sun:

			return "Sun";

		case BodyType.Ring:

			return "Ring";

		default:

			Debug.LogError("No body type found");

			return "null";
		}
	}
}
	
