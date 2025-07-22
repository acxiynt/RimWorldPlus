using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Verse;

public class ModAssetBundlesHandler
{
	private ModContentPack mod;

	public List<AssetBundle> loadedAssetBundles = new List<AssetBundle>();

	public static readonly string[] TextureExtensions = new string[4] { ".png", ".psd", ".jpg", ".jpeg" };

	public static readonly string[] AudioClipExtensions = new string[2] { ".wav", ".mp3" };

	public ModAssetBundlesHandler(ModContentPack mod)
	{
		this.mod = mod;
	}

	public void ReloadAll(bool hotReload = false)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(mod.RootDir, "AssetBundles"));
		if (!directoryInfo.Exists)
		{
			return;
		}
		FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
		foreach (FileInfo fileInfo in files)
		{
			if (!fileInfo.Extension.NullOrEmpty())
			{
				continue;
			}
			AssetBundle val = AssetBundle.LoadFromFile(fileInfo.FullName);
			if ((Object)(object)val != (Object)null)
			{
				if (!loadedAssetBundles.Contains(val))
				{
					loadedAssetBundles.Add(val);
				}
			}
			else
			{
				Log.Error("Could not load asset bundle at " + fileInfo.FullName);
			}
		}
	}

	public void ClearDestroy()
	{
		LongEventHandler.ExecuteWhenFinished(delegate
		{
			for (int i = 0; i < loadedAssetBundles.Count; i++)
			{
				loadedAssetBundles[i].Unload(true);
			}
			loadedAssetBundles.Clear();
		});
	}
}
