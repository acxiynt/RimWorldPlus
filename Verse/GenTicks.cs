using RimWorld;
using UnityEngine;

namespace Verse;

public static class GenTicks
{
	public const int TicksPerRealSecond = 60;

	public const int TickRareInterval = 250;

	public const int TickLongInterval = 2000;

	public const float SecondsPerTick = 1f / 60f;

	public static int TicksAbs
	{
		get
		{
			if (Current.ProgramState != ProgramState.Playing && Find.GameInitData != null && Find.GameInitData.gameToLoad.NullOrEmpty())
			{
				return ConfiguredTicksAbsAtGameStart;
			}
			if (Current.Game != null && Find.TickManager != null)
			{
				return Find.TickManager.TicksAbs;
			}
			return 0;
		}
	}

	public static int TicksGame
	{
		get
		{
			if (Current.Game != null && Find.TickManager != null)
			{
				return Find.TickManager.TicksGame;
			}
			return 0;
		}
	}

	public static int ConfiguredTicksAbsAtGameStart
	{
		get
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			GameInitData gameInitData = Find.GameInitData;
			ConfiguredTicksAbsAtGameStartCache ticksAbsCache = Find.World.ticksAbsCache;
			if (ticksAbsCache.TryGetCachedValue(gameInitData, out var ticksAbs))
			{
				return ticksAbs;
			}
			Vector2 val = ((gameInitData.startingTile < 0) ? Vector2.zero : Find.WorldGrid.LongLatOf(gameInitData.startingTile));
			Twelfth twelfth = ((gameInitData.startingSeason != Season.Undefined) ? gameInitData.startingSeason.GetFirstTwelfth(val.y) : ((gameInitData.startingTile < 0) ? Season.Summer.GetFirstTwelfth(0f) : TwelfthUtility.FindStartingWarmTwelfth(gameInitData.startingTile)));
			int num = (24 - GenDate.TimeZoneAt(val.x)) % 24;
			int num2 = 300000 * (int)twelfth + 2500 * (6 + num);
			ticksAbsCache.Cache(num2, gameInitData);
			return num2;
		}
	}

	public static float TicksToSeconds(this int numTicks)
	{
		return (float)numTicks / 60f;
	}

	public static int SecondsToTicks(this float numSeconds)
	{
		return Mathf.RoundToInt(60f * numSeconds);
	}

	public static string ToStringSecondsFromTicks(this int numTicks)
	{
		return numTicks.TicksToSeconds().ToString("F1") + " " + "SecondsLower".Translate();
	}

	public static string ToStringSecondsFromTicks(this int numTicks, string format)
	{
		return numTicks.TicksToSeconds().ToString(format) + " " + "SecondsLower".Translate();
	}

	public static string ToStringTicksFromSeconds(this float numSeconds)
	{
		return numSeconds.SecondsToTicks().ToString();
	}

	public static int GetTickIntervalOffset(int index, int count, int period)
	{
		return Mathf.CeilToInt((float)period / (float)count * (float)index % (float)period);
	}

	public static bool IsTickInterval(int offset, int period)
	{
		return (TicksGame + offset) % period == 0;
	}

	public static bool IsTickInterval(int period)
	{
		return TicksGame % period == 0;
	}
}
