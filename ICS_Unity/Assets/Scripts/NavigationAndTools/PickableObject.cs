using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
				_ignoreRaycast = 1 << LayerMask.NameToLayer("Ignore Raycast");
			}
			return _ignoreRaycast;
		}
	}
	
	int _startLayer;

	bool _isSelected;

	Bounds? _bounds = null;
	Bounds bounds
	{ get { return _bounds.GetValueOrDefault(); } }

	void Awake()
	{
		_startLayer = gameObject.layer;

		List<Collider> colliders = new List<Collider>();
		List<Renderer> renderers = new List<Renderer>();

		gameObject.GetComponentsInChildren<Collider>(colliders);
		gameObject.GetComponentsInChildren<Renderer>(renderers);

		foreach(var collider in colliders)
		{
			if(_bounds == null)
				_bounds = collider.bounds;
			else
				_bounds.GetValueOrDefault().Encapsulate(collider.bounds);
		}

		foreach(var renderer in renderers)
		{
			if(_bounds == null)
				_bounds = renderer.bounds;
			else
				_bounds.GetValueOrDefault().Encapsulate(renderer.bounds);
		}
	}

	void OnMouseDown()
	{
		ToolsManager.GetInstance().AddSelected(this);
	}

	public void SetSelected(bool inState)
	{
		_isSelected = inState;
		gameObject.layer = _isSelected ? layerIgnoreRaycast : _startLayer;
	}

	public void OnRenderObject()
	{
		if(_isSelected)
		{
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
