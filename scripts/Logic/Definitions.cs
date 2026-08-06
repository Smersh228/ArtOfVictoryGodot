using System;
using System.Collections.Generic;

namespace Logic;

public class Registry
{
	public Dictionary<SquadId, Squads.Definition> Squads { get; init; }
	public Dictionary<SquadId, Hex> Hexes { get; init; }
	//public Dictionary<SquadId, Orders.Order> Orders { get; init; }
}
