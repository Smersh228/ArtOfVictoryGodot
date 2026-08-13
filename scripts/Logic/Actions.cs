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

	public static void TestForAttack(GameState state, Squad attacker, Squad target)
	{
		// 1 count target defense
		int armor = target.Def.Stats.Armor + target.Data.ArmorChange;
		// 2 count view line and distance to target
		byte distance = Map.Distance(attacker.Pos, target.Pos);
		if (distance > attacker.Def.FirePower.Range)
		{
			return;
		}
		// 3 get target type
		var type = target.Def.Stats.Type;
		// 4 get fire power (as D6 count)
		int fp;
		if (attacker.Pos == target.Pos)
		{
			if (!attacker.Def.FirePower.Melee.ContainsKey(type))
				return;
			fp = attacker.Def.FirePower.Melee[type][0];
		}
		if (!attacker.Def.FirePower.Normal.ContainsKey(type))
			return;
		fp = attacker.Def.FirePower.Normal[type][distance - 1];
		// 5 roll dices
		// 6 compare rolls with accuracy
		int ac = attacker.Def.FirePower.Accuracy[distance - 1];
		// 7 remove dices > accuracy for distance
		// 8 other dices counts as attacks
		int attackCount = RollD6AndCount(count: fp, max: ac);
		// 9 attacks count -= targets defense, compute new defense
		if (armor >= attackCount)
			return;
		attackCount -= armor;
		// 10 now attacks counts as hits
		// 11 each hit removes durability count / loss
		target.Data.Loss += (byte)attackCount;
	}

}
