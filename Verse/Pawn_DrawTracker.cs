using RimWorld;
using UnityEngine;

namespace Verse;

public class Pawn_DrawTracker
{
	private Pawn pawn;

	public PawnTweener tweener;

	private JitterHandler jitterer;

	public PawnLeaner leaner;

	public PawnRenderer renderer;

	public PawnUIOverlay ui;

	private PawnFootprintMaker footprintMaker;

	private PawnBreathMoteMaker breathMoteMaker;

	private const float MeleeJitterDistance = 0.5f;

	public Vector3 DrawPos
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			tweener.PreDrawPosCalculation();
			return (tweener.TweenedPos + jitterer.CurrentOffset + leaner.LeanOffset + OffsetForcedByJob()).WithY(pawn.def.Altitude + SeededYOffset);
		}
	}

	public float SeededYOffset { get; }

	public Pawn_DrawTracker(Pawn pawn)
	{
		this.pawn = pawn;
		tweener = new PawnTweener(pawn);
		jitterer = new JitterHandler();
		leaner = new PawnLeaner(pawn);
		renderer = new PawnRenderer(pawn);
		ui = new PawnUIOverlay(pawn);
		footprintMaker = new PawnFootprintMaker(pawn);
		breathMoteMaker = new PawnBreathMoteMaker(pawn);
		SeededYOffset = Rand.RangeSeeded(-1f / 26f, 1f / 26f, pawn.thingIDNumber);
	}

	public void ProcessPostTickVisuals(int ticksPassed)
	{
		if (pawn.Spawned)
		{
			jitterer.ProcessPostTickVisuals(ticksPassed);
			footprintMaker.ProcessPostTickVisuals(ticksPassed);
			breathMoteMaker.ProcessPostTickVisuals(ticksPassed);
			leaner.ProcessPostTickVisuals(ticksPassed);
			renderer.ProcessPostTickVisuals(ticksPassed);
		}
	}

	public void DrawShadowAt(Vector3 loc)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		using (new ProfilerBlock("Draw Shadow At()"))
		{
			renderer.RenderShadowOnlyAt(loc);
		}
	}

	private Vector3 OffsetForcedByJob()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (pawn.jobs?.curDriver != null)
		{
			return pawn.jobs.curDriver.ForcedBodyOffset;
		}
		return Vector3.zero;
	}

	public void Notify_Spawned()
	{
		tweener.ResetTweenedPosToRoot();
	}

	public void Notify_WarmingCastAlongLine(ShootLine newShootLine, IntVec3 ShootPosition)
	{
		leaner.Notify_WarmingCastAlongLine(newShootLine, ShootPosition);
	}

	public void Notify_DamageApplied(DamageInfo dinfo)
	{
		if (!pawn.Destroyed && pawn.Spawned)
		{
			jitterer.Notify_DamageApplied(dinfo);
			renderer.Notify_DamageApplied(dinfo);
		}
	}

	public void Notify_DamageDeflected(DamageInfo dinfo)
	{
		if (!pawn.Destroyed)
		{
			jitterer.Notify_DamageDeflected(dinfo);
		}
	}

	public void Notify_MeleeAttackOn(Thing Target)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (Target.Position != pawn.Position)
		{
			jitterer.AddOffset(0.5f, (Target.Position - pawn.Position).AngleFlat);
		}
		else if (Target.DrawPos != pawn.DrawPos)
		{
			jitterer.AddOffset(0.25f, (Target.DrawPos - pawn.DrawPos).AngleFlat());
		}
	}

	public void Notify_DebugAffected()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 10; i++)
		{
			FleckMaker.ThrowAirPuffUp(pawn.DrawPosHeld.Value, pawn.MapHeld);
		}
		jitterer.AddOffset(0.05f, Rand.Range(0, 360));
	}
}
