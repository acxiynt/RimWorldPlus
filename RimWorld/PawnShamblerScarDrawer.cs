using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class PawnShamblerScarDrawer : PawnOverlayDrawer
{
	private const string ScarTexturePath = "Things/Pawn/Overlays/ShamblerScars/ShamblerScarOverlay";

	public PawnShamblerScarDrawer(Pawn pawn)
		: base(pawn)
	{
	}

	protected override void WriteCache(CacheKey key, PawnDrawParms parms, List<DrawCall> writeTarget)
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		Rot4 pawnRot = key.pawnRot;
		Mesh bodyMesh = key.bodyMesh;
		OverlayLayer layer = key.layer;
		Graphic graphic = ((layer == OverlayLayer.Body) ? pawn.Drawer.renderer.BodyGraphic : pawn.Drawer.renderer.HeadGraphic);
		if (graphic == null)
		{
			return;
		}
		Rand.PushState(pawn.thingIDNumber * (int)(layer + 1));
		try
		{
			Mesh val = (((graphic.EastFlipped && pawnRot == Rot4.East) || (graphic.WestFlipped && pawnRot == Rot4.West)) ? MeshPool.GridPlaneFlip(Vector2.one) : MeshPool.GridPlane(Vector2.one));
			Bounds bounds = bodyMesh.bounds;
			Vector3 size = ((Bounds)(ref bounds)).size;
			float magnitude = ((Vector3)(ref size)).magnitude;
			bounds = val.bounds;
			Vector3 val2 = ((Bounds)(ref bounds)).size * magnitude;
			Vector4 value = default(Vector4);
			((Vector4)(ref value))._002Ector(val2.x / size.x, val2.z / size.z);
			Material val3 = MaterialPool.MatFrom("Things/Pawn/Overlays/ShamblerScars/ShamblerScarOverlay", ShaderDatabase.Wound, Color.white);
			val3 = MaterialPool.MatFrom(new MaterialRequest
			{
				maskTex = (Texture2D)graphic.MatAt(pawnRot).mainTexture,
				mainTex = val3.mainTexture,
				color = val3.color,
				shader = val3.shader
			});
			Vector3 val4 = Rand.InsideUnitCircleVec3 / 2f;
			int rotation = Rand.Range(0, 360);
			writeTarget.Add(new DrawCall
			{
				overlayMat = val3,
				matrix = Matrix4x4.Scale(size),
				overlayMesh = val,
				mainTexScale = value,
				mainTexOffset = new Vector4(val4.x, val4.z),
				rotation = rotation
			});
		}
		finally
		{
			Rand.PopState();
		}
	}
}
