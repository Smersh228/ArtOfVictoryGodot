using System;
using Interaction.Import;

namespace Interaction.UI;

public interface IUI
{
	public void Visualise(Visual.SquadData vis, Logic.Squad squad, LocalePackString locale);

	public void Visualise(Logic.Tile hex);
}
