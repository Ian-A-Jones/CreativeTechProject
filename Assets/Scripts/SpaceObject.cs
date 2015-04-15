//Script for space objects or bodies used in Sim
using UnityEngine;
using System.Collections;

public class SpaceObject : MonoBehaviour 
{
	//Member Variables

	//Scaled for different body types
	static float pScale = 2, rScale = 1.5f, sScale = 1;

	//Whether or not space Object can orbit
	public static bool orbitOn = true;
	
	//Amplication values for Speed and distance spring
	float speedAmp =  25, distanceAmp = 75;

	//IF a SpaceObject has an orbit target then it will only be affected by it's Gravity
	public SpaceObject orbitTarget = null;

	//distance from Orbit target and average speed to be maintained while orbitting
	public float orbitDistance;
	public float avgOrbitVelocity;

	//Variables used in springs
	public float speed;
	public Vector3 deltaPosition;
	public float brakeSpeed;
	public Vector3 normalisedVelocity;
	public Vector3 brakeVelocity;
	public float acclSpeed;
	public float distance;
	public float diff;

	//Direction to orbit taget 
	public Vector3 directionToOrbitTarget;

	//Whether object should maintain it's orbit
	public static bool bMaintainOrbit = true;

	//Body type of said spaceObject 
	BodyType bType;

	//Whether trails should be drawn
	static bool drawTrail;

	//Variables for finding orbit speeds
	float semiMajorAxis;
	float orbitVelocity;
	float OrbitPeriod;

	//Function used to set up body using given variables
	public void init(string name, BodyType _BType, float mass, float diam, SpaceObject _OrbitTarget, float orbitPeriod)
	{
		this.name = name;

		bType = _BType;

		this.setMassAndSize(mass, diam);
		
		//Orbit Target logic
		if(_OrbitTarget != null)
		{
			//Drag aids in when using Orbit targets, this will be set to zero if Orbit target is broken
			rigidbody.drag = 0.1f;
			
			orbitDistance = Vector3.Distance(transform.position, _OrbitTarget.transform.position);

			orbitTarget = _OrbitTarget;	

			//Calulate orbit Velocity for spaceObject With a amplification variables
			avgOrbitVelocity = findSimpleOrbitVelocity(_OrbitTarget, orbitPeriod);

			//Special amp values for springs if using rings
			if(bType == BodyType.Ring)
			{
				speedAmp = 1;
				distanceAmp = 100;
			}
		}
	}

	//Simpler initalise function for firing planets
	public void init(string _Name, BodyType _BType, float mass, float diam, float velocity)
	{
		name = _Name;

		bType = _BType;

		setMassAndSize(mass, diam);

		rigidbody.AddForce(Camera.main.transform.forward * velocity);

		GetComponentInChildren<TrailRenderer>().gameObject.SetActive(false);
	}

	public void Start()
	{
		//If an orbit target has been set then give object spin
		if(orbitTarget)
		{
			StartCoroutine(maintainOrbit());
			rigidbody.AddTorque(Vector3.up * 10);
		}
	}

	//Functuoin that calculates trail based off of orbit target
	void calcTrail()
	{
		GetComponentInChildren<TrailRenderer>().time = 2*Mathf.PI*distance/speed; //Magic number translated speed of object into a estimated time for trail to appear
		GetComponentInChildren<TrailRenderer>().startWidth = transform.localScale.x;
		GetComponentInChildren<TrailRenderer>().endWidth = transform.localScale.x*transform.localScale.x/10; //Half size to make end obvious
	}

	void Update()
	{
		//Clamp speed to help prevent object from moving too fast
		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, 200);

		//Toggle trail of SpaceObjects
		if(Input.GetKeyDown(KeyCode.T))
		{
			if(GetComponentInChildren<TrailRenderer>())
			{
				GetComponentInChildren<TrailRenderer>().enabled = !GetComponentInChildren<TrailRenderer>().enabled;
			}
		}

	}

	void FixedUpdate()
	{
//		0if(orbitTarget)
//		{
//			maintainOrbit();
//		}
	}

	//Function to decide if object can orbit another
	public bool canOrbit(SpaceObject otherSObj, float sqrDist)
	{
		if(orbitOn && orbitTarget == null || orbitTarget == otherSObj 
		   || sqrDist < Mathf.Pow(this.rigidbody.mass, 2) || otherSObj.bType == BodyType.BlackHole)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	//Assigns mass and uses scale values to assign 
	public void setMassAndSize(float massToSet, float diameter)
	{
		rigidbody.mass = massToSet;

		switch(bType)
		{
		case BodyType.planet:

			transform.localScale = threeAsOne(diameter * pScale);
			break;
		case BodyType.Sun:

			transform.localScale = threeAsOne(diameter * sScale);
			break;
		case BodyType.Ring:

			transform.localScale = threeAsOne(diameter * rScale);
			break;

		case BodyType.BlackHole:
			
			transform.localScale = threeAsOne(diameter * sScale);
			break;
		}
	}

	//Returns vector3 with x,y,z as all values
	Vector3 threeAsOne(float val)
	{
		return new Vector3(val, val, val);
	}

	//This function uses the calculation for mean orbital speed 
	public float findSimpleOrbitVelocity(SpaceObject obj)
	{
		semiMajorAxis = Vector3.Distance(this.transform.position, obj.transform.position);
		
		orbitVelocity = 2*Mathf.PI*semiMajorAxis;
		
		OrbitPeriod = Mathf.Pow(semiMajorAxis,3);
		OrbitPeriod /= (obj.rigidbody.mass * SpaceManager.gForceAmp);	
		OrbitPeriod = Mathf.Sqrt(OrbitPeriod);
		OrbitPeriod *= 2*Mathf.PI;
		
		orbitVelocity /=OrbitPeriod;
	
		return orbitVelocity;
	}

	//Same function as before but with amplifcation value
	public float findSimpleOrbitVelocity(SpaceObject obj, float force)
	{
		semiMajorAxis = Vector3.Distance(this.transform.position, obj.transform.position);
		
		orbitVelocity = 2*Mathf.PI*semiMajorAxis;
		
		OrbitPeriod = Mathf.Pow(semiMajorAxis,3);
		OrbitPeriod /= (obj.rigidbody.mass);	
		OrbitPeriod = Mathf.Sqrt(OrbitPeriod);		
		OrbitPeriod *= 2*Mathf.PI;
		
		orbitVelocity /= (OrbitPeriod/ force);
		
		return orbitVelocity;
	}

	public IEnumerator maintainOrbit()
	{
		while(enabled && orbitTarget)
		{
			//Calculate appropriates values
			speed = rigidbody.velocity.magnitude;
			
			deltaPosition = this.transform.position - orbitTarget.transform.position;

			distance = deltaPosition.magnitude;

			diff = orbitDistance - distance;

			deltaPosition.Normalize();

			//Speed spring
			rigidbody.AddForce(new Vector3(directionToOrbitTarget.z * -1, 0, directionToOrbitTarget.x) * 
			                   (avgOrbitVelocity - speed) * speedAmp * Time.deltaTime);
			
			rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, avgOrbitVelocity * 7f);

			//Distance spring
			switch(bType)
			{
			case BodyType.Ring:
				
				rigidbody.AddForce(deltaPosition.normalized * diff * distanceAmp/4  * Time.deltaTime);
				break;
				
			case BodyType.planet:
			case BodyType.BlackHole:
			case BodyType.Sun:

				rigidbody.AddForce(deltaPosition.normalized * diff * distanceAmp  * Time.deltaTime);
				break;
			}

			//Breaking of spring
			if((distance > 1.3f * orbitDistance || distance < 0.7f * orbitDistance) && orbitTarget)
			{
				Debug.Log ("Breaking Orbit of: " + name);
				orbitTarget = null;
				StopCoroutine("maintainOrbit");
				rigidbody.drag = 0;
			}	
			calcTrail();

			switch(bType)
			{
			case BodyType.planet:
				yield return null;
				break;
				
			case BodyType.Sun:
				yield return null;
				break;
				
			case BodyType.Ring:
				yield return null;
				break;
				
			case BodyType.BlackHole:
				yield return null;
				break;
			}
		}
	}
}
