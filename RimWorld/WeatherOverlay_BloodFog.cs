using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WeatherOverlay_BloodFog : SkyOverlay
{
	private static readonly Material FogOverlayWorld = MatLoader.LoadMat("Weather/BloodFogOverlayWorld");

	public WeatherOverlay_BloodFog()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		worldOverlayMat = FogOverlayWorld;
		worldOverlayPanSpeed1 = 0.0005f;
		worldOverlayPanSpeed2 = 0.0004f;
		worldPanDir1 = new Vector2(1f, 1f);
		((Vector2)(ref worldPanDir1)).Normalize();
		worldPanDir2 = new Vector2(0.5f, -0.1f);
		((Vector2)(ref worldPanDir2)).Normalize();
		forceOverlayColor = true;
		forcedColor = new Color(0.6f, 0f, 0f);
	}
}
