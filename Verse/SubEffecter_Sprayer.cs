using RimWorld;
using UnityEngine;

namespace Verse;

public abstract class SubEffecter_Sprayer : SubEffecter
{
	private Mote mote;

	private Vector3? lastOffset;

	public SubEffecter_Sprayer(SubEffecterDef def, Effecter parent)
		: base(def, parent)
	{
	}

	public Vector3 GetAttachedSpawnLoc(TargetInfo tgt)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 centerVector = tgt.CenterVector3;
		if (def.attachPoint != AttachPointType.RootNone && tgt.HasThing && tgt.Thing.TryGetComp(out CompAttachPoints comp))
		{
			return comp.points.GetWorldPos(def.attachPoint);
		}
		return centerVector;
	}

	protected void MakeMote(TargetInfo A, TargetInfo B, int overrideSpawnTick = -1)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_043e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Unknown result type (might be due to invalid IL or missing references)
		//IL_059f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_060c: Unknown result type (might be due to invalid IL or missing references)
		//IL_060d: Unknown result type (might be due to invalid IL or missing references)
		//IL_060f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0627: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0685: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0701: Unknown result type (might be due to invalid IL or missing references)
		//IL_070c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0743: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_0748: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.zero;
		Vector3 val3;
		switch (base.EffectiveSpawnLocType)
		{
		case MoteSpawnLocType.OnSource:
			val = GetAttachedSpawnLoc(A);
			break;
		case MoteSpawnLocType.OnTarget:
			val = GetAttachedSpawnLoc(B);
			break;
		case MoteSpawnLocType.BetweenPositions:
		{
			Vector3 val7 = (A.HasThing ? A.Thing.DrawPos : A.Cell.ToVector3Shifted());
			Vector3 val8 = (B.HasThing ? B.Thing.DrawPos : B.Cell.ToVector3Shifted());
			val = ((A.HasThing && !A.Thing.Spawned) ? val8 : ((!B.HasThing || B.Thing.Spawned) ? (val7 * def.positionLerpFactor + val8 * (1f - def.positionLerpFactor)) : val7));
			break;
		}
		case MoteSpawnLocType.RandomCellOnTarget:
			val = ((!B.HasThing) ? CellRect.CenteredOn(B.Cell, 0) : B.Thing.OccupiedRect()).RandomCell.ToVector3Shifted();
			break;
		case MoteSpawnLocType.RandomDrawPosOnTarget:
			if (B.Thing.DrawSize != Vector2.one && B.Thing.DrawSize != Vector2.zero)
			{
				Vector2 val4 = B.Thing.DrawSize.RotatedBy(B.Thing.Rotation);
				Vector3 val5 = default(Vector3);
				((Vector3)(ref val5))._002Ector(val4.x * Rand.Value, 0f, val4.y * Rand.Value);
				val = B.CenterVector3 + val5 - new Vector3(val4.x / 2f, 0f, val4.y / 2f);
			}
			else
			{
				Vector3 val6 = default(Vector3);
				((Vector3)(ref val6))._002Ector(Rand.Value, 0f, Rand.Value);
				val = B.CenterVector3 + val6 - new Vector3(0.5f, 0f, 0.5f);
			}
			break;
		case MoteSpawnLocType.BetweenTouchingCells:
		{
			Vector3 val2 = A.Cell.ToVector3Shifted();
			val3 = (B.Cell - A.Cell).ToVector3();
			val = val2 + ((Vector3)(ref val3)).normalized * 0.5f;
			break;
		}
		}
		if (parent != null)
		{
			Rand.PushState(parent.GetHashCode());
			if (A.CenterVector3 != B.CenterVector3)
			{
				Vector3 val9 = val;
				val3 = B.CenterVector3 - A.CenterVector3;
				val = val9 + ((Vector3)(ref val3)).normalized * parent.def.offsetTowardsTarget.RandomInRange;
			}
			Vector3 val10 = Gen.RandomHorizontalVector(parent.def.positionRadius);
			Rand.PopState();
			if (base.EffectiveDimensions.HasValue)
			{
				val10 += Gen.Random2DVector(base.EffectiveDimensions.Value);
			}
			val += val10 + parent.offset;
		}
		Map map = A.Map ?? B.Map;
		float num = (def.absoluteAngle ? 0f : ((def.useTargetAInitialRotation && A.HasThing) ? A.Thing.Rotation.AsAngle : ((!def.useTargetBInitialRotation || !B.HasThing) ? (B.Cell - A.Cell).AngleFlat : B.Thing.Rotation.AsAngle)));
		float num2 = ((parent != null) ? parent.scale : 1f);
		if (map == null)
		{
			return;
		}
		int randomInRange = def.burstCount.RandomInRange;
		for (int i = 0; i < randomInRange; i++)
		{
			Vector3 val11 = base.EffectiveOffset;
			if (def.useTargetAInitialRotation && A.HasThing)
			{
				val11 = val11.RotatedBy(A.Thing.Rotation);
			}
			else if (def.useTargetBInitialRotation && B.HasThing)
			{
				val11 = val11.RotatedBy(B.Thing.Rotation);
			}
			else if (def.useTargetABodyAngle && A.HasThing && A.Thing is Pawn pawn)
			{
				val11 = val11.RotatedBy(pawn.Drawer.renderer.BodyAngle(PawnRenderFlags.None));
			}
			else if (def.useTargetBBodyAngle && B.HasThing && B.Thing is Pawn pawn2)
			{
				val11 = val11.RotatedBy(pawn2.Drawer.renderer.BodyAngle(PawnRenderFlags.None));
			}
			if (!def.perRotationOffsets.NullOrEmpty())
			{
				val11 += def.perRotationOffsets[((base.EffectiveSpawnLocType == MoteSpawnLocType.OnSource) ? A.Thing.Rotation : B.Thing.Rotation).AsInt];
			}
			for (int j = 0; j < 5; j++)
			{
				val11 = val11 * num2 + Rand.InsideAnnulusVector3(def.positionRadiusMin, def.positionRadius) * num2;
				if (def.avoidLastPositionRadius < float.Epsilon || !lastOffset.HasValue || (val11 - lastOffset.Value).MagnitudeHorizontal() > def.avoidLastPositionRadius)
				{
					break;
				}
			}
			lastOffset = val11;
			Vector3 val12 = val + val11;
			if (def.rotateTowardsTargetCenter)
			{
				num = (val12 - B.CenterVector3).AngleFlat();
			}
			if (def.moteDef != null && val.ShouldSpawnMotesAt(map, def.moteDef.drawOffscreen))
			{
				mote = (Mote)ThingMaker.MakeThing(def.moteDef);
				GenSpawn.Spawn(mote, val.ToIntVec3(), map);
				mote.Scale = def.scale.RandomInRange * num2;
				mote.exactPosition = val12;
				mote.rotationRate = def.rotationRate.RandomInRange;
				mote.exactRotation = def.rotation.RandomInRange + num;
				mote.instanceColor = base.EffectiveColor;
				mote.yOffset = val11.y;
				mote.curvedScale = def.moteDef.mote.scalers?.ScaleAtTime(0f) ?? Vector3.one;
				if (overrideSpawnTick != -1)
				{
					mote.ForceSpawnTick(overrideSpawnTick);
				}
				if (mote is MoteThrown moteThrown)
				{
					moteThrown.airTimeLeft = def.airTime.RandomInRange;
					moteThrown.SetVelocity(def.angle.RandomInRange + num, def.speed.RandomInRange);
				}
				TryAttachMote(A, B, val11);
				mote.Maintain();
			}
			else if (def.fleckDef != null && val12.ShouldSpawnMotesAt(map, def.fleckDef.drawOffscreen))
			{
				float velocityAngle = (def.fleckUsesAngleForVelocity ? (def.angle.RandomInRange + num) : 0f);
				FleckAttachLink link = FleckAttachLink.Invalid;
				if (def.fleckDef.useAttachLink && base.EffectiveSpawnLocType == MoteSpawnLocType.OnSource && A.IsValid)
				{
					link = new FleckAttachLink(A);
				}
				if (def.fleckDef.useAttachLink && base.EffectiveSpawnLocType == MoteSpawnLocType.OnTarget && B.IsValid)
				{
					link = new FleckAttachLink(B);
				}
				map.flecks.CreateFleck(new FleckCreationData
				{
					def = def.fleckDef,
					scale = def.scale.RandomInRange * num2,
					spawnPosition = val12,
					rotationRate = def.rotationRate.RandomInRange,
					rotation = def.rotation.RandomInRange + num,
					instanceColor = base.EffectiveColor,
					velocitySpeed = def.speed.RandomInRange,
					velocityAngle = velocityAngle,
					ageTicksOverride = overrideSpawnTick,
					orbitSpeed = (def.orbitOrigin ? def.orbitSpeed.RandomInRange : 0f),
					orbitSnapStrength = def.orbitSnapStrength,
					link = link
				});
			}
		}
	}

	private void TryAttachMote(TargetInfo A, TargetInfo B, Vector3 posOffset)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		if (!def.attachToSpawnThing)
		{
			return;
		}
		if (mote is MoteAttached moteAttached)
		{
			bool updateOffsetToMatchTargetRotation = def.moteDef.mote.updateOffsetToMatchTargetRotation;
			Vector3 offset = (updateOffsetToMatchTargetRotation ? base.EffectiveOffset : posOffset);
			if (base.EffectiveSpawnLocType == MoteSpawnLocType.OnSource && A.HasThing)
			{
				moteAttached.Attach(A, offset, updateOffsetToMatchTargetRotation);
			}
			else if (base.EffectiveSpawnLocType == MoteSpawnLocType.OnTarget && B.HasThing)
			{
				moteAttached.Attach(B, offset, updateOffsetToMatchTargetRotation);
			}
		}
		if (mote is MoteDualAttached moteDualAttached)
		{
			bool updateOffsetToMatchTargetRotation2 = def.moteDef.mote.updateOffsetToMatchTargetRotation;
			Vector3 val = (updateOffsetToMatchTargetRotation2 ? base.EffectiveOffset : posOffset);
			if (A.HasThing && B.HasThing)
			{
				moteDualAttached.Attach(A, B, val, Vector3.zero);
			}
			else if (base.EffectiveSpawnLocType == MoteSpawnLocType.OnSource && A.HasThing)
			{
				moteDualAttached.Attach(A, val, updateOffsetToMatchTargetRotation2);
			}
			else if (base.EffectiveSpawnLocType == MoteSpawnLocType.OnTarget && B.HasThing)
			{
				moteDualAttached.Attach(B, val, updateOffsetToMatchTargetRotation2);
			}
		}
	}

	public override void SubTrigger(TargetInfo A, TargetInfo B, int overrideSpawnTick = -1, bool force = false)
	{
		if (def.makeMoteOnSubtrigger)
		{
			MakeMote(A, B);
		}
	}

	public override void SubCleanup()
	{
		if (def.destroyMoteOnCleanup)
		{
			mote?.Destroy();
		}
		base.SubCleanup();
	}

	public override void SubEffectTick(TargetInfo A, TargetInfo B)
	{
		base.SubEffectTick(A, B);
		if (mote != null && mote.def.mote.needsMaintenance)
		{
			mote.Maintain();
		}
	}
}
