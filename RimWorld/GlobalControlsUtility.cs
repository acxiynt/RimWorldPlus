using System;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class GlobalControlsUtility
{
	private const int VisibilityControlsPerRow = 5;

	public static void DoPlaySettings(WidgetRow rowVisibility, bool worldView, ref float curBaseY)
	{
		float y = curBaseY - TimeControls.TimeButSize.y;
		rowVisibility.Init(UI.screenWidth, y, UIDirection.LeftThenUp, 141f);
		Find.PlaySettings.DoPlaySettingsGlobalControls(rowVisibility, worldView);
		curBaseY = rowVisibility.FinalY;
	}

	public static void DoTimespeedControls(float leftX, float width, ref float curBaseY)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		leftX += Mathf.Max(0f, width - 150f);
		width = Mathf.Min(width, 150f);
		float y = TimeControls.TimeButSize.y;
		Rect timerRect = default(Rect);
		((Rect)(ref timerRect))._002Ector(leftX + 16f, curBaseY - y, width, y);
		TimeControls.DoTimeControlsGUI(timerRect);
		curBaseY -= ((Rect)(ref timerRect)).height;
	}

	public static void DoDate(float leftX, float width, ref float curBaseY)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Rect dateRect = default(Rect);
		((Rect)(ref dateRect))._002Ector(leftX, curBaseY - DateReadout.Height, width, DateReadout.Height);
		DateReadout.DateOnGUI(dateRect);
		curBaseY -= ((Rect)(ref dateRect)).height;
	}

	public static void DoRealtimeClock(float leftX, float width, ref float curBaseY)
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(leftX - 20f, curBaseY - 26f, width + 20f - 7f, 26f);
		string text = (Prefs.TwelveHourClockMode ? "hh:mm" : "HH:mm");
		string text2 = "";
		if (Prefs.TwelveHourClockMode)
		{
			text2 = string.Format(" {0}", (DateTime.Now.Hour >= 12) ? "PM".Translate() : "AM".Translate());
		}
		using (new TextBlock((TextAnchor)5))
		{
			Widgets.Label(rect, DateTime.Now.ToString(text) + text2);
		}
		curBaseY -= 26f;
	}
}
