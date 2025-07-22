using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

[StaticConstructorOnStartup]
public class MainTabWindow_Inspect : MainTabWindow, IInspectPane
{
	private Type openTabType;

	private float recentHeight;

	private static IntVec3 lastSelectCell;

	private Gizmo mouseoverGizmo;

	private Gizmo lastMouseOverGizmo;

	private Thing lastSelectedThing;

	private Vector2 lastTabSize;

	private const float GuiltyTexSize = 26f;

	public Type OpenTabType
	{
		get
		{
			return openTabType;
		}
		set
		{
			openTabType = value;
		}
	}

	public float RecentHeight
	{
		get
		{
			return recentHeight;
		}
		set
		{
			recentHeight = value;
		}
	}

	protected override float Margin => 0f;

	public override Vector2 RequestedTabSize => InspectPaneUtility.PaneSizeFor(this);

	private List<object> Selected => Find.Selector.SelectedObjects;

	private Thing SelThing => Find.Selector.SingleSelectedThing;

	private Zone SelZone => Find.Selector.SelectedZone;

	private int NumSelected => Find.Selector.NumSelected;

	public float PaneTopY => (float)UI.screenHeight - 165f - 35f;

	public bool AnythingSelected => NumSelected > 0;

	public Gizmo LastMouseoverGizmo => lastMouseOverGizmo;

	public bool ShouldShowSelectNextInCellButton
	{
		get
		{
			if (NumSelected == 1)
			{
				if (Find.Selector.SelectedZone != null)
				{
					return Find.Selector.SelectedZone.ContainsCell(lastSelectCell);
				}
				return true;
			}
			return false;
		}
	}

	public bool ShouldShowPaneContents
	{
		get
		{
			if (SelThing != null && SelThing.def.hideInspect && !DebugSettings.showHiddenInfo)
			{
				return false;
			}
			if (TryGetSelectedStorageGroup(out var _))
			{
				return true;
			}
			return NumSelected == 1;
		}
	}

	public IEnumerable<InspectTabBase> CurTabs
	{
		get
		{
			if (Find.ScreenshotModeHandler.Active)
			{
				return null;
			}
			if (NumSelected == 1)
			{
				if (SelThing != null && (SelThing.def.inspectorTabsResolved != null || SelThing is IStorageGroupMember { DrawStorageTab: not false }))
				{
					return SelThing.GetInspectTabs();
				}
				if (SelZone != null)
				{
					return SelZone.GetInspectTabs();
				}
			}
			else if (Selected.Count > 1 && Selected.All((object s) => s is IStorageGroupMember))
			{
				return (Selected.First() as Thing)?.GetInspectTabs();
			}
			return null;
		}
	}

	public MainTabWindow_Inspect()
	{
		closeOnAccept = false;
		closeOnCancel = false;
		drawInScreenshotMode = false;
		soundClose = SoundDefOf.TabClose;
	}

	public override void ExtraOnGUI()
	{
		base.ExtraOnGUI();
		InspectPaneUtility.ExtraOnGUI(this);
		if (AnythingSelected && Find.DesignatorManager.SelectedDesignator != null)
		{
			Find.DesignatorManager.SelectedDesignator.DoExtraGuiControls(0f, PaneTopY);
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		InspectPaneUtility.InspectPaneOnGUI(inRect, this);
		if (lastSelectedThing != SelThing)
		{
			SetInitialSizeAndPosition();
			lastSelectedThing = SelThing;
		}
		else if (RequestedTabSize != lastTabSize)
		{
			SetInitialSizeAndPosition();
			lastTabSize = RequestedTabSize;
		}
	}

	public string GetLabel(Rect rect)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return InspectPaneUtility.AdjustedLabelFor(Selected, rect);
	}

	public void DrawInspectGizmos()
	{
		InspectGizmoGrid.DrawInspectGizmoGridFor(Selected, out mouseoverGizmo);
	}

	public void DoPaneContents(Rect rect)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSelectedStorageGroup(out var group))
		{
			InspectPaneFiller.DoPaneContentsForStorageGroup(group, rect);
		}
		else
		{
			InspectPaneFiller.DoPaneContentsFor((ISelectable)Find.Selector.FirstSelectedObject, rect);
		}
	}

	public void DoInspectPaneButtons(Rect rect, ref float lineEndWidth)
	{
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		StorageGroup group;
		if (NumSelected == 1)
		{
			Thing singleSelectedThing = Find.Selector.SingleSelectedThing;
			if (singleSelectedThing != null)
			{
				float num = ((Rect)(ref rect)).width - 48f;
				Widgets.InfoCardButton(num, 0f, Find.Selector.SingleSelectedThing);
				lineEndWidth += 24f;
				Pawn p;
				CompAnimalPenMarker comp;
				if ((p = singleSelectedThing as Pawn) != null)
				{
					if (p.playerSettings != null && p.playerSettings.UsesConfigurableHostilityResponse)
					{
						num -= 24f;
						HostilityResponseModeUtility.DrawResponseButton(new Rect(num, 0f, 24f, 24f), p, paintable: false);
						lineEndWidth += 24f;
					}
					if ((p.Faction == Faction.OfPlayer && p.RaceProps.Animal && p.RaceProps.hideTrainingTab) || (ModsConfig.BiotechActive && p.IsColonyMech))
					{
						num -= 30f;
						RenameUIUtility.DrawRenameButton(new Rect(num, 0f, 30f, 30f), p);
						lineEndWidth += 30f;
					}
					if (p.guilt != null && p.guilt.IsGuilty)
					{
						num -= 26f;
						Rect val = new Rect(num, 0f, 26f, 26f);
						GUI.DrawTexture(val, (Texture)(object)TexUI.GuiltyTex);
						TooltipHandler.TipRegion(val, () => p.guilt.Tip, 6321223);
						lineEndWidth += 26f;
					}
				}
				else if (singleSelectedThing is IStorageGroupMember { ShowRenameButton: not false } storageGroupMember)
				{
					num -= 30f;
					Rect rect2 = default(Rect);
					((Rect)(ref rect2))._002Ector(num, 0f, 30f, 30f);
					if (storageGroupMember.Group != null)
					{
						RenameUIUtility.DrawRenameButton(rect2, storageGroupMember.Group);
					}
					else if (singleSelectedThing.Spawned)
					{
						RenameUIUtility.DrawRenameButton(rect2, storageGroupMember);
					}
					lineEndWidth += 30f;
				}
				else if (singleSelectedThing.Spawned && singleSelectedThing.Faction == Faction.OfPlayer && singleSelectedThing.TryGetComp(out comp))
				{
					num -= 30f;
					RenameUIUtility.DrawRenameButton(new Rect(num, 0f, 30f, 30f), comp);
					lineEndWidth += 30f;
				}
			}
			else if (Find.Selector.SelectedZone != null)
			{
				Rect rect3 = default(Rect);
				((Rect)(ref rect3))._002Ector(((Rect)(ref rect)).width - 30f, 0f, 30f, 30f);
				if (ShouldShowSelectNextInCellButton)
				{
					((Rect)(ref rect3)).x = ((Rect)(ref rect3)).x - 24f;
				}
				RenameUIUtility.DrawRenameButton(rect3, Find.Selector.SelectedZone);
				lineEndWidth += 30f;
			}
		}
		else if (TryGetSelectedStorageGroup(out group))
		{
			float num2 = ((Rect)(ref rect)).width - 30f;
			RenameUIUtility.DrawRenameButton(new Rect(num2, 0f, 30f, 30f), group);
			lineEndWidth += 30f;
		}
	}

	private bool TryGetSelectedStorageGroup(out StorageGroup group)
	{
		bool flag = true;
		group = null;
		if (Find.Selector.SelectedObjects.Count <= 1)
		{
			group = null;
			return false;
		}
		foreach (object selectedObject in Find.Selector.SelectedObjects)
		{
			if (selectedObject is IStorageGroupMember storageGroupMember)
			{
				if (group == null)
				{
					group = storageGroupMember.Group;
				}
				if (storageGroupMember.Group != group || storageGroupMember.Group == null)
				{
					flag = false;
					break;
				}
				continue;
			}
			flag = false;
			break;
		}
		if (flag)
		{
			return group != null;
		}
		return false;
	}

	public void SelectNextInCell()
	{
		if (NumSelected != 1)
		{
			return;
		}
		Selector selector = Find.Selector;
		if (selector.SelectedZone == null || selector.SelectedZone.ContainsCell(lastSelectCell))
		{
			if (selector.SelectedZone == null)
			{
				lastSelectCell = selector.SingleSelectedThing.PositionHeld;
			}
			selector.SelectNextAt(map: (selector.SingleSelectedThing == null) ? selector.SelectedZone.Map : selector.SingleSelectedThing.MapHeld, c: lastSelectCell);
		}
	}

	public override void WindowUpdate()
	{
		base.WindowUpdate();
		InspectPaneUtility.UpdateTabs(this);
		lastMouseOverGizmo = mouseoverGizmo;
		if (mouseoverGizmo != null)
		{
			mouseoverGizmo.GizmoUpdateOnMouseover();
		}
	}

	public void CloseOpenTab()
	{
		openTabType = null;
	}

	public void Reset()
	{
		openTabType = null;
	}
}
