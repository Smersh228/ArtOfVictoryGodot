using System;

namespace Interaction.UI;

public interface IUI
{
	public void Visualise(Visual.SquadData vis, Logic.Squad squad);

	public void Visualise(Logic.Tile hex);
}
