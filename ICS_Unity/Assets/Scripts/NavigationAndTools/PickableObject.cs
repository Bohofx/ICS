using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PickableObject : MonoBehaviour
{
	static Color _boxColor = new Color(0f, 1f, 0f, .5f);

	Bounds? _bounds = null;
	Bounds bounds
	{ get { return _bounds.GetValueOrDefault(); } }

	void Awake()
	{
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

	bool _isSelected;

	void OnMouseDown()
	{
		ToolsManager.GetInstance().SetSelected(this);
	}

	public void SetSelected(bool inState)
	{
		_isSelected = inState;
	}

	static Material lineMaterial;
	static void CreateLineMaterial()
	{
		if(!lineMaterial)
		{
			// Unity has a built-in shader that is useful for drawing
			// simple colored things
			var shader = Shader.Find("Hidden/Internal-Colored");
			lineMaterial = new Material(shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			// Turn on alpha blending
			lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			// Turn off depth writes
			lineMaterial.SetInt("_ZWrite", 0);
		}
	}

	public void OnRenderObject()
	{
		if(_isSelected)
		{
			CreateLineMaterial();

			// Apply the line material.
			lineMaterial.SetPass(0);

			// Draw lines.
			GL.Begin(GL.LINES);
			GL.Color(_boxColor);

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
