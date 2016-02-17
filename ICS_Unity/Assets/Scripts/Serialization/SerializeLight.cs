using UnityEngine;
using System.Collections;

public class SerializeLight : SerializeComponent<Light>
{
	[System.Serializable]
	struct LightData
	{
		public float Intensity;
		public Color Color;
	}

	public override object Serialize()
	{
		LightData lightData = new LightData();
		if(component)
		{
			lightData.Color = component.color;
			lightData.Intensity = component.intensity;
		}
		return lightData;
	}

	public override void DeserializeFromJson(string inJson)
	{
		if(component)
		{
			LightData lightData = JsonUtility.FromJson<LightData>(inJson);
			component.intensity = lightData.Intensity;
			component.color = lightData.Color;
		}
	}
}
