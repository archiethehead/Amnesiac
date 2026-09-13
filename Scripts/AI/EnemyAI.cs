using Godot;
using System;

public partial class EnemyAI : PathfindingAI {

    protected GameManager GameManager = null;
    [Export] private Area3D DetectionCollider = null;

    protected override float Speed {

        get {

            if (CurrentState == EnemyState.Seeking) 
                return SeekSpeed;
            
            return ChaseSpeed;
        
        }
    
    }
    [Export] protected float SeekSpeed = 3.0f;
    [Export] protected float ChaseSpeed = 3.5f;


    [Export] protected float AttackCooldownThreshold {

        get => AttackCooldown.Threshold;
        set => AttackCooldown.Threshold = value;
    }
    private Countdown AttackCooldown = new Countdown();

    [Export] protected float PlayerLostThreshold {

        get => PlayerLostCountdown.Threshold;
        set => PlayerLostCountdown.Threshold = value;
    }
    private Countdown PlayerLostCountdown = new Countdown();

    protected enum EnemyState { 
    
        Seeking,
        Chasing,
        Cooldown
    
    }
    protected EnemyState CurrentState {

        get => StateBuffer;
        
        set {

            if (value == EnemyState.Chasing)
                StuckCountdown.Threshold = ChasingStuckTimerThreshold;

            else
                StuckCountdown.Threshold = StuckTimerThreshold;

            PreviousState = StateBuffer;
            StateBuffer = value;
        
        }

    }
    protected EnemyState StateBuffer = EnemyState.Seeking;
    protected EnemyState PreviousState = EnemyState.Seeking;
    
    protected bool PlayerLost {

        get => PlayerLostBuffer;

        set {

            if (!value)
                PlayerLostCountdown.Reset();

            PlayerLostBuffer = value;
        
        }
    
    }
    private bool PlayerLostBuffer = true;

    protected bool IsStuck {

        get {

            return IsOnWall() && IsOnFloor();

        }

    }

    [Export] protected float ChasingStuckTimerThreshold = 5.0f;
    [Export] protected float StuckTimerThreshold = 1.0f;
    private Countdown StuckCountdown = new Countdown(1.0f);

    private float Stuck = 0.0f;


    public override void _Ready() {

        GameManager = GameManager.Instance;

    }

    public override void _PhysicsProcess(double delta) {

        GameManager.DebugOut("Empty Stuck Timer: {0}", StuckCountdown.ExposedTimer);

        if (IsStuck) {

            if (StuckCountdown.LogTime(delta)) {

                CurrentState = EnemyState.Seeking;
                SetRandomTargetLocation();

            }

        }

        else {

            StuckCountdown.Reset();

        }

        if (PlayerLost && CurrentState == EnemyState.Chasing) {


            if (PlayerLostCountdown.LogTime(delta)) {

                CurrentState = EnemyState.Seeking;
                SetRandomTargetLocation();

            }

        }

        if (CurrentState == EnemyState.Cooldown) {

            if (AttackCooldown.LogTime(delta))
                CurrentState = EnemyState.Chasing;

        }

        else
            Moving(delta);

        // Pathfinder process does all of the Velocity calculations
        // and MUST be called before the MoveAndSlide/Gravity handling
        // in the inherited physics process, in order for our Velocity
        // calculations to actually be applied.

        base._PhysicsProcess(delta);
    
    }

    private void OnBodyEntered(Node Body) {

        if (Body is Player) {

            PlayerLost = false;
            Target = (Node3D)Body;
            CurrentState = EnemyState.Chasing;

        }
    
    }

    private void OnBodyExited(Node Body) {

        if (Body is Player) {

            PlayerLost = true;

        }
    
    }

    public override void NavigationFinished() {

        switch (CurrentState) {

            case EnemyState.Seeking:
                SetRandomTargetLocation();
                break;

            case EnemyState.Chasing:
                Attack();
                break;
        
        }

    }

    protected virtual void Attack() { }

}