using UnityEngine;
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
	string SerializeScene()
	{
		var jsonNode = JSON.Parse("{ }");
		foreach(var instance in _sceneInstances)
		{
			instancesOutput.Append(gameObjectAsJson);
		}

		string result = instancesOutput.ToString();
		#if UNITY_EDITOR
		Debug.Log(result, this);
		#endif

		return result;
	}

	void DeserializeScene()
	{
		
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
			_sceneInstances.Remove(inSerializeBase);
		}
	}
}
