using RimWorld;
using UnityEngine;

namespace Verse;

public class Graphic_Flicker : Graphic_Collection
{
	private const int BaseTicksPerFrameChange = 15;

	private const float MaxOffset = 0.05f;

	public override Material MatSingle => subGraphics[Rand.Range(0, subGraphics.Length)].MatSingle;

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		if (thingDef == null)
		{
			Log.ErrorOnce("Fire DrawWorker with null thingDef: " + loc, 3427324);
			return;
		}
		if (subGraphics == null)
		{
			Log.ErrorOnce("Graphic_Flicker has no subgraphics " + thingDef, 358773632);
			return;
		}
		int num = Find.TickManager.TicksGame;
		if (thing != null)
		{
			num += Mathf.Abs(thing.thingIDNumber ^ 0x80FD52);
		}
		int num2 = num / 15;
		int num3 = Mathf.Abs(num2 ^ ((thing?.thingIDNumber ?? 0) * 391)) % subGraphics.Length;
		float num4 = 1f;
		CompFireOverlayBase compFireOverlayBase = null;
		Fire fire = thing as Fire;
		CompProperties_FireOverlay compProperties = thingDef.GetCompProperties<CompProperties_FireOverlay>();
		if (fire != null)
		{
			num4 = fire.fireSize;
		}
		else if (thing != null)
		{
			compFireOverlayBase = thing.TryGetComp<CompFireOverlayBase>();
			if (compFireOverlayBase != null)
			{
				num4 = compFireOverlayBase.FireSize;
			}
			else
			{
				compFireOverlayBase = thing.TryGetComp<CompDarklightOverlay>();
				if (compFireOverlayBase != null)
				{
					num4 = compFireOverlayBase.FireSize;
				}
			}
		}
		else if (compProperties != null)
		{
			num4 = compProperties.fireSize;
		}
		if (num3 < 0 || num3 >= subGraphics.Length)
		{
			Log.ErrorOnce("Fire drawing out of range: " + num3, 7453435);
			num3 = 0;
		}
		Graphic graphic = subGraphics[num3];
		float num5 = ((compFireOverlayBase == null) ? Mathf.Min(num4 / 1.2f, 1.2f) : num4);
		Vector3 val = GenRadial.RadialPattern[num2 % GenRadial.RadialPattern.Length].ToVector3() / GenRadial.MaxRadialPatternRadius;
		val *= 0.05f;
		Vector3 val2 = loc + val * num4;
		if (thing?.Graphic?.data != null)
		{
			val2 += thing.Graphic.data.DrawOffsetForRot(rot);
		}
		if (compFireOverlayBase != null)
		{
			val2 += compFireOverlayBase.Props.DrawOffsetForRot(rot);
		}
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(num5, 1f, num5);
		Matrix4x4 val4 = default(Matrix4x4);
		((Matrix4x4)(ref val4)).SetTRS(val2, Quaternion.identity, val3);
		Graphics.DrawMesh(MeshPool.plane10, val4, graphic.MatSingle, 0);
	}

	public override string ToString()
	{
		return string.Concat("Flicker(subGraphic[0]=", subGraphics[0], ", count=", subGraphics.Length, ")");
	}
}
