using Godot;
using System;
using System.Collections.Generic;

public partial class UIMapEditor : Control
{
	[Signal]
	public delegate void ColorButtonChangedEventHandler(string type);
	PackedScene colorButton = GD.Load<PackedScene>("res://color_check_button.tscn");
	private ButtonGroup _colorButtons;
	public override void _Ready()
	{
		HBoxContainer container = GetNode<HBoxContainer>("Container");

		_colorButtons = new();
		foreach (var tile in HexTileData.GlobalTileData)
		{

			ColorCheckButton cb = colorButton.Instantiate<ColorCheckButton>();
			// cb.SetColor(new(HexTileData.GlobalTileData[tile.Key]["color"].ToString()));
			cb.TileType = tile.Key;
			cb.SizeFlagsHorizontal = SizeFlags.ExpandFill;
			cb.SizeFlagsStretchRatio = 1;
			cb.Size = new(32, 32);
			container.Size = new(container.Size.X + cb.Size.X, container.Size.Y);
			container.AddChild(cb);
			cb.ButtonGroup = _colorButtons;
		}

		// AddUserSignal("color_button_changed", [new Godot.Collections.Dictionary() {{ "name", "color" },{ "type", (int)Variant.Type.Color }, }]);
		_colorButtons.Pressed += (button) =>
		{
			EmitSignal(SignalName.ColorButtonChanged, ((ColorCheckButton)button).TileType);
		};
	}

	
}
