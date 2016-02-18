using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
	[SerializeField]
	EditorCamera _editorCamera;

	[SerializeField]
	ZoomToAndOrbitCamera _zoomToAndOrbitCamera;

	void Awake()
	{
		_editorCamera.enabled = true;
		_zoomToAndOrbitCamera.enabled = false;
	}

	void Update()
	{
		bool anyArrowKeyDown = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow);
		bool anyMouse = Input.GetMouseButton(0) || Input.GetMouseButton(1);

		if(_editorCamera.enabled && Input.GetKeyDown(KeyCode.F) && Gizmo.GetInstance().SelectedCount > 0)
		{
			_editorCamera.enabled = false;
			_zoomToAndOrbitCamera.enabled = true;
		}
		else if(_zoomToAndOrbitCamera.enabled && anyArrowKeyDown || anyMouse)
		{
			_editorCamera.enabled = true;
			_zoomToAndOrbitCamera.enabled = false;
		}
	}
}
