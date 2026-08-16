using Godot;
using System;
using System.Collections.Generic;

namespace Logic.Interaction;

public partial class Interactor : Node
{
	FreeCam cam = new();

	private GameState session;

	private Visual.Models models;

	public override void _Ready()
	{
		byte R = 10;
		Dictionary<Pos, ushort> keys = [];
		foreach (Pos pos in Map.EnumerateRadius(R))
		{
			keys.Add(pos, 0);
		}
		session = new()
		{
			Commands = [],
			Squads = new()
			{
				Keys = [0, 0],
				Data = new Entities.Squads.Data[2],
				FireSupression = [],
				Positions = [
					new(0, 0),
					new(2, 3)
				],
				Registry = Loader.LoadSquads(),
			},
			Map = new()
			{
				Radius = R,
				Registry = Loader.LoadTiles(),
				Keys = keys,
			}
		};

		models = new();

		Visual.Terrain map = new(models, session.Map);
		Visual.Squads squads = new(models, session.Squads);

		AddChild(cam);
		AddChild(map);
		AddChild(squads);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouse click)
		{
			if (click.IsReleased()) return;

			var unit = cam.RayCast();
			GD.Print("!!! Interactor: ", unit);
		}
	}

}
