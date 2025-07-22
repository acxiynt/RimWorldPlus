using System;
using UnityEngine;

namespace Verse;

[StaticConstructorOnStartup]
public class Dialog_CameraConfig : Window
{
	private static readonly FloatRange MoveScaleFactorRange = new FloatRange(0f, 2f);

	private static readonly FloatRange ZoomScaleFactorRange = new FloatRange(0.1f, 10f);

	private const float SliderHeight = 30f;

	private static readonly Texture2D ArrowTex = ContentFinder<Texture2D>.Get("UI/Overlays/TutorArrowRight");

	public override Vector2 InitialSize => new Vector2(260f, 300f);

	private CameraMapConfig Config => Find.CameraDriver.config;

	protected override float Margin => 4f;

	public Dialog_CameraConfig()
	{
		closeOnAccept = false;
		closeOnCancel = false;
		draggable = true;
		layer = WindowLayer.Super;
		doCloseX = true;
		onlyOneOfTypeAllowed = true;
		preventCameraMotion = false;
		focusWhenOpened = false;
		drawShadow = false;
		drawInScreenshotMode = false;
		Reset();
	}

	public override void DoWindowContents(Rect rect)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Invalid comparison between Unknown and I4
		Text.Font = GameFont.Small;
		Widgets.Label(new Rect(4f, 0f, ((Rect)(ref rect)).width, 30f), "Camera config");
		Rect rect2 = default(Rect);
		((Rect)(ref rect2))._002Ector(4f, 36f, ((Rect)(ref rect)).width - 8f, 30f);
		Widgets.HorizontalSlider(rect2, ref Config.moveSpeedScale, MoveScaleFactorRange, "Pan speed " + Config.moveSpeedScale, 0.005f);
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 36f;
		Widgets.HorizontalSlider(rect2, ref Config.zoomSpeed, ZoomScaleFactorRange, "Zoom speed " + Config.zoomSpeed, 0.1f);
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 36f;
		Widgets.FloatRange(rect2, GetHashCode(), ref Config.sizeRange, 0f, 100f, "ZoomRange", ToStringStyle.FloatOne, 1f);
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 36f;
		bool checkOn = Config.zoomPreserveFactor > 0f;
		Widgets.CheckboxLabeled(rect2, "Continuous zoom", ref checkOn);
		Config.zoomPreserveFactor = (checkOn ? 1f : 0f);
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 30f;
		Widgets.CheckboxLabeled(rect2, "Smooth zoom", ref Config.smoothZoom);
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 30f;
		Widgets.CheckboxLabeled(rect2, "Follow selected pawns", ref Config.followSelected);
		((Rect)(ref rect2)).y = ((Rect)(ref rect2)).y + 30f;
		Widgets.CheckboxLabeled(rect2, "Auto pan while paused", ref Config.autoPanWhilePaused);
		Rect val = new Rect(4f, ((Rect)(ref rect2)).yMax, ((Rect)(ref rect)).width - 8f, 9999f);
		float num = 0f;
		GUI.BeginGroup(val);
		Rect val2 = default(Rect);
		((Rect)(ref val2))._002Ector((((Rect)(ref rect)).width - 8f) / 2f - 15f, 0f, 30f, 30f);
		Widgets.DrawTextureRotated(((Rect)(ref val2)).center, (Texture)(object)ArrowTex, (0f - Config.autoPanTargetAngle) * 57.29578f, 0.4f);
		Rect rect3 = default(Rect);
		((Rect)(ref rect3))._002Ector(0f, ((Rect)(ref val2)).yMax + 3f, ((Rect)(ref rect)).width - 8f, 30f);
		float autoPanTargetAngle = Config.autoPanTargetAngle;
		autoPanTargetAngle = Widgets.HorizontalSlider(rect3, autoPanTargetAngle, 0f, (float)Math.PI * 2f, middleAlignment: false, "Auto pan angle " + (autoPanTargetAngle * 57.29578f).ToString("F0") + "°", "0°", "360°", 0.01f);
		if (autoPanTargetAngle != Config.autoPanTargetAngle)
		{
			Config.autoPanTargetAngle = (Config.autoPanAngle = autoPanTargetAngle);
		}
		num = ((Rect)(ref rect3)).yMax;
		Rect rect4 = default(Rect);
		((Rect)(ref rect4))._002Ector(0f, num + 6f, ((Rect)(ref rect)).width - 8f, 30f);
		float autoPanSpeed = Config.autoPanSpeed;
		autoPanSpeed = Widgets.HorizontalSlider(rect4, autoPanSpeed, 0f, 5f, middleAlignment: false, "Auto pan speed " + Config.autoPanSpeed, "0", "10", 0.05f);
		if (autoPanSpeed != Config.autoPanSpeed)
		{
			Config.autoPanSpeed = autoPanSpeed;
		}
		num = ((Rect)(ref rect4)).yMax;
		GUI.EndGroup();
		Rect val3 = default(Rect);
		((Rect)(ref val3))._002Ector(0f, ((Rect)(ref rect2)).yMax + num + 10f, ((Rect)(ref rect)).width, 30f);
		Rect rect5 = val3;
		((Rect)(ref rect5)).xMax = ((Rect)(ref val3)).width / 3f;
		if (Widgets.ButtonText(rect5, "Reset"))
		{
			Reset();
		}
		((Rect)(ref rect5)).x = ((Rect)(ref rect5)).x + ((Rect)(ref val3)).width / 3f;
		if (Widgets.ButtonText(rect5, "Save"))
		{
			Find.WindowStack.Add(new Dialog_CameraConfigList_Save(Config));
		}
		((Rect)(ref rect5)).x = ((Rect)(ref rect5)).x + ((Rect)(ref val3)).width / 3f;
		if (Widgets.ButtonText(rect5, "Load"))
		{
			Find.WindowStack.Add(new Dialog_CameraConfigList_Load(delegate(CameraMapConfig c)
			{
				Config.moveSpeedScale = c.moveSpeedScale;
				Config.zoomSpeed = c.zoomSpeed;
				Config.sizeRange = c.sizeRange;
				Config.zoomPreserveFactor = c.zoomPreserveFactor;
				Config.smoothZoom = c.smoothZoom;
				Config.followSelected = c.followSelected;
				Config.autoPanTargetAngle = (Config.autoPanAngle = c.autoPanTargetAngle);
				Config.autoPanSpeed = c.autoPanSpeed;
				Config.fileName = c.fileName;
				Config.autoPanWhilePaused = c.autoPanWhilePaused;
			}));
		}
		if ((int)Event.current.type == 8)
		{
			((Rect)(ref windowRect)).height = ((Rect)(ref val3)).yMax + Margin * 2f;
		}
	}

	private void Reset()
	{
		Find.CameraDriver.config = new CameraMapConfig_Normal();
	}

	protected override void SetInitialSizeAndPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Vector2 initialSize = InitialSize;
		windowRect = GenUI.Rounded(new Rect(5f, 5f, initialSize.x, initialSize.y));
	}
}
