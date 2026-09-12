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
    [Export] protected virtual float ChaseSpeed { get; set; } = 2.25f;

    protected virtual float AttackCooldown { get; } = 1.0f;
    private float AttackCooldownTimer = 0.0f;

    protected virtual float PlayerLostThreshold { get; } = 1.0f;
    private float PlayerLostTimer = 0.0f;

    protected enum EnemyState { 
    
        Seeking,
        Chasing,
        Attacking,
        Cooldown
    
    }
    protected EnemyState CurrentState {

        get => StateBuffer;
        
        set {

            PreviousState = StateBuffer;
            StateBuffer = value;
        
        }

    }
    protected EnemyState StateBuffer = EnemyState.Seeking;
    protected EnemyState PreviousState = EnemyState.Seeking;
    protected bool PlayerLost = false;

    protected bool IsStuck {

        get {

            return IsOnWall() && IsOnFloor();

        }

    }
    private float StuckTimerThreshold {

        get {

            switch (CurrentState) {

                case EnemyState.Seeking:
                    return 1.0f;

                default:
                    return 5.0f;

            
            }
        
        }
    
    }
    
    private float StuckTimer = 0.0f;


    public override void _Ready() {

        GameManager = GameManager.Instance;

    }

    public override void _PhysicsProcess(double delta) {

        if (IsStuck && CurrentState != EnemyState.Attacking) {

            StuckTimer += (float)delta;

        }

        else {

            StuckTimer = 0.0f;

        }

        if (StuckTimer >= StuckTimerThreshold) {

            StuckTimer = 0.0f;
            CurrentState = EnemyState.Seeking;
            SetRandomTargetLocation();

        }

        if (CurrentState == EnemyState.Cooldown) {

            AttackCooldownTimer += 1.0f * (float)delta;

            if (AttackCooldownTimer >= AttackCooldown) {

                AttackCooldownTimer = 0.0f;
                CurrentState = EnemyState.Chasing;

            }

        }

        if (PlayerLost && CurrentState == EnemyState.Chasing) {

            PlayerLostTimer += 1.0f * (float)delta;

            if (PlayerLostTimer >= PlayerLostThreshold) {

                PlayerLostTimer = 0.0f;
                CurrentState = EnemyState.Seeking;
                SetRandomTargetLocation();

            }

        }

        // Pathfinder process does all of the Velocity calculations
        // and MUST be called before the MoveAndSlide/Gravity handling
        // in the inherited physics process, in order for our Velocity
        // calculations to actually be applied.

        PathfinderProcess(delta);
        base._PhysicsProcess(delta);
    
    }

    private void OnBodyEntered(Node Body) {

        if (Body is Player) {

            PlayerLost = false;
            Target = (Node3D)Body;
            CurrentState = EnemyState.Chasing;
            _PathfinderState = PathfinderState.Moving;

        }
    
    }

    private void OnBodyExited(Node Body) {

        if (Body is Player) {

            PlayerLost = true;

        }
    
    }

    public override void NavigationFinished() {

        if (CurrentState == EnemyState.Seeking) {

            SetRandomTargetLocation();
            return;

        }

        else if (CurrentState == EnemyState.Chasing) {

            CurrentState = EnemyState.Attacking;
        
        }

        base.NavigationFinished();

    }

    protected override void TakeAction() {

        if (CurrentState == EnemyState.Attacking) Attack();
        CurrentState = EnemyState.Cooldown;

    }

    protected virtual void Attack() { }

}