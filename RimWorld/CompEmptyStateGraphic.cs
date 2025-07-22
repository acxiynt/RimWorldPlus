using UnityEngine;
using Verse;

namespace RimWorld;

public class CompEmptyStateGraphic : ThingComp
{
	private CompProperties_EmptyStateGraphic Props => (CompProperties_EmptyStateGraphic)props;

	public bool ParentIsEmpty
	{
		get
		{
			if (parent is Building_Casket { HasAnyContents: false })
			{
				return true;
			}
			CompPawnSpawnOnWakeup compPawnSpawnOnWakeup = parent.TryGetComp<CompPawnSpawnOnWakeup>();
			if (compPawnSpawnOnWakeup != null && !compPawnSpawnOnWakeup.CanSpawn)
			{
				return true;
			}
			return false;
		}
	}

	public override void PostDraw()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		base.PostDraw();
		if (ParentIsEmpty)
		{
			Mesh obj = Props.graphicData.Graphic.MeshAt(parent.Rotation);
			Vector3 drawPos = parent.DrawPos;
			drawPos.y = AltitudeLayer.BuildingOnTop.AltitudeFor();
			Graphics.DrawMesh(obj, drawPos + Props.graphicData.drawOffset.RotatedBy(parent.Rotation), Quaternion.identity, Props.graphicData.Graphic.MatAt(parent.Rotation), 0);
		}
	}
}
