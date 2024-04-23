using S1.Runtime.Input;
using S1.Runtime.Teams;
using S1.Runtime.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S1.Runtime.UnitManagement.Selection
{
	[RequireComponent(typeof(Camera))]
	public partial class UnitSelectionController : InputBehaviour
	{
		private enum ControlState
		{
			Selecting,
			Selected,
			Move
		}

		private enum SelectionType
		{
			Player,
			Friendly,
			Enemies
		}
		
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
		private Vector3 averageUnitPosition;
		private Vector3 offset;
		private Quaternion rotationOffset; // Not currently used
		private float zoomLevel = 10;
		private bool selectFarSide = true;
		private bool isSelecting;
		private Rect selectionRect;
		private ControlState controlState = ControlState.Selecting;
		
		private Vector3 mouseSelectionStartPosition;
		private SelectionType selectionType = SelectionType.Friendly;
		private List<Unit> previewSelectedUnits = new();

		private Vector3 averageTargetPosition;
		private List<Unit> targetUnits = new();
		
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
		
		private void UpdateControlState(ControlState newState)
		{
			controlState = newState;
		}

		private void DisplayUnitsPreviewPositions()
		{
			var hasTargetUnits = targetUnits.Count > 0;

			foreach (var unit in UnitSelectionManager.SelectedUnits)
			{
				var previewPosition = unit.UnitPosition + offset;
				var directionToTarget = (averageUnitPosition - previewPosition).normalized;
				var unitRotation = Quaternion.LookRotation(directionToTarget);
				if (!hasTargetUnits)
				{
					unitRotation = unit.transform.rotation;
				}
				//Apply rotation offset here when we get around to that
				unit.ShowPreview(previewPosition, unitRotation);
			}
		}
		
		private void HideUnitsPreviewPositions()
		{
			foreach (var unit in UnitSelectionManager.SelectedUnits)
			{
				unit.HidePreview();
			} 
			selectionSphere.SetActive(false);
		}

		private void Update()
		{
			switch (controlState)
			{
				case ControlState.Selecting:
					HandleSelecting();
					break;
				case ControlState.Selected:
					HandleSelecting();
					SelectedStateUpdate();
					HandleMovingToTarget();
					break;
				case ControlState.Move:
					HandleMovingToTarget();
					break;
			}
		}

		private void HandleLeftClick()
		{
			switch (controlState)
			{
				case ControlState.Selecting:
					HandleLeftClickSelecting();
					break;
				case ControlState.Selected:
					HandleLeftClickSelected();
					break;
				case ControlState.Move:
					break;
			}
		}

		private void HandleLeftClickUp()
		{
			switch (controlState)
			{
				case ControlState.Selecting:
					HandleLeftClickUpSelection();
					break;
				case ControlState.Selected:
					if (!DidSelectionHappen())
					{
						HandleMovement();
					}
					break;
				case ControlState.Move:
					HandleMovement();
					break;
			}
		}

		private void HandleRightClick()
		{
			switch (controlState)
			{
				case ControlState.Selecting:
					break;
				case ControlState.Selected:
					ClearSelection();
					break;
				case ControlState.Move:
					ClearSelection();
					break;
			}
		}
		
		private void BeginSelection()
		{
			isSelecting = true;
			mouseSelectionStartPosition = mousePosition;
		}

		private List<Unit> GetSelectedUnits(out SelectionType typeSelected, bool changeSelectionType = true)
		{
			selectionRect = GetSelectionRect(mouseSelectionStartPosition, mousePosition);
			typeSelected = SelectionType.Friendly;
			
			//This will need optimising to visible units only
			var selectedPlayerUnits = (from unit in UnitManager.Instance.PlayerUnits
				let screenPos = unit.PositionOnScreen(mainCamera)
				where selectionRect.Contains(screenPos)
				select unit).ToList();
			
			var selectedFriendlyUnits = (from unit in UnitManager.Instance.FriendlyUnits
				let screenPos = unit.PositionOnScreen(mainCamera)
				where selectionRect.Contains(screenPos)
				select unit).ToList();
			
			var selectedEnemyUnits = (from unit in UnitManager.Instance.EnemyUnits
				let screenPos = unit.PositionOnScreen(mainCamera)
				where selectionRect.Contains(screenPos)
				select unit).ToList();

			var friendlyUnitCount = selectedFriendlyUnits.Count;
			var playerUnitCount = selectedPlayerUnits.Count;
			if (friendlyUnitCount == 0 && selectedEnemyUnits.Count == 0 && playerUnitCount == 0)
			{
				var ray = mainCamera.ScreenPointToRay(mousePosition);
				var hit = Physics.Raycast(ray, out var hitInfo, RAYCAST_DISTANCE, unitLayer);
				var unit = hit ? hitInfo.collider.GetComponent<Unit>() : null;

				if (unit == null) return new List<Unit>();
				var unitTeam = unit.Team;
				typeSelected = unitTeam.IsPlayerTeam()
					? SelectionType.Player
					: unitTeam.IsTeamFriendly()
						? SelectionType.Friendly
						: SelectionType.Enemies;

				return new List<Unit> {unit};
			}

			typeSelected = playerUnitCount > 0
				? SelectionType.Player
				: friendlyUnitCount > 0
					? SelectionType.Friendly
					: SelectionType.Enemies;
			return typeSelected switch
			{
				SelectionType.Player => selectedPlayerUnits,
				SelectionType.Friendly => selectedFriendlyUnits,
				_ => selectedEnemyUnits
			};
		}

		private void HandleMousePosition(Vector2 value)
		{
			mousePosition = value;
		}

		private Rect GetSelectionRect(Vector3 startPosition, Vector3 endPosition)
		{
			var topLeft = Vector3.Min(startPosition, endPosition);
			var bottomRight = Vector3.Max(startPosition, endPosition);
			return new Rect(topLeft.x, Screen.height - bottomRight.y, bottomRight.x - topLeft.x,
				bottomRight.y - topLeft.y);
		}
		
		private void ClearSelection()
		{
			isSelecting = false;
			HideUnitsPreviewPositions();
			UnitSelectionManager.ClearSelectedUnits();
			selectionSphere.SetActive(false);
			UpdateControlState(ControlState.Selecting);
		}

		private void HandleScrollWheel(Vector2 value)
		{
			switch (controlState)
			{
				case ControlState.Selecting:
					break;
				case ControlState.Selected:
					HandleScrollMove(value);
					break;
				case ControlState.Move:
					HandleScrollMove(value);
					break;
			}
		}
	}
}