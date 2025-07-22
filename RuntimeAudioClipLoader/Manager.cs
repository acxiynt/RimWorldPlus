using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NAudio.Wave;
using UnityEngine;
using Verse;

namespace RuntimeAudioClipLoader;

[StaticConstructorOnStartup]
public class Manager : MonoBehaviour
{
	private class AudioInstance
	{
		public AudioClip audioClip;

		public CustomAudioFileReader reader;

		public float[] dataToSet;

		public int samplesCount;

		public Stream streamToDisposeOnceDone;

		public int channels => ((WaveStream)reader).WaveFormat.Channels;

		public int sampleRate => ((WaveStream)reader).WaveFormat.SampleRate;

		public static implicit operator AudioClip(AudioInstance ai)
		{
			return ai.audioClip;
		}
	}

	private static readonly string[] supportedFormats;

	private static Dictionary<string, AudioClip> cache;

	private static Queue<AudioInstance> deferredLoadQueue;

	private static Queue<AudioInstance> deferredSetDataQueue;

	private static Queue<AudioInstance> deferredSetFail;

	private static Thread deferredLoaderThread;

	private static GameObject managerInstance;

	private static Dictionary<AudioClip, AudioClipLoadType> audioClipLoadType;

	private static Dictionary<AudioClip, AudioDataLoadState> audioLoadState;

	static Manager()
	{
		cache = new Dictionary<string, AudioClip>();
		deferredLoadQueue = new Queue<AudioInstance>();
		deferredSetDataQueue = new Queue<AudioInstance>();
		deferredSetFail = new Queue<AudioInstance>();
		audioClipLoadType = new Dictionary<AudioClip, AudioClipLoadType>();
		audioLoadState = new Dictionary<AudioClip, AudioDataLoadState>();
		supportedFormats = Enum.GetNames(typeof(AudioFormat));
	}

	public static AudioClip Load(string filePath, bool doStream = false, bool loadInBackground = true, bool useCache = true)
	{
		if (!IsSupportedFormat(filePath))
		{
			Debug.LogError((object)("Could not load AudioClip at path '" + filePath + "' it's extensions marks unsupported format, supported formats are: " + string.Join(", ", Enum.GetNames(typeof(AudioFormat)))));
			return null;
		}
		if (useCache && cache.TryGetValue(filePath, out var value) && Object.op_Implicit((Object)(object)value))
		{
			return value;
		}
		value = Load(new StreamReader(filePath).BaseStream, GetAudioFormat(filePath), filePath, doStream, loadInBackground);
		if (useCache)
		{
			cache[filePath] = value;
		}
		return value;
	}

	public static AudioClip Load(Stream dataStream, AudioFormat audioFormat, string unityAudioClipName, bool doStream = false, bool loadInBackground = true, bool diposeDataStreamIfNotNeeded = true)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_008f: Expected O, but got Unknown
		AudioClip val = null;
		CustomAudioFileReader reader = null;
		try
		{
			reader = new CustomAudioFileReader(dataStream, audioFormat);
			AudioInstance audioInstance = new AudioInstance
			{
				reader = reader,
				samplesCount = (int)(((Stream)(object)reader).Length / (((WaveStream)reader).WaveFormat.BitsPerSample / 8))
			};
			if (doStream)
			{
				val = (audioInstance.audioClip = AudioClip.Create(unityAudioClipName, audioInstance.samplesCount / audioInstance.channels, audioInstance.channels, audioInstance.sampleRate, doStream, (PCMReaderCallback)delegate(float[] target)
				{
					reader.Read(target, 0, target.Length);
				}, (PCMSetPositionCallback)delegate(int target)
				{
					((Stream)(object)reader).Seek((long)target, SeekOrigin.Begin);
				}));
				SetAudioClipLoadType(audioInstance, (AudioClipLoadType)2);
				SetAudioClipLoadState(audioInstance, (AudioDataLoadState)2);
			}
			else
			{
				val = (audioInstance.audioClip = AudioClip.Create(unityAudioClipName, audioInstance.samplesCount / audioInstance.channels, audioInstance.channels, audioInstance.sampleRate, doStream));
				if (diposeDataStreamIfNotNeeded)
				{
					audioInstance.streamToDisposeOnceDone = dataStream;
				}
				SetAudioClipLoadType(audioInstance, (AudioClipLoadType)0);
				SetAudioClipLoadState(audioInstance, (AudioDataLoadState)1);
				if (loadInBackground)
				{
					lock (deferredLoadQueue)
					{
						deferredLoadQueue.Enqueue(audioInstance);
					}
					RunDeferredLoaderThread();
					EnsureInstanceExists();
				}
				else
				{
					audioInstance.dataToSet = new float[audioInstance.samplesCount];
					audioInstance.reader.Read(audioInstance.dataToSet, 0, audioInstance.dataToSet.Length);
					audioInstance.audioClip.SetData(audioInstance.dataToSet, 0);
					SetAudioClipLoadState(audioInstance, (AudioDataLoadState)2);
				}
			}
		}
		catch (Exception arg)
		{
			SetAudioClipLoadState(val, (AudioDataLoadState)3);
			Debug.LogError((object)$"Could not load AudioClip named '{unityAudioClipName}', exception:{arg}");
		}
		return val;
	}

	private static void RunDeferredLoaderThread()
	{
		if (deferredLoaderThread == null || !deferredLoaderThread.IsAlive)
		{
			deferredLoaderThread = new Thread(DeferredLoaderMain);
			deferredLoaderThread.IsBackground = true;
			deferredLoaderThread.Start();
		}
	}

	private static void DeferredLoaderMain()
	{
		AudioInstance audioInstance = null;
		bool flag = true;
		long num = 100000L;
		while (flag || num > 0)
		{
			num--;
			lock (deferredLoadQueue)
			{
				flag = deferredLoadQueue.Count > 0;
				if (!flag)
				{
					continue;
				}
				audioInstance = deferredLoadQueue.Dequeue();
				goto IL_0054;
			}
			IL_0054:
			num = 100000L;
			try
			{
				audioInstance.dataToSet = new float[audioInstance.samplesCount];
				audioInstance.reader.Read(audioInstance.dataToSet, 0, audioInstance.dataToSet.Length);
				((Stream)(object)audioInstance.reader).Close();
				((Stream)(object)audioInstance.reader).Dispose();
				if (audioInstance.streamToDisposeOnceDone != null)
				{
					audioInstance.streamToDisposeOnceDone.Close();
					audioInstance.streamToDisposeOnceDone.Dispose();
					audioInstance.streamToDisposeOnceDone = null;
				}
				lock (deferredSetDataQueue)
				{
					deferredSetDataQueue.Enqueue(audioInstance);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				lock (deferredSetFail)
				{
					deferredSetFail.Enqueue(audioInstance);
				}
			}
		}
	}

	private void Update()
	{
		AudioInstance audioInstance = null;
		for (bool flag = true; flag; audioInstance.audioClip.SetData(audioInstance.dataToSet, 0), SetAudioClipLoadState(audioInstance, (AudioDataLoadState)2), audioInstance.audioClip = null, audioInstance.dataToSet = null)
		{
			lock (deferredSetDataQueue)
			{
				flag = deferredSetDataQueue.Count > 0;
				if (!flag)
				{
					break;
				}
				audioInstance = deferredSetDataQueue.Dequeue();
				continue;
			}
		}
		lock (deferredSetFail)
		{
			while (deferredSetFail.Count > 0)
			{
				audioInstance = deferredSetFail.Dequeue();
				SetAudioClipLoadState(audioInstance, (AudioDataLoadState)3);
			}
		}
	}

	private static void EnsureInstanceExists()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		if (!Object.op_Implicit((Object)(object)managerInstance))
		{
			managerInstance = new GameObject("Runtime AudioClip Loader Manger singleton instance");
			((Object)managerInstance).hideFlags = (HideFlags)61;
			managerInstance.AddComponent<Manager>();
		}
	}

	public static void SetAudioClipLoadState(AudioClip audioClip, AudioDataLoadState newLoadState)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		audioLoadState[audioClip] = newLoadState;
	}

	public static AudioDataLoadState GetAudioClipLoadState(AudioClip audioClip)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		AudioDataLoadState value = (AudioDataLoadState)3;
		if ((Object)(object)audioClip != (Object)null)
		{
			value = audioClip.loadState;
			audioLoadState.TryGetValue(audioClip, out value);
		}
		return value;
	}

	public static void SetAudioClipLoadType(AudioClip audioClip, AudioClipLoadType newLoadType)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		audioClipLoadType[audioClip] = newLoadType;
	}

	public static AudioClipLoadType GetAudioClipLoadType(AudioClip audioClip)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		AudioClipLoadType value = (AudioClipLoadType)(-1);
		if ((Object)(object)audioClip != (Object)null)
		{
			value = audioClip.loadType;
			audioClipLoadType.TryGetValue(audioClip, out value);
		}
		return value;
	}

	private static string GetExtension(string filePath)
	{
		return Path.GetExtension(filePath).Substring(1).ToLower();
	}

	public static bool IsSupportedFormat(string filePath)
	{
		return supportedFormats.Contains(GetExtension(filePath));
	}

	public static AudioFormat GetAudioFormat(string filePath)
	{
		AudioFormat result = AudioFormat.unknown;
		try
		{
			result = (AudioFormat)Enum.Parse(typeof(AudioFormat), GetExtension(filePath), ignoreCase: true);
		}
		catch
		{
		}
		return result;
	}

	public static void ClearCache()
	{
		cache.Clear();
	}
}
