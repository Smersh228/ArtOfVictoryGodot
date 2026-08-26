using System;
using System.Collections.Generic;
using Logic.Entities.Orders;

namespace Logic;

public enum Order : byte
{
	Defense,
	FireForSupression,
	Fire,
	Smoke,
	ArtilleryFireRedirection,

	//5
	PowerfulAtack,
	Attack,

	//8
	Wait,
	Move,
	BattleMove,
	ShootInMove
}

public class Orders
{
	public Dictionary<Order, Description> Registry { get; init; }
	public Dictionary<Order, Action<GameState, Data>> ActionRegistry { get; init; }
}
