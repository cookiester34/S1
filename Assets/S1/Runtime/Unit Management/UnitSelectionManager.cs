using System.Collections.Generic;
using UnityEngine;

namespace S1.Runtime.UnitManagement
{
	//Reason this is static is just in case we want access in a bunch of places.
	//Should only be one place that adds/removes units from the selection manager
	public static class UnitSelectionManager
	{
		private static UnitSelectionController selectionController;
		
		public static HashSet<Unit> SelectedUnits { get; private set; } = new();
		
		public static bool HasSelectedUnits => SelectedUnits.Count > 0;
		
		public static bool TakeControl(UnitSelectionController controller)
		{
			if (selectionController != null)
			{
				Debug.LogWarning($"Another controller: {selectionController.name} has control of the selection manager.");
				return false;
			}
			
			selectionController = controller;
			return true;
		}
		
		public static void ReleaseControl(UnitSelectionController controller)
		{
			if (selectionController == controller) return;
			if (selectionController != controller) return;
			
			selectionController = null;
		}
		
		public static void SetSelectedUnits(HashSet<Unit> units) => SelectedUnits = units;

		public static void AddSelectedUnit(Unit unit) => SelectedUnits.Add(unit);

		public static void ClearSelectedUnits() => SelectedUnits.Clear();
		
		public static void RemoveUnit(Unit unit) => SelectedUnits.Remove(unit);
	}
}
