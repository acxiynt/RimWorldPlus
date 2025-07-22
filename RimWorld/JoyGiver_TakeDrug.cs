using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld;

public class JoyGiver_TakeDrug : JoyGiver_Ingest
{
	private static List<ThingDef> takeableDrugs = new List<ThingDef>();

	protected override Thing BestIngestItem(Pawn pawn, Predicate<Thing> extraValidator)
	{
		if (pawn.drugs == null)
		{
			return null;
		}
		Predicate<Thing> predicate = delegate(Thing t)
		{
			if (!CanIngestForJoy(pawn, t))
			{
				return false;
			}
			if (extraValidator != null && !extraValidator(t))
			{
				return false;
			}
			return (t.def.ingestible != null && t.def.ingestible.drugCategory != DrugCategory.None) ? true : false;
		};
		ThingOwner<Thing> innerContainer = pawn.inventory.innerContainer;
		for (int num = 0; num < innerContainer.Count; num++)
		{
			if (predicate(innerContainer[num]))
			{
				return innerContainer[num];
			}
		}
		bool flag = false;
		if (pawn.story != null && (pawn.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire) > 0 || pawn.InMentalState))
		{
			flag = true;
		}
		takeableDrugs.Clear();
		DrugPolicy currentPolicy = pawn.drugs.CurrentPolicy;
		for (int num2 = 0; num2 < currentPolicy.Count; num2++)
		{
			if (flag || currentPolicy[num2].allowedForJoy)
			{
				takeableDrugs.Add(currentPolicy[num2].drug);
			}
		}
		takeableDrugs.Shuffle();
		for (int num3 = 0; num3 < takeableDrugs.Count; num3++)
		{
			List<Thing> list = pawn.Map.listerThings.ThingsOfDef(takeableDrugs[num3]);
			if (list.Count > 0)
			{
				Thing thing = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, list, PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, predicate);
				if (thing != null)
				{
					return thing;
				}
			}
		}
		return null;
	}

	public override float GetChance(Pawn pawn)
	{
		int num = 0;
		if (pawn.story != null)
		{
			num = pawn.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire);
		}
		if (num < 0)
		{
			return 0f;
		}
		float num2 = def.baseChance;
		if (num == 1)
		{
			num2 *= 2f;
		}
		if (num == 2)
		{
			num2 *= 5f;
		}
		return num2;
	}

	protected override Job CreateIngestJob(Thing ingestible, Pawn pawn)
	{
		return DrugAIUtility.IngestAndTakeToInventoryJob(ingestible, pawn);
	}
}
