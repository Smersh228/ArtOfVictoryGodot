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
			SquadId id = state.Squads[squadNum.Value];
			var def = state.Registry.Squads[id];
			ref Squads.Data data = ref state.SquadsData[squadNum.Value];

			var roll = Actions.RollD6(def.Stats.Durability.Rolls);
			var required = def.Stats.Durability.Value - data.Loss;
			if (roll <= required)
			{
				state.FireSupressionSquads.Remove(squadNum);
			}
			else
			{
				data.Loss += 1;
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
