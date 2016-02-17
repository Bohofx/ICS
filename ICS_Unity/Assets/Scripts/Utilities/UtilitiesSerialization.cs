using System;
using UnityEngine;

public static class UtilitiesSerialization
{
	public static string ToJsonString(this Color inColor)
	{
		return JsonUtility.ToJson(inColor);
	}

	public static Color ColorFromJsonString(this string inJson)
	{
		return (Color)JsonUtility.FromJson(inJson, typeof(Color));
	}

	public static string ToJsonString(this Vector3 inVector)
	{
		return JsonUtility.ToJson(inVector);
	}

	public static Vector3 Vector3FromJsonString(this string inJson)
	{
		return (Vector3)JsonUtility.FromJson(inJson, typeof(Vector3));
	}

	public static string ToJsonString(this Quaternion inQuaternion)
	{
		return JsonUtility.ToJson(inQuaternion);
	}

	public static Quaternion QuaternionFromJsonString(this string inJson)
	{
		return (Quaternion)JsonUtility.FromJson(inJson, typeof(Quaternion));
	}
}