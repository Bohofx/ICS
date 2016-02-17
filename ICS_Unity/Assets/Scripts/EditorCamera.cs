using UnityEngine;
using System.Collections;

public class EditorCamera : MonoBehaviour
{
	[SerializeField]
	float _lookSensitivity = 90f;

	[SerializeField]
	float _fastMoveSpeed = 10f;
	
	[SerializeField]
	float _normalMoveSpeed = 5f;

	float rotationX = 0.0f;
	float rotationY = 0.0f;

	enum CameraRotationMode
	{
		PivotAboutSelf,
		PivotAboutFocal,
	}
	CameraRotationMode _cameraRotationMode = CameraRotationMode.PivotAboutSelf;

	void Start()
	{
		//Cursor.lockState = CursorLockMode.Locked;
	}

	void Update()
	{
		bool inputAlt = Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.RightAlt);
		bool inputPivotSelf = Input.GetMouseButton(1);
		bool inputPivotFocal = Input.GetMouseButton(0) && inputAlt;
		bool anyRotationKeyPressed = (inputAlt && inputPivotFocal) || (inputPivotSelf);

		if(_cameraRotationMode == CameraRotationMode.PivotAboutFocal && Input.GetMouseButton(1))
		{
			_cameraRotationMode = CameraRotationMode.PivotAboutSelf;
		}

		if(anyRotationKeyPressed)
		{
			rotationX += Input.GetAxis("Mouse X") * _lookSensitivity * Time.deltaTime;
			rotationY += Input.GetAxis("Mouse Y") * _lookSensitivity * Time.deltaTime;
			rotationY = Mathf.Clamp(rotationY, -90, 90);

			transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
			transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
		}

		bool isUsingFastScroll = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

		float moveSpeed = isUsingFastScroll ? _fastMoveSpeed : _normalMoveSpeed;
		transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
		transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;

		if(Input.GetKeyDown(KeyCode.End))
		{
			Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
		}
	}
}