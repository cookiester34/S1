using System.Collections.Generic;

namespace S1.Runtime.UnitManagement
{
	public class UnitManager
	{
		private static UnitManager instance;
		public static UnitManager Instance => instance ??= CreateUnitManager();

		private static UnitManager CreateUnitManager()
		{
			if (instance != null)
			{
				throw new System.Exception("Critical Error, UnitManager already exists....");
			}
		
			return new UnitManager();
		}

		//TODO: Units will need pooling - just steal my pooling system from marching cubes project
		//Also need to think about how to handle different teams, and unit types
		public List<Unit> Units { get; private set; } = new();
	
		public void AddUnit(Unit unit)
		{
			Units.Add(unit);
		}

		public void DestroyUnit(Unit unit)
		{
			Units.Remove(unit);
		}
	}
}