using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace Verse;

public class PawnRenderNodeWorker
{
	private static readonly Dictionary<Type, PawnRenderNodeWorker> CachedWorkers = new Dictionary<Type, PawnRenderNodeWorker>();

	private static readonly Dictionary<Type, PawnRenderSubWorker> CachedSubWorkers = new Dictionary<Type, PawnRenderSubWorker>();

	private static readonly List<PawnRenderSubWorker> EmptySubWorkers = new List<PawnRenderSubWorker>();

	public virtual bool ShouldListOnGraph(PawnRenderNode node, PawnDrawParms parms)
	{
		return true;
	}

	public virtual bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
	{
		if (!node.Props.rotDrawMode.HasFlag(parms.rotDrawMode))
		{
			return false;
		}
		if (node.Props.visibleFacing != null && !node.Props.visibleFacing.Contains(parms.facing))
		{
			return false;
		}
		if (node.Props.skipFlag != RenderSkipFlagDefOf.None && parms.skipFlags.HasFlag(node.Props.skipFlag))
		{
			return false;
		}
		if (node.hediff?.Part?.visibleHediffRots != null && !node.hediff.Part.visibleHediffRots.Contains(parms.facing))
		{
			return false;
		}
		Rot4 rot = Rot4.Invalid;
		if (node.Props.side != PawnRenderNodeProperties.Side.Center)
		{
			rot = ((node.Props.side == PawnRenderNodeProperties.Side.Left) ? Rot4.East : Rot4.West);
		}
		if (node.hediff?.Part != null && node.hediff.Part.def.IsMirroredPart)
		{
			rot = (node.hediff.Part.flipGraphic ? Rot4.East : Rot4.West);
		}
		if (rot != Rot4.Invalid && node.Props.flipGraphic && rot.IsHorizontal)
		{
			rot = rot.Opposite;
		}
		if (parms.facing == rot)
		{
			return false;
		}
		if (node.Props.linkedBodyPartsGroup != null && !parms.pawn.health.hediffSet.GetNotMissingParts().Any((BodyPartRecord x) => x.groups.NotNullAndContains(node.Props.linkedBodyPartsGroup)))
		{
			return false;
		}
		return node.DebugEnabled;
	}

	public virtual void PostDraw(PawnRenderNode node, PawnDrawParms parms, Mesh mesh, Matrix4x4 matrix)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (node.AnimationWorker != null && node.AnimationWorker.Enabled() && !parms.flags.FlagSet(PawnRenderFlags.Portrait))
		{
			node.AnimationWorker.Draw(parms, matrix);
		}
	}

	public virtual Material GetFinalizedMaterial(PawnRenderNode node, PawnDrawParms parms)
	{
		Material material = GetMaterial(node, parms);
		foreach (PawnRenderSubWorker subWorker in node.Props.SubWorkers)
		{
			subWorker.EditMaterial(node, parms, ref material);
		}
		return material;
	}

	public virtual void AppendDrawRequests(PawnRenderNode node, PawnDrawParms parms, List<PawnGraphicDrawRequest> requests)
	{
		Material finalizedMaterial = GetFinalizedMaterial(node, parms);
		if (!((Object)(object)finalizedMaterial == (Object)null))
		{
			Mesh mesh = node.GetMesh(parms);
			if (!((Object)(object)mesh == (Object)null))
			{
				requests.Add(new PawnGraphicDrawRequest(node, mesh, finalizedMaterial));
			}
		}
	}

	protected virtual Graphic GetGraphic(PawnRenderNode node, PawnDrawParms parms)
	{
		return node.Graphic;
	}

	public virtual MaterialPropertyBlock GetMaterialPropertyBlock(PawnRenderNode node, Material material, PawnDrawParms parms)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (GetGraphic(node, parms) == null)
		{
			return null;
		}
		MaterialPropertyBlock matPropBlock = node.MatPropBlock;
		matPropBlock.SetColor(ShaderPropertyIDs.Color, parms.tint * material.color);
		if (parms.pawn.RaceProps.IsMechanoid && parms.pawn.Faction != null && parms.pawn.Faction != Faction.OfMechanoids)
		{
			PawnRenderUtility.SetMatPropBlockOverlay(matPropBlock, parms.pawn.Faction.MechColor);
		}
		return matPropBlock;
	}

	protected virtual Material GetMaterial(PawnRenderNode node, PawnDrawParms parms)
	{
		Graphic graphic = GetGraphic(node, parms);
		if (graphic == null)
		{
			return null;
		}
		if (node.Props.flipGraphic && parms.facing.IsHorizontal)
		{
			parms.facing = parms.facing.Opposite;
		}
		Material val = graphic.NodeGetMat(parms);
		if ((Object)(object)val != (Object)null && !parms.Portrait && parms.flags.FlagSet(PawnRenderFlags.Invisible))
		{
			val = InvisibilityMatPool.GetInvisibleMat(val);
		}
		return val;
	}

	public virtual void PreDraw(PawnRenderNode node, Material mat, PawnDrawParms parms)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		node.MatPropBlock.SetColor(ShaderPropertyIDs.Color, parms.tint * mat.color);
	}

	public virtual Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		Vector3 anchorOffset = Vector3.zero;
		pivot = PivotFor(node, parms);
		if (node.Props.drawData != null)
		{
			if (node.hediff != null && node.Props.drawData.useHediffAnchor)
			{
				foreach (BodyTypeDef.WoundAnchor item in PawnDrawUtility.FindAnchors(parms.pawn, node.hediff.Part))
				{
					if (PawnDrawUtility.AnchorUsable(parms.pawn, item, parms.facing))
					{
						PawnDrawUtility.CalcAnchorData(parms.pawn, item, parms.facing, out anchorOffset, out var _);
					}
				}
			}
			Vector3 val = node.Props.drawData.OffsetForRot(parms.facing);
			if (node.Props.drawData.scaleOffsetByBodySize && parms.pawn.story != null)
			{
				Vector2 bodyGraphicScale = parms.pawn.story.bodyType.bodyGraphicScale;
				float num = (bodyGraphicScale.x + bodyGraphicScale.y) / 2f;
				val *= num;
			}
			anchorOffset += val;
		}
		DrawData drawData = node.Props.drawData;
		if (drawData != null && !drawData.useHediffAnchor && (node.hediff?.Part?.flipGraphic ?? false))
		{
			anchorOffset.x *= -1f;
		}
		anchorOffset += node.DebugOffset;
		if (node.AnimationWorker != null && node.AnimationWorker.Enabled() && !parms.flags.FlagSet(PawnRenderFlags.Portrait))
		{
			anchorOffset += node.AnimationWorker.OffsetAtTick(node.tree.AnimationTick, parms);
		}
		return anchorOffset;
	}

	protected virtual Vector3 PivotFor(PawnRenderNode node, PawnDrawParms parms)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.zero;
		if (node.Props.drawData != null)
		{
			val -= (node.Props.drawData.PivotForRot(parms.facing) - DrawData.PivotCenter).ToVector3();
		}
		if (node.tree.TryGetAnimationPartForNode(node, out var animationPart))
		{
			val = (animationPart.pivot - DrawData.PivotCenter).ToVector3();
		}
		if (node.debugPivotOffset != DrawData.PivotCenter)
		{
			val.x += node.debugPivotOffset.x - DrawData.PivotCenter.x;
			val.z += node.debugPivotOffset.y - DrawData.PivotCenter.y;
		}
		return val;
	}

	public float AltitudeFor(PawnRenderNode node, PawnDrawParms parms)
	{
		float layer = LayerFor(node, parms);
		foreach (PawnRenderSubWorker subWorker in node.Props.SubWorkers)
		{
			subWorker.TransformLayer(node, parms, ref layer);
		}
		return PawnRenderUtility.AltitudeForLayer(layer);
	}

	public virtual float LayerFor(PawnRenderNode node, PawnDrawParms parms)
	{
		return (node.Props.drawData?.LayerForRot(parms.facing, node.Props.baseLayer) ?? node.Props.baseLayer) + node.debugLayerOffset;
	}

	public virtual Quaternion RotationFor(PawnRenderNode node, PawnDrawParms parms)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		float num = node.DebugAngleOffset;
		if (node.Props.drawData != null)
		{
			num += node.Props.drawData.RotationOffsetForRot(parms.facing);
		}
		if (node.AnimationWorker != null && node.AnimationWorker.Enabled() && !parms.flags.FlagSet(PawnRenderFlags.Portrait))
		{
			num += node.AnimationWorker.AngleAtTick(node.tree.AnimationTick, parms);
		}
		if (node.hediff?.Part?.flipGraphic ?? false)
		{
			num *= -1f;
		}
		return Quaternion.AngleAxis(num, Vector3.up);
	}

	public virtual Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.one;
		val.x *= node.Props.drawSize.x * node.debugScale;
		val.z *= node.Props.drawSize.y * node.debugScale;
		if (node.AnimationWorker != null && node.AnimationWorker.Enabled() && !parms.flags.FlagSet(PawnRenderFlags.Portrait))
		{
			val = val.MultipliedBy(node.AnimationWorker.ScaleAtTick(node.tree.AnimationTick, parms));
		}
		if (node.Props.drawData != null)
		{
			val *= node.Props.drawData.ScaleFor(parms.pawn);
		}
		return val;
	}

	public static PawnRenderNodeWorker GetWorker(Type type)
	{
		if (!CachedWorkers.ContainsKey(type))
		{
			if (!UnityData.IsInMainThread)
			{
				Debug.LogError((object)"Cached pawn render node workers need to be initialized on the main thread.");
			}
			CachedWorkers.Add(type, (PawnRenderNodeWorker)Activator.CreateInstance(type));
		}
		return CachedWorkers[type];
	}

	public static PawnRenderSubWorker GetSubWorker(Type type)
	{
		if (!CachedSubWorkers.ContainsKey(type))
		{
			if (!UnityData.IsInMainThread)
			{
				Debug.LogError((object)"Cached pawn render node sub-workers need to be initialized on the main thread.");
			}
			CachedSubWorkers.Add(type, (PawnRenderSubWorker)Activator.CreateInstance(type));
		}
		return CachedSubWorkers[type];
	}

	public static List<PawnRenderSubWorker> GetSubWorkerList(List<Type> types)
	{
		if (types == null)
		{
			return EmptySubWorkers;
		}
		List<PawnRenderSubWorker> list = new List<PawnRenderSubWorker>();
		foreach (Type type in types)
		{
			list.Add(GetSubWorker(type));
		}
		return list;
	}
}
