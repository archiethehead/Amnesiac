using Godot;
using System;

public partial class Empty : EnemyAI {

    public override void _Ready() {

        base._Ready();
        SetRandomTargetLocation();
        _PathfinderState = PathfinderState.Moving;

    }
    
}