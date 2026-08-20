using Godot;
using System;
using System.Collections.Generic;

public abstract class ViewPack
{
	public string Name { get; init; }
}

public class ViewPack3D : ViewPack
{
	public Node3D[] Squads { get; init; }
	public Dictionary<Logic.Entities.Tiles.Type, Node3D> Tiles { get; init; }

	public static ViewPack3D FromName(string tileName, string squadName, Dictionary<string, ushort> keys)
	{
		const float gapScale = 2f / Visual.Models.Root3;

		Dictionary<Logic.Entities.Tiles.Type, Node3D> tiles = new(keys.Count);
		foreach (var type in Enum.GetValues<Logic.Entities.Tiles.Type>())
		{
			string path = $"scenes/tiles/{tileName}/{type}.tscn";
			Node3D tile = GD.Load<PackedScene>(path).Instantiate<Node3D>();
			tile.Scale = new(gapScale, 1f, gapScale);
			tiles.Add(type, tile);
		}

		Node3D[] squads = new Node3D[keys.Count];
		foreach (var name in keys.Keys)
		{
			string path = $"scenes/squads/{squadName}/{name}.tscn";
			Node3D model = GD.Load<PackedScene>(path).Instantiate<Node3D>();
			ushort key = keys[name];
			squads[key] = model;
		}

		return new()
		{
			Tiles = tiles,
			Squads = squads
		};
	}
}
