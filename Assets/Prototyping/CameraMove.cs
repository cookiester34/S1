using UnityEngine;

// [RequireComponent(typeof(UnitSelection))]
public class CameraMove : InputBehaviour
{
	[SerializeField]
	private float moveSpeed = 10f;

	[SerializeField]
	private float rotateSpeed = 10f;

	[SerializeField]
	private float zoomSpeed = 10f;

	[SerializeField]
	private float verticalSpeed = 10f;
	
	[SerializeField]
	private Transform anchor;

	private InputHandlerOfType<Vector2> moveHandler;
	private InputHandlerOfType<float> rotateHandler;
	private InputHandlerOfType<float> verticalHandler;
	
	private bool canRotateXAxis = false;
	
	private UnitSelection2 unitSelection;

	protected override void OnAwake()
	{
		unitSelection = GetComponent<UnitSelection2>();
	}

	protected override void SetupInput()
	{
		inputHandlerPrototype.AddInputAction("MiddleClick", () => canRotateXAxis = true, () => canRotateXAxis = false);
		inputHandlerPrototype.AddInputAction<Vector2>("ScrollWheel", HandleScrollWheel);
		inputHandlerPrototype.AddInputAction<Vector2>("MouseMovement", HandleMouseMovement);

		
		inputHandlerPrototype.AddInputAction<Vector2>("Move", out moveHandler);
		inputHandlerPrototype.AddInputAction<float>("Rotate", out rotateHandler);
		inputHandlerPrototype.AddInputAction<float>("Vertical", out verticalHandler);
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
		
		if (verticalHandler != null)
		{
			HandleVertical(verticalHandler.Value);
		}
	}

	private void HandleCameraMove(Vector2 value)
	{
		var moveDirection = new Vector3(value.x, 0, value.y);
		var move = moveDirection * moveSpeed * Time.deltaTime;
		anchor.Translate(move, Space.Self);
	}
	
	private void HandleRotate(float value)
	{
		anchor.RotateAround(transform.position, new Vector3(0, value, 0), rotateSpeed * Time.deltaTime);
	}
	
	private void HandleVertical(float value)
	{
		var verticalMovement = new Vector3(0, value, 0) * verticalSpeed * Time.deltaTime;
		anchor.Translate(verticalMovement, Space.Self);
	}
	
	private void HandleMouseMovement(Vector2 value)
	{
		if (canRotateXAxis)
		{
			transform.Rotate(new Vector3(-value.y, 0, 0), rotateSpeed * Time.deltaTime);
			anchor.RotateAround(transform.position, new Vector3(0, value.x, 0), rotateSpeed * Time.deltaTime);
		}
	}

	private void HandleScrollWheel(Vector2 value)
	{
		if (unitSelection.HasSelectedUnits) return;
		
		var zoom = new Vector3(0, 0, value.y) * zoomSpeed * Time.deltaTime;
		transform.Translate(zoom, Space.Self);
	}
}