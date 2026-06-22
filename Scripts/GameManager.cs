using Godot;
using System;

public partial class GameManager : Node3D {

	Player Player = null;
	DirectionalLight3D Sun = null;

    public override void _Ready() {

		Player = GetNode<Player>("Player");
		Sun = GetNode<DirectionalLight3D>("Sun");

    }

    public override void _Process(double delta) {

		Sun.RotateX(0.1f * (float)delta);

    }
}