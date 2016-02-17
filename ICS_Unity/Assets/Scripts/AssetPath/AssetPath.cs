using System;
using UnityEngine;
using UObject = UnityEngine.Object;
using System.Collections.Generic;

[Serializable]
public struct AssetPath
{
	[SerializeField]
	string _guid;
	public string guid
	{
		get { return _guid; }
		set { _guid = value; }
	}

	[SerializeField]
	string _path;
	public string path
	{
		get { return _path; }
		set { _path = value; }
	}
	
	[SerializeField]
	string _resx;
	public string resx
	{
		get { return _resx; }
		set { _resx = value; }
	}

	[SerializeField]
	string _name;
	public string name
	{
		get { return _name; }
		set { _name = value; }
	}

	[SerializeField]
	UObject _uobj;
	public UObject uobj
	{
		get { return _uobj; }
		set { _uobj = value; }
	}

	public bool IsValid()
	{ return !string.IsNullOrEmpty(_guid); }

	#region Resources.Load-like functionality

	public static Dictionary<string, UObject> _loadedAssetPathObjects = new Dictionary<string, UObject>();

	public T Load<T>() where T : UObject
	{
		return GetAssetByAssetPath<T>(this);
	}

	public static T GetAssetByAssetPath<T>(AssetPath assetPath) where T : UObject
	{
		if(!assetPath.IsValid())
		{
			Debug.LogError("Tried to load an invalid asset path.");
			return default(T);
		}

		if(assetPath.uobj != null)
		{
			Debug.Assert(assetPath.uobj.GetType() == typeof(T) || assetPath.uobj.GetType().IsAssignableFrom(typeof(T)) || assetPath.uobj.GetType().IsSubclassOf(typeof(T)), "AssetPath has a direct reference but it's a " + assetPath.uobj.GetType() + " and not of the demanded type " + typeof(T) + "\nPath: " + assetPath.path);
			return assetPath.uobj as T;
		}

		UObject asset;
		if(_loadedAssetPathObjects.TryGetValue(assetPath.guid, out asset))
			return asset as T;

		Debug.Assert(!string.IsNullOrEmpty(assetPath.resx));
		T freshAsset = Resources.Load<T>(assetPath.resx);
		_loadedAssetPathObjects[assetPath.guid] = freshAsset;

		return freshAsset;
	}

	public void Unload()
	{
		UObject loadedAsset = null;
		if (_loadedAssetPathObjects.TryGetValue(guid, out loadedAsset))
		{
			Resources.UnloadAsset(loadedAsset);
			_loadedAssetPathObjects.Remove(guid);
		}
		else
		{
			Debug.LogError("Asset " + name + " is not loaded and cannot be unloaded.");
		}
	}

	#endregion

	#region Equals Operator Overloading
	
	public override bool Equals(System.Object obj)
	{
		return obj is AssetPath && this == (AssetPath)obj;
	}
	
	public override int GetHashCode()
	{
		int hashCode;
		if(_guid == null)
			hashCode = string.Empty.GetHashCode();
		else
			hashCode = _guid.GetHashCode();
		return hashCode;
	}
	
	public static bool operator ==(AssetPath a, AssetPath b)
	{
		// Opportunity for optimization, perhaps?
		return a._guid == b.guid;
	}
	
	public static bool operator !=(AssetPath a, AssetPath b)
	{
		return !(a == b);
	}
	#endregion
}

