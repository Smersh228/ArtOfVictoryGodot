using System;

namespace Logic;

public enum AttackResult : byte
{
	Unreachable,
	NoAmmo,
	LowPower,
	Miss,
	NoPenetration,
	Success
}

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
}
