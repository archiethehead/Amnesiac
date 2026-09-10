using Godot;
using System;

public partial class Empty : EnemyAI {

    public override void _Ready() {

        base._Ready();
        SetRandomTargetLocation();
        _PathfinderState = PathfinderState.Moving;

    }

    public override void _Process(double delta) {

        base._Process(delta);
        GameManager.DebugOut("Empty Velocity: {0}", Velocity);

    }
    
}