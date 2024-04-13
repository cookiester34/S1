using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelection : InputBehaviour
{
	[SerializeField]
	private float zoomSpeed = 10f;
	
	public Unit selectedObject;
	private Vector3 targetDestination;
	private float zoomLevel = 5;
	private Vector3 rotatedForward;

	private void OnDrawGizmos()
	{
		if (selectedObject != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(selectedObject.transform.position, 1);
				
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(targetDestination, 1);
		}
	}

	protected override void OnAwake()
	{
			
	}

	protected override void SetupInput()
	{
		inputHandler.AddInputAction("LeftClick", HandleLeftClick);
		inputHandler.AddInputAction("RightClick", HandleRightClick);
		inputHandler.AddInputAction<Vector2>("ScrollWheel", HandleScrollWheel);
		inputHandler.AddInputAction<Vector2>("MouseMovement", HandleMouseMovement);
	}
	
	private void Update()
	{
		if (selectedObject != null)
		{
			targetDestination = selectedObject.transform.position + rotatedForward * zoomLevel;
		}
	}
	
	private void HandleMouseMovement(Vector2 value)
	{
		if (selectedObject == null) return;
		
		var rotation = Quaternion.Euler(0, value.x, rotatedForward.x > 0 ? value.y : -value.y);
		rotatedForward = rotation * rotatedForward;
		Debug.Log(rotatedForward);

		targetDestination = selectedObject.transform.position + rotatedForward * zoomLevel;
	}
		
	private void HandleLeftClick()
	{
		if (selectedObject != null)
		{
			selectedObject.Move(targetDestination);
		}
		
		//Select object
		var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
		var hit = Physics.Raycast(ray, out var hitInfo);
		selectedObject = hit ? hitInfo.collider.GetComponent<Unit>() : null;
			
		if (selectedObject != null)
		{
			var selectedObjectPosition = selectedObject.transform.position;
			targetDestination = selectedObjectPosition + selectedObject.transform.forward * zoomLevel;
			rotatedForward = selectedObject.transform.forward;
		}
	}

	private void HandleRightClick()
	{
		
	}
	
	private void HandleScrollWheel(Vector2 value)
	{
		if (selectedObject == null) return;
		
		zoomLevel += value.y * zoomSpeed * Time.deltaTime;
	}
}