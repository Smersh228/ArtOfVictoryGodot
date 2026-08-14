using System.Collections.Generic;
using Logic.Entities.Orders;

namespace Definitions1;

public enum OrderId : ushort
{
	Defense,
}

public static partial class Classic
{
	public static readonly Dictionary<OrderId, Order>
	Orders = new()
	{
		[OrderId.Defense] = new()
		{
			Description = new()
			{
				Index = 0,
				DefenseChange = +1,
			},
			Execute = Data =>
			{

			}
		},
	};
}
