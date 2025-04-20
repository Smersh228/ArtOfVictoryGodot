using Godot;
using System;
using System.Numerics;

public partial class HexMap3d : Node3D
{
	Tile[,] m;
	Tile oldTileCollider = null;
	OmniLight3D light = new OmniLight3D();
	private PackedScene _tileScene = GD.Load<PackedScene>("res://Tile.tscn");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		m = new Tile[10,10];
		AddChild(light);

		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				m[i,j] = _tileScene.Instantiate<Tile>();
				m[i,j].Initialize(i, j);

				m[i,j].Position = new Godot.Vector3(Convert.ToSingle(Math.Sqrt(3)) / 2 * (i + j % 2f / 2), -i/20f - j/20f, j * 3f / 4f);
				AddChild(m[i,j]);
				GD.Print(m[i,j].Position);
				m[i,j].label.Text = m[i,j].GetHexCoords().ToString();
			}
			
			
		}
		m[5,5]._units[0] = new Unit();
		m[5,5].AddUnit();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	
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
			var tileCollider = (Tile)intersection["collider"];
			light.Position = tileCollider.Position + Godot.Vector3.Up;

			if (tileCollider != oldTileCollider && oldTileCollider != null)
			{
				oldTileCollider.GetNode<MeshInstance3D>("MeshInstance3D").MaterialOverride = GD.Load<Material>("TileColor.tres");
			}
			oldTileCollider = tileCollider;
			
			
			tileCollider.GetNode<MeshInstance3D>("MeshInstance3D").MaterialOverride = GD.Load<Material>("SelectedColor.tres");
			
			Godot.Vector2[] neighbors = tileCollider.GetNeighbors();
			// GD.Print("Neighbors: " + tileCollider.GetHexCoords());
			foreach (Tile tile in GetTree().GetNodesInGroup("highlighted_tiles"))
			{
				try
				{
					tile.meshInstance.MaterialOverlay = null;
					tile.RemoveFromGroup("highlighted_tiles");
				}
				catch{}
				
			}
			foreach (var neighbor in neighbors)
			{
				try
				{
					Tile tile = m[(int)neighbor.X, (int)neighbor.Y];
					tile.SetTemporaryOverlay(GD.Load<Material>("NeighborColor.tres"));
					tile.AddToGroup("highlighted_tiles");
				}
				catch{}
			}

			// Когда нужно сбросить
			


			// for (int i = 0; i < 6; i++)
			// {
			// 	m[(int)neighbors[i].X, (int)neighbors[i].Y].GetNode<MeshInstance3D>("MeshInstance3D").MaterialOverlay = GD.Load<Material>("NeighborColor.tres");
			// }

		}

	}
}
