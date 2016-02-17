using System;
using UnityEngine;
using System.Collections.Generic;

public class AssetManager : Singleton<AssetManager>
{
	int _lastCount = -1;
	int _maxCount = -1;

	void Update()
	{
		int newCount = AssetPath._loadedAssetPathObjects != null ? AssetPath._loadedAssetPathObjects.Count : 0;
		if (newCount != _lastCount)
		{
			_maxCount = Mathf.Max(_maxCount, newCount);
			name = "AssetManager (" + newCount + " / " + _maxCount + ")";
			_lastCount = newCount;
		}
	}
}

