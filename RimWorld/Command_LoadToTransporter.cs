using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld;

[StaticConstructorOnStartup]
public class Command_LoadToTransporter : Command
{
	public CompTransporter transComp;

	private List<CompTransporter> transporters;

	private static HashSet<Building> tmpFuelingPortGivers = new HashSet<Building>();

	public override void ProcessInput(Event ev)
	{
		base.ProcessInput(ev);
		if (transporters == null)
		{
			transporters = new List<CompTransporter>();
		}
		if (!transporters.Contains(transComp))
		{
			transporters.Add(transComp);
		}
		CompLaunchable launchable = transComp.Launchable;
		if (launchable != null)
		{
			Building fuelingPortSource = launchable.FuelingPortSource;
			if (fuelingPortSource != null)
			{
				Map map = transComp.Map;
				tmpFuelingPortGivers.Clear();
				map.floodFiller.FloodFill(fuelingPortSource.Position, (IntVec3 x) => FuelingPortUtility.AnyFuelingPortGiverAt(x, map), delegate(IntVec3 x)
				{
					tmpFuelingPortGivers.Add(FuelingPortUtility.FuelingPortGiverAt(x, map));
				});
				for (int num = 0; num < transporters.Count; num++)
				{
					Building fuelingPortSource2 = transporters[num].Launchable.FuelingPortSource;
					if (fuelingPortSource2 != null && !tmpFuelingPortGivers.Contains(fuelingPortSource2))
					{
						Messages.Message("MessageTransportersNotAdjacent".Translate(), fuelingPortSource2, MessageTypeDefOf.RejectInput, historical: false);
						return;
					}
				}
			}
		}
		for (int num2 = 0; num2 < transporters.Count; num2++)
		{
			if (transporters[num2] != transComp && !transComp.Map.reachability.CanReach(transComp.parent.Position, transporters[num2].parent, PathEndMode.Touch, TraverseParms.For(TraverseMode.PassDoors)))
			{
				Messages.Message("MessageTransporterUnreachable".Translate(), transporters[num2].parent, MessageTypeDefOf.RejectInput, historical: false);
				return;
			}
		}
		Dialog_LoadTransporters dialog_LoadTransporters = new Dialog_LoadTransporters(transComp.Map, transporters);
		dialog_LoadTransporters.autoLoot = transComp.Shuttle != null && transComp.Shuttle.CanAutoLoot;
		Find.WindowStack.Add(dialog_LoadTransporters);
	}

	public override bool InheritInteractionsFrom(Gizmo other)
	{
		if (transComp.Props.max1PerGroup)
		{
			return false;
		}
		Command_LoadToTransporter command_LoadToTransporter = (Command_LoadToTransporter)other;
		if (command_LoadToTransporter.transComp.parent.def != transComp.parent.def)
		{
			return false;
		}
		if (transporters == null)
		{
			transporters = new List<CompTransporter>();
		}
		transporters.Add(command_LoadToTransporter.transComp);
		return false;
	}
}
