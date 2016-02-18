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

	float _rotationX = 0.0f;
	float _rotationY = 0.0f;

	enum CameraRotationMode
	{
		PivotAboutSelf,
		PivotAboutFocal,
	}
	CameraRotationMode _cameraRotationMode = CameraRotationMode.PivotAboutSelf;

	void Awake()
	{
		var euler = transform.localRotation.eulerAngles;
		_rotationY = -euler.x;
		_rotationX = euler.y;
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
			_rotationX += Input.GetAxis("Mouse X") * _lookSensitivity * Time.deltaTime;
			_rotationY += Input.GetAxis("Mouse Y") * _lookSensitivity * Time.deltaTime;
			_rotationY = Mathf.Clamp(_rotationY, -90, 90);

			transform.localRotation = Quaternion.AngleAxis(_rotationX, Vector3.up);
			transform.localRotation *= Quaternion.AngleAxis(_rotationY, Vector3.left);
		}

		bool isUsingFastScroll = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

		float moveSpeed = isUsingFastScroll ? _fastMoveSpeed : _normalMoveSpeed;
		transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
		transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
	}
}