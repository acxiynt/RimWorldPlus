using UnityEngine;
using Verse;

namespace RimWorld.Planet;

[StaticConstructorOnStartup]
public static class WorldTerrainColliderManager
{
	private static GameObject gameObjectInt;

	public static GameObject GameObject => gameObjectInt;

	static WorldTerrainColliderManager()
	{
		gameObjectInt = CreateGameObject();
	}

	private static GameObject CreateGameObject()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		GameObject val = new GameObject("WorldTerrainCollider");
		Object.DontDestroyOnLoad((Object)val);
		val.layer = WorldCameraManager.WorldLayer;
		return val;
	}
}
