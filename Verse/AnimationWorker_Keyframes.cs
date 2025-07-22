using UnityEngine;

namespace Verse;

public class AnimationWorker_Keyframes : AnimationWorker
{
	public AnimationWorker_Keyframes(AnimationDef def, Pawn pawn, AnimationPart part, PawnRenderNode node)
		: base(def, pawn, part, node)
	{
	}

	public override Vector3 OffsetAtTick(int tick, PawnDrawParms parms)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		if (tick <= part.keyframes[0].tick)
		{
			return part.keyframes[0].offset;
		}
		if (tick >= part.keyframes[part.keyframes.Count - 1].tick)
		{
			return part.keyframes[part.keyframes.Count - 1].offset;
		}
		Keyframe keyframe = part.keyframes[0];
		Keyframe keyframe2 = part.keyframes[part.keyframes.Count - 1];
		for (int i = 0; i < part.keyframes.Count; i++)
		{
			if (tick <= part.keyframes[i].tick)
			{
				keyframe2 = part.keyframes[i];
				if (i > 0)
				{
					keyframe = part.keyframes[i - 1];
				}
				break;
			}
		}
		float num = (float)(tick - keyframe.tick) / (float)(keyframe2.tick - keyframe.tick);
		return def.scale * Vector3.Lerp(keyframe.offset, keyframe2.offset, num);
	}

	public override float AngleAtTick(int tick, PawnDrawParms parms)
	{
		if (tick <= part.keyframes[0].tick)
		{
			return part.keyframes[0].angle;
		}
		if (tick >= part.keyframes[part.keyframes.Count - 1].tick)
		{
			return part.keyframes[part.keyframes.Count - 1].angle;
		}
		Keyframe keyframe = part.keyframes[0];
		Keyframe keyframe2 = part.keyframes[part.keyframes.Count - 1];
		for (int i = 0; i < part.keyframes.Count; i++)
		{
			if (tick <= part.keyframes[i].tick)
			{
				keyframe2 = part.keyframes[i];
				if (i > 0)
				{
					keyframe = part.keyframes[i - 1];
				}
				break;
			}
		}
		float num = (float)(tick - keyframe.tick) / (float)(keyframe2.tick - keyframe.tick);
		return def.scale * Mathf.Lerp(keyframe.angle, keyframe2.angle, num);
	}

	public override Vector3 ScaleAtTick(int tick, PawnDrawParms parms)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if (tick <= part.keyframes[0].tick)
		{
			return part.keyframes[0].scale;
		}
		if (tick >= part.keyframes[part.keyframes.Count - 1].tick)
		{
			return part.keyframes[part.keyframes.Count - 1].scale;
		}
		Keyframe keyframe = part.keyframes[0];
		Keyframe keyframe2 = part.keyframes[part.keyframes.Count - 1];
		for (int i = 0; i < part.keyframes.Count; i++)
		{
			if (tick <= part.keyframes[i].tick)
			{
				keyframe2 = part.keyframes[i];
				if (i > 0)
				{
					keyframe = part.keyframes[i - 1];
				}
				break;
			}
		}
		float num = (float)(tick - keyframe.tick) / (float)(keyframe2.tick - keyframe.tick);
		return Vector3.Lerp(keyframe.scale, keyframe2.scale, num);
	}
}
