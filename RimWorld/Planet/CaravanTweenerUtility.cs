using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

public static class CaravanTweenerUtility
{
	private const float BaseRadius = 0.15f;

	private const float BaseDistToCollide = 0.2f;

	public static Vector3 PatherTweenedPosRoot(Caravan caravan)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		WorldGrid worldGrid = Find.WorldGrid;
		if (!caravan.Spawned)
		{
			return worldGrid.GetTileCenter(caravan.Tile);
		}
		if (caravan.pather.Moving)
		{
			float num = (caravan.pather.IsNextTilePassable() ? (1f - caravan.pather.nextTileCostLeft / caravan.pather.nextTileCostTotal) : 0f);
			int tileID = ((caravan.pather.nextTile != caravan.Tile || caravan.pather.previousTileForDrawingIfInDoubt == -1) ? caravan.Tile : caravan.pather.previousTileForDrawingIfInDoubt);
			return worldGrid.GetTileCenter(caravan.pather.nextTile) * num + worldGrid.GetTileCenter(tileID) * (1f - num);
		}
		return worldGrid.GetTileCenter(caravan.Tile);
	}

	public static Vector3 CaravanCollisionPosOffsetFor(Caravan caravan)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		if (!caravan.Spawned)
		{
			return Vector3.zero;
		}
		bool flag = caravan.Spawned && caravan.pather.Moving;
		float num = 0.15f * Find.WorldGrid.averageTileSize;
		if (!flag || caravan.pather.nextTile == caravan.pather.Destination)
		{
			int num2 = ((!flag) ? caravan.Tile : caravan.pather.nextTile);
			int caravansCount = 0;
			int caravansWithLowerIdCount = 0;
			GetCaravansStandingAtOrAboutToStandAt(num2, out caravansCount, out caravansWithLowerIdCount, caravan);
			if (caravansCount == 0)
			{
				return Vector3.zero;
			}
			return WorldRendererUtility.ProjectOnQuadTangentialToPlanet(Find.WorldGrid.GetTileCenter(num2), GenGeo.RegularPolygonVertexPosition(caravansCount, caravansWithLowerIdCount) * num);
		}
		if (DrawPosCollides(caravan))
		{
			Rand.PushState();
			Rand.Seed = caravan.ID;
			float num3 = Rand.Range(0f, 360f);
			Rand.PopState();
			Vector2 point = new Vector2(Mathf.Cos(num3), Mathf.Sin(num3)) * num;
			return WorldRendererUtility.ProjectOnQuadTangentialToPlanet(PatherTweenedPosRoot(caravan), point);
		}
		return Vector3.zero;
	}

	private static void GetCaravansStandingAtOrAboutToStandAt(int tile, out int caravansCount, out int caravansWithLowerIdCount, Caravan forCaravan)
	{
		caravansCount = 0;
		caravansWithLowerIdCount = 0;
		List<Caravan> caravans = Find.WorldObjects.Caravans;
		for (int i = 0; i < caravans.Count; i++)
		{
			Caravan caravan = caravans[i];
			if (caravan.Tile != tile)
			{
				if (!caravan.pather.Moving || caravan.pather.nextTile != caravan.pather.Destination || caravan.pather.Destination != tile)
				{
					continue;
				}
			}
			else if (caravan.pather.Moving)
			{
				continue;
			}
			caravansCount++;
			if (caravan.ID < forCaravan.ID)
			{
				caravansWithLowerIdCount++;
			}
		}
	}

	private static bool DrawPosCollides(Caravan caravan)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = PatherTweenedPosRoot(caravan);
		float num = Find.WorldGrid.averageTileSize * 0.2f;
		List<Caravan> caravans = Find.WorldObjects.Caravans;
		for (int i = 0; i < caravans.Count; i++)
		{
			Caravan caravan2 = caravans[i];
			if (caravan2 != caravan && Vector3.Distance(val, PatherTweenedPosRoot(caravan2)) < num)
			{
				return true;
			}
		}
		return false;
	}
}
