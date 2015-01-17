using UnityEngine;
using System.Collections;

public static class PStats
{
	public static float AUScaling = 10;

	public const float REALEARTHMASS = 5679.6f;

	public const int TOMETRES = 1000;

	#region Diameter (Km) - Planets and Moon

	public static float SunDiam 	= 1392684;
	public static float MercuryDiam	= 0004878;
	public static float VenusDiam	= 0012102;
	public static float EarthDiam	= 0012742;
	public static float MarsDiam	= 0006778;
	public static float JupiterDiam = 0139822;
	public static float SaturnDiam 	= 0116464;
	public static float UranusDiam	= 0050724;
	public static float NeptuneDiam	= 0049244;
	public static float PlutoDiam	= 0002368;
	public static float MoonDiam	= 0003474;

	#endregion

	#region Mass (Me - Mass relative to Earth)

	//All of these values multipied by Earths actual mass will give there mass

	public static float SunMass		= 333000;
	public static float MercuryMass = 0.0553f;
	public static float VenusMass 	= 0.815f;
	public static float EarthMass 	= 1f;
	public static float MarsMass	= 0.107f;
	public static float JupiterMass	= 371.83f;
	public static float SaturnMass	= 95.159f;
	public static float UranusMass 	= 14.536f;
	public static float NeptuneMass	= 17.147f;
	public static float PlutoMass	= 0.0022f;
	public static float MoonMass	= 0.0123f;
	
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
	public static float MoonDist	= 0150005696;

	#endregion 

	//Earth Units = Any unit in Km/Earth's Diameter
	public static float inAUnits(float val)
	{
		return (val/EarthDist) * AUScaling;
	}

	public static float temp(float val)
	{
		return val /= 33300;
	}
}
