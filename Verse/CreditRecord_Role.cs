using UnityEngine;

namespace Verse;

public class CreditRecord_Role : CreditsEntry
{
	public string roleKey;

	public string creditee;

	public string extra;

	public bool displayKey;

	public bool compressed;

	public CreditRecord_Role()
	{
	}

	public CreditRecord_Role(string roleKey, string creditee, string extra = null)
	{
		this.roleKey = roleKey;
		this.creditee = creditee;
		this.extra = extra;
	}

	public override float DrawHeight(float width)
	{
		if (roleKey.NullOrEmpty())
		{
			width *= 0.5f;
		}
		if (!compressed)
		{
			return 50f;
		}
		return Text.CalcHeight(creditee, width * 0.5f);
	}

	public override void Draw(Rect rect)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		Text.Font = GameFont.Medium;
		Text.Anchor = (TextAnchor)3;
		Rect rect2 = rect;
		((Rect)(ref rect2)).width = 0f;
		if (!roleKey.NullOrEmpty())
		{
			((Rect)(ref rect2)).width = ((Rect)(ref rect)).width / 2f;
			if (displayKey)
			{
				Widgets.Label(rect2, roleKey);
			}
		}
		Rect val = rect;
		((Rect)(ref val)).xMin = ((Rect)(ref rect2)).xMax;
		if (roleKey.NullOrEmpty())
		{
			Text.Anchor = (TextAnchor)4;
		}
		Widgets.Label(val, creditee);
		Text.Anchor = (TextAnchor)3;
		if (!extra.NullOrEmpty())
		{
			Rect rect3 = val;
			((Rect)(ref rect3)).yMin = ((Rect)(ref rect3)).yMin + 28f;
			Text.Font = GameFont.Tiny;
			GUI.color = new Color(0.7f, 0.7f, 0.7f);
			Widgets.Label(rect3, extra);
			GUI.color = Color.white;
		}
	}

	public CreditRecord_Role Compress()
	{
		compressed = true;
		return this;
	}
}
