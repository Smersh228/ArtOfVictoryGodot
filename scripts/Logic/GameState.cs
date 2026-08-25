using System;
using System.Collections.Generic;

namespace Logic;

public record struct OrderCommand(Entities.Orders.Order Key, Entities.Orders.Data Data);

public class GameState
{
	public Squads Squads { get; init; }

	public List<OrderCommand> Commands { get; init; } // 1sq = 1hod = 1order

	public Map Map { get; init; }
}
