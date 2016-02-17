using UnityEngine;
using System.Collections;
using UnityEditor;
using UObject = UnityEngine.Object;

[CustomEditor(typeof(AssetManager))]
public class AssetManagerInspector : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if(Application.isPlaying)
		{
			int newCount = AssetPath._loadedAssetPathObjects != null ? AssetPath._loadedAssetPathObjects.Count : 0;
			GUILayout.Label(newCount.ToString() + " Assets are loaded from AssetPaths");
			GUILayout.Space(10f);
			GUI.enabled = false;
			foreach(var kvp in AssetPath._loadedAssetPathObjects)
			{
				EditorGUILayout.ObjectField(kvp.Value.name, kvp.Value, typeof(UObject), false);
			}
			GUI.enabled = true;
		}
	}
}