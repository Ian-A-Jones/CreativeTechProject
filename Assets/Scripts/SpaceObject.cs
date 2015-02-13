using UnityEngine;
using System.Collections;

public class SpaceObject : MonoBehaviour 
{
	//Member Variables
	//Maximum masss a body can Have
	public static int maxMass = 50;
	public static float pScale = 1;
	public static float sScale = 10;
	public static float TwoBodyMassScaling = 100;
	public const float AMP = 100f;
	public static float speedAmp =  1, distanceAmp = 5;
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

	public void init(string name, float mass, float diam, SpaceObject _OrbitTarget, float _MinOrbitP, float _MaxOrbitP)
	{
		this.name = name;
		
		this.setMassAndSize(mass, PStats.inAUnits(diam));
		
		//Orbit Target logic
		if(_OrbitTarget != null)
		{
			this.orbitDistance = Mathf.Abs(this.transform.position.x - _OrbitTarget.transform.position.x);

			Debug.Log (this.name + " is going to oribit " + _OrbitTarget.name);
			this.orbitTarget = _OrbitTarget;	


//			thsis.transform.parent = orbitTarget.transform;
			
			float diff = Mathf.Abs(this.rigidbody.mass - _OrbitTarget.rigidbody.mass);
			
			Debug.Log ("Mass difference of " + name + " and " + _OrbitTarget.name + ": " + diff);
			
			minOrbitP = _MinOrbitP;
			maxOrbitP = _MaxOrbitP;
			
//			if( diff < 1)
//			{
//				this.avgOrbitVelocity = this.findOVWithMass(_OrbitTarget);
//			}
//			else
//			{
				this.avgOrbitVelocity = this.findSimpleOrbitVelocity(_OrbitTarget);
//			}
			
			if(_OrbitTarget.orbitTarget != null)
			{
				orbitDistance/=2;
//				if( diff < 1)
//				{
//					this.avgOrbitVelocity += _OrbitTarget.findOVWithMass(_OrbitTarget.orbitTarget);
//				}
//				else
//				{
					this.avgOrbitVelocity += _OrbitTarget.findSimpleOrbitVelocity(_OrbitTarget.orbitTarget);
//				}
			}

//			this.rigidbody.AddForce(0,0, this.avgOrbitVelocity + _OrbitTarget.avgOrbitVelocity, ForceMode.Force);

			Debug.Log ("Avg V : " + avgOrbitVelocity);

			GetComponentInChildren<TrailRenderer>().time = 8f*orbitDistance/avgOrbitVelocity; //Magic number translated speed of object into a estimated time for trail to appear
			GetComponentInChildren<TrailRenderer>().startWidth = GetComponentInChildren<MeshRenderer>().transform.localScale.x*transform.localScale.x;
			GetComponentInChildren<TrailRenderer>().endWidth = GetComponentInChildren<MeshRenderer>().transform.localScale.x*transform.localScale.x;
		}
	}

	void FixedUpdate()
	{
		if(orbitTarget)
		{
//			Debug.Log ("Maintaning Orbit");
			maintainOrbit();
		}
	}

	public bool canOrbit(SpaceObject otherSObj)
	{
		if(orbitOn && orbitTarget == null || orbitTarget == otherSObj || 
		   Vector3.Distance(this.transform.position, otherSObj.transform.position) < 2)
		{
//			Debug.Log (this.name + " can orbit " + otherSObj.name);
			return true;
		}
		else
		{
//			Debug.Log(this.name + " cannot orbit " + otherSObj.name);
			return false;
		}
	}

	public void setMassAndSize(float massToSet, float diameter)
	{
//		massToSet = Mathf.Clamp(massToSet, 0, maxMass);
		
		rigidbody.mass = massToSet;

		if(name != "Sun")
		{
			Debug.Log("Planet size");
			transform.localScale = threeAsOne(diameter);
			GetComponentInChildren<MeshRenderer>().transform.localScale = threeAsOne(diameter * 
			                                                                         SpaceManager.planetSizeScale);
		}
		else
		{
			Debug.Log ("Sun Size");
			transform.localScale = threeAsOne(diameter);
			GetComponentInChildren<MeshRenderer>().transform.localScale = threeAsOne(diameter * sScale);
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
		Debug.Log ("Simple OV equation");

		float semiMajorAxis = Vector3.Distance(this.transform.position, otherBody.transform.position);
		
		float orbitVelocity = 2*Mathf.PI*semiMajorAxis;
		
		float OrbitPeriod = Mathf.Pow(semiMajorAxis,3);

		OrbitPeriod /= (otherBody.rigidbody.mass * (float)SpaceManager.GRAVITYCONSTANT * SpaceManager.gForceAmp);	
		OrbitPeriod = Mathf.Sqrt(OrbitPeriod);
		
		OrbitPeriod *= 2*Mathf.PI;
		
		Debug.Log ("Orbit period: " + OrbitPeriod);
		
		orbitVelocity /=OrbitPeriod;

		Debug.Log ("Orbit velocity: " + orbitVelocity);
	
		return orbitVelocity;
	}  

	/*Similar to the above function this finds the orbital speed of a body but is used when the masses are Vastly 
	  different e.g. Sun and Earth*/
	public float findOVWithMass(SpaceObject otherBody)
	{
		Debug.Log ("Complex OV equation");

		float OrbitVelocity = this.rigidbody.mass + otherBody.rigidbody.mass;
		
		OrbitVelocity *= (float)SpaceManager.GRAVITYCONSTANT * SpaceManager.gForceAmp;
		
		OrbitVelocity /= Vector3.Distance(this.transform.position, otherBody.transform.position);
		
		OrbitVelocity = Mathf.Sqrt(OrbitVelocity);
		
		Debug.Log ("Orbit Velocity: " + OrbitVelocity);
		
		return OrbitVelocity;
	}
	
	public void maintainOrbit()
	{
		if(orbitTarget != null)
		{
			clampVelocity();
			clampDistance();
		}
	}

	public void clampVelocity()
	{
//		Debug.Log ("Velocity: " + rigidbody.velocity.magnitude);

		float speed = rigidbody.velocity.magnitude;  // test current object speed

		Vector3 deltaPosition = this.transform.position - orbitTarget.transform.position;

		deltaPosition.Normalize();

//		deltaPosition *= Mathf.PI/2;

//		Debug.DrawRay(this.transform.position, deltaPosition);

//		Debug.DrawRay(this.transform.position, deltaPosition.normalized, Color.red);

		Debug.DrawRay(transform.position, new Vector3(directionToOrbitTarget.z * -1, rigidbody.velocity.normalized.y, directionToOrbitTarget.x), Color.green);
		rigidbody.AddForce(new Vector3(directionToOrbitTarget.z * -1, rigidbody.velocity.normalized.y, directionToOrbitTarget.x) * (avgOrbitVelocity - speed) * speedAmp * Time.deltaTime);
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
//			
//		{
//			brakeSpeed = speed - (avgOrbitVelocity);  // calculate the speed decrease
//			
//			normalisedVelocity = rigidbody.velocity.normalized;
//			brakeVelocity = normalisedVelocity * brakeSpeed;  // make the brake Vector3 value
//			
//			rigidbody.AddForce(-brakeVelocity * speed/avgOrbitVelocity * speedAmp * Time.deltaTime);  // apply opposing brake force
//		}
	}

	public void clampDistance()
	{
		distance = Vector3.Distance(this.transform.position, orbitTarget.transform.position);

//		Debug.Log ("Distance to Sun: " + distance);

		deltaPosition = this.transform.position - orbitTarget.transform.position;
		
		//		Debug.DrawLine(this.transform.position, deltaPosition * 0.8f, Color.red); 
		//
//		Debug.DrawRay(transform.position, -deltaPosition, Color.red);

		float diff = orbitDistance - distance;

		rigidbody.AddForce(deltaPosition.normalized * diff * distanceAmp * Time.deltaTime);
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
