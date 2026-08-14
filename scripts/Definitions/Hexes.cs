using System.Collections.Generic;
using System;

namespace Definitions1;

public enum HexId : ushort
{
	Plains,
	Bushes,
	Swamp,
	River,
	Road,
	Farm,
	RiverRoad,
	Lake,
	Stones,
	Ravine,
	SparseForest,
	Forest,
}

public static partial class Classic
{
	public static readonly Dictionary<HexId, Hex> Hexes = new()
	{
		[HexId.Plains] = new()
		{
			Capabilities = new(Barrier: true, TroopTrench: true, TankTrench: true, DOT: true),
			CanAmbushOrder = []
		},
		[HexId.Bushes] = new()
		{
			Capabilities = new(Barrier: true, TroopTrench: true, TankTrench: true, DOT: true),
			CanAmbushOrder = []
		}
	};
}
