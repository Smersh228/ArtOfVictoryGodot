using Godot;
using System;
using System.Collections.Generic;
using Interaction.Import;
using Interaction.Import.Orders;

namespace Interaction;

public readonly ref struct SelectionInfo2(Logic.Pos? pos, ushort? id, Logic.GameState state, Visual.Session visuals, LocalePackString locale)
{
	public Logic.Pos? Pos => pos;
	public ushort? ID => id;

	public readonly Logic.Tile Tile => state.Map[pos ?? new()];

	public readonly IList<ushort> Squads => state.Squads.OnTile(pos ?? new());

	public Visual.SquadData Vis => visuals.Squads.Data[id ?? 0];
	public readonly Logic.Squad Squad => state[id ?? 0];
	public readonly LocalePackString Locale => locale;
}

public enum State : byte
{
	Watch,
	Tile,
	Squad,
	Order
}

public class ImportedSquad<TOrder> where TOrder : unmanaged
{
	public Logic.Entities.Squads.Stats Stats { get; init; }
	public Logic.Entities.Squads.FirePower FirePower { get; init; }
	public HashSet<TOrder> Orders { get; init; }
}

public record struct LSel(Logic.Pos? Pos, ushort? ID);

public partial class Interactor : Node
{
	private FreeCam cam = new();

	private Logic.GameState session;

	private Visual.Session visuals;

	readonly LocalePackString locale = new();

	//NodeSelection selection;
	LSel sel;

	State state = State.Watch;

	// Getters
	UI.IUI UI => GetChild<UI.IUI>(0);

	public override void _Ready()
	{
		//temporary
		var temp = DataPack<ImportedSquad<Order>>.FromName("test");

		var tSet = temp.Registry;
		var nSet = new Logic.Entities.Squads.Definition[tSet.Length];
		for (int i = 0; i < nSet.Length; i++)
		{
			HashSet<byte> nOrders = new(tSet[i].Orders.Count);
			foreach (var order in tSet[i].Orders)
				nOrders.Add((byte)order);
			nSet[i] = new()
			{
				Stats = tSet[i].Stats,
				FirePower = tSet[i].FirePower,
				Orders = nOrders,
			};
		}
		DataPack<Logic.Entities.Squads.Definition> pack = new()
		{
			MetaData = temp.MetaData,
			Keys = temp.Keys,
			Registry = nSet,
		};
		//temporary end

		byte R = 10;
		session = new()
		{
			Orders = new([], [], 2),
			Squads = new(
				keys: [0, 0],
				data: [new(), new()],
				positions: [new(0, 0), new(2, 3)],
				set: pack.Registry
			),
			Map = Logic.Map.BuildRandomHexagonal(R, Loader.LoadTiles()),
		};

		ViewPack3D vPack = ViewPack3D.FromName(tileName: "cuboid", squadName: "paper", keys: pack.Keys);
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
			var result = Interact(node);

			Logic.Pos sPos = default;
			ushort sId = default;
			switch (result)
			{
				case Interaction.Tile:
					sPos = visuals.Map.Positions[node];
					break;
				case Interaction.Squad:
					sId = visuals.Squads.Ids[node];
					var squad = session.Squads[sId, session];
					sPos = squad.Pos;
					break;
			}

			switch (state)
			{
				case State.Order:
				case State.Tile:
				case State.Watch:
					switch (result)
					{
						case Interaction.None:
							DeselectSquad();
							DeselectTile();
							break;
						case Interaction.Squad:
							SelectSquad(sId);
							if (sel.Pos != sPos)
								SelectTile(sPos);

							state = State.Squad;
							break;
						case Interaction.Tile:
							SelectTile(sPos);

							state = State.Tile;
							break;
						case Interaction.Undefined:
							GD.Print("SELECTED UNDEFINED OBJECT! Inetractor.cs 86");
							break;
						default:
							throw new Exception("Fullfill switch > Intercator.cs 89");
					}
					break;
				case State.Squad:
					switch (result)
					{
						case Interaction.None:
							state = State.Watch;
							break;
						case Interaction.Squad:
							if (sel.ID == sId) break;
							DeselectSquad();
							SelectSquad(sId);

							if (sel.Pos != sPos)
								SelectTile(sPos);
							break;
						case Interaction.Tile:
							if (sel.ID is null) break;

							session.Squads.Positions[sel.ID.Value] = sPos;
							visuals.Squads.Models[sel.ID.Value].Position = Visual.Models.PosToWorld(sPos);

							DeselectSquad();
							DeselectTile();

							state = State.Watch;
							break;
						case Interaction.Undefined:
							break;
					}
					break;
			}
			SelectionInfo2 si = new(sel.Pos, sel.ID, session, visuals, locale);
			UI.Update(data: si);
		}
	}

	private void DeselectTile()
	{
		if (!sel.Pos.HasValue) return;

		var model = visuals.Map.Models[sel.Pos.Value];
		model.GetChild<AnimationPlayer>(1).PlayBackwards("SelectUp");

		sel.Pos = null;
	}

	private void DeselectSquad()
	{
		if (!sel.ID.HasValue) return;

		sel.ID = null;
	}

	/*
	private void ShowSelection(NodeSelection selection)
	{
		if (selection.Tile is null)
		{
			//show (hide) ui
			UI.HideAll();
			return;
		}
		var pos = visuals.Map.Positions[selection.Tile];
		var tile = session.Map[pos];
		UI.Update(tile);

		var list = session.Squads.OnTile(pos);
		UI.Update(list);

		if (selection.Squad is null)
		{
			//hide ui
			return;
		}
		var id = visuals.Squads.Ids[selection.Squad];
		var squad = session.Squads[id, session];
		UI.Update(visuals.Squads.Data[id], squad, locale);
	}
	 */

	private void SelectTile(Logic.Pos? pos)
	{
		(bool, bool) values = (sel.Pos.HasValue, pos.HasValue);
		switch (values)
		{
			case (false, false): // none => none
				break;
			case (false, true): // none => selected
				var model1 = visuals.Map.Models[pos.Value];
				model1.GetChild<AnimationPlayer>(1).Play("SelectUp");
				break;
			case (true, false): //selected => none
				DeselectTile();
				break;
			case (true, true): //selected => selected
				if (pos.Value == sel.Pos.Value)
				{
					DeselectTile();
					return;
				}

				DeselectTile();
				var model2 = visuals.Map.Models[pos.Value];
				model2.GetChild<AnimationPlayer>(1).Play("SelectUp");
				break;
		};
		sel.Pos = pos;
		//old
		/*
		if (!sel.Pos.HasValue && !pos.HasValue) return;

		if (pos.HasValue && sel.Pos.HasValue)
		{
			if (pos.Value == sel.Pos.Value)
			{
				DeselectTile();
				return;
			}
		}
		DeselectTile();

		var model = visuals.Map.Models[pos.Value];
		model.GetChild<AnimationPlayer>(1).Play("SelectUp");

		sel.Pos = pos;
		 */
	}

	private void SelectSquad(ushort? id)
	{
		sel.ID = id;
	}

	public enum Interaction
	{
		None,
		Squad,
		Tile,
		Undefined
	}

	private Interaction Interact(Node node)
	{
		if (node is null)
			return Interaction.None;

		if (visuals.Squads.Ids.ContainsKey(node))
			return Interaction.Squad;

		if (visuals.Map.Positions.ContainsKey(node))
			return Interaction.Tile;

		return Interaction.Undefined;
	}
}
