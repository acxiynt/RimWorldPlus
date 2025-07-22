using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld;

public class PawnWoundDrawer : PawnOverlayDrawer
{
	public BodyPartRecord debugDrawPart;

	public bool debugDrawAllParts;

	private const int MaxVisibleHediffsNonHuman = 5;

	private static Color OldWoundColor = new Color(0.3f, 0.3f, 0f, 1f);

	private const float WoundScale = 0.25f;

	private static Vector3 VecOneFlipped = new Vector3(-1f, 1f, 1f);

	private readonly List<BodyTypeDef.WoundAnchor> tmpAnchors = new List<BodyTypeDef.WoundAnchor>();

	public PawnWoundDrawer(Pawn pawn)
		: base(pawn)
	{
	}

	protected override void WriteCache(CacheKey key, PawnDrawParms parms, List<DrawCall> writeTarget)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_059d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Expected O, but got Unknown
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_047f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0482: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_049f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0542: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_056a: Unknown result type (might be due to invalid IL or missing references)
		Rot4 pawnRot = key.pawnRot;
		Mesh bodyMesh = key.bodyMesh;
		OverlayLayer layer = key.layer;
		DebugDraw(writeTarget, parms.matrix, bodyMesh, pawnRot, layer);
		Graphic graphic = ((layer == OverlayLayer.Body) ? pawn.Drawer.renderer.BodyGraphic : pawn.Drawer.renderer.HeadGraphic);
		List<Hediff_MissingPart> missingPartsCommonAncestors = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
		for (int i = 0; i < pawn.health.hediffSet.hediffs.Count && (pawn.RaceProps.Humanlike || writeTarget.Count < 5); i++)
		{
			Hediff hediff = pawn.health.hediffSet.hediffs[i];
			if (hediff.Part == null || !hediff.Visible || !hediff.def.displayWound || (hediff is Hediff_MissingPart && !missingPartsCommonAncestors.Contains(hediff)))
			{
				continue;
			}
			if (hediff is Hediff_AddedPart && pawn.apparel != null)
			{
				bool flag = false;
				foreach (Apparel item in pawn.apparel.WornApparel)
				{
					if (item.def.apparel.blocksAddedPartWoundGraphics && item.def.apparel.CoversBodyPart(hediff.Part))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
			}
			float range = 0f;
			string text = null;
			Vector3 anchorOffset = Vector3.zero;
			if (pawn.story?.bodyType?.woundAnchors != null)
			{
				tmpAnchors.AddRange(PawnDrawUtility.FindAnchors(pawn, hediff.Part));
				if (tmpAnchors.Count > 0)
				{
					for (int num = tmpAnchors.Count - 1; num >= 0; num--)
					{
						if (tmpAnchors[num].layer != layer || !PawnDrawUtility.AnchorUsable(pawn, tmpAnchors[num], pawnRot))
						{
							tmpAnchors.RemoveAt(num);
						}
					}
					if (tmpAnchors.Count == 0)
					{
						continue;
					}
					BodyTypeDef.WoundAnchor woundAnchor = tmpAnchors.RandomElement();
					PawnDrawUtility.CalcAnchorData(pawn, woundAnchor, pawnRot, out anchorOffset, out range);
					range = hediff.def.woundAnchorRange ?? range;
					text = woundAnchor.tag;
				}
				tmpAnchors.Clear();
			}
			else
			{
				GetDefaultAnchor(bodyMesh, out anchorOffset, out range);
			}
			Rand.PushState(pawn.thingIDNumber * i * pawnRot.AsInt);
			try
			{
				FleshTypeDef.ResolvedWound resolvedWound = pawn.RaceProps.FleshType.ChooseWoundOverlay(hediff);
				if (resolvedWound == null || (!resolvedWound.wound.displayPermanent && hediff is Hediff_Injury hd && hd.IsPermanent()))
				{
					continue;
				}
				Vector3 val = resolvedWound.wound.drawOffsetSouth;
				if (pawnRot.IsHorizontal)
				{
					val = resolvedWound.wound.drawOffsetEastWest.ScaledBy((pawnRot == Rot4.East) ? Vector3.one : VecOneFlipped);
				}
				Vector3 val2 = anchorOffset + val;
				Vector4? maskTexOffset = null;
				Vector4? maskTexScale = null;
				bool flip;
				Material val3 = resolvedWound.GetMaterial(pawnRot, out flip);
				if (resolvedWound.wound.flipOnWoundAnchorTag != null && resolvedWound.wound.flipOnWoundAnchorTag == text && resolvedWound.wound.flipOnRotation == pawnRot)
				{
					flip = !flip;
				}
				Mesh val4 = MeshPool.GridPlane(Vector2.one * 0.25f, flip);
				if (!pawn.def.race.Humanlike)
				{
					Vector3 val5 = Rand.InsideUnitCircleVec3 * range;
					if (flip)
					{
						val5.x *= -1f;
					}
					val2 += val5;
					val3 = MaterialPool.MatFrom(new MaterialRequest
					{
						maskTex = (Texture2D)graphic.MatAt(pawnRot).mainTexture,
						mainTex = val3.mainTexture,
						color = val3.color,
						shader = val3.shader
					});
					Vector3 drawPos = pawn.DrawPos;
					Vector3 val6 = drawPos + val2;
					Bounds bounds = val4.bounds;
					Vector3 val7 = val6 - ((Bounds)(ref bounds)).extents;
					bounds = bodyMesh.bounds;
					Vector3 val8 = drawPos - ((Bounds)(ref bounds)).extents;
					bounds = bodyMesh.bounds;
					Vector3 size = ((Bounds)(ref bounds)).size;
					bounds = val4.bounds;
					Vector3 size2 = ((Bounds)(ref bounds)).size;
					bool flag2 = (graphic.EastFlipped && pawnRot == Rot4.East) || (graphic.WestFlipped && pawnRot == Rot4.West);
					maskTexScale = new Vector4(size2.x / size.x, size2.z / size.z);
					maskTexOffset = new Vector4((val7.x - val8.x) / size.x, (val7.z - val8.z) / size.z, (float)(flag2 ? 1 : 0));
				}
				Matrix4x4 matrix = Matrix4x4.TRS(val2, Quaternion.AngleAxis((float)Rand.Range(0, 360), Vector3.up), Vector3.one * resolvedWound.wound.scale);
				Color? colorOverride = null;
				if (resolvedWound.wound.tintWithSkinColor && pawn.story != null)
				{
					colorOverride = pawn.story.SkinColor;
				}
				if (pawn.Corpse != null && pawn.Corpse.GetRotStage() == RotStage.Rotting)
				{
					colorOverride = OldWoundColor;
				}
				if (pawn.IsMutant && pawn.mutant.Def.woundColor.HasValue)
				{
					colorOverride = pawn.mutant.Def.woundColor;
				}
				writeTarget.Add(new DrawCall
				{
					overlayMat = val3,
					matrix = matrix,
					overlayMesh = val4,
					displayOverApparel = resolvedWound.wound.displayOverApparel,
					colorOverride = colorOverride,
					maskTexScale = maskTexScale,
					maskTexOffset = maskTexOffset
				});
			}
			finally
			{
				Rand.PopState();
			}
		}
	}

	private void GetDefaultAnchor(Mesh bodyMesh, out Vector3 anchorOffset, out float range)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = bodyMesh.bounds;
		anchorOffset = ((Bounds)(ref bounds)).center;
		bounds = bodyMesh.bounds;
		float x = ((Bounds)(ref bounds)).extents.x;
		bounds = bodyMesh.bounds;
		range = Mathf.Min(x, ((Bounds)(ref bounds)).extents.z) / 2f;
	}

	private void DebugDraw(List<DrawCall> writeTarget, Matrix4x4 matrix, Mesh bodyMesh, Rot4 pawnRot, OverlayLayer layer)
	{
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		if (debugDrawPart != null)
		{
			bool flag = false;
			foreach (BodyTypeDef.WoundAnchor item in PawnDrawUtility.FindAnchors(pawn, debugDrawPart))
			{
				if (PawnDrawUtility.AnchorUsable(pawn, item, pawnRot))
				{
					flag = true;
					Material overlayMat = MaterialPool.MatFrom(new MaterialRequest((Texture)(object)BaseContent.WhiteTex, ShaderDatabase.SolidColor, item.debugColor));
					PawnDrawUtility.CalcAnchorData(pawn, item, pawnRot, out var anchorOffset, out var range);
					matrix *= Matrix4x4.Translate(anchorOffset) * Matrix4x4.Scale(Vector3.one * (range * pawn.story.bodyType.woundScale));
					if (item.layer == layer)
					{
						writeTarget.Add(new DrawCall
						{
							overlayMat = overlayMat,
							matrix = matrix,
							overlayMesh = MeshPool.circle,
							displayOverApparel = true,
							maskTexOffset = Vector4.zero,
							maskTexScale = Vector4.one
						});
					}
				}
			}
			if (!flag)
			{
				GetDefaultAnchor(bodyMesh, out var anchorOffset2, out var range2);
				matrix *= Matrix4x4.Translate(anchorOffset2) * Matrix4x4.Scale(Vector3.one * range2);
				writeTarget.Add(new DrawCall
				{
					overlayMat = BaseContent.BadMat,
					matrix = matrix,
					overlayMesh = MeshPool.circle,
					displayOverApparel = true,
					maskTexOffset = Vector4.zero,
					maskTexScale = Vector4.one
				});
			}
		}
		else
		{
			if (!debugDrawAllParts)
			{
				return;
			}
			if (pawn.story?.bodyType?.woundAnchors != null)
			{
				foreach (BodyTypeDef.WoundAnchor woundAnchor in pawn.story.bodyType.woundAnchors)
				{
					if (PawnDrawUtility.AnchorUsable(pawn, woundAnchor, pawnRot))
					{
						Material overlayMat2 = MaterialPool.MatFrom(new MaterialRequest((Texture)(object)BaseContent.WhiteTex, ShaderDatabase.SolidColor, woundAnchor.debugColor));
						PawnDrawUtility.CalcAnchorData(pawn, woundAnchor, pawnRot, out var anchorOffset3, out var range3);
						matrix *= Matrix4x4.Translate(anchorOffset3) * Matrix4x4.Scale(Vector3.one * range3);
						if (woundAnchor.layer == layer)
						{
							writeTarget.Add(new DrawCall
							{
								overlayMat = overlayMat2,
								matrix = matrix,
								overlayMesh = MeshPool.circle,
								displayOverApparel = true,
								maskTexOffset = Vector4.zero,
								maskTexScale = Vector4.one
							});
						}
					}
				}
				return;
			}
			GetDefaultAnchor(bodyMesh, out var anchorOffset4, out var range4);
			matrix *= Matrix4x4.Translate(anchorOffset4) * Matrix4x4.Scale(Vector3.one * range4);
			writeTarget.Add(new DrawCall
			{
				overlayMat = BaseContent.BadMat,
				matrix = matrix,
				overlayMesh = MeshPool.circle,
				displayOverApparel = true,
				maskTexOffset = Vector4.zero,
				maskTexScale = Vector4.one
			});
		}
	}
}
