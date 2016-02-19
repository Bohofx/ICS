using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum GizmoTools
{ Position, Rotation, Scale }

public enum GizmoControl
{ Horizontal, Vertical, Both }

public enum GizmoAxis
{ Center, X, Y, Z }

public class Gizmo : Singleton<Gizmo>
{
	[SerializeField]
	GizmoHandle _axisCenter;

	[SerializeField]
	GizmoHandle _axisX;

	[SerializeField]
	GizmoHandle _axisY;

	[SerializeField]
	GizmoHandle _axisZ;

	[SerializeField]
	float _defaultDistance = 10f;
	
	[SerializeField]
	float _scaleFactor = 0.1f;

	public enum TranslationCalculationMode
	{
		MouseDelta,
		HandleUtility
	}

	[SerializeField]
	TranslationCalculationMode _translationMode = TranslationCalculationMode.MouseDelta;
	public TranslationCalculationMode TranslationMode
	{ get { return _translationMode; } set { _translationMode = value; } }

	public List<Transform> Selected;
	public int SelectedCount
	{ get { return Selected.Count; } }

	GizmoTools _activeTool = GizmoTools.Position;

	Vector3 _selectionCenter;

	Vector3 _localScale;

	protected override void Awake()
	{
		base.Awake();
		
		SetType(GizmoTools.Position);
		Hide();

		_axisCenter.Axis = GizmoAxis.Center;
		_axisCenter.Gizmo = this;
		_axisX.Axis = GizmoAxis.X;
		_axisX.Gizmo = this;
		_axisY.Axis = GizmoAxis.Y;
		_axisY.Gizmo = this;
		_axisZ.Axis = GizmoAxis.Z;
		_axisZ.Gizmo = this;

		_localScale = transform.localScale;
		
		Selected = new List<Transform>();
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.W))
		{
			SetType(GizmoTools.Position);
		}
		if(Input.GetKeyDown(KeyCode.E))
		{
			SetType(GizmoTools.Rotation);
		}
		if(Input.GetKeyDown(KeyCode.R))
		{
			SetType(GizmoTools.Scale);
		}
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			ClearSelection();
		}
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if(!EventSystem.current.IsPointerOverGameObject())
			{
				int roomBoundsMask = ~(1 << LayerMask.NameToLayer("RoomBounds"));
				if(!Physics.Raycast(ray, Mathf.Infinity, roomBoundsMask))
					ClearSelection();
			}
		}

		if(SelectedCount > 0)
		{
			// Scale based on distance from the camera.
			var distance = Vector3.Distance(transform.position, Camera.main.transform.position);
			var scale = (distance - _defaultDistance) * _scaleFactor;
			transform.localScale = new Vector3(_localScale.x + scale, _localScale.y + scale, _localScale.z + scale);

			// Move the gizmo to the center of our parent.
			UpdateCenterAndVisibility();
			transform.position = _selectionCenter;
		}
	}

	void SetType(GizmoTools type)
	{
		_activeTool = type;
		_axisCenter.SetType(type);
		_axisX.SetType(type);
		_axisY.SetType(type);
		_axisZ.SetType(type);
	}

	public void ClearSelection()
	{
		while(Selected.Count > 0)
		{
			PickableObject pickable = Selected[0].GetComponent<PickableObject>();
			pickable.SetSelected(false);
			Selected.RemoveAt(0);
		}

		UpdateCenterAndVisibility();
	}

	void UpdateCenterAndVisibility()
	{
		if(Selected.Count > 1)
		{
			var vectors = new Vector3[Selected.Count];
			for(int i = 0; i < Selected.Count; i++)
			{
				vectors[i] = Selected[i].position;
			}
			_selectionCenter = CenterOfVectors(vectors);
		}
		else if(Selected.Count == 1)
		{
			_selectionCenter = Selected[0].position;
		}

		if(Selected.Count > 0)
			Show();
		else
			Hide();
	}

	public Bounds GetBoundsForSelected()
	{
		Bounds bounds = new Bounds();
		if(Selected.Count > 0)
		{
			bounds = Selected[0].GetComponent<PickableObject>().PickableBounds;
			for(int i = 1; i < Selected.Count; ++i)
			{
				PickableObject pickableObject = Selected[i].GetComponent<PickableObject>();
				bounds.Encapsulate(pickableObject.PickableBounds);
			}
		}
		return bounds;
	}

	public void SelectObject(PickableObject inPickable)
	{
		bool inputShiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift);
		if(!inputShiftDown)
		{
			ClearSelection();
		}

		if(!Selected.Contains(inPickable.transform))
			Selected.Add(inPickable.transform);
		inPickable.SetSelected(true);
		
		UpdateCenterAndVisibility();
	}

	public void DeselectObject(PickableObject inPickable)
	{
		if(Selected.Contains(inPickable.transform))
			Selected.Remove(inPickable.transform);
		inPickable.SetSelected(false);

		UpdateCenterAndVisibility();
	}

	public void ActivateAxis(GizmoAxis axis)
	{
		switch(axis)
		{
			case GizmoAxis.Center:
				_axisCenter.SetActive(true);
				break;
			case GizmoAxis.X:
				_axisX.SetActive(true);
				break;
			case GizmoAxis.Y:
				_axisY.SetActive(true);
				break;
			case GizmoAxis.Z:
				_axisZ.SetActive(true);
				break;
		}
		SetType(_activeTool);
	}

	public void DeactivateAxis(GizmoAxis axis)
	{
		switch(axis)
		{
			case GizmoAxis.Center:
				_axisCenter.SetActive(false);
				break;
			case GizmoAxis.X:
				_axisX.SetActive(false);
				break;
			case GizmoAxis.Y:
				_axisY.SetActive(false);
				break;
			case GizmoAxis.Z:
				_axisZ.SetActive(false);
				break;
		}
		SetType(_activeTool);
	}

	public void DeactivateHandles()
	{
		_axisCenter.SetActive(false);
		_axisX.SetActive(false);
		_axisY.SetActive(false);
		_axisZ.SetActive(false);
	}

	void Show()
	{
		gameObject.SetActive(true);
		SetType(_activeTool);
	}

	void Hide()
	{
		gameObject.SetActive(false);
	}

	Vector3 CenterOfVectors(Vector3[] vectors)
	{
		Vector3 sum = Vector3.zero;
		if(vectors == null || vectors.Length == 0)
		{
			return sum;
		}

		foreach(Vector3 vec in vectors)
		{
			sum += vec;
		}
		return sum / vectors.Length;
	}
}
