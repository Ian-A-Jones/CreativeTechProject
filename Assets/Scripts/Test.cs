using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	int fStart = 0, sStart = 0, comparisons = 0, step = 50;

	int bodyEnd = 100;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void test()
	{
		for(int fB = fStart; fB < bodyEnd; fB++)
		{
			for(int sB = sStart; sB < bodyEnd; sB++)
			{
				//if Gravity occurs
				comparisons ++;
				
				if(comparisons > step)
				{
					Debug.Log ("Completed Step");
					Debug.Log (fStart);
					Debug.Log (sStart);
					fStart = fB;
					sStart = sB;
					comparisons = 0;
					break;
				}
			}
		}
	}
}
