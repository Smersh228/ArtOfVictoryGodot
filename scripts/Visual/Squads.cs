using Godot;
using System;
using System.Collections.Generic;
using Logic;
using Logic.Entities.Squads;

namespace Visual;

public partial class Squads : Node3D
{
	public readonly Node3D[] Models;

	public Squads(Models models, Logic.Squads squads)
	{
		Models = new Node3D[squads.Keys.Length];

		for (int i = 0; i < squads.Keys.Length; i++)
		{
			Pos p = squads.Positions[i];
			ushort key = squads.Keys[i];

			//var squadScene = GD.Load<PackedScene>("scenes/squads/paper.tscn");
			//Node3D sq = squadScene.Instantiate<Node3D>();
			Node3D sq = (Node3D)models.squads[key].Duplicate();
			sq.Position = Visual.Models.PosToWorld(p, 2);

			Models[i] = sq;
		}
	}

	public override void _Ready()
	{
		foreach (var model in Models)
			AddChild(model);
	}
}
