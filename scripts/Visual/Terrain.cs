using Godot;
using System;
using System.Collections.Generic;
using Logic;

namespace Visual;

public partial class Map : Node3D
{
	public readonly Dictionary<Logic.Entities.Tiles.Type, Node3D> Registry;
	public readonly Dictionary<Pos, Node3D> Models;

	public Map(Dictionary<Logic.Entities.Tiles.Type, Node3D> registry, Logic.Map map)
	{
		Registry = registry;
		Models = [];
		foreach (var pos in map.EnumerateRadius())
		{
			Vector3 coord = Visual.Models.PosToWorld(pos);
			var key = map.Keys[pos];
			Node3D model = (Node3D)Registry[key].Duplicate();
			model.Position = coord;
			Models.Add(pos, model);
		}
		Models.TrimExcess();
	}

	public override void _Ready()
	{
		foreach (var model in Models.Values)
			AddChild(model);
	}
}
