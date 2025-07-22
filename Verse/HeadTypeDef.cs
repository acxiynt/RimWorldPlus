using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class HeadTypeDef : Def
{
	public string graphicPath;

	public Gender gender;

	public bool narrow;

	public Vector2 hairMeshSize = new Vector2(1.5f, 1.5f);

	public Vector2 beardMeshSize = new Vector2(1.5f, 1.5f);

	public Vector3 beardOffset;

	public Vector3? eyeOffsetEastWest;

	public float beardOffsetXEast;

	public float selectionWeight = 1f;

	public bool randomChosen = true;

	public List<GeneDef> requiredGenes;

	[Unsaved(false)]
	private List<KeyValuePair<Color, Graphic_Multi>> graphics = new List<KeyValuePair<Color, Graphic_Multi>>();

	public Graphic_Multi GetGraphic(Pawn pawn, Color color)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Shader skinShader = ShaderUtility.GetSkinShader(pawn);
		for (int i = 0; i < graphics.Count; i++)
		{
			if (color.IndistinguishableFrom(graphics[i].Key) && (Object)(object)graphics[i].Value.Shader == (Object)(object)skinShader)
			{
				return graphics[i].Value;
			}
		}
		Graphic_Multi graphic_Multi = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>(graphicPath, skinShader, Vector2.one, color);
		graphics.Add(new KeyValuePair<Color, Graphic_Multi>(color, graphic_Multi));
		return graphic_Multi;
	}
}
