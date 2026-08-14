using System.Collections.Generic;
using Logic.Entities.Squads;

namespace Definitions1;

public enum SquadKey : ushort
{
	T34mod1940,
}

public static partial class SRegistry
{
	public static readonly Dictionary<ushort, Definition> Squads = new()
	{
		[(ushort)SquadKey.T34mod1940] = new()
		{
			Stats = new()
			{
				Army = Army.USSR,
				Ammo = 10,
				Armor = 3,
				Cost = 35,
				Count = 3,
				Durability = new()
				{
					Loss = 1,
					Rolls = 2,
					Value = 9
				},
				Type = Type.TankMedium,
			},
			FirePower = new()
			{
				Accuracy = [2, 2, 2, 1, 1],
				Normal = new()
				{
					[Type.Trooper] = [11, 9, 7],
					[Type.Artillery] = [12, 10, 8],
					[Type.GroundVehicle] = [14, 13, 12],
					[Type.GroundVehiclceDefensive] = [13, 12, 11],
					[Type.TankLight] = [11, 10, 9],
					[Type.TankMedium] = [10, 9, 8],
					[Type.TankHeavy] = [9, 8, 7],
				}
			},
			Orders =
			[
				//Orders[OrderId.Defense]
			]
		},
	};
}
