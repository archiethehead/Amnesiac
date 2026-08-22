using Godot;
using System;

public partial class EnemyAI : PathfindingAI {

    [Export] protected Area3D DetectionCollider = null;

    protected override float Speed {

        get {

            switch (CurrentState) {

                case EnemyState.Seeking:
                    return SeekSpeed;

                default:
                    return ChaseSpeed;

            }
        
        }
    
    }

    protected virtual float SeekSpeed { get; } = 3.0f;
    protected virtual float ChaseSpeed { get; } = 5.25f;

    private enum EnemyState { 
    
        Seeking,
        Chasing,
        Attacking
    
    }
    private EnemyState CurrentState = EnemyState.Seeking;

    public override void _Process(double delta) {

        PathfinderProcess(delta);

    }

    protected override void Idle() { }

    private void OnBodyEntered(Node Body) {

        if (Body is Player) {

            Target = (Node3D)Body;
            CurrentState = EnemyState.Chasing;
            _PathfinderState = PathfinderState.Moving;

        }
    
    }

    protected override void TakeAction() {

        Attack();
    
    }

    protected virtual void Attack() { }

}