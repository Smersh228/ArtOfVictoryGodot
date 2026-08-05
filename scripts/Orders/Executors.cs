using Godot;
using System;

namespace Orders;

public static class List
{
	public static Order
	Defense = new()
	{
		Description = new()
		{
			Index = 0,
			DefenseChange = +1,
		},
		Execute = (Data d) =>
		{
			// shoot slavik
		}
	};
}
