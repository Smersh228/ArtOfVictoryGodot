using Godot;
using System.Collections.Generic;

public class MeshData
{
	public List<Vector3> Vertices = [];
	public List<Vector3> Normals = [];
	public List<int> Indices = [];
	// public List<Vector2> UVs = [];

}

public partial class MeshGenerator : MeshInstance3D
{
	List<Vector3> vertices = [];
	List<Vector3> normals = [];
	List<int> indices = [];
	Godot.Collections.Array surfaceArray = [];
	Dictionary<string, MeshData> meshData = new();
	ArrayMesh arrMesh;
	Material material;//ВРЕМЕННО
	Material material1;
	Material material2;
	CollisionShape3D collision;
	float solidRadius = 0.75f;
	public override void _Ready()
	{
		base._Ready();
	}

	private void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, string materialType)
	{
		if (!meshData.ContainsKey(materialType))
		{
			meshData.Add(materialType, new());
		}

		int vertexIndex = meshData[materialType].Vertices.Count;

		meshData[materialType].Vertices.Add(v1);
		meshData[materialType].Vertices.Add(v2);
		meshData[materialType].Vertices.Add(v3);

		meshData[materialType].Indices.Add(vertexIndex);
		meshData[materialType].Indices.Add(vertexIndex + 1);
		meshData[materialType].Indices.Add(vertexIndex + 2);

		meshData[materialType].Normals.AddRange([v1.Normalized(), v2.Normalized(), v3.Normalized()]);
		// meshData[materialType].Normals.AddRange([Vector3.Up, Vector3.Up, Vector3.Up]);

		// meshData[materialType].UVs.Add(new Vector2(0, 0));
		// meshData[materialType].UVs.Add(new Vector2(0, 0));
		// meshData[materialType].UVs.Add(new Vector2(0, 0));


		// vertices.Add(v1);
		// vertices.Add(v2);
		// vertices.Add(v3);
		// indices.Add(vertexIndex);
		// indices.Add(vertexIndex + 1);
		// indices.Add(vertexIndex + 2);
		// normals.AddRange([Vector3.Up, Vector3.Up, Vector3.Up]);
	}

	public void AddHex(Vector3 center, string materialType)
	{
		for (int i = 0; i < 6; i++)
		{
			AddTriangle(center, center + HexMetrics.Corners[i] * solidRadius, center + HexMetrics.Corners[i + 1] * solidRadius, materialType);
		}

		// surfaceArrays[materialType][(int)Mesh.ArrayType.Vertex] = vertices.ToArray();
		// surfaceArrays[materialType][(int)Mesh.ArrayType.Normal] = normals.ToArray();
		// surfaceArrays[materialType][(int)Mesh.ArrayType.Index] = indices.ToArray();	
	}

	public void AddRectangle(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, string materialType)
	{
		// vertices = [];
		// normals = [];
		// indices = [];
		
		AddTriangle(v1, v2, v3, materialType);
		AddTriangle(v1, v3, v4, materialType);



		// arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
	}


	public void RegenerateMesh(Tile[] tiles)
	{
		GD.Print("Start generation");
		GD.Print(Time.GetTicksMsec());

		arrMesh = Mesh as ArrayMesh;
		arrMesh.ClearSurfaces();

		material = GD.Load<Material>("res://resources/tile_color.tres");//ВРЕМЕННО
		material1 = GD.Load<Material>("res://resources/neighbor_color.tres");
		material2 = GD.Load<Material>("res://resources/selected_color.tres");

		surfaceArray.Resize((int)Mesh.ArrayType.Max);

		collision = GetParent().GetNode<CollisionShape3D>("CollisionShape3D");

		foreach (Tile tile in tiles)
		{
			Vector3 tilePosition = tile.GetWorldPosition();
			AddHex(tilePosition, tile.Type);
		}

		foreach (Tile tile in tiles)
		{
			Vector3 tilePosition = tile.GetWorldPosition();
			if (tile.GetNeighbor(HexDirection.NE) != null)
			{
				AddRectangle(
					tilePosition + HexMetrics.Corners[3] * solidRadius,
					tile.GetNeighbor(HexDirection.NE).GetWorldPosition() + HexMetrics.Corners[1] * solidRadius,
					tile.GetNeighbor(HexDirection.NE).GetWorldPosition() + HexMetrics.Corners[0] * solidRadius,
					tilePosition + HexMetrics.Corners[4] * solidRadius,
					"bushes"
					);
			}
			if (tile.GetNeighbor(HexDirection.E) != null)
			{
				AddRectangle(
					tilePosition + HexMetrics.Corners[4] * solidRadius,
					tile.GetNeighbor(HexDirection.E).GetWorldPosition() + HexMetrics.Corners[2] * solidRadius,
					tile.GetNeighbor(HexDirection.E).GetWorldPosition() + HexMetrics.Corners[1] * solidRadius,
					tilePosition + HexMetrics.Corners[5] * solidRadius,
					"bushes"
					);
			}
			if (tile.GetNeighbor(HexDirection.SE) != null)
			{
				AddRectangle(
					tilePosition + HexMetrics.Corners[5] * solidRadius,
					tile.GetNeighbor(HexDirection.SE).GetWorldPosition() + HexMetrics.Corners[3] * solidRadius,
					tile.GetNeighbor(HexDirection.SE).GetWorldPosition() + HexMetrics.Corners[2] * solidRadius,
					tilePosition + HexMetrics.Corners[6] * solidRadius,
					"bushes"
					);
			}
		}

		foreach (Tile tile in tiles)
		{
			Vector3 tilePosition = tile.GetWorldPosition();

			if ((tile.GetNeighbor(HexDirection.NE) != null) && (tile.GetNeighbor(HexDirection.E) != null))
			{
				AddTriangle(
					tilePosition + HexMetrics.Corners[4] * solidRadius,
					tile.GetNeighbor(HexDirection.NE).GetWorldPosition() + HexMetrics.Corners[0] * solidRadius,
					tile.GetNeighbor(HexDirection.E).GetWorldPosition() + HexMetrics.Corners[2] * solidRadius,
					"flat"
				);
			}
			if ((tile.GetNeighbor(HexDirection.SE) != null) && (tile.GetNeighbor(HexDirection.E) != null))
			{
				AddTriangle(
					tilePosition + HexMetrics.Corners[5] * solidRadius,
					tile.GetNeighbor(HexDirection.E).GetWorldPosition() + HexMetrics.Corners[1] * solidRadius,
					tile.GetNeighbor(HexDirection.SE).GetWorldPosition() + HexMetrics.Corners[3] * solidRadius,
					"swamp"
				);
			}
		}

		foreach (KeyValuePair<string, MeshData> i in meshData)
		{
			surfaceArray[(int)Mesh.ArrayType.Vertex] = i.Value.Vertices.ToArray();
			surfaceArray[(int)Mesh.ArrayType.Normal] = i.Value.Normals.ToArray();
			surfaceArray[(int)Mesh.ArrayType.Index] = i.Value.Indices.ToArray();
			// surfaceArray[(int)Mesh.ArrayType.TexUV] = i.Value.UVs.ToArray();
			arrMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, surfaceArray);
					
		}

		collision.Shape = Mesh.CreateConvexShape();

		arrMesh.SurfaceSetMaterial(0, material);
		arrMesh.SurfaceSetMaterial(1, material1);
		arrMesh.SurfaceSetMaterial(2, material2);

		// arrMesh.RegenNormalMaps();

		GD.Print(Time.GetTicksMsec());
		GD.Print("Finish generation");
	}

	public void ColorHex(Vector2 coords, Material material)
	{
		Vector2 offsetCoords = HexMetrics.AxialToOffset(coords);
		arrMesh.SurfaceSetMaterial((int)offsetCoords.Y*10 + (int)offsetCoords.X, material); //КОСТЫЛЬНЫЙ СПОСОБ ОПРЕДЕЛЕНИЯ КООРДИНАТ
	}
}
