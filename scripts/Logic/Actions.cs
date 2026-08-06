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

}
