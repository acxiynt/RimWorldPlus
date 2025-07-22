using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public static class GenDraw
{
	public struct FillableBarRequest
	{
		public Vector3 center;

		public Vector2 size;

		public float fillPercent;

		public Material filledMat;

		public Material unfilledMat;

		public float margin;

		public Rot4 rotation;

		public Vector2 preRotationOffset;
	}

	private static readonly Material TargetSquareMatSingle = MaterialPool.MatFrom("UI/Overlays/TargetHighlight_Square", ShaderDatabase.Transparent);

	private static readonly Material InvalidTargetSquareMatSingle = MaterialPool.MatFrom("UI/Overlays/TargetHighlight_Square", ShaderDatabase.Transparent, Color.gray);

	private const float TargetPulseFrequency = 8f;

	public static readonly string LineTexPath = "UI/Overlays/ThingLine";

	public static readonly string OneSidedLineTexPath = "UI/Overlays/OneSidedLine";

	private static readonly Material LineMatWhite = MaterialPool.MatFrom(LineTexPath, ShaderDatabase.Transparent, Color.white);

	private static readonly Material LineMatRed = MaterialPool.MatFrom(LineTexPath, ShaderDatabase.Transparent, Color.red);

	private static readonly Material LineMatGreen = MaterialPool.MatFrom(LineTexPath, ShaderDatabase.Transparent, Color.green);

	private static readonly Material LineMatBlue = MaterialPool.MatFrom(LineTexPath, ShaderDatabase.Transparent, Color.blue);

	private static readonly Material LineMatMagenta = MaterialPool.MatFrom(LineTexPath, ShaderDatabase.Transparent, Color.magenta);

	private static readonly Material LineMatYellow = MaterialPool.MatFrom(LineTexPath, ShaderDatabase.Transparent, Color.yellow);

	private static readonly Material LineMatCyan = MaterialPool.MatFrom(LineTexPath, ShaderDatabase.Transparent, Color.cyan);

	private static readonly Material LineMatOrange = MaterialPool.MatFrom(LineTexPath, ShaderDatabase.Transparent, ColorLibrary.Orange);

	private static readonly Material LineMatMetaOverlay = MaterialPool.MatFrom(LineTexPath, ShaderDatabase.MetaOverlay);

	private static readonly Material WorldLineMatWhite = MaterialPool.MatFrom(LineTexPath, ShaderDatabase.WorldOverlayTransparent, Color.white, WorldMaterials.WorldLineRenderQueue);

	private static readonly Material OneSidedWorldLineMatWhite = MaterialPool.MatFrom(OneSidedLineTexPath, ShaderDatabase.WorldOverlayTransparent, Color.white, WorldMaterials.WorldLineRenderQueue);

	public static readonly Material RitualStencilMat = MaterialPool.MatFrom(ShaderDatabase.RitualStencil);

	private const float LineWidth = 0.2f;

	private const float BaseWorldLineWidth = 0.2f;

	public static readonly Material InteractionCellMaterial = MaterialPool.MatFrom("UI/Overlays/InteractionCell", ShaderDatabase.Transparent);

	private static readonly Color InteractionCellIntensity = new Color(1f, 1f, 1f, 0.3f);

	public const float MultiItemsPerCellDrawSizeFactor = 0.8f;

	private static List<int> cachedEdgeTiles = new List<int>();

	private static int cachedEdgeTilesForCenter = -1;

	private static int cachedEdgeTilesForRadius = -1;

	private static int cachedEdgeTilesForWorldSeed = -1;

	private static List<IntVec3> ringDrawCells = new List<IntVec3>();

	private static bool maxRadiusMessaged = false;

	private static BoolGrid fieldGrid;

	private static bool[] rotNeeded = new bool[4];

	private static readonly Material AimPieMaterial = SolidColorMaterials.SimpleSolidColorMaterial(new Color(1f, 1f, 1f, 0.3f));

	private static readonly Material ArrowMatWhite = MaterialPool.MatFrom("UI/Overlays/Arrow", ShaderDatabase.CutoutFlying01, Color.white);

	private static readonly Material ArrowMatGhost = MaterialPool.MatFrom("UI/Overlays/ArrowGhost", ShaderDatabase.Transparent, Color.white);

	public static Material CurTargetingMat
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			TargetSquareMatSingle.color = CurTargetingColor;
			return TargetSquareMatSingle;
		}
	}

	public static Color CurTargetingColor
	{
		get
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			float num = (float)Math.Sin(Time.time * 8f);
			num *= 0.2f;
			num += 0.8f;
			return new Color(1f, num, num);
		}
	}

	public static Material InvalidTargetMat => InvalidTargetSquareMatSingle;

	public static void DrawNoBuildEdgeLines()
	{
		DrawMapEdgeLines(10);
	}

	public static void DrawNoZoneEdgeLines()
	{
		DrawMapEdgeLines(5);
	}

	private static void DrawMapEdgeLines(int edgeDist)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		float num = AltitudeLayer.MetaOverlays.AltitudeFor();
		IntVec3 size = Find.CurrentMap.Size;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector((float)edgeDist, num, (float)edgeDist);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector((float)edgeDist, num, (float)(size.z - edgeDist));
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector((float)(size.x - edgeDist), num, (float)(size.z - edgeDist));
		Vector3 val4 = default(Vector3);
		((Vector3)(ref val4))._002Ector((float)(size.x - edgeDist), num, (float)edgeDist);
		DrawLineBetween(val, val2, LineMatMetaOverlay);
		DrawLineBetween(val2, val3, LineMatMetaOverlay);
		DrawLineBetween(val3, val4, LineMatMetaOverlay);
		DrawLineBetween(val4, val, LineMatMetaOverlay);
	}

	public static void DrawLineBetween(Vector3 A, Vector3 B)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DrawLineBetween(A, B, LineMatWhite);
	}

	public static void DrawLineBetween(Vector3 A, Vector3 B, float layer)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DrawLineBetween(A, B, layer, LineMatWhite);
	}

	public static void DrawLineBetween(Vector3 A, Vector3 B, float layer, Material mat, float lineWidth = 0.2f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		DrawLineBetween(A + Vector3.up * layer, B + Vector3.up * layer, mat, lineWidth);
	}

	public static void DrawLineBetween(Vector3 A, Vector3 B, SimpleColor color, float lineWidth = 0.2f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DrawLineBetween(A, B, GetLineMat(color), lineWidth);
	}

	public static void DrawLineBetween(Vector3 A, Vector3 B, Material mat, float lineWidth = 0.2f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (!(Mathf.Abs(A.x - B.x) < 0.01f) || !(Mathf.Abs(A.z - B.z) < 0.01f))
		{
			Vector3 val = (A + B) / 2f;
			if (!(A == B))
			{
				A.y = B.y;
				float num = (A - B).MagnitudeHorizontal();
				Quaternion val2 = Quaternion.LookRotation(A - B);
				Vector3 val3 = default(Vector3);
				((Vector3)(ref val3))._002Ector(lineWidth, 1f, num);
				Matrix4x4 val4 = default(Matrix4x4);
				((Matrix4x4)(ref val4)).SetTRS(val, val2, val3);
				Graphics.DrawMesh(MeshPool.plane10, val4, mat, 0);
			}
		}
	}

	public static void DrawCircleOutline(Vector3 center, float radius)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		DrawCircleOutline(center, radius, LineMatWhite);
	}

	public static void DrawCircleOutline(Vector3 center, float radius, SimpleColor color)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		DrawCircleOutline(center, radius, GetLineMat(color));
	}

	public static void DrawCircleOutline(Vector3 center, float radius, Material material)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.Clamp(Mathf.RoundToInt(24f * radius), 12, 48);
		float num2 = 0f;
		float num3 = (float)Math.PI * 2f / (float)num;
		Vector3 val = center;
		Vector3 a = center;
		for (int i = 0; i < num + 2; i++)
		{
			if (i >= 2)
			{
				DrawLineBetween(a, val, material);
			}
			a = val;
			val = center;
			val.x += Mathf.Cos(num2) * radius;
			val.z += Mathf.Sin(num2) * radius;
			num2 += num3;
		}
	}

	private static Material GetLineMat(SimpleColor color)
	{
		return (Material)(color switch
		{
			SimpleColor.White => LineMatWhite, 
			SimpleColor.Red => LineMatRed, 
			SimpleColor.Green => LineMatGreen, 
			SimpleColor.Blue => LineMatBlue, 
			SimpleColor.Magenta => LineMatMagenta, 
			SimpleColor.Yellow => LineMatYellow, 
			SimpleColor.Cyan => LineMatCyan, 
			SimpleColor.Orange => LineMatOrange, 
			_ => LineMatWhite, 
		});
	}

	public static void DrawWorldLineBetween(Vector3 A, Vector3 B, float widthFactor = 1f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DrawWorldLineBetween(A, B, WorldLineMatWhite, widthFactor);
	}

	public static void DrawWorldLineBetween(Vector3 A, Vector3 B, Material material, float widthFactor = 1f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!(Mathf.Abs(A.x - B.x) < 0.005f) || !(Mathf.Abs(A.y - B.y) < 0.005f) || !(Mathf.Abs(A.z - B.z) < 0.005f))
		{
			Vector3 val = (A + B) / 2f;
			Vector3 val2 = A - B;
			float magnitude = ((Vector3)(ref val2)).magnitude;
			Quaternion val3 = Quaternion.LookRotation(A - B, ((Vector3)(ref val)).normalized);
			Vector3 val4 = default(Vector3);
			((Vector3)(ref val4))._002Ector(0.2f * Find.WorldGrid.averageTileSize * widthFactor, 1f, magnitude);
			Matrix4x4 val5 = default(Matrix4x4);
			((Matrix4x4)(ref val5)).SetTRS(val, val3, val4);
			Graphics.DrawMesh(MeshPool.plane10, val5, material, WorldCameraManager.WorldLayer);
		}
	}

	public static void DrawWorldRadiusRing(int center, int radius)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (radius < 0)
		{
			return;
		}
		if (cachedEdgeTilesForCenter != center || cachedEdgeTilesForRadius != radius || cachedEdgeTilesForWorldSeed != Find.World.info.Seed)
		{
			cachedEdgeTilesForCenter = center;
			cachedEdgeTilesForRadius = radius;
			cachedEdgeTilesForWorldSeed = Find.World.info.Seed;
			cachedEdgeTiles.Clear();
			Find.WorldFloodFiller.FloodFill(center, (int tile) => true, delegate(int tile, int dist)
			{
				if (dist > radius + 1)
				{
					return true;
				}
				if (dist == radius + 1)
				{
					cachedEdgeTiles.Add(tile);
				}
				return false;
			});
			WorldGrid worldGrid = Find.WorldGrid;
			Vector3 c = worldGrid.GetTileCenter(center);
			Vector3 n = ((Vector3)(ref c)).normalized;
			cachedEdgeTiles.Sort(delegate(int a, int b)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000d: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Unknown result type (might be due to invalid IL or missing references)
				//IL_0024: Unknown result type (might be due to invalid IL or missing references)
				//IL_002a: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0034: Unknown result type (might be due to invalid IL or missing references)
				float num = Vector3.Dot(n, Vector3.Cross(worldGrid.GetTileCenter(a) - c, worldGrid.GetTileCenter(b) - c));
				if (Mathf.Abs(num) < 0.0001f)
				{
					return 0;
				}
				return (!(num < 0f)) ? 1 : (-1);
			});
		}
		DrawWorldLineStrip(cachedEdgeTiles, OneSidedWorldLineMatWhite, 5f);
	}

	public static void DrawWorldLineStrip(List<int> edgeTiles, Material material, float widthFactor)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (edgeTiles.Count < 3)
		{
			return;
		}
		WorldGrid worldGrid = Find.WorldGrid;
		float num = 0.05f;
		for (int i = 0; i < edgeTiles.Count; i++)
		{
			int index = ((i == 0) ? (edgeTiles.Count - 1) : (i - 1));
			int num2 = edgeTiles[index];
			int num3 = edgeTiles[i];
			if (worldGrid.IsNeighbor(num2, num3))
			{
				Vector3 val = worldGrid.GetTileCenter(num2);
				Vector3 val2 = worldGrid.GetTileCenter(num3);
				val += ((Vector3)(ref val)).normalized * num;
				val2 += ((Vector3)(ref val2)).normalized * num;
				DrawWorldLineBetween(val, val2, material, widthFactor);
			}
		}
	}

	public static void DrawTargetHighlight(LocalTargetInfo targ)
	{
		if (targ.Thing != null)
		{
			DrawTargetingHighlight_Thing(targ.Thing);
		}
		else
		{
			DrawTargetingHighlight_Cell(targ.Cell);
		}
	}

	private static void DrawTargetingHighlight_Cell(IntVec3 c)
	{
		DrawTargetHighlightWithLayer(c, AltitudeLayer.Building);
	}

	public static void DrawTargetHighlightWithLayer(IntVec3 c, AltitudeLayer layer, Material material = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = c.ToVector3ShiftedWithAltitude(layer);
		Graphics.DrawMesh(MeshPool.plane10, val, Quaternion.identity, material ?? CurTargetingMat, 0);
	}

	public static void DrawTargetHighlightWithLayer(Vector3 c, AltitudeLayer layer)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(c.x, layer.AltitudeFor(), c.z);
		Graphics.DrawMesh(MeshPool.plane10, val, Quaternion.identity, CurTargetingMat, 0);
	}

	private static void DrawTargetingHighlight_Thing(Thing t)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = t.TrueCenter();
		Graphics.DrawMesh(MeshPool.plane10, new Vector3(val.x, AltitudeLayer.MapDataOverlay.AltitudeFor(), val.z), t.Rotation.AsQuat, CurTargetingMat, 0);
		if (t is Pawn || t is Corpse)
		{
			TargetHighlighter.Highlight(t, arrow: false);
		}
	}

	public static void DrawStencilCell(Vector3 c, Material material, float width = 1f, float height = 1f)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 val = default(Matrix4x4);
		((Matrix4x4)(ref val)).SetTRS(new Vector3(c.x, -1f, c.z), Quaternion.identity, new Vector3(width, 1f, height));
		Graphics.DrawMesh(MeshPool.plane10, val, material, 0);
	}

	public static void DrawTargetingHightlight_Explosion(IntVec3 c, float Radius)
	{
		DrawRadiusRing(c, Radius);
	}

	public static void DrawInteractionCells(ThingDef tDef, IntVec3 center, Rot4 placingRot)
	{
		if (!tDef.multipleInteractionCellOffsets.NullOrEmpty())
		{
			foreach (IntVec3 multipleInteractionCellOffset in tDef.multipleInteractionCellOffsets)
			{
				DrawInteractionCell(tDef, multipleInteractionCellOffset, center, placingRot);
			}
			return;
		}
		if (tDef.hasInteractionCell)
		{
			DrawInteractionCell(tDef, tDef.interactionCellOffset, center, placingRot);
		}
	}

	private static void DrawInteractionCell(ThingDef tDef, IntVec3 interactionOffset, IntVec3 center, Rot4 placingRot)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		IntVec3 c = ThingUtility.InteractionCell(interactionOffset, center, placingRot);
		Vector3 val = c.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays);
		if (c.InBounds(Find.CurrentMap))
		{
			Building edifice = c.GetEdifice(Find.CurrentMap);
			if (edifice != null && edifice.def.building != null && edifice.def.building.isSittable)
			{
				return;
			}
		}
		if (tDef.interactionCellGraphic == null && tDef.interactionCellIcon != null)
		{
			ThingDef thingDef = tDef.interactionCellIcon;
			if (thingDef.blueprintDef != null)
			{
				thingDef = thingDef.blueprintDef;
			}
			tDef.interactionCellGraphic = thingDef.graphic.GetColoredVersion(ShaderTypeDefOf.EdgeDetect.Shader, InteractionCellIntensity, Color.white);
		}
		if (tDef.interactionCellGraphic != null)
		{
			Rot4 rot = (tDef.interactionCellIconReverse ? placingRot.Opposite : placingRot);
			tDef.interactionCellGraphic.DrawFromDef(val, rot, tDef.interactionCellIcon);
		}
		else
		{
			Graphics.DrawMesh(MeshPool.plane10, val, Quaternion.identity, InteractionCellMaterial, 0);
		}
	}

	public static void DrawRadiusRing(IntVec3 center, float radius, Color color, Func<IntVec3, bool> predicate = null)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (radius > GenRadial.MaxRadialPatternRadius)
		{
			if (!maxRadiusMessaged)
			{
				Log.Error("Cannot draw radius ring of radius " + radius + ": not enough squares in the precalculated list.");
				maxRadiusMessaged = true;
			}
			return;
		}
		ringDrawCells.Clear();
		int num = GenRadial.NumCellsInRadius(radius);
		for (int i = 0; i < num; i++)
		{
			IntVec3 intVec = center + GenRadial.RadialPattern[i];
			if (predicate == null || predicate(intVec))
			{
				ringDrawCells.Add(intVec);
			}
		}
		DrawFieldEdges(ringDrawCells, color);
	}

	public static void DrawRadiusRing(IntVec3 center, float radius)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		DrawRadiusRing(center, radius, Color.white);
	}

	public static void DrawFieldEdges(List<IntVec3> cells)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DrawFieldEdges(cells, Color.white);
	}

	public static void DrawFieldEdges(List<IntVec3> cells, Color color, float? altOffset = null)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		Map currentMap = Find.CurrentMap;
		Material val = MaterialPool.MatFrom(new MaterialRequest
		{
			shader = ShaderDatabase.Transparent,
			color = color,
			BaseTexPath = "UI/Overlays/TargetHighlight_Side"
		});
		val.GetTexture("_MainTex").wrapMode = (TextureWrapMode)1;
		if (fieldGrid == null)
		{
			fieldGrid = new BoolGrid(currentMap);
		}
		else
		{
			fieldGrid.ClearAndResizeTo(currentMap);
		}
		int x = currentMap.Size.x;
		int z = currentMap.Size.z;
		int count = cells.Count;
		float num = altOffset ?? (Rand.ValueSeeded(((object)color.ToOpaque()/*cast due to .constrained prefix*/).GetHashCode()) * (1f / 26f) / 10f);
		for (int i = 0; i < count; i++)
		{
			if (cells[i].InBounds(currentMap))
			{
				fieldGrid[cells[i].x, cells[i].z] = true;
			}
		}
		for (int j = 0; j < count; j++)
		{
			IntVec3 c = cells[j];
			if (!c.InBounds(currentMap))
			{
				continue;
			}
			rotNeeded[0] = c.z < z - 1 && !fieldGrid[c.x, c.z + 1];
			rotNeeded[1] = c.x < x - 1 && !fieldGrid[c.x + 1, c.z];
			rotNeeded[2] = c.z > 0 && !fieldGrid[c.x, c.z - 1];
			rotNeeded[3] = c.x > 0 && !fieldGrid[c.x - 1, c.z];
			for (int k = 0; k < 4; k++)
			{
				if (rotNeeded[k])
				{
					Graphics.DrawMesh(MeshPool.plane10, c.ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays) + new Vector3(0f, num, 0f), new Rot4(k).AsQuat, val, 0);
				}
			}
		}
	}

	public static void DrawAimPie(Thing shooter, LocalTargetInfo target, int degreesWide, float offsetDist)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		float facing = 0f;
		if (target.Cell != shooter.Position)
		{
			facing = ((target.Thing == null) ? (target.Cell - shooter.Position).AngleFlat : (target.Thing.DrawPos - shooter.Position.ToVector3Shifted()).AngleFlat());
		}
		DrawAimPieRaw(shooter.DrawPos + new Vector3(0f, offsetDist, 0f), facing, degreesWide);
	}

	public static void DrawAimPieRaw(Vector3 center, float facing, int degreesWide)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (degreesWide > 0)
		{
			if (degreesWide > 360)
			{
				degreesWide = 360;
			}
			center += Quaternion.AngleAxis(facing, Vector3.up) * Vector3.forward * 0.8f;
			Graphics.DrawMesh(MeshPool.pies[degreesWide], center, Quaternion.AngleAxis(facing + (float)(degreesWide / 2) - 90f, Vector3.up), AimPieMaterial, 0);
		}
	}

	public static void DrawCooldownCircle(Vector3 center, float radius)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(radius, 1f, radius);
		Matrix4x4 val2 = default(Matrix4x4);
		((Matrix4x4)(ref val2)).SetTRS(center, Quaternion.identity, val);
		Graphics.DrawMesh(MeshPool.circle, val2, AimPieMaterial, 0);
	}

	public static void DrawFillableBar(FillableBarRequest r)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = r.preRotationOffset.RotatedBy(r.rotation.AsAngle);
		ref Vector3 center = ref r.center;
		center += new Vector3(val.x, 0f, val.y);
		if (r.rotation == Rot4.South)
		{
			r.rotation = Rot4.North;
		}
		if (r.rotation == Rot4.West)
		{
			r.rotation = Rot4.East;
		}
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(r.size.x + r.margin, 1f, r.size.y + r.margin);
		Matrix4x4 val3 = default(Matrix4x4);
		((Matrix4x4)(ref val3)).SetTRS(r.center, r.rotation.AsQuat, val2);
		Graphics.DrawMesh(MeshPool.plane10, val3, r.unfilledMat, 0);
		if (r.fillPercent > 0.001f)
		{
			((Vector3)(ref val2))._002Ector(r.size.x * r.fillPercent, 1f, r.size.y);
			val3 = default(Matrix4x4);
			Vector3 val4 = r.center + Vector3.up * 0.01f;
			if (!r.rotation.IsHorizontal)
			{
				val4.x -= r.size.x * 0.5f;
				val4.x += 0.5f * r.size.x * r.fillPercent;
			}
			else
			{
				val4.z -= r.size.x * 0.5f;
				val4.z += 0.5f * r.size.x * r.fillPercent;
			}
			((Matrix4x4)(ref val3)).SetTRS(val4, r.rotation.AsQuat, val2);
			Graphics.DrawMesh(MeshPool.plane10, val3, r.filledMat, 0);
		}
	}

	public static void DrawMeshNowOrLater(Mesh mesh, Vector3 loc, Quaternion quat, Material mat, bool drawNow)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (drawNow)
		{
			if (mat == null || !mat.SetPass(0))
			{
				string obj = ((mat != null) ? ((Object)mat).name : null);
				object obj2;
				if (mat == null)
				{
					obj2 = null;
				}
				else
				{
					Shader shader = mat.shader;
					obj2 = ((shader != null) ? ((Object)shader).name : null);
				}
				Log.Error("SetPass(0) call failed on material " + obj + " with shader " + (string)obj2);
			}
			Graphics.DrawMeshNow(mesh, loc, quat);
		}
		else
		{
			Graphics.DrawMesh(mesh, loc, quat, mat, 0);
		}
	}

	public static void DrawMeshNowOrLater(Mesh mesh, Matrix4x4 matrix, Material mat, bool drawNow, MaterialPropertyBlock properties = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (drawNow)
		{
			mat.SetPass(0);
			Graphics.DrawMeshNow(mesh, matrix);
		}
		else
		{
			Graphics.DrawMesh(mesh, matrix, mat, 0, (Camera)null, 0, properties);
		}
	}

	public static void DrawArrowPointingAt(Vector3 mapTarget, bool offscreenOnly = false)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = UI.UIToMapPosition(UI.screenWidth / 2, UI.screenHeight / 2);
		if ((val - mapTarget).MagnitudeHorizontalSquared() < 81f)
		{
			if (!offscreenOnly)
			{
				Vector3 val2 = mapTarget;
				val2.y = AltitudeLayer.MetaOverlays.AltitudeFor();
				val2.z -= 1.5f;
				Graphics.DrawMesh(MeshPool.plane20, val2, Quaternion.identity, ArrowMatWhite, 0);
			}
		}
		else
		{
			Vector3 val3 = (mapTarget - val).Yto0();
			Vector3 normalized = ((Vector3)(ref val3)).normalized;
			Vector3 val4 = val + normalized * 7f;
			val4.y = AltitudeLayer.MetaOverlays.AltitudeFor();
			Quaternion val5 = Quaternion.LookRotation(normalized);
			Graphics.DrawMesh(MeshPool.plane20, val4, val5, ArrowMatWhite, 0);
		}
	}

	public static void DrawArrowRotated(Vector3 pos, float rotationAngle, bool ghost)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.AngleAxis(rotationAngle, new Vector3(0f, 1f, 0f));
		Vector3 val2 = pos;
		val2.y = AltitudeLayer.MetaOverlays.AltitudeFor();
		Graphics.DrawMesh(MeshPool.plane10, val2, val, ghost ? ArrowMatGhost : ArrowMatWhite, 0);
	}
}
