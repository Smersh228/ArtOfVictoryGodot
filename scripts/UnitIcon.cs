using Godot;
using System;

public partial class UnitIcon : Control
{
	Sprite2D icon;
	private UnitData globalUnitData;

	public override void _Ready()
	{
		icon = GetNode<Sprite2D>("UnitIcon");
	}
	public void SetIcon(Texture2D texture)
	{
		icon.Texture = texture;
	}
	public void SetUnit(string unit)
	{
		globalUnitData = UnitData.Instance;

		icon.Texture = GD.Load<Texture2D>(globalUnitData[unit]["icon"].ToString());
	}
}
