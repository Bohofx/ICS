using UnityEngine;
using System.Collections;
using SimpleJSON;

public class SerializeLight : SerializeComponent<Light>
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
			component.color = inColor;
		}
	}

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
			component.color = inNode["Color"].Value.ColorFromJsonString();
		}
	}
}
