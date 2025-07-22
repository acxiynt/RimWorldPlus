using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Dialog_AssignBuildingOwner : Window
{
	private CompAssignableToPawn assignable;

	private Vector2 scrollPosition;

	private const float EntryHeight = 35f;

	private const int AssignButtonWidth = 165;

	private const int SeparatorHeight = 7;

	private static readonly List<Pawn> tmpPawnSorted = new List<Pawn>(16);

	public override Vector2 InitialSize => new Vector2(520f, 500f);

	public Dialog_AssignBuildingOwner(CompAssignableToPawn assignable)
	{
		this.assignable = assignable;
		doCloseButton = true;
		doCloseX = true;
		closeOnClickedOutside = true;
		absorbInputAroundWindow = true;
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		Rect outRect = default(Rect);
		((Rect)(ref outRect))._002Ector(inRect);
		((Rect)(ref outRect)).yMin = ((Rect)(ref outRect)).yMin + 20f;
		((Rect)(ref outRect)).yMax = ((Rect)(ref outRect)).yMax - 40f;
		float num = 0f;
		num += (float)assignable.AssignedPawnsForReading.Count * 35f;
		num += (float)assignable.AssigningCandidates.Count() * 35f;
		num += 7f;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref outRect)).width, num);
		Widgets.AdjustRectsForScrollView(inRect, ref outRect, ref viewRect);
		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
		SortTmpList(assignable.AssignedPawnsForReading);
		float y = 0f;
		for (int i = 0; i < tmpPawnSorted.Count; i++)
		{
			Pawn pawn = tmpPawnSorted[i];
			DrawAssignedRow(pawn, ref y, viewRect, i);
		}
		if (assignable.AssignedPawnsForReading.Count > 0)
		{
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector(0f, y, ((Rect)(ref viewRect)).width, 7f);
			y += 7f;
			using (new TextBlock(Widgets.SeparatorLineColor))
			{
				Widgets.DrawLineHorizontal(((Rect)(ref val)).x, ((Rect)(ref val)).y + ((Rect)(ref val)).height / 2f, ((Rect)(ref val)).width);
			}
		}
		SortTmpList(assignable.AssigningCandidates);
		for (int j = 0; j < tmpPawnSorted.Count; j++)
		{
			Pawn pawn2 = tmpPawnSorted[j];
			DrawUnassignedRow(pawn2, ref y, viewRect, j);
		}
		tmpPawnSorted.Clear();
		Widgets.EndScrollView();
	}

	private void SortTmpList(IEnumerable<Pawn> collection)
	{
		tmpPawnSorted.Clear();
		tmpPawnSorted.AddRange(collection);
		tmpPawnSorted.SortBy((Pawn x) => x.LabelShort);
	}

	private void DrawAssignedRow(Pawn pawn, ref float y, Rect viewRect, int i)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, y, ((Rect)(ref viewRect)).width, 35f);
		y += 35f;
		if (i % 2 == 1)
		{
			Widgets.DrawLightHighlight(val);
		}
		Rect rect = val;
		((Rect)(ref rect)).width = ((Rect)(ref val)).height;
		Widgets.ThingIcon(rect, pawn);
		Rect rect2 = val;
		((Rect)(ref rect2)).xMin = ((Rect)(ref val)).xMax - 165f - 10f;
		rect2 = rect2.ContractedBy(2f);
		if (Widgets.ButtonText(rect2, "BuildingUnassign".Translate()))
		{
			assignable.TryUnassignPawn(pawn);
			SoundDefOf.Click.PlayOneShotOnCamera();
		}
		Rect rect3 = val;
		((Rect)(ref rect3)).xMin = ((Rect)(ref rect)).xMax + 10f;
		((Rect)(ref rect3)).xMax = ((Rect)(ref rect2)).xMin - 10f;
		using (new TextBlock((TextAnchor)3))
		{
			Widgets.LabelEllipses(rect3, pawn.LabelCap);
		}
	}

	private void DrawUnassignedRow(Pawn pawn, ref float y, Rect viewRect, int i)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		if (assignable.AssignedPawnsForReading.Contains(pawn))
		{
			return;
		}
		AcceptanceReport acceptanceReport = assignable.CanAssignTo(pawn);
		bool accepted = acceptanceReport.Accepted;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, y, ((Rect)(ref viewRect)).width, 35f);
		y += 35f;
		if (i % 2 == 1)
		{
			Widgets.DrawLightHighlight(val);
		}
		if (!accepted)
		{
			GUI.color = Color.gray;
		}
		Rect rect = val;
		((Rect)(ref rect)).width = ((Rect)(ref val)).height;
		Widgets.ThingIcon(rect, pawn);
		Rect rect2 = val;
		((Rect)(ref rect2)).xMin = ((Rect)(ref val)).xMax - 165f - 10f;
		rect2 = rect2.ContractedBy(2f);
		if (!Find.IdeoManager.classicMode && accepted && assignable.IdeoligionForbids(pawn))
		{
			Rect rect3 = val;
			((Rect)(ref rect3)).width = ((Rect)(ref val)).height;
			((Rect)(ref rect3)).x = ((Rect)(ref val)).xMax - ((Rect)(ref val)).height;
			rect3 = rect3.ContractedBy(2f);
			using (new TextBlock((TextAnchor)3))
			{
				Widgets.Label(rect2, "IdeoligionForbids".Translate());
			}
			IdeoUIUtility.DoIdeoIcon(rect3, pawn.ideo.Ideo, doTooltip: true, delegate
			{
				IdeoUIUtility.OpenIdeoInfo(pawn.ideo.Ideo);
				Close();
			});
		}
		else if (accepted)
		{
			TaggedString taggedString = (assignable.AssignedAnything(pawn) ? "BuildingReassign".Translate() : "BuildingAssign".Translate());
			if (Widgets.ButtonText(rect2, taggedString))
			{
				assignable.TryAssignPawn(pawn);
				if (assignable.MaxAssignedPawnsCount == 1)
				{
					Close();
				}
				else
				{
					SoundDefOf.Click.PlayOneShotOnCamera();
				}
			}
		}
		Rect rect4 = val;
		((Rect)(ref rect4)).xMin = ((Rect)(ref rect)).xMax + 10f;
		((Rect)(ref rect4)).xMax = ((Rect)(ref rect2)).xMin - 10f;
		string label = pawn.LabelCap + (accepted ? "" : (" (" + acceptanceReport.Reason.StripTags() + ")"));
		using (new TextBlock((TextAnchor)3))
		{
			Widgets.LabelEllipses(rect4, label);
		}
	}
}
