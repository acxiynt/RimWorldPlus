using System;
using System.Collections.Generic;
using RimWorld;
using Unity.Collections;

namespace Verse;

public static class FloodFillerFog
{
	private static bool testMode = false;

	private static List<IntVec3> cellsToUnfog = new List<IntVec3>(1024);

	private const int MaxNumTestUnfog = 500;

	public static FloodUnfogResult FloodUnfog(IntVec3 root, Map map)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		cellsToUnfog.Clear();
		FloodUnfogResult result = default(FloodUnfogResult);
		NativeArray<bool> fogGridDirect = map.fogGrid.FogGrid_Unsafe;
		FogGrid fogGrid = map.fogGrid;
		List<IntVec3> newlyUnfoggedCells = new List<IntVec3>();
		int numUnfogged = 0;
		bool expanding = false;
		CellRect viewRect = CellRect.ViewRect(map);
		result.allOnScreen = true;
		Predicate<IntVec3> predicate = delegate(IntVec3 c)
		{
			if (!fogGridDirect[map.cellIndices.CellToIndex(c)])
			{
				return false;
			}
			Thing edifice = c.GetEdifice(map);
			if (edifice != null && edifice.def.MakeFog)
			{
				return false;
			}
			return (!testMode || expanding || numUnfogged <= 500) ? true : false;
		};
		Action<IntVec3> processor = delegate(IntVec3 c)
		{
			fogGrid.Unfog(c);
			newlyUnfoggedCells.Add(c);
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (thingList[i] is Pawn pawn)
				{
					pawn.mindState.Active = true;
					if (pawn.def.race.IsMechanoid)
					{
						result.mechanoidFound = true;
					}
				}
				if (ModsConfig.AnomalyActive && AnomalyUtility.ShouldNotifyCodex(thingList[i], EntityDiscoveryType.Unfog, out var entries))
				{
					Find.EntityCodex.SetDiscovered(entries, thingList[i].def, thingList[i]);
				}
				else
				{
					Find.HiddenItemsManager.SetDiscovered(thingList[i].def);
				}
				CompLetterOnRevealed compLetterOnRevealed = thingList[i].TryGetComp<CompLetterOnRevealed>();
				if (compLetterOnRevealed != null)
				{
					Find.LetterStack.ReceiveLetter(compLetterOnRevealed.Props.label, compLetterOnRevealed.Props.text, compLetterOnRevealed.Props.letterDef, thingList[i]);
				}
			}
			if (!viewRect.Contains(c))
			{
				result.allOnScreen = false;
			}
			result.cellsUnfogged++;
			if (testMode)
			{
				numUnfogged++;
				map.debugDrawer.FlashCell(c, (float)numUnfogged / 200f, numUnfogged.ToStringCached());
			}
		};
		map.floodFiller.FloodFill(root, predicate, processor);
		expanding = true;
		for (int num = 0; num < newlyUnfoggedCells.Count; num++)
		{
			IntVec3 intVec = newlyUnfoggedCells[num];
			for (int num2 = 0; num2 < 8; num2++)
			{
				IntVec3 intVec2 = intVec + GenAdj.AdjacentCells[num2];
				if (intVec2.InBounds(map) && fogGrid.IsFogged(intVec2) && !predicate(intVec2))
				{
					cellsToUnfog.Add(intVec2);
				}
			}
		}
		for (int num3 = 0; num3 < cellsToUnfog.Count; num3++)
		{
			fogGrid.Unfog(cellsToUnfog[num3]);
			if (testMode)
			{
				map.debugDrawer.FlashCell(cellsToUnfog[num3], 0.3f, "x");
			}
		}
		cellsToUnfog.Clear();
		return result;
	}

	public static void DebugFloodUnfog(IntVec3 root, Map map)
	{
		map.fogGrid.SetAllFogged();
		foreach (IntVec3 allCell in map.AllCells)
		{
			map.mapDrawer.MapMeshDirty(allCell, MapMeshFlagDefOf.FogOfWar);
		}
		testMode = true;
		FloodUnfog(root, map);
		testMode = false;
	}

	public static void DebugRefogMap(Map map)
	{
		map.fogGrid.SetAllFogged();
		foreach (IntVec3 allCell in map.AllCells)
		{
			map.mapDrawer.MapMeshDirty(allCell, MapMeshFlagDefOf.FogOfWar);
		}
		FloodUnfog(map.mapPawns.FreeColonistsSpawned.RandomElement().Position, map);
	}
}
