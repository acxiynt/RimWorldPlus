using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Gizmo_GrowthTier : Gizmo
{
	private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(GenUI.FillableBar_Empty);

	private const float Spacing = 8f;

	private const float LabelWidthPercent = 0.55f;

	private const float BarMarginY = 2f;

	private const int GrowthTierTooltipId = 837825001;

	private Pawn child;

	private Texture2D barTex;

	private float Width => 190f;

	private int GrowthTier => child.ageTracker.GrowthTier;

	public override bool Visible
	{
		get
		{
			if (!child.IsColonistPlayerControlled && !child.IsPrisonerOfColony)
			{
				return child.IsSlaveOfColony;
			}
			return true;
		}
	}

	public override float GetWidth(float maxWidth)
	{
		return Width;
	}

	public Gizmo_GrowthTier(Pawn child)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		this.child = child;
		barTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.1254902f, 46f / 85f, 0f));
		Order = -100f;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Rect val = GenUI.ContractedBy(rect, 8f);
		Widgets.DrawWindowBackground(rect);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref val)).width, ((Rect)(ref val)).height / 2f);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(((Rect)(ref val)).x, ((Rect)(ref rect2)).yMax, ((Rect)(ref val)).width, ((Rect)(ref rect2)).height);
		((Rect)(ref rect2)).yMax = ((Rect)(ref rect2)).yMax - 2f;
		((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin + 2f;
		DrawGrowthTier(rect2);
		DrawLearning(rect3);
		return new GizmoResult(GizmoState.Clear);
	}

	private string GrowthTierTooltip(Rect rect, int tier)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		TaggedString taggedString = ("StatsReport_GrowthTier".Translate() + ": ").AsTipTitle() + tier + "\n" + "StatsReport_GrowthTierDesc".Translate().Colorize(ColoredText.SubtleGrayColor) + "\n\n";
		if (child.ageTracker.AtMaxGrowthTier)
		{
			taggedString += ("MaxTier".Translate() + ": ").AsTipTitle() + "MaxTierDesc".Translate(child.Named("PAWN"));
		}
		else
		{
			taggedString += ("ProgressToNextGrowthTier".Translate() + ": ").AsTipTitle() + Mathf.FloorToInt(child.ageTracker.growthPoints).ToString() + " / " + GrowthUtility.GrowthTierPointsRequirements[tier + 1];
			if (child.ageTracker.canGainGrowthPoints)
			{
				taggedString += string.Format(" (+{0})", "PerDay".Translate(child.ageTracker.GrowthPointsPerDay.ToStringByStyle(ToStringStyle.FloatMaxTwo)));
			}
		}
		if (child.ageTracker.AgeBiologicalYears < 13)
		{
			int num = 0;
			for (int i = child.ageTracker.AgeBiologicalYears + 1; i <= 13; i++)
			{
				if (GrowthUtility.IsGrowthBirthday(i))
				{
					num = i;
					break;
				}
			}
			taggedString += "\n\n" + ("NextGrowthMomentAt".Translate() + ": ").AsTipTitle() + num;
		}
		taggedString += "\n\n" + ("ThisGrowthTier".Translate(tier) + ":").AsTipTitle();
		if (GrowthUtility.PassionGainsPerTier[tier] > 0)
		{
			taggedString += "\n  - " + "NumPassionsFromOptions".Translate(GrowthUtility.PassionGainsPerTier[tier], GrowthUtility.PassionChoicesPerTier[tier]);
		}
		taggedString += "\n  - " + "NumTraitsFromOptions".Translate(GrowthUtility.TraitGainsPerTier[tier], GrowthUtility.TraitChoicesPerTier[tier]);
		if (!child.ageTracker.AtMaxGrowthTier)
		{
			taggedString += "\n\n" + ("NextGrowthTier".Translate(tier + 1) + ":").AsTipTitle();
			if (GrowthUtility.PassionGainsPerTier[tier + 1] > 0)
			{
				taggedString += "\n  - " + "NumPassionsFromOptions".Translate(GrowthUtility.PassionGainsPerTier[tier + 1], GrowthUtility.PassionChoicesPerTier[tier + 1]);
			}
			taggedString += "\n  - " + "NumTraitsFromOptions".Translate(GrowthUtility.TraitGainsPerTier[tier + 1], GrowthUtility.TraitChoicesPerTier[tier + 1]);
		}
		return taggedString.Resolve();
	}

	private void DrawGrowthTier(Rect rect)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		int growthTier = GrowthTier;
		Rect rect2 = rect;
		((Rect)(ref rect2)).xMax = ((Rect)(ref rect)).x + ((Rect)(ref rect)).width * 0.55f;
		string label = (string)("StatsReport_GrowthTier".Translate() + ": ") + growthTier;
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)3;
		Widgets.Label(rect2, label);
		Text.Anchor = (TextAnchor)0;
		float percentToNextGrowthTier = child.ageTracker.PercentToNextGrowthTier;
		Rect rect3 = rect;
		((Rect)(ref rect3)).xMin = ((Rect)(ref rect2)).xMax;
		((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin + 2f;
		((Rect)(ref rect3)).yMax = ((Rect)(ref rect3)).yMax - 2f;
		Widgets.FillableBar(rect3, percentToNextGrowthTier, barTex, EmptyBarTex, doBorder: true);
		Text.Anchor = (TextAnchor)4;
		float num = GrowthUtility.GrowthTierPointsRequirements[GrowthUtility.GrowthTierPointsRequirements.Length - 1];
		string label2 = (child.ageTracker.AtMaxGrowthTier ? (num + " / " + num) : (Mathf.FloorToInt(child.ageTracker.growthPoints).ToString() + " / " + GrowthUtility.GrowthTierPointsRequirements[growthTier + 1]));
		Widgets.Label(rect3, label2);
		Text.Anchor = (TextAnchor)0;
		if (Mouse.IsOver(rect))
		{
			Widgets.DrawHighlight(rect);
			string text = GrowthTierTooltip(rect, growthTier);
			TooltipHandler.TipRegion(rect, new TipSignal(text, child.thingIDNumber ^ 0x31F031E9));
		}
	}

	private void DrawLearning(Rect rect)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		if (child.needs.learning != null)
		{
			Rect rect2 = rect;
			((Rect)(ref rect2)).xMax = ((Rect)(ref rect)).x + ((Rect)(ref rect)).width * 0.55f;
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)3;
			Widgets.Label(rect2, NeedDefOf.Learning.LabelCap);
			Text.Anchor = (TextAnchor)0;
			Rect rect3 = rect;
			((Rect)(ref rect3)).xMin = ((Rect)(ref rect2)).xMax;
			((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin + 2f;
			((Rect)(ref rect3)).yMax = ((Rect)(ref rect3)).yMax - 2f;
			Widgets.FillableBar(rect3, child.needs.learning.CurLevelPercentage, Widgets.BarFullTexHor, EmptyBarTex, doBorder: true);
			Text.Anchor = (TextAnchor)4;
			string label = child.needs.learning.CurLevelPercentage.ToStringPercent();
			Widgets.Label(rect3, label);
			Text.Anchor = (TextAnchor)0;
			if (Mouse.IsOver(rect))
			{
				Widgets.DrawHighlight(rect);
				TooltipHandler.TipRegion(rect, child.needs.learning.GetTipString());
			}
		}
	}
}
