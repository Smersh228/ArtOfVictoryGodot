using System;

namespace Logic.Entities.Orders;

public enum Order
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
