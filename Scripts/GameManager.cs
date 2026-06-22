using Godot;
using System;

public partial class GameManager : Node3D {

    [Export] Player Player = null;
    [Export] Reactor Reactor = null;
    [Export] DirectionalLight3D Sun = null;

    public override void _Process(double delta) {

        if (Sun != null) {

            Sun.RotateX(0.001f * (float)delta);

        }

    }

    public void ReactorExplode() {

        Reactor.ReactorExplode();
    
    }

}