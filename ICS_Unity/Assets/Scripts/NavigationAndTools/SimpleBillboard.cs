using UnityEngine;
using System.Collections;

public class SimpleBillboard : MonoBehaviour
{
	void Update()
	{
		transform.LookAt(Camera.main.transform, Vector3.up);
	}
}
