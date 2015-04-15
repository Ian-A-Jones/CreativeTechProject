//Manager script that looks after all space objects
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceManager : MonoBehaviour 
{
	//Gravity Scalar
	public static float gForceAmp = 500;

	//Templates for spaceObjects that can be spawned
	SpaceObject planetTemplate;
	SpaceObject sunTemplate;
	SpaceObject ringTemplate;
	SpaceObject BlackHoleTemplate;

	//Objects used for Centre of Mass and spawning reference
	GameObject CentreOfMass;
	GameObject spawningPosition;
	GameObject spawningVeloctiy;

	//GameObjects to change shape of spawningPositiong Reference
	GameObject SpawnPosSphere, SpawnPosCylinder;

	//Variables for dealing with Centre of Mass
	Vector3 COMPos = Vector3.zero;
	float totalMass = 0;

	//Can the user pause the sim - Used to prevent unpausing when loading in the Sim
	bool canPause = true;
	
	//Whether user is spawning a planet, multiple planets or adjusting planet firing variables
	bool spawingBody = false;
	bool spawningMultPlanets = false;
	bool adjustingPlanetFiring = false;

	//Single spaceObject spawning variables
	Vector3 spawnPos;
	float orbitMultiplier;
	BodyType spawnBType = BodyType.planet;
	SpaceObject spawnOrbitTarget;

	//Variables for spawning Rings
	float randAngle;
	float randDist;
	float xPos;
	float zPos;

	//Multiple planet Spawning variables
	int planetsToSpawn;	
	float posRange;
	float massRange;
	
	#region Variables for the GUI elements
	//Strings for GUI

	string spawnButton = "";
	string spawnMultButton = "";
	string pFiringButton = "";

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

	string spawnPFVelocity = "1";
	string spawnPFMass = "1";
	string spawnPFDiameter = "1";

	#endregion

	//Whether an orbit target is being choosing using mouse to prevent camera from swapping targets
	public static bool choosingOrbitTarget;

	//Actual list of spaceObjects to be iterated over
	List<SpaceObject> bodies;
	
	//Variables for Inter-planetary interaction
	Vector3 deltaPosition;
	Vector3 direction;	
	Vector3 edgeOfFirstBody;
	Vector3 edgeOfSecondBody;	
	float relDistance;
	float forceDueToGrav;

	int fB, sB;

	//Varibales for looping through module in stages
	int step = 50, fStart = 0, sStart = 0, comparisons = 0;

	//Height and relating to strings
	float sH = 22;

	public Object[] temp;

	// Use this for initialization
	void Start () 
	{
		//Load and assign appropriate assets
		temp = Resources.LoadAll("SpaceManager");

		BlackHoleTemplate = ((GameObject)temp[0]).GetComponent<SpaceObject>();
		CentreOfMass = Instantiate(temp[1]) as GameObject;
		planetTemplate = ((GameObject)temp[2]).GetComponent<SpaceObject>();
		ringTemplate = ((GameObject)temp[3]).GetComponent<SpaceObject>();
		spawningPosition = Instantiate(temp[4]) as GameObject;
		spawningVeloctiy = Instantiate(temp[5]) as GameObject;
		SpawnPosCylinder = temp[6] as GameObject;
		SpawnPosSphere = temp[7] as GameObject;
		sunTemplate = ((GameObject)temp[8]).GetComponent<SpaceObject>();

		bodies = new List<SpaceObject>(); //Init List

		GameObject.Find("COMCam").GetComponent<COMCamera>().target = CentreOfMass;

		GameObject.Find("COMCam").gameObject.SetActive(false);

		//Example of our Solar System
		spawnBody("Sun", BodyType.Sun, Vector3.zero, PStats.SunMass, PStats.SunDiam, null, 1);

		spawnBody("Mercury", BodyType.planet, new Vector3(PStats.MercuryDist, 0, 0), PStats.MercuryMass, PStats.MercuryDiam, bodies[0], 1);

		spawnBody("Venus", BodyType.planet, new Vector3(PStats.VenusDist, 0, 0), PStats.VenusMass, PStats.VenusDiam, bodies[0], 1);

		spawnBody("Earth", BodyType.planet, new Vector3(PStats.EarthDist, 0, 0), PStats.EarthMass, PStats.EarthDiam, bodies[0], 1);

		spawnBody("Moon", BodyType.planet, new Vector3(PStats.MoonDist, 0, 0), PStats.MoonMass, PStats.MoonDiam, bodies[bodies.Count-1], 20);

		spawnBody("Mars", BodyType.planet, new Vector3(PStats.MarsDist, 0, 0), PStats.MarsMass, PStats.MarsDiam, bodies[0], 1);

		spawnBody("Deimos", BodyType.planet, new Vector3(PStats.DeimosDist, 0, 0), PStats.DeimosMass, PStats.DeimosDiam, bodies[bodies.Count-1], 5);

		spawnBody("Jupiter", BodyType.planet, new Vector3(PStats.JupiterDist, 0, 0), PStats.JupiterMass, PStats.JupiterDiam, bodies[0], 1);

		spawnBody("Saturn", BodyType.planet, new Vector3(PStats.SaturnDist, 0, 0), PStats.SaturnMass, PStats.SaturnDiam, bodies[0], 1);
		
		spawnBody("Uranus", BodyType.planet, new Vector3(PStats.UranusDist, 0, 0), PStats.UranusMass, PStats.UranusDiam, bodies[0], 1);

		spawnBody("Neptune", BodyType.planet, new Vector3(PStats.NeptuneDist, 0, 0), PStats.NeptuneMass, PStats.NeptuneDiam, bodies[0], 1);

		spawnBody("Pluto", BodyType.planet, new Vector3(PStats.PlutoDist, 0, 0), PStats.PlutoMass, PStats.PlutoDiam, bodies[0], 1);

		spawnRing("AstBelt", Vector3.zero, PStats.AstMass, PStats.EarthDiam, bodies[0], 1, 336, 50, 150);

		spawnRing("JupitersBelt", new Vector3(PStats.JupiterDist, 0, 0), PStats.AstMass, PStats.EarthDiam, bodies[7], 3, 30, 25, 10);
		//Saturns Rings
		spawnRing("Ring1", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 10, 6, 30, 0);	
		spawnRing("Ring2", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 10, 6.5f, 30, 0);
		spawnRing("Ring3", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 10, 7, 30, 0);
		spawnRing("Ring4", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 10, 7.5f, 30, 0);
		spawnRing("Ring5", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 10, 8, 30, 0);
		spawnRing("Ring6", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 10, 8.5f, 30, 0);
		spawnRing("Ring7", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 10, 9, 30, 0);
		spawnRing("Ring8", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 10, 9.5f, 30, 0);
		spawnRing("Ring9", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 10, 10, 30, 0);
		spawnRing("Ring10", new Vector3(PStats.SaturnDist, 0, 0), PStats.AstMass, PStats.MoonDiam, bodies[8], 10, 10.5f, 30, 0);
		
		spawnRing("UranusRings", new Vector3(PStats.UranusDist, 0, 0), PStats.AstMass, PStats.EarthDiam, bodies[9], 50, 40, 25, 30);

		spawnRing("NeptuneRings", new Vector3(PStats.NeptuneDist, 0, 0), PStats.AstMass, PStats.EarthDiam, bodies[10], 50, 30, 25, 20);
	}	

	//Function for spawning bodies without causing Unity to freeze
	IEnumerator spawnBodies()
	{
		//Pause Sim
		Time.timeScale = 0;

		//Prevent user from unpauing sim
		canPause = false;
	
		for(int i = 0; i < planetsToSpawn; i++)
		{
			//Random position within range
			float randX = Random.Range(-posRange, posRange);
			float randY = Random.Range(-posRange, posRange);
			float randZ = Random.Range(-posRange, posRange);	

			//Random mass within range
			float randomMass = Random.Range(1, massRange);

			//Spawn planet with given stats
			spawnBody("body" + (bodies.Count), BodyType.planet, new Vector3(randX, randY, randZ), randomMass, 
			          randomMass/10, null, 1);

			if(i%10 == 0)
			{
				yield return null;
			}
		}

		//Start sim again
		Time.timeScale = 1;

		//Allow player to pause and unpause Sim again
		canPause = true;
	}

	//Method for spawning space Objects with multiple variables
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

	//Spawn ring system
	void spawnRing(string _Name, Vector3 orbit, float mass, float diam, SpaceObject _OrbitTarget, float orbitPeriod, 
	               float distance, float amount, float ringWidth)
	{
		for(int i = 0; i < amount; i ++)
		{
			//Find random angle
			randAngle = Random.value * 360;

			//Find rand distance within Range
			randDist = Random.Range(0, ringWidth*2) - ringWidth;

			//Use Trig to find X and Z position
			xPos = (distance + randDist) * Mathf.Cos(randAngle);

			zPos = (distance + randDist) * Mathf.Sin(randAngle);
			
			bodies.Add(Instantiate(ringTemplate, orbit + new Vector3(xPos, 0, zPos), Quaternion.identity) 
			           as SpaceObject);
			
			bodies[bodies.Count-1].init(_Name, BodyType.Ring, mass, diam, _OrbitTarget, orbitPeriod);
		}
	}

	void Update()
	{
		//Spawn body from cameras position using variables
		if(Input.GetKeyDown(KeyCode.F))
		{
			Debug.Log ("Spawning");
			spawnBody(stringCheck(spawnPFMass), stringCheck(spawnPFDiameter), stringCheck(spawnPFVelocity));
		}

		//Logic for pausing 
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

		//Restart sim
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

		//Toggle 
		if(Input.GetKeyDown(KeyCode.C))
		{
			CentreOfMass.SetActive(!CentreOfMass.activeSelf);
		}

		//Check variables of spawning GUI and move refernce objects for Spawn Position and veloctiy
		if(spawingBody)
		{
			orbitMultiplier = stringCheck(spawnSpeedMultiplier);

			//Activate Spawning Position and velocity
			spawningPosition.SetActive(true);
			spawningVeloctiy.SetActive(true);

			//Based on body type
			switch(spawnBType)
			{
			case BodyType.planet:
			case BodyType.Sun:

				//Assign Sphere mesh to position reference
				spawningPosition.GetComponent<MeshFilter>().sharedMesh = SpawnPosSphere.GetComponent<MeshFilter>().sharedMesh;

				//Make position reference Blue or Orange based on whether it's a Planet or Sun
				if(spawnBType == BodyType.planet)
				{
					spawningPosition.renderer.material.color = new Color(0,1,0,0.5f); //Blue
				}
				else
				{
					spawningPosition.renderer.material.color = new Color(1,0.3f,0,0.5f); //Orange

				}

				//Convert given position in GUI to appropriate values
				spawnPos.x = stringCheck(spawnPosX);
				spawnPos.y = stringCheck(spawnPosY);
				spawnPos.z = stringCheck(spawnPosZ);
				
				spawningPosition.transform.position = spawnPos;

				//Convert given diameter
				float diam = stringCheck(spawnDiameter);

				//Set scale based on Diameter
				spawningPosition.transform.localScale = new Vector3(diam, diam, diam);

				//Assign spawining value appropriate values
				spawningVeloctiy.transform.localScale = new Vector3(0.1f, orbitMultiplier, 0.1f);
				spawningVeloctiy.transform.position = spawnPos;
				spawningVeloctiy.transform.rotation = Quaternion.FromToRotation(Vector3.up, new Vector3(orbitMultiplier,
				                                                                                        0,0));
				break;

			case BodyType.Ring:

				//As orbit targets are nesecary to spawn Ring systems position only assign references if there is one
				if(spawnOrbitTarget)
				{
					//Assign cylinder mesh to better represent ring
					spawningPosition.GetComponent<MeshFilter>().sharedMesh = SpawnPosCylinder.GetComponent<MeshFilter>()
						.sharedMesh;
					
					//Rotate and Scale position reference to represent size of Ring system
					spawningPosition.transform.eulerAngles = Vector3.forward*90;	
					spawningPosition.transform.localScale = new Vector3(0.5f, stringCheck(spawnDistance), 0.5f);

					spawningPosition.transform.position = spawnOrbitTarget.transform.position;
					spawningVeloctiy.transform.position = spawningPosition.transform.position + Vector3.right * 
						spawningPosition.transform.localScale.y;

					spawningVeloctiy.transform.localScale = new Vector3(0.5f, stringCheck(spawnRingWidth)+1, 0.5f);
					
					spawningPosition.renderer.material.color = new Color(0,1,0,0.5f);
				}
				else
				{
					spawningPosition.SetActive(false);
					spawningVeloctiy.SetActive(false);
				}

				break;

			case BodyType.BlackHole:

				//Only need position for spawning Blackhole
				spawningPosition.SetActive(true);
				spawningVeloctiy.SetActive(false);

				spawningPosition.GetComponent<MeshFilter>().sharedMesh = SpawnPosSphere.GetComponent<MeshFilter>().
					sharedMesh;

				spawningPosition.renderer.material.color = new Color(0,0,1,0.5f);

				spawnPos.x = stringCheck(spawnPosX);
				spawnPos.y = stringCheck(spawnPosY);
				spawnPos.z = stringCheck(spawnPosZ);
				
				spawningPosition.transform.position = spawnPos;

				spawningPosition.transform.localScale = new Vector3(PStats.JupiterDiam, 
				                                                    PStats.JupiterDiam, 
				                                                    PStats.JupiterDiam);
				break;
			}
		}
		else
		{
			spawningPosition.SetActive(false);
			spawningVeloctiy.SetActive(false);
		}

		//Using mouse click to select orbit Target 
		if(Input.GetMouseButtonDown(0) && choosingOrbitTarget)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit) && hit.collider.tag == "Body")
			{
				spawnOrbitTarget = hit.transform.GetComponent<SpaceObject>();
				choosingOrbitTarget = false;
			}
		}

		//Reset Centre of mass
		COMPos = Vector3.zero;
		totalMass = 0;
		
		//Loop through list of bodies to calculate COM
		//Do this as a seperate loop to forces of gravity as that is done in steps
		for(fB = 0; fB < bodies.Count; fB++)
		{
			//Add current body to centre of Mass calculation
			COMPos.x += bodies[fB].rigidbody.mass * bodies[fB].transform.position.x;
			COMPos.y += bodies[fB].rigidbody.mass * bodies[fB].transform.position.y;
			COMPos.z += bodies[fB].rigidbody.mass * bodies[fB].transform.position.z;
			
			totalMass += bodies[fB].rigidbody.mass;
		}
		
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

	void OnGUI()
	{
		//How many bodies are in sim
		GUI.Label(new Rect(Screen.width/2, 0, 200, sH), "Total bodies: " + bodies.Count);

		//Message to inform user to select Orbit target
		if(choosingOrbitTarget)
		{
			GUI.Label(new Rect(Screen.width/2, 40, 200, sH), "Select orbit target");
		}

		//Mesage to inform user if Sim is paused
		if(Time.timeScale == 0)
		{
			GUI.Label(new Rect(Screen.width/2, Screen.height/2, 120, sH), "Simulation Paused");
		}

		//Various GUI elements for spawning single planet
		#region spawning single plant

		Rect spawningPos;

		if(spawingBody)
		{
			spawningPos = new Rect(Screen.width - 260, 4, 280, 224);
			spawnButton = ">";
		}
		else
		{
			spawningPos = new Rect(Screen.width - 18, 4, 260, 200);
			spawnButton = "<";
		}

		GUI.BeginGroup(spawningPos);

		if(GUI.Button(new Rect(0,0, 18, sH), spawnButton))
		{
			if(spawingBody)
			{
				spawingBody = false;
			}
			else
			{
				spawingBody = true;
			}
		}

		//Single planet GUI
		GUI.BeginGroup(new Rect(18, 0, 280, 195));

		GUI.Box(new Rect(0, 0, 280, 224), "Body Spawning options");

		if(GUI.Button(new Rect(0, sH, 50, sH), "Planet"))
		{
			spawnBType =  BodyType.planet;
		}

		if(GUI.Button(new Rect(52, sH, 35, sH), "Sun"))
		{
			spawnBType = BodyType.Sun;
		}
		
		if(GUI.Button(new Rect(89, sH, 40, sH), "Ring"))
		{
			spawnBType = BodyType.Ring;
		}

		if(GUI.Button(new Rect(131, sH, 56, sH), "B.Hole"))
		{
			spawnBType = BodyType.BlackHole;
		}

		GUI.Label(new Rect(190, sH, 40, sH), getBodyType(spawnBType));

		switch(spawnBType)
		{
		case BodyType.planet:
		case BodyType.Sun:
		case BodyType.BlackHole:

			GUI.Label(new Rect(0, sH*2, 52, sH), "Position:");

			GUI.Label(new Rect(54, sH*2, sH, sH), "X:");
			spawnPosX = GUI.TextField(new Rect(68, sH*2, 36, sH), spawnPosX);
		
			GUI.Label(new Rect(106, sH*2, sH, sH), "Y:");
			spawnPosY = GUI.TextField(new Rect(122, sH*2, 36, sH), spawnPosY);

			GUI.Label(new Rect(162, sH*2, sH, sH), "Z:");
			spawnPosZ = GUI.TextField(new Rect(178, sH*2, 36, sH), spawnPosZ);

			break;

		case BodyType.Ring:

			GUI.Label(new Rect(0, sH*2, 70, sH), "Ring Width");
			spawnRingWidth = GUI.TextField(new Rect(70, sH*2, 36, sH), spawnRingWidth);

			GUI.Label(new Rect(112, sH*2, 50, sH), "Total");
			spawnTotal = GUI.TextField(new Rect(162, sH*2, 36, sH), spawnTotal);

			GUI.Label(new Rect(0, sH*3, 70, sH), "Distance: ");
			spawnDistance = GUI.TextField(new Rect(70, sH*3, 36, sH), spawnDistance);

			break;
		}

		if(spawnBType != BodyType.BlackHole)
		{
			GUI.Label(new Rect(0, sH*4, 130, sH), "Orbit Speed multiplier");

			spawnSpeedMultiplier = GUI.TextField(new Rect(sH*6, sH*4, 68, sH), spawnSpeedMultiplier);

			GUI.Label(new Rect(0, 108, 60, sH), "Mass");
			spawnMass = GUI.TextField(new Rect(sH*6, 108, 68, sH), spawnMass);

			GUI.Label(new Rect(0, 128, 60, sH), "Diameter");
			spawnDiameter = GUI.TextField(new Rect(sH*6, 128, 68, sH), spawnDiameter);
			
			if(GUI.Button(new Rect(0, 148, 130, sH), "Select Orbit Target"))
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
				GUI.Label(new Rect(sH*6, 148, 75, sH), spawnOrbitTarget.name);
			}
			else
			{
				GUI.Label(new Rect(sH*6, 148, 75, sH), "None");
			}
		}

		if(GUI.Button(new Rect(0, 170, 60, sH), "Spawn"))
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
					Debug.Log (spawnDistance);

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

			case BodyType.BlackHole:

				spawnBody("Black Hole", BodyType.BlackHole, spawnPos, 1000000, PStats.JupiterDiam, null, 0);

				break;
			}
		}

		GUI.EndGroup();

		GUI.EndGroup();

		#endregion

		//Various GUI elements for spawning multiple Elements
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
		
		if(GUI.Button(new Rect(0,0, 18, sH), spawnMultButton))
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
		
		GUI.Label(new Rect(0, sH, 160, sH), "Total Planets");

		spawnNumberOfPlanets = GUI.TextField(new Rect(0, sH*2, 36, sH), spawnNumberOfPlanets);

		GUI.Label(new Rect(0, sH*3, 160, 22), "Position Range");

		spawnPosRange = GUI.TextField(new Rect(0, sH*4, 36, 22), spawnPosRange);

		GUI.Label(new Rect(0, sH*5, 60, 22), "Mass Range");
		spawnMassRange = GUI.TextField(new Rect(0, sH*6, 72, 22), spawnMassRange);
		
		if(GUI.Button(new Rect(0, 154, 120, sH), "Spawn Planets"))
		{
			planetsToSpawn = int.Parse(spawnNumberOfPlanets);
			posRange = int.Parse(spawnPosRange);
			massRange = stringCheck(spawnMassRange);
			StartCoroutine("spawnBodies");
		}
		
		GUI.EndGroup();
		
		GUI.EndGroup();

		#endregion

		//Various GUI elements for adjusting variable of planet that is fired
		#region adjusting variables for planet firing
		
		Rect pFiringOptPos;
		
		if(adjustingPlanetFiring)
		{
			pFiringOptPos = new Rect(Screen.width - 226, 448, 224, 200);
			pFiringButton = "<";
		}
		else
		{
			pFiringOptPos = new Rect(Screen.width - 18, 448, 224, 200);
			pFiringButton = ">";
		}
		
		GUI.BeginGroup(pFiringOptPos);
		
		if(GUI.Button(new Rect(0,0, 18, sH), pFiringButton))
		{
			adjustingPlanetFiring = !adjustingPlanetFiring;
		}
		
		GUI.BeginGroup(new Rect(18, 0, 200, 200));
		
		GUI.Box(new Rect(0, 0, 200, 100), "Planet firing options");

		GUI.Label(new Rect(0, sH, 130, sH), "Velocity");	
		spawnPFVelocity = GUI.TextField(new Rect(sH*6, sH, 68, sH), spawnPFVelocity);
		
		GUI.Label(new Rect(0, sH*2, 60, sH), "Mass");
		spawnPFMass = GUI.TextField(new Rect(sH*6, sH*2, 68, sH), spawnPFMass);
		
		GUI.Label(new Rect(0, sH*3, 60, sH), "Diameter");
		spawnPFDiameter = GUI.TextField(new Rect(sH*6, sH*3, 68, sH), spawnPFDiameter);
		
		GUI.EndGroup();
		
		GUI.EndGroup();
		
		#endregion
	}
	
	void FixedUpdate () 
	{
		optimisedFDTG();
	}

	void optimisedFDTG()
	{
		//Handling of null list
		if(bodies == null)
		{
			Debug.LogWarning("List is null");
			return;
		}

		//From start to Finish
		for(fB = fStart; fB < bodies.Count; fB++)
		{
			//handling of null body in list
			if(!bodies[fB])
			{
				bodies.Remove(bodies[fB]);
				Debug.LogWarning("bodies[fB] is null");
				break;
			}		

			for(sB = sStart; sB < bodies.Count; sB++)
			{
				//handling of null body in list
				if(!bodies[sB])
				{
					bodies.Remove(bodies[sB]);
					Debug.LogWarning("bodies[sB] is null");
					break;
				}

				//n^2/2 optimisation where n can orbit n+1 or n+1 can orbit n
				if(fB < sB && (bodies[fB].canOrbit(bodies[sB], relDistance) || bodies[sB].canOrbit(bodies[fB], 
				                                                                                   relDistance)))
				{	
					//Increase total of comparisons
					comparisons ++;

					//Calculate Force due to gravity
					//Find vector from first body to second body
					deltaPosition = bodies[sB].transform.position - bodies[fB].transform.position;

					//Find direction
					direction = Vector3.Normalize(deltaPosition);

					//find r^2
					relDistance = deltaPosition.sqrMagnitude;

					//m1 * m2
					forceDueToGrav = (bodies[fB].rigidbody.mass * bodies[sB].rigidbody.mass);

					//(m1 * m2)/ r^2;
					forceDueToGrav/= (relDistance);

					//Amplify gravity foce
					forceDueToGrav *= gForceAmp;
						
					if(bodies[fB].canOrbit(bodies[sB], relDistance))
					{
						if(bodies[fB].orbitTarget && bodies[fB].orbitTarget == bodies[sB])
						{
							/*If second body is first bodies orbit target then calulate direction of orbit to be used 
							when maintaining orbit*/
							bodies[fB].directionToOrbitTarget = direction;
						}
						bodies[fB].rigidbody.AddForce(direction * forceDueToGrav * Time.deltaTime);
					}

					if(bodies[sB].canOrbit(bodies[fB], relDistance))
					{
						if(bodies[sB].orbitTarget && bodies[sB].orbitTarget == bodies[fB])
						{
							/*If first body is second bodies orbit target then calulate direction of orbit to be used 
							when maintaining orbit*/
							bodies[sB].directionToOrbitTarget = direction * -1;
						}
						bodies[sB].rigidbody.AddForce(direction * -1 * forceDueToGrav  * Time.deltaTime);
					}

					//If comparisions great than stepping value
					if(comparisons >= step)
					{
						//Save current position in List
						fStart = fB;
						sStart = sB;

						//Reset comparisions
						comparisons = 0;

						//Leave function
						return;
					}
				}
			}
			sStart = 0; //If we get there second loop will start at 0 next time around
		}
		fStart = 0; //Same logic as sStart
	}

	//Function to safely remove body
	public void removeBodyAt(int iD)
	{
		foreach(SpaceObject body in bodies.ToArray())
		{
			if(body.transform.GetInstanceID() == iD)
			{
				//Remove body from list
				bodies.Remove(body);

				//Destroy SpaceObject script
				Destroy(body.GetComponent<SpaceObject>());

				//Add script that shrinks Mesh of object and then destroys them slowly
				body.gameObject.AddComponent<DestroySlowly>();

				return;
			}
		}
		Debug.LogError("No object of that ID found"); //Only get here if object isn't found
	}

	//Function that converts strings to Floats
	float stringCheck(string value)
	{
		if(value != "")
		{
			//This statements prevents conversion when the values start like this to allow negatives and decimal values
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

	//Simple function to return string represenation of BodyType
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

		case BodyType.BlackHole:
			
			return "B.Hole";

		default:

			Debug.LogError("No body type found");

			return "null";
		}
	}
}
	
