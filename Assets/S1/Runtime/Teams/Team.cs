using S1.Runtime.Factions;

namespace S1.Runtime.Teams
{
	public class Team
	{
		public string Id { get; private set; }
		public FactionData Faction { get; private set; }

		public Team(string id, FactionData faction)
		{
			Id = id;
			Faction = faction;
		}
	}
}