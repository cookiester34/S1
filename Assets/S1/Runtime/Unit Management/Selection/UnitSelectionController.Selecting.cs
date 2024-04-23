using S1.Runtime.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S1.Runtime.UnitManagement.Selection
{
	public partial class UnitSelectionController
	{
		private void HandleLeftClickSelecting()
		{
			BeginSelection();
		}
		
		//Move this to Move.cs
		private void HandleSelecting()
		{
			if (isSelecting)
			{
				previewSelectedUnits = GetSelectedUnits(out _);
			}
		}

		private void HandleLeftClickUpSelection()
		{
			isSelecting = false;
			zoomLevel = 10;
			
			var selectedUnits = GetSelectedUnits(out var typeSelected);
			UpdateSelection(selectedUnits, typeSelected);
			DisplayUnitsPreviewPositions();
		}

		private void UpdateSelection(IEnumerable<Unit> selectedUnits, SelectionType typeSelected)
		{
			HideUnitsPreviewPositions();
			UnitSelectionManager.ClearSelectedUnits();
			UnitSelectionManager.AddRangeSelectedUnits(selectedUnits);
			selectionType = typeSelected;

			if (!UnitSelectionManager.HasSelectedUnits) return;
			
			averageUnitPosition = UnitSelectionManager.SelectedUnits.Select(unit => unit.UnitPosition).Average();

			ShowSelectionUi(selectedUnits);
			if (selectionType != SelectionType.Player)
			{
				//Need to show stats of the given selection
				UpdateControlState(ControlState.Selected);
				return;
			}
			
			selectionSphere.SetActive(true);
			selectionSphere.transform.position = averageUnitPosition;
			var sphereScale = Vector3.one * zoomLevel * 2f;
			selectionSphere.transform.localScale = sphereScale;
				
			UpdateControlState(ControlState.Selected);
		}
	}
}