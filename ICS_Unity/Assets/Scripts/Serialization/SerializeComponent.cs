using UnityEngine;
using System.Collections;

public abstract class SerializeComponent<T> : SerializeBase where T : Component
{
	T _cachedComponent = null;
	protected T component
	{
		get
		{
			if(_cachedComponent == null)
				_cachedComponent = gameObject.GetComponentInChildren<T>();
			return _cachedComponent;
		}
	}

	PrefabInstance _instanceParent = null;
	protected PrefabInstance instanceParent
	{
		get
		{
			if(_instanceParent == null)
				_instanceParent = gameObject.GetComponentInParent<PrefabInstance>();
			return _instanceParent;
		}
	}
}
