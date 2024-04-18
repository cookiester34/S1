using System.Collections.Generic;

namespace S1.Runtime.Teams
{
	public static class TeamManager
	{
		private static Dictionary<string, Team> registeredTeams = new();
		
		public static bool TryRegisterTeam(Team team)
		{
			return registeredTeams.TryAdd(team.Id, team);
		}
	}
}