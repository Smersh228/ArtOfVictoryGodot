using System;
using System.Collections.Generic;

namespace Units;

readonly struct Durability
{
	byte Rolls { get; init; }
	byte Value { get; init; }
	byte Loss { get; init; }
}

public class StatsGround
{
	Army Army { get; init; }
	byte Count { get; init; }
	Type Type { get; init; }
	Durability Durability { get; init; }
	byte Armor { get; init; }
	byte Ammo { get; init; }
	byte Cost { get; init; }
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
	public StatsGround Stats { get; init; }
	public FirePower FirePower { get; init; }
	public HashSet<Orders.Order> Orders { get; init; }
}

public struct Data
{
	public byte Loss { get; set; }
	public byte ArmorChange { get; set; }
	public byte AmmoLoss { get; set; }
}

public class PlayerTroops
{
	public Definition[] Definitions { get; init; }
	public Data[] Data { get; init; }
}
