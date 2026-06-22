using Godot;
using System;

public partial class GameManager : Node3D {

    Player Player = null;
    Reactor Reactor = null;
    DirectionalLight3D Sun = null;

    public override void _Ready() {

        Player = GetNode<Player>("Player");
        Reactor = GetNode<Reactor>("Reactor");
        Sun = GetNode<DirectionalLight3D>("Sun");

    }

    public override void _Process(double delta) {

        Sun.RotateX(0.1f * (float)delta);

    }

    public void ReactorExplode() {

        Reactor.ReactorExplode();
    
    }

}