using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class CompAbilityEffect_SprayLiquid : CompAbilityEffect
{
	private List<Pair<IntVec3, float>> tmpCellDots = new List<Pair<IntVec3, float>>();

	private List<IntVec3> tmpCells = new List<IntVec3>();

	private new CompProperties_AbilitySprayLiquid Props => (CompProperties_AbilitySprayLiquid)props;

	private Pawn Pawn => parent.pawn;

	public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		foreach (IntVec3 item in AffectedCells(target))
		{
			((Projectile)GenSpawn.Spawn(Props.projectileDef, Pawn.Position, Pawn.Map)).Launch(Pawn, Pawn.DrawPos, item, item, ProjectileHitFlags.IntendedTarget);
		}
		if (Props.sprayEffecter != null)
		{
			Props.sprayEffecter.Spawn(parent.pawn.Position, target.Cell, parent.pawn.Map).Cleanup();
		}
		base.Apply(target, dest);
	}

	public override void DrawEffectPreview(LocalTargetInfo target)
	{
		GenDraw.DrawFieldEdges(AffectedCells(target));
	}

	public override bool AICanTargetNow(LocalTargetInfo target)
	{
		if (Pawn.Faction != null)
		{
			foreach (IntVec3 item in AffectedCells(target))
			{
				List<Thing> thingList = item.GetThingList(Pawn.Map);
				for (int i = 0; i < thingList.Count; i++)
				{
					if (thingList[i].Faction == Pawn.Faction)
					{
						return false;
					}
				}
			}
		}
		return true;
	}

	private List<IntVec3> AffectedCells(LocalTargetInfo target)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		tmpCellDots.Clear();
		tmpCells.Clear();
		tmpCellDots.Add(new Pair<IntVec3, float>(target.Cell, 999f));
		if (Props.numCellsToHit > 1)
		{
			Vector3 val = Pawn.Position.ToVector3Shifted().Yto0();
			Vector3 val2 = target.Cell.ToVector3Shifted().Yto0();
			IntVec3[] adjacentCells = GenAdj.AdjacentCells;
			for (int i = 0; i < adjacentCells.Length; i++)
			{
				IntVec3 first = target.Cell + adjacentCells[i];
				Vector3 val3 = first.ToVector3Shifted().Yto0();
				Vector3 val4 = val3 - val;
				Vector3 normalized = ((Vector3)(ref val4)).normalized;
				val4 = val3 - val2;
				float second = Vector3.Dot(normalized, ((Vector3)(ref val4)).normalized);
				tmpCellDots.Add(new Pair<IntVec3, float>(first, second));
			}
			tmpCellDots.SortBy((Pair<IntVec3, float> x) => 0f - x.Second);
		}
		Map map = Pawn.Map;
		int num = Mathf.Min(tmpCellDots.Count, Props.numCellsToHit);
		for (int num2 = 0; num2 < num; num2++)
		{
			IntVec3 first2 = tmpCellDots[num2].First;
			if (!first2.InBounds(map))
			{
				continue;
			}
			if (first2.Filled(map))
			{
				Building_Door door = first2.GetDoor(map);
				if (door == null || !door.Open)
				{
					continue;
				}
			}
			if (parent.verb.TryFindShootLineFromTo(Pawn.Position, first2, out var _, ignoreRange: true))
			{
				tmpCells.Add(first2);
			}
		}
		return tmpCells;
	}
}
