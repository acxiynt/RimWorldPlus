using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Steam;

namespace RimWorld.Planet;

public class WorldCameraDriver : MonoBehaviour
{
	public WorldCameraConfig config = new WorldCameraConfig_Normal();

	public Quaternion sphereRotation = Quaternion.identity;

	private Vector2 rotationVelocity;

	private Vector2 desiredRotation;

	private Vector2 desiredRotationRaw;

	private float desiredAltitude;

	public float altitude;

	private List<CameraDriver.DragTimeStamp> dragTimeStamps = new List<CameraDriver.DragTimeStamp>();

	private bool releasedLeftWhileHoldingMiddle;

	private Camera cachedCamera;

	private bool mouseCoveredByUI;

	private float mouseTouchingScreenBottomEdgeStartTime = -1f;

	private float fixedTimeStepBuffer;

	private Quaternion rotationAnimation_prevSphereRotation = Quaternion.identity;

	private float rotationAnimation_lerpFactor = 1f;

	private const float SphereRadius = 100f;

	private const float ScreenDollyEdgeWidth = 20f;

	private const float ScreenDollyEdgeWidth_BottomFullscreen = 6f;

	private const float MinDurationForMouseToTouchScreenBottomEdgeToDolly = 0.28f;

	private const float MaxXRotationAtMinAltitude = 88.6f;

	private const float MaxXRotationAtMaxAltitude = 78f;

	private const float TileSizeToRotationSpeed = 0.273f;

	private const float VelocityFromMouseDragInitialFactor = 5f;

	private const float StartingAltitude_Playing = 160f;

	private const float StartingAltitude_Entry = 550f;

	private const float MaxAltitude = 1100f;

	private const float ZoomTightness = 0.4f;

	private const float ZoomScaleFromAltDenominator = 12f;

	private const float PageKeyZoomRate = 2f;

	private const float ScrollWheelZoomRate = 0.1f;

	public static float MinAltitude => 100f + (SteamDeck.IsSteamDeck ? 17f : 25f);

	private Camera MyCamera
	{
		get
		{
			if ((Object)(object)cachedCamera == (Object)null)
			{
				cachedCamera = ((Component)this).GetComponent<Camera>();
			}
			return cachedCamera;
		}
	}

	public WorldCameraZoomRange CurrentZoom
	{
		get
		{
			float altitudePercent = AltitudePercent;
			if (altitudePercent < 0.025f)
			{
				return WorldCameraZoomRange.VeryClose;
			}
			if (altitudePercent < 0.042f)
			{
				return WorldCameraZoomRange.Close;
			}
			if (altitudePercent < 0.125f)
			{
				return WorldCameraZoomRange.Far;
			}
			return WorldCameraZoomRange.VeryFar;
		}
	}

	private float ScreenDollyEdgeWidthBottom
	{
		get
		{
			if (Screen.fullScreen || ResolutionUtility.BorderlessFullscreen)
			{
				return 6f;
			}
			return 20f;
		}
	}

	private Vector3 CurrentRealPosition => ((Component)MyCamera).transform.position;

	public float AltitudePercent => Mathf.InverseLerp(MinAltitude, 1100f, altitude);

	public Vector3 CurrentlyLookingAtPointOnSphere => -(Quaternion.Inverse(sphereRotation) * Vector3.forward);

	private bool AnythingPreventsCameraMotion
	{
		get
		{
			if (!Find.WindowStack.WindowsPreventCameraMotion)
			{
				return !WorldRendererUtility.WorldRenderedNow;
			}
			return true;
		}
	}

	public void Awake()
	{
		ResetAltitude();
		ApplyPositionToGameObject();
	}

	public void WorldCameraDriverOnGUI()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Invalid comparison between Unknown and I4
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Invalid comparison between Unknown and I4
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Invalid comparison between Unknown and I4
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		if (Input.GetMouseButtonUp(0) && Input.GetMouseButton(2))
		{
			releasedLeftWhileHoldingMiddle = true;
		}
		else if ((int)Event.current.rawType == 0 || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(2))
		{
			releasedLeftWhileHoldingMiddle = false;
		}
		mouseCoveredByUI = false;
		if (Find.WindowStack.GetWindowAt(UI.MousePositionOnUIInverted) != null)
		{
			mouseCoveredByUI = true;
		}
		if (AnythingPreventsCameraMotion)
		{
			return;
		}
		if ((!UnityGUIBugsFixer.IsSteamDeckOrLinuxBuild && (int)Event.current.type == 3 && Event.current.button == 2) || (UnityGUIBugsFixer.IsSteamDeckOrLinuxBuild && Input.GetMouseButton(2) && (!SteamDeck.IsSteamDeck || !Find.WorldSelector.AnyCaravanSelected)))
		{
			Vector2 currentEventDelta = UnityGUIBugsFixer.CurrentEventDelta;
			if ((int)Event.current.type == 3)
			{
				Event.current.Use();
			}
			if (currentEventDelta != Vector2.zero)
			{
				PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.FrameInteraction);
				currentEventDelta.x *= -1f;
				desiredRotationRaw += currentEventDelta / GenWorldUI.CurUITileSize() * 0.273f * Prefs.MapDragSensitivity;
			}
		}
		float num = 0f;
		if ((int)Event.current.type == 6)
		{
			num -= Event.current.delta.y * 0.1f;
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
		}
		if (KeyBindingDefOf.MapZoom_In.KeyDownEvent)
		{
			num += 2f;
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
		}
		if (KeyBindingDefOf.MapZoom_Out.KeyDownEvent)
		{
			num -= 2f;
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
		}
		desiredAltitude -= num * config.zoomSpeed * altitude / 12f;
		desiredAltitude = Mathf.Clamp(desiredAltitude, MinAltitude, 1100f);
		desiredRotation = Vector2.zero;
		if (KeyBindingDefOf.MapDolly_Left.IsDown)
		{
			desiredRotation.x = 0f - config.dollyRateKeys;
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
		}
		if (KeyBindingDefOf.MapDolly_Right.IsDown)
		{
			desiredRotation.x = config.dollyRateKeys;
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
		}
		if (KeyBindingDefOf.MapDolly_Up.IsDown)
		{
			desiredRotation.y = config.dollyRateKeys;
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
		}
		if (KeyBindingDefOf.MapDolly_Down.IsDown)
		{
			desiredRotation.y = 0f - config.dollyRateKeys;
			PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.WorldCameraMovement, KnowledgeAmount.SpecificInteraction);
		}
		config.ConfigOnGUI();
	}

	public void Update()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		if (LongEventHandler.ShouldWaitForEvent)
		{
			return;
		}
		if (Find.World == null)
		{
			((Component)MyCamera).gameObject.SetActive(false);
			return;
		}
		if (!Find.WorldInterface.everReset)
		{
			Find.WorldInterface.Reset();
		}
		Vector2 val = CalculateCurInputDollyVect();
		if (val != Vector2.zero)
		{
			float num = (altitude - MinAltitude) / (1100f - MinAltitude) * 0.85f + 0.15f;
			rotationVelocity = new Vector2(val.x, val.y) * num;
		}
		if ((!Input.GetMouseButton(2) || (SteamDeck.IsSteamDeck && releasedLeftWhileHoldingMiddle)) && dragTimeStamps.Any())
		{
			rotationVelocity += CameraDriver.GetExtraVelocityFromReleasingDragButton(dragTimeStamps, 5f);
			dragTimeStamps.Clear();
		}
		if (!AnythingPreventsCameraMotion)
		{
			float num2 = Time.deltaTime * CameraDriver.HitchReduceFactor;
			sphereRotation *= Quaternion.AngleAxis(rotationVelocity.x * num2 * config.rotationSpeedScale, ((Component)MyCamera).transform.up);
			sphereRotation *= Quaternion.AngleAxis((0f - rotationVelocity.y) * num2 * config.rotationSpeedScale, ((Component)MyCamera).transform.right);
			if (desiredRotationRaw != Vector2.zero)
			{
				sphereRotation *= Quaternion.AngleAxis(desiredRotationRaw.x, ((Component)MyCamera).transform.up);
				sphereRotation *= Quaternion.AngleAxis(0f - desiredRotationRaw.y, ((Component)MyCamera).transform.right);
			}
			dragTimeStamps.Add(new CameraDriver.DragTimeStamp
			{
				posDelta = desiredRotationRaw,
				time = Time.time
			});
		}
		desiredRotationRaw = Vector2.zero;
		int num3 = Gen.FixedTimeStepUpdate(ref fixedTimeStepBuffer, 60f);
		for (int i = 0; i < num3; i++)
		{
			if (rotationVelocity != Vector2.zero)
			{
				rotationVelocity *= config.camRotationDecayFactor;
				if (((Vector2)(ref rotationVelocity)).magnitude < 0.05f)
				{
					rotationVelocity = Vector2.zero;
				}
			}
			if (config.smoothZoom)
			{
				float num4 = Mathf.Lerp(altitude, desiredAltitude, 0.05f);
				desiredAltitude += (num4 - altitude) * config.zoomPreserveFactor;
				altitude = num4;
			}
			else
			{
				float num5 = (desiredAltitude - altitude) * 0.4f;
				desiredAltitude += config.zoomPreserveFactor * num5;
				altitude += num5;
			}
		}
		rotationAnimation_lerpFactor += Time.deltaTime * 8f;
		if (Find.PlaySettings.lockNorthUp)
		{
			RotateSoNorthIsUp(interpolate: false);
			ClampXRotation(ref sphereRotation);
		}
		for (int j = 0; j < num3; j++)
		{
			config.ConfigFixedUpdate_60(ref rotationVelocity);
		}
		ApplyPositionToGameObject();
	}

	private void ApplyPositionToGameObject()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		Quaternion invRot = ((!(rotationAnimation_lerpFactor < 1f)) ? sphereRotation : Quaternion.Lerp(rotationAnimation_prevSphereRotation, sphereRotation, rotationAnimation_lerpFactor));
		if (Find.PlaySettings.lockNorthUp)
		{
			ClampXRotation(ref invRot);
		}
		((Component)MyCamera).transform.rotation = Quaternion.Inverse(invRot);
		Vector3 val = ((Component)MyCamera).transform.rotation * Vector3.forward;
		((Component)MyCamera).transform.position = -val * altitude;
	}

	private Vector2 CalculateCurInputDollyVect()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = desiredRotation;
		bool flag = false;
		if ((UnityData.isEditor || Screen.fullScreen || ResolutionUtility.BorderlessFullscreen) && Prefs.EdgeScreenScroll && !mouseCoveredByUI)
		{
			Vector2 mousePositionOnUI = UI.MousePositionOnUI;
			Vector2 mousePositionOnUIInverted = UI.MousePositionOnUIInverted;
			Rect val2 = default(Rect);
			((Rect)(ref val2))._002Ector((float)(UI.screenWidth - 250), 0f, 255f, 255f);
			Rect val3 = default(Rect);
			((Rect)(ref val3))._002Ector(0f, (float)(UI.screenHeight - 250), 225f, 255f);
			Rect val4 = default(Rect);
			((Rect)(ref val4))._002Ector((float)(UI.screenWidth - 250), (float)(UI.screenHeight - 250), 255f, 255f);
			WorldInspectPane inspectPane = Find.World.UI.inspectPane;
			if (Find.WindowStack.IsOpen<WorldInspectPane>() && inspectPane.RecentHeight > ((Rect)(ref val3)).height)
			{
				((Rect)(ref val3)).yMin = (float)UI.screenHeight - inspectPane.RecentHeight;
			}
			if (!((Rect)(ref val3)).Contains(mousePositionOnUIInverted) && !((Rect)(ref val4)).Contains(mousePositionOnUIInverted) && !((Rect)(ref val2)).Contains(mousePositionOnUIInverted))
			{
				Vector2 zero = Vector2.zero;
				if (mousePositionOnUI.x >= 0f && mousePositionOnUI.x < 20f)
				{
					zero.x -= config.dollyRateScreenEdge;
				}
				if (mousePositionOnUI.x <= (float)UI.screenWidth && mousePositionOnUI.x > (float)UI.screenWidth - 20f)
				{
					zero.x += config.dollyRateScreenEdge;
				}
				if (mousePositionOnUI.y <= (float)UI.screenHeight && mousePositionOnUI.y > (float)UI.screenHeight - 20f)
				{
					zero.y += config.dollyRateScreenEdge;
				}
				if (mousePositionOnUI.y >= 0f && mousePositionOnUI.y < ScreenDollyEdgeWidthBottom)
				{
					if (mouseTouchingScreenBottomEdgeStartTime < 0f)
					{
						mouseTouchingScreenBottomEdgeStartTime = Time.realtimeSinceStartup;
					}
					if (Time.realtimeSinceStartup - mouseTouchingScreenBottomEdgeStartTime >= 0.28f)
					{
						zero.y -= config.dollyRateScreenEdge;
					}
					flag = true;
				}
				val += zero;
			}
		}
		if (!flag)
		{
			mouseTouchingScreenBottomEdgeStartTime = -1f;
		}
		if (Input.GetKey((KeyCode)304))
		{
			val *= 2.4f;
		}
		return val;
	}

	public void ResetAltitude()
	{
		if (Current.ProgramState == ProgramState.Playing)
		{
			altitude = 160f;
		}
		else
		{
			altitude = 550f;
		}
		desiredAltitude = altitude;
	}

	public void JumpTo(Vector3 newLookAt)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!Find.WorldInterface.everReset)
		{
			Find.WorldInterface.Reset();
		}
		sphereRotation = Quaternion.Inverse(Quaternion.LookRotation(-((Vector3)(ref newLookAt)).normalized));
	}

	public void JumpTo(int tile)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		JumpTo(Find.WorldGrid.GetTileCenter(tile));
	}

	public void RotateSoNorthIsUp(bool interpolate = true)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (interpolate)
		{
			rotationAnimation_prevSphereRotation = sphereRotation;
		}
		sphereRotation = Quaternion.Inverse(Quaternion.LookRotation(Quaternion.Inverse(sphereRotation) * Vector3.forward));
		if (interpolate)
		{
			rotationAnimation_lerpFactor = 0f;
		}
	}

	private void ClampXRotation(ref Quaternion invRot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.Inverse(invRot);
		Vector3 eulerAngles = ((Quaternion)(ref val)).eulerAngles;
		float altitudePercent = AltitudePercent;
		float num = Mathf.Lerp(88.6f, 78f, altitudePercent);
		bool flag = false;
		if (eulerAngles.x <= 90f)
		{
			if (eulerAngles.x > num)
			{
				eulerAngles.x = num;
				flag = true;
			}
		}
		else if (eulerAngles.x < 360f - num)
		{
			eulerAngles.x = 360f - num;
			flag = true;
		}
		if (flag)
		{
			invRot = Quaternion.Inverse(Quaternion.Euler(eulerAngles));
		}
	}
}
