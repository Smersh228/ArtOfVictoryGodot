using System;
using System.IO;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Interaction.Import;

public enum RuleSet
{
	Classic3,
	Classic4,
	DevArt,
}

public enum Variant
{
	Web,
	Godot,
}

public class MetaData
{
	public string Name { get; init; }
	public ushort Version { get; init; }
	public Variant Variant { get; init; }
	public RuleSet RuleSet { get; init; }
}

public class DataPack<SquadDef>
{
	public MetaData MetaData { get; init; }
	public Dictionary<string, ushort> Keys { get; init; }
	public SquadDef[] Registry { get; init; }

	public DataPack() { }

	public DataPack(Dictionary<string, string> ymls)
	{
		Deserializer deser = new();
		Keys = new(ymls.Count);
		Registry = new SquadDef[ymls.Count];
		ushort key = 0;
		foreach (var squadYml in ymls)
		{
			var def = deser.Deserialize<SquadDef>(squadYml.Value);
			Keys.Add(squadYml.Key, key);
			key++;
			Registry[Keys[squadYml.Key]] = def;
		}
	}

	public static DataPack<SquadDef> FromName(string name)
	{
		DirectoryInfo dir = new($"resources/datapacks/{name}/squads/");

		Dictionary<string, string> ymls = [];
		foreach (var file in dir.EnumerateFiles())
		{
			string yml = File.ReadAllText(file.FullName);
			ymls.Add(file.Name[0..^4], yml);
		}
		ymls.TrimExcess();
		return new(ymls);
	}
}
