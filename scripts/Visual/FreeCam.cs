using Godot;
using System;

public partial class FreeCam : Node3D
{
	Vector3 mVec = new();
	Camera3D cam;

	public FreeCam()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		cam = new Camera3D();
		AddChild(cam);
	}

	public StaticBody3D RayCast()
	{
		var mousePos = GetViewport().GetMousePosition();
		var origin = cam.ProjectRayOrigin(mousePos);
		var end = origin + cam.ProjectRayNormal(mousePos) * 255;
		var query = PhysicsRayQueryParameters3D.Create(origin, end, 1);
		var spaceState = GetWorld3D().DirectSpaceState;
		var intersection = spaceState.IntersectRay(query);
		intersection.TryGetValue("collider", out Variant unit);

		var squad = (StaticBody3D)(unit);
		if (squad is null) return null;

		return squad;
	}

	public Node3D RayCastSquad() => RayCast();

	public override void _Process(double delta)
	{
		Vector3 dir = Transform.Basis * mVec.Normalized() * (float)delta * 3f;
		Position += dir;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion move)
		{
			Camera3D cam = GetChild<Camera3D>(0);
			cam.RotateX(Mathf.DegToRad(-move.Relative.Y));
			this.RotateY(Mathf.DegToRad(-move.Relative.X));
		}

		if (@event is InputEventKey key)
		{
			switch (key.Keycode)
			{
				case Key.D:
					mVec.X = 1;
					return;
				case Key.A:
					mVec.X = -1f;
					return;
				case Key.Shift:
					mVec.Y = -1f;
					return;
				case Key.Space:
					mVec.Y = 1f;
					return;
				case Key.W:
					mVec.Z = -1f;
					return;
				case Key.S:
					mVec.Z = 1f;
					return;
				default:
					mVec = Vector3.Zero;
					return;
			}
		}
	}
}
