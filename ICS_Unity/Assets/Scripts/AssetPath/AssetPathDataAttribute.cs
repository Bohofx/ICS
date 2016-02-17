using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class AssetPathDataAttribute : PropertyAttribute
{
	Type _assetType;
	public Type AssetType
	{ get { return _assetType; } }

	public bool StoreDirectReference
	{ get; set; }

	public bool StoreResourcePath
	{ get; set; }

	public AssetPathDataAttribute(Type inType)
	{
		_assetType = inType;
	}
}