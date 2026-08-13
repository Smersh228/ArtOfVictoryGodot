using System;
using System.Collections.Generic;

namespace Logic;

public readonly ref struct Tile
{
	public readonly Hex Def { get; }
	public readonly HexNum Pos { get; }

	public Tile(Hex def, HexNum pos)
	{
		Def = def;
		Pos = pos;
	}

	public bool IsNeighbour(HexNum pos)
	{
		short
		dif1 = (short)(pos.L1 - Pos.L1),
		dif2 = (short)(pos.L2 - Pos.L2);
		if (Math.Abs(dif1) + Math.Abs(dif2) < 2) return true;

		if (dif1 + dif2 == 0) return true;

		return false;
	}

	public byte DistanceTo(HexNum dest)
	{
		int l1 = Pos.L1 - dest.L1;
		int l2 = Pos.L2 - dest.L2;
		int cubeSum = Math.Abs(l1) + Math.Abs(l1 + l2) + Math.Abs(l2);
		return (byte)(cubeSum / 2);
	}
}

public class Map
{
	public Tile this[HexNum pos]
	{
		get => new();
	}
	public Dictionary<HexNum, HexId> HexIds { get; init; }

	public static byte Distance(HexNum from, HexNum to)
	{
		int l1 = from.L1 - to.L1;
		int l2 = from.L2 - to.L2;
		int cubeSum = Math.Abs(l1) + Math.Abs(l1 + l2) + Math.Abs(l2);
		return (byte)(cubeSum / 2);
	}
}
