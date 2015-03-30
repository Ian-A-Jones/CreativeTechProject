using UnityEngine;
using System.Collections;

public class spawnRef : MonoBehaviour {

	public Mesh planet, ring;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setMesh(bool _Planet)
	{
		if(_Planet)
		{
			this.GetComponent<MeshFilter>().sharedMesh = planet;
		}
		else
		{
			this.GetComponent<MeshFilter>().sharedMesh = ring;

		}
	}
}
