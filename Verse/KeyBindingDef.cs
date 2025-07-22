using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse;

public class KeyBindingDef : Def
{
	public KeyBindingCategoryDef category;

	public KeyCode defaultKeyCodeA;

	public KeyCode defaultKeyCodeB;

	public bool devModeOnly;

	[NoTranslate]
	public List<string> extraConflictTags;

	public KeyCode MainKey
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out var value))
			{
				if ((int)value.keyBindingA != 0)
				{
					return value.keyBindingA;
				}
				if ((int)value.keyBindingB != 0)
				{
					return value.keyBindingB;
				}
			}
			return (KeyCode)0;
		}
	}

	public string MainKeyLabel => MainKey.ToStringReadable();

	public bool KeyDownEvent
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Invalid comparison between Unknown and I4
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Invalid comparison between Unknown and I4
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Invalid comparison between Unknown and I4
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Invalid comparison between Unknown and I4
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Invalid comparison between Unknown and I4
			if ((int)Event.current.type == 4 && (int)Event.current.keyCode != 0 && KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out var value))
			{
				if ((int)value.keyBindingA != 310 && (int)value.keyBindingA != 309 && (int)value.keyBindingB != 310 && (int)value.keyBindingB != 309 && Event.current.command)
				{
					return false;
				}
				if (Event.current.keyCode != value.keyBindingA)
				{
					return Event.current.keyCode == value.keyBindingB;
				}
				return true;
			}
			return false;
		}
	}

	public bool IsDownEvent
	{
		get
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Invalid comparison between Unknown and I4
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Invalid comparison between Unknown and I4
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Invalid comparison between Unknown and I4
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Invalid comparison between Unknown and I4
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Invalid comparison between Unknown and I4
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Invalid comparison between Unknown and I4
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Invalid comparison between Unknown and I4
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Invalid comparison between Unknown and I4
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Invalid comparison between Unknown and I4
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Invalid comparison between Unknown and I4
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Invalid comparison between Unknown and I4
			//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Invalid comparison between Unknown and I4
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Invalid comparison between Unknown and I4
			//IL_0116: Unknown result type (might be due to invalid IL or missing references)
			//IL_0120: Invalid comparison between Unknown and I4
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Invalid comparison between Unknown and I4
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Invalid comparison between Unknown and I4
			if (Event.current == null)
			{
				return false;
			}
			if (!KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out var value))
			{
				return false;
			}
			if (KeyDownEvent)
			{
				return true;
			}
			if (Event.current.shift && ((int)value.keyBindingA == 304 || (int)value.keyBindingA == 303 || (int)value.keyBindingB == 304 || (int)value.keyBindingB == 303))
			{
				return true;
			}
			if (Event.current.control && ((int)value.keyBindingA == 306 || (int)value.keyBindingA == 305 || (int)value.keyBindingB == 306 || (int)value.keyBindingB == 305))
			{
				return true;
			}
			if (Event.current.alt && ((int)value.keyBindingA == 308 || (int)value.keyBindingA == 307 || (int)value.keyBindingB == 308 || (int)value.keyBindingB == 307))
			{
				return true;
			}
			if (Event.current.command && ((int)value.keyBindingA == 310 || (int)value.keyBindingA == 309 || (int)value.keyBindingB == 310 || (int)value.keyBindingB == 309))
			{
				return true;
			}
			return IsDown;
		}
	}

	public bool JustPressed
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out var value))
			{
				if (!Input.GetKeyDown(value.keyBindingA))
				{
					return Input.GetKeyDown(value.keyBindingB);
				}
				return true;
			}
			return false;
		}
	}

	public bool IsDown
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			if (KeyPrefs.KeyPrefsData.keyPrefs.TryGetValue(this, out var value))
			{
				if (!Input.GetKey(value.keyBindingA))
				{
					return Input.GetKey(value.keyBindingB);
				}
				return true;
			}
			return false;
		}
	}

	public KeyCode GetDefaultKeyCode(KeyPrefs.BindingSlot slot)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return (KeyCode)(slot switch
		{
			KeyPrefs.BindingSlot.A => defaultKeyCodeA, 
			KeyPrefs.BindingSlot.B => defaultKeyCodeB, 
			_ => throw new InvalidOperationException(), 
		});
	}

	public static KeyBindingDef Named(string name)
	{
		return DefDatabase<KeyBindingDef>.GetNamedSilentFail(name);
	}
}
