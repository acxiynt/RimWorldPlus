using UnityEngine;
using Verse;

namespace RimWorld;

public static class SkyfallerDrawPosUtility
{
	public static Vector3 DrawPos_Accelerate(Vector3 center, int ticksToImpact, float angle, float speed, CompSkyfallerRandomizeDirection offsetComp = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		ticksToImpact = Mathf.Max(ticksToImpact, 0);
		float dist = Mathf.Pow((float)ticksToImpact, 0.95f) * 1.7f * speed;
		return PosAtDist(center, dist, angle, offsetComp);
	}

	public static Vector3 DrawPos_ConstantSpeed(Vector3 center, int ticksToImpact, float angle, float speed, CompSkyfallerRandomizeDirection offsetComp = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ticksToImpact = Mathf.Max(ticksToImpact, 0);
		float dist = (float)ticksToImpact * speed;
		return PosAtDist(center, dist, angle, offsetComp);
	}

	public static Vector3 DrawPos_Decelerate(Vector3 center, int ticksToImpact, float angle, float speed, CompSkyfallerRandomizeDirection offsetComp = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		ticksToImpact = Mathf.Max(ticksToImpact, 0);
		float dist = (float)(ticksToImpact * ticksToImpact) * 0.00721f * speed;
		return PosAtDist(center, dist, angle, offsetComp);
	}

	private static Vector3 PosAtDist(Vector3 center, float dist, float angle, CompSkyfallerRandomizeDirection offsetComp = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		return center + Vector3Utility.FromAngleFlat(angle - 90f) * dist + (offsetComp?.Offset ?? Vector3.zero);
	}
}
