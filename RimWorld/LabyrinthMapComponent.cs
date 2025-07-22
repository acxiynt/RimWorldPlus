using System.Collections.Generic;
using System.Linq;
using LudeonTK;
using UnityEngine;
using Verse;

namespace RimWorld;

public class LabyrinthMapComponent : CustomMapComponent
{
	public Building labyrinthObelisk;

	public Building abductorObelisk;

	private LayoutStructureSketch structureSketch;

	private List<LayoutRoom> spawnableRooms;

	private bool closing;

	private int nextTeleportTick;

	private static readonly IntRange TeleportDelayTicks = new IntRange(6, 60);

	private const int IntervalCheckCloseTicks = 300;

	private static readonly List<IntVec3> tmpCells = new List<IntVec3>();

	public LabyrinthMapComponent(Map map)
		: base(map)
	{
	}

	public override void MapComponentTick()
	{
		if (!closing && GenTicks.IsTickInterval(300) && abductorObelisk.DestroyedOrNull() && !map.mapPawns.AnyColonistSpawned)
		{
			PocketMapUtility.DestroyPocketMap(map);
		}
		TeleportPawnsClosing();
	}

	private void TeleportPawnsClosing()
	{
		if (!closing || GenTicks.TicksGame < nextTeleportTick)
		{
			return;
		}
		Map dest = null;
		nextTeleportTick = GenTicks.TicksGame + TeleportDelayTicks.RandomInRange;
		foreach (Map map in Find.Maps)
		{
			if (map.IsPlayerHome)
			{
				dest = map;
				break;
			}
		}
		if (dest == null || !CellFinderLoose.TryGetRandomCellWith((IntVec3 pos) => IsValidTeleportCell(pos, dest), dest, 1000, out var result))
		{
			return;
		}
		using (List<Pawn>.Enumerator enumerator2 = base.map.mapPawns.AllPawns.GetEnumerator())
		{
			if (enumerator2.MoveNext())
			{
				Pawn current2 = enumerator2.Current;
				if (SkipUtility.SkipTo(current2, result, dest) is Pawn pawn && PawnUtility.ShouldSendNotificationAbout(pawn))
				{
					Messages.Message("MessagePawnReappeared".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.NeutralEvent, historical: false);
				}
				current2.inventory.UnloadEverything = true;
				return;
			}
		}
		foreach (Thing item in (IEnumerable<Thing>)base.map.spawnedThings)
		{
			if (item.def.category == ThingCategory.Item)
			{
				SkipUtility.SkipTo(item, result, dest);
				return;
			}
		}
		Find.LetterStack.ReceiveLetter("LetterLabelLabyrinthExit".Translate(), "LetterLabyrinthExit".Translate(), LetterDefOf.NeutralEvent);
		PocketMapUtility.DestroyPocketMap(base.map);
		if (abductorObelisk != null)
		{
			abductorObelisk.GetComp<CompObelisk_Abductor>().Notify_MapDestroyed();
			if (abductorObelisk.Spawned)
			{
				EffecterDefOf.Skip_EntryNoDelay.Spawn(abductorObelisk.Position, abductorObelisk.Map, 2f).Cleanup();
				abductorObelisk.Destroy();
			}
		}
	}

	private static bool IsValidTeleportCell(IntVec3 pos, Map dest)
	{
		if (!pos.Fogged(dest) && pos.Standable(dest))
		{
			return dest.reachability.CanReachColony(pos);
		}
		return false;
	}

	public override void MapComponentOnGUI()
	{
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		if (DebugViewSettings.drawMapGraphs)
		{
			foreach (KeyValuePair<Vector2, List<Vector2>> connection in map.layoutStructureSketch.structureLayout.neighbours.connections)
			{
				foreach (Vector2 item in connection.Value)
				{
					Vector2 val = new Vector2(2f, 2f);
					Vector2 val2 = val + connection.Key;
					Vector2 val3 = val + item;
					Vector2 start = UI.MapToUIPosition(new Vector3(val2.x, 0f, val2.y));
					Vector2 end = UI.MapToUIPosition(new Vector3(val3.x, 0f, val3.y));
					DevGUI.DrawLine(start, end, Color.green, 2f);
				}
			}
		}
		if (!DebugViewSettings.drawMapRooms)
		{
			return;
		}
		foreach (LayoutRoom room in map.layoutStructureSketch.structureLayout.Rooms)
		{
			string text = "NA";
			if (!room.defs.NullOrEmpty())
			{
				text = room.defs.Select((LayoutRoomDef x) => x.defName).ToCommaList();
			}
			float widthCached = text.GetWidthCached();
			Vector2 val4 = (room.rects[0].Min + IntVec3.NorthEast * 2).ToVector3().MapToUIPosition();
			DevGUI.Label(new Rect(val4.x - widthCached / 2f, val4.y, widthCached, 20f), text);
			foreach (CellRect rect in room.rects)
			{
				IntVec3 min = rect.Min;
				IntVec3 intVec = rect.Max + new IntVec3(1, 0, 1);
				IntVec3 a = new IntVec3(min.x, 0, min.z);
				IntVec3 intVec2 = new IntVec3(intVec.x, 0, min.z);
				IntVec3 intVec3 = new IntVec3(min.x, 0, intVec.z);
				IntVec3 b = new IntVec3(intVec.x, 0, intVec.z);
				TryDrawLine(a, intVec2);
				TryDrawLine(a, intVec3);
				TryDrawLine(intVec3, b);
				TryDrawLine(intVec2, b);
			}
		}
	}

	private void TryDrawLine(IntVec3 a, IntVec3 b)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Vector2 start = a.ToVector3().MapToUIPosition();
		Vector2 end = b.ToVector3().MapToUIPosition();
		DevGUI.DrawLine(start, end, Color.blue, 2f);
	}

	public void SetSpawnRooms(List<LayoutRoom> rooms)
	{
		spawnableRooms = rooms;
	}

	public void StartClosing()
	{
		closing = true;
	}

	public Thing TeleportToLabyrinth(Thing thing)
	{
		IntVec3 dropPosition = GetDropPosition();
		Thing thing2 = SkipUtility.SkipTo(thing, dropPosition, map);
		if (thing is Pawn pawn)
		{
			pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtDefOf.ObeliskAbduction);
			if (PawnUtility.ShouldSendNotificationAbout(pawn))
			{
				Messages.Message("MessagePawnVanished".Translate(pawn.Named("PAWN")), thing2, MessageTypeDefOf.NeutralEvent, historical: false);
			}
		}
		return thing2;
	}

	private IntVec3 GetDropPosition()
	{
		foreach (CellRect rect in spawnableRooms.RandomElement().rects)
		{
			tmpCells.AddRange(rect.ContractedBy(2));
		}
		IntVec3 root = tmpCells.RandomElement();
		tmpCells.Clear();
		return CellFinder.StandableCellNear(root, map, 5f);
	}

	public override void ExposeData()
	{
		base.ExposeData();
		Scribe_Values.Look(ref closing, "closing", defaultValue: false);
		Scribe_Values.Look(ref nextTeleportTick, "nextTeleportTick", 0);
		Scribe_References.Look(ref labyrinthObelisk, "labyrinthObelisk");
		Scribe_References.Look(ref abductorObelisk, "abductorObelisk");
		Scribe_Deep.Look(ref structureSketch, "structureSketch");
		Scribe_Collections.Look(ref spawnableRooms, "spawnableRects", LookMode.Deep);
	}
}
