using Godot;
using System;

public partial class GameManager : Node3D {

    public static GameManager Instance { get; private set; }
    public Player Player { get; set; } = null;
    public Reactor Reactor { get; set; } = null;

    public override void _Ready() {

        Instance = this;

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

    public void Pause() {

        GetTree().Paused = true;

    }

    public void Unpause() {

        GetTree().Paused = false;

    }

    public void Exit() {

        GetTree().Quit();

    }

}