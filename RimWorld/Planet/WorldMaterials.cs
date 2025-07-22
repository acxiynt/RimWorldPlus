using UnityEngine;
using Verse;

namespace RimWorld.Planet;

[StaticConstructorOnStartup]
public static class WorldMaterials
{
	public static readonly Material WorldTerrain;

	public static readonly Material WorldIce;

	public static readonly Material WorldOcean;

	public static readonly Material UngeneratedPlanetParts;

	public static readonly Material Rivers;

	public static readonly Material RiversBorder;

	public static readonly Material Roads;

	public static int DebugTileRenderQueue;

	public static int WorldObjectRenderQueue;

	public static int WorldLineRenderQueue;

	public static int DynamicObjectRenderQueue;

	public static int FeatureNameRenderQueue;

	public static readonly Material MouseTile;

	public static readonly Material SelectedTile;

	public static readonly Material CurrentMapTile;

	public static readonly Material Stars;

	public static readonly Material Sun;

	public static readonly Material PlanetGlow;

	public static readonly Material SmallHills;

	public static readonly Material LargeHills;

	public static readonly Material Mountains;

	public static readonly Material ImpassableMountains;

	public static readonly Material VertexColor;

	private static readonly Material TargetSquareMatSingle;

	private static int NumMatsPerMode;

	public static Material OverlayModeMatOcean;

	private static Material[] matsFertility;

	private static readonly Color[] FertilitySpectrum;

	private const float TempRange = 50f;

	private static Material[] matsTemperature;

	private static readonly Color[] TemperatureSpectrum;

	private const float ElevationMax = 5000f;

	private static Material[] matsElevation;

	private static readonly Color[] ElevationSpectrum;

	private const float RainfallMax = 5000f;

	private static Material[] matsRainfall;

	private static readonly Color[] RainfallSpectrum;

	public static Material CurTargetingMat
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			TargetSquareMatSingle.color = GenDraw.CurTargetingColor;
			return TargetSquareMatSingle;
		}
	}

	static WorldMaterials()
	{
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Unknown result type (might be due to invalid IL or missing references)
		WorldTerrain = MatLoader.LoadMat("World/WorldTerrain", 3500);
		WorldIce = MatLoader.LoadMat("World/WorldIce", 3500);
		WorldOcean = MatLoader.LoadMat("World/WorldOcean", 3500);
		UngeneratedPlanetParts = MatLoader.LoadMat("World/UngeneratedPlanetParts", 3500);
		Rivers = MatLoader.LoadMat("World/Rivers", 3530);
		RiversBorder = MatLoader.LoadMat("World/RiversBorder", 3520);
		Roads = MatLoader.LoadMat("World/Roads", 3540);
		DebugTileRenderQueue = 3510;
		WorldObjectRenderQueue = 3550;
		WorldLineRenderQueue = 3590;
		DynamicObjectRenderQueue = 3600;
		FeatureNameRenderQueue = 3610;
		MouseTile = MaterialPool.MatFrom("World/MouseTile", ShaderDatabase.WorldOverlayAdditive, 3560);
		SelectedTile = MaterialPool.MatFrom("World/SelectedTile", ShaderDatabase.WorldOverlayAdditive, 3560);
		CurrentMapTile = MaterialPool.MatFrom("World/CurrentMapTile", ShaderDatabase.WorldOverlayTransparent, 3560);
		Stars = MatLoader.LoadMat("World/Stars");
		Sun = MatLoader.LoadMat("World/Sun");
		PlanetGlow = MatLoader.LoadMat("World/PlanetGlow");
		SmallHills = MaterialPool.MatFrom("World/Hills/SmallHills", ShaderDatabase.WorldOverlayTransparentLit, 3510);
		LargeHills = MaterialPool.MatFrom("World/Hills/LargeHills", ShaderDatabase.WorldOverlayTransparentLit, 3510);
		Mountains = MaterialPool.MatFrom("World/Hills/Mountains", ShaderDatabase.WorldOverlayTransparentLit, 3510);
		ImpassableMountains = MaterialPool.MatFrom("World/Hills/Impassable", ShaderDatabase.WorldOverlayTransparentLit, 3510);
		VertexColor = MatLoader.LoadMat("World/WorldVertexColor");
		TargetSquareMatSingle = MaterialPool.MatFrom("UI/Overlays/TargetHighlight_Square", ShaderDatabase.Transparent, 3560);
		NumMatsPerMode = 50;
		OverlayModeMatOcean = SolidColorMaterials.NewSolidColorMaterial(new Color(0.09f, 0.18f, 0.2f), ShaderDatabase.Transparent);
		FertilitySpectrum = (Color[])(object)new Color[2]
		{
			new Color(0f, 1f, 0f, 0f),
			new Color(0f, 1f, 0f, 0.5f)
		};
		TemperatureSpectrum = (Color[])(object)new Color[8]
		{
			new Color(1f, 1f, 1f),
			new Color(0f, 0f, 1f),
			new Color(0.25f, 0.25f, 1f),
			new Color(0.6f, 0.6f, 1f),
			new Color(0.5f, 0.5f, 0.5f),
			new Color(0.5f, 0.3f, 0f),
			new Color(1f, 0.6f, 0.18f),
			new Color(1f, 0f, 0f)
		};
		ElevationSpectrum = (Color[])(object)new Color[4]
		{
			new Color(0.224f, 0.18f, 0.15f),
			new Color(0.447f, 0.369f, 0.298f),
			new Color(0.6f, 0.6f, 0.6f),
			new Color(1f, 1f, 1f)
		};
		RainfallSpectrum = (Color[])(object)new Color[12]
		{
			new Color(0.9f, 0.9f, 0.9f),
			GenColor.FromBytes(190, 190, 190),
			new Color(0.58f, 0.58f, 0.58f),
			GenColor.FromBytes(196, 112, 110),
			GenColor.FromBytes(200, 179, 150),
			GenColor.FromBytes(255, 199, 117),
			GenColor.FromBytes(255, 255, 84),
			GenColor.FromBytes(145, 255, 253),
			GenColor.FromBytes(0, 255, 0),
			GenColor.FromBytes(63, 198, 55),
			GenColor.FromBytes(13, 150, 5),
			GenColor.FromBytes(5, 112, 94)
		};
		GenerateMats(ref matsFertility, FertilitySpectrum, NumMatsPerMode);
		GenerateMats(ref matsTemperature, TemperatureSpectrum, NumMatsPerMode);
		GenerateMats(ref matsElevation, ElevationSpectrum, NumMatsPerMode);
		GenerateMats(ref matsRainfall, RainfallSpectrum, NumMatsPerMode);
	}

	private static void GenerateMats(ref Material[] mats, Color[] colorSpectrum, int numMats)
	{
		mats = (Material[])(object)new Material[numMats];
		for (int i = 0; i < numMats; i++)
		{
			mats[i] = MatsFromSpectrum.Get(colorSpectrum, (float)i / (float)numMats);
		}
	}

	public static Material MatForFertilityOverlay(float fert)
	{
		int num = Mathf.FloorToInt(fert * (float)NumMatsPerMode);
		return matsFertility[Mathf.Clamp(num, 0, NumMatsPerMode - 1)];
	}

	public static Material MatForTemperature(float temp)
	{
		int num = Mathf.FloorToInt(Mathf.InverseLerp(-50f, 50f, temp) * (float)NumMatsPerMode);
		return matsTemperature[Mathf.Clamp(num, 0, NumMatsPerMode - 1)];
	}

	public static Material MatForElevation(float elev)
	{
		int num = Mathf.FloorToInt(Mathf.InverseLerp(0f, 5000f, elev) * (float)NumMatsPerMode);
		return matsElevation[Mathf.Clamp(num, 0, NumMatsPerMode - 1)];
	}

	public static Material MatForRainfallOverlay(float rain)
	{
		int num = Mathf.FloorToInt(Mathf.InverseLerp(0f, 5000f, rain) * (float)NumMatsPerMode);
		return matsRainfall[Mathf.Clamp(num, 0, NumMatsPerMode - 1)];
	}
}
