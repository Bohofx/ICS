using UnityEngine;
using System.Collections;

public abstract class SerializeComponent<T> : SerializeComponentBase where T : Component
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
}
