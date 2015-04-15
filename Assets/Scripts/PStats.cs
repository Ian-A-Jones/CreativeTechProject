//Static class for stats of model of Solar System
using UnityEngine;
using System.Collections;

public static class PStats
{
	#region Diameter (Km) - Planets and Moons

	public static float SunDiam 	= 46.5f; //46.5
	public static float MercuryDiam	= 0.163f; //0.163
	public static float VenusDiam	= 0.405f; //0.405
	public static float EarthDiam	= 0.426f; //0.426
	public static float MarsDiam	= 0.227f; //0.227
	public static float JupiterDiam = 4.78f; //4.78
	public static float SaturnDiam 	= 4.03f; //4.03
	public static float UranusDiam	= 1.71f; //1.71
	public static float NeptuneDiam	= 1.65f; //1.65
	public static float PlutoDiam	= 0.08f; //0.08

	//Moons
	public static float MoonDiam	= 0.111f; //0.111
	public static float DeimosDiam 	= 0.107f; //0.107
	public static float Phobos		= 0000560;

	#endregion

	#region Mass (Me - Mass relative to Earth)

	//All of these values multipied by Earths actual mass will give there mass

	public static float SunMass		= 500;
	public static float MercuryMass = 0.5f;
	public static float VenusMass 	= 3;
	public static float EarthMass 	= 5f;
	public static float MarsMass	= 4;
	public static float JupiterMass	= 10;
	public static float SaturnMass	= 8;
	public static float UranusMass 	= 6;
	public static float NeptuneMass	= 7;
	public static float PlutoMass	= 1f;

	//Moons
	public static float MoonMass	= 0.3f;
	public static float DeimosMass	= 0.5f;

	//Rings
	public static float AstMass		= 0.1f;
	#endregion

	#region Distance from Sun(Km)

	public static float MercuryDist = 48.7f;
	public static float VenusDist 	= 72.8f;
	public static float EarthDist 	= 100;
	public static float MarsDist	= 152.36f;
	public static float JupiterDist	= 520.274f;
	public static float SaturnDist	= 952.2728f;
	public static float UranusDist 	= 1920.822f;
	public static float NeptuneDist	= 3008.69f;
	public static float PlutoDist	= 3974.532f;

	//Moons
	public static float MoonDist	= 115f;
	public static float DeimosDist 	= 163f;

	//Rings
	public static float ABelt		= 336.32f;
	public static float JBelt		= 559f;
	public static float SBelt		= 991f;
	public static float UBelt		= 1959f;
	public static float NBelt		= 3047.4f;


	#endregion 
}
