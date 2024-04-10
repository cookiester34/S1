using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
	[SerializeField]
	private float moveSpeed = 10f;

	[SerializeField]
	private float rotateSpeed = 10f;

	[SerializeField]
	private float zoomSpeed = 10f;

	[SerializeField]
	private Transform anchor;

	private InputHandler inputHandler;
	private InputHandlerOfType<Vector2> moveHandler;
	private InputHandlerOfType<float> rotateHandler;
	
	private bool canRotateXAxis = false;

	private void Awake()
	{
		var playerInput = GetComponent<PlayerInput>();
		if (playerInput != null)
		{
			inputHandler = new InputHandler(playerInput);
			SetupInput();
		}
		else
		{
			throw new Exception("PlayerInput component not found. No input handler created.");
		}
	}

	private void SetupInput()
	{
		inputHandler.AddInputAction("LeftClick", HandleLeftClick);
		inputHandler.AddInputAction("RightClick", HandleRightClick);
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
		var move = new Vector3(value.x, value.y, 0) * moveSpeed * Time.deltaTime;
		anchor.Translate(move, Space.Self);
	}

	private void HandleLeftClick()
	{
		
	}

	private void HandleRightClick()
	{
		
	}
	
	private void HandleMouseMovement(Vector2 value)
	{
		if (canRotateXAxis)
		{
			transform.RotateAround(anchor.position, new Vector3(-value.y, 0, 0), rotateSpeed * Time.deltaTime);
		}
	}

	private void HandleRotate(float value)
	{
		transform.RotateAround(anchor.position, new Vector3(0, value, 0), rotateSpeed * Time.deltaTime);
	}

	private void HandleScrollWheel(Vector2 value)
	{
		var zoom = new Vector3(0, 0, value.y) * zoomSpeed * Time.deltaTime;
		anchor.Translate(zoom, Space.Self);
	}
}