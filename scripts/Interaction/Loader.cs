using System;

public static class Loader
{
	public static Logic.Entities.Tiles.Hex[] LoadTiles()
	{
		return
		[
			new() {
				CanAmbushOrder = [],
				Capabilities = new(),
			}
		];
	}

	public static Logic.Entities.Squads.Definition[] LoadSquads()
	{
		return
		[
			new() {
				Stats = new()
				{
					Ammo = 12,
					Armor = 12,
					Army = Logic.Entities.Squads.Army.USSR,
					Cost = 12,
					Count = 12,
					Durability = new()
					{
						Loss = 1,
						Rolls = 12,
						Value = 12,
					},
					Type = Logic.Entities.Squads.Type.Trooper
				}
			}
		];
	}
}
