using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MonoBehaviourNote))]
public class MonoBehaviourNoteEditor : Editor
{
	MonoBehaviourNote note
	{ get { return target as MonoBehaviourNote; } }

	public override void OnInspectorGUI()
	{
		EditorGUILayout.Space();
		if(GUILayout.Button(new GUIContent(note.unlocked ? "Lock" : "Unlock"), GUILayout.ExpandWidth(false)))
			note.unlocked = !note.unlocked;

		var style = note.unlocked ? EditorStyles.textField : GetLabelStyle();

		EditorGUILayout.Space();
		if(note.unlocked)
		{			
			string newText = EditorGUILayout.TextArea(note.text, style, GUILayout.MaxWidth(Screen.width));
			if(newText != note.text)
			{
				note.text = newText;
				EditorUtility.SetDirty(note);
			}
		}
		else
		{
			var markedUpText = MarkupNativeText(note.text, Color.yellow, true, false);
			var width = Screen.width * .85f;
			float height = style.CalcHeight(new GUIContent(markedUpText), width);
			EditorGUILayout.SelectableLabel(markedUpText, style, new GUILayoutOption[] { GUILayout.Height(height), GUILayout.MaxWidth(width) });
		}
	}

	[System.NonSerialized]
	static GUIStyle _labelStyle;
	GUIStyle GetLabelStyle()
	{
		if(_labelStyle == null)
		{
			_labelStyle = new GUIStyle(GUI.skin.label);
			_labelStyle.richText = true;
			_labelStyle.fontSize = 14;
			_labelStyle.wordWrap = true;
		}
		return _labelStyle;
	}

	static string MarkupNativeText(string inText, Color inColor, bool inBold, bool inItalic)
	{
		var text = "<color=#" + ColorToRGBHex(inColor) + ">" + inText + "</color>";
		if(inBold)
			text = "<b>" + text + "</b>";
		if(inItalic)
			text = "<i>" + text + "</i>";
		return text;
	}

	static string ColorToRGBHex(Color color)
	{
		string rHex = ((int)(color.r * 255)).ToString("x2");
		string gHex = ((int)(color.g * 255)).ToString("x2");
		string bHex = ((int)(color.b * 255)).ToString("x2");
		return rHex + gHex + bHex;
	}
}