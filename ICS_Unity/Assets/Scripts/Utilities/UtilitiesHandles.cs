using UnityEngine;
using System.Collections;

public static class UtilitiesHandles
{
	// UnityEditor.HandleUtility
	/// <summary>
	///   <para>Map a mouse drag onto a movement along a line in 3D space.</para>
	/// </summary>
	/// <param name="src">The source point of the drag.</param>
	/// <param name="dest">The destination point of the drag.</param>
	/// <param name="srcPosition">The 3D position the dragged object had at src ray.</param>
	/// <param name="constraintDir">3D direction of constrained movement.</param>
	/// <returns>
	///   <para>The distance travelled along constraintDir.</para>
	/// </returns>
	public static float CalcLineTranslation(Vector2 src, Vector2 dest, Vector3 srcPosition, Vector3 constraintDir, Matrix4x4 handleMatrix)
	{
		srcPosition = handleMatrix.MultiplyPoint(srcPosition);
		constraintDir = handleMatrix.MultiplyVector(constraintDir);
		float num = 1f;
		Vector3 forward = Camera.main.transform.forward;
		if(Vector3.Dot(constraintDir, forward) < 0f)
		{
			num = -1f;
		}
		Vector3 vector = constraintDir;
		vector.y = -vector.y;
		Camera current = Camera.main;
		Vector2 vector2 = PixelsToPoints(current.WorldToScreenPoint(srcPosition));
		Vector2 vector3 = PixelsToPoints(current.WorldToScreenPoint(srcPosition + constraintDir * num));
		Vector2 x = dest;
		Vector2 x2 = src;
		if(vector2 == vector3)
		{
			return 0f;
		}
		x.y = -x.y;
		x2.y = -x2.y;
		float parametrization = GetParametrization(x2, vector2, vector3);
		float parametrization2 = GetParametrization(x, vector2, vector3);
		return (parametrization2 - parametrization) * num;
	}

	// UnityEditor.HandleUtility
	internal static float GetParametrization(Vector2 x0, Vector2 x1, Vector2 x2)
	{
		return -(Vector2.Dot(x1 - x0, x2 - x1) / (x2 - x1).sqrMagnitude);
	}

	internal static Vector2 PixelsToPoints(Vector2 position)
	{
		float num = 1f / pixelsPerPoint;
		position.x *= num;
		position.y *= num;
		return position;
	}

	internal new static float pixelsPerPoint
	{
		get
		{
			return 1;
			//return GUIUtility.pixelsPerPoint;
		}
	}
}
