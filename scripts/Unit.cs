using Godot;
using System;

public partial class Unit : Node3D
{
	public string Type
	{
		get;
		private set;
	}
	private MeshInstance3D meshInstance;
	private Vector2I coordinates;
	public Vector2I Coordinates
	{
		get
		{
			return coordinates;
		}
		set
		{
			coordinates = value; //Проверку на сеттер?
		}
	}
	public override void _Ready()
	{

	}
	public void Init(string type)
	{
		Type = type;
		meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
		
		Mesh modelMesh = GD.Load<ArrayMesh>(UnitData.GlobalUnitData[type]["model"].ToString());
		meshInstance.Mesh = modelMesh;
	}
}
