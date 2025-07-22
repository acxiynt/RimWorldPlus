using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Gizmo_RefuelableFuelStatus : Gizmo
{
	public CompRefuelable refuelable;

	private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.35f, 0.35f, 0.2f));

	private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.black);

	private static readonly Texture2D TargetLevelArrow = ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarkerRotated");

	private const float ArrowScale = 0.5f;

	public Gizmo_RefuelableFuelStatus()
	{
		Order = -100f;
	}

	public override float GetWidth(float maxWidth)
	{
		return 140f;
	}

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Rect overRect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
		Find.WindowStack.ImmediateWindow(1523289473, overRect, WindowLayer.GameUI, delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0167: Unknown result type (might be due to invalid IL or missing references)
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			Rect rect;
			Rect val = (rect = overRect.AtZero().ContractedBy(6f));
			((Rect)(ref rect)).height = ((Rect)(ref overRect)).height / 2f;
			Text.Font = GameFont.Tiny;
			Widgets.Label(rect, refuelable.Props.FuelGizmoLabel);
			Rect rect2 = val;
			((Rect)(ref rect2)).yMin = ((Rect)(ref overRect)).height / 2f;
			float fillPercent = refuelable.Fuel / refuelable.Props.fuelCapacity;
			Widgets.FillableBar(rect2, fillPercent, FullBarTex, EmptyBarTex, doBorder: false);
			if (refuelable.Props.targetFuelLevelConfigurable)
			{
				float num = refuelable.TargetFuelLevel / refuelable.Props.fuelCapacity;
				float num2 = ((Rect)(ref rect2)).x + num * ((Rect)(ref rect2)).width - (float)((Texture)TargetLevelArrow).width * 0.5f / 2f;
				float num3 = ((Rect)(ref rect2)).y - (float)((Texture)TargetLevelArrow).height * 0.5f;
				GUI.DrawTexture(new Rect(num2, num3, (float)((Texture)TargetLevelArrow).width * 0.5f, (float)((Texture)TargetLevelArrow).height * 0.5f), (Texture)(object)TargetLevelArrow);
			}
			Text.Font = GameFont.Small;
			Text.Anchor = (TextAnchor)4;
			Widgets.Label(rect2, refuelable.Fuel.ToString("F0") + " / " + refuelable.Props.fuelCapacity.ToString("F0"));
			Text.Anchor = (TextAnchor)0;
		});
		return new GizmoResult(GizmoState.Clear);
	}
}
