using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Dialog_StylingStation : Window
{
	public enum StylingTab
	{
		Hair,
		Beard,
		TattooFace,
		TattooBody,
		ApparelColor
	}

	private Pawn pawn;

	private Thing stylingStation;

	private HairDef initialHairDef;

	private BeardDef initialBeardDef;

	private TattooDef initialFaceTattoo;

	private TattooDef initialBodyTattoo;

	private Color desiredHairColor;

	private StylingTab curTab;

	private Vector2 hairScrollPosition;

	private Vector2 beardScrollPosition;

	private Vector2 faceTattooScrollPosition;

	private Vector2 bodyTattooScrollPosition;

	private Vector2 apparelColorScrollPosition;

	private List<TabRecord> tabs = new List<TabRecord>();

	private Dictionary<Apparel, Color> apparelColors = new Dictionary<Apparel, Color>();

	private float viewRectHeight;

	private bool showHeadgear;

	private bool showClothes;

	private bool devEditMode;

	private List<Color> allColors;

	private List<Color> allHairColors;

	private float colorsHeight;

	private static readonly Vector2 ButSize = new Vector2(200f, 40f);

	private static readonly Vector3 PortraitOffset = new Vector3(0f, 0f, 0.15f);

	private const float PortraitZoom = 1.1f;

	private const float TabMargin = 18f;

	private const float IconSize = 60f;

	private const float LeftRectPercent = 0.3f;

	private const float ApparelRowHeight = 126f;

	private const float ApparelRowButtonsHeight = 24f;

	private const float SetColorButtonWidth = 200f;

	private static readonly Texture2D FavoriteColorTex = ContentFinder<Texture2D>.Get("UI/Icons/ColorSelector/ColorFavourite");

	private static readonly Texture2D IdeoColorTex = ContentFinder<Texture2D>.Get("UI/Icons/ColorSelector/ColorIdeology");

	private static List<string> tmpUnwantedStyleNames = new List<string>();

	private static List<StyleItemDef> tmpStyleItems = new List<StyleItemDef>();

	private bool DevMode => stylingStation == null;

	public override Vector2 InitialSize => new Vector2(950f, 750f);

	private List<Color> AllColors
	{
		get
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			if (allColors == null)
			{
				allColors = new List<Color>();
				if (pawn.Ideo != null && !Find.IdeoManager.classicMode)
				{
					allColors.Add(pawn.Ideo.ApparelColor);
				}
				if (pawn.story != null && !pawn.DevelopmentalStage.Baby() && pawn.story.favoriteColor.HasValue && !allColors.Any((Color c) => pawn.story.favoriteColor.Value.IndistinguishableFrom(c)))
				{
					allColors.Add(pawn.story.favoriteColor.Value);
				}
				foreach (ColorDef colDef in DefDatabase<ColorDef>.AllDefs.Where((ColorDef x) => x.colorType == ColorType.Ideo || x.colorType == ColorType.Misc || (DevMode && !ModsConfig.IdeologyActive && x.colorType == ColorType.Structure)))
				{
					if (!allColors.Any((Color x) => x.IndistinguishableFrom(colDef.color)))
					{
						allColors.Add(colDef.color);
					}
				}
				allColors.SortByColor((Color x) => x);
			}
			return allColors;
		}
	}

	private List<Color> AllHairColors
	{
		get
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			if (allHairColors == null)
			{
				allHairColors = new List<Color>();
				foreach (ColorDef allDef in DefDatabase<ColorDef>.AllDefs)
				{
					Color color = allDef.color;
					if (allDef.displayInStylingStationUI && !allHairColors.Any((Color x) => x.WithinDiffThresholdFrom(color, 0.15f)))
					{
						allHairColors.Add(color);
					}
				}
				allHairColors.SortByColor((Color x) => x);
			}
			return allHairColors;
		}
	}

	public Dialog_StylingStation(Pawn pawn, Thing stylingStation)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		this.pawn = pawn;
		this.stylingStation = stylingStation;
		initialHairDef = pawn.story.hairDef;
		desiredHairColor = pawn.story.HairColor;
		initialBeardDef = pawn.style.beardDef;
		initialFaceTattoo = pawn.style.FaceTattoo;
		initialBodyTattoo = pawn.style.BodyTattoo;
		forcePause = true;
		showClothes = true;
		closeOnAccept = false;
		closeOnCancel = false;
		foreach (Apparel item in pawn.apparel.WornApparel)
		{
			if (item.TryGetComp<CompColorable>() != null)
			{
				apparelColors.Add(item, (Color)(((_003F?)item.DesiredColor) ?? item.DrawColor));
			}
		}
	}

	public override void PostOpen()
	{
		if (!ModLister.CheckIdeology("Styling station"))
		{
			Close();
		}
		else
		{
			base.PostOpen();
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		Rect rect = default(Rect);
		((Rect)(ref rect))._002Ector(inRect);
		((Rect)(ref rect)).height = Text.LineHeight * 2f;
		Widgets.Label(rect, "StylePawn".Translate().CapitalizeFirst() + ": " + Find.ActiveLanguageWorker.WithDefiniteArticle(pawn.Name.ToStringShort, pawn.gender, plural: false, name: true).ApplyTag(TagType.Name));
		Text.Font = GameFont.Small;
		((Rect)(ref inRect)).yMin = ((Rect)(ref rect)).yMax + 4f;
		Rect rect2 = inRect;
		((Rect)(ref rect2)).width = ((Rect)(ref rect2)).width * 0.3f;
		((Rect)(ref rect2)).yMax = ((Rect)(ref rect2)).yMax - (ButSize.y + 4f);
		DrawPawn(rect2);
		Rect rect3 = inRect;
		((Rect)(ref rect3)).xMin = ((Rect)(ref rect2)).xMax + 10f;
		((Rect)(ref rect3)).yMax = ((Rect)(ref rect3)).yMax - (ButSize.y + 4f);
		DrawTabs(rect3);
		DrawBottomButtons(inRect);
		if (Prefs.DevMode)
		{
			Widgets.CheckboxLabeled(new Rect(((Rect)(ref inRect)).xMax - 120f, 0f, 120f, 30f), "DEV: Show all", ref devEditMode);
		}
	}

	private void DrawPawn(Rect rect)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		Rect val = rect;
		((Rect)(ref val)).yMin = ((Rect)(ref rect)).yMax - Text.LineHeight * 2f;
		Widgets.CheckboxLabeled(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y, ((Rect)(ref val)).width, ((Rect)(ref val)).height / 2f), "ShowHeadgear".Translate(), ref showHeadgear);
		Widgets.CheckboxLabeled(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y + ((Rect)(ref val)).height / 2f, ((Rect)(ref val)).width, ((Rect)(ref val)).height / 2f), "ShowApparel".Translate(), ref showClothes);
		((Rect)(ref rect)).yMax = ((Rect)(ref val)).yMin - 4f;
		Widgets.BeginGroup(rect);
		for (int i = 0; i < 3; i++)
		{
			Rect val2 = GenUI.ContractedBy(new Rect(0f, ((Rect)(ref rect)).height / 3f * (float)i, ((Rect)(ref rect)).width, ((Rect)(ref rect)).height / 3f), 4f);
			RenderTexture val3 = PortraitsCache.Get(pawn, new Vector2(((Rect)(ref val2)).width, ((Rect)(ref val2)).height), new Rot4(2 - i), PortraitOffset, 1.1f, supersample: true, compensateForUIScale: true, showHeadgear, showClothes, apparelColors, desiredHairColor, stylingStation: true);
			GUI.DrawTexture(val2, (Texture)(object)val3);
		}
		Widgets.EndGroup();
		if (pawn.style.HasAnyUnwantedStyleItem)
		{
			string text = "PawnUnhappyWithStyleItems".Translate(pawn.Named("PAWN")) + ": ";
			tmpUnwantedStyleNames.Clear();
			if (pawn.style.HasUnwantedHairStyle)
			{
				tmpUnwantedStyleNames.Add("Hair".Translate());
			}
			if (pawn.style.HasUnwantedBeard)
			{
				tmpUnwantedStyleNames.Add("Beard".Translate());
			}
			if (pawn.style.HasUnwantedFaceTattoo)
			{
				tmpUnwantedStyleNames.Add("TattooFace".Translate());
			}
			if (pawn.style.HasUnwantedBodyTattoo)
			{
				tmpUnwantedStyleNames.Add("TattooBody".Translate());
			}
			GUI.color = ColorLibrary.RedReadable;
			Widgets.Label(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).yMin - 30f, ((Rect)(ref rect)).width, Text.LineHeight * 2f + 10f), "Warning".Translate() + ": " + text + tmpUnwantedStyleNames.ToCommaList().CapitalizeFirst());
			GUI.color = Color.white;
		}
	}

	private void DrawTabs(Rect rect)
	{
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		tabs.Clear();
		tabs.Add(new TabRecord("Hair".Translate().CapitalizeFirst(), delegate
		{
			curTab = StylingTab.Hair;
		}, curTab == StylingTab.Hair));
		if (pawn.style.CanWantBeard || devEditMode)
		{
			tabs.Add(new TabRecord("Beard".Translate().CapitalizeFirst(), delegate
			{
				curTab = StylingTab.Beard;
			}, curTab == StylingTab.Beard));
		}
		tabs.Add(new TabRecord("TattooFace".Translate().CapitalizeFirst(), delegate
		{
			curTab = StylingTab.TattooFace;
		}, curTab == StylingTab.TattooFace));
		tabs.Add(new TabRecord("TattooBody".Translate().CapitalizeFirst(), delegate
		{
			curTab = StylingTab.TattooBody;
		}, curTab == StylingTab.TattooBody));
		tabs.Add(new TabRecord("ApparelColor".Translate().CapitalizeFirst(), delegate
		{
			curTab = StylingTab.ApparelColor;
		}, curTab == StylingTab.ApparelColor));
		Widgets.DrawMenuSection(rect);
		TabDrawer.DrawTabs(rect, tabs);
		rect = rect.ContractedBy(18f);
		switch (curTab)
		{
		case StylingTab.Hair:
			((Rect)(ref rect)).yMax = ((Rect)(ref rect)).yMax - colorsHeight;
			DrawStylingItemType(rect, ref hairScrollPosition, delegate(Rect r, HairDef h)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				GUI.color = desiredHairColor;
				Widgets.DefIcon(r, h, null, 1.25f);
				GUI.color = Color.white;
			}, delegate(HairDef h)
			{
				pawn.story.hairDef = h;
			}, (StyleItemDef h) => pawn.story.hairDef == h, (StyleItemDef h) => initialHairDef == h, null, doColors: true);
			break;
		case StylingTab.Beard:
			DrawStylingItemType(rect, ref beardScrollPosition, delegate(Rect r, BeardDef b)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				GUI.color = desiredHairColor;
				Widgets.DefIcon(r, b, null, 1.25f);
				GUI.color = Color.white;
			}, delegate(BeardDef b)
			{
				pawn.style.beardDef = b;
			}, (StyleItemDef b) => pawn.style.beardDef == b, (StyleItemDef b) => initialBeardDef == b);
			break;
		case StylingTab.TattooFace:
			DrawStylingItemType(rect, ref faceTattooScrollPosition, delegate(Rect r, TattooDef t)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				Widgets.DefIcon(r, t);
			}, delegate(TattooDef t)
			{
				pawn.style.FaceTattoo = t;
			}, (StyleItemDef t) => pawn.style.FaceTattoo == t, (StyleItemDef t) => initialFaceTattoo == t, (StyleItemDef t) => ((TattooDef)t).tattooType == TattooType.Face);
			break;
		case StylingTab.TattooBody:
			DrawStylingItemType(rect, ref bodyTattooScrollPosition, delegate(Rect r, TattooDef t)
			{
				//IL_0000: Unknown result type (might be due to invalid IL or missing references)
				Widgets.DefIcon(r, t);
			}, delegate(TattooDef t)
			{
				pawn.style.BodyTattoo = t;
			}, (StyleItemDef t) => pawn.style.BodyTattoo == t, (StyleItemDef t) => initialBodyTattoo == t, (StyleItemDef t) => ((TattooDef)t).tattooType == TattooType.Body);
			break;
		case StylingTab.ApparelColor:
			DrawApparelColor(rect);
			break;
		}
	}

	private void DrawDyeRequirement(Rect rect, ref float curY, int requiredDye)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		Widgets.ThingIcon(new Rect(((Rect)(ref rect)).x, curY, Text.LineHeight, Text.LineHeight), ThingDefOf.Dye, null, null, 1.1f);
		string text = (string)("Required".Translate() + ": ") + requiredDye + " " + ThingDefOf.Dye.label;
		float x = Text.CalcSize(text).x;
		Widgets.Label(new Rect(((Rect)(ref rect)).x + Text.LineHeight + 4f, curY, x, Text.LineHeight), text);
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(((Rect)(ref rect)).x, curY, x + Text.LineHeight + 8f, Text.LineHeight);
		if (Mouse.IsOver(rect2))
		{
			Widgets.DrawHighlight(rect2);
			TooltipHandler.TipRegionByKey(rect2, "TooltipDyeExplanation");
		}
		curY += Text.LineHeight;
	}

	private void DrawHairColors(Rect rect)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		float y = ((Rect)(ref rect)).y;
		Widgets.ColorSelector(new Rect(((Rect)(ref rect)).x, y, ((Rect)(ref rect)).width, colorsHeight), ref desiredHairColor, AllHairColors, out colorsHeight);
		y += colorsHeight;
		if (pawn.Spawned && desiredHairColor != pawn.story.HairColor)
		{
			Color val = desiredHairColor;
			Color? nextHairColor = pawn.style.nextHairColor;
			if (!nextHairColor.HasValue || val != nextHairColor.GetValueOrDefault())
			{
				DrawDyeRequirement(rect, ref y, 1);
				if (pawn.Map.resourceCounter.GetCount(ThingDefOf.Dye) < 1)
				{
					Rect rect2 = new Rect(((Rect)(ref rect)).x, y, ((Rect)(ref rect)).width, Text.LineHeight);
					Color color = GUI.color;
					GUI.color = ColorLibrary.RedReadable;
					Widgets.Label(rect2, "NotEnoughDye".Translate() + " " + "NotEnoughDyeWillRecolorHair".Translate());
					GUI.color = color;
				}
			}
		}
		colorsHeight += Text.LineHeight * 2f;
	}

	private void DrawApparelColor(Rect rect)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Invalid comparison between Unknown and I4
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - 16f, viewRectHeight);
		Widgets.BeginScrollView(rect, ref apparelColorScrollPosition, viewRect);
		int num = 0;
		float curY = ((Rect)(ref rect)).y;
		Rect val = default(Rect);
		foreach (Apparel item in pawn.apparel.WornApparel)
		{
			if (!apparelColors.TryGetValue(item, out var value))
			{
				continue;
			}
			((Rect)(ref val))._002Ector(((Rect)(ref rect)).x, curY, ((Rect)(ref viewRect)).width, 92f);
			curY += ((Rect)(ref val)).height + 10f;
			if (!pawn.apparel.IsLocked(item) || DevMode)
			{
				Widgets.ColorSelector(val, ref value, AllColors, out var _, (Texture)(object)Widgets.GetIconFor(item.def, item.Stuff, item.StyleDef), 22, 2, ColorSelecterExtraOnGUI);
				float num2 = ((Rect)(ref val)).x;
				if (pawn.Ideo != null && !Find.IdeoManager.classicMode)
				{
					((Rect)(ref val))._002Ector(num2, curY, 200f, 24f);
					if (Widgets.ButtonText(val, "SetIdeoColor".Translate()))
					{
						value = pawn.Ideo.ApparelColor;
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					}
					num2 += 210f;
				}
				Pawn_StoryTracker story = pawn.story;
				if (story != null && story.favoriteColor.HasValue)
				{
					((Rect)(ref val))._002Ector(num2, curY, 200f, 24f);
					if (Widgets.ButtonText(val, "SetFavoriteColor".Translate()))
					{
						value = pawn.story.favoriteColor.Value;
						SoundDefOf.Tick_Low.PlayOneShotOnCamera();
					}
				}
				if (!value.IndistinguishableFrom(item.DrawColor))
				{
					num++;
				}
				apparelColors[item] = value;
			}
			else
			{
				Widgets.ColorSelectorIcon(new Rect(((Rect)(ref val)).x, ((Rect)(ref val)).y, 88f, 88f), (Texture)(object)item.def.uiIcon, value);
				Text.Anchor = (TextAnchor)3;
				Rect rect2 = val;
				((Rect)(ref rect2)).x = ((Rect)(ref rect2)).x + 100f;
				Widgets.Label(rect2, ((string)"ApparelLockedCannotRecolor".Translate(pawn.Named("PAWN"), item.Named("APPAREL"))).Colorize(ColorLibrary.RedReadable));
				Text.Anchor = (TextAnchor)0;
			}
			curY += 34f;
		}
		if (pawn.Spawned)
		{
			if (num > 0)
			{
				DrawDyeRequirement(rect, ref curY, num);
			}
			if (pawn.Map.resourceCounter.GetCount(ThingDefOf.Dye) < num)
			{
				Rect rect3 = default(Rect);
				((Rect)(ref rect3))._002Ector(((Rect)(ref rect)).x, curY, ((Rect)(ref rect)).width - 16f - 10f, 60f);
				Color color = GUI.color;
				GUI.color = ColorLibrary.RedReadable;
				Widgets.Label(rect3, "NotEnoughDye".Translate() + " " + "NotEnoughDyeWillRecolorApparel".Translate());
				GUI.color = color;
				curY += ((Rect)(ref rect3)).height;
			}
		}
		if ((int)Event.current.type == 8)
		{
			viewRectHeight = curY - ((Rect)(ref rect)).y;
		}
		Widgets.EndScrollView();
	}

	private void ColorSelecterExtraOnGUI(Color color, Rect boxRect)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = null;
		TaggedString taggedString = null;
		bool flag = Mouse.IsOver(boxRect);
		Pawn_StoryTracker story = pawn.story;
		if (story != null && story.favoriteColor.HasValue && color.IndistinguishableFrom(pawn.story.favoriteColor.Value))
		{
			val = FavoriteColorTex;
			if (flag)
			{
				taggedString = "FavoriteColorPickerTip".Translate(pawn.Named("PAWN"));
			}
		}
		else if (pawn.Ideo != null && !Find.IdeoManager.classicMode && color.IndistinguishableFrom(pawn.Ideo.ApparelColor))
		{
			val = IdeoColorTex;
			if (flag)
			{
				taggedString = "IdeoColorPickerTip".Translate(pawn.Named("PAWN"));
			}
		}
		if ((Object)(object)val != (Object)null)
		{
			Rect val2 = boxRect.ContractedBy(4f);
			GUI.color = Color.black.ToTransparent(0.2f);
			GUI.DrawTexture(new Rect(((Rect)(ref val2)).x + 2f, ((Rect)(ref val2)).y + 2f, ((Rect)(ref val2)).width, ((Rect)(ref val2)).height), (Texture)(object)val);
			GUI.color = Color.white.ToTransparent(0.8f);
			GUI.DrawTexture(val2, (Texture)(object)val);
			GUI.color = Color.white;
		}
		if (!taggedString.NullOrEmpty())
		{
			TooltipHandler.TipRegion(boxRect, taggedString);
		}
	}

	private void DrawStylingItemType<T>(Rect rect, ref Vector2 scrollPosition, Action<Rect, T> drawAction, Action<T> selectAction, Func<StyleItemDef, bool> hasStyleItem, Func<StyleItemDef, bool> hadStyleItem, Func<StyleItemDef, bool> extraValidator = null, bool doColors = false) where T : StyleItemDef
	{
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Invalid comparison between Unknown and I4
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		Rect viewRect = default(Rect);
		((Rect)(ref viewRect))._002Ector(((Rect)(ref rect)).x, ((Rect)(ref rect)).y, ((Rect)(ref rect)).width - 16f, viewRectHeight);
		int num = Mathf.FloorToInt(((Rect)(ref viewRect)).width / 60f) - 1;
		float num2 = (((Rect)(ref viewRect)).width - (float)num * 60f - (float)(num - 1) * 10f) / 2f;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		tmpStyleItems.Clear();
		tmpStyleItems.AddRange(DefDatabase<T>.AllDefs.Where((T x) => (devEditMode || PawnStyleItemChooser.WantsToUseStyle(pawn, x) || hadStyleItem(x)) && (extraValidator == null || extraValidator(x))));
		tmpStyleItems.SortBy((StyleItemDef x) => 0f - PawnStyleItemChooser.FrequencyFromGender(x, pawn));
		if (tmpStyleItems.NullOrEmpty())
		{
			Widgets.NoneLabelCenteredVertically(rect, "(" + "NoneUsableForPawn".Translate(pawn.Named("PAWN")) + ")");
		}
		else
		{
			Widgets.BeginScrollView(rect, ref scrollPosition, viewRect);
			Rect val = default(Rect);
			foreach (StyleItemDef tmpStyleItem in tmpStyleItems)
			{
				if (num5 >= num - 1)
				{
					num5 = 0;
					num4++;
				}
				else if (num3 > 0)
				{
					num5++;
				}
				((Rect)(ref val))._002Ector(((Rect)(ref rect)).x + num2 + (float)num5 * 60f + (float)num5 * 10f, ((Rect)(ref rect)).y + (float)num4 * 60f + (float)num4 * 10f, 60f, 60f);
				Widgets.DrawHighlight(val);
				if (Mouse.IsOver(val))
				{
					Widgets.DrawHighlight(val);
					TooltipHandler.TipRegion(val, tmpStyleItem.LabelCap);
				}
				drawAction?.Invoke(val, tmpStyleItem as T);
				if (hasStyleItem(tmpStyleItem))
				{
					Widgets.DrawBox(val, 2);
				}
				if (Widgets.ButtonInvisible(val))
				{
					selectAction?.Invoke(tmpStyleItem as T);
					SoundDefOf.Tick_High.PlayOneShotOnCamera();
					pawn.Drawer.renderer.SetAllGraphicsDirty();
				}
				num3++;
			}
			if ((int)Event.current.type == 8)
			{
				viewRectHeight = (float)(num4 + 1) * 60f + (float)num4 * 10f + 10f;
			}
			Widgets.EndScrollView();
		}
		if (doColors)
		{
			DrawHairColors(new Rect(((Rect)(ref rect)).x, ((Rect)(ref rect)).yMax + 10f, ((Rect)(ref rect)).width, colorsHeight));
		}
	}

	private void DrawBottomButtons(Rect inRect)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		if (Widgets.ButtonText(new Rect(((Rect)(ref inRect)).x, ((Rect)(ref inRect)).yMax - ButSize.y, ButSize.x, ButSize.y), "Cancel".Translate()))
		{
			Reset();
			Close();
		}
		if (Widgets.ButtonText(new Rect(((Rect)(ref inRect)).xMin + ((Rect)(ref inRect)).width / 2f - ButSize.x / 2f, ((Rect)(ref inRect)).yMax - ButSize.y, ButSize.x, ButSize.y), "Reset".Translate()))
		{
			Reset();
			SoundDefOf.Tick_Low.PlayOneShotOnCamera();
		}
		if (!Widgets.ButtonText(new Rect(((Rect)(ref inRect)).xMax - ButSize.x, ((Rect)(ref inRect)).yMax - ButSize.y, ButSize.x, ButSize.y), "Accept".Translate()))
		{
			return;
		}
		if (pawn.story.hairDef != initialHairDef || pawn.style.beardDef != initialBeardDef || pawn.style.FaceTattoo != initialFaceTattoo || pawn.style.BodyTattoo != initialBodyTattoo || pawn.story.HairColor != desiredHairColor)
		{
			if (!DevMode)
			{
				pawn.style.SetupNextLookChangeData(pawn.story.hairDef, pawn.style.beardDef, pawn.style.FaceTattoo, pawn.style.BodyTattoo, desiredHairColor);
				Reset(resetColors: false);
				pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.UseStylingStation, stylingStation), JobTag.Misc);
			}
			else
			{
				pawn.story.HairColor = desiredHairColor;
				pawn.style.Notify_StyleItemChanged();
			}
		}
		ApplyApparelColors();
		Close();
	}

	private void ApplyApparelColors()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<Apparel, Color> apparelColor in apparelColors)
		{
			if (!DevMode)
			{
				if (apparelColor.Key.DrawColor != apparelColor.Value)
				{
					apparelColor.Key.DesiredColor = apparelColor.Value;
				}
			}
			else if (apparelColor.Key.DrawColor != apparelColor.Value)
			{
				apparelColor.Key.DrawColor = apparelColor.Value;
				apparelColor.Key.Notify_ColorChanged();
				apparelColor.Key.DesiredColor = null;
			}
		}
		pawn.mindState.Notify_OutfitChanged();
	}

	private void Reset(bool resetColors = true)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (resetColors)
		{
			apparelColors.Clear();
			foreach (Apparel item in pawn.apparel.WornApparel)
			{
				if (item.TryGetComp<CompColorable>() != null)
				{
					apparelColors.Add(item, (Color)(((_003F?)item.DesiredColor) ?? item.DrawColor));
				}
			}
			desiredHairColor = pawn.story.HairColor;
		}
		pawn.story.hairDef = initialHairDef;
		pawn.style.beardDef = initialBeardDef;
		pawn.style.FaceTattoo = initialFaceTattoo;
		pawn.style.BodyTattoo = initialBodyTattoo;
		pawn.Drawer.renderer.SetAllGraphicsDirty();
	}
}
