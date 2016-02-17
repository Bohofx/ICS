using UnityEngine;
using System.Collections;

public class PickableObject : MonoBehaviour
{
	Color _originalColor;

	IEnumerator OnMouseEnter()
	{
		var renderer = GetComponent<Renderer>();
		_originalColor = renderer.material.color;
		renderer.material.color = Color.red;
		while(!Input.GetMouseButton(0))
		{
			yield return null;
		}
		renderer.material.color = _originalColor;
	}

	void OnMouseExit()
	{
		var renderer = GetComponent<Renderer>();
		renderer.material.color = _originalColor;
	}
}
