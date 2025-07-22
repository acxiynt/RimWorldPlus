using Verse;

namespace RimWorld;

public class WorkGiver_TendOther : WorkGiver_Tend
{
	public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
	{
		if (pawn.health.hediffSet.BleedRateTotal > 0f)
		{
			return base.HasJobOnThing(pawn, (Thing)null, forced);
		}
		if (base.HasJobOnThing(pawn, t, forced))
		{
			return pawn != t;
		}
		return false;
	}
}
