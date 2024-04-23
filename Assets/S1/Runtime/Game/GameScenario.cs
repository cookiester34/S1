using S1.Runtime.Teams;
using S1.Runtime.UnitManagement;
using System;
using UnityEngine;

namespace S1.Runtime.S1.Runtime.Game
{
	public class GameScenario : MonoBehaviour
	{
		[SerializeField]
		//This will be removed later on when we have a proper ship system.
		private GameObject exampleShip;
		
		[SerializeField]
		private GameScenarioTeamData[] teams;

		private void Awake()
		{
			for (var index = 0; index < teams.Length; index++)
			{
				var teamData = teams[index];
				var team = teamData.team;
				
				if (index == 0)
				{
					TeamManager.SetPlayerTeam(team);
				}
				
				team.SetTeamId(Guid.NewGuid().ToString());
				if (TeamManager.TryRegisterTeam(team))
				{
					foreach (var spawnPoint in teamData.unitSpawnPoints)
					{
						var ship = Instantiate(exampleShip, spawnPoint.position, spawnPoint.rotation);
						var unit = ship.GetComponent<Unit>();
						unit.TrySetTeam(team);
					}
				}
			}
		}
	}
	
	[Serializable]
	public class GameScenarioTeamData
	{
		[SerializeField]
		// For now this is fine, later on we will need to know which ships to spawn and where.
		public Transform[] unitSpawnPoints;

		[SerializeField]
		public Team team;
	}
}