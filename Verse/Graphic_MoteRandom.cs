using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class Graphic_MoteRandom : Graphic_Random
{
	protected static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

	protected virtual bool ForcePropertyBlock => false;

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Graphic_Mote.DrawMote(data, SubGraphicFor((Mote)thing).MatSingle, base.Color, loc, rot, thingDef, thing, 0, ForcePropertyBlock);
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
