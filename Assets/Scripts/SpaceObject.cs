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
			Debug.Log (this.name + " can orbit " + otherSObj.name);
			return true;
		}
		else
		{
			Debug.Log(this.name + " cannot orbit " + otherSObj.name);
			return false;
		}
	}

	public void setMass(float massToSet)
	{
		massToSet = Mathf.Clamp(massToSet, 0, maxMass);
		
		rigidbody.mass = massToSet;


		transform.localScale = new Vector3(massToSet * pScale, massToSet * pScale, massToSet * pScale);
	}
}