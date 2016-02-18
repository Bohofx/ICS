using UnityEngine;
using System.Collections;

public class ZoomToAndOrbitCamera : MonoBehaviour
{
	[SerializeField]
	float _zoomTime = .5f;

	[SerializeField]
	float _boundsZoomFactor = 1.15f;

	[SerializeField]
	float _orbitSpeed = 10f;

	Coroutine _zoomAndOrbitCoroutine;

	void OnEnable()
	{
		_zoomAndOrbitCoroutine = StartCoroutine(ZoomToAndOrbit_Coroutine());
	}

	void OnDisable()
	{
		if(_zoomAndOrbitCoroutine != null)
		{
			StopCoroutine(_zoomAndOrbitCoroutine);
			_zoomAndOrbitCoroutine = null;
		}
	}

	IEnumerator ZoomToAndOrbit_Coroutine()
	{
		Bounds bounds = Gizmo.GetInstance().GetBoundsForSelected();

		Vector3 startPosition = transform.position;
		Vector3 targetPosition = bounds.center + (startPosition - bounds.center).normalized * bounds.extents.magnitude * _boundsZoomFactor;

		Quaternion startRotation = transform.rotation;
		Quaternion targetRotation = Quaternion.LookRotation((targetPosition - startPosition).normalized, Vector3.up);

		float t = -Time.deltaTime;
		while(t < _zoomTime)
		{
			t += Time.deltaTime;
			t = Mathf.Clamp(t, 0f, _zoomTime);

			Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, t / _zoomTime);
			transform.position = newPosition;

			Quaternion newRotation = Quaternion.Lerp(startRotation, targetRotation, t / _zoomTime);
			transform.rotation = newRotation;

			yield return null;
		}

		do
		{
			transform.RotateAround(bounds.center, Vector3.up, _orbitSpeed * Time.deltaTime);
			yield return null;
		}
		while(true);
	}
}