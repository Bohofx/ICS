using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SerializeTransform : SerializeComponent<Transform>
{
	public override void Serialize(JSONNode inNode)
	{
		base.Serialize(inNode);
		inNode["Position"] = component.position.ToJsonString();
		inNode["Rotation"] = component.rotation.ToJsonString();
		inNode["LocalScale"] = component.localScale.ToJsonString();
	}

	public override void Deserialize(JSONNode inNode)
	{
		base.Deserialize(inNode);
		component.position = inNode["Position"].Value.Vector3FromJsonString();
		component.rotation = inNode["Rotation"].Value.QuaternionFromJsonString();
		component.localScale = inNode["LocalScale"].Value.Vector3FromJsonString();
	}
}
