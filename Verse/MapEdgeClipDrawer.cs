using RimWorld;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public static class MapEdgeClipDrawer
{
	public static readonly Material ClipMat = SolidColorMaterials.NewSolidColorMaterial(new Color(0.1f, 0.1f, 0.1f), ShaderDatabase.MetaOverlay);

	public static readonly Material ClipMatMetalhell = SolidColorMaterials.NewSolidColorMaterial(new Color(0.03f, 0.04f, 0.04f), ShaderDatabase.MetaOverlay);

	private static readonly float ClipAltitude = AltitudeLayer.WorldClipper.AltitudeFor();

	private const float ClipWidth = 500f;

	public static void DrawClippers(Map map)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		Material val = ClipMat;
		if (ModsConfig.AnomalyActive && Find.CurrentMap?.generatorDef == MapGeneratorDefOf.MetalHell)
		{
			val = ClipMatMetalhell;
		}
		IntVec3 size = map.Size;
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(500f, 1f, (float)size.z);
		Matrix4x4 val3 = default(Matrix4x4);
		((Matrix4x4)(ref val3)).SetTRS(new Vector3(-250f, ClipAltitude, (float)size.z / 2f), Quaternion.identity, val2);
		Graphics.DrawMesh(MeshPool.plane10, val3, val, 0);
		val3 = default(Matrix4x4);
		((Matrix4x4)(ref val3)).SetTRS(new Vector3((float)size.x + 250f, ClipAltitude, (float)size.z / 2f), Quaternion.identity, val2);
		Graphics.DrawMesh(MeshPool.plane10, val3, val, 0);
		((Vector3)(ref val2))._002Ector(1000f, 1f, 500f);
		val3 = default(Matrix4x4);
		((Matrix4x4)(ref val3)).SetTRS(new Vector3((float)(size.x / 2), ClipAltitude, (float)size.z + 250f), Quaternion.identity, val2);
		Graphics.DrawMesh(MeshPool.plane10, val3, val, 0);
		val3 = default(Matrix4x4);
		((Matrix4x4)(ref val3)).SetTRS(new Vector3((float)(size.x / 2), ClipAltitude, -250f), Quaternion.identity, val2);
		Graphics.DrawMesh(MeshPool.plane10, val3, val, 0);
	}
}
