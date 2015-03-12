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

	public static float SunMass		= 300;
	public static float MercuryMass = 0.5f;
	public static float VenusMass 	= 3;
	public static float EarthMass 	= 5f;
	public static float MarsMass	= 4;
	public static float JupiterMass	= 50;
	public static float SaturnMass	= 20;
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

	public static float MercuryDist = 0057910000;
	public static float VenusDist 	= 0108942780;
	public static float EarthDist 	= 0149600000;
	public static float MarsDist	= 0227940000;
	public static float JupiterDist	= 0778330000;
	public static float SaturnDist	= 1424600000;
	public static float UranusDist 	= 2873550000;
	public static float NeptuneDist	= 4501000000;
	public static float PlutoDist	= 5945900000;

	//Moons
	public static float MoonDist	= 0150005696;
	public static float DeimosDist 	= 0227963458;

	//Rings
	public static float ABelt		= 0503135000;

	#endregion 

	//Earth Units = Any unit in Km/Earth's Diameter
	public static float inAUnits(float val)
	{
		return (val/EarthDist) * AUScaling * DistScale;
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
