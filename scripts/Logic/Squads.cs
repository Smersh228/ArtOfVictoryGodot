using System;
using System.Collections.Generic;
using Logic.Entities.Squads;

namespace Logic;

public readonly ref struct Squad(ushort id, Squads squads)
{
	public Definition Def => squads.Registry[id];
	public ref Data Data => ref squads.Data[id];
	public Pos Pos => squads.Positions[id];
}

public class Squads(Definition[] set, ushort[] keys, Data[] data, Pos[] positions)
{
	// Key -> Def
	public Definition[] Registry => set;
	// Id -> Key
	public ushort[] Keys => keys;
	// Id -> Data
	public Data[] Data => data;
	// Id -> Pos
	public Pos[] Positions => positions;
	// Id -> Is under fire supression
	public HashSet<ushort> FireSupression { get; init; }

	public Squad this[ushort id] => new(id, this);
}
