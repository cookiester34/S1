using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UnitSelection))]
public class CameraMove : InputBehaviour
{
	[SerializeField]
	private float moveSpeed = 10f;

	[SerializeField]
	private float rotateSpeed = 10f;

	[SerializeField]
	private float zoomSpeed = 10f;

	[SerializeField]
	private Transform anchor;

	private InputHandlerOfType<Vector2> moveHandler;
	private InputHandlerOfType<float> rotateHandler;
	
	private bool canRotateXAxis = false;
	
	private UnitSelection unitSelection;

	protected override void OnAwake()
	{
		unitSelection = GetComponent<UnitSelection>();
	}

	protected override void SetupInput()
	{
		inputHandler.AddInputAction("MiddleClick", () => canRotateXAxis = true, () => canRotateXAxis = false);
		inputHandler.AddInputAction<Vector2>("ScrollWheel", HandleScrollWheel);
		inputHandler.AddInputAction<Vector2>("MouseMovement", HandleMouseMovement);

		
		inputHandler.AddInputAction<Vector2>("Move", out moveHandler);
		inputHandler.AddInputAction<float>("Rotate", out rotateHandler);
	}

	private void Update()
	{
		if (moveHandler != null)
		{
			HandleCameraMove(moveHandler.Value);
		}

		if (rotateHandler != null)
		{
			HandleRotate(rotateHandler.Value);
		}
	}

	private void HandleCameraMove(Vector2 value)
	{
		var forward = transform.forward;
		var forwardXZ = new Vector3(forward.x, 0, forward.z).normalized;
		var moveDirection = new Vector3(value.x, 0, value.y);
		var move = moveDirection * moveSpeed * Time.deltaTime;
		anchor.Translate(move, Space.Self);
	}
	
	private void HandleMouseMovement(Vector2 value)
	{
		if (canRotateXAxis)
		{
			transform.Rotate(new Vector3(-value.y, 0, 0), rotateSpeed * Time.deltaTime);
			anchor.RotateAround(transform.position, new Vector3(0, value.x, 0), rotateSpeed * Time.deltaTime);
		}
	}

	private void HandleRotate(float value)
	{
		anchor.RotateAround(transform.position, new Vector3(0, value, 0), rotateSpeed * Time.deltaTime);
	}

	private void HandleScrollWheel(Vector2 value)
	{
		if (unitSelection.selectedObject != null) return;
		
		var zoom = new Vector3(0, 0, value.y) * zoomSpeed * Time.deltaTime;
		transform.Translate(zoom, Space.Self);
	}
}