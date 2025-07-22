using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class CompProjectileInterceptor : ThingComp
{
	private int lastInterceptTicks = -999999;

	private int nextChargeTick = -1;

	private bool shutDown;

	private StunHandler stunner;

	private Sustainer sustainer;

	public int currentHitPoints = -1;

	public int? maxHitPointsOverride;

	private float lastInterceptAngle;

	private bool drawInterceptCone;

	private bool debugInterceptNonHostileProjectiles;

	private static readonly Material ForceFieldMat = MaterialPool.MatFrom("Other/ForceField", ShaderDatabase.MoteGlow);

	private static readonly Material ForceFieldConeMat = MaterialPool.MatFrom("Other/ForceFieldCone", ShaderDatabase.MoteGlow);

	private static readonly MaterialPropertyBlock MatPropertyBlock = new MaterialPropertyBlock();

	private const float TextureActualRingSizeFactor = 1.1601562f;

	private static readonly Color InactiveColor = new Color(0.2f, 0.2f, 0.2f);

	private const int NumInactiveDots = 7;

	private static Material ShieldDotMat => MaterialPool.MatFrom("Things/Mote/ShieldDownDot", ShaderDatabase.MoteGlow);

	public CompProperties_ProjectileInterceptor Props => (CompProperties_ProjectileInterceptor)props;

	public bool Active
	{
		get
		{
			if (OnCooldown || Charging || stunner.Stunned || shutDown || currentHitPoints == 0 || !parent.Spawned)
			{
				return false;
			}
			if (parent is Pawn p && (p.IsCharging() || p.IsSelfShutdown()))
			{
				return false;
			}
			CompCanBeDormant comp = parent.GetComp<CompCanBeDormant>();
			if (comp != null && !comp.Awake)
			{
				return false;
			}
			return true;
		}
	}

	public bool OnCooldown => Find.TickManager.TicksGame < lastInterceptTicks + Props.cooldownTicks;

	public bool Charging
	{
		get
		{
			if (nextChargeTick >= 0)
			{
				return Find.TickManager.TicksGame > nextChargeTick;
			}
			return false;
		}
	}

	public int ChargeCycleStartTick
	{
		get
		{
			if (nextChargeTick < 0)
			{
				return 0;
			}
			return nextChargeTick;
		}
	}

	public int ChargingTicksLeft
	{
		get
		{
			if (nextChargeTick < 0)
			{
				return 0;
			}
			return Mathf.Max(nextChargeTick + Props.chargeDurationTicks - Find.TickManager.TicksGame, 0);
		}
	}

	public int CooldownTicksLeft
	{
		get
		{
			if (!OnCooldown)
			{
				return 0;
			}
			return Props.cooldownTicks - (Find.TickManager.TicksGame - lastInterceptTicks);
		}
	}

	public bool ReactivatedThisTick => Find.TickManager.TicksGame - lastInterceptTicks == Props.cooldownTicks;

	public bool ShouldDisplayGizmo
	{
		get
		{
			if (parent is Pawn pawn && (pawn.IsColonistPlayerControlled || pawn.RaceProps.IsMechanoid))
			{
				return HitPointsMax > 0;
			}
			return false;
		}
	}

	public int HitPointsMax => maxHitPointsOverride ?? Props.hitPoints;

	public override void PostPostMake()
	{
		base.PostPostMake();
		if (Props.chargeIntervalTicks > 0)
		{
			nextChargeTick = Find.TickManager.TicksGame + Rand.Range(0, Props.chargeIntervalTicks);
		}
		if (HitPointsMax > 0)
		{
			currentHitPoints = HitPointsMax;
		}
		stunner = new StunHandler(parent);
	}

	public override void PostDeSpawn(Map map)
	{
		sustainer?.End();
	}

	public bool CheckIntercept(Projectile projectile, Vector3 lastExactPos, Vector3 newExactPos)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		if (!ModLister.CheckRoyaltyOrBiotech("projectile interceptor"))
		{
			return false;
		}
		Vector3 val = parent.Position.ToVector3Shifted();
		float num = Props.radius + projectile.def.projectile.SpeedTilesPerTick + 0.1f;
		if ((newExactPos.x - val.x) * (newExactPos.x - val.x) + (newExactPos.z - val.z) * (newExactPos.z - val.z) > num * num)
		{
			return false;
		}
		if (!Active)
		{
			return false;
		}
		if (!InterceptsProjectile(Props, projectile))
		{
			return false;
		}
		if (projectile.Launcher == null && !debugInterceptNonHostileProjectiles && !Props.interceptNonHostileProjectiles)
		{
			return false;
		}
		if (parent.Faction != null)
		{
			if (projectile.Launcher != null && projectile.Launcher.Spawned && !projectile.Launcher.HostileTo(parent.Faction))
			{
				return false;
			}
			if (projectile.Launcher != null && !projectile.Launcher.Spawned && !projectile.Launcher.Faction.HostileTo(parent.Faction))
			{
				return false;
			}
		}
		if (!Props.interceptOutgoingProjectiles)
		{
			Vector2 val2 = new Vector2(val.x, val.z) - new Vector2(lastExactPos.x, lastExactPos.z);
			if (((Vector2)(ref val2)).sqrMagnitude <= Props.radius * Props.radius)
			{
				return false;
			}
		}
		if (!GenGeo.IntersectLineCircleOutline(new Vector2(val.x, val.z), Props.radius, new Vector2(lastExactPos.x, lastExactPos.z), new Vector2(newExactPos.x, newExactPos.z)))
		{
			return false;
		}
		lastInterceptAngle = lastExactPos.AngleToFlat(parent.TrueCenter());
		lastInterceptTicks = Find.TickManager.TicksGame;
		drawInterceptCone = true;
		TriggerEffecter(newExactPos.ToIntVec3());
		if (projectile.def.projectile.damageDef == DamageDefOf.EMP && Props.disarmedByEmpForTicks > 0)
		{
			BreakShieldEmp(new DamageInfo(projectile.def.projectile.damageDef, projectile.def.projectile.damageDef.defaultDamage));
		}
		if (currentHitPoints > 0)
		{
			currentHitPoints -= projectile.DamageAmount;
			if (currentHitPoints < 0)
			{
				currentHitPoints = 0;
			}
			if (currentHitPoints == 0)
			{
				nextChargeTick = Find.TickManager.TicksGame;
				BreakShieldHitpoints(new DamageInfo(projectile.def.projectile.damageDef, projectile.def.projectile.damageDef.defaultDamage));
				return true;
			}
		}
		return true;
	}

	public bool CheckBombardmentIntercept(Bombardment bombardment, Bombardment.BombardmentProjectile projectile)
	{
		if (!Active || !Props.interceptAirProjectiles)
		{
			return false;
		}
		if (!projectile.targetCell.InHorDistOf(parent.Position, Props.radius))
		{
			return false;
		}
		if ((bombardment.instigator == null || !bombardment.instigator.HostileTo(parent)) && !debugInterceptNonHostileProjectiles && !Props.interceptNonHostileProjectiles)
		{
			return false;
		}
		lastInterceptTicks = Find.TickManager.TicksGame;
		drawInterceptCone = false;
		TriggerEffecter(projectile.targetCell);
		return true;
	}

	public bool BombardmentCanStartFireAt(Bombardment bombardment, IntVec3 cell)
	{
		if (!Active || !Props.interceptAirProjectiles)
		{
			return true;
		}
		if ((bombardment.instigator == null || !bombardment.instigator.HostileTo(parent)) && !debugInterceptNonHostileProjectiles && !Props.interceptNonHostileProjectiles)
		{
			return true;
		}
		return !cell.InHorDistOf(parent.Position, Props.radius);
	}

	private void TriggerEffecter(IntVec3 pos)
	{
		Effecter effecter = new Effecter(Props.interceptEffect ?? EffecterDefOf.Interceptor_BlockedProjectile);
		effecter.Trigger(new TargetInfo(pos, parent.Map), TargetInfo.Invalid);
		effecter.Cleanup();
	}

	public static bool InterceptsProjectile(CompProperties_ProjectileInterceptor props, Projectile projectile)
	{
		if (props.interceptGroundProjectiles)
		{
			return !projectile.def.projectile.flyOverhead;
		}
		if (props.interceptAirProjectiles)
		{
			return projectile.def.projectile.flyOverhead;
		}
		return false;
	}

	public override void CompTick()
	{
		if (HitPointsMax > 0 && ChargingTicksLeft == 0)
		{
			if (currentHitPoints == 0 && Props.hitPointsRestoreInstantlyAfterCharge)
			{
				currentHitPoints = HitPointsMax;
			}
			if (currentHitPoints >= 0 && currentHitPoints < HitPointsMax && parent.IsHashIntervalTick(Props.rechargeHitPointsIntervalTicks))
			{
				currentHitPoints++;
			}
			if (nextChargeTick > 0)
			{
				nextChargeTick = -1;
			}
		}
		if (ReactivatedThisTick && Props.reactivateEffect != null)
		{
			Props.reactivateEffect.Spawn(parent, parent.MapHeld).Cleanup();
		}
		if (Props.chargeIntervalTicks > 0 && Find.TickManager.TicksGame >= nextChargeTick + Props.chargeDurationTicks)
		{
			nextChargeTick += Props.chargeIntervalTicks;
		}
		stunner.StunHandlerTick();
		if (Props.activeSound.NullOrUndefined())
		{
			return;
		}
		if (Active)
		{
			if (sustainer == null || sustainer.Ended)
			{
				sustainer = Props.activeSound.TrySpawnSustainer(SoundInfo.InMap(parent));
			}
			sustainer.Maintain();
		}
		else if (sustainer != null && !sustainer.Ended)
		{
			sustainer.End();
		}
	}

	public override void PostDrawExtraSelectionOverlays()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		base.PostDrawExtraSelectionOverlays();
		if (!Active)
		{
			for (int i = 0; i < 7; i++)
			{
				Vector3 val = Vector3Utility.RotatedBy(new Vector3(0f, 0f, 1f), (float)i / 7f * 360f) * (Props.radius * 0.966f);
				Vector3 val2 = parent.DrawPos + val;
				Graphics.DrawMesh(MeshPool.plane10, new Vector3(val2.x, AltitudeLayer.MoteOverhead.AltitudeFor(), val2.z), Quaternion.identity, ShieldDotMat, 0);
			}
		}
	}

	public override void Notify_LordDestroyed()
	{
		base.Notify_LordDestroyed();
		shutDown = true;
	}

	public override void PostDraw()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		base.PostDraw();
		Vector3 drawPos = parent.DrawPos;
		drawPos.y = AltitudeLayer.MoteOverhead.AltitudeFor();
		float currentAlpha = GetCurrentAlpha();
		if (currentAlpha > 0f)
		{
			Color val = ((!Active && Find.Selector.IsSelected(parent)) ? InactiveColor : Props.color);
			val.a *= currentAlpha;
			MatPropertyBlock.SetColor(ShaderPropertyIDs.Color, val);
			Matrix4x4 val2 = default(Matrix4x4);
			((Matrix4x4)(ref val2)).SetTRS(drawPos, Quaternion.identity, new Vector3(Props.radius * 2f * 1.1601562f, 1f, Props.radius * 2f * 1.1601562f));
			Graphics.DrawMesh(MeshPool.plane10, val2, ForceFieldMat, 0, (Camera)null, 0, MatPropertyBlock);
		}
		float currentConeAlpha_RecentlyIntercepted = GetCurrentConeAlpha_RecentlyIntercepted();
		if (currentConeAlpha_RecentlyIntercepted > 0f)
		{
			Color color = Props.color;
			color.a *= currentConeAlpha_RecentlyIntercepted;
			MatPropertyBlock.SetColor(ShaderPropertyIDs.Color, color);
			Matrix4x4 val3 = default(Matrix4x4);
			((Matrix4x4)(ref val3)).SetTRS(drawPos, Quaternion.Euler(0f, lastInterceptAngle - 90f, 0f), new Vector3(Props.radius * 2f * 1.1601562f, 1f, Props.radius * 2f * 1.1601562f));
			Graphics.DrawMesh(MeshPool.plane10, val3, ForceFieldConeMat, 0, (Camera)null, 0, MatPropertyBlock);
		}
	}

	private float GetCurrentAlpha()
	{
		return Mathf.Max(Mathf.Max(Mathf.Max(Mathf.Max(GetCurrentAlpha_Idle(), GetCurrentAlpha_Selected()), GetCurrentAlpha_RecentlyIntercepted()), GetCurrentAlpha_RecentlyActivated()), Props.minAlpha);
	}

	private float GetCurrentAlpha_Idle()
	{
		float idlePulseSpeed = Props.idlePulseSpeed;
		float minIdleAlpha = Props.minIdleAlpha;
		if (!Active)
		{
			return 0f;
		}
		if (parent.Faction == Faction.OfPlayer && !debugInterceptNonHostileProjectiles)
		{
			return 0f;
		}
		if (Find.Selector.IsSelected(parent))
		{
			return 0f;
		}
		return Mathf.Lerp(minIdleAlpha, 0.11f, (Mathf.Sin((float)(Gen.HashCombineInt(parent.thingIDNumber, 96804938) % 100) + Time.realtimeSinceStartup * idlePulseSpeed) + 1f) / 2f);
	}

	private float GetCurrentAlpha_Selected()
	{
		float num = Mathf.Max(2f, Props.idlePulseSpeed);
		if ((!Find.Selector.IsSelected(parent) && !Props.drawWithNoSelection) || !Active)
		{
			return 0f;
		}
		return Mathf.Lerp(0.2f, 0.62f, (Mathf.Sin((float)(Gen.HashCombineInt(parent.thingIDNumber, 35990913) % 100) + Time.realtimeSinceStartup * num) + 1f) / 2f);
	}

	private float GetCurrentAlpha_RecentlyIntercepted()
	{
		int num = Find.TickManager.TicksGame - lastInterceptTicks;
		return Mathf.Clamp01(1f - (float)num / 40f) * 0.09f;
	}

	private float GetCurrentAlpha_RecentlyActivated()
	{
		if (!Active)
		{
			return 0f;
		}
		int num = Find.TickManager.TicksGame - (lastInterceptTicks + Props.cooldownTicks);
		return Mathf.Clamp01(1f - (float)num / 50f) * 0.09f;
	}

	private float GetCurrentConeAlpha_RecentlyIntercepted()
	{
		if (!drawInterceptCone)
		{
			return 0f;
		}
		int num = Find.TickManager.TicksGame - lastInterceptTicks;
		return Mathf.Clamp01(1f - (float)num / 40f) * 0.82f;
	}

	public override IEnumerable<Gizmo> CompGetGizmosExtra()
	{
		if (ShouldDisplayGizmo)
		{
			Gizmo_ProjectileInterceptorHitPoints gizmo_ProjectileInterceptorHitPoints = new Gizmo_ProjectileInterceptorHitPoints();
			gizmo_ProjectileInterceptorHitPoints.interceptor = this;
			yield return gizmo_ProjectileInterceptorHitPoints;
		}
		if (!DebugSettings.ShowDevGizmos)
		{
			yield break;
		}
		if (OnCooldown)
		{
			yield return new Command_Action
			{
				defaultLabel = "DEV: Reset cooldown",
				action = delegate
				{
					lastInterceptTicks = Find.TickManager.TicksGame - Props.cooldownTicks;
				}
			};
		}
		yield return new Command_Toggle
		{
			defaultLabel = "DEV: Intercept non-hostile",
			isActive = () => debugInterceptNonHostileProjectiles,
			toggleAction = delegate
			{
				debugInterceptNonHostileProjectiles = !debugInterceptNonHostileProjectiles;
			}
		};
	}

	public override string CompInspectStringExtra()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (Props.interceptGroundProjectiles || Props.interceptAirProjectiles)
		{
			string text = ((!Props.interceptGroundProjectiles) ? ((string)"InterceptsProjectiles_AerialProjectiles".Translate()) : ((string)"InterceptsProjectiles_GroundProjectiles".Translate()));
			if (Props.cooldownTicks > 0)
			{
				stringBuilder.AppendTagged("InterceptsProjectilesEvery".Translate(text, Props.cooldownTicks.ToStringTicksToPeriod()));
			}
			else
			{
				stringBuilder.Append("InterceptsProjectiles".Translate(text));
			}
		}
		if (OnCooldown)
		{
			if (stringBuilder.Length != 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendTagged("CooldownTime".Translate() + ": " + CooldownTicksLeft.ToStringTicksToPeriod());
		}
		if (stunner.Stunned)
		{
			if (stringBuilder.Length != 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendTagged("DisarmedTime".Translate() + ": " + stunner.StunTicksLeft.ToStringTicksToPeriod());
		}
		if (shutDown)
		{
			if (stringBuilder.Length != 0)
			{
				stringBuilder.AppendLine();
			}
			stringBuilder.Append("ShutDown".Translate());
		}
		else if (Props.chargeIntervalTicks > 0)
		{
			if (stringBuilder.Length != 0)
			{
				stringBuilder.AppendLine();
			}
			if (Charging)
			{
				stringBuilder.AppendTagged("ChargingTime".Translate() + ": " + ChargingTicksLeft.ToStringTicksToPeriod());
			}
			else
			{
				stringBuilder.AppendTagged("ChargingNext".Translate((ChargeCycleStartTick - Find.TickManager.TicksGame).ToStringTicksToPeriod(), Props.chargeDurationTicks.ToStringTicksToPeriod(), Props.chargeIntervalTicks.ToStringTicksToPeriod()));
			}
		}
		return stringBuilder.ToString();
	}

	public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
	{
		base.PostPreApplyDamage(ref dinfo, out absorbed);
		if (dinfo.Def == DamageDefOf.EMP && Props.disarmedByEmpForTicks > 0)
		{
			BreakShieldEmp(dinfo);
		}
	}

	private void BreakShieldEmp(DamageInfo dinfo)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		float fTheta;
		Vector3 center;
		if (Active)
		{
			EffecterDefOf.Shield_Break.SpawnAttached(parent, parent.MapHeld, Props.radius);
			int num = Mathf.CeilToInt(Props.radius * 2f);
			fTheta = (float)Math.PI * 2f / (float)num;
			center = parent.TrueCenter();
			for (int i = 0; i < num; i++)
			{
				FleckMaker.ConnectingLine(PosAtIndex(i), PosAtIndex((i + 1) % num), FleckDefOf.LineEMP, parent.Map, 1.5f);
			}
		}
		dinfo.SetAmount((float)Props.disarmedByEmpForTicks / 30f);
		stunner.Notify_DamageApplied(dinfo);
		Vector3 PosAtIndex(int index)
		{
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(Props.radius * Mathf.Cos(fTheta * (float)index) + center.x, 0f, Props.radius * Mathf.Sin(fTheta * (float)index) + center.z);
		}
	}

	private void BreakShieldHitpoints(DamageInfo dinfo)
	{
		EffecterDefOf.Shield_Break.SpawnAttached(parent, parent.MapHeld, Props.radius);
		stunner.Notify_DamageApplied(dinfo);
	}

	public override void PostExposeData()
	{
		base.PostExposeData();
		Scribe_Values.Look(ref lastInterceptTicks, "lastInterceptTicks", -999999);
		Scribe_Values.Look(ref shutDown, "shutDown", defaultValue: false);
		Scribe_Values.Look(ref nextChargeTick, "nextChargeTick", -1);
		Scribe_Deep.Look(ref stunner, "stunner", parent);
		Scribe_Values.Look(ref currentHitPoints, "currentHitPoints", -1);
		Scribe_Values.Look(ref maxHitPointsOverride, "maxHitPointsOverride");
		if (Scribe.mode == LoadSaveMode.PostLoadInit)
		{
			if (Props.chargeIntervalTicks > 0 && nextChargeTick <= 0)
			{
				nextChargeTick = Find.TickManager.TicksGame + Rand.Range(0, Props.chargeIntervalTicks);
			}
			if (stunner == null)
			{
				stunner = new StunHandler(parent);
			}
		}
	}
}
