using System.IO;
using Steamworks;

namespace Verse.Steam;

public class WorkshopItem
{
	protected DirectoryInfo directoryInt;

	private PublishedFileId_t pfidInt;

	public DirectoryInfo Directory => directoryInt;

	public virtual PublishedFileId_t PublishedFileId
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return pfidInt;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			pfidInt = value;
		}
	}

	public static WorkshopItem MakeFrom(PublishedFileId_t pfid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		ulong num = default(ulong);
		string path = default(string);
		uint num2 = default(uint);
		bool itemInstallInfo = SteamUGC.GetItemInstallInfo(pfid, ref num, ref path, 257u, ref num2);
		WorkshopItem workshopItem = null;
		if (!itemInstallInfo)
		{
			workshopItem = new WorkshopItem_Downloading();
		}
		else
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			if (!directoryInfo.Exists)
			{
				Log.Error(string.Concat("Created WorkshopItem for ", pfid, " but there is no folder for it."));
				return new WorkshopItem_NotInstalled();
			}
			FileInfo[] files = directoryInfo.GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				if (files[i].Extension == ".rsc")
				{
					workshopItem = new WorkshopItem_Scenario();
					break;
				}
			}
			if (workshopItem == null)
			{
				workshopItem = new WorkshopItem_Mod();
			}
			workshopItem.directoryInt = directoryInfo;
		}
		workshopItem.PublishedFileId = pfid;
		return workshopItem;
	}

	public override string ToString()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return GetType().ToString() + "-" + PublishedFileId;
	}
}
