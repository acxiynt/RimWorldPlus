using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class GraphicMote_RandomWithAgeSecs : Graphic_Random
{
	protected static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

	protected virtual bool ForcePropertyBlock => true;

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Mote mote = (Mote)thing;
		propertyBlock.SetColor(ShaderPropertyIDs.Color, color);
		propertyBlock.SetFloat(ShaderPropertyIDs.AgeSecs, mote.AgeSecs);
		propertyBlock.SetFloat(ShaderPropertyIDs.AgeSecsPausable, mote.AgeSecsPausable);
		Graphic_Mote.DrawMote(data, SubGraphicFor((Mote)thing).MatSingle, base.Color, loc, rot, thingDef, thing, 0, ForcePropertyBlock, propertyBlock);
	}

	public Graphic SubGraphicFor(Mote mote)
	{
		return subGraphics[mote.offsetRandom % subGraphics.Length];
	}

	public override string ToString()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		return string.Concat("Mote(path=", path, ", shader=", base.Shader, ", color=", color, ", colorTwo=unsupported)");
	}
}
