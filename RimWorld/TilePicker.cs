using System;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class TilePicker
{
	private static readonly Vector2 ButtonSize = new Vector2(150f, 38f);

	private const int Padding = 8;

	private const int BottomPanelYOffset = -50;

	private Func<int, bool> validator;

	private bool allowEscape;

	private bool active;

	private Action<int> tileChosen;

	private Action noTileChosen;

	private string title;

	public bool Active => active;

	public bool AllowEscape => allowEscape;

	public void StartTargeting(Func<int, bool> validator, Action<int> tileChosen, bool allowEscape = true, Action noTileChosen = null, string title = null)
	{
		this.validator = validator;
		this.allowEscape = allowEscape;
		this.noTileChosen = noTileChosen;
		this.tileChosen = tileChosen;
		this.title = title;
		Find.WorldSelector.ClearSelection();
		active = true;
	}

	public void StopTargeting()
	{
		if (active && noTileChosen != null)
		{
			noTileChosen();
		}
		StopTargetingInt();
	}

	private void StopTargetingInt()
	{
		active = false;
	}

	public void TileSelectorOnGUI()
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		if (!title.NullOrEmpty())
		{
			Text.Font = GameFont.Medium;
			Vector2 val = Text.CalcSize(title);
			Widgets.Label(new Rect((float)UI.screenWidth / 2f - val.x / 2f, 4f, val.x + 4f, val.y), title);
			Text.Font = GameFont.Small;
		}
		Vector2 buttonSize = ButtonSize;
		int num = 24;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector((float)UI.screenWidth / 2f - 2f * buttonSize.x / 2f - (float)num / 2f, (float)UI.screenHeight - (buttonSize.y + 8f) + -50f, 2f * buttonSize.x + (float)num, buttonSize.y + 16f);
		Widgets.DrawWindowBackground(rect);
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).x + 8f, ((Rect)(ref rect)).y + 8f, buttonSize.x, buttonSize.y), "SelectRandomSite".Translate()))
		{
			SoundDefOf.Click.PlayOneShotOnCamera();
			Find.WorldInterface.SelectedTile = TileFinder.RandomStartingTile();
			Find.WorldCameraDriver.JumpTo(Find.WorldGrid.GetTileCenter(Find.WorldInterface.SelectedTile));
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref rect)).x + 16f + buttonSize.x, ((Rect)(ref rect)).y + 8f, buttonSize.x, buttonSize.y), "Next".Translate()))
		{
			SoundDefOf.Click.PlayOneShotOnCamera();
			int selectedTile = Find.WorldInterface.SelectedTile;
			if (selectedTile < 0)
			{
				Messages.Message("MustSelectStartingSite".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			}
			else if (validator(selectedTile))
			{
				StopTargetingInt();
				tileChosen(selectedTile);
				Event.current.Use();
			}
		}
		if (KeyBindingDefOf.Cancel.KeyDownEvent && Active && !allowEscape)
		{
			Event.current.Use();
		}
	}
}
