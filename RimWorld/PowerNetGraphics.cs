using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public static class PowerNetGraphics
{
	private const AltitudeLayer WireAltitude = AltitudeLayer.SmallWire;

	private static readonly Material WireMat = MaterialPool.MatFrom("Things/Special/Power/Wire");

	public static void PrintWirePieceConnecting(SectionLayer layer, Thing A, Thing B, bool forPowerOverlay)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		Material mat = WireMat;
		float y = AltitudeLayer.SmallWire.AltitudeFor();
		if (forPowerOverlay)
		{
			mat = PowerOverlayMats.MatConnectorLine;
			y = AltitudeLayer.MapDataOverlay.AltitudeFor();
		}
		Vector3 center = (A.TrueCenter() + A.Graphic.DrawOffset(A.Rotation) + B.TrueCenter() + B.Graphic.DrawOffset(B.Rotation)) / 2f;
		center.y = y;
		Vector3 v = B.TrueCenter() - A.TrueCenter();
		Vector2 size = default(Vector2);
		((Vector2)(ref size))._002Ector(1f, v.MagnitudeHorizontal());
		float rot = v.AngleFlat();
		Printer_Plane.PrintPlane(layer, center, size, mat, rot);
	}

	public static void RenderAnticipatedWirePieceConnecting(IntVec3 userPos, Rot4 rotation, IntVec2 thingSize, Thing transmitter)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = GenThing.TrueCenter(userPos, rotation, thingSize, AltitudeLayer.MapDataOverlay.AltitudeFor());
		if (userPos != transmitter.Position)
		{
			Vector3 val2 = transmitter.TrueCenter();
			val2.y = AltitudeLayer.MapDataOverlay.AltitudeFor();
			Vector3 val3 = (val + val2) / 2f;
			Vector3 v = val2 - val;
			Vector3 val4 = default(Vector3);
			((Vector3)(ref val4))._002Ector(1f, 1f, v.MagnitudeHorizontal());
			Quaternion val5 = Quaternion.LookRotation(val2 - val);
			Matrix4x4 val6 = default(Matrix4x4);
			((Matrix4x4)(ref val6)).SetTRS(val3, val5, val4);
			Graphics.DrawMesh(MeshPool.plane10, val6, PowerOverlayMats.MatConnectorAnticipated, 0);
		}
	}

	public static void PrintOverlayConnectorBaseFor(SectionLayer layer, Thing t)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Vector3 center = t.TrueCenter();
		center.y = AltitudeLayer.MapDataOverlay.AltitudeFor();
		Printer_Plane.PrintPlane(layer, center, new Vector2(1f, 1f), PowerOverlayMats.MatConnectorBase);
	}
}
