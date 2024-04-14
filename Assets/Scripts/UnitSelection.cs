using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class UnitSelection : InputBehaviour
{
	[SerializeField]
	private float zoomSpeed = 10f;
	
	private Camera mainCamera;
	private Vector3 mousePosition;
	private Vector3 rotatedForward;
	private Vector3 mouseSelectionStartPosition;
	private Vector3 averageUnitPosition;
	private float zoomLevel = 10;
	
	private Mesh debugSpherMesh;
	private Rect selectionRect;
	
	private List<Unit> selectedUnits = new();
	private List<Unit> previewSelectedUnits = new();
	
	public bool HasSelectedUnits => selectedUnits.Count > 0;
	public bool IsSelecting { get; private set; }
	
	private void OnDrawGizmos()
	{
		if (HasSelectedUnits)
		{
			foreach (var unit in selectedUnits)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireMesh(debugSpherMesh, unit.UnitPosition, Quaternion.identity, Vector3.one);
				
				var unitTargetPosition = unit.UnitPosition + rotatedForward * zoomLevel;
				
				Gizmos.color = Color.green;
				Gizmos.DrawWireMesh(debugSpherMesh, unitTargetPosition, Quaternion.identity, Vector3.one);
			}
			
			Gizmos.color = Color.blue;
			var scale = Vector3.one * zoomLevel * 2f;
			Gizmos.DrawWireMesh(debugSpherMesh, averageUnitPosition, Quaternion.identity, scale);
			
			Gizmos.color = Color.magenta;
			var averageTargetPosition = averageUnitPosition + rotatedForward * zoomLevel;
			Gizmos.DrawWireMesh(debugSpherMesh, averageTargetPosition, Quaternion.identity, Vector3.one);
		}
	}

	private void OnGUI()
	{
		if (IsSelecting)
		{
			var unitBoxSize = new Vector2(10f, 10f);
			foreach (var unit in previewSelectedUnits)
			{
				Vector3 screenPos = unit.LastScreenPosition;

				var unitBoxRect = new Rect(screenPos.x, screenPos.y, unitBoxSize.x, unitBoxSize.y);

				GUI.Box(unitBoxRect, GUIContent.none);
			}
			
			GUI.Box(selectionRect, GUIContent.none);
		}
	}

	protected override void OnAwake()
	{
		mainCamera = GetComponent<Camera>();
		var primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		debugSpherMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
	}

	protected override void SetupInput()
	{
		inputHandler.AddInputAction("LeftClick", HandleLeftClick, HandleLeftClickUp);
		inputHandler.AddInputAction("RightClick", HandleRightClick);
		inputHandler.AddInputAction<Vector2>("ScrollWheel", HandleScrollWheel);
		inputHandler.AddInputAction<Vector2>("MouseMovement", HandleMouseMovement);
		inputHandler.AddInputAction<Vector2>("MousePosition", HandleMousePosition);
	}
	
	private void Update()
	{
	}
	
	private void HandleMouseMovement(Vector2 value)
	{
		if (IsSelecting)
		{
			selectionRect = GetSelectionRect(mouseSelectionStartPosition, mousePosition);
		
			var allUnits = UnitManager.Instance.Units;
			previewSelectedUnits.Clear();
			foreach (var unit in allUnits)
			{
				var screenPos = unit.CameraPosition(mainCamera);
				if (selectionRect.Contains(screenPos))
				{
					previewSelectedUnits.Add(unit);
				}
			}
		}
		
		if (!HasSelectedUnits) return;
		
		var rotation = Quaternion.Euler(-value.y, value.x, 0);
		rotatedForward = rotation * rotatedForward;
	}
		
	private void HandleLeftClick()
	{
		IsSelecting = true;
		mouseSelectionStartPosition = mousePosition;

		if (HasSelectedUnits)
		{
			foreach (var unit in selectedUnits)
			{
				unit.Move(unit.UnitPosition + rotatedForward * zoomLevel);
			}
		}
		
		selectedUnits.Clear();
		previewSelectedUnits.Clear();
	}

	private void HandleLeftClickUp()
	{
		previewSelectedUnits.Clear();
		IsSelecting = false;
		zoomLevel = 10;
		AreaSelect(mouseSelectionStartPosition, mousePosition);

		if (HasSelectedUnits)
		{
			averageUnitPosition = selectedUnits.Select(unit => unit.UnitPosition).Average();
			rotatedForward = (selectedUnits.Select(unit => unit.Forward).Average() + rotatedForward * zoomLevel).normalized;
		}
		else
		{
			var ray = mainCamera.ScreenPointToRay(mousePosition);
			var hit = Physics.Raycast(ray, out var hitInfo);
			var unit = hit ? hitInfo.collider.GetComponent<Unit>() : null;

			if (unit == null) return;
			
			selectedUnits.Add(unit);
			averageUnitPosition = unit.UnitPosition;
			rotatedForward = unit.Forward;
		}
	}
	
	private void HandleRightClick()
	{
		IsSelecting = false;
		selectedUnits.Clear();
	}
	
	private void HandleMousePosition(Vector2 value)
	{
		mousePosition = value;
	}

	private void AreaSelect(Vector3 startPosition, Vector3 endPosition)
	{
		selectionRect = GetSelectionRect(startPosition, endPosition);
		selectedUnits.Clear();
		
		var allUnits = UnitManager.Instance.Units;
		foreach (var unit in allUnits)
		{
			var screenPos = unit.CameraPosition(mainCamera);
			if (selectionRect.Contains(screenPos))
			{
				selectedUnits.Add(unit);
			}
		}
	}
	
	private Rect GetSelectionRect(Vector3 startPosition, Vector3 endPosition)
	{
		var topLeft = Vector3.Min(startPosition, endPosition);
		var bottomRight = Vector3.Max(startPosition, endPosition);
		return new Rect(topLeft.x, Screen.height - bottomRight.y, bottomRight.x - topLeft.x, bottomRight.y - topLeft.y);
	}
	
	private void HandleScrollWheel(Vector2 value)
	{
		if (!HasSelectedUnits) return;
		
		zoomLevel += value.y * zoomSpeed * Time.deltaTime;
	}
}