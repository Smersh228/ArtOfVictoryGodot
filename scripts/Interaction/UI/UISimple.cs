using Godot;
using System;

namespace Interaction.UI;

public partial class UISimple : Control, IUI
{
	public void Visualise(Visual.SquadData vis, Logic.Squad squad)
	{
		if (squad.Def == null)
		{
			Visible = false;
			return;
		}
		Visible = true;

		var container = GetChild<HBoxContainer>(0);
		container.GetChild<Label>(0).Text = vis.Name;
		container.GetChild<Label>(1).Text = squad.Pos.ToString();
		container.GetChild<Label>(2).Text = squad.Def.Stats.Type.ToString();
	}

	public void Visualise(Logic.Tile tile)
	{

	}
}
