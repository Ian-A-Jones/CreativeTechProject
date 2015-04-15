//Simple script to create Perlin Noise Texture for planet which varies between two given colours
using UnityEngine;
using System.Collections;

public class TextureGenerator : MonoBehaviour {

	public Color cMin, cMax;

	private Texture2D texture;
	
	float scale = 6;
	
	float xStart, yStart;

	// Use this for initialization
	void Start () {
		StartCoroutine(genTerrain());
	}

	//Coroutine to generate Terrain without impededing runtime of application
	public IEnumerator genTerrain()
	{
		texture = new Texture2D(1024, 512, TextureFormat.RGBA32, false);
		texture.name = "Planet Texture";
		texture.wrapMode = TextureWrapMode.Clamp;
		
		GetComponent<MeshRenderer>().material.mainTexture = texture;
		
		//TODO:Change x and y Start to use Planet generated
		xStart = Random.Range (0.0f, 102400.0f);
		yStart = Random.Range (0.0f, 51200.0f);
		
		for (int y = 0; y < 512; y++) 
		{
			for (int x = 0; x < 1024; x++) 
			{
				float col = Mathf.PerlinNoise(
					(xStart + (float)x)/1024 * scale, 
					(yStart + (float)y)/512  * scale);            
				
				
				float r = ((col * (cMax.r-cMin.r)) + cMin.r) * 255;
				float g = ((col * (cMax.g-cMin.g)) + cMin.g) * 255;
				float b = ((col * (cMax.b-cMin.b)) + cMin.b) * 255;
				
				texture.SetPixel(x,y, new Color32((byte)r, (byte)g, (byte)b, 255));
			}

			//Keep looping if not at end of list
			if(y < 511)
			{
				yield return null;
			}
		}
		
		texture.Apply();
		renderer.material.SetTexture("_MainTex", texture);
	}
}
