using S1.Runtime.Input;
using S1.Runtime.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S1.Runtime.UnitManagement
{
	[RequireComponent(typeof(Camera))]
	public class UnitSelectionController : InputBehaviour
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
		private bool isSelecting;
		private Rect selectionRect;
		private List<Unit> previewSelectedUnits = new();
		
		//Debug Stuff
		private Mesh debugSpherMesh;

		private void OnDrawGizmos()
		{
			if (UnitSelectionManager.HasSelectedUnits)
			{
				foreach (var unit in UnitSelectionManager.SelectedUnits)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawWireMesh(debugSpherMesh, unit.UnitPosition, Quaternion.identity, Vector3.one);

					var unitTargetPosition = unit.UnitPosition + offset;

					Gizmos.color = Color.green;
					Gizmos.DrawWireMesh(debugSpherMesh, unitTargetPosition, Quaternion.identity, Vector3.one);

					Gizmos.DrawLine(unit.UnitPosition, unitTargetPosition);
				}

				Gizmos.color = Color.magenta;
				var averageTargetPosition = averageUnitPosition + offset;
				Gizmos.DrawWireMesh(debugSpherMesh, averageTargetPosition, Quaternion.identity, Vector3.one);
			}
		}

		private void OnGUI()
		{
			if (isSelecting)
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
			if (!UnitSelectionManager.TakeControl(this))
			{
				throw new Exception("Unit Selection Controller failed to take control, one probably already exists");
			}
				
			mainCamera = GetComponent<Camera>();
			selectionSphere = Instantiate(selectionSpherePrefab);
			selectionSphere.SetActive(false);

			var primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			debugSpherMesh = primitive.GetComponent<MeshFilter>().sharedMesh;
		}

		protected override void SetupInput()
		{
			InputHandler.AddInputAction("LeftClick", HandleLeftClick, HandleLeftClickUp);
			InputHandler.AddInputAction("RightClick", HandleRightClick);
			InputHandler.AddInputAction("ToggleSelectionSide", () => selectFarSide = true, () => selectFarSide = false);
			InputHandler.AddInputAction<Vector2>("ScrollWheel", HandleScrollWheel);
			InputHandler.AddInputAction<Vector2>("MousePosition", HandleMousePosition);
		}

		private void Update()
		{
			if (isSelecting)
			{
				selectionRect = GetSelectionRect(mouseSelectionStartPosition, mousePosition);

				var allUnits = UnitManager.Instance.Units;
				previewSelectedUnits.Clear();
				foreach (var unit in allUnits)
				{
					var screenPos = unit.PositionOnScreen(mainCamera);
					if (selectionRect.Contains(screenPos))
					{
						previewSelectedUnits.Add(unit);
					}
				}
			}

			if (!UnitSelectionManager.HasSelectedUnits) return;

			Physics.queriesHitBackfaces = true;
			var ray = mainCamera.ScreenPointToRay(mousePosition);

			var farAwayPoint = ray.origin + ray.direction * 1000f;
			var rayOpposite = new Ray(farAwayPoint, -ray.direction);

			var hit = Physics.Raycast(ray, out var hitInfo, RAYCAST_DISTANCE, selectionSphereLayer);
			var hitOpposite = Physics.Raycast(rayOpposite, out var hitInfoOpposite, RAYCAST_DISTANCE,
				selectionSphereLayer);

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
			isSelecting = true;
			mouseSelectionStartPosition = mousePosition;

			if (UnitSelectionManager.HasSelectedUnits)
			{
				foreach (var unit in UnitSelectionManager.SelectedUnits)
				{
					unit.Move(unit.UnitPosition + offset);
				}
			}

			UnitSelectionManager.ClearSelectedUnits();
			previewSelectedUnits.Clear();
			selectionSphere.SetActive(false);
		}

		private void HandleLeftClickUp()
		{
			previewSelectedUnits.Clear();
			isSelecting = false;
			zoomLevel = 10;
			AreaSelect(mouseSelectionStartPosition, mousePosition);

			if (UnitSelectionManager.HasSelectedUnits)
			{
				averageUnitPosition = UnitSelectionManager.SelectedUnits.Select(unit => unit.UnitPosition).Average();
			}
			else
			{
				var ray = mainCamera.ScreenPointToRay(mousePosition);
				var hit = Physics.Raycast(ray, out var hitInfo, RAYCAST_DISTANCE, unitLayer);
				var unit = hit ? hitInfo.collider.GetComponent<Unit>() : null;

				if (unit == null) return;

				UnitSelectionManager.AddSelectedUnit(unit);
				averageUnitPosition = unit.UnitPosition;
			}

			if (UnitSelectionManager.HasSelectedUnits)
			{
				selectionSphere.SetActive(true);
				selectionSphere.transform.position = averageUnitPosition;
				var sphereScale = Vector3.one * zoomLevel * 2f;
				selectionSphere.transform.localScale = sphereScale;
			}
		}

		private void HandleRightClick()
		{
			isSelecting = false;
			UnitSelectionManager.ClearSelectedUnits();
			selectionSphere.SetActive(false);
		}

		private void HandleMousePosition(Vector2 value)
		{
			mousePosition = value;
		}

		private void AreaSelect(Vector3 startPosition, Vector3 endPosition)
		{
			selectionRect = GetSelectionRect(startPosition, endPosition);
			UnitSelectionManager.ClearSelectedUnits();

			var allUnits = UnitManager.Instance.Units;
			foreach (var unit in allUnits)
			{
				var screenPos = unit.PositionOnScreen(mainCamera);
				if (selectionRect.Contains(screenPos))
				{
					UnitSelectionManager.AddSelectedUnit(unit);
				}
			}
		}

		private Rect GetSelectionRect(Vector3 startPosition, Vector3 endPosition)
		{
			var topLeft = Vector3.Min(startPosition, endPosition);
			var bottomRight = Vector3.Max(startPosition, endPosition);
			return new Rect(topLeft.x, Screen.height - bottomRight.y, bottomRight.x - topLeft.x,
				bottomRight.y - topLeft.y);
		}

		private void HandleScrollWheel(Vector2 value)
		{
			if (!UnitSelectionManager.HasSelectedUnits) return;

			zoomLevel += value.y * zoomSpeed * Time.deltaTime;
			var sphereScale = Vector3.one * zoomLevel * 2f;
			selectionSphere.transform.localScale = sphereScale;
		}
	}
}