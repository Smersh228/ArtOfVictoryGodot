using Godot;
using System;

public partial class UnitIcon : Control
{
	Sprite2D icon;
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
		icon.Texture = GD.Load<Texture2D>(UnitData.GlobalUnitData[unit]["icon"].ToString());
	}
}
