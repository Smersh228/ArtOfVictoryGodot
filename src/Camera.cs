using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Camera : Camera3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event.IsAction("ui_left"))
		{
			Position = Position.MoveToward(Vector3.Left * 1000, 0.1f);
		}
		if (@event.IsAction("ui_right"))
		{
			Position = Position.MoveToward(Vector3.Right * 1000, 0.1f);
		}
		if (@event.IsAction("ui_up"))
		{
			Position = Position.MoveToward(Vector3.Forward * 1000, 0.1f);
		}
		if (@event.IsAction("ui_down"))
		{
			Position = Position.MoveToward(Vector3.Back * 1000, 0.1f);
		}
	}
}
