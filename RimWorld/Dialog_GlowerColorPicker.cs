using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld;

public class Dialog_GlowerColorPicker : Window
{
	private const float GlowValue = 1f;

	private const int ContextHash = 195906069;

	private static readonly List<Color> colors = new List<Color>
	{
		Color.HSVToRGB(0f, 0f, 1f),
		Color.HSVToRGB(0f, 0.5f, 1f),
		Color.HSVToRGB(0f, 0.33f, 1f),
		Color.HSVToRGB(1f / 18f, 1f, 1f),
		Color.HSVToRGB(1f / 18f, 0.5f, 1f),
		Color.HSVToRGB(1f / 18f, 0.33f, 1f),
		Color.HSVToRGB(1f / 9f, 1f, 1f),
		Color.HSVToRGB(1f / 9f, 0.5f, 1f),
		Color.HSVToRGB(1f / 9f, 0.33f, 1f),
		Color.HSVToRGB(1f / 6f, 1f, 1f),
		Color.HSVToRGB(1f / 6f, 0.5f, 1f),
		Color.HSVToRGB(1f / 6f, 0.33f, 1f),
		Color.HSVToRGB(2f / 9f, 1f, 1f),
		Color.HSVToRGB(2f / 9f, 0.5f, 1f),
		Color.HSVToRGB(2f / 9f, 0.33f, 1f),
		Color.HSVToRGB(5f / 18f, 1f, 1f),
		Color.HSVToRGB(5f / 18f, 0.5f, 1f),
		Color.HSVToRGB(5f / 18f, 0.33f, 1f),
		Color.HSVToRGB(1f / 3f, 1f, 1f),
		Color.HSVToRGB(1f / 3f, 0.5f, 1f),
		Color.HSVToRGB(1f / 3f, 0.33f, 1f),
		Color.HSVToRGB(7f / 18f, 1f, 1f),
		Color.HSVToRGB(7f / 18f, 0.5f, 1f),
		Color.HSVToRGB(7f / 18f, 0.33f, 1f),
		Color.HSVToRGB(4f / 9f, 1f, 1f),
		Color.HSVToRGB(4f / 9f, 0.5f, 1f),
		Color.HSVToRGB(4f / 9f, 0.33f, 1f),
		Color.HSVToRGB(0.5f, 1f, 1f),
		Color.HSVToRGB(0.5f, 0.5f, 1f),
		Color.HSVToRGB(0.5f, 0.33f, 1f),
		Color.HSVToRGB(5f / 9f, 1f, 1f),
		Color.HSVToRGB(5f / 9f, 0.5f, 1f),
		Color.HSVToRGB(5f / 9f, 0.33f, 1f),
		Color.HSVToRGB(11f / 18f, 1f, 1f),
		Color.HSVToRGB(11f / 18f, 0.5f, 1f),
		Color.HSVToRGB(11f / 18f, 0.33f, 1f),
		Color.HSVToRGB(2f / 3f, 1f, 1f),
		Color.HSVToRGB(2f / 3f, 0.5f, 1f),
		Color.HSVToRGB(2f / 3f, 0.33f, 1f),
		Color.HSVToRGB(13f / 18f, 1f, 1f),
		Color.HSVToRGB(13f / 18f, 0.5f, 1f),
		Color.HSVToRGB(13f / 18f, 0.33f, 1f),
		Color.HSVToRGB(7f / 9f, 1f, 1f),
		Color.HSVToRGB(7f / 9f, 0.5f, 1f),
		Color.HSVToRGB(7f / 9f, 0.33f, 1f),
		Color.HSVToRGB(5f / 6f, 1f, 1f),
		Color.HSVToRGB(5f / 6f, 0.5f, 1f),
		Color.HSVToRGB(5f / 6f, 0.33f, 1f),
		Color.HSVToRGB(8f / 9f, 1f, 1f),
		Color.HSVToRGB(8f / 9f, 0.5f, 1f),
		Color.HSVToRGB(8f / 9f, 0.33f, 1f),
		Color.HSVToRGB(17f / 18f, 1f, 1f),
		Color.HSVToRGB(17f / 18f, 0.5f, 1f),
		Color.HSVToRGB(17f / 18f, 0.33f, 1f)
	};

	private static readonly List<string> focusableControlNames = new List<string> { "title", "colorTextfields_0", "colorTextfields_1", "colorTextfields_2", "colorTextfields_3", "colorTextfields_4" };

	private const int ColorWheelSize = 128;

	private const int ColorTextfieldsWidth = 125;

	private const int PaletteColumns = 9;

	private const int ColorIconSize = 22;

	private const int ColorIconPadding = 2;

	private const int CurrentColorLabelWidth = 100;

	private const int PaletteWidth = 250;

	private const int ColorTemperatureBarHeight = 34;

	private CompGlower glower;

	private CompGlower[] extraGlowers;

	private Color color;

	private Color oldColor;

	private bool hsvColorWheelDragging;

	private bool colorTemperatureDragging;

	private string[] textfieldBuffers = new string[6];

	private Color textfieldColorBuffer;

	private string previousFocusedControlName;

	private Widgets.ColorComponents visibleTextfields;

	private Widgets.ColorComponents editableTextfields;

	protected static readonly Vector2 ButSize = new Vector2(150f, 38f);

	public override Vector2 InitialSize => new Vector2(600f, 450f);

	public bool ShowDarklight { get; set; } = true;

	public Dialog_GlowerColorPicker(CompGlower glower, IList<CompGlower> extraGlowers, Widgets.ColorComponents visibleTextfields, Widgets.ColorComponents editableTextfields)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		this.glower = glower;
		this.extraGlowers = new CompGlower[extraGlowers.Count];
		extraGlowers.CopyTo(this.extraGlowers, 0);
		float num = default(float);
		float num2 = default(float);
		float num3 = default(float);
		Color.RGBToHSV(glower.GlowColor.ToColor, ref num, ref num2, ref num3);
		color = Color.HSVToRGB(num, num2, 1f);
		oldColor = color;
		this.visibleTextfields = visibleTextfields;
		this.editableTextfields = editableTextfields;
		forcePause = true;
		absorbInputAroundWindow = true;
		closeOnClickedOutside = true;
		closeOnAccept = false;
	}

	private static void HeaderRow(ref RectDivider layout)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		using (new TextBlock(GameFont.Medium))
		{
			TaggedString taggedString = "ChooseAColor".Translate().CapitalizeFirst();
			string text = taggedString;
			Rect rect = layout.Rect;
			RectDivider rectDivider = layout.NewRow(Text.CalcHeight(text, ((Rect)(ref rect)).width));
			GUI.SetNextControlName(focusableControlNames[0]);
			Rect rect2 = rectDivider.Rect;
			((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y - 5f;
			Widgets.Label(rect2, taggedString);
		}
	}

	private void BottomButtons(ref RectDivider layout)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		RectDivider rectDivider = layout.NewRow(ButSize.y, VerticalJustification.Bottom);
		if (Widgets.ButtonText(rectDivider.NewCol(ButSize.x), "Cancel".Translate()))
		{
			Close();
		}
		if (Widgets.ButtonText(rectDivider.NewCol(ButSize.x, HorizontalJustification.Right), "Accept".Translate()))
		{
			float hue = default(float);
			float sat = default(float);
			float num = default(float);
			Color.RGBToHSV(color, ref hue, ref sat, ref num);
			ColorInt glowColor = glower.GlowColor;
			glowColor.SetHueSaturation(hue, sat);
			glower.GlowColor = glowColor;
			CompGlower[] array = extraGlowers;
			foreach (CompGlower obj in array)
			{
				glowColor = obj.GlowColor;
				glowColor.SetHueSaturation(hue, sat);
				obj.GlowColor = glowColor;
			}
			Close();
		}
	}

	private static void ColorPalette(ref RectDivider layout, ref Color color, Color defaultColor, bool showDarklight, out float paletteHeight)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		using (new TextBlock((TextAnchor)3))
		{
			RectDivider rectDivider = layout;
			RectDivider rectDivider2 = rectDivider.NewCol(250f, HorizontalJustification.Right);
			int num = 26;
			RectDivider rectDivider3 = rectDivider2.NewRow(num);
			int num2 = 4;
			rectDivider3.Rect.SplitVertically(num2 * (num + 2), out var left, out var right);
			RectDivider rectDivider4 = new RectDivider(left, 195906069, (Vector2?)new Vector2(10f, 2f));
			Widgets.ColorBox(rectDivider4.NewCol(num), ref color, defaultColor);
			Widgets.Label(rectDivider4, "Default".Translate().CapitalizeFirst());
			RectDivider rectDivider5 = new RectDivider(right, 195906069, (Vector2?)new Vector2(10f, 2f));
			Color defaultDarklight = DarklightUtility.DefaultDarklight;
			Rect rect = rectDivider5.NewCol(num);
			if (showDarklight)
			{
				Widgets.ColorBox(rect, ref color, defaultDarklight);
				Widgets.Label(rectDivider5, "Darklight".Translate().CapitalizeFirst());
			}
			Widgets.ColorSelector(rectDivider2, ref color, colors, out paletteHeight);
			paletteHeight += num + 2;
		}
	}

	private void ColorTextfields(ref RectDivider layout, out Vector2 size)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		Rect rect = layout.Rect;
		RectAggregator aggregator = new RectAggregator(new Rect(((Rect)(ref rect)).position, new Vector2(125f, 0f)), 195906069);
		bool num = Widgets.ColorTextfields(ref aggregator, ref color, ref textfieldBuffers, ref textfieldColorBuffer, previousFocusedControlName, "colorTextfields", editableTextfields, visibleTextfields);
		rect = aggregator.Rect;
		size = ((Rect)(ref rect)).size;
		if (num)
		{
			float num2 = default(float);
			float num3 = default(float);
			float num4 = default(float);
			Color.RGBToHSV(color, ref num2, ref num3, ref num4);
			color = Color.HSVToRGB(num2, num3, 1f);
		}
	}

	private static void ColorReadback(Rect rect, Color color, Color oldColor, bool showDarklight)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		rect.SplitVertically((((Rect)(ref rect)).width - 26f) / 2f, out var left, out var right);
		RectDivider rectDivider = new RectDivider(left, 195906069);
		TaggedString label = "CurrentColor".Translate().CapitalizeFirst();
		TaggedString label2 = "OldColor".Translate().CapitalizeFirst();
		float width = Mathf.Max(new float[3]
		{
			100f,
			label.GetWidthCached(),
			label2.GetWidthCached()
		});
		RectDivider rectDivider2 = rectDivider.NewRow(Text.LineHeight);
		Widgets.Label(rectDivider2.NewCol(width), label);
		Widgets.DrawBoxSolid(rectDivider2, color);
		RectDivider rectDivider3 = rectDivider.NewRow(Text.LineHeight);
		Widgets.Label(rectDivider3.NewCol(width), label2);
		Widgets.DrawBoxSolid(rectDivider3, oldColor);
		RectDivider rectDivider4 = new RectDivider(right, 195906069);
		rectDivider4.NewCol(26f);
		if (showDarklight)
		{
			if (DarklightUtility.IsDarklight(color))
			{
				Widgets.Label(rectDivider4, "Darklight".Translate().CapitalizeFirst());
			}
			else
			{
				Widgets.Label(rectDivider4, "NotDarklight".Translate().CapitalizeFirst());
			}
		}
	}

	private static void TabControl()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		if ((int)Event.current.type == 4 && (int)Event.current.keyCode == 9)
		{
			bool num = !Event.current.shift;
			Event.current.Use();
			string text = GUI.GetNameOfFocusedControl();
			if (text.NullOrEmpty())
			{
				text = focusableControlNames[0];
			}
			int num2 = focusableControlNames.IndexOf(text);
			if (num2 < 0)
			{
				num2 = focusableControlNames.Count;
			}
			num2 = ((!num) ? (num2 - 1) : (num2 + 1));
			if (num2 >= focusableControlNames.Count)
			{
				num2 = 0;
			}
			else if (num2 < 0)
			{
				num2 = focusableControlNames.Count - 1;
			}
			GUI.FocusControl(focusableControlNames[num2]);
		}
	}

	public override void DoWindowContents(Rect inRect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Invalid comparison between Unknown and I4
		using (TextBlock.Default())
		{
			RectDivider layout = new RectDivider(inRect, 195906069);
			HeaderRow(ref layout);
			layout.NewRow(0f);
			BottomButtons(ref layout);
			layout.NewRow(0f, VerticalJustification.Bottom);
			float num = default(float);
			float num2 = default(float);
			float num3 = default(float);
			Color.RGBToHSV(glower.Props.glowColor.ToColor, ref num, ref num2, ref num3);
			Color defaultColor = Color.HSVToRGB(num, num2, 1f);
			defaultColor.a = 1f;
			ColorPalette(ref layout, ref color, defaultColor, ShowDarklight, out var paletteHeight);
			ColorTextfields(ref layout, out var size);
			float height = Mathf.Max(new float[3] { paletteHeight, 128f, size.y });
			RectDivider rectDivider = layout.NewRow(height);
			rectDivider.NewCol(size.x);
			rectDivider.NewCol(250f, HorizontalJustification.Right);
			Rect rect = rectDivider.Rect;
			Rect rect2 = rectDivider.Rect;
			float marginX = (((Rect)(ref rect2)).width - 128f) / 2f;
			rect2 = rectDivider.Rect;
			Widgets.HSVColorWheel(rect.ContractedBy(marginX, (((Rect)(ref rect2)).height - 128f) / 2f), ref color, ref hsvColorWheelDragging, 1f);
			layout.NewRow(10f);
			Widgets.ColorTemperatureBar(layout.NewRow(34f), ref color, ref colorTemperatureDragging, 1f);
			layout.NewRow(26f);
			ColorReadback(layout, color, oldColor, ShowDarklight);
			TabControl();
			if ((int)Event.current.type == 8)
			{
				previousFocusedControlName = GUI.GetNameOfFocusedControl();
			}
		}
	}
}
