namespace Interaction.Import.Orders;

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
