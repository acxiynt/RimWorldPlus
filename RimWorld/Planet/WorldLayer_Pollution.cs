using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class WorldLayer_Pollution : WorldLayer
{
	private const int TilesPerSubMesh = 500;

	private const float ScaleUVFactor = 0.1f;

	private static readonly Color DefaultTileColor = Color.white;

	private static readonly Color BordersUnpollutedTileColor = new Color(1f, 1f, 1f, 0.4f);

	private List<Vector3> verts = new List<Vector3>();

	private Dictionary<int, List<LayerSubMesh>> subMeshesByRegion = new Dictionary<int, List<LayerSubMesh>>();

	private Queue<int> regionsToRegenerate = new Queue<int>();

	private Material lightPollution;

	private Material moderatePollution;

	private Material extemePollution;

	private List<int> tmpNeighbors = new List<int>();

	private HashSet<Vector3> tmpBordersUnpollutedVerts = new HashSet<Vector3>();

	private List<Vector3> tmpVerts = new List<Vector3>();

	private static List<int> tmpChangedNeighbours = new List<int>();

	private Material LightPollution
	{
		get
		{
			if ((Object)(object)lightPollution == (Object)null)
			{
				lightPollution = MaterialPool.MatFrom("World/Pollution/Light", ShaderDatabase.WorldOverlayTransparentLitPollution, 3510);
			}
			return lightPollution;
		}
	}

	private Material ModeratePollution
	{
		get
		{
			if ((Object)(object)moderatePollution == (Object)null)
			{
				moderatePollution = MaterialPool.MatFrom("World/Pollution/Moderate", ShaderDatabase.WorldOverlayTransparentLitPollution, 3510);
			}
			return moderatePollution;
		}
	}

	private Material ExtremePollution
	{
		get
		{
			if ((Object)(object)extemePollution == (Object)null)
			{
				extemePollution = MaterialPool.MatFrom("World/Pollution/Extreme", ShaderDatabase.WorldOverlayTransparentLitPollution, 3510);
			}
			return extemePollution;
		}
	}

	private int GetRegionIdForTile(int tileId)
	{
		return Mathf.FloorToInt((float)tileId / 500f);
	}

	public List<LayerSubMesh> GetSubMeshesForRegion(int regionId)
	{
		if (!subMeshesByRegion.ContainsKey(regionId))
		{
			subMeshesByRegion[regionId] = new List<LayerSubMesh>();
		}
		return subMeshesByRegion[regionId];
	}

	public LayerSubMesh GetSubMeshForMaterialAndRegion(Material material, int regionId)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		List<LayerSubMesh> subMeshesForRegion = GetSubMeshesForRegion(regionId);
		for (int i = 0; i < subMeshesForRegion.Count; i++)
		{
			if ((Object)(object)subMeshesForRegion[i].material == (Object)(object)material)
			{
				return subMeshesForRegion[i];
			}
		}
		Mesh val = new Mesh();
		if (UnityData.isEditor)
		{
			((Object)val).name = "WorldLayerSubMesh_" + GetType().Name + "_" + Find.World.info.seedString;
		}
		LayerSubMesh layerSubMesh = new LayerSubMesh(val, material);
		subMeshesForRegion.Add(layerSubMesh);
		subMeshes.Add(layerSubMesh);
		return layerSubMesh;
	}

	private void RegnerateRegion(int regionId)
	{
		List<LayerSubMesh> subMeshesForRegion = GetSubMeshesForRegion(regionId);
		for (int i = 0; i < subMeshesForRegion.Count; i++)
		{
			subMeshesForRegion[i].Clear(MeshParts.All);
		}
		int num = regionId * 500;
		int num2 = num + 500;
		for (int j = num; j < num2 && Find.World.grid.InBounds(j); j++)
		{
			TryAddMeshForTile(j);
		}
		for (int k = 0; k < subMeshesForRegion.Count; k++)
		{
			if (subMeshesForRegion[k].verts.Count > 0)
			{
				subMeshesForRegion[k].FinalizeMesh(MeshParts.All);
			}
		}
	}

	public override IEnumerable Regenerate()
	{
		if (!ModsConfig.BiotechActive)
		{
			yield break;
		}
		foreach (object item in base.Regenerate())
		{
			yield return item;
		}
		int num = 500;
		Mathf.CeilToInt((float)Find.WorldGrid.TilesCount / (float)num);
		WorldGrid worldGrid = Find.WorldGrid;
		int tilesCount = worldGrid.TilesCount;
		int pollutedMeshesPrinted = 0;
		verts.Clear();
		subMeshesByRegion.Clear();
		regionsToRegenerate.Clear();
		for (int i = 0; i < tilesCount; i++)
		{
			if (TryAddMeshForTile(i))
			{
				pollutedMeshesPrinted++;
				if (pollutedMeshesPrinted % 1000 == 0)
				{
					yield return null;
				}
			}
		}
		FinalizeMesh(MeshParts.All);
	}

	private bool TryAddMeshForTile(int tileId)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		PollutionLevel pollution = Find.World.grid[tileId].PollutionLevel();
		Material materialForTilePollution = GetMaterialForTilePollution(pollution);
		if ((Object)(object)materialForTilePollution == (Object)null)
		{
			return false;
		}
		int regionIdForTile = GetRegionIdForTile(tileId);
		LayerSubMesh subMeshForMaterialAndRegion = GetSubMeshForMaterialAndRegion(materialForTilePollution, regionIdForTile);
		Find.WorldGrid.GetTileVertices(tileId, verts);
		Find.WorldGrid.GetTileNeighbors(tileId, tmpNeighbors);
		int count = subMeshForMaterialAndRegion.verts.Count;
		tmpBordersUnpollutedVerts.Clear();
		tmpVerts.Clear();
		for (int i = 0; i < tmpNeighbors.Count; i++)
		{
			if (Find.World.grid[tmpNeighbors[i]].PollutionLevel() >= PollutionLevel.Moderate)
			{
				continue;
			}
			Vector3 center = Find.WorldGrid.GetTileCenter(tmpNeighbors[i]);
			tmpVerts.AddRange(verts);
			tmpVerts.SortBy((Vector3 v) => Vector2.Distance(Vector2.op_Implicit(center), Vector2.op_Implicit(v)));
			for (int num = 0; num < 2; num++)
			{
				if (!tmpBordersUnpollutedVerts.Contains(tmpVerts[num]))
				{
					tmpBordersUnpollutedVerts.Add(tmpVerts[num]);
				}
			}
		}
		int num2 = 0;
		for (int count2 = verts.Count; num2 < count2; num2++)
		{
			Vector3 val = verts[num2];
			Vector3 val2 = verts[num2];
			Vector3 val3 = val + ((Vector3)(ref val2)).normalized * 0.012f;
			subMeshForMaterialAndRegion.verts.Add(val3);
			subMeshForMaterialAndRegion.uvs.Add(val3 * 0.1f);
			Color val4 = (tmpBordersUnpollutedVerts.Contains(verts[num2]) ? BordersUnpollutedTileColor : DefaultTileColor);
			subMeshForMaterialAndRegion.colors.Add(Color32.op_Implicit(val4));
			if (num2 < count2 - 2)
			{
				subMeshForMaterialAndRegion.tris.Add(count + num2 + 2);
				subMeshForMaterialAndRegion.tris.Add(count + num2 + 1);
				subMeshForMaterialAndRegion.tris.Add(count);
			}
		}
		tmpBordersUnpollutedVerts.Clear();
		tmpVerts.Clear();
		return true;
	}

	private Material GetMaterialForTilePollution(PollutionLevel pollution)
	{
		return (Material)(pollution switch
		{
			PollutionLevel.Light => LightPollution, 
			PollutionLevel.Moderate => ModeratePollution, 
			PollutionLevel.Extreme => ExtremePollution, 
			_ => null, 
		});
	}

	public void Notify_TilePollutionChanged(int tileId)
	{
		int regionIdForTile = GetRegionIdForTile(tileId);
		if (!regionsToRegenerate.Contains(regionIdForTile))
		{
			regionsToRegenerate.Enqueue(regionIdForTile);
		}
		Find.WorldGrid.GetTileNeighbors(tileId, tmpChangedNeighbours);
		for (int i = 0; i < tmpChangedNeighbours.Count; i++)
		{
			int regionIdForTile2 = GetRegionIdForTile(tmpChangedNeighbours[i]);
			if (!regionsToRegenerate.Contains(regionIdForTile2))
			{
				regionsToRegenerate.Enqueue(regionIdForTile2);
			}
		}
		tmpChangedNeighbours.Clear();
	}

	public override void Render()
	{
		if (regionsToRegenerate.Count > 0)
		{
			int regionId = regionsToRegenerate.Dequeue();
			RegnerateRegion(regionId);
		}
		base.Render();
	}
}
