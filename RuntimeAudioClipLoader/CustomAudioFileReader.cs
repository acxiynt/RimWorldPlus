using System.IO;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NVorbis.NAudioSupport;
using UnityEngine;

namespace RuntimeAudioClipLoader;

internal class CustomAudioFileReader : WaveStream, ISampleProvider
{
	private WaveStream readerStream;

	private readonly SampleChannel sampleChannel;

	private readonly int destBytesPerSample;

	private readonly int sourceBytesPerSample;

	private readonly long length;

	private readonly object lockObject;

	public override WaveFormat WaveFormat => sampleChannel.WaveFormat;

	public override long Length => length;

	public override long Position
	{
		get
		{
			return SourceToDest(((Stream)(object)readerStream).Position);
		}
		set
		{
			lock (lockObject)
			{
				((Stream)(object)readerStream).Position = DestToSource(value);
			}
		}
	}

	public float Volume
	{
		get
		{
			return sampleChannel.Volume;
		}
		set
		{
			sampleChannel.Volume = value;
		}
	}

	public CustomAudioFileReader(Stream stream, AudioFormat format)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		lockObject = new object();
		CreateReaderStream(stream, format);
		sourceBytesPerSample = readerStream.WaveFormat.BitsPerSample / 8 * readerStream.WaveFormat.Channels;
		sampleChannel = new SampleChannel((IWaveProvider)(object)readerStream, false);
		destBytesPerSample = 4 * sampleChannel.WaveFormat.Channels;
		length = SourceToDest(((Stream)(object)readerStream).Length);
	}

	private void CreateReaderStream(Stream stream, AudioFormat format)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Expected O, but got Unknown
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Invalid comparison between Unknown and I4
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		switch (format)
		{
		case AudioFormat.wav:
			readerStream = (WaveStream)new WaveFileReader(stream);
			if ((int)readerStream.WaveFormat.Encoding != 1 && (int)readerStream.WaveFormat.Encoding != 3)
			{
				readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
				readerStream = (WaveStream)new BlockAlignReductionStream(readerStream);
			}
			break;
		case AudioFormat.mp3:
			readerStream = (WaveStream)new Mp3FileReader(stream);
			break;
		case AudioFormat.aiff:
			readerStream = (WaveStream)new AiffFileReader(stream);
			break;
		case AudioFormat.ogg:
			readerStream = (WaveStream)(object)new VorbisWaveReader(stream);
			break;
		default:
			Debug.LogWarning((object)$"Audio format {format} is not supported");
			break;
		}
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		WaveBuffer val = new WaveBuffer(buffer);
		int count2 = count / 4;
		return Read(val.FloatBuffer, offset / 4, count2) * 4;
	}

	public int Read(float[] buffer, int offset, int count)
	{
		lock (lockObject)
		{
			return sampleChannel.Read(buffer, offset, count);
		}
	}

	private long SourceToDest(long sourceBytes)
	{
		return destBytesPerSample * (sourceBytes / sourceBytesPerSample);
	}

	private long DestToSource(long destBytes)
	{
		return sourceBytesPerSample * (destBytes / destBytesPerSample);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && readerStream != null)
		{
			((Stream)(object)readerStream).Dispose();
			readerStream = null;
		}
		((Stream)this).Dispose(disposing);
	}
}
