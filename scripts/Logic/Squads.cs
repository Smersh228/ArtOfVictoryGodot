using System;
using System.Collections.Generic;
using Logic.Entities.Squads;

namespace Logic;

public readonly ref struct Squad(ushort id, GameState state)
{
	// Entity values
	public ushort ID => id;
	public ushort Key => state.Squads.Keys[id];
	public Definition Def => state.Squads.Registry[Key];
	public ref Data Data => ref state.Squads.Data[id];
	public Pos Pos => state.Squads.Positions[id];
	// Entity compute values
	public byte Ammo => (byte)(Def.Stats.Ammo - Data.AmmoLoss);
	public byte Armor => (byte)(Def.Stats.Armor + Data.ArmorChange);
	public byte Count => (byte)(Def.Stats.Count - Data.Loss);
	public byte Durability => (byte)(Def.Stats.Durability.Value - Data.Loss);
	// State dependent
	public byte Order => state.Orders.Keys[id];
	public IEnumerable<Pos> Sector => Map.Sector(Pos, new(0, -1), Def.FirePower.Range);
	// Data dependent
	public bool Reach(Pos pos) => Map.Distance(Pos, pos) <= Def.FirePower.Range;
	public byte FP(ushort sId)
	{
		var enemy = state[sId];
		var dist = Map.Distance(Pos, enemy.Pos);

		if (dist > Def.FirePower.Range)
			return 0;

		var power = Def.FirePower[enemy.Def.Stats.Type];
		if (power.Length == 0)
			return 0;

		byte level = (byte)(power.Length - Count);
		return power[level];
	}
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

	public Squad this[ushort id, GameState state] => new(id, state);

	public List<ushort> OnTile(Pos pos)
	{
		List<ushort> list = new(5);
		for (ushort id = 0; id < Positions.Length; id++)
		{
			if (positions[id] == pos)
				list.Add(id);
		}
		return list;
	}
}
