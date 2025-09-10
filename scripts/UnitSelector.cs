using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class UnitSelector : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode("IconContainer").ProcessMode = ProcessModeEnum.Inherit;
		UpdateIcons();
	}
	Node iconContainer;
	public void UpdateIcons()
	{
		iconContainer = GetNode<Node2D>("IconContainer");
		int childCount = iconContainer.GetChildCount();
		Array<Node> children = iconContainer.GetChildren();
		if (childCount == 0) return;
		for (int i = 0; i < childCount; i++)
		{
			Control child = children[i] as Control;
			child.Position = Vector2.Up.Rotated(Mathf.DegToRad(60) * i) * 100;
		}
	}
	public void SetUnits(Unit[] units)
	{
		foreach (Node child in iconContainer.GetChildren())
		{
			child.Free();
		}
		foreach (Unit unit in units)
		{
			if (unit is null) continue;
			UnitIcon unitIcon = GD.Load<PackedScene>("res://scenes/unit_icon.tscn").Instantiate<UnitIcon>();
			iconContainer.AddChild(unitIcon);
			unitIcon.ProcessMode = ProcessModeEnum.Inherit;
			unitIcon.SetUnit(unit.Type);
		}
	}
	public void ChangeChildVisibility(bool visible)
	{
		foreach (UnitIcon child in iconContainer.GetChildren())
		{
			child.Visible = visible;
		}
	}
	/// <summary>
	/// Делает селектор видимым. Селектор отображает юнитов на тайле
	/// </summary>
	/// <param name="tile">Тайл, юниты с которого будут показаны в селекторе</param>
	public void ShowSelector(Tile tile)
	{
		Visible = true;
		ProcessMode = ProcessModeEnum.Inherit;
		SetUnits(tile.Units);
		UpdateIcons();
		ChangeChildVisibility(true);
	}
	public void HideSelector()
	{
		ChangeChildVisibility(false);
		Visible = false;
		ProcessMode = ProcessModeEnum.Disabled;
	}
}
