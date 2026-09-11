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

    public override void _PhysicsProcess(double delta) {

        // Pathfinder process does all of the Velocity calculations
        // and MUST be called before the MoveAndSlide/Gravity handling
        // in the inherited physics process, in order for our Velocity
        // calculations to actually be applied.

        PathfinderProcess(delta);
        base._PhysicsProcess(delta);
    
    }

    private void OnBodyEntered(Node Body) {

        if (Body is Player) {

            Target = (Node3D)Body;
            CurrentState = EnemyState.Chasing;
            _PathfinderState = PathfinderState.Moving;

        }
    
    }

    public override void NavigationFinished() {

         if (CurrentState == EnemyState.Seeking) {

            SetRandomTargetLocation();
            return;
        
        }

        base.NavigationFinished();

    }

    protected override void TakeAction() {

        if (CurrentState == EnemyState.Attacking) Attack();

    }

    protected virtual void Attack() { }

}