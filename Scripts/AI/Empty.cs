using Godot;
using System;

public partial class Empty : EnemyAI {

    private float Damage = 25.0f;

    public override void _Ready() {

        base._Ready();
        SetRandomTargetLocation();


    }

    protected override void Attack() {

        Vector3 velocity = Velocity;
        velocity.X = 0.0f;
        velocity.Z = 0.0f;
        Velocity = velocity;

        if (Target is Hitable h) {

            h.Hit(Damage);

            if (h.IsDestroyed) {

                CurrentState = EnemyState.Seeking;
                return;
            
            } 

        }

    }
    
}