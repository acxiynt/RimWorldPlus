using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace Verse;

public class Graphic_Appearances : Graphic
{
	protected Graphic[] subGraphics;

	public override Material MatSingle => subGraphics[StuffAppearanceDefOf.Smooth.index].MatSingle;

	private ThingDef StuffOfThing(Thing thing)
	{
		if (thing is IConstructible constructible)
		{
			return constructible.EntityToBuildStuff();
		}
		return thing.Stuff;
	}

	public override Material MatAt(Rot4 rot, Thing thing = null)
	{
		return SubGraphicFor(thing).MatAt(rot, thing);
	}

	public override void Init(GraphicRequest req)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		data = req.graphicData;
		path = req.path;
		color = req.color;
		drawSize = req.drawSize;
		List<StuffAppearanceDef> allDefsListForReading = DefDatabase<StuffAppearanceDef>.AllDefsListForReading;
		subGraphics = new Graphic[allDefsListForReading.Count];
		for (int i = 0; i < subGraphics.Length; i++)
		{
			StuffAppearanceDef stuffAppearance = allDefsListForReading[i];
			string text = req.path;
			if (!stuffAppearance.pathPrefix.NullOrEmpty())
			{
				text = stuffAppearance.pathPrefix + "/" + text.Split('/').Last();
			}
			Texture2D val = (from x in ContentFinder<Texture2D>.GetAllInFolder(text)
				where ((Object)x).name.EndsWith(stuffAppearance.defName)
				select x).FirstOrDefault();
			if ((Object)(object)val != (Object)null)
			{
				subGraphics[i] = GraphicDatabase.Get<Graphic_Single>(text + "/" + ((Object)val).name, req.shader, drawSize, color);
			}
		}
		for (int num = 0; num < subGraphics.Length; num++)
		{
			if (subGraphics[num] == null)
			{
				subGraphics[num] = subGraphics[StuffAppearanceDefOf.Smooth.index];
			}
		}
	}

	public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (newColorTwo != Color.white)
		{
			Log.ErrorOnce("Cannot use Graphic_Appearances.GetColoredVersion with a non-white colorTwo.", 9910251);
		}
		return GraphicDatabase.Get<Graphic_Appearances>(path, newShader, drawSize, newColor, Color.white, data);
	}

	public override Material MatSingleFor(Thing thing)
	{
		return SubGraphicFor(thing).MatSingleFor(thing);
	}

	public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		SubGraphicFor(thing).DrawWorker(loc, rot, thingDef, thing, extraRotation);
	}

	public Graphic SubGraphicFor(Thing thing)
	{
		StuffAppearanceDef smooth = StuffAppearanceDefOf.Smooth;
		if (thing != null)
		{
			return SubGraphicFor(StuffOfThing(thing));
		}
		return subGraphics[smooth.index];
	}

	public Graphic SubGraphicFor(ThingDef stuff)
	{
		StuffAppearanceDef app = StuffAppearanceDefOf.Smooth;
		if (stuff != null && stuff.stuffProps.appearance != null)
		{
			app = stuff.stuffProps.appearance;
		}
		return SubGraphicFor(app);
	}

	public Graphic SubGraphicFor(StuffAppearanceDef app)
	{
		return subGraphics[app.index];
	}

	public override string ToString()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		return string.Concat("Appearance(path=", path, ", color=", color, ", colorTwo=unsupported)");
	}
}
