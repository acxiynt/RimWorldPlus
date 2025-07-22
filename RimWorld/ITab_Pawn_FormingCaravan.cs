using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace RimWorld;

public class ITab_Pawn_FormingCaravan : ITab
{
	private Vector2 scrollPosition;

	private float lastDrawnHeight;

	private List<Thing> thingsToSelect = new List<Thing>();

	private static List<Thing> tmpSingleThing = new List<Thing>();

	private const float TopPadding = 20f;

	private const float StandardLineHeight = 22f;

	private const float ExtraSpaceBetweenSections = 4f;

	private const float SpaceBetweenItemsLists = 10f;

	private const float ThingRowHeight = 28f;

	private const float ThingIconSize = 28f;

	private const float ThingLeftX = 36f;

	private static readonly Color ThingLabelColor = ITab_Pawn_Gear.ThingLabelColor;

	private static readonly Color ThingHighlightColor = ITab_Pawn_Gear.HighlightColor;

	private static List<Thing> tmpPawns = new List<Thing>();

	public override bool IsVisible => SelPawn.IsFormingCaravan();

	public ITab_Pawn_FormingCaravan()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		size = new Vector2(480f, 450f);
		labelKey = "TabFormingCaravan";
	}

	protected override void FillTab()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		thingsToSelect.Clear();
		Rect outRect = GenUI.ContractedBy(new Rect(default(Vector2), size), 10f);
		((Rect)(ref outRect)).yMin = ((Rect)(ref outRect)).yMin + 20f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, ((Rect)(ref outRect)).width - 16f, Mathf.Max(lastDrawnHeight, ((Rect)(ref outRect)).height));
		Widgets.BeginScrollView(outRect, ref scrollPosition, val);
		float num = 0f;
		string status = ((LordJob_FormAndSendCaravan)SelPawn.GetLord().LordJob).Status;
		Widgets.Label(new Rect(0f, num, ((Rect)(ref val)).width, 100f), status);
		num += 22f;
		num += 4f;
		DoPeopleAndAnimals(val, ref num);
		num += 4f;
		DoItemsLists(val, ref num);
		lastDrawnHeight = num;
		Widgets.EndScrollView();
		if (thingsToSelect.Any())
		{
			SelectNow(thingsToSelect);
			thingsToSelect.Clear();
		}
	}

	public override void TabUpdate()
	{
		base.TabUpdate();
		if (SelPawn != null && SelPawn.GetLord() != null)
		{
			Lord lord = SelPawn.GetLord();
			for (int i = 0; i < lord.ownedPawns.Count; i++)
			{
				TargetHighlighter.Highlight(lord.ownedPawns[i], arrow: false, colonistBar: false, circleOverlay: true);
			}
		}
	}

	private void DoPeopleAndAnimals(Rect inRect, ref float curY)
	{
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		Widgets.ListSeparator(ref curY, ((Rect)(ref inRect)).width, "CaravanMembers".Translate());
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		Lord lord = SelPawn.GetLord();
		for (int i = 0; i < lord.ownedPawns.Count; i++)
		{
			Pawn pawn = lord.ownedPawns[i];
			if (pawn.IsFreeColonist)
			{
				num++;
				if (pawn.InMentalState)
				{
					num2++;
				}
			}
			else if (pawn.IsPrisoner)
			{
				num3++;
				if (pawn.InMentalState)
				{
					num4++;
				}
			}
			else if (pawn.RaceProps.Animal)
			{
				num5++;
				if (pawn.InMentalState)
				{
					num6++;
				}
				if (pawn.RaceProps.packAnimal)
				{
					num7++;
				}
			}
		}
		string pawnsCountLabel = GetPawnsCountLabel(num, num2, -1);
		string pawnsCountLabel2 = GetPawnsCountLabel(num3, num4, -1);
		string pawnsCountLabel3 = GetPawnsCountLabel(num5, num6, num7);
		float num8 = curY;
		DoPeopleAndAnimalsEntry(inRect, Faction.OfPlayer.def.pawnsPlural.CapitalizeFirst(), pawnsCountLabel, ref curY, out var drawnWidth);
		float num9 = curY;
		DoPeopleAndAnimalsEntry(inRect, "CaravanPrisoners".Translate(), pawnsCountLabel2, ref curY, out var drawnWidth2);
		float num10 = curY;
		DoPeopleAndAnimalsEntry(inRect, "CaravanAnimals".Translate(), pawnsCountLabel3, ref curY, out var drawnWidth3);
		float num11 = Mathf.Max(new float[3] { drawnWidth, drawnWidth2, drawnWidth3 }) + 2f;
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, num8, num11, 22f);
		if (Mouse.IsOver(val))
		{
			Widgets.DrawHighlight(val);
			HighlightColonists();
		}
		if (Widgets.ButtonInvisible(val))
		{
			SelectColonistsLater();
		}
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(0f, num9, num11, 22f);
		if (Mouse.IsOver(val2))
		{
			Widgets.DrawHighlight(val2);
			HighlightPrisoners();
		}
		if (Widgets.ButtonInvisible(val2))
		{
			SelectPrisonersLater();
		}
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(0f, num10, num11, 22f);
		if (Mouse.IsOver(val3))
		{
			Widgets.DrawHighlight(val3);
			HighlightAnimals();
		}
		if (Widgets.ButtonInvisible(val3))
		{
			SelectAnimalsLater();
		}
	}

	private void DoPeopleAndAnimalsEntry(Rect inRect, string leftLabel, string rightLabel, ref float curY, out float drawnWidth)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, curY, ((Rect)(ref inRect)).width, 100f);
		Widgets.Label(rect, leftLabel);
		((Rect)(ref rect)).xMin = ((Rect)(ref rect)).xMin + 120f;
		Widgets.Label(rect, rightLabel);
		curY += 22f;
		drawnWidth = 120f + Text.CalcSize(rightLabel).x;
	}

	private void DoItemsLists(Rect inRect, ref float curY)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		LordJob_FormAndSendCaravan lordJob_FormAndSendCaravan = (LordJob_FormAndSendCaravan)SelPawn.GetLord().LordJob;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(0f, curY, (((Rect)(ref inRect)).width - 10f) / 2f, ((Rect)(ref inRect)).height);
		float curY2 = 0f;
		Widgets.BeginGroup(rect);
		Widgets.ListSeparator(ref curY2, ((Rect)(ref rect)).width, "ItemsToLoad".Translate());
		bool flag = false;
		for (int i = 0; i < lordJob_FormAndSendCaravan.transferables.Count; i++)
		{
			TransferableOneWay transferableOneWay = lordJob_FormAndSendCaravan.transferables[i];
			if (transferableOneWay.CountToTransfer > 0 && transferableOneWay.HasAnyThing)
			{
				flag = true;
				DoThingRow(transferableOneWay.AnyThing.GetInnerIfMinified().def, transferableOneWay.CountToTransfer, transferableOneWay.things, ((Rect)(ref rect)).width, ref curY2);
			}
		}
		if (!flag)
		{
			Widgets.NoneLabel(ref curY2, ((Rect)(ref rect)).width);
		}
		Widgets.EndGroup();
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector((((Rect)(ref inRect)).width + 10f) / 2f, curY, (((Rect)(ref inRect)).width - 10f) / 2f, ((Rect)(ref inRect)).height);
		float curY3 = 0f;
		Widgets.BeginGroup(rect2);
		Widgets.ListSeparator(ref curY3, ((Rect)(ref rect2)).width, "LoadedItems".Translate());
		bool flag2 = false;
		for (int j = 0; j < lordJob_FormAndSendCaravan.lord.ownedPawns.Count; j++)
		{
			Pawn pawn = lordJob_FormAndSendCaravan.lord.ownedPawns[j];
			if (!pawn.inventory.UnloadEverything)
			{
				for (int k = 0; k < pawn.inventory.innerContainer.Count; k++)
				{
					Thing thing = pawn.inventory.innerContainer[k];
					flag2 = true;
					tmpSingleThing.Clear();
					tmpSingleThing.Add(thing);
					DoThingRow(thing.def, thing.stackCount, tmpSingleThing, ((Rect)(ref rect2)).width, ref curY3);
					tmpSingleThing.Clear();
				}
			}
		}
		if (!flag2)
		{
			Widgets.NoneLabel(ref curY3, ((Rect)(ref rect)).width);
		}
		Widgets.EndGroup();
		curY += Mathf.Max(curY2, curY3);
	}

	private void SelectColonistsLater()
	{
		Lord lord = SelPawn.GetLord();
		tmpPawns.Clear();
		for (int i = 0; i < lord.ownedPawns.Count; i++)
		{
			if (lord.ownedPawns[i].IsFreeColonist)
			{
				tmpPawns.Add(lord.ownedPawns[i]);
			}
		}
		SelectLater(tmpPawns);
		tmpPawns.Clear();
	}

	private void HighlightColonists()
	{
		Lord lord = SelPawn.GetLord();
		for (int i = 0; i < lord.ownedPawns.Count; i++)
		{
			if (lord.ownedPawns[i].IsFreeColonist)
			{
				TargetHighlighter.Highlight(lord.ownedPawns[i]);
			}
		}
	}

	private void SelectPrisonersLater()
	{
		Lord lord = SelPawn.GetLord();
		tmpPawns.Clear();
		for (int i = 0; i < lord.ownedPawns.Count; i++)
		{
			if (lord.ownedPawns[i].IsPrisoner)
			{
				tmpPawns.Add(lord.ownedPawns[i]);
			}
		}
		SelectLater(tmpPawns);
		tmpPawns.Clear();
	}

	private void HighlightPrisoners()
	{
		Lord lord = SelPawn.GetLord();
		for (int i = 0; i < lord.ownedPawns.Count; i++)
		{
			if (lord.ownedPawns[i].IsPrisoner)
			{
				TargetHighlighter.Highlight(lord.ownedPawns[i]);
			}
		}
	}

	private void SelectAnimalsLater()
	{
		Lord lord = SelPawn.GetLord();
		tmpPawns.Clear();
		for (int i = 0; i < lord.ownedPawns.Count; i++)
		{
			if (lord.ownedPawns[i].RaceProps.Animal)
			{
				tmpPawns.Add(lord.ownedPawns[i]);
			}
		}
		SelectLater(tmpPawns);
		tmpPawns.Clear();
	}

	private void HighlightAnimals()
	{
		Lord lord = SelPawn.GetLord();
		for (int i = 0; i < lord.ownedPawns.Count; i++)
		{
			if (lord.ownedPawns[i].RaceProps.Animal)
			{
				TargetHighlighter.Highlight(lord.ownedPawns[i]);
			}
		}
	}

	private void SelectLater(List<Thing> things)
	{
		thingsToSelect.Clear();
		thingsToSelect.AddRange(things);
	}

	public static void SelectNow(List<Thing> things)
	{
		if (!things.Any())
		{
			return;
		}
		Find.Selector.ClearSelection();
		bool flag = false;
		for (int i = 0; i < things.Count; i++)
		{
			if (things[i].SpawnedOrAnyParentSpawned)
			{
				if (!flag)
				{
					CameraJumper.TryJump(things[i]);
				}
				Find.Selector.Select(things[i]);
				flag = true;
			}
		}
		if (!flag)
		{
			CameraJumper.TryJumpAndSelect(things[0]);
		}
	}

	private string GetPawnsCountLabel(int count, int countInMentalState, int countPackAnimals)
	{
		string text = count.ToString();
		bool flag = countInMentalState > 0;
		bool flag2 = count > 0 && countPackAnimals >= 0;
		if (flag || flag2)
		{
			text += " (";
			if (flag2)
			{
				text += countPackAnimals + " " + "PackAnimalsCountLower".Translate();
			}
			if (flag)
			{
				if (flag2)
				{
					text += ", ";
				}
				text += countInMentalState + " " + "InMentalStateCountLower".Translate();
			}
			text += ")";
		}
		return text;
	}

	private void DoThingRow(ThingDef thingDef, int count, List<Thing> things, float width, ref float curY)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, curY, width, 28f);
		if (things.Count == 1)
		{
			Widgets.InfoCardButton(((Rect)(ref val)).width - 24f, curY, things[0]);
		}
		else
		{
			Widgets.InfoCardButton(((Rect)(ref val)).width - 24f, curY, thingDef);
		}
		((Rect)(ref val)).width = ((Rect)(ref val)).width - 24f;
		if (Mouse.IsOver(val))
		{
			GUI.color = ThingHighlightColor;
			GUI.DrawTexture(val, (Texture)(object)TexUI.HighlightTex);
		}
		if ((Object)(object)thingDef.DrawMatSingle != (Object)null && (Object)(object)thingDef.DrawMatSingle.mainTexture != (Object)null)
		{
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(4f, curY, 28f, 28f);
			if (things.Count == 1)
			{
				Widgets.ThingIcon(rect, things[0]);
			}
			else
			{
				Widgets.ThingIcon(rect, thingDef);
			}
		}
		Text.Anchor = (TextAnchor)3;
		GUI.color = ThingLabelColor;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(36f, curY, ((Rect)(ref val)).width - 36f, ((Rect)(ref val)).height);
		string text = ((things.Count != 1 || count != things[0].stackCount) ? GenLabel.ThingLabel(thingDef, null, count).CapitalizeFirst() : things[0].LabelCap);
		Text.WordWrap = false;
		Widgets.Label(rect2, text.Truncate(((Rect)(ref rect2)).width));
		Text.WordWrap = true;
		Text.Anchor = (TextAnchor)0;
		TooltipHandler.TipRegion(val, text);
		if (Widgets.ButtonInvisible(val))
		{
			SelectLater(things);
		}
		if (Mouse.IsOver(val))
		{
			for (int i = 0; i < things.Count; i++)
			{
				TargetHighlighter.Highlight(things[i]);
			}
		}
		curY += 28f;
	}
}
