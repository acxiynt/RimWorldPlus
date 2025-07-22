using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace Verse;

public class TerrainDef : BuildableDef
{
	public enum TerrainEdgeType : byte
	{
		Hard,
		Fade,
		FadeRough,
		Water
	}

	public enum TerrainCategoryType : byte
	{
		Misc,
		Soil,
		Stone,
		Sand
	}

	[NoTranslate]
	public string texturePath;

	public TerrainEdgeType edgeType;

	[NoTranslate]
	public string waterDepthShader;

	public List<ShaderParameter> waterDepthShaderParameters;

	public int renderPrecedence;

	public List<TerrainAffordanceDef> affordances = new List<TerrainAffordanceDef>();

	public bool layerable;

	[NoTranslate]
	public string scatterType;

	public bool takeFootprints;

	public bool natural;

	public bool takeSplashes;

	public bool avoidWander;

	public bool changeable = true;

	public TerrainDef smoothedTerrain;

	public bool holdSnow = true;

	public bool isPaintable;

	public bool extinguishesFire;

	public bool bridge;

	public Color color = Color.white;

	public ColorDef colorDef;

	public TerrainDef driesTo;

	[NoTranslate]
	public List<string> tags;

	public TerrainDef burnedDef;

	public List<Tool> tools;

	public float extraDeteriorationFactor;

	public float destroyOnBombDamageThreshold = -1f;

	public bool destroyBuildingsOnDestroyed;

	public ThoughtDef traversedThought;

	public int extraDraftedPerceivedPathCost;

	public int extraNonDraftedPerceivedPathCost;

	public EffecterDef destroyEffect;

	public EffecterDef destroyEffectWater;

	public bool autoRebuildable;

	public TerrainCategoryType categoryType;

	[NoTranslate]
	public string pollutedTexturePath;

	[NoTranslate]
	public string pollutionOverlayTexturePath;

	public ShaderTypeDef pollutionShaderType;

	public Color pollutionColor = Color.white;

	public Vector2 pollutionOverlayScrollSpeed = Vector2.zero;

	public Vector2 pollutionOverlayScale = Vector2.one;

	public Color pollutionCloudColor = Color.white;

	public Color pollutionTintColor = Color.white;

	public ThingDef generatedFilth;

	public FilthSourceFlags filthAcceptanceMask = FilthSourceFlags.Any;

	[Unsaved(false)]
	public Material waterDepthMaterial;

	[Unsaved(false)]
	public Graphic graphicPolluted = BaseContent.BadGraphic;

	public bool Removable => layerable;

	public bool IsCarpet
	{
		get
		{
			if (researchPrerequisites != null)
			{
				return researchPrerequisites.Contains(ResearchProjectDefOf.CarpetMaking);
			}
			return false;
		}
	}

	public Color DrawColor
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (colorDef != null)
			{
				return colorDef.color;
			}
			return color;
		}
	}

	public bool IsRiver => HasTag("River");

	public bool IsOcean => HasTag("Ocean");

	public bool IsWater => HasTag("Water");

	public bool IsFine => HasTag("FineFloor");

	public bool IsSoil => HasTag("Soil");

	public bool IsRoad => HasTag("Road");

	public bool IsFloor => HasTag("Floor");

	public Shader Shader
	{
		get
		{
			Shader result = null;
			switch (edgeType)
			{
			case TerrainEdgeType.Hard:
				result = ShaderDatabase.TerrainHard;
				break;
			case TerrainEdgeType.Fade:
				result = ShaderDatabase.TerrainFade;
				break;
			case TerrainEdgeType.FadeRough:
				result = ShaderDatabase.TerrainFadeRough;
				break;
			case TerrainEdgeType.Water:
				result = ShaderDatabase.TerrainWater;
				break;
			}
			return result;
		}
	}

	private Shader ShaderPolluted
	{
		get
		{
			if (pollutionShaderType != null)
			{
				return pollutionShaderType.Shader;
			}
			Shader result = null;
			switch (edgeType)
			{
			case TerrainEdgeType.Hard:
				result = ShaderDatabase.TerrainHardPolluted;
				break;
			case TerrainEdgeType.Fade:
				result = ShaderDatabase.TerrainFadePolluted;
				break;
			case TerrainEdgeType.FadeRough:
				result = ShaderDatabase.TerrainFadeRoughPolluted;
				break;
			}
			return result;
		}
	}

	public Material DrawMatPolluted
	{
		get
		{
			if (graphicPolluted == BaseContent.BadGraphic)
			{
				return graphic.MatSingle;
			}
			return graphicPolluted.MatSingle;
		}
	}

	public override void PostLoad()
	{
		placingDraggableDimensions = 2;
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_015f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01be: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			if (graphic == BaseContent.BadGraphic)
			{
				Shader shader = Shader;
				graphic = GraphicDatabase.Get<Graphic_Terrain>(texturePath, shader, Vector2.one, DrawColor, 2000 + renderPrecedence);
				if ((Object)(object)shader == (Object)(object)ShaderDatabase.TerrainFadeRough || (Object)(object)shader == (Object)(object)ShaderDatabase.TerrainWater)
				{
					graphic.MatSingle.SetTexture("_AlphaAddTex", (Texture)(object)TexGame.AlphaAddTex);
				}
			}
			if (!waterDepthShader.NullOrEmpty())
			{
				waterDepthMaterial = MaterialAllocator.Create(ShaderDatabase.LoadShader(waterDepthShader));
				waterDepthMaterial.renderQueue = 2000 + renderPrecedence;
				waterDepthMaterial.SetTexture("_AlphaAddTex", (Texture)(object)TexGame.AlphaAddTex);
				if (waterDepthShaderParameters != null)
				{
					for (int i = 0; i < waterDepthShaderParameters.Count; i++)
					{
						waterDepthShaderParameters[i].Apply(waterDepthMaterial);
					}
				}
			}
			if (ModsConfig.BiotechActive && graphicPolluted == BaseContent.BadGraphic && (!pollutionOverlayTexturePath.NullOrEmpty() || !pollutedTexturePath.NullOrEmpty()))
			{
				Texture2D val = null;
				if (!pollutionOverlayTexturePath.NullOrEmpty())
				{
					val = ContentFinder<Texture2D>.Get(pollutionOverlayTexturePath);
				}
				graphicPolluted = GraphicDatabase.Get<Graphic_Terrain>(pollutedTexturePath ?? texturePath, ShaderPolluted, Vector2.one, DrawColor, 2000 + renderPrecedence);
				Material matSingle = graphicPolluted.MatSingle;
				if ((Object)(object)val != (Object)null)
				{
					matSingle.SetTexture("_BurnTex", (Texture)(object)val);
				}
				matSingle.SetColor("_BurnColor", pollutionColor);
				matSingle.SetVector("_ScrollSpeed", Vector4.op_Implicit(pollutionOverlayScrollSpeed));
				matSingle.SetVector("_BurnScale", Vector4.op_Implicit(pollutionOverlayScale));
				matSingle.SetColor("_PollutionTintColor", pollutionTintColor);
				if ((Object)(object)ShaderPolluted == (Object)(object)ShaderDatabase.TerrainFadeRoughPolluted)
				{
					matSingle.SetTexture("_AlphaAddTex", (Texture)(object)TexGame.AlphaAddTex);
				}
			}
		});
		if (tools != null)
		{
			for (int num = 0; num < tools.Count; num++)
			{
				tools[num].id = num.ToString();
			}
		}
		base.PostLoad();
	}

	protected override void ResolveIcon()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.ResolveIcon();
		uiIconColor = DrawColor;
	}

	public override IEnumerable<string> ConfigErrors()
	{
		foreach (string item in base.ConfigErrors())
		{
			yield return item;
		}
		if (texturePath.NullOrEmpty())
		{
			yield return "missing texturePath";
		}
		if (fertility < 0f)
		{
			yield return string.Concat("Terrain Def ", this, " has no fertility value set.");
		}
		if (renderPrecedence > 400)
		{
			yield return "Render order " + renderPrecedence + " is out of range (must be < 400)";
		}
		if (generatedFilth != null && (filthAcceptanceMask & FilthSourceFlags.Terrain) > FilthSourceFlags.None)
		{
			yield return defName + " makes terrain filth and also accepts it.";
		}
		if (this.Flammable() && burnedDef == null && !layerable)
		{
			yield return "flammable but burnedDef is null and not layerable";
		}
		if (burnedDef != null && burnedDef.Flammable())
		{
			yield return "burnedDef is flammable";
		}
	}

	public static TerrainDef Named(string defName)
	{
		return DefDatabase<TerrainDef>.GetNamed(defName);
	}

	public bool HasTag(string tag)
	{
		if (tags != null)
		{
			return tags.Contains(tag);
		}
		return false;
	}

	public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
	{
		foreach (StatDrawEntry item in base.SpecialDisplayStats(req))
		{
			yield return item;
		}
		string[] array = (from ta in affordances.Distinct()
			orderby ta.order
			select ta.label).ToArray();
		if (array.Length != 0)
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Terrain, "Supports".Translate(), array.ToCommaList().CapitalizeFirst(), "Stat_Thing_Terrain_Supports_Desc".Translate(), 2000);
		}
		if (IsFine && ModsConfig.RoyaltyActive)
		{
			yield return new StatDrawEntry(StatCategoryDefOf.Terrain, "Stat_Thing_Terrain_Fine_Name".Translate(), "Stat_Thing_Terrain_Fine_Value".Translate(), "Stat_Thing_Terrain_Fine_Desc".Translate(), 2000);
		}
	}
}
