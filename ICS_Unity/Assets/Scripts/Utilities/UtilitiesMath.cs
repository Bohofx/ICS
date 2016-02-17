using UnityEngine;
using System.Collections;

public static class UtilitiesMath
{
	public static Vector3 TLF(this Bounds b)
	{ return new Vector3(b.min.x, b.max.y, b.max.z); }

	public static Vector3 TRF(this Bounds b)
	{ return new Vector3(b.max.x, b.max.y, b.max.z); }

	public static Vector3 BRF(this Bounds b)
	{ return new Vector3(b.max.x, b.min.y, b.max.z); }

	public static Vector3 BLF(this Bounds b)
	{ return new Vector3(b.min.x, b.min.y, b.max.z); }

	public static Vector3 TLB(this Bounds b)
	{ return new Vector3(b.min.x, b.max.y, b.min.z); }

	public static Vector3 TRB(this Bounds b)
	{ return new Vector3(b.max.x, b.max.y, b.min.z); }

	public static Vector3 BRB(this Bounds b)
	{ return new Vector3(b.max.x, b.min.y, b.min.z); }

	public static Vector3 BLB(this Bounds b)
	{ return new Vector3(b.min.x, b.min.y, b.min.z); }
}
