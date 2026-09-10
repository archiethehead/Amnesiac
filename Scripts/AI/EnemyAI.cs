using Godot;
using System;

public partial class EnemyAI : PathfindingAI {

    protected GameManager GameManager = null;
    [Export] private Area3D DetectionCollider = null;

    protected override float Speed {

        get {

            if (CurrentState == EnemyState.Seeking) return SeekSpeed;
            return ChaseSpeed;
        
        }
    
    }

    [Export] protected virtual float SeekSpeed { get; set; } = 3.0f;
    [Export] protected virtual float ChaseSpeed { get; set; } = 5.25f;

    protected enum EnemyState { 
    
        Seeking,
        Chasing,
        Attacking
    
    }
    protected EnemyState CurrentState = EnemyState.Seeking;

    public override void _Ready() {

        GameManager = GameManager.Instance;

    }

    public override void _Process(double delta) {

        PathfinderProcess(delta);

    }

    private void OnBodyEntered(Node Body) {

        if (Body is Player) {

            Navigator.PathDesiredDistance = 1.0f;
            Target = (Node3D)Body;
            CurrentState = EnemyState.Chasing;
            _PathfinderState = PathfinderState.Moving;

        }
    
    }

    public override void TargetReached() {

        if (CurrentState == EnemyState.Seeking) {

            SetRandomTargetLocation();
            return;
        
        }

        base.TargetReached();

    }

    protected override void TakeAction() {

        if (CurrentState == EnemyState.Attacking) Attack();

    }

    protected virtual void Attack() { }

}