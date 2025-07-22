using UnityEngine;

namespace Verse;

public class Graphic_Single : Graphic
{
	protected Material mat;

	public static readonly string MaskSuffix = "_m";

	public override Material MatSingle => mat;

	public override Material MatWest => mat;

	public override Material MatSouth => mat;

	public override Material MatEast => mat;

	public override Material MatNorth => mat;

	public override bool ShouldDrawRotated
	{
		get
		{
			if (data != null && !data.drawRotated)
			{
				return false;
			}
			return true;
		}
	}

	public override void TryInsertIntoAtlas(TextureAtlasGroup groupKey)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		Texture2D mask = null;
		if (mat.HasProperty(ShaderPropertyIDs.MaskTex))
		{
			mask = (Texture2D)mat.GetTexture(ShaderPropertyIDs.MaskTex);
		}
		GlobalTextureAtlasManager.TryInsertStatic(groupKey, (Texture2D)mat.mainTexture, mask);
	}

	public override void Init(GraphicRequest req)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		data = req.graphicData;
		path = req.path;
		maskPath = req.maskPath;
		color = req.color;
		colorTwo = req.colorTwo;
		drawSize = req.drawSize;
		MaterialRequest req2 = new MaterialRequest
		{
			mainTex = (Texture)(object)(req.texture ?? ContentFinder<Texture2D>.Get(req.path)),
			shader = req.shader,
			color = color,
			colorTwo = colorTwo,
			renderQueue = req.renderQueue,
			shaderParameters = req.shaderParameters
		};
		if (req.shader.SupportsMaskTex())
		{
			req2.maskTex = ContentFinder<Texture2D>.Get(maskPath.NullOrEmpty() ? (path + MaskSuffix) : maskPath, reportFailure: false);
		}
		mat = MaterialPool.MatFrom(req2);
	}

	public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return GraphicDatabase.Get<Graphic_Single>(path, newShader, drawSize, newColor, newColorTwo, data);
	}

	public override Material MatAt(Rot4 rot, Thing thing = null)
	{
		return mat;
	}

	public override string ToString()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		return string.Concat("Single(path=", path, ", color=", color, ", colorTwo=", colorTwo, ")");
	}
}
