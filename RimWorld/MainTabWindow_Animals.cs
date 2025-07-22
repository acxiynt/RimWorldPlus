using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class MainTabWindow_Animals : MainTabWindow_PawnTable
{
	protected override PawnTableDef PawnTableDef => PawnTableDefOf.Animals;

	protected override IEnumerable<Pawn> Pawns => from p in Find.CurrentMap.mapPawns.PawnsInFaction(Faction.OfPlayer)
		where p.IsNonMutantAnimal
		select p;

	public override void DoWindowContents(Rect rect)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		base.DoWindowContents(rect);
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, Mathf.Min(((Rect)(ref rect)).width, 260f), 32f), "ManageAutoSlaughter".Translate()))
		{
			Find.WindowStack.Add(new Dialog_AutoSlaughter(Find.CurrentMap));
		}
	}
}
