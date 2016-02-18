using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// BENNOTE: Doesn't accomdate for the user destroying the affiliated Transform after it's been involved
/// in commands. In that case it'd need to eject commands referencing the destroyed Transform from
/// the UndoManager.
/// </summary>
class MoveObjectCommand : ICommand
{
	List<Transform> _transforms = new List<Transform>();

	public Vector3 WorldDelta
	{ get; set; }

	private MoveObjectCommand()
	{ }

	public MoveObjectCommand(IEnumerable<Transform> inTransforms)
	{
		_transforms.AddRange(inTransforms);
	}

	public void Do()
	{
		foreach(var transform in _transforms)
		{
			if(transform != null)
			{
				transform.position += WorldDelta;
			}
		}
	}

	public void Undo()
	{
		foreach(var transform in _transforms)
		{
			if(transform != null)
			{
				transform.position -= WorldDelta;
			}
		}
	}
}