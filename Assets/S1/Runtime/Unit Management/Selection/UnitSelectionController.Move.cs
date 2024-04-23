using UnityEngine;

namespace S1.Runtime.UnitManagement.Selection
{
	public partial class UnitSelectionController
	{
		private void HandleScrollMove(Vector2 value)
		{
			if (!UnitSelectionManager.HasSelectedUnits) return;

			zoomLevel += value.y * zoomSpeed * Time.deltaTime;
			var sphereScale = Vector3.one * zoomLevel * 2f;
			selectionSphere.transform.localScale = sphereScale;
		}
		
		private void HandleMovement()
		{
			if (UnitSelectionManager.HasSelectedUnits)
			{
				var hasTargetUnits = targetUnits.Count > 0;
				foreach (var unit in UnitSelectionManager.SelectedUnits)
				{
					var targetPosition = unit.UnitPosition + offset;
					var directionToTarget = (averageUnitPosition - targetPosition).normalized;
					var unitRotation = Quaternion.LookRotation(directionToTarget);
					if (!hasTargetUnits)
					{
						unitRotation = unit.transform.rotation;
					}
					unit.Move(targetPosition, unitRotation);
				}
			}
			
			HideUnitsPreviewPositions();
			UnitSelectionManager.ClearSelectedUnits();
		}
		
		private void HandleMovingToTarget()
		{
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
			
			DisplayUnitsPreviewPositions();
		}
	}
}