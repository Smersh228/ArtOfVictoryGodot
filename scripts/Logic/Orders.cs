using System;
using System.Collections.Generic;
using Logic.Entities.Orders;

namespace Logic;

public readonly ref struct Order(ushort squadId, Orders orders)
{
	public byte Key => orders.Keys[squadId];
	public Description Desc => orders.Registry[Key];
	public Action<GameState, Data> Action => orders.ActionRegistry[Key];
	public Data Data => orders.Data[squadId];

	public void Execute(GameState state) => Action.Invoke(state, Data);
};

public class Orders (Dictionary<byte, Description> set, Dictionary<byte, Action<GameState, Data>> actionSet, ushort squadsCount)
{
	// Key -> Def
	public Dictionary<byte, Description> Registry { get; } = set;
	// Key -> Function
	public Dictionary<byte, Action<GameState, Data>> ActionRegistry { get; } = actionSet;
	// SquadId -> Key
	public byte[] Keys { get; } = new byte[squadsCount];
	// SquadId -> Data
	public Data[] Data { get; } = new Data[squadsCount];
}
