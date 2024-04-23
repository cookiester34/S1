using S1.Runtime.Factions;
using S1.Runtime.UnitManagement;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace S1.Runtime.Teams
{
	[Serializable]
	public class Team
	{
		[field:SerializeField]
		public string Id { get; set; }
		[field:SerializeField]
		public FactionData Faction { get; set; }
		[field:SerializeField]
		public List<Team> Allies { get; set; } = new();
		[field:SerializeField]
		public List<Unit> Units { get; set; } = new();

		public Team(string id, FactionData faction)
		{
			Id = id;
			Faction = faction;
		}
		
		public void SetTeamId(string id)
		{
			Id = id;
		}
		
		public bool IsAlly(Team team)
		{
			return Allies.Contains(team);
		}
		
		public void AddAlly(Team ally)
		{
			if (Allies.Contains(ally)) return;
			
			Allies.Add(ally);
			ally.AddAlly(this);
		}
		
		public void RemoveAlly(Team ally)
		{
			if (!Allies.Contains(ally)) return;
			
			Allies.Remove(ally);
			ally.RemoveAlly(this);
		}

		public void AddUnit(Unit unit)
		{
			Units.Add(unit);
		}

		public void RemoveUnit(Unit unit)
		{
			Units.Remove(unit);
		}
	}
}