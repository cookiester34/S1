using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class UnitSelection2 : InputBehaviour
{
	private const float RAYCAST_DISTANCE = 10000f;
	
	[Header("Controls")]
	[SerializeField]
	private float zoomSpeed = 10f;
	
	[Header("Unit Selection")]
	[SerializeField]
	private LayerMask unitLayer;
	
	[Header("Selection Sphere")]
	[SerializeField]
	private GameObject selectionSpherePrefab;

	[SerializeField]
	private LayerMask selectionSphereLayer;
	
	private Camera mainCamera;
	private GameObject selectionSphere;
	private Vector3 mousePosition;
	private Vector3 mouseSelectionStartPosition;
	private Vector3 averageUnitPosition;
	private Vector3 offset;
	private float zoomLevel = 10;
	private bool selectFarSide = true;
	
	private Rect selectionRect;
	
	private List<Unit> selectedUnits = new();
	private List<Unit> previewSelectedUnits = new();
	
	public bool HasSelectedUnits => selectedUnits.Count > 0;
	public bool IsSelecting { get; private set; }
	
	//Debug Stuff
	private Mesh debugSpherMesh;
	
	private void OnDrawGizmos()
	{
		if (HasSelectedUnits)
		{
			foreach (var unit in selectedUnits)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireMesh(debugSpherMesh, unit.UnitPosition, Quaternion.identity, Vector3.one);
				
				var unitTargetPosition = unit.UnitPosition + offset;
				
				Gizmos.color = Color.green;
				Gizmos.DrawWireMesh(debugSpherMesh, unitTargetPosition, Quaternion.identity, Vector3.one);
				
				Gizmos.DrawLine(unit.UnitPosition, unitTargetPosition);
			}
			
			// Gizmos.color = Color.blue;
			// var scale = Vector3.one * zoomLevel * 2f;
			// Gizmos.DrawWireMesh(debugSpherMesh, averageUnitPosition, Quaternion.identity, scale);
			
			Gizmos.color = Color.magenta;
			var averageTargetPosition = averageUnitPosition + offset;
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
		selectionSphere = Instantiate(selectionSpherePrefab);
		selectionSphere.SetActive(false);
		
		var primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		debugSpherMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
	}

	protected override void SetupInput()
	{
		inputHandlerPrototype.AddInputAction("LeftClick", HandleLeftClick, HandleLeftClickUp);
		inputHandlerPrototype.AddInputAction("RightClick", HandleRightClick);
		inputHandlerPrototype.AddInputAction("ToggleSelectionSide", () => selectFarSide = true, () => selectFarSide = false);
		inputHandlerPrototype.AddInputAction<Vector2>("ScrollWheel", HandleScrollWheel);
		inputHandlerPrototype.AddInputAction<Vector2>("MousePosition", HandleMousePosition);
	}

	private void Update()
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
		
		Physics.queriesHitBackfaces = true;
		var ray = mainCamera.ScreenPointToRay(mousePosition);
		
		var farAwayPoint = ray.origin + ray.direction * 1000f;
		var rayOpposite = new Ray(farAwayPoint, -ray.direction);
		
		var hit = Physics.Raycast(ray, out var hitInfo, RAYCAST_DISTANCE, selectionSphereLayer);
		var hitOpposite = Physics.Raycast(rayOpposite, out var hitInfoOpposite, RAYCAST_DISTANCE, selectionSphereLayer);
		
		Physics.queriesHitBackfaces = false;

		if (selectFarSide && hitOpposite)
		{
			offset = hitInfoOpposite.point - averageUnitPosition;
		}
		else if (hit)
		{
			offset = hitInfo.point - averageUnitPosition;
		}
	}
		
	private void HandleLeftClick()
	{
		IsSelecting = true;
		mouseSelectionStartPosition = mousePosition;

		if (HasSelectedUnits)
		{
			foreach (var unit in selectedUnits)
			{
				unit.Move(unit.UnitPosition + offset);
			}
		}
		
		selectedUnits.Clear();
		previewSelectedUnits.Clear();
		selectionSphere.SetActive(false);
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
		}
		else
		{
			var ray = mainCamera.ScreenPointToRay(mousePosition);
			var hit = Physics.Raycast(ray, out var hitInfo, RAYCAST_DISTANCE, unitLayer);
			var unit = hit ? hitInfo.collider.GetComponent<Unit>() : null;

			if (unit == null) return;
			
			selectedUnits.Add(unit);
			averageUnitPosition = unit.UnitPosition;
		}

		if (HasSelectedUnits)
		{
			selectionSphere.SetActive(true);
			selectionSphere.transform.position = averageUnitPosition;
			var sphereScale = Vector3.one * zoomLevel * 2f;
			selectionSphere.transform.localScale = sphereScale;
		}
	}
	
	private void HandleRightClick()
	{
		IsSelecting = false;
		selectedUnits.Clear();
		selectionSphere.SetActive(false);
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
		var sphereScale = Vector3.one * zoomLevel * 2f;
		selectionSphere.transform.localScale = sphereScale;
	}
}