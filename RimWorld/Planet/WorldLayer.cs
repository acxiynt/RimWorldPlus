using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet;

[StaticConstructorOnStartup]
public class WorldLayer
{
	protected List<LayerSubMesh> subMeshes = new List<LayerSubMesh>();

	private bool dirty = true;

	private static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

	private const int MaxVerticesPerMesh = 40000;

	public virtual bool ShouldRegenerate => dirty;

	protected virtual int Layer => WorldCameraManager.WorldLayer;

	protected virtual Quaternion Rotation => Quaternion.identity;

	protected virtual float Alpha => 1f;

	public bool Dirty => dirty;

	protected LayerSubMesh GetSubMesh(Material material)
	{
		int subMeshIndex;
		return GetSubMesh(material, out subMeshIndex);
	}

	protected LayerSubMesh GetSubMesh(Material material, out int subMeshIndex)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		for (int i = 0; i < subMeshes.Count; i++)
		{
			LayerSubMesh layerSubMesh = subMeshes[i];
			if ((Object)(object)layerSubMesh.material == (Object)(object)material && layerSubMesh.verts.Count < 40000)
			{
				subMeshIndex = i;
				return layerSubMesh;
			}
		}
		Mesh val = new Mesh();
		if (UnityData.isEditor)
		{
			((Object)val).name = "WorldLayerSubMesh_" + GetType().Name + "_" + Find.World.info.seedString;
		}
		LayerSubMesh layerSubMesh2 = new LayerSubMesh(val, material);
		subMeshIndex = subMeshes.Count;
		subMeshes.Add(layerSubMesh2);
		return layerSubMesh2;
	}

	protected void FinalizeMesh(MeshParts tags)
	{
		for (int i = 0; i < subMeshes.Count; i++)
		{
			if (subMeshes[i].verts.Count > 0)
			{
				subMeshes[i].FinalizeMesh(tags);
			}
		}
	}

	public void RegenerateNow()
	{
		dirty = false;
		Regenerate().ExecuteEnumerable();
	}

	public virtual void Render()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		if (ShouldRegenerate)
		{
			RegenerateNow();
		}
		int layer = Layer;
		Quaternion rotation = Rotation;
		float alpha = Alpha;
		for (int i = 0; i < subMeshes.Count; i++)
		{
			if (subMeshes[i].finalized)
			{
				if (alpha != 1f)
				{
					Color color = subMeshes[i].material.color;
					propertyBlock.SetColor(ShaderPropertyIDs.Color, new Color(color.r, color.g, color.b, color.a * alpha));
					Graphics.DrawMesh(subMeshes[i].mesh, Vector3.zero, rotation, subMeshes[i].material, layer, (Camera)null, 0, propertyBlock);
				}
				else
				{
					Graphics.DrawMesh(subMeshes[i].mesh, Vector3.zero, rotation, subMeshes[i].material, layer);
				}
			}
		}
	}

	public virtual IEnumerable Regenerate()
	{
		dirty = false;
		ClearSubMeshes(MeshParts.All);
		yield break;
	}

	public void SetDirty()
	{
		dirty = true;
	}

	private void ClearSubMeshes(MeshParts parts)
	{
		for (int i = 0; i < subMeshes.Count; i++)
		{
			subMeshes[i].Clear(parts);
		}
	}
}
