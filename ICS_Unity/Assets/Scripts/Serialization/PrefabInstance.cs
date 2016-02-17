using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PrefabInstance : MonoBehaviour, ISceneSerializable
{
	[SerializeField, AssetPathData(typeof(GameObject), StoreResourcePath = true)]
	AssetPath _prefabSource;

	public string ResourcePath
	{ get { return _prefabSource.resx; } }

	string _guid;

	protected virtual void Awake()
	{
		_guid = System.Guid.NewGuid().ToString();
		Environment.GetInstance().AddInstance(this);
	}

	protected virtual void OnDestroy()
	{
		if(Environment.HasInstance())
			Environment.GetInstance().RemoveInstance(this);
	}

	public void Serialize(JSONNode inNode)
	{
		inNode[]
	}

	public void Deserialize(JSONNode inNode)
	{
		PrefabInstanceData prefabInstanceData = JsonUtility.FromJson<PrefabInstanceData>(inJson);
		_guid = prefabInstanceData.Guid;
		_prefabSource = prefabInstanceData.PrefabSource;
	}
}
