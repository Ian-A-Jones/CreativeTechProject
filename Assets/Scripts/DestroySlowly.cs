//Script that slowly shrinkts object and then destroys it
using UnityEngine;
using System.Collections;

public class DestroySlowly : MonoBehaviour {


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.localScale *= 0.6f;

		if(transform.localScale.x < 0.01f)
		{
			Destroy(this.gameObject);
		}
	}
}
