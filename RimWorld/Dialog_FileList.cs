using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Verse;

namespace RimWorld;

public abstract class Dialog_FileList : Window
{
	protected string interactButLabel = "Error";

	protected float bottomAreaHeight;

	protected List<SaveFileInfo> files = new List<SaveFileInfo>();

	protected Vector2 scrollPosition = Vector2.zero;

	protected string typingName = "";

	private bool focusedNameArea;

	protected string deleteTipKey = "DeleteThisSavegame";

	protected const float EntryHeight = 40f;

	protected const float FileNameLeftMargin = 8f;

	protected const float FileNameRightMargin = 4f;

	protected const float FileInfoWidth = 94f;

	protected const float InteractButWidth = 100f;

	protected const float InteractButHeight = 36f;

	protected const float DeleteButSize = 36f;

	private static readonly Color DefaultFileTextColor = new Color(1f, 1f, 0.6f);

	protected const float NameTextFieldWidth = 400f;

	protected const float NameTextFieldHeight = 35f;

	protected const float NameTextFieldButtonSpace = 20f;

	public override Vector2 InitialSize => new Vector2(620f, 700f);

	protected virtual bool ShouldDoTypeInField => false;

	public Dialog_FileList()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		doCloseButton = true;
		doCloseX = true;
		forcePause = true;
		absorbInputAroundWindow = true;
		closeOnAccept = false;
		ReloadFiles();
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(((Rect)(ref inRect)).width - 16f, 40f);
		float y = val.y;
		float num = (float)files.Count * y;
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(0f, 0f, ((Rect)(ref inRect)).width - 16f, num);
		float num2 = ((Rect)(ref inRect)).height - Window.CloseButSize.y - bottomAreaHeight - 18f;
		if (ShouldDoTypeInField)
		{
			num2 -= 53f;
		}
		Rect outRect = inRect.TopPartPixels(num2);
		Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
		float num3 = 0f;
		int num4 = 0;
		Rect rect = default(Rect);
		Rect val2 = default(Rect);
		Rect rect2 = default(Rect);
		Rect rect3 = default(Rect);
		Rect rect4 = default(Rect);
		foreach (SaveFileInfo file in files)
		{
			if (num3 + val.y >= scrollPosition.y && num3 <= scrollPosition.y + ((Rect)(ref outRect)).height)
			{
				((Rect)(ref rect))._002Ector(0f, num3, val.x, val.y);
				if (num4 % 2 == 0)
				{
					Widgets.DrawAltRect(rect);
				}
				Widgets.BeginGroup(rect);
				((Rect)(ref val2))._002Ector(((Rect)(ref rect)).width - 36f, (((Rect)(ref rect)).height - 36f) / 2f, 36f, 36f);
				if (Widgets.ButtonImage(val2, TexButton.Delete, Color.white, GenUI.SubtleMouseoverColor))
				{
					FileInfo localFile = file.FileInfo;
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(localFile.Name), delegate
					{
						localFile.Delete();
						ReloadFiles();
					}, destructive: true));
				}
				TooltipHandler.TipRegionByKey(val2, deleteTipKey);
				Text.Font = GameFont.Small;
				((Rect)(ref rect2))._002Ector(((Rect)(ref val2)).x - 100f, (((Rect)(ref rect)).height - 36f) / 2f, 100f, 36f);
				if (Widgets.ButtonText(rect2, interactButLabel))
				{
					DoFileInteraction(Path.GetFileNameWithoutExtension(file.FileName));
				}
				((Rect)(ref rect3))._002Ector(((Rect)(ref rect2)).x - 94f, 0f, 94f, ((Rect)(ref rect)).height);
				DrawDateAndVersion(file, rect3);
				GUI.color = Color.white;
				Text.Anchor = (TextAnchor)0;
				GUI.color = FileNameColor(file);
				((Rect)(ref rect4))._002Ector(8f, 0f, ((Rect)(ref rect3)).x - 8f - 4f, ((Rect)(ref rect)).height);
				Text.Anchor = (TextAnchor)3;
				Text.Font = GameFont.Small;
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
				Widgets.Label(rect4, fileNameWithoutExtension.Truncate(((Rect)(ref rect4)).width * 1.8f));
				GUI.color = Color.white;
				Text.Anchor = (TextAnchor)0;
				Widgets.EndGroup();
			}
			num3 += val.y;
			num4++;
		}
		Widgets.EndScrollView();
		if (ShouldDoTypeInField)
		{
			DoTypeInField(inRect.TopPartPixels(((Rect)(ref inRect)).height - Window.CloseButSize.y - 18f));
		}
	}

	protected abstract void DoFileInteraction(string fileName);

	protected abstract void ReloadFiles();

	protected virtual void DoTypeInField(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		bool flag = (int)Event.current.type == 4 && (int)Event.current.keyCode == 13;
		float num = ((Rect)(ref rect)).height - 35f;
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)3;
		GUI.SetNextControlName("MapNameField");
		string str = Widgets.TextField(new Rect(5f, num, 400f, 35f), typingName);
		if (GenText.IsValidFilename(str))
		{
			typingName = str;
		}
		if (!focusedNameArea)
		{
			UI.FocusControl("MapNameField", this);
			focusedNameArea = true;
		}
		if (Widgets.ButtonText(new Rect(420f, num, ((Rect)(ref rect)).width - 400f - 20f, 35f), "SaveGameButton".Translate()) || flag)
		{
			if (typingName.NullOrEmpty())
			{
				Messages.Message("NeedAName".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			}
			else
			{
				DoFileInteraction(typingName?.Trim());
			}
		}
		Text.Anchor = (TextAnchor)0;
		Widgets.EndGroup();
	}

	protected virtual Color FileNameColor(SaveFileInfo sfi)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return DefaultFileTextColor;
	}

	public static void DrawDateAndVersion(SaveFileInfo sfi, Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		Widgets.BeginGroup(rect);
		Text.Font = GameFont.Tiny;
		Text.Anchor = (TextAnchor)0;
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(0f, 2f, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height / 2f);
		GUI.color = SaveFileInfo.UnimportantTextColor;
		Widgets.Label(rect2, sfi.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(0f, ((Rect)(ref rect2)).yMax, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height / 2f);
		GUI.color = sfi.VersionColor;
		Widgets.Label(rect3, sfi.GameVersion);
		if (Mouse.IsOver(rect3))
		{
			TooltipHandler.TipRegion(rect3, sfi.CompatibilityTip);
		}
		Widgets.EndGroup();
	}
}
