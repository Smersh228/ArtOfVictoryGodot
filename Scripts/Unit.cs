using Godot;
using System;

public partial class Unit : Node3D
{
		private MeshInstance3D _meshInstance;

		public override void _Ready()
		{

		}
		public void Init(string modelPath)
		{
			_meshInstance = GetNode<MeshInstance3D>("MeshInstance3D");
			Mesh modelMesh = GD.Load<ArrayMesh>(modelPath);
			_meshInstance.Mesh = modelMesh;
		}
		
}
