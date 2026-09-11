using System;
using System.Collections;
using System.Collections.Generic;
using Logic.Entities.Squads;

namespace Logic;

public readonly ref struct Squad(ushort id, Squads squads)
{
	// Entity values
	public ushort ID => id;
	public ushort Key => squads.Keys[id];
	public Definition Def => squads.Registry[Key];
	public ref Data Data => ref squads.Data[id];
	public Pos Pos => squads.Positions[id];
	// Entity compute values
	public byte Ammo => (byte)(Def.Stats.Ammo - Data.AmmoLoss);
	public byte Armor => (byte)(Def.Stats.Armor + Data.ArmorChange);
	public byte Count => (byte)(Def.Stats.Count - Data.Loss);
	public byte Durability => (byte)(Def.Stats.Durability.Value - Data.Loss);
	// Data dependent
	public bool Reach(Pos pos) => Map.Distance(Pos, pos) <= Def.FirePower.Range;
	public byte FP(Squad enemy)
	{
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
	public BitArray FireSupression { get; } = new(keys.Length);

	public Squad this[ushort id] => new(id, this);

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

	public AttackResult Attack(ushort attackerId, ushort targetId, byte cost = 1, float fpMult = 1f, byte? accuracySet = null)
	{
		var attacker = new Squad(attackerId, this);
		var target = new Squad(targetId, this);
		// 0 check ammo
		if (attacker.Ammo >= cost)
			attacker.Data.AmmoLoss += cost;
		else return AttackResult.NoAmmo;
		// 1 count target defense
		// 2 count view line and distance to target
		byte distance = Map.Distance(attacker.Pos, attacker.Pos);
		if (attacker.Def.FirePower.Range < distance)
			return AttackResult.Unreachable;
		// 3 get target type
		// 4 get fire power (as D6 count) (intensity)
		var fp = (byte)MathF.Floor(attacker.FP(target) * fpMult);
		if (fp == 0) return AttackResult.LowPower;
		// 5 roll dices
		// 6 compare rolls with accuracy
		byte acc;
		if (accuracySet.HasValue)
		{
			acc = accuracySet.Value;
		}
		else acc = attacker.Def.FirePower.Accuracy[distance - 1];
		// 7 remove dices > accuracy for distance
		// 8 other dices counts as attacks
		int attackCount = Dices.D6.Roll(fp).Count(max: acc);
		if (attackCount == 0) return AttackResult.Miss;
		// 9 attacks count -= targets defense, compute new defense
		if (target.Armor >= attackCount)
			return AttackResult.NoPenetration;
		attackCount -= target.Armor;
		// 10 now attacks counts as hits
		// 11 each hit removes durability count / loss
		target.Data.Loss += (byte)attackCount;
		return AttackResult.Success;
	}

	public void DurabilityTest(ushort id)
	{
		var squad = new Squad(id, this);
		var count = squad.Def.Stats.Durability.Rolls;
		var roll = Dices.D6.Roll(count).Sum();
		if (roll <= squad.Durability)
		{
			FireSupression[id] = false;
		}
		else
		{
			FireSupression[id] = true;
			squad.Data.Loss += 1;
		}
	}
}
