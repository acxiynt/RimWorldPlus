using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class PawnHeadOverlays
{
	private Pawn pawn;

	private const float AngerBlinkPeriod = 1.2f;

	private const float AngerBlinkLength = 0.4f;

	private static readonly Material UnhappyMat = MaterialPool.MatFrom("Things/Pawn/Effects/Unhappy");

	private static readonly Material MentalStateImminentMat = MaterialPool.MatFrom("Things/Pawn/Effects/MentalStateImminent");

	public PawnHeadOverlays(Pawn pawn)
	{
		this.pawn = pawn;
	}

	public void RenderStatusOverlays(Vector3 offset, Quaternion quat, Mesh headMesh)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (!pawn.IsColonistPlayerControlled)
		{
			return;
		}
		Vector3 headLoc = pawn.DrawPos + offset + new Vector3(0f, 0f, 0.32f);
		if (pawn.needs.mood == null || pawn.Downed || pawn.HitPoints <= 0)
		{
			return;
		}
		if (pawn.mindState.mentalBreaker.BreakExtremeIsImminent)
		{
			if (Time.time % 1.2f < 0.4f)
			{
				DrawHeadGlow(headLoc, MentalStateImminentMat);
			}
		}
		else if (pawn.mindState.mentalBreaker.BreakExtremeIsApproaching && Time.time % 1.2f < 0.4f)
		{
			DrawHeadGlow(headLoc, UnhappyMat);
		}
	}

	private void DrawHeadGlow(Vector3 headLoc, Material mat)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Graphics.DrawMesh(MeshPool.plane20, headLoc, Quaternion.identity, mat, 0);
	}
}
