using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UndoManager : Singleton<UndoManager>
{
	Stack<ICommand> _undo = new Stack<ICommand>();
	Stack<ICommand> _redo = new Stack<ICommand>();

	// Adds to the undo stack without executing Do().
	public void Record(ICommand inCommand)
	{
		_undo.Push(inCommand);
		_redo.Clear();
	}

	public void Do(ICommand inCommand)
	{
		inCommand.Do();
		_undo.Push(inCommand);
		_redo.Clear();
	}

	public void Undo()
	{
		if(_undo.Count > 0)
		{
			ICommand command = _undo.Pop();
			command.Undo();
			_redo.Push(command);
		}
	}

	public void Redo()
	{
		if(_redo.Count > 0)
		{
			ICommand command = _redo.Pop();
			command.Do();
			_undo.Push(command);
		}
	}

	void Update()
	{
		bool modifierDown =
#if UNITY_EDITOR
			Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#else
			Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
#endif
		if(Input.GetKeyDown(KeyCode.Z) && modifierDown)
		{
			Undo();
		}
		else if(Input.GetKeyDown(KeyCode.Y) && modifierDown)
		{
			Redo();
		}
	}

	public void Reset()
	{
		_undo.Clear();
		_redo.Clear();
	}
}
