using System;
using UnityEngine;

namespace Verse;

public static class GenGeo
{
	public static float AngleDifferenceBetween(float A, float B)
	{
		float num = A + 360f;
		float num2 = B + 360f;
		float num3 = 9999f;
		float num4 = 0f;
		num4 = A - B;
		if (num4 < 0f)
		{
			num4 *= -1f;
		}
		if (num4 < num3)
		{
			num3 = num4;
		}
		num4 = num - B;
		if (num4 < 0f)
		{
			num4 *= -1f;
		}
		if (num4 < num3)
		{
			num3 = num4;
		}
		num4 = A - num2;
		if (num4 < 0f)
		{
			num4 *= -1f;
		}
		if (num4 < num3)
		{
			num3 = num4;
		}
		return num3;
	}

	public static float MagnitudeHorizontal(this Vector3 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return (float)Math.Sqrt(v.x * v.x + v.z * v.z);
	}

	public static float MagnitudeHorizontalSquared(this Vector3 v)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return v.x * v.x + v.z * v.z;
	}

	public static bool LinesIntersect(Vector3 line1V1, Vector3 line1V2, Vector3 line2V1, Vector3 line2V2)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		float num = line1V2.z - line1V1.z;
		float num2 = line1V1.x - line1V2.x;
		float num3 = num * line1V1.x + num2 * line1V1.z;
		float num4 = line2V2.z - line2V1.z;
		float num5 = line2V1.x - line2V2.x;
		float num6 = num4 * line2V1.x + num5 * line2V1.z;
		float num7 = num * num5 - num4 * num2;
		if (num7 == 0f)
		{
			return false;
		}
		float num8 = (num5 * num3 - num2 * num6) / num7;
		float num9 = (num * num6 - num4 * num3) / num7;
		if ((num8 > line1V1.x && num8 > line1V2.x) || (num8 > line2V1.x && num8 > line2V2.x) || (num8 < line1V1.x && num8 < line1V2.x) || (num8 < line2V1.x && num8 < line2V2.x) || (num9 > line1V1.z && num9 > line1V2.z) || (num9 > line2V1.z && num9 > line2V2.z) || (num9 < line1V1.z && num9 < line1V2.z) || (num9 < line2V1.z && num9 < line2V2.z))
		{
			return false;
		}
		return true;
	}

	public static bool IntersectLineCircle(Vector2 center, float radius, Vector2 lineA, Vector2 lineB)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = center - lineA;
		Vector2 val2 = lineB - lineA;
		float num = Vector2.Dot(val2, val2);
		float num2 = Vector2.Dot(val, val2) / num;
		if (num2 < 0f)
		{
			num2 = 0f;
		}
		else if (num2 > 1f)
		{
			num2 = 1f;
		}
		Vector2 val3 = val2 * num2 + lineA - center;
		return Vector2.Dot(val3, val3) <= radius * radius;
	}

	public static bool IntersectLineCircleOutline(Vector2 center, float radius, Vector2 lineA, Vector2 lineB)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = lineA - center;
		bool num = ((Vector2)(ref val)).sqrMagnitude <= radius * radius;
		val = lineB - center;
		bool flag = ((Vector2)(ref val)).sqrMagnitude <= radius * radius;
		if (num && flag)
		{
			return false;
		}
		return IntersectLineCircle(center, radius, lineA, lineB);
	}

	public static Vector3 RegularPolygonVertexPositionVec3(int polygonVertices, int vertexIndex)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = RegularPolygonVertexPosition(polygonVertices, vertexIndex);
		return new Vector3(val.x, 0f, val.y);
	}

	public static Vector2 RegularPolygonVertexPosition(int polygonVertices, int vertexIndex, float angleOffset = 0f)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (vertexIndex < 0 || vertexIndex >= polygonVertices)
		{
			Log.Warning("Vertex index out of bounds. polygonVertices=" + polygonVertices + " vertexIndex=" + vertexIndex);
			return Vector2.zero;
		}
		if (polygonVertices == 1)
		{
			return Vector2.zero;
		}
		return CalculatePolygonVertexPosition(polygonVertices, vertexIndex, angleOffset);
	}

	private static Vector2 CalculatePolygonVertexPosition(int polygonVertices, int vertexIndex, float angleOffset = 0f)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)Math.PI * 2f / (float)polygonVertices * (float)vertexIndex;
		num += (float)Math.PI;
		num += (float)Math.PI / 180f * angleOffset;
		return Vector2.op_Implicit(new Vector3(Mathf.Cos(num), Mathf.Sin(num)));
	}

	public static Vector2 InverseQuadBilinear(Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		float num = (p0 - p).Cross(p0 - p2);
		float num2 = ((p0 - p).Cross(p1 - p3) + (p1 - p).Cross(p0 - p2)) / 2f;
		float num3 = (p1 - p).Cross(p1 - p3);
		float num4 = num2 * num2 - num * num3;
		if (num4 < 0f)
		{
			return new Vector2(-1f, -1f);
		}
		num4 = Mathf.Sqrt(num4);
		float num5;
		if (Mathf.Abs(num - 2f * num2 + num3) < 0.0001f)
		{
			num5 = num / (num - num3);
		}
		else
		{
			float num6 = (num - num2 + num4) / (num - 2f * num2 + num3);
			float num7 = (num - num2 - num4) / (num - 2f * num2 + num3);
			num5 = ((!(Mathf.Abs(num6 - 0.5f) < Mathf.Abs(num7 - 0.5f))) ? num7 : num6);
		}
		float num8 = (1f - num5) * (p0.x - p2.x) + num5 * (p1.x - p3.x);
		float num9 = (1f - num5) * (p0.y - p2.y) + num5 * (p1.y - p3.y);
		if (Mathf.Abs(num8) < Mathf.Abs(num9))
		{
			return new Vector2(num5, ((1f - num5) * (p0.y - p.y) + num5 * (p1.y - p.y)) / num9);
		}
		return new Vector2(num5, ((1f - num5) * (p0.x - p.x) + num5 * (p1.x - p.x)) / num8);
	}

	public static int GetAdjacencyScore(this CellRect rect, CellRect other)
	{
		if (rect.Overlaps(other))
		{
			return 0;
		}
		if (rect.maxZ == other.minZ - 1 && rect.minX < other.maxX && rect.maxX > other.minX)
		{
			int num = Mathf.Max(rect.minX, other.minX);
			return Mathf.Min(rect.maxX, other.maxX) - num;
		}
		if (rect.minZ == other.maxZ + 1 && rect.minX < other.maxX && rect.maxX > other.minX)
		{
			int num2 = Mathf.Max(rect.minX, other.minX);
			return Mathf.Min(rect.maxX, other.maxX) - num2;
		}
		if (rect.minX == other.maxX + 1 && rect.minZ < other.maxZ && rect.maxZ > other.minZ)
		{
			int num3 = Mathf.Max(rect.minZ, other.minZ);
			return Mathf.Min(rect.maxZ, other.maxZ) - num3;
		}
		if (rect.maxX == other.minX - 1 && rect.minZ < other.maxZ && rect.maxZ > other.minZ)
		{
			int num4 = Mathf.Max(rect.minZ, other.minZ);
			return Mathf.Min(rect.maxZ, other.maxZ) - num4;
		}
		return 0;
	}

	public static CellRect ExpandToFit(this CellRect rect, IntVec3 position)
	{
		if (rect.Contains(position))
		{
			return rect;
		}
		rect.minX = Mathf.Min(rect.minX, position.x);
		rect.minZ = Mathf.Min(rect.minZ, position.z);
		rect.maxX = Mathf.Max(rect.maxX, position.x);
		rect.maxZ = Mathf.Max(rect.maxZ, position.z);
		return rect;
	}
}
