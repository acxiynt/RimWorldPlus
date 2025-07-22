using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public static class MeshMakerCircles
{
	public static Mesh MakePieMesh(int DegreesWide)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Expected O, but got Unknown
		List<Vector2> list = new List<Vector2>();
		list.Add(new Vector2(0f, 0f));
		Vector2 item = default(Vector2);
		for (int i = 0; i < DegreesWide; i++)
		{
			float num = (float)i / 180f * (float)Math.PI;
			((Vector2)(ref item))._002Ector(0f, 0f);
			item.x = (float)(0.550000011920929 * Math.Cos(num));
			item.y = (float)(0.550000011920929 * Math.Sin(num));
			list.Add(item);
		}
		Vector3[] array = (Vector3[])(object)new Vector3[list.Count];
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = new Vector3(list[j].x, 0f, list[j].y);
		}
		int[] triangles = new Triangulator(list.ToArray()).Triangulate();
		Mesh val = new Mesh
		{
			name = "MakePieMesh()",
			vertices = array,
			uv = (Vector2[])(object)new Vector2[list.Count],
			triangles = triangles
		};
		val.RecalculateNormals();
		val.RecalculateBounds();
		return val;
	}

	public static Mesh MakeCircleMesh(float radius)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Expected O, but got Unknown
		List<Vector2> list = new List<Vector2>();
		list.Add(new Vector2(0f, 0f));
		for (int i = 0; i <= 360; i += 4)
		{
			float num = (float)i / 180f * (float)Math.PI;
			list.Add(new Vector2(radius * Mathf.Cos(num), radius * Mathf.Sin(num)));
		}
		Vector3[] array = (Vector3[])(object)new Vector3[list.Count];
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = new Vector3(list[j].x, 0f, list[j].y);
		}
		int[] array2 = new int[(array.Length - 1) * 3];
		for (int k = 1; k < array.Length; k++)
		{
			int num2 = (k - 1) * 3;
			array2[num2] = 0;
			array2[num2 + 1] = (k + 1) % array.Length;
			array2[num2 + 2] = k;
		}
		return new Mesh
		{
			name = "MakeCircleMesh()",
			vertices = array,
			triangles = array2
		};
	}
}
