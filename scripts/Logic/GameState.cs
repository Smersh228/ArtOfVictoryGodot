using System;

namespace Logic;

public class GameState
{
	public Squads Squads { get; init; }

	public Orders Orders { get; init; } // 1sq = 1hod = 1order

	public Map Map { get; init; }
}
