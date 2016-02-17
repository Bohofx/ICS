using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SerializeMeshRenderer : SerializeComponent<MeshRenderer>
{
	public override void Serialize(JSONNode inNode)
	{
		base.Serialize(inNode);
		if(component)
		{
			inNode["Color"] = component.sharedMaterial.color.ToJsonString();
		}
	}

	public override void Deserialize(JSONNode inNode)
	{
		base.Deserialize(inNode);
		if(component)
		{
			component.sharedMaterial.color = inNode["Color"].Value.ColorFromJsonString();
		}
	}
}
