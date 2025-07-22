using System.Collections.Generic;
using LudeonTK;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public static class TrainingCardUtility
{
	public const float RowHeight = 28f;

	public const float RenameButtonSize = 30f;

	private const float InfoHeaderHeight = 50f;

	[TweakValue("Interface", -100f, 300f)]
	private static float TrainabilityLeft = 220f;

	[TweakValue("Interface", -100f, 300f)]
	private static float TrainabilityTop = 0f;

	private static readonly Texture2D LearnedTrainingTex = ContentFinder<Texture2D>.Get("UI/Icons/FixedCheck");

	private static readonly Texture2D LearnedNotTrainingTex = ContentFinder<Texture2D>.Get("UI/Icons/FixedCheckOff");

	public static void DrawTrainingCard(Rect rect, Pawn pawn)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Small;
		RenameUIUtility.DrawRenameButton(new Rect(TrainabilityLeft, TrainabilityTop, 30f, 30f), pawn);
		Listing_Standard listing_Standard = new Listing_Standard();
		listing_Standard.Begin(rect);
		listing_Standard.Label("CreatureTrainability".Translate(pawn.def.label).CapitalizeFirst() + ": " + pawn.RaceProps.trainability.LabelCap, 22f);
		listing_Standard.Label("CreatureWildness".Translate(pawn.def.label).CapitalizeFirst() + ": " + pawn.RaceProps.wildness.ToStringPercent(), 22f, TrainableUtility.GetWildnessExplanation(pawn.def));
		if (pawn.training.HasLearned(TrainableDefOf.Obedience))
		{
			Rect rect2 = listing_Standard.GetRect(25f);
			Widgets.Label(rect2, "Master".Translate() + ": ");
			((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).center.x;
			if (pawn.RaceProps.playerCanChangeMaster || !ModsConfig.IdeologyActive)
			{
				TrainableUtility.MasterSelectButton(rect2, pawn, paintable: false);
			}
			else if (pawn.playerSettings?.Master != null)
			{
				Widgets.Label(rect2, TrainableUtility.MasterString(pawn).Truncate(((Rect)(ref rect2)).width));
				TooltipHandler.TipRegion(rect2, "DryadCannotChangeMaster".Translate(pawn.Named("ANIMAL"), pawn.playerSettings.Master.Named("MASTER")).CapitalizeFirst());
			}
			listing_Standard.Gap();
			Rect rect3 = listing_Standard.GetRect(25f);
			bool checkOn = pawn.playerSettings.followDrafted;
			Widgets.CheckboxLabeled(rect3, "CreatureFollowDrafted".Translate(), ref checkOn);
			if (checkOn != pawn.playerSettings.followDrafted)
			{
				pawn.playerSettings.followDrafted = checkOn;
			}
			Rect rect4 = listing_Standard.GetRect(25f);
			bool checkOn2 = pawn.playerSettings.followFieldwork;
			Widgets.CheckboxLabeled(rect4, "CreatureFollowFieldwork".Translate(), ref checkOn2);
			if (checkOn2 != pawn.playerSettings.followFieldwork)
			{
				pawn.playerSettings.followFieldwork = checkOn2;
			}
		}
		if (pawn.RaceProps.showTrainables)
		{
			listing_Standard.Gap();
			float num = 50f;
			List<TrainableDef> trainableDefsInListOrder = TrainableUtility.TrainableDefsInListOrder;
			for (int i = 0; i < trainableDefsInListOrder.Count; i++)
			{
				if (TryDrawTrainableRow(listing_Standard.GetRect(28f), pawn, trainableDefsInListOrder[i]))
				{
					num += 28f;
				}
			}
		}
		listing_Standard.End();
	}

	public static float TotalHeightForPawn(Pawn p)
	{
		if (p == null)
		{
			return 0f;
		}
		int num = 0;
		if (p.RaceProps.showTrainables)
		{
			for (int i = 0; i < DefDatabase<TrainableDef>.AllDefsListForReading.Count; i++)
			{
				p.training.CanAssignToTrain(DefDatabase<TrainableDef>.AllDefsListForReading[i], out var visible);
				if (visible)
				{
					num++;
				}
			}
		}
		float num2 = 112f + 28f * (float)num;
		if (p.training.HasLearned(TrainableDefOf.Obedience))
		{
			num2 += 75f;
			num2 += 12f;
		}
		return num2;
	}

	private static bool TryDrawTrainableRow(Rect rect, Pawn pawn, TrainableDef td)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		bool flag = pawn.training.HasLearned(td);
		bool visible;
		AcceptanceReport canTrain = pawn.training.CanAssignToTrain(td, out visible);
		if (!visible)
		{
			return false;
		}
		Widgets.DrawHighlightIfMouseover(rect);
		Rect rect2 = rect;
		((Rect)(ref rect2)).width = ((Rect)(ref rect2)).width - 50f;
		((Rect)(ref rect2)).xMin = ((Rect)(ref rect2)).xMin + (float)td.indent * 10f;
		Rect val = rect;
		((Rect)(ref val)).xMin = ((Rect)(ref val)).xMax - 50f + 17f;
		DoTrainableCheckbox(rect2, pawn, td, canTrain, drawLabel: true, doTooltip: false);
		if (flag)
		{
			GUI.color = Color.green;
		}
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(val, pawn.training.GetSteps(td) + " / " + td.steps);
		Text.Anchor = (TextAnchor)0;
		if (DebugSettings.godMode && !pawn.training.HasLearned(td))
		{
			Rect rect3 = val;
			((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMax - 10f;
			((Rect)(ref rect3)).xMin = ((Rect)(ref rect3)).xMax - 10f;
			if (Widgets.ButtonText(rect3, "+"))
			{
				pawn.training.Train(td, pawn.Map.mapPawns.FreeColonistsSpawned.RandomElement());
			}
		}
		DoTrainableTooltip(rect, pawn, td, canTrain);
		GUI.color = Color.white;
		return true;
	}

	public static void DoTrainableCheckbox(Rect rect, Pawn pawn, TrainableDef td, AcceptanceReport canTrain, bool drawLabel, bool doTooltip)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		bool num = pawn.training.HasLearned(td);
		bool checkOn = pawn.training.GetWanted(td);
		bool flag = checkOn;
		Texture2D texChecked = (num ? LearnedTrainingTex : null);
		Texture2D texUnchecked = (num ? LearnedNotTrainingTex : null);
		if (drawLabel)
		{
			Widgets.CheckboxLabeled(rect, td.LabelCap, ref checkOn, !canTrain.Accepted, texChecked, texUnchecked);
		}
		else
		{
			Widgets.Checkbox(((Rect)(ref rect)).position, ref checkOn, ((Rect)(ref rect)).width, !canTrain.Accepted, paintable: true, texChecked, texUnchecked);
		}
		if (checkOn != flag)
		{
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.AnimalTraining, KnowledgeAmount.Total);
			pawn.training.SetWantedRecursive(td, checkOn);
		}
		if (doTooltip)
		{
			DoTrainableTooltip(rect, pawn, td, canTrain);
		}
	}

	private static void DoTrainableTooltip(Rect rect, Pawn pawn, TrainableDef td, AcceptanceReport canTrain)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!Mouse.IsOver(rect))
		{
			return;
		}
		TooltipHandler.TipRegion(rect, delegate
		{
			string text = td.LabelCap + "\n\n" + td.description;
			if (!canTrain.Accepted)
			{
				text = text + "\n\n" + canTrain.Reason;
			}
			else if (!td.prerequisites.NullOrEmpty())
			{
				text += "\n";
				for (int i = 0; i < td.prerequisites.Count; i++)
				{
					if (!pawn.training.HasLearned(td.prerequisites[i]))
					{
						text += "\n" + "TrainingNeedsPrerequisite".Translate(td.prerequisites[i].LabelCap);
					}
				}
			}
			return text;
		}, (int)(((Rect)(ref rect)).y * 612f + ((Rect)(ref rect)).x));
	}
}
