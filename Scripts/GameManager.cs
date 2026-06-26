using Godot;
using System;

public partial class GameManager : Node3D {

    [Export] private Player Player = null;
    [Export] private Reactor Reactor = null;
    [Export] private DirectionalLight3D Sun = null;

    public override void _Process(double delta) {

        if (Sun != null) {

            Sun.RotateX(0.001f * (float)delta);

        }

    }

    public void ReactorExplode() {

        if (Reactor != null) {

            Reactor.ReactorExplode();
        
        }

    }

    public void FacePlayer(Node3D Object) {

        Object.LookAt(Player.GlobalTransform.Origin, Vector3.Up, true);
        Object.GlobalRotation = new Vector3(0.0f, Object.GlobalRotation.Y, Object.GlobalRotation.Z);

    }

}