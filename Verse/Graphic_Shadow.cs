using System;
using LudeonTK;
using RimWorld;
using UnityEngine;

namespace Verse;

public class Graphic_Shadow : Graphic
{
	private Mesh shadowMesh;

	private ShadowData shadowInfo;

	[TweakValue("Graphics_Shadow", -5f, 5f)]
	private static float GlobalShadowPosOffsetX;

	[TweakValue("Graphics_Shadow", -5f, 5f)]
	private static float GlobalShadowPosOffsetZ;

	public Graphic_Shadow(ShadowData shadowInfo)
	{
		this.shadowInfo = shadowInfo;
		if (shadowInfo == null)
		{
			throw new ArgumentNullException("shadowInfo");
		}
		shadowMesh = ShadowMeshPool.GetShadowMesh(shadowInfo);
	}

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)shadowMesh != (Object)null && thingDef != null && shadowInfo != null && (Find.CurrentMap == null || !loc.ToIntVec3().InBounds(Find.CurrentMap) || !Find.CurrentMap.roofGrid.Roofed(loc.ToIntVec3())) && DebugViewSettings.drawShadows)
		{
			Vector3 val = loc + shadowInfo.offset;
			val.y = AltitudeLayer.Shadows.AltitudeFor();
			Graphics.DrawMesh(shadowMesh, val, rot.AsQuat, MatBases.SunShadowFade, 0);
		}
	}

	public override void Print(SectionLayer layer, Thing thing, float extraRotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 center = thing.TrueCenter() + (shadowInfo.offset + new Vector3(GlobalShadowPosOffsetX, 0f, GlobalShadowPosOffsetZ)).RotatedBy(thing.Rotation);
		center.y = AltitudeLayer.Shadows.AltitudeFor();
		Printer_Shadow.PrintShadow(layer, center, shadowInfo, thing.Rotation);
	}

	public override string ToString()
	{
		return string.Concat("Graphic_Shadow(", shadowInfo, ")");
	}
}
