using Godot;
using System;

public partial class PathfindingAI : CharacterBody3D {

    protected virtual float Speed { get; set; } = 3.0f;
    protected virtual float JumpVelocity { get; set; } = 6.0f;
    protected virtual float ActionCooldownTimer { get; set; } = 1.5f;
    private float ActionTimer = 0.0f;

    protected enum EnemyState {

        Idle,
        Cooldown,
        WaitingToMove,
        Moving

    }

    protected EnemyState State = EnemyState.Idle;
    [Export] protected Node3D Target;
    [Export] protected NavigationAgent3D Navigator = null;


    public override void _PhysicsProcess(double delta) {

        switch (State) {

            case EnemyState.Idle:
                Idle();
                break;

            case EnemyState.Cooldown:
                ActionCooldown();
                break;

            case EnemyState.WaitingToMove:
                WaitingToMove((float)delta);
                break;

            case EnemyState.Moving:
                Moving();
                break;


        }


        if (!IsOnFloor()) {
            Velocity += GetGravity() * (float)delta;
        }


        MoveAndSlide();
    }

    private void Idle() {

        ActionCooldown();


    }

    private void WaitingToMove(float delta) {

        ActionTimer -= 1.0f * delta;

        if (ActionTimer <= 0.0f) {

            State = EnemyState.Moving;

        }

    }

    private void ActionCooldown() {

        Velocity = Vector3.Zero;

        if (Target is not null) {

            ActionTimer = ActionCooldownTimer;
            State = EnemyState.WaitingToMove;

        }

    }

    private void Moving() {

        if (IsOnWall() && IsOnFloor()) {

            Vector3 velocity = Velocity;
            velocity.Y = JumpVelocity;
            Velocity = velocity;

        }

        Navigator.TargetPosition = Target.GlobalTransform.Origin;
        Vector3 CurrentPosition = this.GlobalTransform.Origin;
        Vector3 NextPosition = Navigator.GetNextPathPosition();
        Vector3 Direction = (NextPosition - CurrentPosition).Normalized();
        Velocity = Direction * Speed;


    }

    protected virtual void TakeAction() { }

    public void TargetReached() {

        TakeAction();
        State = EnemyState.Cooldown;

    }

}