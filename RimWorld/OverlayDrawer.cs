using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class OverlayDrawer
{
	private Dictionary<Thing, OverlayTypes> overlaysToDraw = new Dictionary<Thing, OverlayTypes>();

	private Dictionary<Thing, ThingOverlaysHandle> overlayHandles = new Dictionary<Thing, ThingOverlaysHandle>();

	private Vector3 curOffset;

	private DrawBatch drawBatch = new DrawBatch();

	private static readonly Material ForbiddenMat;

	private static readonly Material NeedsPowerMat;

	private static readonly Material PowerOffMat;

	private static readonly Material QuestionMarkMat;

	private static readonly Material BrokenDownMat;

	private static readonly Material OutOfFuelMat;

	private static readonly Material WickMaterialA;

	private static readonly Material WickMaterialB;

	private static readonly Material SelfShutdownMat;

	private const int AltitudeIndex_Forbidden = 4;

	private const int AltitudeIndex_BurningWick = 5;

	private const int AltitudeIndex_QuestionMark = 6;

	private static float SingleCellForbiddenOffset;

	private const float PulseFrequency = 4f;

	private const float PulseAmplitude = 0.7f;

	private static readonly float BaseAlt;

	private const float StackOffsetMultipiler = 0.25f;

	static OverlayDrawer()
	{
		ForbiddenMat = MaterialPool.MatFrom("Things/Special/ForbiddenOverlay", ShaderDatabase.MetaOverlay);
		NeedsPowerMat = MaterialPool.MatFrom("UI/Overlays/NeedsPower", ShaderDatabase.MetaOverlay);
		PowerOffMat = MaterialPool.MatFrom("UI/Overlays/PowerOff", ShaderDatabase.MetaOverlay);
		QuestionMarkMat = MaterialPool.MatFrom("UI/Overlays/QuestionMark", ShaderDatabase.MetaOverlay);
		BrokenDownMat = MaterialPool.MatFrom("UI/Overlays/BrokenDown", ShaderDatabase.MetaOverlay);
		OutOfFuelMat = MaterialPool.MatFrom("UI/Overlays/OutOfFuel", ShaderDatabase.MetaOverlay);
		WickMaterialA = MaterialPool.MatFrom("Things/Special/BurningWickA", ShaderDatabase.MetaOverlay);
		WickMaterialB = MaterialPool.MatFrom("Things/Special/BurningWickB", ShaderDatabase.MetaOverlay);
		SelfShutdownMat = MaterialPool.MatFrom("UI/Overlays/SelfShutdown", ShaderDatabase.MetaOverlay);
		SingleCellForbiddenOffset = 0.3f;
		BaseAlt = AltitudeLayer.MetaOverlays.AltitudeFor();
	}

	public ThingOverlaysHandle GetOverlaysHandle(Thing thing)
	{
		if (!thing.Spawned)
		{
			return null;
		}
		if (!overlayHandles.TryGetValue(thing, out var value))
		{
			value = new ThingOverlaysHandle(this, thing);
			overlayHandles.Add(thing, value);
		}
		return value;
	}

	public void DisposeHandle(Thing thing)
	{
		if (overlayHandles.TryGetValue(thing, out var value))
		{
			value.Dispose();
		}
		overlayHandles.Remove(thing);
	}

	public OverlayHandle Enable(Thing thing, OverlayTypes types)
	{
		return GetOverlaysHandle(thing).Enable(types);
	}

	public void Disable(Thing thing, ref OverlayHandle? handle)
	{
		GetOverlaysHandle(thing).Disable(ref handle);
	}

	public void DrawOverlay(Thing t, OverlayTypes overlayType)
	{
		if (overlayType != OverlayTypes.None)
		{
			if (overlaysToDraw.TryGetValue(t, out var value))
			{
				overlaysToDraw[t] = value | overlayType;
			}
			else
			{
				overlaysToDraw.Add(t, overlayType);
			}
		}
	}

	public void DrawAllOverlays()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			foreach (KeyValuePair<Thing, ThingOverlaysHandle> overlayHandle in overlayHandles)
			{
				if (!overlayHandle.Key.Fogged())
				{
					DrawOverlay(overlayHandle.Key, overlayHandle.Value.OverlayTypes);
				}
			}
			foreach (KeyValuePair<Thing, OverlayTypes> item in overlaysToDraw)
			{
				curOffset = Vector3.zero;
				Thing key = item.Key;
				OverlayTypes value = item.Value;
				if ((value & OverlayTypes.BurningWick) != OverlayTypes.None)
				{
					RenderBurningWick(key);
				}
				else
				{
					OverlayTypes overlayTypes = OverlayTypes.NeedsPower | OverlayTypes.PowerOff;
					int bitCountOf = Gen.GetBitCountOf((long)(value & overlayTypes));
					float num = StackOffsetFor(key);
					switch (bitCountOf)
					{
					case 1:
						curOffset = Vector3.zero;
						break;
					case 2:
						curOffset = new Vector3(-0.5f * num, 0f, 0f);
						break;
					case 3:
						curOffset = new Vector3(-1.5f * num, 0f, 0f);
						break;
					}
					if ((value & OverlayTypes.NeedsPower) != OverlayTypes.None)
					{
						RenderNeedsPowerOverlay(key);
					}
					if ((value & OverlayTypes.PowerOff) != OverlayTypes.None)
					{
						RenderPowerOffOverlay(key);
					}
					if ((value & OverlayTypes.BrokenDown) != OverlayTypes.None)
					{
						RenderBrokenDownOverlay(key);
					}
					if ((value & OverlayTypes.OutOfFuel) != OverlayTypes.None)
					{
						RenderOutOfFuelOverlay(key);
					}
				}
				if ((value & OverlayTypes.ForbiddenBig) != OverlayTypes.None)
				{
					RenderForbiddenBigOverlay(key);
				}
				if ((value & OverlayTypes.Forbidden) != OverlayTypes.None)
				{
					RenderForbiddenOverlay(key);
				}
				if ((value & OverlayTypes.ForbiddenRefuel) != OverlayTypes.None)
				{
					RenderForbiddenRefuelOverlay(key);
				}
				if ((value & OverlayTypes.QuestionMark) != OverlayTypes.None)
				{
					RenderQuestionMarkOverlay(key);
				}
				if ((value & OverlayTypes.SelfShutdown) != OverlayTypes.None && ModsConfig.BiotechActive)
				{
					RenderRechargineOverlay(key);
				}
				if ((value & OverlayTypes.ForbiddenAtomizer) != OverlayTypes.None && ModsConfig.BiotechActive)
				{
					RenderForbiddenAtomizerOverlay(key);
				}
			}
		}
		finally
		{
			overlaysToDraw.Clear();
		}
		drawBatch.Flush();
	}

	private float StackOffsetFor(Thing t)
	{
		return (float)t.RotatedSize.x * 0.25f;
	}

	private void RenderNeedsPowerOverlay(Thing t)
	{
		RenderPulsingOverlay(t, NeedsPowerMat, 2);
	}

	private void RenderPowerOffOverlay(Thing t)
	{
		RenderPulsingOverlay(t, PowerOffMat, 3);
	}

	private void RenderBrokenDownOverlay(Thing t)
	{
		RenderPulsingOverlay(t, BrokenDownMat, 4);
	}

	private void RenderRechargineOverlay(Thing t)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Vector3 drawPos = t.DrawPos;
		drawPos.y = BaseAlt + 0.23076925f;
		RenderPulsingOverlayInternal(t, SelfShutdownMat, drawPos, MeshPool.plane05);
	}

	private void RenderOutOfFuelOverlay(Thing t)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		CompRefuelable compRefuelable = t.TryGetComp<CompRefuelable>();
		Material mat = MaterialPool.MatFrom((compRefuelable != null) ? compRefuelable.Props.FuelIcon : ThingDefOf.Chemfuel.uiIcon, ShaderDatabase.MetaOverlay, Color.white);
		RenderPulsingOverlay(t, mat, 5, incrementOffset: false);
		RenderPulsingOverlay(t, OutOfFuelMat, 6);
	}

	private void RenderPulsingOverlay(Thing thing, Material mat, int altInd, bool incrementOffset = true)
	{
		Mesh plane = MeshPool.plane08;
		RenderPulsingOverlay(thing, mat, altInd, plane, incrementOffset);
	}

	private void RenderPulsingOverlay(Thing thing, Material mat, int altInd, Mesh mesh, bool incrementOffset = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = thing.TrueCenter();
		val.y = BaseAlt + 1f / 26f * (float)altInd;
		val += curOffset;
		if (thing.def.building != null && thing.def.building.isAttachment)
		{
			val += (thing.Rotation.AsVector2 * 0.5f).ToVector3();
		}
		val.y = Mathf.Min(val.y, ((Component)Find.Camera).transform.position.y - 1f);
		if (incrementOffset)
		{
			curOffset.x += StackOffsetFor(thing);
		}
		RenderPulsingOverlayInternal(thing, mat, val, mesh);
	}

	private void RenderPulsingOverlayInternal(Thing thing, Material mat, Vector3 drawPos, Mesh mesh)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		float num = ((float)Math.Sin((Time.realtimeSinceStartup + 397f * (float)(thing.thingIDNumber % 571)) * 4f) + 1f) * 0.5f;
		num = 0.3f + num * 0.7f;
		Material material = FadedMaterialPool.FadedVersionOf(mat, num);
		drawBatch.DrawMesh(mesh, Matrix4x4.TRS(drawPos, Quaternion.identity, Vector3.one), material, 0, renderInstanced: true);
	}

	private void RenderForbiddenRefuelOverlay(Thing t)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		CompRefuelable compRefuelable = t.TryGetComp<CompRefuelable>();
		Material material = MaterialPool.MatFrom((compRefuelable != null) ? compRefuelable.Props.FuelIcon : ThingDefOf.Chemfuel.uiIcon, ShaderDatabase.MetaOverlayDesaturated, Color.white);
		Vector3 val = t.TrueCenter();
		val.y = BaseAlt + 5f / 26f;
		new Vector3(val.x, val.y + 1f / 26f, val.z);
		drawBatch.DrawMesh(MeshPool.plane08, Matrix4x4.TRS(val, Quaternion.identity, Vector3.one), material, 0, renderInstanced: true);
		drawBatch.DrawMesh(MeshPool.plane08, Matrix4x4.TRS(val, Quaternion.identity, Vector3.one), ForbiddenMat, 0, renderInstanced: true);
	}

	private void RenderForbiddenAtomizerOverlay(Thing t)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (ModsConfig.BiotechActive)
		{
			t.TryGetComp<CompAtomizer>();
			Material material = MaterialPool.MatFrom(ThingDefOf.Wastepack.uiIcon, ShaderDatabase.MetaOverlayDesaturated, Color.white);
			Vector3 val = t.TrueCenter();
			val.y = BaseAlt + 5f / 26f;
			drawBatch.DrawMesh(MeshPool.plane08, Matrix4x4.TRS(val, Quaternion.identity, Vector3.one), material, 0, renderInstanced: true);
			drawBatch.DrawMesh(MeshPool.plane08, Matrix4x4.TRS(val, Quaternion.identity, Vector3.one), ForbiddenMat, 0, renderInstanced: true);
		}
	}

	private void RenderForbiddenOverlay(Thing t)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = t.DrawPos;
		bool? flag = (t.def.entityDefToBuild as ThingDef)?.building?.isAttachment;
		bool num;
		if (!flag.HasValue)
		{
			BuildingProperties building = t.def.building;
			if (building == null)
			{
				goto IL_0094;
			}
			num = building.isAttachment;
		}
		else
		{
			num = flag == true;
		}
		if (num)
		{
			val += (t.Rotation.AsVector2 * 0.5f).ToVector3();
		}
		goto IL_0094;
		IL_0094:
		if (t.RotatedSize.z == 1)
		{
			val.z -= SingleCellForbiddenOffset;
		}
		else
		{
			val.z -= (float)t.RotatedSize.z * 0.3f;
		}
		val.y = BaseAlt + 0.15384616f;
		drawBatch.DrawMesh(MeshPool.plane05, Matrix4x4.TRS(val, Quaternion.identity, Vector3.one), ForbiddenMat, 0, renderInstanced: true);
	}

	private void RenderForbiddenBigOverlay(Thing t)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = t.DrawPos;
		bool? flag = (t.def.entityDefToBuild as ThingDef)?.building?.isAttachment;
		bool num;
		if (!flag.HasValue)
		{
			BuildingProperties building = t.def.building;
			if (building == null)
			{
				goto IL_0094;
			}
			num = building.isAttachment;
		}
		else
		{
			num = flag == true;
		}
		if (num)
		{
			val += (t.Rotation.AsVector2 * 0.5f).ToVector3();
		}
		goto IL_0094;
		IL_0094:
		val.y = BaseAlt + 0.15384616f;
		drawBatch.DrawMesh(MeshPool.plane10, Matrix4x4.TRS(val, Quaternion.identity, Vector3.one), ForbiddenMat, 0, renderInstanced: true);
	}

	private void RenderBurningWick(Thing parent)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		Material material = (((parent.thingIDNumber + Find.TickManager.TicksGame) % 6 >= 3) ? WickMaterialB : WickMaterialA);
		Vector3 drawPos = parent.DrawPos;
		drawPos.y = BaseAlt + 5f / 26f;
		drawBatch.DrawMesh(MeshPool.plane20, Matrix4x4.TRS(drawPos, Quaternion.identity, Vector3.one), material, 0, renderInstanced: true);
	}

	private void RenderQuestionMarkOverlay(Thing t)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 drawPos = t.DrawPos;
		drawPos.y = BaseAlt + 0.23076925f;
		if (t is Pawn)
		{
			drawPos.x += (float)t.def.size.x - 0.52f;
			drawPos.z += (float)t.def.size.z - 0.45f;
		}
		RenderPulsingOverlayInternal(t, QuestionMarkMat, drawPos, MeshPool.plane05);
	}
}
