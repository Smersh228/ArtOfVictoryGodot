using Godot;
using System.Collections.Generic;

namespace Interaction.UI;

public partial class UISimple : Control, IUI
{
	[Export] Control Tile;
	[Export] ItemList SquadList;
	[Export] Control Squad;
	[Export] ItemList Orders;

	Logic.Pos? pos = null;
	ushort? id = null;

	public override void _Ready() => HideAll();

	public void HideAll()
	{
		Tile.Visible = false;
		SquadList.Visible = false;
		Squad.Visible = false;
		Orders.Visible = false;
	}

	public void Update(SelectionInfo2 data)
	{
		if (!data.Pos.HasValue)
		{
			HideAll();
			return;
		}
		else if (data.Pos != pos)
		{
			Update(data.Tile);
			Tile.Show();
		}

		if (data.Squads.Count == 0)
		{
			SquadList.Hide();
			Squad.Hide();
			Orders.Hide();
			return;
		}
		else
		{
			Update(data.Squads);
			SquadList.Show();
		}

		if (!data.ID.HasValue)
		{
			Squad.Hide();
			Orders.Hide();
			return;
		}
		else if (data.ID != id)
		{
			Update(data.Vis, data.Squad, data.Locale);
		}
		Squad.Show();
		Orders.Show();

		pos = data.Pos;
		id = data.ID;
	}

	private void Update(Logic.Tile tile)
	{
		Tile.GetNode<Label>("./Values/Pos").Text = $"Позиция ({tile.Pos.L1};{tile.Pos.L2})";
		Tile.GetNode<Label>("./Values/Barrier").Visible = tile.Def.Capabilities.Barrier;
		Tile.GetNode<Label>("./Values/TroopTrench").Visible = tile.Def.Capabilities.TroopTrench;
		Tile.GetNode<Label>("./Values/TankTrench").Visible = tile.Def.Capabilities.TankTrench;
		Tile.GetNode<Label>("./Values/DOT").Visible = tile.Def.Capabilities.DOT;
	}

	private void Update(IList<ushort> squads)
	{
		SquadList.Clear();
		foreach (var squad in squads)
		{
			SquadList.AddItem($"sID: {squad}");
		}
	}

	private void Update(Visual.SquadData vis, Logic.Squad squad, LocalePackString locale)
	{
		Squad.GetNode<Label>("./Selected/Name").Text = vis.Name;
		Squad.GetNode<Label>("./Selected/Type").Text = "Тип: " + locale.Types[squad.Def.Stats.Type.ToString()];
		Squad.GetNode<Label>("./Selected/Ammo").Text = $"Ammo: {squad.Ammo}";
		Squad.GetNode<Label>("./Selected/Armor").Text = $"Armor: {squad.Armor}";
		Squad.GetNode<Label>("./Selected/Count").Text = $"Count: {squad.Count}";
		Squad.GetNode<Label>("./Selected/Durability").Text = $"Durability: {squad.Durability}";

		Orders.Clear();
		foreach (byte key in squad.Def.Orders)
		{
			string text = $"Order {key + 1}";
			Orders.AddItem(text);
		}
	}
}
