using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld.Planet;

public class WITab_Caravan_Needs : WITab
{
	private Vector2 scrollPosition;

	private float scrollViewHeight;

	private Pawn specificNeedsTabForPawn;

	private Vector2 thoughtScrollPosition;

	private bool doNeeds;

	private float SpecificNeedsTabWidth
	{
		get
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (specificNeedsTabForPawn.DestroyedOrNull())
			{
				return 0f;
			}
			return NeedsCardUtility.GetSize(specificNeedsTabForPawn).x;
		}
	}

	public WITab_Caravan_Needs()
	{
		labelKey = "TabCaravanNeeds";
	}

	protected override void FillTab()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		EnsureSpecificNeedsTabForPawnValid();
		CaravanNeedsTabUtility.DoRows(size, base.SelCaravan.PawnsListForReading, base.SelCaravan, ref scrollPosition, ref scrollViewHeight, ref specificNeedsTabForPawn, doNeeds);
	}

	protected override void UpdateSize()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		EnsureSpecificNeedsTabForPawnValid();
		base.UpdateSize();
		size = CaravanNeedsTabUtility.GetSize(base.SelCaravan.PawnsListForReading, PaneTopY);
		if (size.x + SpecificNeedsTabWidth > (float)UI.screenWidth)
		{
			doNeeds = false;
			size = CaravanNeedsTabUtility.GetSize(base.SelCaravan.PawnsListForReading, PaneTopY, doNeeds: false);
		}
		else
		{
			doNeeds = true;
		}
		size.y = Mathf.Max(size.y, NeedsCardUtility.FullSize.y);
	}

	protected override void ExtraOnGUI()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		EnsureSpecificNeedsTabForPawnValid();
		base.ExtraOnGUI();
		Pawn localSpecificNeedsTabForPawn = specificNeedsTabForPawn;
		if (localSpecificNeedsTabForPawn == null)
		{
			return;
		}
		Rect tabRect = base.TabRect;
		float specificNeedsTabWidth = SpecificNeedsTabWidth;
		Rect rect = new Rect(((Rect)(ref tabRect)).xMax - 1f, ((Rect)(ref tabRect)).yMin, specificNeedsTabWidth, ((Rect)(ref tabRect)).height);
		Find.WindowStack.ImmediateWindow(1439870015, rect, WindowLayer.GameUI, delegate
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			if (!localSpecificNeedsTabForPawn.DestroyedOrNull())
			{
				NeedsCardUtility.DoNeedsMoodAndThoughts(rect.AtZero(), localSpecificNeedsTabForPawn, ref thoughtScrollPosition);
				if (Widgets.CloseButtonFor(rect.AtZero()))
				{
					specificNeedsTabForPawn = null;
					SoundDefOf.TabClose.PlayOneShotOnCamera();
				}
			}
		});
	}

	public override void Notify_ClearingAllMapsMemory()
	{
		base.Notify_ClearingAllMapsMemory();
		specificNeedsTabForPawn = null;
	}

	private void EnsureSpecificNeedsTabForPawnValid()
	{
		if (specificNeedsTabForPawn != null && (specificNeedsTabForPawn.Destroyed || !base.SelCaravan.ContainsPawn(specificNeedsTabForPawn)))
		{
			specificNeedsTabForPawn = null;
		}
	}
}
