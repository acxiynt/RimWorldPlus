using RimWorld;
using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker_HediffEye : PawnRenderNodeWorker_Eye
{
	public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = base.OffsetFor(node, parms, out pivot);
		if (TryGetWoundAnchor(node.hediff.Part?.woundAnchorTag, parms, out var anchor))
		{
			PawnDrawUtility.CalcAnchorData(parms.pawn, anchor, parms.facing, out var anchorOffset, out var _);
			val += anchorOffset;
		}
		return val;
	}
}
