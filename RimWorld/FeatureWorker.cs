using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class FeatureWorker
{
	public FeatureDef def;

	protected static bool[] visited;

	protected static int[] groupSize;

	protected static int[] groupID;

	private static List<int> tmpNeighbors = new List<int>();

	private static HashSet<int> tmpTilesForTextDrawPosCalculationSet = new HashSet<int>();

	private static List<int> tmpEdgeTiles = new List<int>();

	private static List<Pair<int, int>> tmpTraversedTiles = new List<Pair<int, int>>();

	public abstract void GenerateWhereAppropriate();

	protected void AddFeature(List<int> members, List<int> tilesForTextDrawPosCalculation)
	{
		WorldFeature worldFeature = new WorldFeature();
		worldFeature.uniqueID = Find.UniqueIDsManager.GetNextWorldFeatureID();
		worldFeature.def = def;
		worldFeature.name = NameGenerator.GenerateName(def.nameMaker, Find.WorldFeatures.features.Select((WorldFeature x) => x.name), appendNumberIfNameUsed: false, "r_name");
		WorldGrid worldGrid = Find.WorldGrid;
		for (int num = 0; num < members.Count; num++)
		{
			worldGrid[members[num]].feature = worldFeature;
		}
		AssignBestDrawPos(worldFeature, tilesForTextDrawPosCalculation);
		Find.WorldFeatures.features.Add(worldFeature);
	}

	private void AssignBestDrawPos(WorldFeature newFeature, List<int> tilesForTextDrawPosCalculation)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		WorldGrid worldGrid = Find.WorldGrid;
		tmpEdgeTiles.Clear();
		tmpTilesForTextDrawPosCalculationSet.Clear();
		tmpTilesForTextDrawPosCalculationSet.AddRange(tilesForTextDrawPosCalculation);
		Vector3 val = Vector3.zero;
		for (int i = 0; i < tilesForTextDrawPosCalculation.Count; i++)
		{
			int num = tilesForTextDrawPosCalculation[i];
			val += worldGrid.GetTileCenter(num);
			bool flag = worldGrid.IsOnEdge(num);
			if (!flag)
			{
				worldGrid.GetTileNeighbors(num, tmpNeighbors);
				for (int j = 0; j < tmpNeighbors.Count; j++)
				{
					if (!tmpTilesForTextDrawPosCalculationSet.Contains(tmpNeighbors[j]))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				tmpEdgeTiles.Add(num);
			}
		}
		val /= (float)tilesForTextDrawPosCalculation.Count;
		if (!tmpEdgeTiles.Any())
		{
			tmpEdgeTiles.Add(tilesForTextDrawPosCalculation.RandomElement());
		}
		int bestTileDist = 0;
		tmpTraversedTiles.Clear();
		Find.WorldFloodFiller.FloodFill(-1, (int x) => tmpTilesForTextDrawPosCalculationSet.Contains(x), delegate(int tile, int traversalDist)
		{
			tmpTraversedTiles.Add(new Pair<int, int>(tile, traversalDist));
			bestTileDist = traversalDist;
			return false;
		}, int.MaxValue, tmpEdgeTiles);
		int num2 = -1;
		float num3 = -1f;
		for (int num4 = 0; num4 < tmpTraversedTiles.Count; num4++)
		{
			if (tmpTraversedTiles[num4].Second == bestTileDist)
			{
				Vector3 val2 = worldGrid.GetTileCenter(tmpTraversedTiles[num4].First) - val;
				float sqrMagnitude = ((Vector3)(ref val2)).sqrMagnitude;
				if (num2 == -1 || sqrMagnitude < num3)
				{
					num2 = tmpTraversedTiles[num4].First;
					num3 = sqrMagnitude;
				}
			}
		}
		float maxDrawSizeInTiles = (float)bestTileDist * 2f * 1.2f;
		newFeature.drawCenter = worldGrid.GetTileCenter(num2);
		newFeature.maxDrawSizeInTiles = maxDrawSizeInTiles;
	}

	protected static void ClearVisited()
	{
		ClearOrCreate(ref visited);
	}

	protected static void ClearGroupSizes()
	{
		ClearOrCreate(ref groupSize);
	}

	protected static void ClearGroupIDs()
	{
		ClearOrCreate(ref groupID);
	}

	private static void ClearOrCreate<T>(ref T[] array)
	{
		int tilesCount = Find.WorldGrid.TilesCount;
		if (array == null || array.Length != tilesCount)
		{
			array = new T[tilesCount];
		}
		else
		{
			Array.Clear(array, 0, array.Length);
		}
	}
}
