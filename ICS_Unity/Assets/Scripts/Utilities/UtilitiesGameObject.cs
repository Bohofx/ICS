using System;
using UnityEditor;
using UnityEngine;

public static class UtilitiesGameObject
{
	public static string GetHierarchyPath(Component inComponent, Transform inRelativeTo)
	{
		if(inComponent)
		{
			string path = string.Empty;
			if(inRelativeTo == null || inComponent.transform != inRelativeTo)
			{
				while(inComponent && inComponent.transform.parent)
				{
					inComponent = inComponent.transform.parent;
					if(inRelativeTo != null && inComponent == inRelativeTo)
					{
						break;
					}
					else
					{
						path = inComponent.name + "/" + path;
					}
				}
			}
			return path;
		}
		return string.Empty;
	}
}