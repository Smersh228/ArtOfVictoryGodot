using Godot;
using System.Collections.Generic;

namespace Interaction.UI;

public partial class UISimple : Control, IUI
{
	[Export] Control Tile;
	[Export] ItemList SquadList;
	[Export] Control Squad;
	[Export] ItemList Orders;

	public override void _Ready() => HideAll();

	public void HideAll()
	{
		Tile.Visible = false;
		SquadList.Visible = false;
		Squad.Visible = false;
		Orders.Visible = false;
	}

	public void Update(Logic.Tile tile, IList<ushort> squads)
	{
		Tile.GetNode<Label>("./Values/Pos").Text = $"Позиция ({tile.Pos.L1};{tile.Pos.L2})";
		Tile.GetNode<Label>("./Values/Barrier").Visible = tile.Def.Capabilities.Barrier;
		Tile.GetNode<Label>("./Values/TroopTrench").Visible = tile.Def.Capabilities.TroopTrench;
		Tile.GetNode<Label>("./Values/TankTrench").Visible = tile.Def.Capabilities.TankTrench;
		Tile.GetNode<Label>("./Values/DOT").Visible = tile.Def.Capabilities.DOT;
		Tile.Show();

		SquadList.Clear();
		foreach (var squad in squads)
		{
			SquadList.AddItem($"sID: {squad}");
		}
		if (squads.Count == 0)
		{
			SquadList.Hide();
		}
		else SquadList.Show();
	}

	public void HideTile()
	{
		Tile.Hide();
		SquadList.Hide();
	}

	public void Update(Visual.SquadData vis, Logic.Squad squad, LocalePackString locale)
	{
		Squad.GetNode<Label>("./Selected/Name").Text = vis.Name;
		Squad.GetNode<Label>("./Selected/Type").Text = "Тип: " + locale.Types[squad.Def.Stats.Type.ToString()];
		Squad.GetNode<Label>("./Selected/Ammo").Text = $"Ammo: {squad.Ammo}";
		Squad.GetNode<Label>("./Selected/Armor").Text = $"Armor: {squad.Armor}";
		Squad.GetNode<Label>("./Selected/Count").Text = $"Count: {squad.Count}";
		Squad.GetNode<Label>("./Selected/Durability").Text = $"Durability: {squad.Durability}";
		Squad.Show();

		Orders.Clear();
		foreach (byte key in squad.Def.Orders)
		{
			string text = $"Order {key + 1}";
			Orders.AddItem(text);
		}
		Orders.Show();
	}

	public void HideSquad()
	{
		Squad.Hide();
		Orders.Hide();
	}
}
