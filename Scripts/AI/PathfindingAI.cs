using Godot;
using System;

public partial class PathfindingAI : CharacterBody3D {

    protected virtual float Speed { get; } = 3.0f;
    protected virtual float JumpVelocity { get; set; } = 6.0f;
    protected virtual float ActionCooldownTimer { get; set; } = 1.5f;
    private float ActionTimer = 0.0f;

    protected enum PathfinderState {

        Idle,
        Cooldown,
        WaitingToMove,
        Moving

    }

    protected PathfinderState _PathfinderState = PathfinderState.Idle;
    [Export] protected Node3D Target;
    [Export] protected NavigationAgent3D Navigator = null;


    public override void _PhysicsProcess(double delta) {

        if (!IsOnFloor()) {
            Velocity += GetGravity() * (float)delta;
        }

        MoveAndSlide();
    }

    protected void PathfinderProcess(double delta) {

        switch (_PathfinderState) {

            case PathfinderState.Idle:
                Idle();
                break;

            case PathfinderState.Cooldown:
                ActionCooldown();
                break;

            case PathfinderState.WaitingToMove:
                WaitingToMove((float)delta);
                break;

            case PathfinderState.Moving:
                Moving();
                break;


        }

    }

    protected virtual void Idle() {

        ActionCooldown();


    }

    private void WaitingToMove(float delta) {

        ActionTimer -= 1.0f * delta;

        if (ActionTimer <= 0.0f) {

            _PathfinderState = PathfinderState.Moving;

        }

    }

    private void ActionCooldown() {

        Velocity = Vector3.Zero;

        if (Target is not null) {

            ActionTimer = ActionCooldownTimer;
            _PathfinderState = PathfinderState.WaitingToMove;

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
        _PathfinderState = PathfinderState.Cooldown;

    }

}