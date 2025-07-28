using Godot;
using Godot.Collections;
using System;
using System.IO;
using System.Linq;

public partial class HexMap3d : Node3D
{
	[Export] int height = 10;
	[Export] int width = 10;
	private int _targetHeight = 0;
	private Control _ui;
	MeshGenerator meshGenerator;

	Tile[] tiles;
	Material material; //ВРЕМЕННО
	Material material1;
	Material material2;
	public override void _Ready()
	{
		material = GD.Load<Material>("res://resources/tile_color.tres"); //ВРЕМЕННО
		material1 = GD.Load<Material>("res://resources/neighbor_color.tres");
		material2 = GD.Load<Material>("res://resources/selected_color.tres");


		//Обработка UI
		_ui = GetNode<Control>("MapEditorUI");
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

		Random random = new();

		tiles = new Tile[height * width];
		for (int y = 0; y < height; y++)//ВИТЯ, ТЫ ИДИОТ, X И Y ПЕРЕПУТАНЫ МЕСТАМИ, ПЕРЕПИСЫВАЙ СИСТЕМУ КООРДИНАТ НАХРЕН
		{
			for (int x = 0; x < width; x++)
			{
				Tile tile = new(x, y, (int)random.NextInt64(0,16), "flat"); //random.NextSingle() > 0.5 ? "flat" : "bushes"
				if (x > 0)
				{
					// GD.Print(tiles[y * height + x - 1]);
					// GD.Print(y * height + x - 1);
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
	Vector2? oldCoords = null;

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		var spaceState = GetWorld3D().DirectSpaceState;
		Camera3D cam = GetNode<Camera3D>("Camera3D");
		Godot.Vector2 mousePosition = GetViewport().GetMousePosition();
		var rayOrigin = cam.ProjectRayOrigin(mousePosition);
		var rayEnd = rayOrigin + cam.ProjectRayNormal(mousePosition) * 500;
		PhysicsRayQueryParameters3D parameters = new();
		parameters.From = rayOrigin;
		parameters.To = rayEnd;
		var intersection = spaceState.IntersectRay(parameters);

		if (intersection.Count > 0)
		{
			Vector2 coords = HexMetrics.FromCoords(intersection["position"].As<Vector3>());
			// meshGenerator.ColorHex(coords, material1); //Изменение материала
			// // GD.Print(coords);
			// if ((oldCoords != null) && (oldCoords != coords))
			// {
			// 	meshGenerator.ColorHex((Vector2)oldCoords, material);
			// }
			// oldCoords = coords;


			if (@event.IsActionPressed("ui_mouse_click"))
			{
				Vector2 offsetCoords = HexMetrics.AxialToOffset(coords);
				GD.Print(tiles[(int)offsetCoords.Y*10 + (int)offsetCoords.X].GetNeighbor(HexDirection.NW).GetHexCoords()); //КОСТЫЛЬНЫЕ КООРДИНАТЫ
			}
		}
	}
}
