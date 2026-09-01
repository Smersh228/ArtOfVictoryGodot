using System.Collections.Generic;

namespace Interaction.UI;

public interface IUI
{
	void Update(Logic.Tile tile, IList<ushort> squads);

	void HideTile();

	void Update(Visual.SquadData vis, Logic.Squad squad, LocalePackString locale);

	void HideSquad();
}
