using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

[System.Serializable]
public class SelectionStateChanged : UnityEvent<bool> { }

public class PickableObject : MonoBehaviour
{
	static Color _selectedBoxColor = new Color(0f, 1f, 0f, .5f);

	static Material _lineMaterial = null;
	static Material lineMaterial
	{
		get
		{
			if(!_lineMaterial)
			{
				// Unity has a built-in shader that is useful for drawing
				// simple colored things
				var shader = Shader.Find("Hidden/Internal-Colored");
				_lineMaterial = new Material(shader);
				_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
				// Turn on alpha blending
				_lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				_lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				// Turn backface culling off
				_lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
				// Turn off depth writes
				_lineMaterial.SetInt("_ZWrite", 0);
			}
			return _lineMaterial;
		}
	}

	static int _ignoreRaycast = -1;
	int layerIgnoreRaycast
	{
		get
		{
			if(_ignoreRaycast == -1)
			{
				_ignoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
			}
			return _ignoreRaycast;
		}
	}

	public SelectionStateChanged onSelectionStateChanged = new SelectionStateChanged();
	
	int _startLayer;

	bool _isSelected;
	
	List<Collider> _colliders = new List<Collider>();
	
	List<Renderer> _renderers = new List<Renderer>();

	void Awake()
	{
		_startLayer = gameObject.layer;

		gameObject.GetComponentsInChildren<Collider>(_colliders);
		gameObject.GetComponentsInChildren<Renderer>(_renderers);
	}

	void OnMouseDown()
	{
		Gizmo.GetInstance().SelectObject(this);
	}

	public void SetSelected(bool inState)
	{
		_isSelected = inState;
		gameObject.layer = _isSelected ? layerIgnoreRaycast : _startLayer;
		onSelectionStateChanged.Invoke(_isSelected);
	}

	public void OnRenderObject()
	{
		if(_isSelected)
		{
			// Calculate bounds.
			Bounds? calculatedBounds = null;

			foreach(var collider in _colliders)
			{
				if(calculatedBounds == null)
					calculatedBounds = collider.bounds;
				else
					calculatedBounds.GetValueOrDefault().Encapsulate(collider.bounds);
			}

			foreach(var renderer in _renderers)
			{
				if(calculatedBounds == null)
					calculatedBounds = renderer.bounds;
				else
					calculatedBounds.GetValueOrDefault().Encapsulate(renderer.bounds);
			}

			var bounds = calculatedBounds.GetValueOrDefault();

			// Apply the line material.
			lineMaterial.SetPass(0);

			// Draw lines.
			GL.Begin(GL.LINES);
			GL.Color(_selectedBoxColor);

			// Top
			GL.Vertex(bounds.TLF());
			GL.Vertex(bounds.TRF());
			GL.Vertex(bounds.TRF());
			GL.Vertex(bounds.TRB());
			GL.Vertex(bounds.TRB());
			GL.Vertex(bounds.TLB());
			GL.Vertex(bounds.TLB());
			GL.Vertex(bounds.TLF());

			// Bottom
			GL.Vertex(bounds.BLF());
			GL.Vertex(bounds.BRF());
			GL.Vertex(bounds.BRF());
			GL.Vertex(bounds.BRB());
			GL.Vertex(bounds.BRB());
			GL.Vertex(bounds.BLB());
			GL.Vertex(bounds.BLB());
			GL.Vertex(bounds.BLF());

			// Sides
			GL.Vertex(bounds.BLF());
			GL.Vertex(bounds.TLF());
			GL.Vertex(bounds.BRF());
			GL.Vertex(bounds.TRF());
			GL.Vertex(bounds.BLB());
			GL.Vertex(bounds.TLB());
			GL.Vertex(bounds.BRB());
			GL.Vertex(bounds.TRB());

			GL.End();
		}
	}
}
