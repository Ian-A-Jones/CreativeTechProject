using UnityEngine;
using System.Collections;

public static class PStats
{
	public static float AUScaling = 100;

	public static float DistScale = 1f;

	public const float REALEARTHMASS = 5679.6f;

	public const int TOMETRES = 1000;

	#region Diameter (Km) - Planets and Moons

	public static float SunDiam 	= 1392684;
	public static float MercuryDiam	= 0004879;
	public static float VenusDiam	= 0012104;
	public static float EarthDiam	= 0012756;
	public static float MarsDiam	= 0006792;
	public static float JupiterDiam = 0142982;
	public static float SaturnDiam 	= 0120536;
	public static float UranusDiam	= 0051118;
	public static float NeptuneDiam	= 0049528;
	public static float PlutoDiam	= 0002390;

	//Moons
	public static float MoonDiam	= 0003340;
	public static float DeimosDiam 	= 0003200;
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

	//Earth Units = Any unit in Km/Earth's Diameter
	public static float inAUnits(float val)
	{
		return (val/0149600000) * AUScaling * DistScale;
	}

	public static float inEUnits(float val)
	{
		return (val/(EarthDiam * AUScaling));
	}

	public static float temp(float val)
	{
		return val /= 33300;
	}
}
