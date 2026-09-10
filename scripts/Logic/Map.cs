using System;
using System.Collections.Generic;
using System.Numerics;
using Logic.Entities.Tiles;

namespace Logic;

enum Axis
{
	///<summary> Top -> Bottom </summary>
	L1,
	///<summary> Bottom right -> Top left </summary>
	L2,
	///<summary> Bottom left -> Top Right </summary>
	L3
}

///<summary> Axial coordinates directions (all 6 values)</summary>
public static class Dir
{
	public static readonly Pos
	Top = new(00, -1),
	TopR = new(+1, -1),
	BotR = new(+1, 00),
	Bot = new(00, +1),
	BotL = new(-1, +1),
	TopL = new(-1, 00);
}

public record struct Pos(sbyte L1, sbyte L2) : IEquatable<Pos>
{
	static public Pos operator +(Pos a, Pos b) => new((sbyte)(a.L1 + b.L1), (sbyte)(a.L2 + b.L2));

	static public Pos operator *(Pos a, byte b) => new((sbyte)(a.L1 * b), (sbyte)(a.L2 * b));

	public readonly Vector2 Vec2
	{
		get
		{
			float x = HStep * L1;
			float vOffset = -((L1 / 2f) + L2);
			float y = VStep * vOffset;
			return new(x, y);
		}
	}

	// For 2D transformation
	const float
	Root3 = 1.732051f,
	HStep = Root3 / 2f,
	VStep = 1f;
};

public static class Vector2Extension
{
	/// <summary>
	/// Returns angle in <b>degrees</b>
	/// </summary>
	public static float AngleTo(this Vector2 a, Vector2 b)
	{
		float dot = Vector2.Dot(a, b);
		float cosAlpha = dot / (a.Length() * b.Length());
		float rad = MathF.Acos(cosAlpha);
		return rad * 180f / MathF.PI;
	}
}

public readonly ref struct Tile(Pos pos, Map map)
{
	public Definition Def => map.Registry[map.Keys[pos]];
	public Pos Pos => pos;
}

public class Map
{
	// Key -> Def
	public Dictionary<byte, Definition> Registry { get; init; }
	// Pos -> Key
	public Dictionary<Pos, byte> Keys { get; init; }

	private readonly byte radius;

	// Invariants
	public const byte
	MinRadius = 1,
	MaxRadius = 127;
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

	// Indexer

	public Tile this[Pos pos] => new(pos, this);

	// Constructors

	public static Map BuildRandomHexagonal(byte radius, Dictionary<byte, Definition> registry)
	{
		Dictionary<Pos, byte> keys = [];
		var count = registry.Count;
		byte[] values = new byte[count];
		registry.Keys.CopyTo(values, 0);
		foreach (Pos pos in EnumerateRadius(radius))
		{
			var index = Random.Shared.Next() % values.Length;
			keys.Add(pos, values[index]);
		}
		keys.TrimExcess();

		return new()
		{
			Radius = radius,
			Registry = registry,
			Keys = keys,
		};
	}

	// Functions

	public bool IsNeighbour(Pos pos, Pos pos2)
	{
		short
		dif1 = (short)(pos.L1 - pos2.L1),
		dif2 = (short)(pos.L2 - pos2.L2);
		if (Math.Abs(dif1) + Math.Abs(dif2) < 2) return true;

		if (dif1 + dif2 == 0) return true;

		return false;
	}

	public IEnumerable<Pos> EnumerateRadius() => EnumerateRadius(Radius);

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

	/// <summary>
	/// Includes FROM position
	/// </summary>
	public static IEnumerable<Pos> Raycast(Pos from, Pos to)
	{
		var distance = Distance(from, to);
		var samples = distance + 1;

		for (byte s = 0; s < samples; s++)
		{
			float t = 1f / distance * s;

			Lerp(from.L1, to.L1, t, out float L1);
			Lerp(from.L2, to.L2, t, out float L2);
			Round(ref L1, ref L2);

			yield return new Pos((sbyte)L1, (sbyte)L2);
		}
	}

	private static void Lerp(float a, float b, float t, out float result)
	{
		result = a + (b - a) * t;
	}

	private static void Round(ref float L1, ref float L2)
	{
		float L3 = -L1 + -L2;

		float
		l1 = MathF.Round(L1),
		l2 = MathF.Round(L2),
		l3 = MathF.Round(L3);

		float
		abs1 = MathF.Abs(l1 - L1),
		abs2 = MathF.Abs(l2 - L2),
		abs3 = MathF.Abs(l3 - L3);

		if (abs1 > abs2 && abs1 > abs3)
			l1 = -l2 + -l3;
		else if (abs2 > abs3)
			l2 = -l1 + -l3;

		L1 = l1;
		L2 = l2;
	}

	public static readonly
	Dictionary<Pos, (Pos L, Pos R)> Sector60Vecs = new()
	{
		[Dir.Top ] = (Dir.TopL, Dir.TopR),
		[Dir.TopR] = (Dir.Top, 	Dir.BotR),
		[Dir.BotR] = (Dir.TopR, Dir.Bot	),
		[Dir.Bot ] = (Dir.BotR, Dir.BotL),
		[Dir.BotL] = (Dir.Bot, 	Dir.TopL),
		[Dir.TopL] = (Dir.BotL, Dir.Top	),
	};

	// Shoot sector
	public static IEnumerable<Pos> Sector(Pos from, Pos dir, byte range)
	{
		(Pos L, Pos R) = Sector60Vecs[dir];
		Pos pos0 = from + dir;

		for (byte l = 0; l < range; l++)
			for (byte r = 0; r < range; r++)
				yield return pos0 + (L * l) + (R * r);
	}

	public static bool IsInSector60(Pos target, Pos center, Pos secDir)
	{
		var dir = target.Vec2 - center.Vec2;
		short deg = (short)secDir.Vec2.AngleTo(dir);
		return MathF.Abs(deg) <= 30;
	}
}
