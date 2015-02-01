using UnityEngine;
using System.Collections;

public class SpaceObject : MonoBehaviour 
{
	//Member Variables
	//Maximum masss a body can Have
	public static int maxMass = 50;
	public static float pScale = 10;
	public static float sScale = 10;
	//IF a SpaceObject has an orbit target then it will only be affected by it's Gravity
	public SpaceObject orbitTarget = null;
	public bool orbitOn = true;
	public float orbitDistance;

	public float avgOrbitVelocity;

	public void init(string name, float mass, float diam, SpaceObject _OrbitTarget)
	{
		this.name = name;
		
		this.setMassAndSize(mass, PStats.inAUnits(diam));
		
		//Orbit Target logic
		if(_OrbitTarget != null)
		{
			this.orbitDistance = Mathf.Abs(this.transform.position.x - _OrbitTarget.transform.position.x);
			
			Debug.Log (this.name + " is going to oribit " + _OrbitTarget.name);
			this.orbitTarget = _OrbitTarget;	
			
			float diff = Mathf.Abs(this.rigidbody.mass - _OrbitTarget.rigidbody.mass);
			
			Debug.Log ("Mass difference of " + name + " and " + _OrbitTarget.name + ": " + diff);
			
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
				//				if( diff < 1)
				//				{
				//					this.avgOrbitVelocity += _OrbitTarget.findOVWithMass(_OrbitTarget.orbitTarget);
				//				}
				//				else
				//				{
									this.avgOrbitVelocity += _OrbitTarget.findSimpleOrbitVelocity(_OrbitTarget.orbitTarget);
				//				}
			}
			
			this.rigidbody.AddForce(0,0, this.avgOrbitVelocity, ForceMode.VelocityChange);

			GetComponentInChildren<TrailRenderer>().startWidth = transform.localScale.x;
			GetComponentInChildren<TrailRenderer>().endWidth = transform.localScale.x;
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
			transform.localScale = new Vector3(diameter * pScale, diameter * pScale, diameter * pScale);
		}
		else
		{
			Debug.Log ("Sun Size");
			transform.localScale = new Vector3(diameter * sScale, diameter * sScale, diameter * sScale);
		}
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
//			clampVelocity();
//			clampDistance();
		}
	}

	public void clampVelocity()
	{
		float speed = Vector3.Magnitude (rigidbody.velocity);  // test current object speed

		Vector3 deltaPosition = this.transform.position - orbitTarget.transform.position;

		deltaPosition.Normalize();

//		deltaPosition *= Mathf.PI/2;

//		Debug.DrawRay(this.transform.position, deltaPosition);

//		Debug.DrawRay(this.transform.position, deltaPosition.normalized, Color.red);

		if (speed > (avgOrbitVelocity * 1.2))
			
		{
			float brakeSpeed = speed - (avgOrbitVelocity);  // calculate the speed decrease
			
			Vector3 normalisedVelocity = rigidbody.velocity.normalized;
			Vector3 brakeVelocity = normalisedVelocity * brakeSpeed;  // make the brake Vector3 value
			
			rigidbody.AddForce(-brakeVelocity);  // apply opposing brake force
		}

		if (speed < (avgOrbitVelocity * 0.8))
		{
			float acclSpeed = speed + (avgOrbitVelocity);

			Vector3 normalisedVelocity = rigidbody.velocity.normalized;
			Vector3 brakeVelocity = normalisedVelocity * acclSpeed;  // make the brake Vector3 value
			
			rigidbody.AddForce(brakeVelocity);  // apply opposing brake force

		}
	}

	public void clampDistance()
	{
		float distance = Vector3.Distance(this.transform.position, orbitTarget.transform.position);

		Vector3 deltaPosition = this.transform.position - orbitTarget.transform.position;

//		Debug.DrawLine(this.transform.position, deltaPosition * 0.8f, Color.red); 
//
//		Debug.DrawLine(this.transform.position, deltaPosition * - 1.2f, Color.red);

		if(distance < orbitDistance * 0.8f)
		{

			deltaPosition = Vector3.Normalize(deltaPosition);
			
			rigidbody.AddForce(deltaPosition * orbitDistance);
		}
		
		if(distance > orbitDistance * 1.2f)
		{
			
			deltaPosition = Vector3.Normalize(deltaPosition);
			
			rigidbody.AddForce( -deltaPosition * orbitDistance);
		}
	}
}