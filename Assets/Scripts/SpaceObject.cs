using UnityEngine;
using System.Collections;

public class SpaceObject : MonoBehaviour 
{
	//Member Variables
	//Maximum masss a body can Have
	public static int maxMass = 50;
	public static float pScale = 1;
	//IF a SpaceObject has an orbit target then it will only be affected by it's Gravity
	public SpaceObject orbitTarget = null;
	public bool orbitOn = true;

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

		transform.localScale = new Vector3(diameter * pScale, diameter * pScale, diameter * pScale);
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
	
		return 2*orbitVelocity;
	}  

	/*Similar to the above function this finds the orbital speed of a body but is used when the masses are Vastly 
	  different e.g. Sun and Earth*/
	public float findOVWithMass(SpaceObject otherBody)
	{
		Debug.Log ("Complex OV equation");

		float OrbitVelocity = this.rigidbody.mass + otherBody.rigidbody.mass;
		
		OrbitVelocity *= (float)SpaceManager.GRAVITYCONSTANT * PStats.REALEARTHMASS * SpaceManager.gForceAmp;
		
		OrbitVelocity /= Vector3.Distance(this.transform.position, otherBody.transform.position);
		
		OrbitVelocity = Mathf.Sqrt(OrbitVelocity);
		
		Debug.Log ("Orbit Velocity: " + OrbitVelocity);
		
		return OrbitVelocity;
	}
}