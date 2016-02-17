using UnityEngine;
using System.Collections;

public class SerializeTransform : SerializeComponent<Transform>
{
	[System.Serializable]
	struct TransformData
	{
		public Vector3    Position;
		public Quaternion Rotation;
		public Vector3    LocalScale;
	}

	public override object Serialize()
	{
		TransformData transformData = new TransformData() { Position = component.position, Rotation = component.rotation, LocalScale = component.localScale };
		return transformData;
	}

	public override void DeserializeFromJson(string inJson)
	{
		TransformData transformData = JsonUtility.FromJson<TransformData>(inJson);
		component.position = transformData.Position;
		component.rotation = transformData.Rotation;
		component.localScale = transformData.LocalScale;
	}
}
