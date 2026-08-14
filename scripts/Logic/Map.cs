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

	// Key -> Def
	public Hex[] Registry { get; init; }
	// Pos -> Key
	public Dictionary<Pos, ushort> Keys { get; init; }

	public static byte Distance(Pos from, Pos to)
	{
		int l1 = from.L1 - to.L1;
		int l2 = from.L2 - to.L2;
		int cubeSum = Math.Abs(l1) + Math.Abs(l1 + l2) + Math.Abs(l2);
		return (byte)(cubeSum / 2);
	}
}
