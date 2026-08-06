using System;
using System.Collections.Generic;

namespace Logic;

public record struct HexId(ushort Value);
public record struct SquadId(ushort Value);
public record struct OrderId(ushort Value);

public record struct HexNum(ushort Value);
public record struct SquadNum(ushort Value);

public record struct OrderCommand(SquadNum Executor, OrderId Order);

public readonly ref struct Squad
{
	public readonly Squads.Definition Def;
	public readonly ref Squads.Data Data;

	public Squad(Squads.Definition def, ref Squads.Data data)
	{
		Def = def;
		Data = ref data;
	}
}

public class GameState
{
	public Squad this[SquadNum num]
	{
		get => new(Registry.Squads[Squads[num.Value]], ref SquadsData[num.Value]);
	}

	public Registry Registry { get; init; }
	//public HexId[] Hexes { get; init; }
	//public SquadId[] Squads { get; init; }
	public HexId[] Hexes { get; init; }
	public SquadId[] Squads { get; init; }
	public Squads.Data[] SquadsData { get; init; }
	public ushort[] Positions { get; init; }

	//public Dictionary<Squads.Army>
	public Queue<OrderCommand> Commands { get; init; } // 1sq = 1hod = 1order
	public HashSet<SquadNum> FireSupressionSquads { get; init; }
}
