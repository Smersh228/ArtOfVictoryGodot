using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public partial class HexMap3d : Node3D
{
	[Export] int height = 10;
	[Export] int width = 10;
	private int _targetHeight = 10;
	private bool _tileHasRiver;
	private bool _changeTileHeight;

	private Control _ui;
	Camera3D cam;
	PhysicsDirectSpaceState3D spaceState;
	MeshGenerator meshGenerator;

	Tile[,] tiles;
	Tile activeTile;
	private string _targetType;
	Decal highlighterDecal, activeHighlighterDecal;
	ObjectPool<Decal> decalPool;
	Node decalHolder;
	PackedScene baseUnitScene;
	Unit testUnit, testUnit2; //ТЕСТОВЫЙ ЮНИТ
	public override void _Ready()
	{
		cam = GetNode<Camera3D>("Camera3D");
		spaceState = GetWorld3D().DirectSpaceState;


		highlighterDecal = GetNode<Decal>("Map/HighlighterDecal");
		highlighterDecal.Visible = false;
		activeHighlighterDecal = GetNode<Decal>("Map/RedHighlighterDecal");
		activeHighlighterDecal.Visible = false;
		decalPool = new(GD.Load<PackedScene>("res://scenes/highlighter_decal.tscn"));
		decalHolder = GetNode<Node>("DecalHolder");

		//Обработка UI
		_ui = GetNode<Control>("MapEditorUI");
		_ui.GetNode<Label>("Label").Text = _targetHeight.ToString();
		_ui.GetNode<Button>("HeightUpButton").Pressed += () =>
		{
			if (_targetHeight < 15)
			{
				_targetHeight++;
			}
			_ui.GetNode<Label>("Label").Text = _targetHeight.ToString();
			GD.Print(_targetHeight);
		};
		_ui.GetNode<Button>("HeightDownButton").Pressed += () =>
		{
			if (_targetHeight > 0)
			{
				_targetHeight--;
			}
			_ui.GetNode<Label>("Label").Text = _targetHeight.ToString();
			GD.Print(_targetHeight);
		};
		_ui.GetNode<Button>("HasRiverButton").Toggled += (hasRiver) =>
		{
			_tileHasRiver = hasRiver;
		};
		_ui.GetNode<Button>("ChangeHeightButton").Toggled += (changeHeight) =>
		{
			_changeTileHeight = changeHeight;
		};

		//Селектор юнитов
		selector = GD.Load<PackedScene>("res://scenes/unit_selector.tscn").Instantiate<UnitSelector>();
		selector.Visible = false;
		selector.ProcessMode = ProcessModeEnum.Disabled;
		AddChild(selector);



		Random random = new();  // Тестовая генерация карты. РЕАЛИЗОВАТЬ ЗАГРУЗКУ ИЗ ФАЙЛА
		tiles = new Tile[width, height];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				Tile tile = new(x, y, (int)random.NextInt64(0, 16), random.NextSingle() > 0.5 ? "flat" : "bushes");
				if (x > 0) //Назначение соседей
				{
					tile.SetNeighbor(HexDirection.W, tiles[x - 1, y]); //Сосед слева
				}
				if (y > 0)
				{
					if (y % 2 == 0) //чётная строка
					{
						tile.SetNeighbor(HexDirection.NE, tiles[x, y - 1]);
						if (x > 0)
						{
							tile.SetNeighbor(HexDirection.NW, tiles[x - 1, y - 1]);
						}
					}
					else //нечётная строка
					{
						tile.SetNeighbor(HexDirection.NW, tiles[x, y - 1]);
						if ((x + 1) < width)
						{
							tile.SetNeighbor(HexDirection.NE, tiles[x + 1, y - 1]);
						}
					}
				}
				tiles[x, y] = tile;
			}
		}
		meshGenerator = GetNode<MeshGenerator>("Map/MeshGenerator");
		meshGenerator.RegenerateMesh(tiles);

		baseUnitScene = GD.Load<PackedScene>("res://scenes/unit.tscn");

		testUnit = InstantiateUnit("po2", tiles[1, 1]);
		testUnit2 = InstantiateUnit("po2", tiles[1, 1]);
		MoveUnit(testUnit2, new Vector2I(2, 2));
		MoveUnit(testUnit, new Vector2I(2, 2));
	}
	Tile hoverTile = null, oldHoverTile;
	UnitSelector selector;
	public override void _Input(InputEvent @event)
	{
		Vector2 mousePosition = GetViewport().GetMousePosition(); //Рэйкаст
		var rayOrigin = cam.ProjectRayOrigin(mousePosition);
		var rayEnd = rayOrigin + cam.ProjectRayNormal(mousePosition) * 100;
		PhysicsRayQueryParameters3D parameters = new();
		parameters.From = rayOrigin;
		parameters.To = rayEnd;
		var intersection = spaceState.IntersectRay(parameters);

		if (intersection.Count == 0) //Если смотрим в пустоту
		{
			highlighterDecal.Visible = false;
			return;
		}

		Vector3I coords = HexMetrics.AxialFromWorldCoords(intersection["position"].As<Vector3>()); //Получение координат гекса из коорд пересечения рэйкаста

		oldHoverTile = hoverTile;
		hoverTile = GetAxialTile(HexMetrics.AxialToOffset(coords));

		// Подсветка тайла под курсором
		if (hoverTile == activeTile)
		{
			highlighterDecal.Visible = false;
		}
		else
		{
			highlighterDecal.Visible = true;
			highlighterDecal.Position = hoverTile.GetWorldPosition();
		}
		/*  // Эксперименты с соседями и пулом декалей
				if (oldHoverTile != hoverTile)
				{
					foreach (Decal d in decalHolder.GetChildren())
					{
						decalPool.Add(d);
					}
				}

				foreach (Vector3I vector in GetTileCoordsInRadius(coords, 1))
				{
					if (oldHoverTile == hoverTile)
					{
						break;
					}

					Tile tile = GetAxialTile(HexMetrics.AxialToOffset(vector));
					if (tile == null)
					{
						continue;
					}

					Decal decal = decalPool.Pull();
					GD.Print(decal);
					decal.Position = tile.GetWorldPosition();
					if (decal.GetParent() == null)
					{
						decalHolder.AddChild(decal);
					}
				}
				*/
		if (@event.IsActionPressed("left_mouse_click"))
		{
			if (activeTile == hoverTile) //Если кликаем по активному тайлу, снимаем выделениеы
			{
				activeTile = null;
				activeHighlighterDecal.Visible = false;
			}
			else //Если выбираем новый тайл
			{
				activeTile = hoverTile;
				activeHighlighterDecal.Position = activeTile.GetWorldPosition();
				activeHighlighterDecal.Visible = true;
			}
			if (_changeTileHeight)
			{
				hoverTile.Height = _targetHeight;
			}
			if (_targetType != null)
			{
				hoverTile.Type = _targetType;
			}
			// hoverTile.HasRiver = _tileHasRiver; //Нужно придумать, как реализовать это в интерфейсе, но пока в теории должно работать
			// hoverTile.SetBorderRiverState(HexDirection.NE, HexBorderRiverState.OUT); //РИСУЕТ ДВОЙНУЮ ДЕКАЛЬ. ПЕРЕПИСАТЬ КОД ДЕКАЛЕЙ

			// hoverTile.HasCity = !hoverTile.HasCity; // ТЕСТ
			meshGenerator.RegenerateMesh(tiles); //ОПТИМИЗИРОВАТЬ

			if (activeTile.UnitCount > 1) // Призыв селектора юнитов
			{
				selector.ShowSelector(hoverTile);
			}
			else
			{
				selector.HideSelector();
			}
		}
	}

	public override void _Process(double delta)
	{
		if (activeTile is not null)
		{
			selector.Position = cam.UnprojectPosition(activeTile.GetWorldPosition());
		}
	}

	public void ChangeColor(string type)
	{
		_targetType = type;
	}

	public Vector3I[] GetTileCoordsInRadius(Vector3I center, int radius)
	{
		List<Vector3I> tileCoords = new();

		for (int q = -radius; q <= radius; q++)
		{
			for (int r = Math.Max(-radius, -q - radius); r <= Math.Min(radius, -q + radius); r++)
			{
				tileCoords.Add(new Vector3I(q, r, -q - r) + center);
				GD.Print(new Vector3I(q, r, -q - r) + center);
			}
		}
		return tileCoords.ToArray<Vector3I>();
	}

#nullable enable
	public Tile? GetAxialTile(Vector2I vector)
	{
		if (vector.X < 0 || vector.X >= width)
		{
			return null;
		}
		if (vector.Y < 0 || vector.Y >= height)
		{
			return null;
		}
		return tiles[vector.X, vector.Y];
	}
	public Tile? GetAxialTile(Vector3I vector)
	{
		if (vector.X < 0 || vector.X >= width)
		{
			return null;
		}
		if (vector.Y < 0 || vector.Y >= height)
		{
			return null;
		}
		Vector2I offsetCoords = HexMetrics.AxialToOffset(vector);
		return tiles[offsetCoords.X, offsetCoords.Y];
	}
#nullable disable

	public void MoveUnit(Unit unit, Vector2I coords)
	{
		tiles[unit.Coordinates.X, unit.Coordinates.Y].RemoveUnit(unit);
		Tile tile = tiles[coords.X, coords.Y];
		unit.Position = tile.GetWorldPosition();
		unit.Coordinates = new(coords.X, coords.Y);
		tile.AddUnit(unit);
	}
	public void MoveUnit(Unit unit, Tile tile)
	{
		tiles[unit.Coordinates.X, unit.Coordinates.Y].RemoveUnit(unit);
		unit.Position = tile.GetWorldPosition();
		unit.Coordinates = tile.Coordinates;
		tile.AddUnit(unit);
	}
	/// <summary>
	/// Добавляет юнита на карту
	/// </summary>
	/// <param name="unit"></param>
	/// <param name="tile"></param>
	public void PlaceUnit(Unit unit, Tile tile)
	{
		unit.Position = tile.GetWorldPosition();
		unit.Coordinates = tile.Coordinates;
		tile.AddUnit(unit);
	}
	/// <summary>
	/// Создаёт экземпляр юнита и размещает его на указанном тайле
	/// </summary>
	/// <param name="id">Текстовый идентификатор юнита (po2, pz2 ...)</param>
	/// <param name="tile"></param>
	/// <returns></returns>
	public Unit InstantiateUnit(string id, Tile tile)
	{
		Unit unit = baseUnitScene.Instantiate<Unit>();
		unit.Init(id);
		PlaceUnit(unit, tile);
		AddChild(unit);
		return unit;
	}
}
