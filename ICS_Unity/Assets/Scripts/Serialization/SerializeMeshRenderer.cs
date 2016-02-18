using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SerializeMeshRenderer : SerializeComponent<MeshRenderer>
{
	protected override void OnSelectionStateChanged(bool inState)
	{
		base.OnSelectionStateChanged(inState);
		if(inState)
		{
			HUD.GetInstance().ShowColorPicker.AddRequest(new LensHandle(this));
			HUD.GetInstance().ColorPicker.onValueChanged.AddListener(UpdateColor);
		}
		else
		{
			HUD.GetInstance().ShowColorPicker.RemoveRequestsWithContext(this);
			HUD.GetInstance().ColorPicker.onValueChanged.RemoveListener(UpdateColor);
		}
	}

	void UpdateColor(Color inColor)
	{
		if(component)
		{
			component.material.color = inColor;
		}
	}

	public override void Serialize(JSONNode inNode)
	{
		base.Serialize(inNode);
		if(component)
		{
			inNode["Color"] = component.material.color.ToJsonString();
		}
	}

	public override void Deserialize(JSONNode inNode)
	{
		base.Deserialize(inNode);
		if(component)
		{
			component.material.color = inNode["Color"].Value.ColorFromJsonString();
		}
	}
}
