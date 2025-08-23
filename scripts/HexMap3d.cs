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

	Tile[] tiles;
	string _targetType;
	Decal highlighterDecal;
	ObjectPool<Decal> decalPool;
	Node decalHolder;
	public override void _Ready()
	{
		cam = GetNode<Camera3D>("Camera3D");
		spaceState = GetWorld3D().DirectSpaceState;


		highlighterDecal = GetNode<Decal>("Map/HighlighterDecal");
		highlighterDecal.Visible = false;
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



		Random random = new();

		tiles = new Tile[height * width];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				Tile tile = new(x, y, (int)random.NextInt64(0, 16), random.NextSingle() > 0.5 ? "flat" : "bushes");
				if (x > 0)
				{
					tile.SetNeighbor(HexDirection.W, tiles[y * width + x - 1]); //Сосед слева
				}
				if (y > 0)
				{
					if (y % 2 == 0) //чётная строка
					{
						tile.SetNeighbor(HexDirection.NE, tiles[(y - 1) * width + x]);
						if (x > 0)
						{
							tile.SetNeighbor(HexDirection.NW, tiles[(y - 1) * width + x - 1]);
						}
					}
					else //нечётная строка
					{
						tile.SetNeighbor(HexDirection.NW, tiles[(y - 1) * width + x]);
						if ((x + 1) < width)
						{
							tile.SetNeighbor(HexDirection.NE, tiles[(y - 1) * width + x + 1]);
						}
					}
				}
				tiles[y * width + x] = tile;
			}

		}
		meshGenerator = GetNode<MeshGenerator>("Map/MeshGenerator");
		meshGenerator.RegenerateMesh(tiles);
	}
	Tile hoverTile = null, oldHoverTile;
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		Vector2 mousePosition = GetViewport().GetMousePosition();
		var rayOrigin = cam.ProjectRayOrigin(mousePosition);
		var rayEnd = rayOrigin + cam.ProjectRayNormal(mousePosition) * 500;
		PhysicsRayQueryParameters3D parameters = new();
		parameters.From = rayOrigin;
		parameters.To = rayEnd;
		var intersection = spaceState.IntersectRay(parameters);


		if (intersection.Count > 0)
		{
			Vector2I coords = HexMetrics.FromCoords(intersection["position"].As<Vector3>()); //Получение координат гекса из коорд пересечения рейкаста
			Vector2I offsetCoords = HexMetrics.AxialToOffset(coords);

			oldHoverTile = hoverTile;
			hoverTile = tiles[offsetCoords.Y * height + offsetCoords.X];

			highlighterDecal.Visible = true;
			highlighterDecal.Position = hoverTile.GetWorldPosition();


			if (oldHoverTile != hoverTile)
			{
				foreach (Decal d in decalHolder.GetChildren())
				{
					decalPool.Add(d);
				}
			}

			for (int i = 0; i < 6; i++) // Эксперименты с соседями и пулом декалей
			{
				if (oldHoverTile == hoverTile)
				{
					break;
				}
				if (hoverTile.GetNeighbor((HexDirection)i) == null)
				{
					continue;
				}


				Tile neighbor = hoverTile.GetNeighbor((HexDirection)i);
				Decal decal = decalPool.Pull();
				GD.Print(decal);
				decal.Position = neighbor.GetWorldPosition();
				if (decal.GetParent() == null)
				{
					decalHolder.AddChild(decal);
				}
			}

			if (@event.IsActionPressed("left_mouse_click"))
			{

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

				meshGenerator.RegenerateMesh(tiles);

			}

		}
		else
		{
			highlighterDecal.Visible = false;
		}
	}

	public void ChangeColor(string type)
	{
		_targetType = type;
	}

	
}
