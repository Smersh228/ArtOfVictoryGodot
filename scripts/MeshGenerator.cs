using Godot;
using System;
using System.Collections.Generic;

public partial class MeshGenerator : MeshInstance3D
{
	Godot.Collections.Array surfaceArray = [];
	ArrayMesh arrMesh;
	CollisionShape3D collision;
	float solidRadius = 0.8f;
	float waterRadius = 0.5f;
	float waterBottomRadius = 0.3f;
	float waterDepth = 0.2f;
	Dictionary<string, Color> colors = new() { };
	SurfaceTool st;
	List<string> tileTypes;
	public override void _Ready()
	{
		base._Ready();
		foreach (var tile in HexTileData.GlobalTileData)
		{
			colors.Add(tile.Key, new(HexTileData.GlobalTileData[tile.Key]["color"].ToString()));
		}
	}

	private void AddTriangle(Vector3 v1, Color c1, Vector3 v2, Color c2, Vector3 v3, Color c3) //Возможна оптимизация, но пошла она нафиг, я устал, а переполнения всё равно не будет
	{

		st.SetColor(c1);
		st.SetNormal(new Vector3(0, 0, 1));
		st.SetUV(new Vector2(0, 0));
		st.AddVertex(v1);
		st.SetColor(c2);
		st.SetNormal(new Vector3(0, 0, 1));
		st.SetUV(new Vector2(0, 1));
		st.AddVertex(v2);
		st.SetColor(c3);
		st.SetNormal(new Vector3(0, 0, 1));
		st.SetUV(new Vector2(1, 1));
		st.AddVertex(v3);
	}

	public void AddHex(Vector3 center, Color color)
	{
		float radius = solidRadius;
		for (int i = 0; i < 6; i++)
		{
			AddTriangle(center, color, center + HexMetrics.Corners[i] * radius, color, center + HexMetrics.Corners[i + 1] * radius, color);
		}
	}
	public void AddHex(Vector3 center, Color color, float radius)
	{
		for (int i = 0; i < 6; i++)
		{
			AddTriangle(center, color, center + HexMetrics.Corners[i] * radius, color, center + HexMetrics.Corners[i + 1] * radius, color);
		}
	}
	public void AddRectangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color eastColor, Color westColor)
	{
		AddTriangle(v1, eastColor, v2, westColor, v3, westColor);
		AddTriangle(v1, eastColor, v3, westColor, v4, eastColor);
	}
	public void AddRectangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color c1, Color c2, Color c3, Color c4)
	{
		AddTriangle(v1, c1, v2, c2, v3, c3);
		AddTriangle(v1, c1, v3, c3, v4, c4);
	}

	public void RegenerateMesh(Tile[,] tiles)
	{
		GD.Print("Start generation");
		GD.Print(Time.GetTicksMsec());

		st = new();
		st.Begin(Mesh.PrimitiveType.Triangles);
		st.SetSmoothGroup(UInt32.MaxValue);

		arrMesh = Mesh as ArrayMesh;
		arrMesh.ClearSurfaces();
		foreach (Node child in GetChildren())
		{
			child.QueueFree();
		}

		surfaceArray.Resize((int)Mesh.ArrayType.Max);

		collision = GetParent().GetNode<CollisionShape3D>("CollisionShape3D");

		tileTypes = new();


		foreach (Tile tile in tiles)
		{
			GenerateFlatHex(tile);
			if (tile.HasRiver)
			{
				for (int i = 0; i < 6; i++)
				{
					if (tile.GetNeighbor((HexDirection)i) == null)
					{
						continue;
					}
					if (!tile.GetNeighbor((HexDirection)i).HasRiver)
					{
						continue;
					}
					if (tile.Rivers[i] == HexBorderRiverState.NO)
					{
						continue;
					}
					var riverDecal = new Decal();
					riverDecal.TextureAlbedo = GD.Load<Texture2D>("res://resources/texture_river.jpg");
					riverDecal.Position = tile.GetWorldPosition() + HexMetrics.RotateVector(HexMetrics.Corners[i], 30) * 0.5f;
					riverDecal.Scale = new(0.2f, 1f, 0.5f);
					riverDecal.Rotate(Vector3.Down, Mathf.DegToRad(i * 60 + 30));
					AddChild(riverDecal);
				}
			}
			if (tile.HasCity) 
			{
				Node3D city = ResourceLoader.Load<PackedScene>("res://models/Town.glb").Instantiate<Node3D>();
				city.Position = tile.GetWorldPosition();
				AddChild(city);
			}
		}

		st.GenerateNormals();
		st.GenerateTangents();

		st.Commit(arrMesh);

		collision.Shape = Mesh.CreateTrimeshShape();

		GD.Print(Time.GetTicksMsec());
		GD.Print("Finish generation");
	}

	public void GenerateFlatHex(Tile tile)
	{
		Vector3 tilePosition = tile.GetWorldPosition();
		if (!tileTypes.Contains(tile.Type)) tileTypes.Add(tile.Type);

		AddHex(tilePosition, colors[tile.Type]); // основная поверхность

		for (int i = 0; i < 3; i++) // четырёхугольники к восточным соседям
		{
			// if ((tile.GetNeighbor((HexDirection)i) == null) || tile.GetNeighbor((HexDirection)i).HasRiver)
			if (tile.GetNeighbor((HexDirection)i) == null)
			{
				continue;
			}

			AddRectangle(
				tilePosition + HexMetrics.Corners[i] * solidRadius,
				tile.GetNeighbor((HexDirection)i).GetWorldPosition() + HexMetrics.Corners[(i + 4) % 6] * solidRadius,
				tile.GetNeighbor((HexDirection)i).GetWorldPosition() + HexMetrics.Corners[i + 3] * solidRadius,
				tilePosition + HexMetrics.Corners[i + 1] * solidRadius,
				colors[tile.Type],
				colors[tile.GetNeighbor((HexDirection)i).Type]
			);

		}

		for (int i = 0; i < 2; i++) // треугольники к восточным соседям
		{
			if ((tile.GetNeighbor((HexDirection)i) == null) || (tile.GetNeighbor((HexDirection)i + 1) == null))
			{
				continue;
			}
			AddTriangle(
				tilePosition + HexMetrics.Corners[i + 1] * solidRadius,
				colors[tile.Type],
				tile.GetNeighbor((HexDirection)i).GetWorldPosition() + HexMetrics.Corners[i + 3] * solidRadius,
				colors[tile.GetNeighbor((HexDirection)i).Type],
				tile.GetNeighbor((HexDirection)i + 1).GetWorldPosition() + HexMetrics.Corners[(i + 5) % 6] * solidRadius,
				colors[tile.GetNeighbor((HexDirection)i + 1).Type]
			);
		}
	}

	/* // ГЕНЕРАЦИЯ ДЕФОРМИРОВАННОГО ГЕКСА. НЕ ИСПОЛЬЗУЕТСЯ. РЕКИ - ДЕКАЛИ
	public void GenerateRiverHex(Tile tile)
	{
		Vector3 tilePosition = tile.GetWorldPosition();
		if (!tileTypes.Contains(tile.Type)) tileTypes.Add(tile.Type);

		for (int i = 0; i < 6; i++) // i = HexDirection // i = HexMetrics.Corners
		{
			if (tile.GetNeighbor((HexDirection)i) == null)
			{
				continue;
			}

			float rotation = Mathf.DegToRad(i * 60);

			if (!tile.GetNeighbor((HexDirection)i).HasRiver) //Если нет реки, обычный четырёхугольник 
			{
				AddRectangle(   //СКАТ / SLOPE
					tilePosition + HexMetrics.Corners[i] * solidRadius,
					tile.GetNeighbor((HexDirection)i).GetWorldPosition() + HexMetrics.Corners[(i + 4) % 6] * solidRadius,
					tile.GetNeighbor((HexDirection)i).GetWorldPosition() + HexMetrics.Corners[(i + 3) % 6] * solidRadius,
					tilePosition + HexMetrics.Corners[(i + 1) % 6] * solidRadius,
					colors[tile.Type],
					colors[tile.GetNeighbor((HexDirection)i).Type]
				);

				AddRectangle(   //ЗЕМЛЯ / SOLID
					tilePosition + HexMetrics.Corners[i] * waterRadius,
					tilePosition + HexMetrics.Corners[i] * solidRadius,
					tilePosition + HexMetrics.Corners[(i + 1) % 6] * solidRadius,
					tilePosition + HexMetrics.Corners[(i + 1) % 6] * waterRadius,
					colors["sand"],
					colors[tile.Type]
				);

				AddRectangle(   //БЕРЕГ / BANK
					tilePosition + HexMetrics.Corners[i] * waterBottomRadius + Vector3.Down * waterDepth,
					tilePosition + HexMetrics.Corners[i] * waterRadius,
					tilePosition + HexMetrics.Corners[(i + 1) % 6] * waterRadius,
					tilePosition + HexMetrics.Corners[(i + 1) % 6] * waterBottomRadius + Vector3.Down * waterDepth,
					colors["riverBottom"],
					colors["sand"]
				);

			}
			else //Если сосед - РЕКА
			{
				AddTriangle(    //ЗЕМЛЯ / SOLID	//Мб добавить перегрузку с одним цветом?
					tilePosition + HexMetrics.Corners[i] * waterRadius,
					colors["sand"],
					tilePosition + HexMetrics.Corners[i] * solidRadius,
					colors[tile.Type],
					tilePosition + HexMetrics.Intersect(solidRadius, waterRadius, true, i * 60),
					colors["sand"]
				);
				AddTriangle(    //ЗЕМЛЯ / SOLID	//Мб добавить перегрузку с одним цветом?
					tilePosition + HexMetrics.Intersect(solidRadius, waterRadius, false, i * 60),
					colors["sand"],
					tilePosition + HexMetrics.Corners[i + 1] * solidRadius,
					colors[tile.Type],
					tilePosition + HexMetrics.Corners[i + 1] * waterRadius,
					colors["sand"]
				);
				AddRectangle(   //ЗЕМЛЯ / SOLID
					tile.GetNeighbor((HexDirection)i).GetWorldPosition() + HexMetrics.Corners[(i + 4) % 6] * solidRadius,
					tile.GetNeighbor((HexDirection)i).GetWorldPosition() + HexMetrics.Intersect(solidRadius, waterRadius, false, i * 60 + 180),
					tilePosition + HexMetrics.Intersect(solidRadius, waterRadius, true, i * 60),
					tilePosition + HexMetrics.Corners[i] * solidRadius,
					colors[tile.GetNeighbor((HexDirection)i).Type],
					colors["sand"],
					colors["sand"],
					colors[tile.Type]
				);

				AddRectangle(   //БЕРЕГ / BANK
					tilePosition + HexMetrics.Intersect(solidRadius, waterRadius, true, i * 60),
					tilePosition + HexMetrics.Intersect(solidRadius, waterBottomRadius, true, i * 60) + Vector3.Down * waterDepth,
					tilePosition + HexMetrics.Corners[i] * waterBottomRadius + Vector3.Down * waterDepth,
					tilePosition + HexMetrics.Corners[i] * waterRadius,
					colors["sand"],
					colors["riverBottom"]
				);
				AddRectangle(   //БЕРЕГ / BANK
					tilePosition + HexMetrics.Intersect(solidRadius, waterBottomRadius, false, i * 60) + Vector3.Down * waterDepth,
					tilePosition + HexMetrics.Intersect(solidRadius, waterRadius, false, i * 60),
					tilePosition + HexMetrics.Corners[i + 1] * waterRadius,
					tilePosition + HexMetrics.Corners[i + 1] * waterBottomRadius + Vector3.Down * waterDepth,
					colors["riverBottom"],
					colors["sand"]

				);
				AddRectangle(   //БЕРЕГ / BANK
					tile.GetNeighbor((HexDirection)i).GetWorldPosition() + HexMetrics.Intersect(solidRadius, waterRadius, false, i * 60 + 180),
					tile.GetNeighbor((HexDirection)i).GetWorldPosition() + HexMetrics.Intersect(solidRadius, waterBottomRadius, false, i * 60 + 180) + Vector3.Down * waterDepth,
					tilePosition + HexMetrics.Intersect(solidRadius, waterBottomRadius, true, i * 60) + Vector3.Down * waterDepth,
					tilePosition + HexMetrics.Intersect(solidRadius, waterRadius, true, i * 60),
					colors["sand"],
					colors["riverBottom"]
				);



				AddRectangle(   //ДНО / BOTTOM
					tilePosition + HexMetrics.Corners[i] * waterBottomRadius + Vector3.Down * waterDepth,
					tilePosition + HexMetrics.Intersect(solidRadius, waterBottomRadius, true, i * 60) + Vector3.Down * waterDepth,
					tilePosition + HexMetrics.Intersect(solidRadius, waterBottomRadius, false, i * 60) + Vector3.Down * waterDepth,
					tilePosition + HexMetrics.Corners[i + 1] * waterBottomRadius + Vector3.Down * waterDepth,
					colors["riverBottom"],
					colors["riverBottom"]
				);
				AddRectangle(   //ДНО / BOTTOM // ОТРИСОВЫВАЕТСЯ ДВАЖДЫ В ОДНОЙ ПОЗИЦИИ
					tilePosition + HexMetrics.Intersect(solidRadius, waterBottomRadius, true, i * 60) + Vector3.Down * waterDepth,
					tile.GetNeighbor((HexDirection)i).GetWorldPosition() + HexMetrics.Intersect(solidRadius, waterBottomRadius, false, i * 60 + 180) + Vector3.Down * waterDepth,
					tile.GetNeighbor((HexDirection)i).GetWorldPosition() + HexMetrics.Intersect(solidRadius, waterBottomRadius, true, i * 60 + 180) + Vector3.Down * waterDepth,
					tilePosition + HexMetrics.Intersect(solidRadius, waterBottomRadius, false, i * 60) + Vector3.Down * waterDepth,
					colors["riverBottom"],
					colors["riverBottom"]
				);

			}

			AddHex(tilePosition + Vector3.Down * waterDepth, colors["riverBottom"], waterBottomRadius); //Дно
		}

		//ДУБЛИРУЮЩИЙСЯ КОД С GenerateFlatHex(Tile tile)
		for (int i = 0; i < 2; i++) // треугольники к восточным соседям
		{
			if ((tile.GetNeighbor((HexDirection)i) == null) || (tile.GetNeighbor((HexDirection)i + 1) == null))
			{
				continue;
			}
			AddTriangle(
				tilePosition + HexMetrics.Corners[i + 1] * solidRadius,
				colors[tile.Type],
				tile.GetNeighbor((HexDirection)i).GetWorldPosition() + HexMetrics.Corners[i + 3] * solidRadius,
				colors[tile.GetNeighbor((HexDirection)i).Type],
				tile.GetNeighbor((HexDirection)i + 1).GetWorldPosition() + HexMetrics.Corners[(i + 5) % 6] * solidRadius,
				colors[tile.GetNeighbor((HexDirection)i + 1).Type]
			);
		}
	}
	*/
}
