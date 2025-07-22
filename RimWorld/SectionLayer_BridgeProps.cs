using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class SectionLayer_BridgeProps : SectionLayer
{
	private static readonly Material PropsLoopMat = MaterialPool.MatFrom("Terrain/Misc/BridgeProps_Loop", ShaderDatabase.Transparent);

	private static readonly Material PropsRightMat = MaterialPool.MatFrom("Terrain/Misc/BridgeProps_Right", ShaderDatabase.Transparent);

	public override bool Visible => DebugViewSettings.drawTerrain;

	public SectionLayer_BridgeProps(Section section)
		: base(section)
	{
		relevantChangeTypes = MapMeshFlagDefOf.Terrain;
	}

	public override void Regenerate()
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		ClearSubMeshes(MeshParts.All);
		Map map = base.Map;
		TerrainGrid terrainGrid = map.terrainGrid;
		CellRect cellRect = section.CellRect;
		float num = AltitudeLayer.TerrainScatter.AltitudeFor();
		foreach (IntVec3 item in cellRect)
		{
			if (ShouldDrawPropsBelow(item, terrainGrid))
			{
				IntVec3 c = item;
				c.x++;
				Material material = ((!c.InBounds(map) || !ShouldDrawPropsBelow(c, terrainGrid)) ? PropsRightMat : PropsLoopMat);
				LayerSubMesh subMesh = GetSubMesh(material);
				int count = subMesh.verts.Count;
				subMesh.verts.Add(new Vector3((float)item.x, num, (float)(item.z - 1)));
				subMesh.verts.Add(new Vector3((float)item.x, num, (float)item.z));
				subMesh.verts.Add(new Vector3((float)(item.x + 1), num, (float)item.z));
				subMesh.verts.Add(new Vector3((float)(item.x + 1), num, (float)(item.z - 1)));
				subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(0f, 0f)));
				subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(0f, 1f)));
				subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(1f, 1f)));
				subMesh.uvs.Add(Vector2.op_Implicit(new Vector2(1f, 0f)));
				subMesh.tris.Add(count);
				subMesh.tris.Add(count + 1);
				subMesh.tris.Add(count + 2);
				subMesh.tris.Add(count);
				subMesh.tris.Add(count + 2);
				subMesh.tris.Add(count + 3);
			}
		}
		FinalizeMesh(MeshParts.All);
	}

	private bool ShouldDrawPropsBelow(IntVec3 c, TerrainGrid terrGrid)
	{
		TerrainDef terrainDef = terrGrid.TerrainAt(c);
		if (terrainDef == null || !terrainDef.bridge)
		{
			return false;
		}
		IntVec3 c2 = c;
		c2.z--;
		Map map = base.Map;
		if (!c2.InBounds(map))
		{
			return false;
		}
		TerrainDef terrainDef2 = terrGrid.TerrainAt(c2);
		if (terrainDef2.bridge)
		{
			return false;
		}
		if (terrainDef2.passability != Traversability.Impassable && !c2.SupportsStructureType(map, TerrainDefOf.Bridge.terrainAffordanceNeeded))
		{
			return false;
		}
		return true;
	}
}
