using System;
using System.Collections.Generic;
using Logic.Entities.Tiles;

namespace Logic;

public record struct Pos(byte L1, byte L2);

public readonly ref struct Tile(Hex def, Pos pos)
{
	public readonly Hex Def { get; } = def;
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

	public const byte MaxRadius = 127;
	private readonly byte radius;
	public byte Radius
	{
		get => radius;
		init
		{
			if (value > MaxRadius)
				radius = MaxRadius;
			radius = value;
		}
	}

	// Key -> Def
	public Hex[] Registry { get; init; }
	// Pos -> Key
	public Dictionary<Pos, ushort> Keys { get; init; }

	public IEnumerable<Pos> EnumerateRadius()
	{
		return EnumerateRadius(Radius);
	}

	public static IEnumerable<Pos> EnumerateRadius(byte radius)
	{
		int D = radius * 2 + 1;
		byte L1, L2;
		for (L1 = 0; L1 < radius; L1++)
		{
			for (L2 = (byte)(radius - L1); L2 < D; L2++)
			{
				yield return new Pos(L1, L2);
			}
		}
		for (L2 = 0; L2 < D; L2++)
		{
			yield return new Pos(L1, L2);
		}
		L1++;
		for (int c = 0; c < radius; c++, L1++)
		{
			for (L2 = 0; L2 < D - c - 1; L2++)
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
}
