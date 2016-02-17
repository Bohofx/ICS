using UnityEngine;
using System.Collections;

public class ToolsManager : Singleton<ToolsManager>
{
	PickableObject _selectedObject = null;
	public PickableObject SelectedObject
	{ get { return _selectedObject; } }

	public void SetSelected(PickableObject inPickableObject)
	{
		if(_selectedObject)
		{
			_selectedObject.SetSelected(false);
		}
		
		if(inPickableObject)
		{
			inPickableObject.SetSelected(true);
		}

		_selectedObject = inPickableObject;
	}
}
