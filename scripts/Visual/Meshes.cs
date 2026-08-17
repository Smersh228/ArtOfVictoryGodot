using Godot;
using System;

namespace Visual;

public class Models
{
	//static readonly float Root3 = MathF.Sqrt(3);
	public const float
	Root3 = 1.732051f,
	XStep = Root3,
	ZStep = 0.75f,

	HexW = Root3,
	HexH = 2,

	HStep = Root3 / 2f,
	VStep1 = 4f / 2f,
	VStep2 = 1f;

	public static Vector3 PosToWorld(Logic.Pos pos, byte radius)
	{
		//float x = Terrain.XStep * pos.L1  / 2f;
		//float z = Terrain.ZStep * (((pos.L1 - radius) / 2f) + pos.L2);

		//xOffset = XStep * pos.L1 / 2f;
		//zOffset = ZStep * (((pos.L1 - map.Radius) / 2f) + pos.L2);

		// xOffset = XStep * pos.L1 / 2f;
		// zOffset = ZStep * (((pos.L1 - map.Radius) / 2f) + pos.L2) / 0.75f;

		//xOffset = HexW * pos.L1 / 2f;
		//zOffset = HexH * (((pos.L1 - map.Radius) / 2f) + pos.L2) / 2f;

		//xOffset = HStep * pos.L1;
		// zOffset = VStep1 * (((pos.L1 - map.Radius) / 2f) + pos.L2) / 2f;

		//var vOffset = (pos.L1 - map.Radius) / 2f;
		//zOffset = VStep2 * (vOffset + pos.L2);


		float x = HStep * pos.L1;
		float vOffset = (pos.L1 - radius) / 2f;
		float z = VStep2 * (vOffset + pos.L2);

		return new(x, 0, z);
	}

	public static Vector3 PosToWorld(Logic.Pos pos)
	{
		float x = HStep * pos.L1;
		float vOffset = -((pos.L1 / 2f) + pos.L2);
		float z = VStep2 * vOffset;

		return new(x, 0, z);
	}
}
