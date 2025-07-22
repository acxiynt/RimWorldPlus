using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse;

public class WaterInfo : MapComponent
{
	public byte[] riverOffsetMap;

	public Texture2D riverOffsetTexture;

	public List<Vector3> riverDebugData = new List<Vector3>();

	public float[] riverFlowMap;

	public CellRect riverFlowMapBounds;

	public const int RiverOffsetMapBorder = 2;

	public WaterInfo(Map map)
		: base(map)
	{
	}

	public override void MapRemoved()
	{
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			Object.Destroy((Object)(object)riverOffsetTexture);
		});
	}

	public void SetTextures()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		Camera subcamera = Current.SubcameraDriver.GetSubcamera(SubcameraDefOf.WaterDepth);
		if ((Object)(object)subcamera != (Object)null)
		{
			Shader.SetGlobalTexture(ShaderPropertyIDs.WaterOutputTex, (Texture)(object)subcamera.targetTexture);
		}
		if ((Object)(object)riverOffsetTexture == (Object)null && riverOffsetMap != null && riverOffsetMap.Length != 0)
		{
			riverOffsetTexture = new Texture2D(map.Size.x + 4, map.Size.z + 4, (TextureFormat)19, false);
			riverOffsetTexture.LoadRawTextureData(riverOffsetMap);
			((Texture)riverOffsetTexture).wrapMode = (TextureWrapMode)1;
			riverOffsetTexture.Apply();
		}
		Shader.SetGlobalTexture(ShaderPropertyIDs.WaterOffsetTex, (Texture)(object)riverOffsetTexture);
	}

	public Vector3 GetWaterMovement(Vector3 position)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		if (riverOffsetMap == null)
		{
			return Vector3.zero;
		}
		if (riverFlowMap == null)
		{
			GenerateRiverFlowMap();
		}
		IntVec3 intVec = new IntVec3(Mathf.FloorToInt(position.x), 0, Mathf.FloorToInt(position.z));
		IntVec3 c = new IntVec3(Mathf.FloorToInt(position.x) + 1, 0, Mathf.FloorToInt(position.z) + 1);
		if (!riverFlowMapBounds.Contains(intVec) || !riverFlowMapBounds.Contains(c))
		{
			return Vector3.zero;
		}
		int num = riverFlowMapBounds.IndexOf(intVec);
		int num2 = num + 1;
		int num3 = num + riverFlowMapBounds.Width;
		int num4 = num3 + 1;
		Vector3 val = Vector3.Lerp(new Vector3(riverFlowMap[num * 2], 0f, riverFlowMap[num * 2 + 1]), new Vector3(riverFlowMap[num2 * 2], 0f, riverFlowMap[num2 * 2 + 1]), position.x - Mathf.Floor(position.x));
		Vector3 val2 = Vector3.Lerp(new Vector3(riverFlowMap[num3 * 2], 0f, riverFlowMap[num3 * 2 + 1]), new Vector3(riverFlowMap[num4 * 2], 0f, riverFlowMap[num4 * 2 + 1]), position.x - Mathf.Floor(position.x));
		return Vector3.Lerp(val, val2, position.z - (float)Mathf.FloorToInt(position.z));
	}

	public void GenerateRiverFlowMap()
	{
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		if (riverOffsetMap == null)
		{
			return;
		}
		riverFlowMapBounds = new CellRect(-2, -2, map.Size.x + 4, map.Size.z + 4);
		riverFlowMap = new float[riverFlowMapBounds.Area * 2];
		float[] array = new float[riverFlowMapBounds.Area * 2];
		Buffer.BlockCopy(riverOffsetMap, 0, array, 0, array.Length * 4);
		Vector3 val = default(Vector3);
		for (int i = riverFlowMapBounds.minZ; i <= riverFlowMapBounds.maxZ; i++)
		{
			int newZ = ((i == riverFlowMapBounds.minZ) ? i : (i - 1));
			int newZ2 = ((i == riverFlowMapBounds.maxZ) ? i : (i + 1));
			float num = ((i == riverFlowMapBounds.minZ || i == riverFlowMapBounds.maxZ) ? 1 : 2);
			for (int j = riverFlowMapBounds.minX; j <= riverFlowMapBounds.maxX; j++)
			{
				int newX = ((j == riverFlowMapBounds.minX) ? j : (j - 1));
				int newX2 = ((j == riverFlowMapBounds.maxX) ? j : (j + 1));
				float num2 = ((j == riverFlowMapBounds.minX || j == riverFlowMapBounds.maxX) ? 1 : 2);
				float num3 = (array[riverFlowMapBounds.IndexOf(new IntVec3(newX2, 0, i)) * 2 + 1] - array[riverFlowMapBounds.IndexOf(new IntVec3(newX, 0, i)) * 2 + 1]) / num2;
				float num4 = (array[riverFlowMapBounds.IndexOf(new IntVec3(j, 0, newZ2)) * 2 + 1] - array[riverFlowMapBounds.IndexOf(new IntVec3(j, 0, newZ)) * 2 + 1]) / num;
				((Vector3)(ref val))._002Ector(num3, 0f, num4);
				if (((Vector3)(ref val)).magnitude > 0.0001f)
				{
					val = ((Vector3)(ref val)).normalized / ((Vector3)(ref val)).magnitude;
					int num5 = riverFlowMapBounds.IndexOf(new IntVec3(j, 0, i)) * 2;
					riverFlowMap[num5] = val.x;
					riverFlowMap[num5 + 1] = val.z;
				}
			}
		}
	}

	public override void ExposeData()
	{
		base.ExposeData();
		DataExposeUtility.LookByteArray(ref riverOffsetMap, "riverOffsetMap");
		GenerateRiverFlowMap();
	}

	public void DebugDrawRiver()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < riverDebugData.Count; i += 2)
		{
			GenDraw.DrawLineBetween(riverDebugData[i], riverDebugData[i + 1], SimpleColor.Magenta);
		}
	}
}
