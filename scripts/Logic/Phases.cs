using System;

namespace Logic;

public static class Phases
{
	//1
	public static void GettingOrders()
	{

	}

	//2
	public static void DurabilityTest(GameState state, ushort sId)
	{
		Squad squad = state[sId];
		var roll = Actions.RollD6(squad.Def.Stats.Durability.Rolls);
		if (roll <= squad.Durability)
		{
			state.Squads.FireSupression.Remove(sId);
		}
		else
		{
			state.Squads.FireSupression.Add(sId);
			squad.Data.Loss += 1;
		}
	}

	public static void RecoveryTests(GameState state)
	{
		var count = state.Squads.FireSupression.Count;
		ushort[] FireSupressed = new ushort[count];
		state.Squads.FireSupression.CopyTo(FireSupressed);

		foreach (var sId in FireSupressed)
		{
			DurabilityTest(state, sId);
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
