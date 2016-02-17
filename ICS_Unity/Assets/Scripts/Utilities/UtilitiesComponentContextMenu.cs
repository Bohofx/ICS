using System;
using UnityEditor;
using UnityEngine;

public static class UtilitiesComponentContextMenu
{
	[MenuItem("CONTEXT/Component/Move To Top")]
	static void MoveComponentToTop(MenuCommand menuCommand)
	{
		var component = menuCommand.context as Component;
		var moved = false;
		do
		{
			moved = UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
		}
		while(moved);
	}

	[MenuItem("CONTEXT/Component/Move To Bottom")]
	static void MoveComponentToBottom(MenuCommand menuCommand)
	{
		var component = menuCommand.context as Component;
		var moved = false;
		do
		{
			moved = UnityEditorInternal.ComponentUtility.MoveComponentDown(component);
		}
		while(moved);
	}
}