using System;
using UnityEngine;

namespace S1.Runtime.Factions
{
	[Serializable]
	public class FactionData
	{
		[field:SerializeField]
		public string Name { get; set; }
		[field:SerializeField]
		public string Description { get; set; }
	}
}