//Script to manage both cameras
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour 
{
	public Camera freeRoamCam;
	public Camera COMCam;

	string activeCam;
	// Use this for initialization
	void Start () 
	{
		activeCam = Camera.main.gameObject.name;
	}

	void OnGUI()
	{
		GUI.Label(new Rect(4, 0, 200, 20), "Active Camera: " + activeCam);
	}
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKey(KeyCode.Alpha1))
		{
			freeRoamCam.gameObject.SetActive(true);
			COMCam.gameObject.SetActive(false);

			activeCam = freeRoamCam.gameObject.name;
		}

		if(Input.GetKey(KeyCode.Alpha2))
		{
			COMCam.gameObject.SetActive(true);
			freeRoamCam.gameObject.SetActive(false);

			activeCam = COMCam.gameObject.name;
		}
	}
}
