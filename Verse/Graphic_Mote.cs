using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class Graphic_Mote : Graphic_Single
{
	protected static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

	protected virtual bool ForcePropertyBlock => false;

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		DrawMoteInternal(loc, rot, thingDef, thing, 0);
	}

	public void DrawMoteInternal(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, int layer)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		DrawMote(data, MatSingle, base.Color, loc, rot, thingDef, thing, 0, ForcePropertyBlock);
	}

	public static void DrawMote(GraphicData data, Material material, Color color, Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, int layer, bool forcePropertyBlock = false, MaterialPropertyBlock overridePropertyBlock = null)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		Mote mote = (Mote)thing;
		float alpha = mote.Alpha;
		if (!(alpha <= 0f))
		{
			Color val = color * mote.instanceColor;
			val.a *= alpha;
			Vector3 exactScale = mote.ExactScale;
			exactScale.x *= data.drawSize.x;
			exactScale.z *= data.drawSize.y;
			Matrix4x4 val2 = default(Matrix4x4);
			((Matrix4x4)(ref val2)).SetTRS(mote.DrawPos, Quaternion.AngleAxis(mote.exactRotation, Vector3.up), exactScale);
			if (!forcePropertyBlock && val.IndistinguishableFrom(material.color))
			{
				Graphics.DrawMesh(MeshPool.plane10, val2, material, layer, (Camera)null, 0);
				return;
			}
			propertyBlock.SetColor(ShaderPropertyIDs.Color, val);
			Graphics.DrawMesh(MeshPool.plane10, val2, material, layer, (Camera)null, 0, overridePropertyBlock ?? propertyBlock);
		}
	}

	public override string ToString()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		return string.Concat("Mote(path=", path, ", shader=", base.Shader, ", color=", color, ", colorTwo=unsupported)");
	}
}
