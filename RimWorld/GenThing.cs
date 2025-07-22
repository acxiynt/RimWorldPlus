using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class GenThing
{
	private static List<Thing> tmpThings = new List<Thing>();

	private static List<string> tmpThingLabels = new List<string>();

	private static List<Pair<string, int>> tmpThingCounts = new List<Pair<string, int>>();

	public static Vector3 TrueCenter(this Thing t)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (t is Pawn pawn)
		{
			return pawn.Drawer.DrawPos;
		}
		if (t.def.category == ThingCategory.Item && t.Spawned)
		{
			return ItemCenterAt(t.Position, t.Map, t.def.Altitude, t.thingIDNumber);
		}
		return TrueCenter(t.Position, t.Rotation, t.def.size, t.def.Altitude);
	}

	public static Vector3 TrueCenter(IntVec3 loc, Rot4 rotation, IntVec2 thingSize, float altitude)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = loc.ToVector3ShiftedWithAltitude(altitude);
		if (thingSize.x != 1 || thingSize.z != 1)
		{
			if (rotation.IsHorizontal)
			{
				int x = thingSize.x;
				thingSize.x = thingSize.z;
				thingSize.z = x;
			}
			switch (rotation.AsInt)
			{
			case 0:
				if (thingSize.x % 2 == 0)
				{
					result.x += 0.5f;
				}
				if (thingSize.z % 2 == 0)
				{
					result.z += 0.5f;
				}
				break;
			case 1:
				if (thingSize.x % 2 == 0)
				{
					result.x += 0.5f;
				}
				if (thingSize.z % 2 == 0)
				{
					result.z -= 0.5f;
				}
				break;
			case 2:
				if (thingSize.x % 2 == 0)
				{
					result.x -= 0.5f;
				}
				if (thingSize.z % 2 == 0)
				{
					result.z -= 0.5f;
				}
				break;
			case 3:
				if (thingSize.x % 2 == 0)
				{
					result.x -= 0.5f;
				}
				if (thingSize.z % 2 == 0)
				{
					result.z += 0.5f;
				}
				break;
			}
		}
		return result;
	}

	private static Vector3 ItemCenterAt(IntVec3 c, Map map, float altitude, int thingID)
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int num2 = 0;
		bool flag = false;
		bool flag2 = true;
		ThingDef thingDef = null;
		List<Thing> thingList = c.GetThingList(map);
		for (int i = 0; i < thingList.Count; i++)
		{
			Thing thing = thingList[i];
			if (thing.def.category == ThingCategory.Item)
			{
				if (thingDef == null)
				{
					thingDef = thing.def;
				}
				num++;
				if (thing.def.IsWeapon && thing.def != ThingDefOf.WoodLog)
				{
					flag = true;
				}
				if (thing.thingIDNumber < thingID)
				{
					num2++;
				}
				if (thing.def != thingDef)
				{
					flag2 = false;
				}
			}
		}
		float num3 = (float)num2 * (1f / 26f) / 10f;
		if (num <= 1)
		{
			Vector3 val = c.ToVector3Shifted();
			return new Vector3(val.x, altitude, val.z);
		}
		if (flag)
		{
			Vector3 val2 = c.ToVector3Shifted();
			float num4 = 1f / (float)num;
			int num5 = GetRowItemCount(new IntVec3(c.x - 1, c.y, c.z)) + num2;
			return new Vector3(val2.x - 0.5f + num4 * ((float)num2 + 0.5f), altitude + num3, val2.z + ((num5 % 2 == 0) ? (-0.02f) : 0.2f));
		}
		if (flag2)
		{
			Vector3 val3 = c.ToVector3Shifted();
			return new Vector3(val3.x + (float)num2 * 0.11f - 0.08f, altitude + num3, val3.z + (float)num2 * 0.24f - 0.05f);
		}
		Vector3 val4 = c.ToVector3Shifted();
		Vector2 val5 = GenGeo.RegularPolygonVertexPosition(num, num2, ((c.x + c.z) % 2 == 0) ? 0f : 60f) * 0.3f;
		return new Vector3(val5.x + val4.x, altitude + num3, val5.y + val4.z);
		int GetRowItemCount(IntVec3 x)
		{
			if (!x.InBounds(map))
			{
				return 0;
			}
			int itemCount = x.GetItemCount(map);
			if (itemCount <= 1)
			{
				return 0;
			}
			x.x--;
			return itemCount + GetRowItemCount(x);
		}
	}

	public static bool TryDropAndSetForbidden(Thing th, IntVec3 pos, Map map, ThingPlaceMode mode, out Thing resultingThing, bool forbidden)
	{
		if (GenDrop.TryDropSpawn(th, pos, map, ThingPlaceMode.Near, out resultingThing))
		{
			if (resultingThing != null)
			{
				resultingThing.SetForbidden(forbidden, warnOnFail: false);
			}
			return true;
		}
		resultingThing = null;
		return false;
	}

	public static string ThingsToCommaList(IList<Thing> things, bool useAnd = false, bool aggregate = true, int maxCount = -1)
	{
		tmpThings.Clear();
		tmpThingLabels.Clear();
		tmpThingCounts.Clear();
		tmpThings.AddRange(things);
		if (tmpThings.Count >= 2)
		{
			tmpThings.SortByDescending((Thing x) => x is Pawn, (Thing x) => x.def.BaseMarketValue * (float)x.stackCount);
		}
		for (int num = 0; num < tmpThings.Count; num++)
		{
			string text = ((tmpThings[num] is Pawn) ? tmpThings[num].LabelShort : tmpThings[num].LabelNoCount);
			bool flag = false;
			if (aggregate)
			{
				for (int num2 = 0; num2 < tmpThingCounts.Count; num2++)
				{
					if (tmpThingCounts[num2].First == text)
					{
						tmpThingCounts[num2] = new Pair<string, int>(tmpThingCounts[num2].First, tmpThingCounts[num2].Second + tmpThings[num].stackCount);
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				tmpThingCounts.Add(new Pair<string, int>(text, tmpThings[num].stackCount));
			}
		}
		tmpThings.Clear();
		bool flag2 = false;
		int num3 = tmpThingCounts.Count;
		if (maxCount >= 0 && num3 > maxCount)
		{
			num3 = maxCount;
			flag2 = true;
		}
		for (int num4 = 0; num4 < num3; num4++)
		{
			string text2 = tmpThingCounts[num4].First;
			if (tmpThingCounts[num4].Second != 1)
			{
				text2 = text2 + " x" + tmpThingCounts[num4].Second;
			}
			tmpThingLabels.Add(text2);
		}
		string text3 = tmpThingLabels.ToCommaList(useAnd && !flag2);
		if (flag2)
		{
			text3 += "...";
		}
		return text3;
	}

	public static float GetMarketValue(IList<Thing> things)
	{
		float num = 0f;
		for (int i = 0; i < things.Count; i++)
		{
			num += things[i].MarketValue * (float)things[i].stackCount;
		}
		return num;
	}

	public static bool CloserThingBetween(ThingDef thingDef, IntVec3 a, IntVec3 b, Map map, Thing thingToIgnore = null)
	{
		foreach (IntVec3 item in CellRect.FromLimits(a, b))
		{
			if (item == a || item == b || !item.InBounds(map))
			{
				continue;
			}
			foreach (Thing thing in item.GetThingList(map))
			{
				if ((thingToIgnore == null || thingToIgnore != thing) && (thing.def == thingDef || thing.def.entityDefToBuild == thingDef))
				{
					return true;
				}
			}
		}
		return false;
	}
}
