using System.Collections.Generic;
using System.Text;
using RimWorld;
using UnityEngine;

namespace Verse;

public class SkyManager
{
	private Map map;

	private float curSkyGlowInt;

	private List<Pair<SkyOverlay, float>> tempOverlays = new List<Pair<SkyOverlay, float>>();

	private static readonly Color FogOfWarBaseColor = Color32.op_Implicit(new Color32((byte)77, (byte)69, (byte)66, byte.MaxValue));

	public const float NightMaxCelGlow = 0.1f;

	public const float DuskMaxCelGlow = 0.6f;

	private List<GameCondition> tempAllGameConditionsAffectingMap = new List<GameCondition>();

	public float CurSkyGlow => curSkyGlowInt;

	public SkyManager(Map map)
	{
		this.map = map;
	}

	public void SkyManagerUpdate()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		SkyTarget curSky = CurrentSkyTarget();
		curSkyGlowInt = curSky.glow;
		if (map == Find.CurrentMap)
		{
			MatBases.LightOverlay.color = curSky.colors.sky;
			Find.CameraColor.saturation = curSky.colors.saturation;
			Color sky = curSky.colors.sky;
			sky.a = 1f;
			sky *= FogOfWarBaseColor;
			MatBases.FogOfWar.color = sky;
			Color val = curSky.colors.shadow;
			Vector3? overridenShadowVector = GetOverridenShadowVector();
			if (overridenShadowVector.HasValue)
			{
				SetSunShadowVector(Vector2.op_Implicit(overridenShadowVector.Value));
			}
			else
			{
				SetSunShadowVector(GenCelestial.GetLightSourceInfo(map, GenCelestial.LightType.Shadow).vector);
				val = Color.Lerp(Color.white, val, GenCelestial.CurShadowStrength(map));
			}
			GenCelestial.LightInfo lightSourceInfo = GenCelestial.GetLightSourceInfo(map, GenCelestial.LightType.LightingSun);
			GenCelestial.LightInfo lightSourceInfo2 = GenCelestial.GetLightSourceInfo(map, GenCelestial.LightType.LightingMoon);
			Shader.SetGlobalVector(ShaderPropertyIDs.WaterCastVectSun, new Vector4(lightSourceInfo.vector.x, 0f, lightSourceInfo.vector.y, lightSourceInfo.intensity));
			Shader.SetGlobalVector(ShaderPropertyIDs.WaterCastVectMoon, new Vector4(lightSourceInfo2.vector.x, 0f, lightSourceInfo2.vector.y, lightSourceInfo2.intensity));
			Shader.SetGlobalFloat("_LightsourceShineSizeReduction", 20f * (1f / curSky.lightsourceShineSize));
			Shader.SetGlobalFloat("_LightsourceShineIntensity", curSky.lightsourceShineIntensity);
			Shader.SetGlobalFloat("_DayPercent", GenLocalDate.DayPercent(map));
			MatBases.SunShadow.color = val;
			MatBases.SunShadowFade.color = val;
			UpdateOverlays(curSky);
		}
	}

	public void ForceSetCurSkyGlow(float curSkyGlow)
	{
		curSkyGlowInt = curSkyGlow;
	}

	private void UpdateOverlays(SkyTarget curSky)
	{
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		tempOverlays.Clear();
		List<SkyOverlay> overlays = map.weatherManager.curWeather.Worker.overlays;
		for (int i = 0; i < overlays.Count; i++)
		{
			AddTempOverlay(new Pair<SkyOverlay, float>(overlays[i], map.weatherManager.TransitionLerpFactor));
		}
		List<SkyOverlay> overlays2 = map.weatherManager.lastWeather.Worker.overlays;
		for (int j = 0; j < overlays2.Count; j++)
		{
			AddTempOverlay(new Pair<SkyOverlay, float>(overlays2[j], 1f - map.weatherManager.TransitionLerpFactor));
		}
		for (int k = 0; k < map.gameConditionManager.ActiveConditions.Count; k++)
		{
			GameCondition gameCondition = map.gameConditionManager.ActiveConditions[k];
			List<SkyOverlay> list = gameCondition.SkyOverlays(map);
			if (list != null)
			{
				for (int l = 0; l < list.Count; l++)
				{
					AddTempOverlay(new Pair<SkyOverlay, float>(list[l], gameCondition.SkyTargetLerpFactor(map)));
				}
			}
		}
		for (int m = 0; m < tempOverlays.Count; m++)
		{
			Color overlayColor = ((!tempOverlays[m].First.forceOverlayColor) ? curSky.colors.overlay : tempOverlays[m].First.forcedColor);
			overlayColor.a = tempOverlays[m].Second;
			tempOverlays[m].First.OverlayColor = overlayColor;
		}
	}

	private void AddTempOverlay(Pair<SkyOverlay, float> pair)
	{
		for (int i = 0; i < tempOverlays.Count; i++)
		{
			if (tempOverlays[i].First == pair.First)
			{
				tempOverlays[i] = new Pair<SkyOverlay, float>(tempOverlays[i].First, Mathf.Clamp01(tempOverlays[i].Second + pair.Second));
				return;
			}
		}
		tempOverlays.Add(pair);
	}

	private void SetSunShadowVector(Vector2 vec)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Shader.SetGlobalVector(ShaderPropertyIDs.MapSunLightDirection, new Vector4(vec.x, 0f, vec.y, GenCelestial.CurShadowStrength(map)));
	}

	private SkyTarget CurrentSkyTarget()
	{
		SkyTarget b = map.weatherManager.curWeather.Worker.CurSkyTarget(map);
		SkyTarget skyTarget = SkyTarget.Lerp(map.weatherManager.lastWeather.Worker.CurSkyTarget(map), b, map.weatherManager.TransitionLerpFactor);
		map.gameConditionManager.GetAllGameConditionsAffectingMap(map, tempAllGameConditionsAffectingMap);
		for (int i = 0; i < tempAllGameConditionsAffectingMap.Count; i++)
		{
			SkyTarget? skyTarget2 = tempAllGameConditionsAffectingMap[i].SkyTarget(map);
			if (skyTarget2.HasValue)
			{
				skyTarget = SkyTarget.LerpDarken(skyTarget, skyTarget2.Value, tempAllGameConditionsAffectingMap[i].SkyTargetLerpFactor(map));
			}
		}
		tempAllGameConditionsAffectingMap.Clear();
		List<WeatherEvent> liveEventsListForReading = map.weatherManager.eventHandler.LiveEventsListForReading;
		for (int j = 0; j < liveEventsListForReading.Count; j++)
		{
			if (liveEventsListForReading[j].CurrentlyAffectsSky)
			{
				skyTarget = SkyTarget.Lerp(skyTarget, liveEventsListForReading[j].SkyTarget, liveEventsListForReading[j].SkyTargetLerpFactor);
			}
		}
		List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.AffectsSky);
		for (int k = 0; k < list.Count; k++)
		{
			CompAffectsSky compAffectsSky = list[k].TryGetComp<CompAffectsSky>();
			if (compAffectsSky.LerpFactor > 0f)
			{
				skyTarget = ((!compAffectsSky.Props.lerpDarken) ? SkyTarget.Lerp(skyTarget, compAffectsSky.SkyTarget, compAffectsSky.LerpFactor) : SkyTarget.LerpDarken(skyTarget, compAffectsSky.SkyTarget, compAffectsSky.LerpFactor));
			}
		}
		return skyTarget;
	}

	private Vector3? GetOverridenShadowVector()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		List<WeatherEvent> liveEventsListForReading = map.weatherManager.eventHandler.LiveEventsListForReading;
		for (int i = 0; i < liveEventsListForReading.Count; i++)
		{
			Vector2? overrideShadowVector = liveEventsListForReading[i].OverrideShadowVector;
			if (overrideShadowVector.HasValue)
			{
				Vector2? val = overrideShadowVector;
				if (!val.HasValue)
				{
					return null;
				}
				return Vector2.op_Implicit(val.GetValueOrDefault());
			}
		}
		List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.AffectsSky);
		for (int j = 0; j < list.Count; j++)
		{
			Vector2? overrideShadowVector2 = list[j].TryGetComp<CompAffectsSky>().OverrideShadowVector;
			if (overrideShadowVector2.HasValue)
			{
				Vector2? val = overrideShadowVector2;
				if (!val.HasValue)
				{
					return null;
				}
				return Vector2.op_Implicit(val.GetValueOrDefault());
			}
		}
		return null;
	}

	public string DebugString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("SkyManager: ");
		stringBuilder.AppendLine("CurCelestialSunGlow: " + GenCelestial.CurCelestialSunGlow(Find.CurrentMap));
		stringBuilder.AppendLine("CurSkyGlow: " + CurSkyGlow.ToStringPercent());
		stringBuilder.AppendLine("CurrentSkyTarget: " + CurrentSkyTarget().ToString());
		return stringBuilder.ToString();
	}
}
