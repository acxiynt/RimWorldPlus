namespace Verse;

public class Graphic_FleckPulse : Graphic_Fleck
{
	protected override bool AllowInstancing => false;

	public override void DrawFleck(FleckDrawData drawData, DrawBatch batch)
	{
		drawData.propertyBlock = drawData.propertyBlock ?? batch.GetPropertyBlock();
		drawData.propertyBlock.SetFloat(ShaderPropertyIDs.AgeSecs, drawData.ageSecs);
		drawData.propertyBlock.SetFloat(ShaderPropertyIDs.RandomPerObject, drawData.id);
		base.DrawFleck(drawData, batch);
	}

	public override string ToString()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		return string.Concat("Graphic_FleckPulse(path=", path, ", shader=", base.Shader, ", color=", color, ", colorTwo=unsupported)");
	}
}
