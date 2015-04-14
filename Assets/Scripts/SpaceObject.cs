using UnityEngine;
using System.Collections;

public class SpaceObject : MonoBehaviour 
{
	//Member Variables
	//Maximum masss a body can Have
	public static int maxMass = 50;
	static float pScale = 1, rScale = 1, sScale = 1;
	static float meshScale = 10;
	static float oTScale = 1;
	float speedAmp =  25, distanceAmp = 75;
	//IF a SpaceObject has an orbit target then it will only be affected by it's Gravity
	public SpaceObject orbitTarget = null;
	public static bool orbitOn = true;
	public float orbitDistance;

	public float avgOrbitVelocity;

	float minOrbitP, maxOrbitP;

	public float speed;
	public Vector3 deltaPosition;
	public float brakeSpeed;
	public Vector3 normalisedVelocity;
	public Vector3 brakeVelocity;
	public float acclSpeed;
	public float distance;

	public Vector3 directionToOrbitTarget;

	public float diff;

	public static bool bMaintainOrbit = true;

	BodyType bType;

	static bool drawTrail;

	public void init(string name, BodyType _BType, float mass, float diam, SpaceObject _OrbitTarget, float orbitPeriod)
	{
		this.name = name;

		bType = _BType;

		this.setMassAndSize(mass, diam);
		
		//Orbit Target logic
		if(_OrbitTarget != null)
		{
			this.rigidbody.drag = 0.1f;
			//Make it larger
			GetComponentInChildren<MeshRenderer>().transform.localScale *= oTScale;
			
			this.orbitDistance = Vector3.Distance(this.transform.position, _OrbitTarget.transform.position);
			
//			Debug.Log (this.name + " is going to oribit " + _OrbitTarget.name);
			this.orbitTarget = _OrbitTarget;	
			
			
			//			thsis.transform.parent = orbitTarget.transform;
			
			float diff = Mathf.Abs(this.rigidbody.mass - _OrbitTarget.rigidbody.mass);
			
//			Debug.Log ("Mass difference of " + name + " and " + _OrbitTarget.name + ": " + diff);

			this.avgOrbitVelocity = this.findSimpleOrbitVelocity(_OrbitTarget, orbitPeriod);
			
//			Debug.Log ("Avg V : " + avgOrbitVelocity);
			
//			calcTrail(orbitDistance, avgOrbitVelocity);

			if(bType == BodyType.Ring)
			{
				speedAmp = 1;
				distanceAmp = 100;
			}

			if(_OrbitTarget.orbitTarget)
			{
				distanceAmp = 1000;
			}

		}
	}

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
//		if(bType != BodyType.Ring)
//		{
		StartCoroutine(maintainOrbit());
		rigidbody.AddTorque(Vector3.up * 10);
//		}
	}

	void calcTrail()
	{
			GetComponentInChildren<TrailRenderer>().time = 2*Mathf.PI*distance/speed; //Magic number translated speed of object into a estimated time for trail to appear
			GetComponentInChildren<TrailRenderer>().startWidth = transform.localScale.x;
			GetComponentInChildren<TrailRenderer>().endWidth = transform.localScale.x*transform.localScale.x/10; //Half size to make end obvious
	}

	void Update()
	{
		rigidbody.velocity =  Vector3.ClampMagnitude(rigidbody.velocity, 200);

		if(Input.GetKeyDown(KeyCode.T) && bType != BodyType.Ring)
		{
			if(GetComponentInChildren<TrailRenderer>())
			{
				GetComponentInChildren<TrailRenderer>().enabled = !GetComponentInChildren<TrailRenderer>().enabled;
			}
		}

	}

	void FixedUpdate()
	{
		if(orbitTarget)
		{
//			Debug.Log ("Maintaning Orbit");
		}
	}

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

	public void setMassAndSize(float massToSet, float diameter)
	{
//		massToSet = Mathf.Clamp(massToSet, 0, maxMass);

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

	/*This function uses the calculation for mean orbital speed with a small eccentricity orbit to calculate what 
	Velocity is required for orbit. This function should be used when the masses of the two bodies are similar
	 e.g. Moon and Earth*/
	public float findSimpleOrbitVelocity(SpaceObject otherBody)
	{
//		Debug.Log ("Simple OV equation");

		float semiMajorAxis = Vector3.Distance(this.transform.position, otherBody.transform.position);
		
		float orbitVelocity = 2*Mathf.PI*semiMajorAxis;
		
		float OrbitPeriod = Mathf.Pow(semiMajorAxis,3);

		OrbitPeriod /= (otherBody.rigidbody.mass * SpaceManager.gForceAmp);	
		OrbitPeriod = Mathf.Sqrt(OrbitPeriod);

		OrbitPeriod *= 2*Mathf.PI;
		
//		Debug.Log ("Orbit period: " + OrbitPeriod);
		
		orbitVelocity /=OrbitPeriod;

//		Debug.Log ("Orbit velocity: " + orbitVelocity);
	
		return orbitVelocity;
	}

	public float findSimpleOrbitVelocity(SpaceObject otherBody, float force)
	{
//		Debug.Log ("Simple OV equation");
		
		float semiMajorAxis = Vector3.Distance(this.transform.position, otherBody.transform.position);
		
		float orbitVelocity = 2*Mathf.PI*semiMajorAxis;
		
		float OrbitPeriod = Mathf.Pow(semiMajorAxis,3);
		
		OrbitPeriod /= (otherBody.rigidbody.mass);	
		OrbitPeriod = Mathf.Sqrt(OrbitPeriod);
		
		OrbitPeriod *= 2*Mathf.PI;
		
//		Debug.Log ("Orbit period: " + OrbitPeriod);
		
		orbitVelocity /= (OrbitPeriod/ force);
		
//		Debug.Log ("Orbit velocity: " + orbitVelocity);
		
		return orbitVelocity;
	}

//	public float findSimpleOrbitVelocity(SpaceObject otherBody, float orbitPeriod)
//	{
//		Debug.Log ("Simple OV equation");
//		
//		float semiMajorAxis = Vector3.Distance(this.transform.position, otherBody.transform.position);
//		
//		float orbitVelocity = 2*Mathf.PI*semiMajorAxis;
//
//		Debug.Log ("Orbit period: " + orbitPeriod);
//		
//		orbitVelocity /= (orbitPeriod/3.6f);
//		
//		Debug.Log ("Orbit velocity: " + orbitVelocity);
//
//		return orbitVelocity;
//	}

	/*Similar to the above function this finds the orbital speed of a body but is used when the masses are Vastly 
	  different e.g. Sun and Earth*/
	public float findOVWithMass(SpaceObject otherBody)
	{
//		Debug.Log ("Complex OV equation");

		float OrbitVelocity = this.rigidbody.mass + otherBody.rigidbody.mass;
		
		OrbitVelocity *= SpaceManager.gForceAmp;
		
		OrbitVelocity /= Vector3.Distance(this.transform.position, otherBody.transform.position);
		
		OrbitVelocity = Mathf.Sqrt(OrbitVelocity);
		
//		Debug.Log ("Orbit Velocity: " + OrbitVelocity);
		
		return OrbitVelocity;
	}
	
	public IEnumerator maintainOrbit()
	{
		while(this.enabled)
		{
			if(orbitTarget && bMaintainOrbit)
			{
				//Calculate appropriates values
				speed = rigidbody.velocity.magnitude;  // test current object speed
				
				deltaPosition = this.transform.position - orbitTarget.transform.position;

				distance = deltaPosition.magnitude;

				diff = orbitDistance - distance;

				if(distance < 0)
				{
					Debug.Log ("Wrong");
				}

				deltaPosition.Normalize();

				//Speed spring
				rigidbody.AddForce(new Vector3(directionToOrbitTarget.z * -1, 0, directionToOrbitTarget.x) * (avgOrbitVelocity - speed) * speedAmp * Time.deltaTime);
				
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

				if((distance > 1.3f * orbitDistance || distance < 0.7f * orbitDistance) && orbitTarget)
				{
					Debug.Log ("Breaking Orbit of: " + name);
					orbitTarget = null;
					rigidbody.drag = 0;
				}	

				calcTrail();
			}

			switch(bType)
			{
				case BodyType.planet:
					yield return null;
					break;

				case BodyType.Sun:
					yield return null;
					break;

				case BodyType.Ring:
					yield return new WaitForSeconds(0.5f);
					break;

				case BodyType.BlackHole:
					yield return null;
					break;
			}
		}
	}
}
