using System;
using System.Collections.Generic;
using Logic.Entities.Tiles;

namespace Logic;

public record struct Pos(sbyte L1, sbyte L2);

public readonly ref struct Tile(Definition def, Pos pos)
{
	public readonly Definition Def { get; } = def;
	public readonly Pos Pos { get; } = pos;

	public bool IsNeighbour(Pos pos)
	{
		short
		dif1 = (short)(pos.L1 - Pos.L1),
		dif2 = (short)(pos.L2 - Pos.L2);
		if (Math.Abs(dif1) + Math.Abs(dif2) < 2) return true;

		if (dif1 + dif2 == 0) return true;

		return false;
	}

	public byte DistanceTo(Pos dest)
	{
		int l1 = Pos.L1 - dest.L1;
		int l2 = Pos.L2 - dest.L2;
		int cubeSum = Math.Abs(l1) + Math.Abs(l1 + l2) + Math.Abs(l2);
		return (byte)(cubeSum / 2);
	}
}

public class Map
{
	public Tile this[Pos pos]
	{
		get => new(
			def: Registry[Keys[pos]],
			pos: pos
		);
	}

	public const byte
	MinRadius = 1,
	MaxRadius = 127;
	private readonly byte radius;
	public required byte Radius
	{
		get => radius;
		init
		{
			if (value > MaxRadius)
				radius = MaxRadius;
			else if (value < MinRadius)
				radius = MinRadius;
			else
				radius = value;
		}
	}

	// Key -> Def
	public Dictionary<Entities.Tiles.Type, Definition> Registry { get; init; }
	// Pos -> Key
	public Dictionary<Pos, Entities.Tiles.Type> Keys { get; init; }

	public IEnumerable<Pos> EnumerateRadius()
	{
		return EnumerateRadius(Radius);
	}

	public static IEnumerable<Pos> EnumerateRadius(byte radius)
	{
		sbyte L1, L2;

		for (L1 = (sbyte)-radius; L1 < 1; L1++)
		{
			for (L2 = (sbyte)(-radius - L1); L2 < radius + 1; L2++)
			{
				yield return new Pos(L1, L2);
			}
		}
		for ( ; L1 < radius + 1; L1++)
		{
			for (L2 = (sbyte)-radius; L2 <= radius - L1; L2++)
			{
				yield return new Pos(L1, L2);
			}
		}
	}

	public static byte Distance(Pos from, Pos to)
	{
		int l1 = from.L1 - to.L1;
		int l2 = from.L2 - to.L2;
		int cubeSum = Math.Abs(l1) + Math.Abs(l1 + l2) + Math.Abs(l2);
		return (byte)(cubeSum / 2);
	}

	public static int CountArea(int radius)
	{
		int res = radius * radius;
		for (int i = 1; i <= radius; i++)
			res -= i * 2;
		return res;
	}
}
