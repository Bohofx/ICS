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
			// If nothing else is selected, initialize the color picker to the color of the MeshRenderer material.
			if(Gizmo.GetInstance().SelectedCount == 1 && component)
			{
				HUD.GetInstance().ColorPicker.CurrentColor = component.material.color;
			}

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
