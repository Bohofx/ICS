using UnityEngine;
using System.Collections.Generic;

using System;
using UObject = UnityEngine.Object;

[System.Serializable]
public class LensHandle
{
	// Requesting context.
	UObject _context;
	public UObject context
	{ get { return _context; } }

	// The requesting context's name for debug error info when destroyed incorrectly.
	string _contextName;
	public string contextName
	{ get { return _contextName; } }

	public LensHandle(UObject inContext)
	{
		_context = inContext;
		
		if(inContext)
			_contextName = inContext.name;
	}
}

#region Common LensHandle derived
[System.Serializable]
public class FloatLensHandle : LensHandle
{
	float _val;
	public float Value
	{ get { return _val; } }
	public FloatLensHandle(UObject inContext, float inVal) : base(inContext)
	{
		_val = inVal;
	}
}

public class IntLensHandle : LensHandle
{
	int _val;
	public float Value
	{ get { return _val; } }
	public IntLensHandle(UObject inContext, int inVal) : base(inContext)
	{
		_val = inVal;
	}
}

public class ColorLensHandle : LensHandle
{
	Color _val;
	public Color Value
	{ get { return _val; } }
	public ColorLensHandle(UObject inContext, Color inVal)
		: base(inContext)
	{
		_val = inVal;
	}
}
#endregion

// T : Handle
// TResult : Result type of evaluation
[System.Serializable]
public class LensManager<T, TResult> where T : LensHandle
{
	bool _didWarnAboutTooManyRequests = false;

	public LensManager() { }
	public LensManager(Func<List<T>, TResult> inEvaluateFunc)
	{
		_evaluateAction = inEvaluateFunc;
	}

	protected Func<List<T>, TResult> _evaluateAction = null;

	protected TResult _cachedResult;
	public TResult Result
	{ get { return _cachedResult; } }

	public static implicit operator TResult(LensManager<T, TResult> lm) { return lm.Result; }

	public Func<List<T>, TResult> EvaluateAction
	{
		get { return _evaluateAction; }
		set { _evaluateAction = value; }
	}

	protected List<T> _requestList = new List<T>(2);
	public List<T> RequestList
	{ get { return _requestList; } }

	public int RequestCount
	{ get { return _requestList.Count; } }

	public T[] AddRequests(params T[] inHandles)
	{
		if(inHandles != null)
		{
			foreach(var handle in inHandles)
			{
				if(handle != null && handle.context != null)
				{
					_requestList.Add(handle);
				}
				else
				{
					Debug.LogError("Something passed to AddRequests was null, ignoring.", handle.context);
				}
			}

			EvaluateRequests();
		}
		return inHandles;
	}

	public T AddRequest(T inHandle)
	{
		AddRequests(inHandle);
		return inHandle;
	}

	public void EndRequests(params T[] inHandles)
	{
		EndRequests_Internal(inHandles);
	}

	void EndRequests_Internal(IEnumerable<T> inHandles)
	{
		if(inHandles != null)
		{
			var enumerator = inHandles.GetEnumerator();
			while(enumerator.MoveNext())
			{
				var handle = enumerator.Current;
				if(handle != null)
				{
					if(_requestList.Contains(handle))
						_requestList.Remove(handle);
				}
			}

			EvaluateRequests();
		}
	}

	public void RemoveRequest(T inHandle)
	{
		EndRequests(inHandle);
	}

	List<T> _requestsToRemove = new List<T>();

	public void RemoveRequestsWithContext(UObject inContext)
	{
		if(inContext)
		{
			_requestsToRemove.Clear();
			for(int i = 0; i < _requestList.Count; ++i)
			{
				if(_requestList[i].context == inContext)
					_requestsToRemove.Add(_requestList[i]);
			}

			EndRequests_Internal(_requestsToRemove);
		}
	}

	public void ClearAllRequests()
	{
		_requestList.Clear();
		EvaluateRequests();
	}

	TResult EvaluateRequests()
	{
		// First, check if any of our requests have had their contexts destroyed without telling us.
		for(int i = 0; i < _requestList.Count; ++i)
		{
			var request = _requestList[i];

			// Probably "Unity-destroyed" fake null.
			if(request.context == null)
			{
				Debug.LogError("Request with context " + request.contextName + " has been destroyed without removing request " + request.ToString());
				_requestList.Remove(request);
				--i;
			}
		}

		// TODO: make this tweakable per instance if desired, but generally having a lot of requests indicates a likely bug.
		if(!_didWarnAboutTooManyRequests && _requestList.Count > 20)
		{
			_didWarnAboutTooManyRequests = true;
			Debug.LogError("A LensManager<" + typeof(T) + "," + typeof(TResult) + "> has " + _requestList.Count + " requests, which is likely indicative of a bug");
			foreach(var request in _requestList)
				Debug.LogError(request.ToString());
		}

		TResult result;
		if(_evaluateAction != null)
			result = _evaluateAction(_requestList);
		else
			result = EvaluateMethod();

		_cachedResult = result;
		return result;
	}

	protected virtual TResult EvaluateMethod()
	{
		throw new System.NotImplementedException("LensManager-derived classes must override the EvaluateMethod function or non-derived versions must assign an evaluate Func!");
	}
}