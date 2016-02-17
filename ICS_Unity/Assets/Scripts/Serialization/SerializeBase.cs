using UnityEngine;
using System.Collections;

public abstract class SerializeBase : MonoBehaviour, ISceneSerializable
{
	public abstract object Serialize();

	public abstract void DeserializeFromJson(string inJson);
}
