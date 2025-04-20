using Godot;
using System;

public partial class Tile : StaticBody3D
{
	public MeshInstance3D meshInstance;
	public Unit[] _units;
	private Material _originalMaterial;
	private Material _originalOverlay;
	private PackedScene _tileScene;
	public void StoreOriginalMaterials()
	{
		_originalMaterial = meshInstance.MaterialOverride;
		_originalOverlay = meshInstance.MaterialOverlay;
	}
	
	public void RestoreMaterials()
	{
		meshInstance.MaterialOverride = _originalMaterial;
		meshInstance.MaterialOverlay = _originalOverlay;
	}

	public void SetTemporaryOverlay(Material tempMaterial)
	{
		meshInstance.MaterialOverlay = tempMaterial;
	}


	private int x, y;
	public Label3D label;
	public void Initialize(int x, int y)
	{
		this.x = x;
		this.y = y;
		_units = new Unit[3];
	}
	public Vector3 GetHexCoords()
	{
		return new Vector3(x - y / 2, y, - (x - y / 2) - y);
	}
	public Vector2[] GetNeighbors()
	{
		return new Vector2[] {
			new Vector2(x+1,y),
			new Vector2(x + y % 2,y+1),
			new Vector2(x-1+ y % 2,y+1),
			new Vector2(x-1,y),
			new Vector2(x + y % 2 - 1,y-1),
			new Vector2(x + 1 + y % 2 - 1,y-1)
		};
	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
		Mesh c = GD.Load<Mesh>("res://Tile.tres");
		meshInstance.Mesh = c;
		label = GetNode<Label3D>("CoordsLabel");
		_tileScene = GD.Load<PackedScene>("res://unit.tscn");
	}


	public void AddUnit()
	{

		for (int i = 0; i < 3; i ++)
		{
			if (_units[i] != null);
			{
			_units[i] = _tileScene.Instantiate<Unit>();
			AddChild(_units[i]);	
			}
			
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	
}
