using System;
using System.Collections;

namespace Logic.Entities.Tiles;

public readonly struct Capabilities(bool Barrier, bool TroopTrench, bool TankTrench, bool DOT)
{
	private readonly BitArray Flags = new([Barrier, TroopTrench, TankTrench, DOT]);
	public bool Barrier => Flags[0];
	public bool TroopTrench => Flags[1];
	public bool TankTrench => Flags[2];
	public bool DOT => Flags[3];
}

public class Definition
{
	public Capabilities Capabilities { get; init; }

	public Squads.Type[] CanAmbushOrder { get; init; }
}
