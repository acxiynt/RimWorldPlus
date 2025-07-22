using System;
using System.Collections.Generic;
using System.IO;
using RimWorld.IO;
using RuntimeAudioClipLoader;
using UnityEngine;

namespace Verse;

public static class ModContentLoader<T> where T : class
{
	private static string[] AcceptableExtensionsAudio = new string[7] { ".wav", ".mp3", ".ogg", ".xm", ".it", ".mod", ".s3m" };

	private static string[] AcceptableExtensionsTexture = new string[4] { ".png", ".jpg", ".jpeg", ".psd" };

	private static string[] AcceptableExtensionsString = new string[1] { ".txt" };

	public static bool IsAcceptableExtension(string extension)
	{
		string[] array;
		if (typeof(T) == typeof(AudioClip))
		{
			array = AcceptableExtensionsAudio;
		}
		else if (typeof(T) == typeof(Texture2D))
		{
			array = AcceptableExtensionsTexture;
		}
		else
		{
			if (!(typeof(T) == typeof(string)))
			{
				Log.Error("Unknown content type " + typeof(T));
				return false;
			}
			array = AcceptableExtensionsString;
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (extension.ToLower() == text)
			{
				return true;
			}
		}
		return false;
	}

	public static IEnumerable<Pair<string, LoadedContentItem<T>>> LoadAllForMod(ModContentPack mod)
	{
		DeepProfiler.Start(string.Concat("Loading assets of type ", typeof(T), " for mod ", mod));
		Dictionary<string, FileInfo> allFilesForMod = ModContentPack.GetAllFilesForMod(mod, GenFilePaths.ContentPath<T>(), IsAcceptableExtension);
		foreach (KeyValuePair<string, FileInfo> item in allFilesForMod)
		{
			LoadedContentItem<T> loadedContentItem = LoadItem((FilesystemFile)item.Value);
			if (loadedContentItem != null)
			{
				yield return new Pair<string, LoadedContentItem<T>>(item.Key, loadedContentItem);
			}
		}
		DeepProfiler.End();
	}

	public static LoadedContentItem<T> LoadItem(VirtualFile file)
	{
		try
		{
			if (typeof(T) == typeof(string))
			{
				return new LoadedContentItem<T>(file, (T)(object)file.ReadAllText());
			}
			if (typeof(T) == typeof(Texture2D))
			{
				return new LoadedContentItem<T>(file, (T)(object)LoadTexture(file));
			}
			if (typeof(T) == typeof(AudioClip))
			{
				IDisposable disposable = null;
				bool doStream = ShouldStreamAudioClipFromFile(file);
				Stream stream = file.CreateReadStream();
				T val;
				try
				{
					val = (T)(object)Manager.Load(stream, GetFormat(file.Name), file.Name, doStream);
				}
				catch (Exception)
				{
					stream.Dispose();
					throw;
				}
				disposable = stream;
				Object val2 = (Object)(object)((val is Object) ? val : null);
				if (val2 != (Object)null)
				{
					val2.name = Path.GetFileNameWithoutExtension(file.Name);
				}
				return new LoadedContentItem<T>(file, val, disposable);
			}
		}
		catch (Exception arg)
		{
			Log.Error($"Exception loading {typeof(T)} from file.\nabsFilePath: {file.FullPath}\nException: {arg}");
		}
		if (typeof(T) == typeof(Texture2D))
		{
			return (LoadedContentItem<T>)(object)new LoadedContentItem<Texture2D>(file, BaseContent.BadTex);
		}
		return null;
	}

	private static AudioFormat GetFormat(string filename)
	{
		switch (Path.GetExtension(filename))
		{
		case ".ogg":
			return AudioFormat.ogg;
		case ".mp3":
			return AudioFormat.mp3;
		case ".aiff":
		case ".aif":
		case ".aifc":
			return AudioFormat.aiff;
		case ".wav":
			return AudioFormat.wav;
		default:
			return AudioFormat.unknown;
		}
	}

	private static AudioType GetAudioTypeFromURI(string uri)
	{
		if (!uri.EndsWith(".ogg"))
		{
			return (AudioType)20;
		}
		return (AudioType)14;
	}

	private static bool ShouldStreamAudioClipFromFile(VirtualFile file)
	{
		if (!(file is FilesystemFile) || !file.Exists)
		{
			return false;
		}
		return file.Length > 307200;
	}

	private static Texture2D LoadTexture(VirtualFile file)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		Texture2D val = null;
		if (file.Exists)
		{
			byte[] array = file.ReadAllBytes();
			val = new Texture2D(2, 2, (TextureFormat)1, true);
			ImageConversion.LoadImage(val, array);
			if (((Texture)val).width % 4 != 0 || ((Texture)val).height % 4 != 0)
			{
				if (Prefs.LogVerbose)
				{
					Debug.LogWarning((object)$"Texture does not support mipmapping, needs to be divisible by 4 ({((Texture)val).width}x{((Texture)val).height}) for '{file.Name}'");
				}
				val = new Texture2D(2, 2, (TextureFormat)1, false);
				ImageConversion.LoadImage(val, array);
			}
			if (Prefs.TextureCompression)
			{
				val.Compress(true);
			}
			((Object)val).name = Path.GetFileNameWithoutExtension(file.Name);
			((Texture)val).filterMode = (FilterMode)2;
			((Texture)val).anisoLevel = 2;
			val.Apply(true, true);
		}
		return val;
	}
}
