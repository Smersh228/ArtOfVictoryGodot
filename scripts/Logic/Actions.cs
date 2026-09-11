using System;
using System.Collections.Generic;

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

public struct Dice(byte sides)
{
	public readonly byte Roll() => (byte)Random.Shared.Next(1, sides + 1);

	public readonly IEnumerable<byte> Roll(int count)
	{
		for (int i = 0; i < count; i++)
			yield return Roll();
	}
}

public static class Dices
{
	public static readonly Dice
	D4 = new(4),
	D6 = new(6),
	D20 = new(20);

	public static int Sum(this IEnumerable<byte> rolls)
	{
		int result = 0;
		foreach (var roll in rolls)
			result += roll;
		return result;
	}

	public static int Count(this IEnumerable<byte> rolls, byte max = byte.MaxValue, byte min = 1)
	{
		ushort result = 0;
		foreach (var roll in rolls)
		{
			if (roll >= min && roll <= max)
				result += 1;
		}
		return result;
	}
}
