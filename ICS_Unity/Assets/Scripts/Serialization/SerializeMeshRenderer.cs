using UnityEngine;
using System.Collections;

public class SerializeMeshRenderer : SerializeComponent<MeshRenderer>
{
	[System.Serializable]
	struct MeshRendererData
	{
		public Color Color;
	}

	public override object Serialize()
	{
		MeshRendererData meshRendererData = new MeshRendererData();
		if(component)
		{
			meshRendererData.Color = component.sharedMaterial.color;
		}
		return meshRendererData;
	}

	public override void DeserializeFromJson(string inJson)
	{
		if(component)
		{
			MeshRendererData meshRendererData = JsonUtility.FromJson<MeshRendererData>(inJson);
			component.sharedMaterial.color = meshRendererData.Color;
		}
	}
}
