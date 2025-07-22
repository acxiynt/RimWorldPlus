using System;
using System.Collections.Generic;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class WorldTargeter
{
	private Func<GlobalTargetInfo, bool> action;

	private bool canTargetTiles;

	private Texture2D mouseAttachment;

	public bool closeWorldTabWhenFinished;

	private Action onUpdate;

	private Func<GlobalTargetInfo, string> extraLabelGetter;

	private Func<GlobalTargetInfo, bool> canSelectTarget;

	private const float BaseFeedbackTexSize = 0.8f;

	public bool IsTargeting => action != null;

	public void BeginTargeting(Func<GlobalTargetInfo, bool> action, bool canTargetTiles, Texture2D mouseAttachment = null, bool closeWorldTabWhenFinished = false, Action onUpdate = null, Func<GlobalTargetInfo, string> extraLabelGetter = null, Func<GlobalTargetInfo, bool> canSelectTarget = null)
	{
		this.action = action;
		this.canTargetTiles = canTargetTiles;
		this.mouseAttachment = mouseAttachment;
		this.closeWorldTabWhenFinished = closeWorldTabWhenFinished;
		this.onUpdate = onUpdate;
		this.extraLabelGetter = extraLabelGetter;
		this.canSelectTarget = canSelectTarget;
	}

	public void StopTargeting()
	{
		if (closeWorldTabWhenFinished)
		{
			CameraJumper.TryHideWorld();
		}
		action = null;
		canTargetTiles = false;
		mouseAttachment = null;
		closeWorldTabWhenFinished = false;
		onUpdate = null;
		extraLabelGetter = null;
	}

	public void ProcessInputEvents()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Event.current.type == 0)
		{
			if (Event.current.button == 0 && IsTargeting)
			{
				GlobalTargetInfo arg = CurrentTargetUnderMouse();
				if ((canSelectTarget == null || canSelectTarget(arg)) && action(arg))
				{
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
					StopTargeting();
				}
				Event.current.Use();
			}
			if (Event.current.button == 1 && IsTargeting)
			{
				SoundDefOf.CancelMode.PlayOneShotOnCamera();
				StopTargeting();
				Event.current.Use();
			}
		}
		if (KeyBindingDefOf.Cancel.KeyDownEvent && IsTargeting)
		{
			SoundDefOf.CancelMode.PlayOneShotOnCamera();
			StopTargeting();
			Event.current.Use();
		}
	}

	public void TargeterOnGUI()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		if (!IsTargeting || Mouse.IsInputBlockedNow)
		{
			return;
		}
		Vector2 mousePosition = Event.current.mousePosition;
		Texture2D val = mouseAttachment ?? TexCommand.Attack;
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector(mousePosition.x + 8f, mousePosition.y + 8f, 32f, 32f);
		GUI.DrawTexture(val2, (Texture)(object)val);
		if (extraLabelGetter != null)
		{
			GUI.color = Color.white;
			string text = extraLabelGetter(CurrentTargetUnderMouse());
			if (!text.NullOrEmpty())
			{
				Color color = GUI.color;
				GUI.color = Color.white;
				Rect rect = default(Rect);
				((Rect)(ref rect))._002Ector(((Rect)(ref val2)).xMax, ((Rect)(ref val2)).y, 9999f, 100f);
				Vector2 val3 = Text.CalcSize(text);
				GUI.DrawTexture(new Rect(((Rect)(ref rect)).x - val3.x * 0.1f, ((Rect)(ref rect)).y, val3.x * 1.2f, val3.y), (Texture)(object)TexUI.GrayTextBG);
				GUI.color = color;
				Widgets.Label(rect, text);
			}
			GUI.color = Color.white;
		}
	}

	public void TargeterUpdate()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (IsTargeting)
		{
			Vector3 pos = Vector3.zero;
			GlobalTargetInfo arg = CurrentTargetUnderMouse();
			if (arg.HasWorldObject)
			{
				pos = arg.WorldObject.DrawPos;
			}
			else if (arg.Tile >= 0)
			{
				pos = Find.WorldGrid.GetTileCenter(arg.Tile);
			}
			if (arg.IsValid && !Mouse.IsInputBlockedNow && (canSelectTarget == null || canSelectTarget(arg)))
			{
				WorldRendererUtility.DrawQuadTangentialToPlanet(pos, 0.8f * Find.WorldGrid.averageTileSize, 0.018f, WorldMaterials.CurTargetingMat);
			}
			if (onUpdate != null)
			{
				onUpdate();
			}
		}
	}

	public bool IsTargetedNow(WorldObject o, List<WorldObject> worldObjectsUnderMouse = null)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!IsTargeting)
		{
			return false;
		}
		if (worldObjectsUnderMouse == null)
		{
			worldObjectsUnderMouse = GenWorldUI.WorldObjectsUnderMouse(UI.MousePositionOnUI);
		}
		if (worldObjectsUnderMouse.Any())
		{
			return o == worldObjectsUnderMouse[0];
		}
		return false;
	}

	private GlobalTargetInfo CurrentTargetUnderMouse()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!IsTargeting)
		{
			return GlobalTargetInfo.Invalid;
		}
		List<WorldObject> list = GenWorldUI.WorldObjectsUnderMouse(UI.MousePositionOnUI);
		if (list.Any())
		{
			return list[0];
		}
		if (canTargetTiles)
		{
			int num = GenWorld.MouseTile();
			if (num >= 0)
			{
				return new GlobalTargetInfo(num);
			}
			return GlobalTargetInfo.Invalid;
		}
		return GlobalTargetInfo.Invalid;
	}
}
