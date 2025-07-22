using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public static class GenWorldUI
{
	private static List<Caravan> clickedCaravans = new List<Caravan>();

	private static List<WorldObject> clickedDynamicallyDrawnObjects = new List<WorldObject>();

	public static float CaravanDirectClickRadius => 0.35f * Find.WorldGrid.averageTileSize;

	private static float CaravanWideClickRadius => 0.75f * Find.WorldGrid.averageTileSize;

	private static float DynamicallyDrawnObjectDirectClickRadius => 0.35f * Find.WorldGrid.averageTileSize;

	public static List<WorldObject> WorldObjectsUnderMouse(Vector2 mousePos)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		List<WorldObject> list = new List<WorldObject>();
		ExpandableWorldObjectsUtility.GetExpandedWorldObjectUnderMouse(mousePos, list);
		float caravanDirectClickRadius = CaravanDirectClickRadius;
		clickedCaravans.Clear();
		List<Caravan> caravans = Find.WorldObjects.Caravans;
		for (int i = 0; i < caravans.Count; i++)
		{
			Caravan caravan = caravans[i];
			if (caravan.DistanceToMouse(mousePos) < caravanDirectClickRadius)
			{
				clickedCaravans.Add(caravan);
			}
		}
		clickedCaravans.SortBy((Caravan x) => x.DistanceToMouse(mousePos));
		for (int num = 0; num < clickedCaravans.Count; num++)
		{
			if (!list.Contains(clickedCaravans[num]))
			{
				list.Add(clickedCaravans[num]);
			}
		}
		float dynamicallyDrawnObjectDirectClickRadius = DynamicallyDrawnObjectDirectClickRadius;
		clickedDynamicallyDrawnObjects.Clear();
		List<WorldObject> allWorldObjects = Find.WorldObjects.AllWorldObjects;
		for (int num2 = 0; num2 < allWorldObjects.Count; num2++)
		{
			WorldObject worldObject = allWorldObjects[num2];
			if (worldObject.def.useDynamicDrawer && worldObject.DistanceToMouse(mousePos) < dynamicallyDrawnObjectDirectClickRadius)
			{
				clickedDynamicallyDrawnObjects.Add(worldObject);
			}
		}
		clickedDynamicallyDrawnObjects.SortBy((WorldObject x) => x.DistanceToMouse(mousePos));
		for (int num3 = 0; num3 < clickedDynamicallyDrawnObjects.Count; num3++)
		{
			if (!list.Contains(clickedDynamicallyDrawnObjects[num3]))
			{
				list.Add(clickedDynamicallyDrawnObjects[num3]);
			}
		}
		int num4 = GenWorld.TileAt(mousePos);
		List<WorldObject> allWorldObjects2 = Find.WorldObjects.AllWorldObjects;
		for (int num5 = 0; num5 < allWorldObjects2.Count; num5++)
		{
			if (allWorldObjects2[num5].Tile == num4 && !list.Contains(allWorldObjects2[num5]))
			{
				list.Add(allWorldObjects2[num5]);
			}
		}
		float caravanWideClickRadius = CaravanWideClickRadius;
		clickedCaravans.Clear();
		List<Caravan> caravans2 = Find.WorldObjects.Caravans;
		for (int num6 = 0; num6 < caravans2.Count; num6++)
		{
			Caravan caravan2 = caravans2[num6];
			if (caravan2.DistanceToMouse(mousePos) < caravanWideClickRadius)
			{
				clickedCaravans.Add(caravan2);
			}
		}
		clickedCaravans.SortBy((Caravan x) => x.DistanceToMouse(mousePos));
		for (int num7 = 0; num7 < clickedCaravans.Count; num7++)
		{
			if (!list.Contains(clickedCaravans[num7]))
			{
				list.Add(clickedCaravans[num7]);
			}
		}
		clickedCaravans.Clear();
		return list;
	}

	public static Vector2 WorldToUIPosition(Vector3 worldLoc)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Find.WorldCamera.WorldToScreenPoint(worldLoc) / Prefs.UIScale;
		return new Vector2(val.x, (float)UI.screenHeight - val.y);
	}

	public static float CurUITileSize()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localPosition = ((Component)Find.WorldCamera).transform.localPosition;
		Quaternion rotation = ((Component)Find.WorldCamera).transform.rotation;
		((Component)Find.WorldCamera).transform.localPosition = new Vector3(0f, 0f, ((Vector3)(ref localPosition)).magnitude);
		((Component)Find.WorldCamera).transform.rotation = Quaternion.identity;
		float x = (WorldToUIPosition(new Vector3((0f - Find.WorldGrid.averageTileSize) / 2f, 0f, 100f)) - WorldToUIPosition(new Vector3(Find.WorldGrid.averageTileSize / 2f, 0f, 100f))).x;
		((Component)Find.WorldCamera).transform.localPosition = localPosition;
		((Component)Find.WorldCamera).transform.rotation = rotation;
		return x;
	}
}
