using System;
using System.Collections.Generic;

namespace Logic.Entities.Squads;

public readonly struct Durability
{
	public byte Rolls { get; init; }
	public byte Value { get; init; }
	public byte Loss { get; init; }
}

public class Stats
{
	public Army Army { get; init; }
	public byte Count { get; init; }
	public Type Type { get; init; }
	public Durability Durability { get; init; }
	public byte Armor { get; init; }
	public byte Ammo { get; init; }
	public byte Cost { get; init; }
}

public class FirePower
{
	public byte Range => (byte)Accuracy.Length;
	public byte[] Accuracy { get; init; }
	public Dictionary<Type, byte[]> Normal { get; init; }
	public Dictionary<Type, byte[]> Melee { get; init; }
}

public class Definition
{
	public Stats Stats { get; init; }
	public FirePower FirePower { get; init; }
	public HashSet<Orders.Order> Orders { get; init; }
}

public struct Data
{
	public byte Loss { get; set; }
	public byte ArmorChange { get; set; }
	public byte AmmoLoss { get; set; }
}
