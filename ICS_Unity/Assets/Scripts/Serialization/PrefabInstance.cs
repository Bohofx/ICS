using UnityEngine;
using System.Collections;
using SimpleJSON;

public class PrefabInstance : MonoBehaviour
{
	[SerializeField, AssetPathData(typeof(GameObject), StoreResourcePath = true)]
	AssetPath _prefabSource;
	public AssetPath PrefabSource
	{ set { _prefabSource = value; } }

	string _instanceGuid;
	public string InstanceGUID
	{ get { return _instanceGuid; } }

	protected virtual void Awake()
	{
		_instanceGuid = System.Guid.NewGuid().ToString();
		Environment.GetInstance().AddInstance(this);
	}

	protected virtual void OnDestroy()
	{
		if(Environment.HasInstance())
			Environment.GetInstance().RemoveInstance(this);
	}

	public void Serialize(JSONNode inNode)
	{
		inNode["ResourceName"] = _prefabSource.name;
		inNode["ResourcePath"] = _prefabSource.resx;
		inNode["ResourceGUID"] = _prefabSource.guid;
		inNode["InstanceGUID"] = _instanceGuid;

		inNode["SerializedComponents"] = new JSONArray();

		SerializeComponentBase[] serializedComponents = gameObject.GetComponentsInChildren<SerializeComponentBase>();
		for(int i = 0; i < serializedComponents.Length; ++i)
		{
			string nodeName = serializedComponents[i].name + "_" + serializedComponents[i].GetType();
			inNode["SerializedComponents"][i] = new JSONClass();
			serializedComponents[i].Serialize(inNode["SerializedComponents"][i]);
		}
	}

	public void Deserialize(JSONNode inNode)
	{
		_instanceGuid = inNode["InstanceGUID"];

		var count = inNode["SerializedComponents"].Count;
		for(int i = 0; i < count; ++i)
		{
			JSONNode serializedComponent = inNode["SerializedComponents"][i];
			
			string path = serializedComponent["Path"];
			
			Transform sourceGameObject = transform.FindChild(path);
			if(sourceGameObject)
			{
				string type = serializedComponent["Type"];

				SerializeComponentBase component = sourceGameObject.GetComponent(type) as SerializeComponentBase;
				if(component)
				{
					component.Deserialize(inNode);
				}
			}
		}
	}
}
