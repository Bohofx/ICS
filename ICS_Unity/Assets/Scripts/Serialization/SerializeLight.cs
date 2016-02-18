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
			// If nothing else is selected, initialize the color picker to the color of the light.
			if(Gizmo.GetInstance().SelectedCount == 1 && component)
			{
				HUD.GetInstance().ColorPicker.CurrentColor = component.color;
			}

			HUD.GetInstance().ShowColorPicker.AddRequest(new LensHandle(this));
			HUD.GetInstance().ColorPicker.onValueChanged.AddListener(UpdateColor);

			HUD.GetInstance().ShowFloatInspector.AddRequest(new LensHandle(this));
			HUD.GetInstance().WidgetFloatInspector.onValueChanged.AddListener(UpdateIntensity);
		}
		else
		{
			HUD.GetInstance().ShowColorPicker.RemoveRequestsWithContext(this);
			HUD.GetInstance().ColorPicker.onValueChanged.RemoveListener(UpdateColor);

			HUD.GetInstance().ShowFloatInspector.RemoveRequestsWithContext(this);
			HUD.GetInstance().WidgetFloatInspector.onValueChanged.RemoveListener(UpdateIntensity);
		}
	}

	void UpdateIntensity(float inValue)
	{
		if(component)
		{
			component.intensity = inValue;
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
