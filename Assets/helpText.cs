using UnityEngine;
using System.Collections;

public class helpText : MonoBehaviour {

	public GUIText text;

	bool toggleText = false;
	bool startFade = true;

	Color startingCol;
	Color invisCol;
	Color targetCol;

	float basicallyOne = 0.9f;
	float basicallyZero = 0.0005f;

	// Use this for initialization
	void Start () 
	{
		startingCol = text.material.color;
		invisCol = new Color(startingCol.r, startingCol.g, startingCol.b, 0);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.anyKey && startFade)
		{
			toggleText = true;
			startFade = false;
			targetCol = invisCol;
		}

		if(Input.GetKeyDown(KeyCode.H) && !toggleText)
		{
			toggleText = true;

			if(text.material.color.a >= basicallyOne)
			{
				targetCol = invisCol;
			}
			else
			{
				targetCol = startingCol;
			}

		}

		if(toggleText)
		{
			text.material.color = Color.Lerp (text.material.color, targetCol, 0.1f);

			if((targetCol.a == 1 && text.material.color.a > basicallyOne) || 
			   (targetCol.a == 0 && text.material.color.a < basicallyZero))
			{
				toggleText = false;
			}
		}
	}
	
}
