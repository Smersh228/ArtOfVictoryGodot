using Godot;
using System;

public partial class ColorCheckButton : Button
{
	private Color _color;
	private string _type;
	public void SetColor(Color color)
	{
		_color = color;
		StyleBoxFlat newStyleboxNormal = GetThemeStylebox("normal").Duplicate() as StyleBoxFlat;
		newStyleboxNormal.BgColor = _color;
		AddThemeStyleboxOverride("normal", newStyleboxNormal);
		StyleBoxFlat newStyleboxPressed = GetThemeStylebox("pressed").Duplicate() as StyleBoxFlat;
		newStyleboxPressed.BgColor = _color;
		AddThemeStyleboxOverride("pressed", newStyleboxNormal);
	}
	public Color GetColor()
	{
		return _color;
	}

	public string TileType
	{
		get { return _type; }
		set
		{
			_type = value;
			SetColor(new(HexTileData.GlobalTileData[_type]["color"].ToString()));
		}
	}
}
