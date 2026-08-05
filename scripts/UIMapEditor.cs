using Godot;
using System;
using System.Collections.Generic;

public partial class UIMapEditor : Control
{
	[Signal]
	public delegate void ColorButtonChangedEventHandler(string type);
	PackedScene colorButton = GD.Load<PackedScene>("res://scenes/color_check_button.tscn");
	private ButtonGroup _colorButtons;
	public override void _Ready()
	{
		HBoxContainer container = GetNode<HBoxContainer>("Container");

		_colorButtons = new();
		float sizeX = 0f;
		foreach (var tile in HexTileData.GlobalTileData)
		{
			sizeX += 32f;
			ColorCheckButton cb = colorButton.Instantiate<ColorCheckButton>();
			// cb.SetColor(new(HexTileData.GlobalTileData[tile.Key]["color"].ToString()));
			cb.TileType = tile.Key;
			cb.SizeFlagsHorizontal = SizeFlags.ExpandFill;
			cb.SizeFlagsStretchRatio = 1;
			container.AddChild(cb);
			cb.ButtonGroup = _colorButtons;
		}
		container.Size = new(container.Size.X + sizeX, container.Size.Y);

		// AddUserSignal("color_button_changed", [new Godot.Collections.Dictionary() {{ "name", "color" },{ "type", (int)Variant.Type.Color }, }]);
		_colorButtons.Pressed += (button) =>
		{
			EmitSignal(SignalName.ColorButtonChanged, ((ColorCheckButton)button).TileType);
		};
	}
}
