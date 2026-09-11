using Godot;
using System;
using System.Threading.Tasks;

public partial class PathfindingAI : CharacterBody3D {

    [Export(PropertyHint.None, "suffix:m")] private float RandomPathDistance = 5.0f;

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
    [Export] protected Node3D TempTarget = null;
    [Export] protected Node3D Target = null;
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

    protected virtual void Idle() { }

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

    protected void Moving() {

        Navigator.TargetPosition = Target.GlobalTransform.Origin;
        Vector3 CurrentPosition = this.GlobalTransform.Origin;
        Vector3 NextPosition = Navigator.GetNextPathPosition();
        Vector3 Direction = (NextPosition - CurrentPosition).Normalized();
        Vector3 NewVelocity = Direction * Speed;
        Navigator.Velocity = NewVelocity;

    }

    protected virtual void TakeAction() { }

    protected virtual void SetRandomTargetLocation() {

        TempTarget.Reparent(this);
        Vector3 RandomPoint = NavigationServer3D.MapGetRandomPoint(GetWorld3D().NavigationMap, 1, true);
        TempTarget.GlobalPosition = RandomPoint; 
        Target = TempTarget;
        TempTarget.Reparent(GameManager.Instance);
        GameManager.Instance.DebugOut("Random Point: {0}", RandomPoint);

    }

    public virtual void NavigationFinished() {

        TakeAction();
        _PathfinderState = PathfinderState.Cooldown;

    }

    public void NavigatorVelocitySet(Vector3 SafeVelocity) {

        if (IsOnFloor()) {

            Velocity = Navigator.Velocity.MoveToward(SafeVelocity, 0.25f);

        }
    
    }

}