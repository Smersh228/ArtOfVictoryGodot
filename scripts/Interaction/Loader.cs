using System;
using Godot;

namespace Interaction;

public static class Loader
{
	public static Node3D[] LoadTileModels()
	{
		const float gapScale = 2f / Visual.Models.Root3;
		MeshInstance3D tile = new()
		{
			Mesh = new CylinderMesh()
			{
				Height = 0.2f,
				TopRadius = 0.49f * gapScale,
				BottomRadius = 0.49f * gapScale,
				Material = new StandardMaterial3D()
				{
					AlbedoColor = new(r: 0.8f, g: 0.8f, b: 0.4f),
				},
				RadialSegments = 6,
			}
		};
		tile.RotateY(Mathf.DegToRad(30f));
		return [tile];
	}

	public static Node3D[] LoadSquadModels()
	{
		var scene = GD.Load<PackedScene>("scenes/squads/paper.tscn");
		Node3D model = scene.Instantiate<Node3D>();
		return [model];
	}

	public static Logic.Entities.Tiles.Hex[] LoadTiles()
	{
		return
		[
			new() {
				CanAmbushOrder = [],
				Capabilities = new(),
			}
		];
	}

	public static Logic.Entities.Squads.Definition[] LoadSquads()
	{
		return
		[
			new() {
				Stats = new()
				{
					Ammo = 12,
					Armor = 12,
					Army = Logic.Entities.Squads.Army.USSR,
					Cost = 12,
					Count = 12,
					Durability = new()
					{
						Loss = 1,
						Rolls = 12,
						Value = 12,
					},
					Type = Logic.Entities.Squads.Type.Trooper
				}
			}
		];
	}
}
