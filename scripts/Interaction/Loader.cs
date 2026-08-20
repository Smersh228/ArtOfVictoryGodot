using System;
using System.Collections.Generic;
using System.IO;
using Godot;
using YamlDotNet.Serialization;

namespace Interaction;

public static class Loader
{
	public static Dictionary<Logic.Entities.Tiles.Type, Logic.Entities.Tiles.Definition> LoadTiles()
	{
		Dictionary<Logic.Entities.Tiles.Type, Logic.Entities.Tiles.Definition> result = [];
		foreach (var t in Enum.GetValues<Logic.Entities.Tiles.Type>())
		{
			result.Add(t, new());
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
