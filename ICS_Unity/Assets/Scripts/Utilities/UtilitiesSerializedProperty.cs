using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;

public static class UtilitiesSerializedProperty
{
	public static SerializedProperty Get(this SerializedProperty inProperty, string inName)
	{
		return inProperty.FindPropertyRelative(inName);
	}
}
