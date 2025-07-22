using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public abstract class FeatureWorker_FloodFill : FeatureWorker
{
	private List<int> roots = new List<int>();

	private HashSet<int> rootsSet = new HashSet<int>();

	private List<int> possiblyAllowed = new List<int>();

	private HashSet<int> possiblyAllowedSet = new HashSet<int>();

	private List<int> currentGroup = new List<int>();

	private List<int> currentGroupMembers = new List<int>();

	private static List<int> tmpGroup = new List<int>();

	protected virtual int MinSize => def.minSize;

	protected virtual int MaxSize => def.maxSize;

	protected virtual int MaxPossiblyAllowedSizeToTake => def.maxPossiblyAllowedSizeToTake;

	protected virtual float MaxPossiblyAllowedSizePctOfMeToTake => def.maxPossiblyAllowedSizePctOfMeToTake;

	protected abstract bool IsRoot(int tile);

	protected virtual bool IsPossiblyAllowed(int tile)
	{
		return false;
	}

	protected virtual bool IsMember(int tile)
	{
		return Find.WorldGrid[tile].feature == null;
	}

	public override void GenerateWhereAppropriate()
	{
		CalculateRootsAndPossiblyAllowedTiles();
		CalculateContiguousGroups();
	}

	private void CalculateRootsAndPossiblyAllowedTiles()
	{
		roots.Clear();
		possiblyAllowed.Clear();
		int tilesCount = Find.WorldGrid.TilesCount;
		for (int i = 0; i < tilesCount; i++)
		{
			if (IsRoot(i))
			{
				roots.Add(i);
			}
			if (IsPossiblyAllowed(i))
			{
				possiblyAllowed.Add(i);
			}
		}
		rootsSet.Clear();
		rootsSet.AddRange(roots);
		possiblyAllowedSet.Clear();
		possiblyAllowedSet.AddRange(possiblyAllowed);
	}

	private void CalculateContiguousGroups()
	{
		WorldFloodFiller worldFloodFiller = Find.WorldFloodFiller;
		WorldGrid worldGrid = Find.WorldGrid;
		_ = worldGrid.TilesCount;
		int minSize = MinSize;
		int maxSize = MaxSize;
		int maxPossiblyAllowedSizeToTake = MaxPossiblyAllowedSizeToTake;
		float maxPossiblyAllowedSizePctOfMeToTake = MaxPossiblyAllowedSizePctOfMeToTake;
		FeatureWorker.ClearVisited();
		FeatureWorker.ClearGroupSizes();
		for (int i = 0; i < possiblyAllowed.Count; i++)
		{
			int num = possiblyAllowed[i];
			if (!FeatureWorker.visited[num] && !rootsSet.Contains(num))
			{
				tmpGroup.Clear();
				worldFloodFiller.FloodFill(num, (int x) => possiblyAllowedSet.Contains(x) && !rootsSet.Contains(x), delegate(int x)
				{
					FeatureWorker.visited[x] = true;
					tmpGroup.Add(x);
				});
				for (int num2 = 0; num2 < tmpGroup.Count; num2++)
				{
					FeatureWorker.groupSize[tmpGroup[num2]] = tmpGroup.Count;
				}
			}
		}
		for (int num3 = 0; num3 < roots.Count; num3++)
		{
			int num4 = roots[num3];
			if (FeatureWorker.visited[num4])
			{
				continue;
			}
			int initialMembersCountClamped = 0;
			worldFloodFiller.FloodFill(num4, (int x) => (rootsSet.Contains(x) || possiblyAllowedSet.Contains(x)) && IsMember(x), delegate(int x)
			{
				FeatureWorker.visited[x] = true;
				initialMembersCountClamped++;
				return initialMembersCountClamped >= minSize;
			});
			if (initialMembersCountClamped < minSize)
			{
				continue;
			}
			int initialRootsCount = 0;
			worldFloodFiller.FloodFill(num4, (int x) => rootsSet.Contains(x), delegate(int x)
			{
				FeatureWorker.visited[x] = true;
				initialRootsCount++;
			});
			if (initialRootsCount < minSize || initialRootsCount > maxSize)
			{
				continue;
			}
			int traversedRootsCount = 0;
			currentGroup.Clear();
			worldFloodFiller.FloodFill(num4, (int x) => rootsSet.Contains(x) || (possiblyAllowedSet.Contains(x) && FeatureWorker.groupSize[x] <= maxPossiblyAllowedSizeToTake && (float)FeatureWorker.groupSize[x] <= maxPossiblyAllowedSizePctOfMeToTake * (float)Mathf.Max(traversedRootsCount, initialRootsCount) && FeatureWorker.groupSize[x] < maxSize), delegate(int x)
			{
				FeatureWorker.visited[x] = true;
				if (rootsSet.Contains(x))
				{
					traversedRootsCount++;
				}
				currentGroup.Add(x);
			});
			if (currentGroup.Count < minSize || currentGroup.Count > maxSize || (!def.canTouchWorldEdge && currentGroup.Any((int x) => worldGrid.IsOnEdge(x))))
			{
				continue;
			}
			currentGroupMembers.Clear();
			for (int num5 = 0; num5 < currentGroup.Count; num5++)
			{
				if (IsMember(currentGroup[num5]))
				{
					currentGroupMembers.Add(currentGroup[num5]);
				}
			}
			if (currentGroupMembers.Count < minSize)
			{
				continue;
			}
			if (currentGroup.Any((int x) => worldGrid[x].feature == null))
			{
				currentGroup.RemoveAll((int x) => worldGrid[x].feature != null);
			}
			AddFeature(currentGroupMembers, currentGroup);
		}
	}
}
