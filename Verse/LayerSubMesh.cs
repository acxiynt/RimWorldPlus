using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class LayerSubMesh
{
	public bool finalized;

	public bool disabled;

	public Material material;

	public Mesh mesh;

	public List<Vector3> verts = new List<Vector3>();

	public List<int> tris = new List<int>();

	public List<Color32> colors = new List<Color32>();

	public List<Vector3> pollution = new List<Vector3>();

	public List<Vector3> uvs = new List<Vector3>();

	public LayerSubMesh(Mesh mesh, Material material, Bounds? bounds = null)
	{
		this.mesh = mesh;
		this.material = material;
	}

	public void Clear(MeshParts parts)
	{
		if ((parts & MeshParts.Verts) != MeshParts.None)
		{
			verts.Clear();
		}
		if ((parts & MeshParts.Tris) != MeshParts.None)
		{
			tris.Clear();
		}
		if ((parts & MeshParts.Colors) != MeshParts.None)
		{
			colors.Clear();
		}
		if ((parts & MeshParts.UVs) != MeshParts.None)
		{
			uvs.Clear();
		}
		finalized = false;
	}

	public void FinalizeMesh(MeshParts parts)
	{
		if (finalized)
		{
			Log.Warning("Finalizing mesh which is already finalized. Did you forget to call Clear()?");
		}
		if ((parts & MeshParts.Verts) != MeshParts.None || (parts & MeshParts.Tris) != MeshParts.None)
		{
			mesh.Clear();
		}
		if ((parts & MeshParts.Verts) != MeshParts.None)
		{
			if (verts.Count > 0)
			{
				mesh.SetVertices(verts);
			}
			else
			{
				Log.Error("Cannot cook Verts for " + ((object)material).ToString() + ": no ingredients data. If you want to not render this submesh, disable it.");
			}
		}
		if ((parts & MeshParts.Tris) != MeshParts.None)
		{
			if (tris.Count > 0)
			{
				mesh.SetTriangles(tris, 0);
			}
			else
			{
				Log.Error("Cannot cook Tris for " + ((object)material).ToString() + ": no ingredients data.");
			}
		}
		if ((parts & MeshParts.Colors) != MeshParts.None && colors.Count > 0)
		{
			mesh.SetColors(colors);
		}
		if ((parts & MeshParts.UVs) != MeshParts.None && uvs.Count > 0)
		{
			mesh.SetUVs(0, uvs);
		}
		finalized = true;
	}

	public override string ToString()
	{
		return "LayerSubMesh(" + ((object)material).ToString() + ")";
	}
}
