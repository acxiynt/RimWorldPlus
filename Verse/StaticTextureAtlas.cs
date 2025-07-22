using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Verse;

public class StaticTextureAtlas
{
	public readonly TextureAtlasGroupKey groupKey;

	private List<Texture2D> textures = new List<Texture2D>();

	private Dictionary<Texture2D, Texture2D> masks = new Dictionary<Texture2D, Texture2D>();

	private Dictionary<Texture, StaticTextureAtlasTile> tiles = new Dictionary<Texture, StaticTextureAtlasTile>();

	private Texture2D colorTexture;

	private Texture2D maskTexture;

	public const int MaxTextureSizeForTiles = 512;

	public const int TexturePadding = 8;

	public Texture2D ColorTexture => colorTexture;

	public Texture2D MaskTexture => maskTexture;

	public static int MaxPixelsPerAtlas => MaxAtlasSize / 2 * (MaxAtlasSize / 2);

	public static int MaxAtlasSize => SystemInfo.maxTextureSize;

	public StaticTextureAtlas(TextureAtlasGroupKey groupKey)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		this.groupKey = groupKey;
		colorTexture = new Texture2D(1, 1, (TextureFormat)5, false);
	}

	public void Insert(Texture2D texture, Texture2D mask = null)
	{
		if (groupKey.hasMask && (Object)(object)mask == (Object)null)
		{
			Log.Error("Tried to insert a mask-less texture into a static atlas which does have a mask atlas");
		}
		if (!groupKey.hasMask && (Object)(object)mask != (Object)null)
		{
			Log.Error("Tried to insert a mask texture into a static atlas which does not have a mask atlas");
		}
		textures.Add(texture);
		if ((Object)(object)mask != (Object)null && groupKey.hasMask)
		{
			masks.Add(texture, mask);
		}
	}

	public void Bake(bool rebake = false)
	{
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Expected O, but got Unknown
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		if (rebake)
		{
			foreach (KeyValuePair<Texture, StaticTextureAtlasTile> tile in tiles)
			{
				Object.Destroy((Object)(object)tile.Value.mesh);
			}
			tiles.Clear();
		}
		List<Texture2D> destroyTextures = new List<Texture2D>();
		try
		{
			Texture2D[] array = ((IEnumerable<Texture2D>)textures).Select((Func<Texture2D, Texture2D>)delegate(Texture2D t)
			{
				if (!((Texture)t).isReadable)
				{
					Texture2D val4 = TextureAtlasHelper.MakeReadableTextureInstance(t);
					destroyTextures.Add(val4);
					return val4;
				}
				return t;
			}).ToArray();
			DeepProfiler.Start("Texture2D.PackTextures()");
			Rect[] array2 = colorTexture.PackTextures(array, 8, MaxAtlasSize, false);
			DeepProfiler.End();
			((Object)colorTexture).name = "TextureAtlas_" + groupKey.ToString() + "_" + ((Object)colorTexture).GetInstanceID();
			if (groupKey.hasMask)
			{
				maskTexture = new Texture2D(((Texture)colorTexture).width, ((Texture)colorTexture).height, (TextureFormat)5, false);
			}
			for (int num = 0; num < array2.Length; num++)
			{
				Texture2D key = textures[num];
				if (masks.TryGetValue(key, out var value))
				{
					Rect val = array2[num];
					int num2 = (int)(((Rect)(ref val)).xMin * (float)((Texture)colorTexture).width);
					int num3 = (int)(((Rect)(ref val)).yMin * (float)((Texture)colorTexture).height);
					if (!((Texture)value).isReadable)
					{
						Texture2D val2 = TextureAtlasHelper.MakeReadableTextureInstance(value);
						destroyTextures.Add(val2);
						value = val2;
					}
					DeepProfiler.Start("maskTexture.SetPixels()");
					maskTexture.SetPixels(num2, num3, ((Texture)textures[num]).width, ((Texture)textures[num]).height, value.GetPixels(0), 0);
					DeepProfiler.End();
				}
			}
			if ((Object)(object)maskTexture != (Object)null)
			{
				((Object)maskTexture).name = "Mask_" + ((Object)colorTexture).name;
				DeepProfiler.Start("maskTexture.Apply()");
				maskTexture.Apply(true, false);
				DeepProfiler.End();
			}
			if (array2.Length != array.Length)
			{
				Log.Error("Texture packing failed! Clearing out atlas...");
				textures.Clear();
				return;
			}
			for (int num4 = 0; num4 < array.Length; num4++)
			{
				Mesh val3 = TextureAtlasHelper.CreateMeshForUV(array2[num4], 0.5f);
				((Object)val3).name = "TextureAtlasMesh_" + groupKey.ToString() + "_" + ((Object)val3).GetInstanceID();
				tiles.Add((Texture)(object)textures[num4], new StaticTextureAtlasTile
				{
					atlas = this,
					mesh = val3,
					uvRect = array2[num4]
				});
			}
			if (Prefs.TextureCompression)
			{
				DeepProfiler.Start("Texture2D.Compress()");
				if ((Object)(object)colorTexture != (Object)null)
				{
					colorTexture.Compress(true);
				}
				if ((Object)(object)maskTexture != (Object)null)
				{
					maskTexture.Compress(true);
				}
				DeepProfiler.End();
			}
			DeepProfiler.Start("Texture2D.Apply()");
			if ((Object)(object)colorTexture != (Object)null)
			{
				colorTexture.Apply(false, true);
			}
			if ((Object)(object)maskTexture != (Object)null)
			{
				maskTexture.Apply(false, true);
			}
			DeepProfiler.End();
		}
		finally
		{
			foreach (Texture2D item in destroyTextures)
			{
				Object.Destroy((Object)(object)item);
			}
		}
	}

	public bool TryGetTile(Texture texture, out StaticTextureAtlasTile tile)
	{
		return tiles.TryGetValue(texture, out tile);
	}

	public void Destroy()
	{
		Object.Destroy((Object)(object)colorTexture);
		Object.Destroy((Object)(object)maskTexture);
		foreach (KeyValuePair<Texture, StaticTextureAtlasTile> tile in tiles)
		{
			Object.Destroy((Object)(object)tile.Value.mesh);
		}
		textures.Clear();
		tiles.Clear();
	}
}
