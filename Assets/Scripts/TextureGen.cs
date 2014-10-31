using UnityEngine;
using System.Collections;

public class TextureGen : MonoBehaviour 
{
	public int width;
	public int height;
	public bool randomXYStart;
	public float xStart;
	public float yStart;
	public float scale;
	private Texture2D planetTexture;
	 
	// Use this for initialization
	void Start () 
	{

		planetTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
		planetTexture.name = "Proc Texture";
		GetComponent<MeshRenderer>().material.mainTexture = planetTexture;
		planetTexture.wrapMode = TextureWrapMode.Repeat;
		if(randomXYStart)
		{
			xStart = Random.Range (0.0f, 102400.0f);
			yStart = Random.Range (0.0f, 102400.0f);
		}

		for (int y = 0; y < height; y++) 
		{
			for (int x = 0; x < width; x++) 
			{
				float col = Mathf.PerlinNoise(
					(xStart + (float)x)/width * scale, 
					(yStart + (float)y)/height * scale);			

				byte col32 = (byte)(col * 255);

				planetTexture.SetPixel(x,y, new Color32(col32, col32, col32, 255));
			}
		}

		planetTexture.Apply();

		Texture2D norm = planetTexture;
		renderer.material.SetTexture("_BumpMap", planetTexture);

	}
	
	// Update is called once per frame
	void Update () 
	{
	}


}
	