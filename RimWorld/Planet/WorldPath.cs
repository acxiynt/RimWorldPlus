using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public class WorldPath : IDisposable
{
	private List<int> nodes = new List<int>(128);

	private float totalCostInt;

	private int curNodeIndex;

	public bool inUse;

	public bool Found => totalCostInt >= 0f;

	public float TotalCost => totalCostInt;

	public int NodesLeftCount => curNodeIndex + 1;

	public List<int> NodesReversed => nodes;

	public int FirstNode => nodes[nodes.Count - 1];

	public int LastNode => nodes[0];

	public static WorldPath NotFound => WorldPathPool.NotFoundPath;

	public void AddNodeAtStart(int tile)
	{
		nodes.Add(tile);
	}

	public void SetupFound(float totalCost)
	{
		if (this == NotFound)
		{
			Log.Warning("Calling SetupFound with totalCost=" + totalCost + " on WorldPath.NotFound");
			return;
		}
		totalCostInt = totalCost;
		curNodeIndex = nodes.Count - 1;
	}

	public void Dispose()
	{
		ReleaseToPool();
	}

	public void ReleaseToPool()
	{
		if (this != NotFound)
		{
			totalCostInt = 0f;
			nodes.Clear();
			inUse = false;
		}
	}

	public static WorldPath NewNotFound()
	{
		return new WorldPath
		{
			totalCostInt = -1f
		};
	}

	public int ConsumeNextNode()
	{
		int result = Peek(1);
		curNodeIndex--;
		return result;
	}

	public int Peek(int nodesAhead)
	{
		return nodes[curNodeIndex - nodesAhead];
	}

	public override string ToString()
	{
		if (!Found)
		{
			return "WorldPath(not found)";
		}
		if (!inUse)
		{
			return "WorldPath(not in use)";
		}
		return "WorldPath(nodeCount= " + nodes.Count + ((nodes.Count > 0) ? (" first=" + FirstNode + " last=" + LastNode) : "") + " cost=" + totalCostInt + " )";
	}

	public void DrawPath(Caravan pathingCaravan)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		if (!Found || NodesLeftCount <= 0)
		{
			return;
		}
		WorldGrid worldGrid = Find.WorldGrid;
		float num = 0.05f;
		for (int i = 0; i < NodesLeftCount - 1; i++)
		{
			Vector3 val = worldGrid.GetTileCenter(Peek(i));
			Vector3 val2 = worldGrid.GetTileCenter(Peek(i + 1));
			val += ((Vector3)(ref val)).normalized * num;
			val2 += ((Vector3)(ref val2)).normalized * num;
			GenDraw.DrawWorldLineBetween(val, val2);
		}
		if (pathingCaravan != null)
		{
			Vector3 val3 = pathingCaravan.DrawPos;
			Vector3 val4 = worldGrid.GetTileCenter(Peek(0));
			val3 += ((Vector3)(ref val3)).normalized * num;
			val4 += ((Vector3)(ref val4)).normalized * num;
			Vector3 val5 = val3 - val4;
			if (((Vector3)(ref val5)).sqrMagnitude > 0.005f)
			{
				GenDraw.DrawWorldLineBetween(val3, val4);
			}
		}
	}
}
