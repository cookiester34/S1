using S1.Runtime.Teams;
using System.Collections.Generic;
using System.Linq;

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
		public List<Unit> PlayerUnits => TeamManager.PlayerTeam.Units;
		public List<Unit> FriendlyUnits
		{
			get
			{
				var teams = TeamManager.GetFriendlyTeams();
				List<Unit> units = new();
				foreach (var team in teams)
				{
					units.AddRange(team.Units);
				}
				return units;
			}
		}

		public List<Unit> EnemyUnits
		{
			get
			{
				var teams = TeamManager.GetEnemyTeams();
				List<Unit> units = new();
				foreach (var team in teams)
				{
					units.AddRange(team.Units);
				}
				return units;
			}
		}
	}
}