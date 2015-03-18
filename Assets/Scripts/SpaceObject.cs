using UnityEngine;
using System.Collections;

public class SpaceObject : MonoBehaviour 
{
	//Member Variables
	//Maximum masss a body can Have
	public static int maxMass = 50;
	static float pScale = 50, rScale = 200, sScale = 50;
	static float meshScale = 10;
	static float oTScale = 1;
	float speedAmp =  25, distanceAmp = 200;
	//IF a SpaceObject has an orbit target then it will only be affected by it's Gravity
	public SpaceObject orbitTarget = null;
	public bool orbitOn = true;
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

	public enum bodyType
	{
		planet,
		Sun,
		Ring,
		BlackHole,
	};

	bodyType bType;

	public void init(string name, bodyType _BType, float mass, float diam, SpaceObject _OrbitTarget, float orbitPeriod)
	{
		this.name = name;

		bType = _BType;

		this.setMassAndSize(mass, PStats.inAUnits(diam));
		
		//Orbit Target logic
		if(_OrbitTarget != null)
		{
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

//			if(bType == bodyType.Ring)
//			{
//				transform.parent = _OrbitTarget.transform;
//			}

			if(_OrbitTarget.orbitTarget)
			{
				distanceAmp = 1000;
			}
		}
	}

	public void init(string _Name, bodyType _BType, float mass, float diam, float velocity)
	{
		name = _Name;

		bType = _BType;

		setMassAndSize(mass, PStats.inAUnits(diam));

		rigidbody.AddForce(Camera.main.transform.forward * velocity);

		GetComponentInChildren<TrailRenderer>().gameObject.SetActive(false);
	}

	public void Start()
	{
//		if(bType != bodyType.Ring)
//		{
			StartCoroutine(maintainOrbit());
//		}
	}

	void calcTrail()
	{
		if(GetComponentInChildren<TrailRenderer>().enabled)
		{
			GetComponentInChildren<TrailRenderer>().time = 2*Mathf.PI*distance/speed; //Magic number translated speed of object into a estimated time for trail to appear
			GetComponentInChildren<TrailRenderer>().startWidth = transform.localScale.x;
			GetComponentInChildren<TrailRenderer>().endWidth = transform.localScale.x*transform.localScale.x/10; //Half size to make end obvious
		}
	}

	void FixedUpdate()
	{
		if(orbitTarget)
		{
//			Debug.Log ("Maintaning Orbit");
		}
	}

	public bool canOrbit(SpaceObject otherSObj)
	{
		if(orbitOn && orbitTarget == null || orbitTarget == otherSObj || otherSObj.bType == bodyType.BlackHole)
			//|| Vector3.Distance(this.transform.position, otherSObj.transform.position) < 25
		{
//			if(this.name == "Moon")
//				Debug.Log (this.name + " can orbit " + otherSObj.name);
			return true;
		}
		else
		{
//			if(this.name == "Moon")
//				Debug.Log(this.name + " cannot orbit " + otherSObj.name);
			return false;
		}
	}

	public void setMassAndSize(float massToSet, float diameter)
	{
//		massToSet = Mathf.Clamp(massToSet, 0, maxMass);

		rigidbody.mass = massToSet;

		switch(bType)
		{
		case bodyType.planet:

			transform.localScale = threeAsOne(diameter * pScale);
			break;
		case bodyType.Sun:

			transform.localScale = threeAsOne(diameter * sScale);
			break;
		case bodyType.Ring:

			transform.localScale = threeAsOne(diameter * rScale);
			break;

		case bodyType.BlackHole:
			
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
		
		OrbitPeriod /= (otherBody.rigidbody.mass * SpaceManager.gForceAmp);	
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
				clampVelocity();
				clampDistance();

				calcTrail();
			}

			switch(bType)
			{
				case bodyType.planet:
					yield return null;
					break;

				case bodyType.Sun:
					yield return null;
					break;

				case bodyType.Ring:
					yield return new WaitForSeconds(0.5f);
//					yield return null;
					break;

				case bodyType.BlackHole:
					yield return null;
					break;
			}
		}
	}

	public void clampVelocity()
	{
//		Debug.Log ("Velocity: " + rigidbody.velocity.magnitude);

		speed = rigidbody.velocity.magnitude;  // test current object speed

		Vector3 deltaPosition = this.transform.position - orbitTarget.transform.position;

		deltaPosition.Normalize();

//		deltaPosition *= Mathf.PI/2;

//		Debug.DrawRay(this.transform.position, deltaPosition);

//		Debug.DrawRay(this.transform.position, deltaPosition.normalized, Color.red);

//		Debug.DrawRay(transform.position, new Vector3(directionToOrbitTarget.z * -1, rigidbody.velocity.normalized.y, directionToOrbitTarget.x), Color.green);
		rigidbody.AddForce(new Vector3(directionToOrbitTarget.z * -1, 0, directionToOrbitTarget.x) * (avgOrbitVelocity - speed) * speedAmp * Time.deltaTime);

		rigidbody.velocity = Vector3.ClampMagnitude(rigidbody.velocity, avgOrbitVelocity * 7f);
//		if (speed < (avgOrbitVelocity * minOrbitP) && speed > 0)
//		{
//			acclSpeed = speed + (avgOrbitVelocity);
//			
//			normalisedVelocity = rigidbody.velocity.normalized;
//			brakeVelocity = normalisedVelocity * acclSpeed;  // make the brake Vector3 value
//			
//			rigidbody.AddForce(brakeVelocity * avgOrbitVelocity/speed * speedAmp * Time.deltaTime);  // apply opposing brake force
//			
//		}
//
//		if (speed > (avgOrbitVelocity * maxOrbitP) && speed > 0)
	}
//			
	//TODO:Break clamp if goes too far
	public void clampDistance()
	{
		distance = Vector3.Distance(this.transform.position, orbitTarget.transform.position);

//		Debug.Log ("Distance to Sun: " + distance);

		deltaPosition = this.transform.position - orbitTarget.transform.position;
		
		//		Debug.DrawLine(this.transform.position, deltaPosition * 0.8f, Color.red); 
		//
//		Debug.DrawRay(transform.position, -deltaPosition, Color.red);

		diff = orbitDistance - distance;
		switch(bType)
		{
			case bodyType.Ring:
				
				rigidbody.AddForce(deltaPosition.normalized * diff * distanceAmp/4  * Time.deltaTime);
				break;

			case bodyType.planet:
				
				rigidbody.AddForce(deltaPosition.normalized * diff * distanceAmp  * Time.deltaTime);
				break;

			case bodyType.Sun:
			
				rigidbody.AddForce(deltaPosition.normalized * diff * distanceAmp  * Time.deltaTime);
				break;

			case bodyType.BlackHole:
				
				rigidbody.AddForce(deltaPosition.normalized * diff * distanceAmp  * Time.deltaTime);
				break;
		}
		//
//		if(distance < orbitDistance * minOrbitP)
//		{
//			rigidbody.AddForce(deltaPosition.normalized * orbitDistance/distance * distanceAmp * Time.deltaTime);
//		}
//		
//		if(distance > orbitDistance * maxOrbitP)
//		{
////			Debug.Log ("Pushing force str: " + (distance/orbitDistance * distanceAmp * Time.deltaTime));
//			rigidbody.AddForce(-deltaPosition.normalized * distance/orbitDistance * distanceAmp * Time.deltaTime);
//		}
	}
}
