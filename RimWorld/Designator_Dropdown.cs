using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Designator_Dropdown : DesignatorWithEyedropper
{
	private List<Designator> elements = new List<Designator>();

	private Designator activeDesignator;

	private bool activeDesignatorSet;

	public static readonly Texture2D PlusTex = ContentFinder<Texture2D>.Get("UI/Widgets/PlusOptions");

	private const float ButSize = 16f;

	private const float ButPadding = 1f;

	public override string Label => activeDesignator.Label + (activeDesignatorSet ? "" : "...");

	public override string Desc => activeDesignator.Desc;

	public override Color IconDrawColor => activeDesignator.IconDrawColor;

	public override bool Visible
	{
		get
		{
			for (int i = 0; i < elements.Count; i++)
			{
				if (elements[i].Visible)
				{
					return true;
				}
			}
			return false;
		}
	}

	public List<Designator> Elements => elements;

	public override float PanelReadoutTitleExtraRightMargin => activeDesignator.PanelReadoutTitleExtraRightMargin;

	public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		GizmoResult result = base.GizmoOnGUI(topLeft, maxWidth, parms);
		DrawExtraOptionsIcon(topLeft, GetWidth(maxWidth));
		return result;
	}

	public static void DrawExtraOptionsIcon(Vector2 topLeft, float width)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		GUI.DrawTexture(new Rect(topLeft.x + width - 16f - 1f, topLeft.y + 1f, 16f, 16f), (Texture)(object)PlusTex);
	}

	public void Add(Designator des)
	{
		elements.Add(des);
		if (activeDesignator == null)
		{
			SetActiveDesignator(des, explicitySet: false);
		}
	}

	public void SetActiveDesignator(Designator des, bool explicitySet = true)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		activeDesignator = des;
		icon = des.icon;
		iconDrawScale = des.iconDrawScale;
		iconProportions = des.iconProportions;
		iconTexCoords = des.iconTexCoords;
		iconAngle = des.iconAngle;
		iconOffset = des.iconOffset;
		if (explicitySet)
		{
			activeDesignatorSet = true;
		}
	}

	public override void DrawMouseAttachments()
	{
		activeDesignator.DrawMouseAttachments();
	}

	public override void ProcessInput(Event ev)
	{
		BuildableDef placingDef;
		Window window = (elements.Any((Designator x) => x is Designator_Place designator_Place && (placingDef = designator_Place.PlacingDef) != null && placingDef.designatorDropdown.useGridMenu) ? SetupGridMenu(ev) : SetupFloatMenu(ev));
		Find.WindowStack.Add(window);
		Find.DesignatorManager.Select(activeDesignator);
	}

	private Window SetupGridMenu(Event ev)
	{
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Expected O, but got Unknown
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		List<FloatMenuGridOption> list = new List<FloatMenuGridOption>();
		if (elements.Any((Designator x) => x is Designator_Place { PlacingDef: TerrainDef placingDef2 } && placingDef2.designatorDropdown.includeEyeDropperTool))
		{
			if (eyedropper == null)
			{
				eyedropper = new Designator_Eyedropper(delegate(ColorDef newCol)
				{
					for (int i = 0; i < elements.Count; i++)
					{
						Designator designator2 = elements[i];
						if (designator2 is Designator_Place { PlacingDef: TerrainDef placingDef2 } && placingDef2.colorDef == newCol)
						{
							GetDesignatorSelectAction(ev, designator2)();
							break;
						}
					}
				}, "SelectColoredFloor".Translate(), "DesignatorEyeDropperDesc_Carpet".Translate());
			}
			list.Add(new FloatMenuGridOption(Designator_Eyedropper.EyeDropperTex, delegate
			{
				Find.DesignatorManager.Select(eyedropper);
			}, null, "DesignatorEyeDropperDesc_Carpet".Translate()));
		}
		for (int num = 0; num < elements.Count; num++)
		{
			Designator designator = elements[num];
			if (!designator.Visible)
			{
				continue;
			}
			BuildableDef placingDef;
			if (designator is Designator_Place designator_Place && (placingDef = designator_Place.PlacingDef) != null)
			{
				ThingDef designatorCost;
				if (placingDef.designatorDropdown.iconSource == DesignatorDropdownGroupDef.IconSource.Cost && (designatorCost = GetDesignatorCost(designator)) != null)
				{
					list.Add(new FloatMenuGridOption(Widgets.GetIconFor(designatorCost), GetDesignatorSelectAction(ev, designator), null, designator.LabelCap));
				}
				else if (placingDef.designatorDropdown.iconSource == DesignatorDropdownGroupDef.IconSource.Placed)
				{
					FloatMenuGridOption floatMenuGridOption = new FloatMenuGridOption((Texture2D)designator_Place.icon, GetDesignatorSelectAction(ev, designator), designator_Place.IconDrawColor, designator.LabelCap);
					if (placingDef is TerrainDef)
					{
						floatMenuGridOption.iconTexCoords = Widgets.CroppedTerrainTextureRect((Texture2D)designator_Place.icon);
					}
					list.Add(floatMenuGridOption);
				}
			}
			else
			{
				Log.Error("Trying to setup grid float menu with designator without icon.");
			}
		}
		return new FloatMenuGrid(list)
		{
			onCloseCallback = delegate
			{
				activeDesignatorSet = true;
			}
		};
	}

	private Window SetupFloatMenu(Event ev)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		List<FloatMenuOption> list = new List<FloatMenuOption>();
		for (int i = 0; i < elements.Count; i++)
		{
			Designator designator = elements[i];
			if (!designator.Visible)
			{
				continue;
			}
			BuildableDef placingDef;
			if (designator is Designator_Place designator_Place && (placingDef = designator_Place.PlacingDef) != null)
			{
				ThingDef designatorCost;
				if (placingDef.designatorDropdown.iconSource == DesignatorDropdownGroupDef.IconSource.Cost && (designatorCost = GetDesignatorCost(designator)) != null)
				{
					list.Add(new FloatMenuOption(designator.LabelCap, GetDesignatorSelectAction(ev, designator), designatorCost));
					continue;
				}
				if (placingDef.designatorDropdown.iconSource == DesignatorDropdownGroupDef.IconSource.Placed)
				{
					FloatMenuOption floatMenuOption = new FloatMenuOption(designator.LabelCap, GetDesignatorSelectAction(ev, designator), (Texture2D)designator_Place.icon, designator_Place.IconDrawColor);
					if (placingDef is TerrainDef)
					{
						floatMenuOption.iconTexCoords = Widgets.CroppedTerrainTextureRect((Texture2D)designator_Place.icon);
					}
					list.Add(floatMenuOption);
					continue;
				}
			}
			list.Add(new FloatMenuOption(designator.LabelCap, GetDesignatorSelectAction(ev, designator)));
		}
		return new FloatMenu(list)
		{
			onCloseCallback = delegate
			{
				activeDesignatorSet = true;
			}
		};
	}

	private Action GetDesignatorSelectAction(Event ev, Designator des)
	{
		return delegate
		{
			base.ProcessInput(ev);
			Find.DesignatorManager.Select(des);
			SetActiveDesignator(des);
		};
	}

	public override void DrawIcon(Rect rect, Material buttonMat, GizmoRenderParms parms)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		activeDesignator?.DrawIcon(rect, buttonMat, parms);
	}

	public override AcceptanceReport CanDesignateCell(IntVec3 loc)
	{
		return activeDesignator.CanDesignateCell(loc);
	}

	public override void SelectedUpdate()
	{
		activeDesignator.SelectedUpdate();
	}

	public override void DrawPanelReadout(ref float curY, float width)
	{
		activeDesignator.DrawPanelReadout(ref curY, width);
	}

	private ThingDef GetDesignatorCost(Designator des)
	{
		if (des is Designator_Place { PlacingDef: var placingDef } && placingDef.CostList != null && placingDef.CostList.Count > 0)
		{
			return placingDef.CostList.MaxBy((ThingDefCountClass c) => c.thingDef.BaseMarketValue * (float)c.count).thingDef;
		}
		return null;
	}
}
