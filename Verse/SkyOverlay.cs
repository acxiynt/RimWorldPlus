using UnityEngine;

namespace Verse;

public abstract class SkyOverlay
{
	public Material worldOverlayMat;

	public Material screenOverlayMat;

	public bool forceOverlayColor;

	public Color forcedColor;

	protected float worldOverlayPanSpeed1;

	protected float worldOverlayPanSpeed2;

	protected Vector2 worldPanDir1;

	protected Vector2 worldPanDir2;

	public Color OverlayColor
	{
		set
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)worldOverlayMat != (Object)null)
			{
				worldOverlayMat.color = value;
			}
			if ((Object)(object)screenOverlayMat != (Object)null)
			{
				screenOverlayMat.color = value;
			}
		}
	}

	public SkyOverlay()
	{
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			OverlayColor = Color.clear;
		});
	}

	public virtual void TickOverlay(Map map)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)worldOverlayMat != (Object)null)
		{
			worldOverlayMat.SetTextureOffset("_MainTex", (float)(Find.TickManager.TicksGame % 3600000) * worldPanDir1 * -1f * worldOverlayPanSpeed1 * worldOverlayMat.GetTextureScale("_MainTex").x);
			if (worldOverlayMat.HasProperty("_MainTex2"))
			{
				worldOverlayMat.SetTextureOffset("_MainTex2", (float)(Find.TickManager.TicksGame % 3600000) * worldPanDir2 * -1f * worldOverlayPanSpeed2 * worldOverlayMat.GetTextureScale("_MainTex2").x);
			}
		}
	}

	public void DrawOverlay(Map map)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)worldOverlayMat != (Object)null)
		{
			Vector3 val = map.Center.ToVector3ShiftedWithAltitude(AltitudeLayer.Weather);
			Graphics.DrawMesh(MeshPool.wholeMapPlane, val, Quaternion.identity, worldOverlayMat, 0);
		}
		if ((Object)(object)screenOverlayMat != (Object)null)
		{
			float num = Find.Camera.orthographicSize * 2f;
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(num * Find.Camera.aspect, 1f, num);
			Vector3 position = ((Component)Find.Camera).transform.position;
			position.y = AltitudeLayer.Weather.AltitudeFor() + 1f / 26f;
			Matrix4x4 val3 = default(Matrix4x4);
			((Matrix4x4)(ref val3)).SetTRS(position, Quaternion.identity, val2);
			Graphics.DrawMesh(MeshPool.plane10, val3, screenOverlayMat, 0);
		}
	}

	public override string ToString()
	{
		if ((Object)(object)worldOverlayMat != (Object)null)
		{
			return ((Object)worldOverlayMat).name;
		}
		if ((Object)(object)screenOverlayMat != (Object)null)
		{
			return ((Object)screenOverlayMat).name;
		}
		return "NoOverlayOverlay";
	}
}
