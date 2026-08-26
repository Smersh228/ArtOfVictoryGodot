using Godot;
using System;
using System.Collections.Generic;

namespace Interaction.Import;

public abstract class ViewPack
{
	public string Name { get; init; }
}

public class ViewPack3D : ViewPack
{
	public Node3D[] Squads { get; init; }
	public Dictionary<byte, Node3D> Tiles { get; init; }

	public static ViewPack3D FromName(string tileName, string squadName, Dictionary<string, ushort> keys)
	{
		const float gapScale = 2f / Visual.Models.Root3;

		Dictionary<byte, Node3D> tiles = new(keys.Count);

		string pathT = $"scenes/tiles/tile.tscn";
		PackedScene baseScene = GD.Load<PackedScene>(pathT);
		foreach (var type in Enum.GetValues<TileType>())
		{
			var tile = baseScene.Instantiate<StaticBody3D>();
			tile.Scale = new(gapScale, 1f, gapScale);

			Node3D model = GD.Load<PackedScene>($"scenes/tiles/{tileName}/models/{type}.tscn").Instantiate<Node3D>();
			tile.AddChild(model);
			tile.GetChild<AnimationPlayer>(1).RootNode = "../Model";
			tiles.Add((byte)type, tile);
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
