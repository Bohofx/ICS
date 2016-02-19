using UnityEngine;
using System.Collections;

public class GizmoHandle : MonoBehaviour 
{
    public Gizmo Gizmo;
    public GizmoControl Control;
    public GizmoTools Type;
    public GameObject PositionCap;
    public GameObject RotationCap;
    public GameObject ScaleCap;
    public Material ActiveMaterial;
    public GizmoAxis Axis;

    public float MoveSensitivity = 10f;
    public float RotationSensitivity = 64f;
    public float ScaleSensitivity = 10f;

    private Material inactiveMaterial;
    private bool activeHandle;

	MoveObjectCommand _moveObjectCommand;

	Vector3 _worldDelta;
	
	Vector3 _startDragMousePosition;
	Vector3 _startDragSelectedCenter;

    void Awake()
    {
        inactiveMaterial = GetComponent<Renderer>().material;
    }

	void OnDisable()
	{
		FlushCommands();
	}

	void FlushCommands()
	{
		if(_moveObjectCommand != null)
		{
			_moveObjectCommand.WorldDelta = _worldDelta;
			UndoManager.GetInstance().Record(_moveObjectCommand);
			_worldDelta = Vector3.zero;
			_moveObjectCommand = null;
		}
	}

    void OnMouseDown()
    {
		switch(Type)
		{
			case GizmoTools.Position:
				_moveObjectCommand = new MoveObjectCommand(Gizmo.Selected);
				_startDragMousePosition = Input.mousePosition;
				_startDragSelectedCenter = Gizmo.GetBoundsForSelected().center;
				_worldDelta = Vector3.zero;
				break;
			default:
				break;
		}

        Gizmo.DeactivateHandles();
        SetActive(true);
    }

	void OnMouseUp()
	{
		FlushCommands();
	}

	public void OnMouseDrag()
	{
		Vector3 frameDelta = Vector3.zero;

		var delta = 0f;
		var vert = 0f;
		var horz = 0f;
		if(activeHandle)
		{
			horz = Input.GetAxis("Mouse X") * Time.deltaTime;
			vert = Input.GetAxis("Mouse Y") * Time.deltaTime;

			// TODO: GizmoControl should be based on the camera not a selection -- X, Z are set to "both" for now.
			switch(Control)
			{
				case GizmoControl.Horizontal:
					delta = Input.GetAxis("Mouse X") * Time.deltaTime;
					break;
				case GizmoControl.Vertical:
					delta = Input.GetAxis("Mouse Y") * Time.deltaTime;
					break;
				case GizmoControl.Both:
					delta = (Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y")) * Time.deltaTime;
					break;
			}

			switch(Type)
			{
				case GizmoTools.Position:
					delta *= MoveSensitivity;
					horz *= MoveSensitivity;
					vert *= MoveSensitivity;

					Vector3 constraintDir = Vector3.zero;
					switch(Axis)
					{
						case GizmoAxis.X:
							constraintDir = Vector3.right;
							break;
						case GizmoAxis.Y:
							constraintDir = Vector3.up;
							break;
						case GizmoAxis.Z:
							constraintDir = Vector3.forward;
							break;
						case GizmoAxis.Center:
							// Based on the camera position we need to either move X horizontal or vertical / vice versa with Z
							foreach(var obj in Gizmo.Selected)
							{
								Vector3 xDelta = Vector3.right * horz;
								Vector3 zDelta = Vector3.forward * vert;
								frameDelta = xDelta + zDelta;
								obj.Translate(xDelta, Space.World);
								obj.Translate(zDelta, Space.World);
							}
							break;
					}

					if(constraintDir.sqrMagnitude > 0f)
					{
						if(Gizmo.TranslationMode == Gizmo.TranslationCalculationMode.MouseDelta)
						{
							frameDelta = constraintDir * delta;
							foreach(var obj in Gizmo.Selected)
							{
								obj.Translate(frameDelta, Space.World);
							}
							_worldDelta += frameDelta;
						}
						else
						{
							float amountMoved = UtilitiesHandles.CalcLineTranslation(_startDragMousePosition, Input.mousePosition, _startDragSelectedCenter, constraintDir, Matrix4x4.identity);
							Vector3 newPosition = _startDragSelectedCenter + amountMoved * constraintDir;
							frameDelta = newPosition - _startDragSelectedCenter;

							// Remove delta offset from previous frame, then add this frame's.
							foreach(var obj in Gizmo.Selected)
							{
								obj.Translate(-_worldDelta + frameDelta, Space.World);
							}
							_worldDelta = frameDelta;
						}
					}
					break;

				case GizmoTools.Scale:
					delta *= ScaleSensitivity;
					switch(Axis)
					{
						case GizmoAxis.X:
							foreach(var obj in Gizmo.Selected)
								obj.localScale = new Vector3(obj.localScale.x + delta, obj.localScale.y, obj.localScale.z);
							break;
						case GizmoAxis.Y:
							foreach(var obj in Gizmo.Selected)
								obj.localScale = new Vector3(obj.localScale.x, obj.localScale.y + delta, obj.localScale.z);
							break;
						case GizmoAxis.Z:
							foreach(var obj in Gizmo.Selected)
								obj.localScale = new Vector3(obj.localScale.x, obj.localScale.y, obj.localScale.z + delta);
							break;
						case GizmoAxis.Center:
							foreach(var obj in Gizmo.Selected)
								obj.localScale = new Vector3(obj.localScale.x + delta, obj.localScale.y + delta, obj.localScale.z + delta);
							break;
					}
					break;

				case GizmoTools.Rotation:
					delta *= RotationSensitivity;
					switch(Axis)
					{
						case GizmoAxis.X:
							foreach(var obj in Gizmo.Selected)
								obj.Rotate(Vector3.right * delta);
							break;
						case GizmoAxis.Y:
							foreach(var obj in Gizmo.Selected)
								obj.Rotate(Vector3.up * delta);
							break;
						case GizmoAxis.Z:
							foreach(var obj in Gizmo.Selected)
								obj.Rotate(Vector3.forward * delta);
							break;
						case GizmoAxis.Center:
							foreach(var obj in Gizmo.Selected)
							{
								obj.Rotate(Vector3.right * delta);
								obj.Rotate(Vector3.up * delta);
								obj.Rotate(Vector3.forward * delta);
							}
							break;
					}
					break;
			}
		}
	}

    public void SetActive(bool active)
    {
        if (active)
        {
            activeHandle = true;
            GetComponent<Renderer>().material = ActiveMaterial;
            if (Axis != GizmoAxis.Center)
            {
                PositionCap.GetComponent<Renderer>().material = ActiveMaterial;
                RotationCap.GetComponent<Renderer>().material = ActiveMaterial;
                ScaleCap.GetComponent<Renderer>().material = ActiveMaterial;
            }
        }
        else
        {
            activeHandle = false;
            GetComponent<Renderer>().material = inactiveMaterial;
            if (Axis != GizmoAxis.Center)
            {
                PositionCap.GetComponent<Renderer>().material = inactiveMaterial;
                RotationCap.GetComponent<Renderer>().material = inactiveMaterial;
                ScaleCap.GetComponent<Renderer>().material = inactiveMaterial;
            }
        }
    }

    public void SetType(GizmoTools type)
    {
        Type = type;
        if (Axis != GizmoAxis.Center)
        {
            PositionCap.SetActive(type == GizmoTools.Position);
            RotationCap.SetActive(type == GizmoTools.Rotation);
            ScaleCap.SetActive(type == GizmoTools.Scale);
        }
    }

}
