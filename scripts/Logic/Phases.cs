using System;

namespace Logic;

public static class Phases
{
	//1
	public static void GettingOrders()
	{

	}

	//2
	public static void RecoveryTests(GameState state)
	{
		foreach (var sId in state.Squads.FireSupression)
		{
			Squad squad = state.Squads[sId];

			var roll = Actions.RollD6(squad.Def.Stats.Durability.Rolls);
			var required = squad.Def.Stats.Durability.Value - squad.Data.Loss;
			if (roll <= required)
			{
				state.Squads.FireSupression.Remove(sId);
			}
			else
			{
				squad.Data.Loss += 1;
			}
		}
	}

	//3
	public static void RadioInterception()
	{

	}

	//4
	public static void ExecuteOrders()
	{

	}
}
