using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class ZoneColorUtility
{
	private static List<Color> growingZoneColors;

	private static List<Color> storageZoneColors;

	private static int nextGrowingZoneColorIndex;

	private static int nextStorageZoneColorIndex;

	private const float ZoneOpacity = 0.09f;

	static ZoneColorUtility()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		growingZoneColors = new List<Color>();
		storageZoneColors = new List<Color>();
		nextGrowingZoneColorIndex = 0;
		nextStorageZoneColorIndex = 0;
		Color item = default(Color);
		foreach (Color item3 in GrowingZoneColors())
		{
			((Color)(ref item))._002Ector(item3.r, item3.g, item3.b, 0.09f);
			growingZoneColors.Add(item);
		}
		Color item2 = default(Color);
		foreach (Color item4 in StorageZoneColors())
		{
			((Color)(ref item2))._002Ector(item4.r, item4.g, item4.b, 0.09f);
			storageZoneColors.Add(item2);
		}
	}

	public static Color NextGrowingZoneColor()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Color result = growingZoneColors[nextGrowingZoneColorIndex];
		nextGrowingZoneColorIndex++;
		if (nextGrowingZoneColorIndex >= growingZoneColors.Count)
		{
			nextGrowingZoneColorIndex = 0;
		}
		return result;
	}

	public static Color NextStorageZoneColor()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Color result = storageZoneColors[nextStorageZoneColorIndex];
		nextStorageZoneColorIndex++;
		if (nextStorageZoneColorIndex >= storageZoneColors.Count)
		{
			nextStorageZoneColorIndex = 0;
		}
		return result;
	}

	private static IEnumerable<Color> GrowingZoneColors()
	{
		yield return Color.Lerp(new Color(0f, 1f, 0f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(1f, 1f, 0f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(0.5f, 1f, 0f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(1f, 1f, 0.5f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(0.5f, 1f, 0.5f), Color.gray, 0.5f);
	}

	private static IEnumerable<Color> StorageZoneColors()
	{
		yield return Color.Lerp(new Color(1f, 0f, 0f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(1f, 0f, 1f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(0f, 0f, 1f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(1f, 0f, 0.5f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(0f, 0.5f, 1f), Color.gray, 0.5f);
		yield return Color.Lerp(new Color(0.5f, 0f, 1f), Color.gray, 0.5f);
	}
}
