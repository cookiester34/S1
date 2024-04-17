using System.Collections.Generic;

public class UnitManager
{
	private static UnitManager instance;
	public static UnitManager Instance => instance ??= CreateUnitManager();

	private static UnitManager CreateUnitManager()
	{
		if (instance != null)
		{
			throw new System.Exception("UnitManager already exists");
		}
		
		return new UnitManager();
	}

	public List<Unit> Units { get; private set; } = new();
	
	public void AddUnit(Unit unit)
	{
		Units.Add(unit);
	}
	
	public void RemoveUnit(Unit unit)
	{
		Units.Remove(unit);
	}
}