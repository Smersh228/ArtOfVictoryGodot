using Godot;
using Interaction.Import;
using System;

namespace Interaction.UI;

public partial class UISimple : Control, IUI
{
	public void Visualise(Visual.SquadData vis, Logic.Squad squad, LocalePackString locale)
	{
		if (squad.Def == null)
		{
			Visible = false;
			return;
		}
		Visible = true;

		var container = GetChild<HBoxContainer>(0);
		container.GetChild<Label>(0).Text = vis.Name;
		container.GetChild<Label>(1).Text = $"<{squad.Pos.L1};{squad.Pos.L2}>";
		container.GetChild<Label>(2).Text = "Тип: " + locale.Types[squad.Def.Stats.Type.ToString()];
	}

	public void Visualise(Logic.Tile tile)
	{

	}
}
