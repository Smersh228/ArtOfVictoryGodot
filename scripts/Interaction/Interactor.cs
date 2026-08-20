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

	Node3D selectedSquad = null;

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

			var node = cam.RayCastSquad();

			var result = Select(node);

			switch (result)
			{
				case Selection.None:
					return;
				case Selection.Squad:
					SelectSquad(node);
					return;
				case Selection.Tile:
					if (selectedSquad is null) return;
					var pos = visuals.Map.Positions[node];

					var id = visuals.Squads.Ids[selectedSquad];
					session.Squads.Positions[id] = pos;

					selectedSquad.Position = Visual.Models.PosToWorld(pos);
					selectedSquad = null;
					return;
				case Selection.Undefined:
					GD.Print("SELECTED UNDEFINED OBJECT! Inetractor.cs 86");
					return;
				default:
					throw new Exception("Fullfill switch > Intercator.cs 89");
			}
		}
	}

	private void SelectSquad(Node3D unit)
	{
		selectedSquad = unit;

		ushort id = visuals.Squads.Ids[unit];

		var squad = session.Squads[id];
		var data = visuals.Squads.Data[id];

		UI.IUI ui = GetChild<UI.IUI>(0);
		ui.Visualise(data, squad, locale);
	}

	public enum Selection
	{
		None,
		Squad,
		Tile,
		Undefined
	}

	private Selection Select(Node node)
	{
		if (node is null)
			return Selection.None;

		if (visuals.Squads.Ids.ContainsKey(node))
			return Selection.Squad;

		if (visuals.Map.Positions.ContainsKey(node))
			return Selection.Tile;

		return Selection.Undefined;
	}
}
