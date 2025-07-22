using System.Collections.Generic;
using System.Text;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld;

public class Page_SelectStartingSite : Page
{
	private const float GapBetweenBottomButtons = 10f;

	private const float UseTwoRowsIfScreenWidthBelowBase = 540f;

	private static List<Vector3> tmpTileVertices = new List<Vector3>();

	private int? tutorialStartTilePatch;

	public override string PageTitle => "SelectStartingSite".TranslateWithBackup("SelectLandingSite");

	public override Vector2 InitialSize => Vector2.zero;

	protected override float Margin => 0f;

	public Page_SelectStartingSite()
	{
		absorbInputAroundWindow = false;
		shadowAlpha = 0f;
		preventCameraMotion = false;
	}

	public override void PreOpen()
	{
		base.PreOpen();
		Find.World.renderer.wantedMode = WorldRenderMode.Planet;
		Find.WorldInterface.Reset();
		((MainButtonWorker_ToggleWorld)MainButtonDefOf.World.Worker).resetViewNextTime = true;
	}

	public override void PostOpen()
	{
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		base.PostOpen();
		Find.GameInitData.ChooseRandomStartingTile();
		LessonAutoActivator.TeachOpportunity(ConceptDefOf.WorldCameraMovement, OpportunityType.Important);
		TutorSystem.Notify_Event("PageStart-SelectStartingSite");
		tutorialStartTilePatch = null;
		if (!TutorSystem.TutorialMode || Find.Tutor.activeLesson == null || Find.Tutor.activeLesson.Current == null || Find.Tutor.activeLesson.Current.Instruction != InstructionDefOf.ChooseLandingSite)
		{
			return;
		}
		Find.WorldCameraDriver.ResetAltitude();
		Find.WorldCameraDriver.Update();
		List<int> list = new List<int>();
		float[] array = new float[Find.WorldGrid.TilesCount];
		WorldGrid worldGrid = Find.WorldGrid;
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector((float)Screen.width / 2f, (float)Screen.height / 2f);
		float num = Vector2.Distance(val, Vector2.zero);
		for (int i = 0; i < worldGrid.TilesCount; i++)
		{
			Tile tile = worldGrid[i];
			if (TutorSystem.AllowAction("ChooseBiome-" + tile.biome.defName + "-" + tile.hilliness))
			{
				tmpTileVertices.Clear();
				worldGrid.GetTileVertices(i, tmpTileVertices);
				Vector3 val2 = Vector3.zero;
				for (int j = 0; j < tmpTileVertices.Count; j++)
				{
					val2 += tmpTileVertices[j];
				}
				val2 /= (float)tmpTileVertices.Count;
				Vector3 val3 = Find.WorldCamera.WorldToScreenPoint(val2) / Prefs.UIScale;
				val3.y = (float)UI.screenHeight - val3.y;
				val3.x = Mathf.Clamp(val3.x, 0f, (float)UI.screenWidth);
				val3.y = Mathf.Clamp(val3.y, 0f, (float)UI.screenHeight);
				float num2 = 1f - Vector2.Distance(val, Vector2.op_Implicit(val3)) / num;
				Vector3 val4 = val2 - ((Component)Find.WorldCamera).transform.position;
				Vector3 normalized = ((Vector3)(ref val4)).normalized;
				float num3 = Vector3.Dot(((Component)Find.WorldCamera).transform.forward, normalized);
				array[i] = num2 * num3;
			}
			else
			{
				array[i] = float.NegativeInfinity;
			}
		}
		for (int k = 0; k < 16; k++)
		{
			for (int l = 0; l < array.Length; l++)
			{
				list.Clear();
				worldGrid.GetTileNeighbors(l, list);
				float num4 = array[l];
				if (num4 < 0f)
				{
					continue;
				}
				for (int m = 0; m < list.Count; m++)
				{
					float num5 = array[list[m]];
					if (!(num5 < 0f))
					{
						num4 += num5;
					}
				}
				array[l] = num4 / (float)list.Count;
			}
		}
		float num6 = float.NegativeInfinity;
		int num7 = -1;
		for (int n = 0; n < array.Length; n++)
		{
			if (array[n] > 0f && num6 < array[n])
			{
				num6 = array[n];
				num7 = n;
			}
		}
		if (num7 != -1)
		{
			tutorialStartTilePatch = num7;
		}
	}

	public override void PostClose()
	{
		base.PostClose();
		Find.World.renderer.wantedMode = WorldRenderMode.None;
	}

	public override void DoWindowContents(Rect rect)
	{
		if (Find.WorldInterface.SelectedTile >= 0)
		{
			Find.GameInitData.startingTile = Find.WorldInterface.SelectedTile;
		}
		else if (Find.WorldSelector.FirstSelectedObject != null)
		{
			Find.GameInitData.startingTile = Find.WorldSelector.FirstSelectedObject.Tile;
		}
	}

	public override void ExtraOnGUI()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		base.ExtraOnGUI();
		Text.Anchor = (TextAnchor)1;
		DrawPageTitle(new Rect(0f, 5f, (float)UI.screenWidth, 300f));
		Text.Anchor = (TextAnchor)0;
		DoCustomBottomButtons();
		if (tutorialStartTilePatch.HasValue)
		{
			tmpTileVertices.Clear();
			Find.WorldGrid.GetTileVertices(tutorialStartTilePatch.Value, tmpTileVertices);
			Vector3 val = Vector3.zero;
			for (int i = 0; i < tmpTileVertices.Count; i++)
			{
				val += tmpTileVertices[i];
			}
			Color color = GUI.color;
			GUI.color = Color.white;
			GenUI.DrawArrowPointingAtWorldspace(val / (float)tmpTileVertices.Count, Find.WorldCamera);
			GUI.color = color;
		}
	}

	protected override bool CanDoNext()
	{
		if (!base.CanDoNext())
		{
			return false;
		}
		int selectedTile = Find.WorldInterface.SelectedTile;
		if (selectedTile < 0)
		{
			Messages.Message("MustSelectStartingSite".TranslateWithBackup("MustSelectLandingSite"), MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (!TileFinder.IsValidTileForNewSettlement(selectedTile, stringBuilder))
		{
			Messages.Message(stringBuilder.ToString(), MessageTypeDefOf.RejectInput, historical: false);
			return false;
		}
		Tile tile = Find.WorldGrid[selectedTile];
		if (!TutorSystem.AllowAction("ChooseBiome-" + tile.biome.defName + "-" + tile.hilliness))
		{
			return false;
		}
		return true;
	}

	protected override void DoNext()
	{
		int selTile = Find.WorldInterface.SelectedTile;
		SettlementProximityGoodwillUtility.CheckConfirmSettle(selTile, delegate
		{
			Find.GameInitData.startingTile = selTile;
			base.DoNext();
		});
	}

	private void DoCustomBottomButtons()
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		int num = (TutorSystem.TutorialMode ? 4 : 5);
		int num2 = ((num < 4 || !((float)UI.screenWidth < 540f + (float)num * (Page.BottomButSize.x + 10f))) ? 1 : 2);
		int num3 = Mathf.CeilToInt((float)num / (float)num2);
		float num4 = Page.BottomButSize.x * (float)num3 + 10f * (float)(num3 + 1);
		float num5 = (float)num2 * Page.BottomButSize.y + 10f * (float)(num2 + 1);
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(((float)UI.screenWidth - num4) / 2f, (float)UI.screenHeight - num5 - 4f, num4, num5);
		WorldInspectPane worldInspectPane = Find.WindowStack.WindowOfType<WorldInspectPane>();
		if (worldInspectPane != null && ((Rect)(ref val)).x < InspectPaneUtility.PaneWidthFor(worldInspectPane) + 4f)
		{
			((Rect)(ref val)).x = InspectPaneUtility.PaneWidthFor(worldInspectPane) + 4f;
		}
		Widgets.DrawWindowBackground(val);
		float num6 = ((Rect)(ref val)).xMin + 10f;
		float num7 = ((Rect)(ref val)).yMin + 10f;
		Text.Font = GameFont.Small;
		if ((Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "Back".Translate()) || KeyBindingDefOf.Cancel.KeyDownEvent) && CanDoBack())
		{
			DoBack();
		}
		num6 += Page.BottomButSize.x + 10f;
		if (!TutorSystem.TutorialMode)
		{
			if (Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "Advanced".Translate()))
			{
				Find.WindowStack.Add(new Dialog_AdvancedGameConfig(Find.WorldInterface.SelectedTile));
			}
			num6 += Page.BottomButSize.x + 10f;
		}
		if (Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "SelectRandomSite".Translate()))
		{
			SoundDefOf.Click.PlayOneShotOnCamera();
			Find.WorldInterface.SelectedTile = TileFinder.RandomStartingTile();
			Find.WorldCameraDriver.JumpTo(Find.WorldGrid.GetTileCenter(Find.WorldInterface.SelectedTile));
		}
		num6 += Page.BottomButSize.x + 10f;
		if (num2 == 2)
		{
			num6 = ((Rect)(ref val)).xMin + 10f;
			num7 += Page.BottomButSize.y + 10f;
		}
		if (Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "WorldFactionsTab".Translate()))
		{
			Find.WindowStack.Add(new Dialog_FactionDuringLanding());
		}
		num6 += Page.BottomButSize.x + 10f;
		if (Widgets.ButtonText(new Rect(num6, num7, Page.BottomButSize.x, Page.BottomButSize.y), "Next".Translate()) && CanDoNext())
		{
			DoNext();
		}
		num6 += Page.BottomButSize.x + 10f;
		GenUI.AbsorbClicksInRect(val);
	}
}
