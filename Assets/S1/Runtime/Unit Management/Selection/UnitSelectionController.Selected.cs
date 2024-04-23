using S1.Runtime.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace S1.Runtime.UnitManagement.Selection
{
	public partial class UnitSelectionController
	{
		private void HandleLeftClickSelected()
		{
			BeginSelection();
		}

		private void SelectedStateUpdate()
		{
			if (!UnitSelectionManager.HasSelectedUnits)
			{
				//Shouldn't be in this state if there are no selected units
				UpdateControlState(ControlState.Selecting);
				return;
			}
			
			if (isSelecting)
			{
				//Preview Selected Units
				// previewSelectedUnits = GetSelectedUnits(out _);
			}
		}

		private bool DidSelectionHappen()
		{
			HideUnitsPreviewPositions();
			isSelecting = false;
			zoomLevel = 10;

			var selectedUnits = GetSelectedUnits(out var typeSelected);
			if (selectedUnits.Count <= 0) return false;
			if (typeSelected == SelectionType.Player)
			{
				//Reset Selection to this selection
				UpdateSelection(selectedUnits, typeSelected);
			}
			else
			{
				//TODO: Should display some stats here
				//We are targeting enemies, change state to attack & update selection Sphere
				UpdateControlState(ControlState.Move);
				targetUnits.Clear();
				targetUnits.AddRange(selectedUnits);
				averageTargetPosition = selectedUnits.Select(unit => unit.UnitPosition).Average();
				
				selectionSphere.SetActive(true);
				selectionSphere.transform.position = averageTargetPosition;
				var sphereScale = Vector3.one * zoomLevel * 2f;
				selectionSphere.transform.localScale = sphereScale;
			}
			DisplayUnitsPreviewPositions();

			return true;
		}

		private void ShowSelectionUi(IEnumerable<Unit> selectedUnits)
		{
			switch (selectionType)
			{
				case SelectionType.Player:
					break;
				case SelectionType.Friendly:
					break;
				case SelectionType.Enemies:
					break;
			}
		}
	}
}