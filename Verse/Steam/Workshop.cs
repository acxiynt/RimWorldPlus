using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LudeonTK;
using RimWorld;
using Steamworks;

namespace Verse.Steam;

public static class Workshop
{
	private static WorkshopItemHook uploadingHook;

	private static UGCUpdateHandle_t curUpdateHandle;

	private static WorkshopInteractStage curStage = WorkshopInteractStage.None;

	private static Callback<RemoteStoragePublishedFileSubscribed_t> subscribedCallback;

	private static Callback<RemoteStoragePublishedFileUnsubscribed_t> unsubscribedCallback;

	private static Callback<ItemInstalled_t> installedCallback;

	private static CallResult<SubmitItemUpdateResult_t> submitResult;

	private static CallResult<CreateItemResult_t> createResult;

	private static CallResult<SteamUGCRequestUGCDetailsResult_t> requestDetailsResult;

	private static UGCQueryHandle_t detailsQueryHandle;

	private static int detailsQueryCount = -1;

	public const uint InstallInfoFolderNameMaxLength = 257u;

	public static WorkshopInteractStage CurStage => curStage;

	internal static void Init()
	{
		subscribedCallback = Callback<RemoteStoragePublishedFileSubscribed_t>.Create((DispatchDelegate<RemoteStoragePublishedFileSubscribed_t>)OnItemSubscribed);
		installedCallback = Callback<ItemInstalled_t>.Create((DispatchDelegate<ItemInstalled_t>)OnItemInstalled);
		unsubscribedCallback = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create((DispatchDelegate<RemoteStoragePublishedFileUnsubscribed_t>)OnItemUnsubscribed);
	}

	internal static void Upload(WorkshopUploadable item)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		if (curStage != WorkshopInteractStage.None)
		{
			Messages.Message("UploadAlreadyInProgress".Translate(), MessageTypeDefOf.RejectInput, historical: false);
			return;
		}
		uploadingHook = item.GetWorkshopItemHook();
		if (uploadingHook.PublishedFileId != PublishedFileId_t.Invalid)
		{
			if (Prefs.LogVerbose)
			{
				Log.Message("Workshop: Starting item update for mod '" + uploadingHook.Name + "' with PublishedFileId " + uploadingHook.PublishedFileId);
			}
			curStage = WorkshopInteractStage.SubmittingItem;
			curUpdateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), uploadingHook.PublishedFileId);
			SetWorkshopItemDataFrom(curUpdateHandle, uploadingHook, creating: false);
			SteamAPICall_t val = SteamUGC.SubmitItemUpdate(curUpdateHandle, "[Auto-generated text]: Update on " + DateTime.Now.ToString() + ".");
			submitResult = CallResult<SubmitItemUpdateResult_t>.Create((APIDispatchDelegate<SubmitItemUpdateResult_t>)OnItemSubmitted);
			submitResult.Set(val, (APIDispatchDelegate<SubmitItemUpdateResult_t>)null);
		}
		else
		{
			if (Prefs.LogVerbose)
			{
				Log.Message("Workshop: Starting item creation for mod '" + uploadingHook.Name + "'.");
			}
			curStage = WorkshopInteractStage.CreatingItem;
			SteamAPICall_t val2 = SteamUGC.CreateItem(SteamUtils.GetAppID(), (EWorkshopFileType)0);
			createResult = CallResult<CreateItemResult_t>.Create((APIDispatchDelegate<CreateItemResult_t>)OnItemCreated);
			createResult.Set(val2, (APIDispatchDelegate<CreateItemResult_t>)null);
		}
		Find.WindowStack.Add(new Dialog_WorkshopOperationInProgress());
	}

	internal static void Unsubscribe(PublishedFileId_t pfid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SteamUGC.UnsubscribeItem(pfid);
	}

	internal static void Unsubscribe(WorkshopUploadable item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		SteamUGC.UnsubscribeItem(item.GetPublishedFileId());
	}

	internal static void RequestItemsDetails(PublishedFileId_t[] publishedFileIds)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (detailsQueryCount >= 0)
		{
			Log.Error("Requested Workshop item details while a details request was already pending.");
			return;
		}
		detailsQueryCount = publishedFileIds.Length;
		detailsQueryHandle = SteamUGC.CreateQueryUGCDetailsRequest(publishedFileIds, (uint)detailsQueryCount);
		SteamAPICall_t val = SteamUGC.SendQueryUGCRequest(detailsQueryHandle);
		requestDetailsResult = CallResult<SteamUGCRequestUGCDetailsResult_t>.Create((APIDispatchDelegate<SteamUGCRequestUGCDetailsResult_t>)OnGotItemDetails);
		requestDetailsResult.Set(val, (APIDispatchDelegate<SteamUGCRequestUGCDetailsResult_t>)null);
	}

	internal static void OnItemSubscribed(RemoteStoragePublishedFileSubscribed_t result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (IsOurAppId(result.m_nAppID))
		{
			if (Prefs.LogVerbose)
			{
				Log.Message("Workshop: Item subscribed: " + result.m_nPublishedFileId);
			}
			Find.WindowStack.WindowOfType<Page_ModsConfig>()?.Notify_SteamItemSubscribed(result.m_nPublishedFileId);
			WorkshopItems.Notify_Subscribed(result.m_nPublishedFileId);
		}
	}

	internal static void OnItemInstalled(ItemInstalled_t result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (IsOurAppId(result.m_unAppID))
		{
			if (Prefs.LogVerbose)
			{
				Log.Message("Workshop: Item installed: " + result.m_nPublishedFileId);
			}
			Find.WindowStack.WindowOfType<Page_ModsConfig>()?.Notify_SteamItemInstalled(result.m_nPublishedFileId);
			WorkshopItems.Notify_Installed(result.m_nPublishedFileId);
		}
	}

	internal static void OnItemUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (IsOurAppId(result.m_nAppID))
		{
			if (Prefs.LogVerbose)
			{
				Log.Message("Workshop: Item unsubscribed: " + result.m_nPublishedFileId);
			}
			Find.WindowStack.WindowOfType<Page_ModsConfig>()?.Notify_SteamItemUnsubscribed(result.m_nPublishedFileId);
			Find.WindowStack.WindowOfType<Page_SelectScenario>()?.Notify_SteamItemUnsubscribed(result.m_nPublishedFileId);
			WorkshopItems.Notify_Unsubscribed(result.m_nPublishedFileId);
		}
	}

	private static void OnItemCreated(CreateItemResult_t result, bool IOFailure)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if (IOFailure || (int)result.m_eResult != 1)
		{
			uploadingHook = null;
			Dialog_WorkshopOperationInProgress.CloseAll();
			Log.Error("Workshop: OnItemCreated failure. Result: " + result.m_eResult.GetLabel());
			Find.WindowStack.Add(new Dialog_MessageBox("WorkshopSubmissionFailed".Translate(GenText.SplitCamelCase(result.m_eResult.GetLabel()))));
			return;
		}
		uploadingHook.PublishedFileId = result.m_nPublishedFileId;
		if (Prefs.LogVerbose)
		{
			Log.Message("Workshop: Item created. PublishedFileId: " + uploadingHook.PublishedFileId);
		}
		curUpdateHandle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), uploadingHook.PublishedFileId);
		SetWorkshopItemDataFrom(curUpdateHandle, uploadingHook, creating: true);
		curStage = WorkshopInteractStage.SubmittingItem;
		if (Prefs.LogVerbose)
		{
			Log.Message("Workshop: Submitting item.");
		}
		SteamAPICall_t val = SteamUGC.SubmitItemUpdate(curUpdateHandle, "[Auto-generated text]: Initial upload.");
		submitResult = CallResult<SubmitItemUpdateResult_t>.Create((APIDispatchDelegate<SubmitItemUpdateResult_t>)OnItemSubmitted);
		submitResult.Set(val, (APIDispatchDelegate<SubmitItemUpdateResult_t>)null);
		createResult = null;
	}

	private static void OnItemSubmitted(SubmitItemUpdateResult_t result, bool IOFailure)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Invalid comparison between Unknown and I4
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		if (IOFailure || (int)result.m_eResult != 1)
		{
			uploadingHook = null;
			Dialog_WorkshopOperationInProgress.CloseAll();
			Log.Error("Workshop: OnItemSubmitted failure. Result: " + result.m_eResult.GetLabel());
			Find.WindowStack.Add(new Dialog_MessageBox("WorkshopSubmissionFailed".Translate(GenText.SplitCamelCase(result.m_eResult.GetLabel()))));
		}
		else
		{
			SteamUtility.OpenWorkshopPage(uploadingHook.PublishedFileId);
			Messages.Message("WorkshopUploadSucceeded".Translate(uploadingHook.Name), MessageTypeDefOf.TaskCompletion, historical: false);
			if (Prefs.LogVerbose)
			{
				Log.Message("Workshop: Item submit result: " + result.m_eResult);
			}
		}
		curStage = WorkshopInteractStage.None;
		submitResult = null;
	}

	private static void OnGotItemDetails(SteamUGCRequestUGCDetailsResult_t result, bool IOFailure)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Invalid comparison between Unknown and I4
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (IOFailure)
		{
			Log.Error("Workshop: OnGotItemDetails IOFailure.");
			detailsQueryCount = -1;
			return;
		}
		if (detailsQueryCount < 0)
		{
			Log.Warning("Got unexpected Steam Workshop item details response.");
		}
		string text = "Steam Workshop Item details received:";
		SteamUGCDetails_t val = default(SteamUGCDetails_t);
		for (int i = 0; i < detailsQueryCount; i++)
		{
			SteamUGC.GetQueryUGCResult(detailsQueryHandle, (uint)i, ref val);
			if ((int)val.m_eResult != 1)
			{
				text = text + "\n  Query result: " + val.m_eResult;
			}
			else
			{
				text = text + "\n  Title: " + ((SteamUGCDetails_t)(ref val)).m_rgchTitle;
				text = text + "\n  PublishedFileId: " + val.m_nPublishedFileId;
				text = text + "\n  Created: " + DateTime.FromFileTimeUtc(val.m_rtimeCreated).ToString();
				text = text + "\n  Updated: " + DateTime.FromFileTimeUtc(val.m_rtimeUpdated).ToString();
				text = text + "\n  Added to list: " + DateTime.FromFileTimeUtc(val.m_rtimeAddedToUserList).ToString();
				text = text + "\n  File size: " + val.m_nFileSize.ToStringKilobytes();
				text = text + "\n  Preview size: " + val.m_nPreviewFileSize.ToStringKilobytes();
				text = text + "\n  File name: " + ((SteamUGCDetails_t)(ref val)).m_pchFileName;
				text = text + "\n  CreatorAppID: " + val.m_nCreatorAppID;
				text = text + "\n  ConsumerAppID: " + val.m_nConsumerAppID;
				text = text + "\n  Visibiliy: " + val.m_eVisibility;
				text = text + "\n  FileType: " + val.m_eFileType;
				text = text + "\n  Owner: " + val.m_ulSteamIDOwner;
			}
			text += "\n";
		}
		Log.Message(text.TrimEndNewlines());
		detailsQueryCount = -1;
	}

	public static void GetUpdateStatus(out EItemUpdateStatus updateStatus, out float progPercent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected I4, but got Unknown
		ulong num = default(ulong);
		ulong num2 = default(ulong);
		updateStatus = (EItemUpdateStatus)(int)SteamUGC.GetItemUpdateProgress(curUpdateHandle, ref num, ref num2);
		progPercent = (float)num / (float)num2;
	}

	public static string UploadButtonLabel(PublishedFileId_t pfid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return (pfid != PublishedFileId_t.Invalid) ? "UpdateOnSteamWorkshop".Translate() : "UploadToSteamWorkshop".Translate();
	}

	private static void SetWorkshopItemDataFrom(UGCUpdateHandle_t updateHandle, WorkshopItemHook hook, bool creating)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		hook.PrepareForWorkshopUpload();
		SteamUGC.SetItemTitle(updateHandle, hook.Name);
		if (creating)
		{
			SteamUGC.SetItemDescription(updateHandle, hook.Description);
		}
		if (!File.Exists(hook.PreviewImagePath))
		{
			Log.Warning("Missing preview file at " + hook.PreviewImagePath);
		}
		else
		{
			SteamUGC.SetItemPreview(updateHandle, hook.PreviewImagePath);
		}
		IList<string> tags = hook.Tags;
		foreach (Version supportedVersion in hook.SupportedVersions)
		{
			tags.Add(supportedVersion.Major + "." + supportedVersion.Minor);
		}
		SteamUGC.SetItemTags(updateHandle, tags);
		SteamUGC.SetItemContent(updateHandle, hook.Directory.FullName);
	}

	internal static IEnumerable<PublishedFileId_t> AllSubscribedItems()
	{
		uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
		PublishedFileId_t[] subbedItems = (PublishedFileId_t[])(object)new PublishedFileId_t[numSubscribedItems];
		uint count = SteamUGC.GetSubscribedItems(subbedItems, numSubscribedItems);
		for (int i = 0; i < count; i++)
		{
			yield return subbedItems[i];
		}
	}

	[DebugOutput("System", false)]
	internal static void SteamWorkshopStatus()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All subscribed items (" + SteamUGC.GetNumSubscribedItems() + " total):");
		List<PublishedFileId_t> list = AllSubscribedItems().ToList();
		for (int i = 0; i < list.Count; i++)
		{
			stringBuilder.AppendLine("   " + ItemStatusString(list[i]));
		}
		stringBuilder.AppendLine("All installed mods:");
		foreach (ModMetaData allInstalledMod in ModLister.AllInstalledMods)
		{
			stringBuilder.AppendLine("   " + allInstalledMod.PackageIdPlayerFacing + ": " + ItemStatusString(allInstalledMod.GetPublishedFileId()));
		}
		Log.Message(stringBuilder.ToString());
		List<PublishedFileId_t> list2 = AllSubscribedItems().ToList();
		PublishedFileId_t[] array = (PublishedFileId_t[])(object)new PublishedFileId_t[list2.Count];
		for (int j = 0; j < list2.Count; j++)
		{
			array[j] = (PublishedFileId_t)list2[j].m_PublishedFileId;
		}
		RequestItemsDetails(array);
	}

	private static string ItemStatusString(PublishedFileId_t pfid)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (pfid == PublishedFileId_t.Invalid)
		{
			return "[unpublished]";
		}
		string text = string.Concat("[", pfid, "] ");
		ulong num = default(ulong);
		string text2 = default(string);
		uint num2 = default(uint);
		if (SteamUGC.GetItemInstallInfo(pfid, ref num, ref text2, 257u, ref num2))
		{
			text += "\n      installed";
			text = text + "\n      folder=" + text2;
			return text + "\n      sizeOnDisk=" + ((float)num / 1024f).ToString("F2") + "Kb";
		}
		return text + "\n      not installed";
	}

	private static bool IsOurAppId(AppId_t appId)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (appId != SteamUtils.GetAppID())
		{
			return false;
		}
		return true;
	}
}
