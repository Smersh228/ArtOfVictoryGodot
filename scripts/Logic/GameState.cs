using System;
using System.Collections.Generic;

namespace Logic;

public record struct HexId(ushort Value);
public record struct SquadId(ushort Value);
public record struct OrderId(ushort Value);

public record struct HexNum(byte L1, byte L2);
public record struct SquadNum(ushort Value);

public record struct OrderCommand(SquadNum Executor, OrderId Order);

public readonly ref struct Squad
{
	public readonly Squads.Definition Def;
	public readonly ref Squads.Data Data;
	public readonly HexNum Pos;

	public Squad(Squads.Definition def, ref Squads.Data data, HexNum pos)
	{
		Def = def;
		Data = ref data;
		Pos = pos;
	}
}

public class GameState
{
	public Squad this[SquadNum num]
	{
		get => new(
			Registry.Squads[Squads[num.Value]],
			ref SquadsData[num.Value],
			Positions[num.Value]
		);
	}

	public Registry Registry { get; init; }
	//public HexId[] Hexes { get; init; }
	//public SquadId[] Squads { get; init; }
	public HexId[] Hexes { get; init; }
	public SquadId[] Squads { get; init; }
	public Squads.Data[] SquadsData { get; init; }
	public HexNum[] Positions { get; init; }

	//public Dictionary<Squads.Army>
	public Queue<OrderCommand> Commands { get; init; } // 1sq = 1hod = 1order
	public HashSet<SquadNum> FireSupressionSquads { get; init; }

	public Map Map { get; init; }
	public Tile this[HexNum pos]
	{
		get => new();
		//get => new(Registry.Hexes[Hexes[pos.]], pos);
	}
}
