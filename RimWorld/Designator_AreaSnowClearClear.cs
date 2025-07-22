using UnityEngine;
using Verse;

namespace RimWorld;

public class Designator_AreaSnowClearClear : Designator_AreaSnowClear
{
	public Designator_AreaSnowClearClear()
		: base(DesignateMode.Remove)
	{
		defaultLabel = "DesignatorAreaSnowClearClear".Translate();
		defaultDesc = "DesignatorAreaSnowClearClearDesc".Translate();
		icon = (Texture)(object)ContentFinder<Texture2D>.Get("UI/Designators/SnowClearAreaOff");
		soundDragSustain = SoundDefOf.Designate_DragAreaDelete;
		soundDragChanged = null;
		soundSucceeded = SoundDefOf.Designate_ZoneDelete;
	}
}
