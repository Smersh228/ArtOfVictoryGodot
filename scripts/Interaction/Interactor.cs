using Godot;
using System;
using System.Collections.Generic;
using Interaction.Import;

namespace Interaction;

public enum State : byte
{
	Watch,
	Tile,
	Squad,
	Order
}

public partial class Interactor : Node
{
	private FreeCam cam = new();

	private Logic.GameState session;

	private Visual.Session visuals;

	readonly DataPack<Logic.Entities.Squads.Definition> pack = DataPack<Logic.Entities.Squads.Definition>.FromName("test");
	ViewPack3D vPack;
	readonly LocalePackString locale = new();

	Node3D selectedSquad = null;
	Node3D selectedTile = null;

	State state = State.Watch;

	public override void _Ready()
	{
		byte R = 10;
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
			Map = Logic.Map.BuildRandomHexagonal(R, Loader.LoadTiles()),
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
			switch (state)
			{
				case State.Order:
				case State.Tile:
				case State.Watch:
					switch (result)
					{
						case Selection.None:
							return;
						case Selection.Squad:
							state = State.Squad;
							SelectTile(selectedTile);
							SelectSquad(node);
							return;
						case Selection.Tile:
							state = State.Tile;
							SelectTile(node);
							return;
						case Selection.Undefined:
							GD.Print("SELECTED UNDEFINED OBJECT! Inetractor.cs 86");
							return;
						default:
							throw new Exception("Fullfill switch > Intercator.cs 89");
					}
				case State.Squad:
					switch (result)
					{
						case Selection.None:
							state = State.Watch;
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
							state = State.Watch;
							return;
						case Selection.Undefined:
							return;
					}
					return;
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

	private void SelectTile(Node3D tile)
	{
		selectedTile?.GetChild<AnimationPlayer>(1).PlayBackwards("SelectUp");

		if (selectedTile == tile)
		{
			selectedTile = null;
			return;
		}

		selectedTile = tile;
		tile.GetChild<AnimationPlayer>(1).Play("SelectUp");
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
