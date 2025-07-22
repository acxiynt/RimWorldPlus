using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace RimWorld.Planet;

public class WorldLayer_Roads : WorldLayer_Paths
{
	private ModuleBase roadDisplacementX = new Perlin(1.0, 2.0, 0.5, 3, 74173887, QualityMode.Medium);

	private ModuleBase roadDisplacementY = new Perlin(1.0, 2.0, 0.5, 3, 67515931, QualityMode.Medium);

	private ModuleBase roadDisplacementZ = new Perlin(1.0, 2.0, 0.5, 3, 87116801, QualityMode.Medium);

	public override IEnumerable Regenerate()
	{
		foreach (object item in base.Regenerate())
		{
			yield return item;
		}
		LayerSubMesh subMesh = GetSubMesh(WorldMaterials.Roads);
		WorldGrid grid = Find.WorldGrid;
		List<RoadWorldLayerDef> roadLayerDefs = DefDatabase<RoadWorldLayerDef>.AllDefs.OrderBy((RoadWorldLayerDef rwld) => rwld.order).ToList();
		int i = 0;
		while (i < grid.TilesCount)
		{
			if (i % 1000 == 0)
			{
				yield return null;
			}
			if (subMesh.verts.Count > 60000)
			{
				subMesh = GetSubMesh(WorldMaterials.Roads);
			}
			Tile tile = grid[i];
			if (!tile.WaterCovered)
			{
				List<OutputDirection> list = new List<OutputDirection>();
				if (tile.potentialRoads != null)
				{
					bool allowSmoothTransition = true;
					for (int num = 0; num < tile.potentialRoads.Count - 1; num++)
					{
						if (tile.potentialRoads[num].road.worldTransitionGroup != tile.potentialRoads[num + 1].road.worldTransitionGroup)
						{
							allowSmoothTransition = false;
						}
					}
					for (int num2 = 0; num2 < roadLayerDefs.Count; num2++)
					{
						bool flag = false;
						list.Clear();
						for (int num3 = 0; num3 < tile.potentialRoads.Count; num3++)
						{
							RoadDef road = tile.potentialRoads[num3].road;
							float layerWidth = road.GetLayerWidth(roadLayerDefs[num2]);
							if (layerWidth > 0f)
							{
								flag = true;
							}
							list.Add(new OutputDirection
							{
								neighbor = tile.potentialRoads[num3].neighbor,
								width = layerWidth,
								distortionFrequency = road.distortionFrequency,
								distortionIntensity = road.distortionIntensity
							});
						}
						if (flag)
						{
							GeneratePaths(subMesh, i, list, Color32.op_Implicit(roadLayerDefs[num2].color), allowSmoothTransition);
						}
					}
				}
			}
			int num4 = i + 1;
			i = num4;
		}
		FinalizeMesh(MeshParts.All);
	}

	public override Vector3 FinalizePoint(Vector3 inp, float distortionFrequency, float distortionIntensity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		Vector3 coordinate = inp * distortionFrequency;
		float magnitude = ((Vector3)(ref inp)).magnitude;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(roadDisplacementX.GetValue(coordinate), roadDisplacementY.GetValue(coordinate), roadDisplacementZ.GetValue(coordinate));
		if ((double)((Vector3)(ref val)).magnitude > 0.0001)
		{
			float num = (1f / (1f + Mathf.Exp((0f - ((Vector3)(ref val)).magnitude) / 1f * 2f)) * 2f - 1f) * 1f;
			val = ((Vector3)(ref val)).normalized * num;
		}
		Vector3 val2 = inp + val * distortionIntensity;
		inp = ((Vector3)(ref val2)).normalized * magnitude;
		return inp + ((Vector3)(ref inp)).normalized * 0.012f;
	}
}
