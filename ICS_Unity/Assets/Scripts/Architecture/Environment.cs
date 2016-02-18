using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class Environment : Singleton<Environment>
{
	List<PrefabInstance> _sceneInstances = new List<PrefabInstance>();

	#if UNITY_EDITOR
	[ContextMenu("Serialize scene")]
#endif
	void SerializeScene()
	{
		SerializeScene("TestSerialization.dat");
	}
	
	public void SerializeScene(string inFilePath)
	{
		var jsonNode = JSON.Parse("{ }");
		jsonNode["Version"] = "1";

		jsonNode["SceneInstances"] = new JSONArray();
		for(int i = 0; i < _sceneInstances.Count; ++i)
		{
			jsonNode["SceneInstances"][i] = new JSONClass();
			_sceneInstances[i].Serialize(jsonNode["SceneInstances"][i]);
		}

		jsonNode.SaveToFile(inFilePath);
		
		#if UNITY_EDITOR
		Debug.Log(jsonNode.ToString(), this);
		#endif
	}

	#if UNITY_EDITOR
	[ContextMenu("Deserialize scene")]
#endif
	void DeserializeScene()
	{
		DeserializeScene("TestSerialization.dat");
	}

	public void DeserializeScene(string inFilePath)
	{
		ClearScene();
		using(var stream = File.OpenRead(inFilePath))
		{
			using(var reader = new BinaryReader(stream))
			{
				var node = JSONNode.Deserialize(reader);
				var count = node["SceneInstances"].Count;
				for(int i = 0; i < count; ++i)
				{
					JSONNode instanceNode = node["SceneInstances"][i];
					PrefabInstance prefabInstance = CreatePrefabInstanceFromJson(instanceNode);
					if(prefabInstance)
					{
						prefabInstance.Deserialize(instanceNode);
					}
				}
			}
		}
	}

	public void ClearScene()
	{
		Gizmo.GetInstance().ClearSelection();
		UndoManager.GetInstance().Reset();
		while(_sceneInstances.Count > 0)
		{
			Destroy(_sceneInstances[0].gameObject);
			_sceneInstances.RemoveAt(0);
		}
	}

	static PrefabInstance CreatePrefabInstanceFromJson(JSONNode inNode)
	{
		AssetPath assetPath = new AssetPath()
		{
			name = inNode["ResourceName"].Value,
			resx = inNode["ResourcePath"].Value,
			guid = inNode["ResourceGUID"].Value,
		};

		PrefabInstance instance = PrefabInstance.CreateFromAssetPath(assetPath);
		return instance;
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
