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
		foreach (SquadNum squadNum in state.FireSupressionSquads)
		{
			Squad squad = state[squadNum];

			var roll = Actions.RollD6(squad.Def.Stats.Durability.Rolls);
			var required = squad.Def.Stats.Durability.Value - squad.Data.Loss;
			if (roll <= required)
			{
				state.FireSupressionSquads.Remove(squadNum);
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
