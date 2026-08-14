using System;

namespace Logic.Entities.Orders;

public class Description
{
	public byte Index { get; init; }
	public sbyte DefenseChange { get; init; }
	public byte SquadCount { get; init; } = 0;
	public byte Duration { get; init; } = 1;
	public byte MoveRange { get; init; }
}

public struct Data
{
	byte ExecutorId { get; set; }
	byte Duration { get; set; }
	byte Direction { get; set; } //(neighbour hex number) (direction sets to squad after execution)
	byte TargetId { get; set; } //target (squad ID) (or hex num)
								// hexes for moving to (maximum 4?)
}

public class Order
{
	public Description Description { get; init; }
	public Action<Data> Execute { get; init; }
}
