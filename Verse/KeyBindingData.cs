using System.Runtime.CompilerServices;
using UnityEngine;

namespace Verse;

public class KeyBindingData
{
	public KeyCode keyBindingA;

	public KeyCode keyBindingB;

	public KeyBindingData()
	{
	}

	public KeyBindingData(KeyCode keyBindingA, KeyCode keyBindingB)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		this.keyBindingA = keyBindingA;
		this.keyBindingB = keyBindingB;
	}

	public override string ToString()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		string text = "[";
		if ((int)keyBindingA != 0)
		{
			text += ((object)System.Runtime.CompilerServices.Unsafe.As<KeyCode, KeyCode>(ref keyBindingA)/*cast due to .constrained prefix*/).ToString();
		}
		if ((int)keyBindingB != 0)
		{
			text = text + ", " + ((object)System.Runtime.CompilerServices.Unsafe.As<KeyCode, KeyCode>(ref keyBindingB)/*cast due to .constrained prefix*/).ToString();
		}
		return text + "]";
	}
}
