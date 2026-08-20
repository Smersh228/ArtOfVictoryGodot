using Godot;
using System;
using System.Collections.Generic;
using Interaction.Import;

namespace Interaction;

public partial class Interactor : Node
{
	private FreeCam cam = new();

	private Logic.GameState session;

	private Visual.Session visuals;

	readonly DataPack<Logic.Entities.Squads.Definition> pack = DataPack<Logic.Entities.Squads.Definition>.FromName("test");
	ViewPack3D vPack;
	readonly LocalePackString locale = new();

	public override void _Ready()
	{
		byte R = 10;
		Dictionary<Logic.Pos, Logic.Entities.Tiles.Type> keys = [];
		var values = Enum.GetValues<Logic.Entities.Tiles.Type>();
		foreach (Logic.Pos pos in Logic.Map.EnumerateRadius(R))
		{
			var index = Random.Shared.Next() % values.Length;
			keys.Add(pos, values[index]);
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
				Registry = pack.Registry
			},
			Map = new()
			{
				Radius = R,
				Registry = Loader.LoadTiles(),
				Keys = keys,
			}
		};

		vPack = ViewPack3D.FromName(tileName: "cuboid", squadName: "paper", keys: pack.Keys);
		visuals = new()
		{
			Map = new(vPack.Tiles, session.Map),
			Squads = new(vPack.Squads, session.Squads),
		};

		locale.LoadFromName("test", "locale");

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
		ui.Visualise(data, squad, locale);
	}
}
