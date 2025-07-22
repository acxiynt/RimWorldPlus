using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace RimWorld.Planet;

public class WorldGenStep_Pollution : WorldGenStep
{
	private const float MinPollution = 0.25f;

	private const float MaxPollution = 1f;

	private const float PerlinFrequency = 0.1f;

	private List<int> tmpTiles = new List<int>();

	private Dictionary<int, float> tmpTileNoise = new Dictionary<int, float>();

	public override int SeedPart => 759372056;

	public override void GenerateFresh(string seed)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		WorldGrid worldGrid = Find.WorldGrid;
		float pollution = Find.World.info.pollution;
		if (pollution <= 0f)
		{
			return;
		}
		Perlin perlin = new Perlin(0.10000000149011612, 2.0, 0.5, 6, seed.GetHashCode(), QualityMode.Medium);
		tmpTiles.Clear();
		tmpTileNoise.Clear();
		for (int i = 0; i < worldGrid.TilesCount; i++)
		{
			if (worldGrid[i].biome.allowPollution)
			{
				tmpTiles.Add(i);
				tmpTileNoise.Add(i, perlin.GetValue(worldGrid.GetTileCenter(i)));
			}
		}
		tmpTiles.SortByDescending((int t) => tmpTileNoise[t]);
		int num = Mathf.RoundToInt((float)tmpTiles.Count * pollution);
		for (int num2 = 0; num2 < num; num2++)
		{
			worldGrid[tmpTiles[num2]].pollution = Mathf.Lerp(0.25f, 1f, tmpTileNoise[tmpTiles[num2]]);
		}
		tmpTiles.Clear();
		tmpTileNoise.Clear();
	}
}
