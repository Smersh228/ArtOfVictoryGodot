using Godot;
using System;

public partial class Unit : Node3D
{
	/// <summary>
	/// Строковый идентификатор типа юнита
	/// </summary>
	public string Type
	{
		get;
		private set;
	}
	private MeshInstance3D meshInstance;
	private Vector2I coordinates;
	private UnitData globalUnitData;
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
		globalUnitData = UnitData.Instance;
		Type = type;
		meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
		
		Mesh modelMesh = GD.Load<ArrayMesh>(globalUnitData[type]["model"].ToString());
		meshInstance.Mesh = modelMesh;
	}
}
