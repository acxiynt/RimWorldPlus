using System;
using LudeonTK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Verse;

namespace RimWorld.Planet;

[StaticConstructorOnStartup]
public class WorldFeatureTextMesh_TextMeshPro : WorldFeatureTextMesh
{
	private TextMeshPro textMesh;

	public static readonly GameObject WorldTextPrefab = Resources.Load<GameObject>("Prefabs/WorldText");

	[TweakValue("Interface.World", 0f, 5f)]
	private static float TextScale = 1f;

	public override bool Active => ((Component)textMesh).gameObject.activeInHierarchy;

	public override Vector3 Position => textMesh.transform.position;

	public override Color Color
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Graphic)textMesh).color;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			((Graphic)textMesh).color = value;
		}
	}

	public override string Text
	{
		get
		{
			return ((TMP_Text)textMesh).text;
		}
		set
		{
			((TMP_Text)textMesh).text = value;
		}
	}

	public override float Size
	{
		set
		{
			((TMP_Text)textMesh).fontSize = value * TextScale;
		}
	}

	public override Quaternion Rotation
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return textMesh.transform.rotation;
		}
		set
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			textMesh.transform.rotation = value;
		}
	}

	public override Vector3 LocalPosition
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return textMesh.transform.localPosition;
		}
		set
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			textMesh.transform.localPosition = value;
		}
	}

	private static void TextScale_Changed()
	{
		Find.WorldFeatures.textsCreated = false;
	}

	public override void SetActive(bool active)
	{
		((Component)textMesh).gameObject.SetActive(active);
	}

	public override void Destroy()
	{
		Object.Destroy((Object)(object)((Component)textMesh).gameObject);
	}

	public override void Init()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = Object.Instantiate<GameObject>(WorldTextPrefab);
		Object.DontDestroyOnLoad((Object)(object)val);
		textMesh = val.GetComponent<TextMeshPro>();
		Color = new Color(1f, 1f, 1f, 0f);
		Material[] sharedMaterials = ((Renderer)((Component)textMesh).GetComponent<MeshRenderer>()).sharedMaterials;
		for (int i = 0; i < sharedMaterials.Length; i++)
		{
			sharedMaterials[i].renderQueue = WorldMaterials.FeatureNameRenderQueue;
		}
	}

	public override void WrapAroundPlanetSurface()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		((TMP_Text)textMesh).ForceMeshUpdate(false, false);
		TMP_TextInfo textInfo = ((TMP_Text)textMesh).textInfo;
		int characterCount = textInfo.characterCount;
		if (characterCount == 0)
		{
			return;
		}
		Bounds bounds = ((TMP_Text)textMesh).bounds;
		float num = ((Bounds)(ref bounds)).extents.x * 2f;
		float num2 = Find.WorldGrid.DistOnSurfaceToAngle(num);
		Matrix4x4 localToWorldMatrix = textMesh.transform.localToWorldMatrix;
		Matrix4x4 worldToLocalMatrix = textMesh.transform.worldToLocalMatrix;
		Vector3 val3 = default(Vector3);
		for (int i = 0; i < characterCount; i++)
		{
			TMP_CharacterInfo val = textInfo.characterInfo[i];
			if (val.isVisible)
			{
				int materialReferenceIndex = ((TMP_Text)textMesh).textInfo.characterInfo[i].materialReferenceIndex;
				int vertexIndex = val.vertexIndex;
				Vector3 val2 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex] + ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 1] + ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 2] + ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 3];
				val2 /= 4f;
				float num3 = val2.x / (num / 2f);
				bool flag = num3 >= 0f;
				num3 = Mathf.Abs(num3);
				float num4 = num2 / 2f * num3;
				float num5 = (180f - num4) / 2f;
				float num6 = 200f * Mathf.Tan(num4 / 2f * ((float)Math.PI / 180f));
				((Vector3)(ref val3))._002Ector(Mathf.Sin(num5 * ((float)Math.PI / 180f)) * num6 * (flag ? 1f : (-1f)), val2.y, Mathf.Cos(num5 * ((float)Math.PI / 180f)) * num6);
				Vector3 val4 = val3 - val2;
				Vector3 val5 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex] + val4;
				Vector3 val6 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 1] + val4;
				Vector3 val7 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 2] + val4;
				Vector3 val8 = ((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 3] + val4;
				Quaternion val9 = Quaternion.Euler(0f, num4 * (flag ? (-1f) : 1f), 0f);
				val5 = val9 * (val5 - val3) + val3;
				val6 = val9 * (val6 - val3) + val3;
				val7 = val9 * (val7 - val3) + val3;
				val8 = val9 * (val8 - val3) + val3;
				Vector3 val10 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(val5);
				val5 = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint(((Vector3)(ref val10)).normalized * (100f + WorldAltitudeOffsets.WorldText));
				val10 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(val6);
				val6 = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint(((Vector3)(ref val10)).normalized * (100f + WorldAltitudeOffsets.WorldText));
				val10 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(val7);
				val7 = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint(((Vector3)(ref val10)).normalized * (100f + WorldAltitudeOffsets.WorldText));
				val10 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(val8);
				val8 = ((Matrix4x4)(ref worldToLocalMatrix)).MultiplyPoint(((Vector3)(ref val10)).normalized * (100f + WorldAltitudeOffsets.WorldText));
				((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex] = val5;
				((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 1] = val6;
				((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 2] = val7;
				((TMP_Text)textMesh).textInfo.meshInfo[materialReferenceIndex].vertices[vertexIndex + 3] = val8;
			}
		}
		((TMP_Text)textMesh).UpdateVertexData((TMP_VertexDataUpdateFlags)255);
	}
}
