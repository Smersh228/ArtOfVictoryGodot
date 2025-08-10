using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public class MeshData
{
	public List<Vector3> Vertices = [];
	public List<Vector3> Normals = [];
	public List<int> Indices = [];
	public List<Vector2> UVs = [];

}

public partial class MeshGenerator : MeshInstance3D
{
	Godot.Collections.Array surfaceArray = [];
	ArrayMesh arrMesh;
	CollisionShape3D collision;
	float solidRadius = 0.75f;
	Dictionary<string, Color> colors = new() {};
	SurfaceTool st;

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
		for (int i = 0; i < 6; i++)
		{
			AddTriangle(center, color, center + HexMetrics.Corners[i] * solidRadius, color, center + HexMetrics.Corners[i + 1] * solidRadius, color);
		}
	}

	public void AddRectangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color eastColor, Color westColor)
	{
		AddTriangle(v1, eastColor, v2, westColor, v3, westColor);
		AddTriangle(v1, eastColor, v3, westColor, v4, eastColor);
	}


	public void RegenerateMesh(Tile[] tiles)
	{
		GD.Print("Start generation");
		GD.Print(Time.GetTicksMsec());

		st = new();
		st.Begin(Mesh.PrimitiveType.Triangles);
		st.SetSmoothGroup(UInt32.MaxValue);



		arrMesh = Mesh as ArrayMesh;
		arrMesh.ClearSurfaces();

		surfaceArray.Resize((int)Mesh.ArrayType.Max);

		collision = GetParent().GetNode<CollisionShape3D>("CollisionShape3D");

		List<string> tileTypes = new();


		foreach (Tile tile in tiles)
		{
			Vector3 tilePosition = tile.GetWorldPosition();
			if (!tileTypes.Contains(tile.Type)) tileTypes.Add(tile.Type);

			AddHex(tilePosition, colors[tile.Type]); // основная поверхность

			if (tile.GetNeighbor(HexDirection.NE) != null) // скаты
			{
				AddRectangle(
					tilePosition + HexMetrics.Corners[3] * solidRadius,
					tile.GetNeighbor(HexDirection.NE).GetWorldPosition() + HexMetrics.Corners[1] * solidRadius,
					tile.GetNeighbor(HexDirection.NE).GetWorldPosition() + HexMetrics.Corners[0] * solidRadius,
					tilePosition + HexMetrics.Corners[4] * solidRadius,
					colors[tile.Type],
					colors[tile.GetNeighbor(HexDirection.NE).Type]
					);
			}
			if (tile.GetNeighbor(HexDirection.E) != null)
			{
				AddRectangle(
					tilePosition + HexMetrics.Corners[4] * solidRadius,
					tile.GetNeighbor(HexDirection.E).GetWorldPosition() + HexMetrics.Corners[2] * solidRadius,
					tile.GetNeighbor(HexDirection.E).GetWorldPosition() + HexMetrics.Corners[1] * solidRadius,
					tilePosition + HexMetrics.Corners[5] * solidRadius,
					colors[tile.Type],
					colors[tile.GetNeighbor(HexDirection.E).Type]
					);
			}
			if (tile.GetNeighbor(HexDirection.SE) != null)
			{
				AddRectangle(
					tilePosition + HexMetrics.Corners[5] * solidRadius,
					tile.GetNeighbor(HexDirection.SE).GetWorldPosition() + HexMetrics.Corners[3] * solidRadius,
					tile.GetNeighbor(HexDirection.SE).GetWorldPosition() + HexMetrics.Corners[2] * solidRadius,
					tilePosition + HexMetrics.Corners[6] * solidRadius,
					colors[tile.Type],
					colors[tile.GetNeighbor(HexDirection.SE).Type]
					);
			}

			if ((tile.GetNeighbor(HexDirection.NE) != null) && (tile.GetNeighbor(HexDirection.E) != null)) // треугольники
			{
				AddTriangle(
					tilePosition + HexMetrics.Corners[4] * solidRadius,
					colors[tile.Type],
					tile.GetNeighbor(HexDirection.NE).GetWorldPosition() + HexMetrics.Corners[0] * solidRadius,
					colors[tile.GetNeighbor(HexDirection.NE).Type],
					tile.GetNeighbor(HexDirection.E).GetWorldPosition() + HexMetrics.Corners[2] * solidRadius,
					colors[tile.GetNeighbor(HexDirection.E).Type]
				);
			}
			if ((tile.GetNeighbor(HexDirection.SE) != null) && (tile.GetNeighbor(HexDirection.E) != null))
			{
				AddTriangle(
					tilePosition + HexMetrics.Corners[5] * solidRadius,
					colors[tile.Type],
					tile.GetNeighbor(HexDirection.E).GetWorldPosition() + HexMetrics.Corners[1] * solidRadius,
					colors[tile.GetNeighbor(HexDirection.E).Type],
					tile.GetNeighbor(HexDirection.SE).GetWorldPosition() + HexMetrics.Corners[3] * solidRadius,
					colors[tile.GetNeighbor(HexDirection.SE).Type]
				);
			}

		}

		st.GenerateNormals();
		st.GenerateTangents();

		st.Commit(arrMesh);
		

		collision.Shape = Mesh.CreateTrimeshShape();

		GD.Print(Time.GetTicksMsec());
		GD.Print("Finish generation");
	}

	public void ColorHex(Vector2 coords, Material material)
	{
		
	}
}
