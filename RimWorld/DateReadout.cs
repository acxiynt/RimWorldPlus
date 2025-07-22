using System;
using System.Collections.Generic;
using System.Text;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld;

public static class DateReadout
{
	private static string dateString;

	private static int dateStringDay;

	private static Season dateStringSeason;

	private static Quadrum dateStringQuadrum;

	private static int dateStringYear;

	private static readonly List<string> fastHourStrings24h;

	private static readonly List<string> fastHourStrings12h;

	private static readonly List<string> seasonsCached;

	private const float DateRightPadding = 7f;

	public static float Height => 48 + (SeasonLabelVisible ? 26 : 0);

	private static bool SeasonLabelVisible
	{
		get
		{
			if (!WorldRendererUtility.WorldRenderedNow)
			{
				return Find.CurrentMap != null;
			}
			return false;
		}
	}

	static DateReadout()
	{
		dateStringDay = -1;
		dateStringSeason = Season.Undefined;
		dateStringQuadrum = Quadrum.Undefined;
		dateStringYear = -1;
		fastHourStrings24h = new List<string>();
		fastHourStrings12h = new List<string>();
		seasonsCached = new List<string>();
		Reset();
	}

	public static void Reset()
	{
		dateString = null;
		dateStringDay = -1;
		dateStringSeason = Season.Undefined;
		dateStringQuadrum = Quadrum.Undefined;
		dateStringYear = -1;
		fastHourStrings24h.Clear();
		for (int i = 0; i < 24; i++)
		{
			fastHourStrings24h.Add(i + (string)"LetterHour".Translate());
		}
		fastHourStrings12h.Clear();
		for (int j = 0; j < 24; j++)
		{
			TaggedString taggedString = ((j >= 12) ? "PM".Translate() : "AM".Translate());
			string item = ((j == 0) ? $"12 {taggedString}" : ((j > 12) ? $"{j - 12} {taggedString}" : $"{j} {taggedString}"));
			fastHourStrings12h.Add(item);
		}
		seasonsCached.Clear();
		int length = Enum.GetValues(typeof(Season)).Length;
		for (int k = 0; k < length; k++)
		{
			Season season = (Season)k;
			seasonsCached.Add((season == Season.Undefined) ? "" : season.LabelCap());
		}
	}

	public static void DateOnGUI(Rect dateRect)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val;
		if (WorldRendererUtility.WorldRenderedNow && Find.WorldSelector.selectedTile >= 0)
		{
			val = Find.WorldGrid.LongLatOf(Find.WorldSelector.selectedTile);
		}
		else if (WorldRendererUtility.WorldRenderedNow && Find.WorldSelector.NumSelectedObjects > 0)
		{
			val = Find.WorldGrid.LongLatOf(Find.WorldSelector.FirstSelectedObject.Tile);
		}
		else
		{
			if (Find.CurrentMap == null)
			{
				return;
			}
			val = Find.WorldGrid.LongLatOf(Find.CurrentMap.Tile);
		}
		int index = GenDate.HourInteger(Find.TickManager.TicksAbs, val.x);
		int num = GenDate.DayOfTwelfth(Find.TickManager.TicksAbs, val.x);
		Season season = GenDate.Season(Find.TickManager.TicksAbs, val);
		Quadrum quadrum = GenDate.Quadrum(Find.TickManager.TicksAbs, val.x);
		int num2 = GenDate.Year(Find.TickManager.TicksAbs, val.x);
		string text = (SeasonLabelVisible ? seasonsCached[(int)season] : "");
		if (num != dateStringDay || season != dateStringSeason || quadrum != dateStringQuadrum || num2 != dateStringYear)
		{
			dateString = GenDate.DateReadoutStringAt(Find.TickManager.TicksAbs, val);
			dateStringDay = num;
			dateStringSeason = season;
			dateStringQuadrum = quadrum;
			dateStringYear = num2;
		}
		Text.Font = GameFont.Small;
		float num3 = Mathf.Max(Mathf.Max(Text.CalcSize(Prefs.TwelveHourClockMode ? fastHourStrings12h[index] : fastHourStrings24h[index]).x, Text.CalcSize(dateString).x), Text.CalcSize(text).x) + 7f;
		((Rect)(ref dateRect)).xMin = ((Rect)(ref dateRect)).xMax - num3;
		if (Mouse.IsOver(dateRect))
		{
			Widgets.DrawHighlight(dateRect);
		}
		Widgets.BeginGroup(dateRect);
		Text.Font = GameFont.Small;
		Text.Anchor = (TextAnchor)2;
		Rect rect = dateRect.AtZero();
		((Rect)(ref rect)).xMax = ((Rect)(ref rect)).xMax - 7f;
		if (Prefs.TwelveHourClockMode)
		{
			Widgets.Label(rect, fastHourStrings12h[index]);
		}
		else
		{
			Widgets.Label(rect, fastHourStrings24h[index]);
		}
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 26f;
		Widgets.Label(rect, dateString);
		((Rect)(ref rect)).yMin = ((Rect)(ref rect)).yMin + 26f;
		if (!text.NullOrEmpty())
		{
			Widgets.Label(rect, text);
		}
		Text.Anchor = (TextAnchor)0;
		Widgets.EndGroup();
		if (Mouse.IsOver(dateRect))
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 4; i++)
			{
				Quadrum quadrum2 = (Quadrum)i;
				stringBuilder.AppendLine(quadrum2.Label() + " - " + quadrum2.GetSeason(val.y).LabelCap());
			}
			TaggedString taggedString = "DateReadoutTip".Translate(GenDate.DaysPassed, 15, season.LabelCap(), 15, GenDate.Quadrum(GenTicks.TicksAbs, val.x).Label(), stringBuilder.ToString());
			TooltipHandler.TipRegion(dateRect, new TipSignal(taggedString, 86423));
		}
	}
}
