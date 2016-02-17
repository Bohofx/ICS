using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Environment : Singleton<Environment>
{
	List<PrefabInstance> _sceneInstances = new List<PrefabInstance>();

	void Start()
	{
		SerializeScene();
	}

	#if UNITY_EDITOR
	[ContextMenu("Serialize scene")]
	#endif
	void SerializeScene()
	{
		var jsonNode = JSON.Parse("{ }");
		jsonNode["Version"] = "1";

		jsonNode["SceneInstances"] = new JSONArray();
		for(int i = 0; i < _sceneInstances.Count; ++i)
		{
			jsonNode["SceneInstances"][i] = new JSONClass();
			_sceneInstances[i].Serialize(jsonNode["SceneInstances"][i]);
		}

		jsonNode.SaveToFile("TestSerialization.dat");
		
		#if UNITY_EDITOR
		Debug.Log(jsonNode.ToString(), this);
		#endif
	}

	#if UNITY_EDITOR
	[ContextMenu("Deserialize scene")]
	#endif
	void DeserializeScene()
	{
		ClearScene();
		using(var stream = File.OpenRead("TestSerialization.dat"))
		{
			using(var reader = new BinaryReader(stream))
			{
				var node = JSONNode.Deserialize(reader);
				var count = node["SceneInstances"].Count;
				for(int i = 0; i < count; ++i)
				{
					JSONNode instanceNode = node["SceneInstances"][i];
					PrefabInstance prefabInstance = CreatePrefabInstance(instanceNode);
					if(prefabInstance)
					{
						prefabInstance.Deserialize(instanceNode);
					}
				}
			}
		}
	}

	void ClearScene()
	{
		while(_sceneInstances.Count > 0)
		{
			Destroy(_sceneInstances[0].gameObject);
			_sceneInstances.RemoveAt(0);
		}
	}

	static PrefabInstance CreatePrefabInstance(JSONNode inNode)
	{
		AssetPath prefabSource = new AssetPath()
		{
			name = inNode["ResourceName"].ToString().Replace("\"", string.Empty),
			resx = inNode["ResourcePath"].ToString().Replace("\"", string.Empty),
			guid = inNode["ResourceGUID"].ToString().Replace("\"", string.Empty),
		};
		
		GameObject resource = prefabSource.Load<GameObject>();
		
		GameObject gameObjectInstance = GameObject.Instantiate<GameObject>(resource);
		
		PrefabInstance prefabInstance = gameObjectInstance.GetComponentInChildren<PrefabInstance>(true);
		if(prefabInstance)
		{
			prefabInstance.PrefabSource = prefabSource;
		}
		return prefabInstance;
	}

	internal void AddInstance(PrefabInstance inInstance)
	{
		if(inInstance != null)
		{
			_sceneInstances.Add(inInstance);
		}
	}

	internal void RemoveInstance(PrefabInstance inInstance)
	{
		if(inInstance != null)
		{
			_sceneInstances.Remove(inInstance);
		}
	}
}
