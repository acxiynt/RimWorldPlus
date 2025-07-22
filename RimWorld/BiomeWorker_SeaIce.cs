using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace RimWorld;

public class BiomeWorker_SeaIce : BiomeWorker
{
	private ModuleBase cachedSeaIceAllowedNoise;

	private int cachedSeaIceAllowedNoiseForSeed;

	public override float GetScore(Tile tile, int tileID)
	{
		if (!tile.WaterCovered)
		{
			return -100f;
		}
		if (!AllowedAt(tileID))
		{
			return -100f;
		}
		return BiomeWorker_IceSheet.PermaIceScore(tile) - 23f;
	}

	private bool AllowedAt(int tile)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		Vector3 tileCenter = Find.WorldGrid.GetTileCenter(tile);
		Vector3 viewCenter = Find.WorldGrid.viewCenter;
		float num = Vector3.Angle(viewCenter, tileCenter);
		float viewAngle = Find.WorldGrid.viewAngle;
		float num2 = Mathf.Min(7.5f, viewAngle * 0.12f);
		float num3 = Mathf.InverseLerp(viewAngle - num2, viewAngle, num);
		if (num3 <= 0f)
		{
			return true;
		}
		if (cachedSeaIceAllowedNoise == null || cachedSeaIceAllowedNoiseForSeed != Find.World.info.Seed)
		{
			cachedSeaIceAllowedNoise = new Perlin(0.017000000923871994, 2.0, 0.5, 6, Find.World.info.Seed, QualityMode.Medium);
			cachedSeaIceAllowedNoiseForSeed = Find.World.info.Seed;
		}
		float headingFromTo = Find.WorldGrid.GetHeadingFromTo(viewCenter, tileCenter);
		float num4 = (float)cachedSeaIceAllowedNoise.GetValue(headingFromTo, 0.0, 0.0) * 0.5f + 0.5f;
		return num3 <= num4;
	}
}
