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

// TODO: add hexes for moving orders (maximum 4?)
public record struct Data(ushort ExecutorId, Pos Direction, ushort TargetId, Pos TargetPos, byte Duration);

public class Definition
{
	public Description Description { get; init; }
	public Action<Data> Execute { get; init; }
}
