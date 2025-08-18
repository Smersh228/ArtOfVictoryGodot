using Godot;
using System;

public partial class Tile
{
	private int _x, _y;  //квадратные координаты
	public int X
	{
		get { return _x; }
	}
	public int Y
	{
		get { return _y; }
	}
	private int _height;
	public int Height
	{
		get { return _height; }
		set { if ((0 <= value) && (value <= 15)) _height = value; }
	}
	private string _tileType;
	public string Type
	{
		get { return _tileType; }
		set { _tileType = value; } //Добавить проверку на валидный тип
	}
	public bool IsActiveTile
	{
		get;
		private set;
	}
	public bool HasRiver
	{
		get;
		set;
	}
	public void ChangeRiver()
	{
		HasRiver = !HasRiver;
	}

	private Tile[] neighbors = new Tile[6];

	public Tile(int x, int y, int height, string tileType)
	{
		_x = x;
		_y = y;
		Height = height;
		_tileType = tileType;
	}
	public Vector3 GetHexCoords()
	{
		return new Vector3(_x - _y / 2, _y, -(_x - _y / 2) - _y);
	}
	public Vector3 GetWorldPosition()
	{
		Vector3 pos = new();
		pos.X = (X + Y % 2f / 2) * HexMetrics.innerRadius * 2;
		pos.Y = Height / 16f;
		pos.Z = Y * HexMetrics.outerRadius * 3 / 2;
		return pos;
	}
	public Vector2[] GetNeighborsCoords()
	{
		return [
			new Vector2(_x+1,_y),
			new Vector2(_x + _y % 2,_y+1),
			new Vector2(_x-1+ _y % 2,_y+1),
			new Vector2(_x-1,_y),
			new Vector2(_x + _y % 2 - 1,_y-1),
			new Vector2(_x + 1 + _y % 2 - 1,_y-1)
		];
	}

	public Tile GetNeighbor(HexDirection direction)
	{
		return neighbors[(int)direction];
	}
	public void SetNeighbor(HexDirection direction, Tile cell)
	{
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

}
