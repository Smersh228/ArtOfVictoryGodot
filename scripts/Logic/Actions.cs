using System;

namespace Logic;

public static class Actions
{
	static readonly Random random = new();

	public static ushort RollD6(int count = 1)
	{
		ushort result = 0;
		for (int i = 0; i < count; i++)
		{
			result += (ushort)random.Next(1, 7);
		}
		return result;
	}

	public static ushort RollD6AndCount(int count = 1, int min = 1, int max = 6)
	{
		ushort result = 0;
		ushort roll;
		for (int i = 0; i < count; i++)
		{
			roll = (ushort)random.Next(1, 7);
			if (roll >= min && roll <= max)
				result += 1;
		}
		return result;
	}

	public static void TestForAttack(GameState state, SquadNum attacker, SquadNum target)
	{
		// 1 count target defense
		Squad t = state[target];
		Squad a = state[attacker];
		int armor = t.Def.Stats.Armor + t.Data.ArmorChange;
		// 2 count view line and distance to target
		byte distance = Map.Distance(a.Pos, t.Pos);
		if (distance > a.Def.FirePower.Range)
		{
			return;
		}
		// 3 get target type
		var type = t.Def.Stats.Type;
		// 4 get fire power (as D6 count)
		int fp;
		if (a.Pos == t.Pos)
		{
			if (!a.Def.FirePower.Melee.ContainsKey(type))
				return;
			fp = a.Def.FirePower.Melee[type][0];
		}
		if (!a.Def.FirePower.Normal.ContainsKey(type))
			return;
		fp = a.Def.FirePower.Normal[type][distance - 1];
		// 5 roll dices
		// 6 compare rolls with accuracy
		int ac = a.Def.FirePower.Accuracy[distance - 1];
		// 7 remove dices > accuracy for distance
		// 8 other dices counts as attacks
		int attackCount = RollD6AndCount(count: fp, max: ac);
		// 9 attacks count -= targets defense, compute new defense
		if (armor >= attackCount)
			return;
		attackCount -= armor;
		// 10 now attacks counts as hits
		// 11 each hit removes durability count / loss
		t.Data.Loss += (byte)attackCount;
	}

}
