using Steamworks;

namespace Verse.Steam;

internal static class WorkshopUtility
{
	public static string GetLabel(this WorkshopInteractStage stage)
	{
		if (stage == WorkshopInteractStage.None)
		{
			return "None".Translate();
		}
		return ("WorkshopInteractStage_" + stage).Translate();
	}

	public unsafe static string GetLabel(this EItemUpdateStatus status)
	{
		return ("EItemUpdateStatus_" + ((object)(*(EItemUpdateStatus*)(&status))/*cast due to .constrained prefix*/).ToString()).Translate();
	}

	public unsafe static string GetLabel(this EResult result)
	{
		return ((object)(*(EResult*)(&result))/*cast due to .constrained prefix*/).ToString().Substring(9);
	}
}
