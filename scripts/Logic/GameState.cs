using System;
using System.Collections.Generic;

namespace Logic;

public record struct OrderCommand(ushort ExecutorId, ushort OrderKey);

public class GameState
{
	public Squads Squads { get; init; }

	public Queue<OrderCommand> Commands { get; init; } // 1sq = 1hod = 1order

	public Map Map { get; init; }
}
