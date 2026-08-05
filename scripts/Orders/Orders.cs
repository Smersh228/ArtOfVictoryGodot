using Godot;
using System;

namespace Orders;

public class Description
{
	public byte Index { get; init; }
	public sbyte DefenseChange { get; init; } // or dAddition byte and dSubstraction byte
	public byte SquadCount { get; init; } = 0;
	public byte Duration { get; init; } = 1;
	public byte MoveRange { get; init; }
	// if DoFireSupression: fire intensity *= 1.5f (round to bigger)
}

public struct Data
{
	byte ExecutorId { get; set; }
	byte Duration { get; set; }
	byte Direction { get; set; } //(neighbour hex number) (direction sets to squad after execution)
	byte TargetId { get; set; } //target (squad ID) (or hex num)
								// current squad hex
								// hexes for moving to (maximum 4?)
}

public class Order
{
	public Description Description { get; init; }
	public Action<Data> Execute { get; init; }
}

public static class Ordrers
{
	public static Description
	Defense = new()
	{
		Index = 0,
		DefenseChange = +1,
		SquadCount = 0,
		Duration = 1,
		MoveRange = 0,
	},
	FireSupression = new()
	{
		Index = 1,
	}
	;

	//1
	public static void ExecuteDefense(Data order)
	{
		//minimum target range = 1 (not 0)
	}

	//2
	public static void ExecuteFireSupression(Data data)
	{

	}

	// 3

	public static void ExecuteFire(byte targetId)
	{

	}

	public static void ExecuteSmokeScreen(byte hexId)
	{

	}

	public static void ChangeArtilleryFire(byte friendlyTargetId)
	{

	}

	//4
	public static void ExecuteAirStrike(byte hexId) // storming/bombing
	{

	}

	public static void ExecuteResponseFire(byte squadId)
	{

	}

	public static void ExecuteInterception(byte squadId)
	{

	}

	public static void ExecuteTroopsLanding(byte squadId, byte hexId)
	{

	}

	public static void ExecuteSupply(byte hexId)
	{

	}

	public static void ExecuteAirProspecting(byte centerHexId, byte duration, byte radius)
	{

	}

	public static void ExecuteAirEscort(byte targetId, byte duration)
	{

	}

	public static void ExecuteAirPatrol(byte centerHex, byte radius, byte duration) //max 4 rad
	{

	}

	//5
	public static void ExecutePowerfulAttack(byte targetId, byte[] moveToHexes)
	{

	}

	public static void ExecuteAttack(byte targetId, byte[] moveToHexes)
	{

	}

	//6
	public static void ExecuteAmbush(byte targetRange, byte direction, byte currentHex, byte duration) //засада
	{
		//add 1 defense

	}

	//7


}
