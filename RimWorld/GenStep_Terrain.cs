using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public class GenStep_Terrain : GenStep
{
	private struct GRLT_Entry
	{
		public float bestDistance;

		public IntVec3 bestNode;
	}

	private static bool debug_WarnedMissingTerrain = false;

	private static HashSet<IntVec3> tmpVisited = new HashSet<IntVec3>();

	private static List<IntVec3> tmpIsland = new List<IntVec3>();

	public override int SeedPart => 262606459;

	public override void Generate(Map map, GenStepParams parms)
	{
		BeachMaker.Init(map);
		RiverMaker riverMaker = GenerateRiver(map);
		List<IntVec3> list = new List<IntVec3>();
		MapGenFloatGrid elevation = MapGenerator.Elevation;
		MapGenFloatGrid fertility = MapGenerator.Fertility;
		MapGenFloatGrid caves = MapGenerator.Caves;
		TerrainGrid terrainGrid = map.terrainGrid;
		foreach (IntVec3 allCell in map.AllCells)
		{
			Building edifice = allCell.GetEdifice(map);
			TerrainDef terrainDef = null;
			terrainDef = (((edifice == null || edifice.def.Fillage != FillCategory.Full) && !(caves[allCell] > 0f)) ? TerrainFrom(allCell, map, elevation[allCell], fertility[allCell], riverMaker, preferSolid: false) : TerrainFrom(allCell, map, elevation[allCell], fertility[allCell], riverMaker, preferSolid: true));
			if (terrainDef.IsRiver && edifice != null)
			{
				list.Add(edifice.Position);
				edifice.Destroy();
			}
			terrainGrid.SetTerrain(allCell, terrainDef);
		}
		riverMaker?.ValidatePassage(map);
		RemoveIslands(map);
		RoofCollapseCellsFinder.RemoveBulkCollapsingRoofs(list, map);
		BeachMaker.Cleanup();
		foreach (TerrainPatchMaker terrainPatchMaker in map.Biome.terrainPatchMakers)
		{
			terrainPatchMaker.Cleanup();
		}
	}

	private TerrainDef TerrainFrom(IntVec3 c, Map map, float elevation, float fertility, RiverMaker river, bool preferSolid)
	{
		TerrainDef terrainDef = null;
		if (river != null)
		{
			terrainDef = river.TerrainAt(c, recordForValidation: true);
		}
		if (terrainDef == null && preferSolid)
		{
			return GenStep_RocksFromGrid.RockDefAt(c).building.naturalTerrain;
		}
		TerrainDef terrainDef2 = BeachMaker.BeachTerrainAt(c, map.Biome);
		if (terrainDef2 == TerrainDefOf.WaterOceanDeep)
		{
			return terrainDef2;
		}
		if (terrainDef != null && terrainDef.IsRiver)
		{
			return terrainDef;
		}
		if (terrainDef2 != null)
		{
			return terrainDef2;
		}
		if (terrainDef != null)
		{
			return terrainDef;
		}
		for (int i = 0; i < map.Biome.terrainPatchMakers.Count; i++)
		{
			terrainDef2 = map.Biome.terrainPatchMakers[i].TerrainAt(c, map, fertility);
			if (terrainDef2 != null)
			{
				return terrainDef2;
			}
		}
		if (elevation > 0.55f && elevation < 0.61f)
		{
			return TerrainDefOf.Gravel;
		}
		if (elevation >= 0.61f)
		{
			return GenStep_RocksFromGrid.RockDefAt(c).building.naturalTerrain;
		}
		terrainDef2 = TerrainThreshold.TerrainAtValue(map.Biome.terrainsByFertility, fertility);
		if (terrainDef2 != null)
		{
			return terrainDef2;
		}
		if (!debug_WarnedMissingTerrain)
		{
			Log.Error("No terrain found in biome " + map.Biome.defName + " for elevation=" + elevation + ", fertility=" + fertility);
			debug_WarnedMissingTerrain = true;
		}
		return TerrainDefOf.Sand;
	}

	private void RemoveIslands(Map map)
	{
		CellRect mapRect = CellRect.WholeMap(map);
		int num = 0;
		tmpVisited.Clear();
		foreach (IntVec3 allCell in map.AllCells)
		{
			if (tmpVisited.Contains(allCell) || Impassable(allCell))
			{
				continue;
			}
			int area = 0;
			bool touchesMapEdge = false;
			map.floodFiller.FloodFill(allCell, (IntVec3 x) => !Impassable(x), delegate(IntVec3 x)
			{
				tmpVisited.Add(x);
				area++;
				if (mapRect.IsOnEdge(x))
				{
					touchesMapEdge = true;
				}
			});
			if (touchesMapEdge)
			{
				num = Mathf.Max(num, area);
			}
		}
		if (num < 30)
		{
			return;
		}
		tmpVisited.Clear();
		foreach (IntVec3 allCell2 in map.AllCells)
		{
			if (tmpVisited.Contains(allCell2) || Impassable(allCell2))
			{
				continue;
			}
			tmpIsland.Clear();
			TerrainDef adjacentImpassableTerrain = null;
			bool touchesMapEdge2 = false;
			map.floodFiller.FloodFill(allCell2, delegate(IntVec3 x)
			{
				if (Impassable(x))
				{
					adjacentImpassableTerrain = x.GetTerrain(map);
					return false;
				}
				return true;
			}, delegate(IntVec3 x)
			{
				tmpVisited.Add(x);
				tmpIsland.Add(x);
				if (mapRect.IsOnEdge(x))
				{
					touchesMapEdge2 = true;
				}
			});
			if ((tmpIsland.Count <= num / 20 || (!touchesMapEdge2 && tmpIsland.Count < num / 2)) && adjacentImpassableTerrain != null)
			{
				for (int num2 = 0; num2 < tmpIsland.Count; num2++)
				{
					map.terrainGrid.SetTerrain(tmpIsland[num2], adjacentImpassableTerrain);
				}
			}
		}
		bool Impassable(IntVec3 x)
		{
			return x.GetTerrain(map).passability == Traversability.Impassable;
		}
	}

	private RiverMaker GenerateRiver(Map map)
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		List<Tile.RiverLink> rivers = map.TileInfo.Rivers;
		if (rivers == null || rivers.Count == 0)
		{
			return null;
		}
		float angle = Find.WorldGrid.GetHeadingFromTo(map.Tile, rivers.OrderBy((Tile.RiverLink rl) => -rl.river.degradeThreshold).First().neighbor);
		Rot4 rot = Find.World.CoastDirectionAt(map.Tile);
		if (rot != Rot4.Invalid)
		{
			angle = rot.AsAngle + (float)Rand.RangeInclusive(-30, 30);
		}
		RiverMaker riverMaker = new RiverMaker(new Vector3(Rand.Range(0.3f, 0.7f) * (float)map.Size.x, 0f, Rand.Range(0.3f, 0.7f) * (float)map.Size.z), angle, rivers.OrderBy((Tile.RiverLink rl) => -rl.river.degradeThreshold).FirstOrDefault().river);
		GenerateRiverLookupTexture(map, riverMaker);
		return riverMaker;
	}

	private void UpdateRiverAnchorEntry(Dictionary<int, GRLT_Entry> entries, IntVec3 center, int entryId, float zValue)
	{
		float num = zValue - (float)entryId;
		if (!(num > 2f) && (!entries.ContainsKey(entryId) || entries[entryId].bestDistance > num))
		{
			entries[entryId] = new GRLT_Entry
			{
				bestDistance = num,
				bestNode = center
			};
		}
	}

	private void GenerateRiverLookupTexture(Map map, RiverMaker riverMaker)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0866: Unknown result type (might be due to invalid IL or missing references)
		//IL_086b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_050f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_0537: Unknown result type (might be due to invalid IL or missing references)
		//IL_0554: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0578: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0583: Unknown result type (might be due to invalid IL or missing references)
		//IL_0585: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0590: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_062f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.CeilToInt(DefDatabase<RiverDef>.AllDefs.Select((RiverDef rd) => rd.widthOnMap / 2f + 8f).Max());
		int num2 = Mathf.Max(4, num) * 2;
		Dictionary<int, GRLT_Entry> dictionary = new Dictionary<int, GRLT_Entry>();
		Dictionary<int, GRLT_Entry> dictionary2 = new Dictionary<int, GRLT_Entry>();
		Dictionary<int, GRLT_Entry> dictionary3 = new Dictionary<int, GRLT_Entry>();
		for (int num3 = -num2; num3 < map.Size.z + num2; num3++)
		{
			for (int num4 = -num2; num4 < map.Size.x + num2; num4++)
			{
				IntVec3 intVec = new IntVec3(num4, 0, num3);
				Vector3 val = riverMaker.WaterCoordinateAt(intVec);
				int entryId = Mathf.FloorToInt(val.z / 4f);
				UpdateRiverAnchorEntry(dictionary, intVec, entryId, (val.z + Mathf.Abs(val.x)) / 4f);
				UpdateRiverAnchorEntry(dictionary2, intVec, entryId, (val.z + Mathf.Abs(val.x - (float)num)) / 4f);
				UpdateRiverAnchorEntry(dictionary3, intVec, entryId, (val.z + Mathf.Abs(val.x + (float)num)) / 4f);
			}
		}
		int num5 = Mathf.Max(new int[3]
		{
			dictionary.Keys.Min(),
			dictionary2.Keys.Min(),
			dictionary3.Keys.Min()
		});
		int num6 = Mathf.Min(new int[3]
		{
			dictionary.Keys.Max(),
			dictionary2.Keys.Max(),
			dictionary3.Keys.Max()
		});
		for (int num7 = num5; num7 < num6; num7++)
		{
			WaterInfo waterInfo = map.waterInfo;
			if (dictionary2.ContainsKey(num7) && dictionary2.ContainsKey(num7 + 1))
			{
				waterInfo.riverDebugData.Add(dictionary2[num7].bestNode.ToVector3Shifted());
				waterInfo.riverDebugData.Add(dictionary2[num7 + 1].bestNode.ToVector3Shifted());
			}
			if (dictionary.ContainsKey(num7) && dictionary.ContainsKey(num7 + 1))
			{
				waterInfo.riverDebugData.Add(dictionary[num7].bestNode.ToVector3Shifted());
				waterInfo.riverDebugData.Add(dictionary[num7 + 1].bestNode.ToVector3Shifted());
			}
			if (dictionary3.ContainsKey(num7) && dictionary3.ContainsKey(num7 + 1))
			{
				waterInfo.riverDebugData.Add(dictionary3[num7].bestNode.ToVector3Shifted());
				waterInfo.riverDebugData.Add(dictionary3[num7 + 1].bestNode.ToVector3Shifted());
			}
			if (dictionary2.ContainsKey(num7) && dictionary.ContainsKey(num7))
			{
				waterInfo.riverDebugData.Add(dictionary2[num7].bestNode.ToVector3Shifted());
				waterInfo.riverDebugData.Add(dictionary[num7].bestNode.ToVector3Shifted());
			}
			if (dictionary.ContainsKey(num7) && dictionary3.ContainsKey(num7))
			{
				waterInfo.riverDebugData.Add(dictionary[num7].bestNode.ToVector3Shifted());
				waterInfo.riverDebugData.Add(dictionary3[num7].bestNode.ToVector3Shifted());
			}
		}
		CellRect cellRect = new CellRect(-2, -2, map.Size.x + 4, map.Size.z + 4);
		float[] array = new float[cellRect.Area * 2];
		int num8 = 0;
		for (int num9 = cellRect.minZ; num9 <= cellRect.maxZ; num9++)
		{
			for (int num10 = cellRect.minX; num10 <= cellRect.maxX; num10++)
			{
				IntVec3 intVec2 = new IntVec3(num10, 0, num9);
				bool flag = true;
				for (int num11 = 0; num11 < GenAdj.AdjacentCellsAndInside.Length; num11++)
				{
					if (riverMaker.TerrainAt(intVec2 + GenAdj.AdjacentCellsAndInside[num11]) != null)
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					Vector2 p = intVec2.ToIntVec2.ToVector2();
					int num12 = int.MinValue;
					Vector2 zero = Vector2.zero;
					for (int num13 = num5; num13 < num6; num13++)
					{
						if (dictionary2.ContainsKey(num13) && dictionary2.ContainsKey(num13 + 1) && dictionary.ContainsKey(num13) && dictionary.ContainsKey(num13 + 1) && dictionary3.ContainsKey(num13) && dictionary3.ContainsKey(num13 + 1))
						{
							Vector2 p2 = dictionary2[num13].bestNode.ToIntVec2.ToVector2();
							Vector2 p3 = dictionary2[num13 + 1].bestNode.ToIntVec2.ToVector2();
							Vector2 p4 = dictionary[num13].bestNode.ToIntVec2.ToVector2();
							Vector2 p5 = dictionary[num13 + 1].bestNode.ToIntVec2.ToVector2();
							Vector2 p6 = dictionary3[num13].bestNode.ToIntVec2.ToVector2();
							Vector2 p7 = dictionary3[num13 + 1].bestNode.ToIntVec2.ToVector2();
							Vector2 val2 = GenGeo.InverseQuadBilinear(p, p4, p2, p5, p3);
							if (val2.x >= -0.0001f && val2.x <= 1.0001f && val2.y >= -0.0001f && val2.y <= 1.0001f)
							{
								((Vector2)(ref zero))._002Ector((0f - val2.x) * (float)num, (val2.y + (float)num13) * 4f);
								num12 = num13;
								break;
							}
							Vector2 val3 = GenGeo.InverseQuadBilinear(p, p4, p6, p5, p7);
							if (val3.x >= -0.0001f && val3.x <= 1.0001f && val3.y >= -0.0001f && val3.y <= 1.0001f)
							{
								((Vector2)(ref zero))._002Ector(val3.x * (float)num, (val3.y + (float)num13) * 4f);
								num12 = num13;
								break;
							}
						}
					}
					if (num12 == int.MinValue)
					{
						Log.ErrorOnce("Failed to find all necessary river flow data", 5273133);
					}
					array[num8] = zero.x;
					array[num8 + 1] = zero.y;
				}
				num8 += 2;
			}
		}
		float[] array2 = new float[cellRect.Area * 2];
		float[] array3 = new float[9] { 0.123317f, 0.123317f, 0.123317f, 0.123317f, 0.077847f, 0.077847f, 0.077847f, 0.077847f, 0.195346f };
		int num14 = 0;
		for (int num15 = cellRect.minZ; num15 <= cellRect.maxZ; num15++)
		{
			for (int num16 = cellRect.minX; num16 <= cellRect.maxX; num16++)
			{
				IntVec3 intVec3 = new IntVec3(num16, 0, num15);
				float num17 = 0f;
				float num18 = 0f;
				float num19 = 0f;
				for (int num20 = 0; num20 < GenAdj.AdjacentCellsAndInside.Length; num20++)
				{
					IntVec3 c = intVec3 + GenAdj.AdjacentCellsAndInside[num20];
					if (cellRect.Contains(c))
					{
						int num21 = num14 + (GenAdj.AdjacentCellsAndInside[num20].x + GenAdj.AdjacentCellsAndInside[num20].z * cellRect.Width) * 2;
						if (array[num21] != 0f || array[num21 + 1] != 0f)
						{
							num17 += array[num21] * array3[num20];
							num18 += array[num21 + 1] * array3[num20];
							num19 += array3[num20];
						}
					}
				}
				if (num19 > 0f)
				{
					array2[num14] = num17 / num19;
					array2[num14 + 1] = num18 / num19;
				}
				num14 += 2;
			}
		}
		array = array2;
		for (int num22 = 0; num22 < array.Length; num22 += 2)
		{
			if (array[num22] != 0f || array[num22 + 1] != 0f)
			{
				Vector2 val4 = Rand.InsideUnitCircle * 0.4f;
				array[num22] += val4.x;
				array[num22 + 1] += val4.y;
			}
		}
		byte[] array4 = new byte[array.Length * 4];
		Buffer.BlockCopy(array, 0, array4, 0, array.Length * 4);
		map.waterInfo.riverOffsetMap = array4;
		map.waterInfo.GenerateRiverFlowMap();
	}
}
