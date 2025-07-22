using Steamworks;
using UnityEngine;
using Verse.Steam;

namespace Verse;

public static class SteamUtility
{
	private static string cachedPersonaName;

	public static string SteamPersonaName
	{
		get
		{
			if (SteamManager.Initialized && cachedPersonaName == null)
			{
				cachedPersonaName = SteamFriends.GetPersonaName();
			}
			if (cachedPersonaName == null)
			{
				return "???";
			}
			return cachedPersonaName;
		}
	}

	public static void OpenUrl(string url)
	{
		Application.OpenURL(url);
	}

	public static void OpenWorkshopPage(PublishedFileId_t pfid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		OpenUrl(SteamWorkshopPageUrl(pfid));
	}

	public static void OpenSteamWorkshopPage()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		OpenUrl("http://steamcommunity.com/workshop/browse/?appid=" + SteamUtils.GetAppID());
	}

	public static string SteamWorkshopPageUrl(PublishedFileId_t pfid)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return "steam://url/CommunityFilePage/" + pfid;
	}
}
