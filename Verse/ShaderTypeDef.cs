using UnityEngine;

namespace Verse;

public class ShaderTypeDef : Def
{
	[NoTranslate]
	public string shaderPath;

	[Unsaved(false)]
	private Shader shaderInt;

	public Shader Shader
	{
		get
		{
			if ((Object)(object)shaderInt == (Object)null)
			{
				shaderInt = ShaderDatabase.LoadShader(shaderPath);
			}
			return shaderInt;
		}
	}
}
