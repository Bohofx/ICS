using UnityEngine;
using System.Collections;

public static class UtilitiesComponent
{
	public static T GetOrAddComponent<T>(this Component inComponent) where T : Component
	{
		return inComponent.gameObject.GetOrAddComponent<T>();
	}

	public static T GetOrAddComponent<T>(this GameObject inGameObject) where T : Component
	{
		T t = inGameObject.GetComponent<T>();
		if(t == null)
		{
			t = inGameObject.AddComponent<T>();
		}
		return t;
	}
}
