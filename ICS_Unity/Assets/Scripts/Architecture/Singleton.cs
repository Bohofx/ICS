using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using UObject = UnityEngine.Object;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	static T _instance = null;

	public static T GetInstance()
	{
		if(_instance == null)
		{
			_instance = GameObject.FindObjectOfType<T>();
			if(_instance != null)
			{
				var singleton = _instance as Singleton<T>;
				singleton.OnEstablishInstance();
			}
		}
		return _instance;
	}

	public static bool HasInstance()
	{
		return _instance != null;
	}

	protected virtual void OnEstablishInstance()
	{ }

	protected virtual void Awake()
	{
		if(_instance == null)
		{
			_instance = this as T;

			var singleton = _instance as Singleton<T>;
			singleton.OnEstablishInstance();
		}
	}

	protected virtual void OnDestroy()
	{
		if(_instance == this)
			_instance = null;
	}
}