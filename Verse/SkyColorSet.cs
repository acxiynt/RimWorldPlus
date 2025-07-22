using UnityEngine;

namespace Verse;

public struct SkyColorSet
{
	public Color sky;

	public Color shadow;

	public Color overlay;

	public float saturation;

	public SkyColorSet(Color sky, Color shadow, Color overlay, float saturation)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		this.sky = sky;
		this.shadow = shadow;
		this.overlay = overlay;
		this.saturation = saturation;
	}

	public static SkyColorSet Lerp(SkyColorSet A, SkyColorSet B, float t)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		return new SkyColorSet
		{
			sky = Color.Lerp(A.sky, B.sky, t),
			shadow = Color.Lerp(A.shadow, B.shadow, t),
			overlay = Color.Lerp(A.overlay, B.overlay, t),
			saturation = Mathf.Lerp(A.saturation, B.saturation, t)
		};
	}

	public static SkyColorSet LerpDarken(SkyColorSet A, SkyColorSet B, float t)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		return new SkyColorSet
		{
			sky = Color.Lerp(A.sky, A.sky.Min(B.sky), t),
			shadow = Color.Lerp(A.shadow, A.shadow.Min(B.shadow), t),
			overlay = Color.Lerp(A.overlay, A.overlay.Min(B.overlay), t),
			saturation = Mathf.Lerp(A.saturation, Mathf.Min(A.saturation, B.saturation), t)
		};
	}

	public override string ToString()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		return string.Concat("(sky=", sky, ", shadow=", shadow, ", overlay=", overlay, ", sat=", saturation, ")");
	}
}
