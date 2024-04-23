using System.Collections.Generic;
using System.Linq;

namespace S1.Runtime.Teams
{
	public static class TeamManager
	{
		public static Team PlayerTeam { get; private set; }
		public static Dictionary<string, Team> Teams { get; } = new();
		
		public static List<Team> GetFriendlyTeams() => Teams.Values.Where(team => team.IsTeamFriendly()).ToList();

		public static List<Team> GetEnemyTeams() => Teams.Values.Where(team => !team.IsTeamFriendly()).ToList();

		public static void SetPlayerTeam(Team team)
		{
			PlayerTeam = team;
		}
		
		public static bool TryRegisterTeam(Team team)
		{
			return Teams.TryAdd(team.Id, team);
		}
		
		public static bool IsPlayerTeam(this Team team)
		{
			return team == PlayerTeam;
		}

		public static bool IsTeamFriendly(this Team team)
		{
			return team.IsAlly(PlayerTeam);
		}
	}
}