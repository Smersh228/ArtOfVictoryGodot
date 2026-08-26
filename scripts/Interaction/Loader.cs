using System;
using System.Collections.Generic;

namespace Interaction;

public static class Loader
{
	public static Dictionary<byte, Logic.Entities.Tiles.Definition> LoadTiles()
	{
		Dictionary<byte, Logic.Entities.Tiles.Definition> result = [];
		foreach (var t in Enum.GetValues<Import.TileType>())
		{
			result.Add((byte)t, new());
		}
		result.TrimExcess();
		return result;
	}

	// public static Dictionary<Logic.Pos, Logic.Entities.Tiles.Type> LoadMap()
	// {
	// 	Deserializer deser = new();
	// 	return deser.Deserialize<Dictionary<Logic.Pos, Logic.Entities.Tiles.Type>>(YMLLoader.ymlMap);
	// }
}
