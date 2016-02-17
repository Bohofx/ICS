using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SerializeLight : SerializeComponent<Light>
{
	public override void Serialize(JSONNode inNode)
	{
		base.Serialize(inNode);
		if(component)
		{
			inNode["Intensity"] = component.intensity.ToString();
			inNode["Color"] = component.color.ToJsonString();
		}
	}

	public override void Deserialize(JSONNode inNode)
	{
		base.Deserialize(inNode);
		if(component)
		{
			component.intensity = inNode["Intensity"].AsFloat;
			component.color = inNode["Color"].ToString().ColorFromJsonString();
		}
	}
}
