using Godot;
using System;

public partial class Empty : EnemyAI {

    private float Damage = 25.0f;

    [Export] private RayCast3D HitCast = null;

    public override void _Ready() {

        base._Ready();
        SetRandomTargetLocation();

    }

    protected override void Attack() {

        HitCast.TargetPosition = HitCast.ToLocal(Target.GlobalPosition);

        Vector3 velocity = Velocity;
        velocity.X = 0.0f;
        velocity.Z = 0.0f;
        Navigator.Velocity = velocity;

        HitCast.ForceRaycastUpdate();

        if (Target is Hitable h && (HitCast.IsColliding() && HitCast.GetCollider() == Target)) {

            h.Hit(Damage);

            if (h.IsDestroyed) {

                CurrentState = EnemyState.Seeking;
                return;
            
            } 

        }

        CurrentState = EnemyState.Cooldown;

    }
    
}