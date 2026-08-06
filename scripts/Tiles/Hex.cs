using System;
using System.Collections;

public readonly struct Capabilities(bool Barrier, bool TroopTrench, bool TankTrench, bool DOT)
{
	private readonly BitArray Flags = new([Barrier, TroopTrench, TankTrench, DOT]);
	public bool Barrier => Flags[0];
	public bool TroopTrench => Flags[1];
	public bool TankTrench => Flags[2];
	public bool DOT => Flags[3];
}

public class Hex
{
	public Capabilities Capabilities { get; init; }

	public Units.Type[] CanAmbushOrder { get; init; }
}
