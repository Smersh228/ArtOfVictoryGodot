using Godot;
using System;

public partial class Tile
{
	private int x, y;  //квадратные координаты
	public int X
	{
		get { return x; }
	}
	public int Y
	{
		get { return y; }
	}
	public Vector2I Coordinates
	{
		get
		{
			return new(X, Y);
		}
	}
	private int height;
	public int Height
	{
		get { return height; }
		set { if ((0 <= value) && (value <= 15)) height = value; }
	}
	private string tileType;
	public string Type
	{
		get { return tileType; }
		set { tileType = value; } //Добавить проверку на валидный тип
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
	public bool HasCity
	{
		get;
		set;
	}
	private HexBorderRiverState[] rivers = new HexBorderRiverState[6]; //Оптимизировать до 2 бит на сторону света
	public HexBorderRiverState[] Rivers
	{
		get
		{
			return rivers;
		}
	}
	private byte maxUnitCount = 3;
	private Unit[] units;
	public Unit[] Units
	{
		get
		{
			return units;
		}
	}
	public int UnitCount
	{
		get;
		private set;
	}
	private Tile[] _neighbors = new Tile[6];

	public Tile(int x, int y, int height, string tileType) // КОНСТРУКТОР ТУТ <---
	{
		this.x = x;
		this.y = y;
		Height = height;
		this.tileType = tileType;
		units = new Unit[maxUnitCount];
		UnitCount = 0;
	}

	public bool AddUnit(Unit unit)
	{
		for (int i = 0; i < maxUnitCount; i++)
		{
			if (units[i] == null)
			{
				units[i] = unit;
				UnitCount++;
				return true;
			}
		}
		return false;
	}
	public void RemoveUnit(Unit unit)
	{
		units[Array.IndexOf(units, unit)] = null;
		UnitCount--;
	}
	public void SetBorderRiverState(HexDirection direction, HexBorderRiverState state)
	{
		rivers[(int)direction] = state;
		GetNeighbor(direction).HasRiver = true;
		GetNeighbor(direction).rivers[(int)direction.Opposite()] = state.Opposite();
	}
	public Vector3 GetHexCoords()
	{
		return new Vector3(x - y / 2, y, -(x - y / 2) - y);
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
			new Vector2(x+1,y),
			new Vector2(x + y % 2,y+1),
			new Vector2(x-1+ y % 2,y+1),
			new Vector2(x-1,y),
			new Vector2(x + y % 2 - 1,y-1),
			new Vector2(x + 1 + y % 2 - 1,y-1)
		];
	}

	public Tile GetNeighbor(HexDirection direction)
	{
		return _neighbors[(int)direction];
	}
	public void SetNeighbor(HexDirection direction, Tile cell)
	{
		_neighbors[(int)direction] = cell;
		cell._neighbors[(int)direction.Opposite()] = this;
	}

}
