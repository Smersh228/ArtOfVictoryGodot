using System;
using System.Collections.Generic;
using Logic.Entities.Squads;

namespace Logic;

public readonly ref struct Squad
{
	public readonly Definition Def;
	public readonly ref Data Data;
	public readonly Pos Pos;

	public Squad(Definition def, ref Data data, Pos pos)
	{
		Def = def;
		Data = ref data;
		Pos = pos;
	}
}

public class Squads
{
	public Squad this[ushort id]
	{
		get => new(
			def: Registry[id],
			data: ref Data[id],
			pos: Positions[id]
		);
	}
	// Key -> Def
	public Definition[] Registry { get; init; }
	// Id -> Key
	public ushort[] Keys { get; init; }
	// Id -> Data
	public Data[] Data { get; init; }
	// Id -> Pos
	public Pos[] Positions { get; init; }
	// Id -> Is under fire supression
	public HashSet<ushort> FireSupression { get; init; }
}
