using Godot;
using System;
using System.Collections.Generic;
using Logic;

namespace Visual;

public partial class Terrain : Node3D
{
	public readonly Dictionary<Pos, Node3D> Models;

	public Terrain(Models meshes, Map map)
	{
		Models = [];
		foreach (var pos in map.EnumerateRadius())
		{
			Vector3 coord = Visual.Models.PosToWorld(pos);
			ushort key = map.Keys[pos];
			Node3D model = (Node3D)meshes.tiles[key].Duplicate();
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
