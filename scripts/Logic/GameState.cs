using System;

namespace Logic;

public class GameState
{
	public Squads Squads { get; init; }

	public Orders Orders { get; init; } // 1sq = 1hod = 1order

	public Map Map { get; init; }

	// Functions

	public bool Observe(Pos from, Pos to)
	{
		foreach (var pos in Map.Raycast(from, to))
		{
			if (Map[pos].Def.Capabilities.Barrier)
				return false;
		}
		return true;
	}

	public bool Observe(ushort fromId, ushort toId)
		=> Observe(Squads[fromId].Pos, Squads[toId].Pos);

	public bool IsInSector60(ushort target, ushort attacker)
		=> Map.IsInSector60(Squads[target].Pos, Squads[attacker].Pos, Orders.Data[attacker].Direction);

	public AttackResult Attack(ushort target, ushort attacker, byte cost = 1, float fpMult = 1f, byte? accuracySet = null)
	{
		if (IsInSector60(target, attacker))
		{
			return Squads.Attack(attacker, target, cost, fpMult, accuracySet);
		}
		else return AttackResult.Unreachable;
	}

	public void RecoveryTests()
	{
		var flags = Squads.FireSupression;
		for (ushort id = 0; id < flags.Length; id++)
		{
			if (flags[id]) Squads.DurabilityTest(id);
		}
	}

	public byte Distance(ushort fromId, ushort toId)
	{
		return Map.Distance(Squads[fromId].Pos, Squads[toId].Pos);
	}
}
