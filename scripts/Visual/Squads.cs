using Godot;
using System;
using System.Collections.Generic;
using Logic;

namespace Visual;

public class SquadData
{
	public string Name { get; init; }
}

public partial class Squads : Node3D
{
	public readonly Node3D[] Registry;

	// Node -> Id
	public readonly Dictionary<Node, ushort> Ids;
	// Id -> Visual data
	public readonly SquadData[] Data;

	public Squads(Node3D[] registry, Logic.Squads squads)
	{
		var count = squads.Keys.Length;

		Registry = registry;
		Ids = new(squads.Keys.Length);
		Data = new SquadData[count];

		for (ushort id = 0; id < squads.Keys.Length; id++)
		{
			Pos p = squads.Positions[id];
			ushort key = squads.Keys[id];

			Node3D model = (Node3D)Registry[key].Duplicate();
			model.Position = Models.PosToWorld(p, 2);

			Ids.Add(model, id);
			Data[id] = new() { Name = "Тест имя" };
		}
	}

	public override void _Ready()
	{
		foreach (var model in Ids.Keys)
			AddChild(model);
	}
}
