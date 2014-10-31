using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Deformer : MonoBehaviour 
{
	public GameObject sphere;

	List<int> closestVertices;
	List<float> distanceToClosest;

	List<int> verticesToBeHit;

	// Use this for initialization
	void Start () 
	{
		closestVertices = new List<int>();
		distanceToClosest = new List<float>();

		verticesToBeHit = new List<int>();
	}
	
	// Update is called once per frame
	void Update () 
	{
//		if (Input.GetMouseButtonDown(0))	   
//		{
//			Ray rayClick = camera.ScreenPointToRay(Input.mousePosition);
//			RaycastHit hitInfo;
//
//			if(Physics.Raycast(rayClick, out hitInfo))
//			{
//				Vector3[] tempVertices = sphere.GetComponent<MeshFilter>().mesh.vertices;
//
//				for(int i = 0; i < tempVertices.GetLength(0); i++)
//				{
//					float distancefromHit = Vector3.Distance(hitInfo.point, tempVertices[i]);
//
//					if(distancefromHit < 5)
//					{
//						closestVertices.Add(i);
//						distanceToClosest.Add(distancefromHit);
//					}
//				}
//
//				foreach(int vertice in closestVertices)
//				{
//					//Find the direction and magnitude of the current vertex with the centre of the sphere
//					Vector3 directionToCentre = sphere.transform.position - tempVertices[vertice];
//					float distanceToCentre = directionToCentre.magnitude;
//
//					//Normalise direction
//					directionToCentre = Vector3.Normalize(directionToCentre);
//
//					tempVertices[vertice] += directionToCentre * (distanceToCentre * 0.2f);
//				}
//
//				sphere.GetComponent<MeshFilter>().mesh.vertices = tempVertices;
//			}
//		}
	}

	void OnCollisionEnter(Collision collision) 
	{
		Debug.Log("Collided");

		Vector3[] tempVertices = sphere.GetComponent<MeshFilter>().mesh.vertices;

		foreach (ContactPoint contact in collision.contacts) 
		{
			for(int i = 0; i < tempVertices.GetLength(0); i++)
			{
				float distancefromHit = Vector3.Distance(contact.point, tempVertices[i]);

				if(distancefromHit < 1)
				{
					//Debug.Log(distancefromHit);
					verticesToBeHit.Add(i);
					distanceToClosest.Add(distancefromHit);
				}
			}

			Debug.DrawRay(contact.point, contact.normal, Color.red);
		}

		foreach(int vertice in verticesToBeHit)
		{
			//Find the direction and magnitude of the current vertex with the centre of the sphere
			Vector3 directionToCentre = sphere.transform.position - tempVertices[vertice];
			float distanceToCentre = directionToCentre.magnitude;

			//Normalise direction
			directionToCentre = Vector3.Normalize(directionToCentre);

			//Debug.Log("Vertex No." + vertice + " Before move pos: " + tempVertices[vertice].ToString());
			tempVertices[vertice] += directionToCentre * (distanceToCentre) *0.5f;
			//Debug.Log("Vertex No." + vertice + " After move pos: " + tempVertices[vertice].ToString());
		}

		sphere.GetComponent<MeshFilter>().mesh.vertices = tempVertices;
		
	}

}
