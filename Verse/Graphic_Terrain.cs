namespace Verse;

public class Graphic_Terrain : Graphic_Single
{
	public override void Init(GraphicRequest req)
	{
		base.Init(req);
	}

	public override string ToString()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		return string.Concat("Terrain(path=", path, ", shader=", base.Shader, ", color=", color, ")");
	}
}
