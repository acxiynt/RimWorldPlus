using UnityEngine;

namespace Verse;

public class Graphic_MoteWithAgeSecs : Graphic_Mote
{
	protected override bool ForcePropertyBlock => true;

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		Mote mote = (Mote)thing;
		Graphic_Mote.propertyBlock.SetColor(ShaderPropertyIDs.Color, color);
		Graphic_Mote.propertyBlock.SetFloat(ShaderPropertyIDs.AgeSecs, mote.AgeSecs);
		Graphic_Mote.propertyBlock.SetFloat(ShaderPropertyIDs.AgeSecsPausable, mote.AgeSecsPausable);
		Graphic_Mote.propertyBlock.SetFloat(ShaderPropertyIDs.RandomPerObject, (float)Gen.HashCombineInt(mote.spawnTick, ((object)mote.DrawPos/*cast due to .constrained prefix*/).GetHashCode()));
		Graphic_Mote.propertyBlock.SetFloat(ShaderPropertyIDs.RandomPerObjectOffsetRandom, (float)Gen.HashCombineInt(mote.spawnTick, mote.offsetRandom));
		DrawMoteInternal(loc, rot, thingDef, thing, 0);
	}

	public override string ToString()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		return string.Concat("Graphic_MoteWithAgeSecs(path=", path, ", shader=", base.Shader, ", color=", color, ", colorTwo=unsupported)");
	}
}
