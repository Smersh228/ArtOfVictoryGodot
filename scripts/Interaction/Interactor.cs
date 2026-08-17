using Godot;
using System;
using System.Collections.Generic;

namespace Interaction;

public partial class Interactor : Node
{
	private FreeCam cam = new();

	private Logic.GameState session;

	private Visual.Session visuals;

	public override void _Ready()
	{
		byte R = 10;
		Dictionary<Logic.Pos, ushort> keys = [];
		foreach (Logic.Pos pos in Logic.Map.EnumerateRadius(R))
		{
			keys.Add(pos, 0);
		}
		session = new()
		{
			Commands = [],
			Squads = new()
			{
				Keys = [0, 0],
				Data = new Logic.Entities.Squads.Data[2],
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

		visuals = new()
		{
			Map = new(Loader.LoadTileModels(), session.Map),
			Squads = new(Loader.LoadSquadModels(), session.Squads),
		};

		AddChild(cam);
		AddChild(visuals.Map);
		AddChild(visuals.Squads);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouse click)
		{
			if (click.IsReleased()) return;

			var unit = cam.RayCastSquad();
			if (unit is null) return;

			SelectSquad(unit);
		}
	}

	private void SelectSquad(Node3D unit)
	{
		Logic.Squad squad = default;
		Visual.SquadData data = default;

		if (visuals.Squads.Ids.TryGetValue(unit, out ushort id))
		{
			squad = session.Squads[id];
			data = visuals.Squads.Data[id];
		}

		UI.IUI ui = GetChild<UI.IUI>(0);
		ui.Visualise(data, squad);
	}
}
