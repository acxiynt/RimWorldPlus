using UnityEngine;

namespace Verse;

public static class ShaderUtility
{
	public static bool SupportsMaskTex(this Shader shader)
	{
		if (!((Object)(object)shader == (Object)(object)ShaderDatabase.CutoutComplex) && !((Object)(object)shader == (Object)(object)ShaderDatabase.CutoutSkinOverlay) && !((Object)(object)shader == (Object)(object)ShaderDatabase.Wound) && !((Object)(object)shader == (Object)(object)ShaderDatabase.FirefoamOverlay) && !((Object)(object)shader == (Object)(object)ShaderDatabase.CutoutWithOverlay) && !((Object)(object)shader == (Object)(object)ShaderDatabase.CutoutComplexBlend))
		{
			return (Object)(object)shader == (Object)(object)ShaderDatabase.BioferriteHarvester;
		}
		return true;
	}

	public static Shader GetSkinShader(Pawn pawn)
	{
		foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
		{
			if (hediff.def.skinShader != null)
			{
				return hediff.def.skinShader.Shader;
			}
		}
		bool dead = pawn.Dead || (pawn.IsMutant && pawn.mutant.HasTurned);
		return GetSkinShaderAbstract(pawn.story != null && pawn.story.SkinColorOverriden, dead);
	}

	public static Shader GetSkinShaderAbstract(bool skinColorOverriden, bool dead)
	{
		if (skinColorOverriden)
		{
			return ShaderDatabase.CutoutSkinColorOverride;
		}
		if (dead)
		{
			return ShaderDatabase.Cutout;
		}
		return ShaderDatabase.CutoutSkin;
	}
}
