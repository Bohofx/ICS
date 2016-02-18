using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToolsManager : Singleton<ToolsManager>
{
	[SerializeField]
	Gizmo _gizmo = null;

	HashSet<PickableObject> _selectedObjects = new HashSet<PickableObject>();

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			ClearAllSelected();
		}
	}

	public void AddSelected(PickableObject inPickableObject)
	{
		bool inputShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);

		if(!inputShiftDown)
		{
			ClearAllSelected();
		}
		
		if(inPickableObject)
		{
			if(!_selectedObjects.Contains(inPickableObject))
			{
				_selectedObjects.Add(inPickableObject);
				_gizmo.SelectObject(inPickableObject.transform);
				inPickableObject.SetSelected(true);
			}
			else
			{
				_selectedObjects.Remove(inPickableObject);
				_gizmo.DeselectObject(inPickableObject.transform);
				inPickableObject.SetSelected(false);
			}
		}

		if(_selectedObjects.Count > 0)
		{
			_gizmo.Show();
		}
		else
		{
			_gizmo.Hide();
		}
	}

	void ClearAllSelected()
	{
		var enumerator = _selectedObjects.GetEnumerator();
		while(enumerator.MoveNext())
		{
			enumerator.Current.SetSelected(false);
		}
		_selectedObjects.Clear();
		_gizmo.ClearSelection();
	}
}
