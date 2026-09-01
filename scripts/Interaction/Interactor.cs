using Godot;
using System;
using System.Collections.Generic;
using Interaction.Import;
using Interaction.Import.Orders;

namespace Interaction;

class Selectable<T> where T : struct, IEquatable<T>
{
	public T? Field;
	public event Action<T> OnSelect, OnDeselect;

	public bool Empty => !Field.HasValue;

	public Diff Cmp(T value)
	{
		if (Empty)
			return Diff.Add;
		else if (value.Equals(Field.Value))
			return Diff.None;
		else //if (value != Field)
			return Diff.Mod;
	}

	public void Deselect()
	{
		if (!Field.HasValue) return;
		OnDeselect(Field.Value);
		Field = null;
	}

	public void Select(T value)
	{
		OnSelect(value);
		Field = value;
	}

	public void Set(T value)
	{
		Deselect();
		Select(value);
	}
}

enum Diff
{
	Add, // null -> obj
	None, // obj1 -> obj1
	Mod, // obj1 -> obj2
}

public class ImportedSquad<TOrder> where TOrder : unmanaged
{
	public Logic.Entities.Squads.Stats Stats { get; init; }
	public Logic.Entities.Squads.FirePower FirePower { get; init; }
	public HashSet<TOrder> Orders { get; init; }
}

public partial class Interactor : Node
{
	private FreeCam cam = new();

	private Logic.GameState session;

	private Visual.Session visuals;

	readonly LocalePackString locale = new();

	readonly Selectable<Logic.Pos> tile = new();
	readonly Selectable<ushort> squad = new();

	// Getters
	UI.IUI UI => GetChild<UI.IUI>(0);

	public override void _Ready()
	{
		#region temp
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
		#endregion

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
			Map = new(vPack.Tiles, session.Map)
			{
				Select = pos =>
				{
					var model = visuals.Map.Models[pos];
					model.GetChild<AnimationPlayer>(1).Play("SelectUp");
				},
				Deselect = pos =>
				{
					var model = visuals.Map.Models[pos];
					model.GetChild<AnimationPlayer>(1).PlayBackwards("SelectUp");
				}
			},
			Squads = new(vPack.Squads, session.Squads),
		};

		locale.LoadFromName("test", "locale");

		//selection
		tile.OnSelect += pos =>
		{
			visuals.Map.Select(pos);
			var squads = session.Squads.OnTile(pos);
			UI.Update(tile: session.Map[pos], squads);
		};
		tile.OnDeselect += pos =>
		{
			visuals.Map.Deselect(pos);
			UI.HideTile();
		};
		squad.OnSelect += id =>
		{
			UI.Update(squad: session[id], locale: locale, vis: visuals.Squads.Data[id]);
		};
		squad.OnDeselect += id =>
		{
			UI.HideSquad();
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

			var node = cam.RayCastSquad();

			if (node is null)
			{
				return;
			}
			else if (visuals.Map.Positions.ContainsKey(node))
			{
				ProcessTile(node);
			}
			else if (visuals.Squads.Ids.ContainsKey(node))
			{
				ProcessSquad(node);
			}
		}
	}

	public void ProcessTile(Node node)
	{
		var pos = visuals.Map.Positions[node];

		switch (tile.Cmp(pos))
		{
			case Diff.Add:
				tile.Select(pos); break;
			case Diff.Mod:
				squad.Deselect();
				tile.Set(pos);
				break;
			case Diff.None:
				if (squad.Empty)
				{
					tile.Deselect();
				}
				else squad.Deselect();
				break;
		}
	}

	public void ProcessSquad(Node node)
	{
		var id = visuals.Squads.Ids[node];
		var lSquad = session.Squads[id, session];
		var pos = lSquad.Pos;

		switch (tile.Cmp(pos))
		{
			case Diff.Add:
				tile.Select(pos);
				break;
			case Diff.Mod:
				tile.Set(pos);
				break;
		}
		switch (squad.Cmp(id))
		{
			case Diff.Add:
				squad.Select(id);
				break;
			case Diff.Mod:
				squad.Set(id);
				break;
			case Diff.None:
				squad.Deselect();
				tile.Deselect();
				break;
		}
	}
}
