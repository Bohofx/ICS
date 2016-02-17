using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using UObject = UnityEngine.Object;

[CustomPropertyDrawer(typeof(AssetPath))]
public class AssetPathPropertyDrawer : PropertyDrawer
{
	int kSizeTexture2D
	{ get { return 64; } }

	bool _didSearchAttribute = false;

	AssetPathDataAttribute _attribute;
	AssetPathDataAttribute assetPathData
	{
		get
		{
			if(!_didSearchAttribute && _attribute == null)
			{
				_attribute = fieldInfo.GetCustomAttributes(typeof(AssetPathDataAttribute), true).FirstOrDefault() as AssetPathDataAttribute;
				_didSearchAttribute = true;
			}
			return _attribute;
		}
	}

	public override void OnGUI(Rect inPosition, SerializedProperty inProperty, GUIContent inLabel)
	{
		EditorGUI.BeginProperty(inPosition, inLabel, inProperty);
		{
			var guid = inProperty.Get("_guid");
			var path = inProperty.Get("_path");
			var resx = inProperty.Get("_resx");
			var name = inProperty.Get("_name");
			var uobj = inProperty.Get("_uobj");

			UObject asset = null;
			
			// If we have a direct reference, great, use that.
			if(assetPathData.StoreDirectReference)
			{
				asset = uobj.objectReferenceValue;

				// If we switched from a resources path reference to direct, try to patch it together.
				if (asset == null && IsValid(inProperty))
				{
					var assetDatabaseGuid = guid.stringValue;
					var assetPath = AssetDatabase.GUIDToAssetPath(assetDatabaseGuid);
					asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UObject));
				}

			}
			// Check to see if we can load from resources OK, if requested.
			else if(assetPathData.StoreResourcePath)
			{
				asset = Resources.Load(resx.stringValue);
				if(!asset)
				{
					var assetDatabaseGuid = guid.stringValue;
					var assetPath = AssetDatabase.GUIDToAssetPath(assetDatabaseGuid);
					asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UObject));
					if(asset)
					{
						// Once the object picker is closed, block selection of non-Resources.
						if(EditorGUIUtility.GetObjectPickerControlID() <= 0 && !assetPath.Contains("/Resources/"))
						{
							// This is an error - probably not in a Resources folder.
							Debug.LogError("Cannot store reference to asset " + name.stringValue + " because it's not in a Resources folder!");
							Reset(inProperty);
						}
						else
						{
							PropagateData(asset, guid, path, resx, name, uobj, assetPathData);

							var owningObject = inProperty.serializedObject.targetObject;
							if(PrefabUtility.GetPrefabType(owningObject) == PrefabType.Prefab)
							{
								Debug.LogWarning("Had to re-establish resource/file path to " + asset.name + " for AssetPath on object " + inProperty.serializedObject.targetObject.name + ". You should resave the prefab.");
							}
						}
					}
				}
			}
			// Load from GUID?
			else
			{
				asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid.stringValue), assetPathData.AssetType);
			}

			// Check to see if we're storing a direct reference, and if that matches what the attribute desires.
			if((assetPathData.StoreDirectReference && !uobj.objectReferenceValue) || (!assetPathData.StoreDirectReference && uobj.objectReferenceValue))
				PropagateData(asset, guid, path, resx, name, uobj, assetPathData);

			var position = inPosition;
			var type = assetPathData != null ? assetPathData.AssetType : asset != null ? asset.GetType() : typeof(UObject);

			UObject newAsset = null;
			newAsset = EditorGUI.ObjectField(position, inLabel, asset, type, false);

			if(newAsset != asset)
				PropagateData(newAsset, guid, path, resx, name, uobj, assetPathData);
		}
		EditorGUI.EndProperty();
	}

	public static void PropagateData(SerializedProperty inAssetPathSerializedProperty, UObject inAsset, AssetPathDataAttribute inAttribute)
	{
		var guid = inAssetPathSerializedProperty.Get("_guid");
		var path = inAssetPathSerializedProperty.Get("_path");
		var resx = inAssetPathSerializedProperty.Get("_resx");
		var name = inAssetPathSerializedProperty.Get("_name");
		var uobj = inAssetPathSerializedProperty.Get("_uobj");

		PropagateData(inAsset, guid, path, resx, name, uobj, inAttribute);
	}

	static void PropagateData(UObject inAsset, SerializedProperty inGuid, SerializedProperty inPath, SerializedProperty inResx, SerializedProperty inName, SerializedProperty inUnityObject, AssetPathDataAttribute inAssetPathData)
	{
		string assetPath = string.Empty, resourcePath = string.Empty;
		
		assetPath = AssetDatabase.GetAssetPath(inAsset);

		// New asset has to have "Resources" in the path.
		if(inAssetPathData.StoreResourcePath && assetPath.Contains("Resources"))
		{
			// Get the path excluding "Resources".
			resourcePath = assetPath.RemoveUpToAndIncludingLast("Resources/").RemoveExtension();

			// Confirm that it actually loads via Resources correctly.
			var newAssetFromResources = Resources.Load(resourcePath);
			if(!newAssetFromResources)
				resourcePath = string.Empty;
		}
		
		var newGuid = AssetDatabase.AssetPathToGUID(assetPath);
		inGuid.stringValue = newGuid;
		inPath.stringValue = assetPath;
		inResx.stringValue = resourcePath;
		inName.stringValue = inAsset != null ? inAsset.name.RemoveExtension() : string.Empty;

		// Ensure that the direct reference is set in the way we want it.
		if(inAssetPathData.StoreDirectReference)
			inUnityObject.objectReferenceValue = inAsset;
		else if(!inAssetPathData.StoreDirectReference && inUnityObject.objectReferenceValue)
			inUnityObject.objectReferenceValue = null;
	}

	public override float GetPropertyHeight(SerializedProperty inRoot, GUIContent inLabel)
	{
		var size = EditorGUI.GetPropertyHeight(inRoot, inLabel);
		/*
		if(assetPathType != null)
		{
			if(assetPathType.AssetType == typeof(Texture2D))
				size = kSizeTexture2D;
		}
		*/
		size = 16f;
		return size;
	}

	public static bool IsValid(SerializedProperty inAssetPathProp)
	{
		if (inAssetPathProp == null)
			return false;

		var guidProp = inAssetPathProp.Get("_guid");
		return guidProp != null && !string.IsNullOrEmpty(guidProp.stringValue);
	}

	public static void Reset(SerializedProperty inAssetPathSerializedProperty)
	{
		var guid = inAssetPathSerializedProperty.Get("_guid");
		var path = inAssetPathSerializedProperty.Get("_path");
		var resx = inAssetPathSerializedProperty.Get("_resx");
		var name = inAssetPathSerializedProperty.Get("_name");
		var uobj = inAssetPathSerializedProperty.Get("_uobj");

		if (guid.stringValue != string.Empty)
			GUI.changed = true;

		guid.stringValue = string.Empty;
		path.stringValue = string.Empty;
		resx.stringValue = string.Empty;
		name.stringValue = string.Empty;
		uobj.objectReferenceValue = null;
	}
}
